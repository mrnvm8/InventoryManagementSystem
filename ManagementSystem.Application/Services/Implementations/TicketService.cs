using MailKit.Net.Smtp;
using MailKit.Security;
using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Helpers;
using ManagementSystem.Application.Helpers.Errors.Services;
using ManagementSystem.Application.Mappings;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Application.Services.Interface;
using ManagementSystem.Shared.Helpers;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System.Data;

namespace ManagementSystem.Application.Services.Implementations;

internal class TicketService(
     IUnitOfWork _unitOfWork,
     IEmployeeService _employeeService,
     IDeviceService _deviceService,
     IConfiguration _config,
     IHttpContextService _http,
     IAuthRepository _authRepository,
     IAuthService _authService,
     ILogger<TicketService> _logger
    ) : ITicketService
{
    #region Add new Ticket to the DB
    public async Task<Result<TicketDto>> AddTicket(TicketRequest request, CancellationToken token = default)
    {
        //get the user id
        var userId = _http.GetUserId();
        //map ticket request to Ticket model
        var ticket = request.CreateMapToTicket(userId);
        //checking if device issue ticket that is existing is available
        var existing = await _unitOfWork.Tickets.ExistTicketAsync(ticket);
        if (existing)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {new ServicesErrorMessages(request.DeviceId).Duplicate}");
            return Result<TicketDto>.Failure(ServicesErrorMessages.TicketExist);
        }

        await _unitOfWork.Tickets.CreateAsync(ticket, token);
        var added = await _unitOfWork.SaveChangesAsync(token);
        if (!added.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {added.Message}");
            return Result<TicketDto>.Failure(added.Message);
        }

        //get user by the user Id
        var user = await _authRepository.GetUserById(userId, token);
        if (user is null)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                $"Error Ticket Id: {new ServicesErrorMessages(userId).IdNotFound}");
            return Result<TicketDto>.Failure(new ServicesErrorMessages(userId).IdNotFound);
        }

        //send email address to the techservices@axiumeducation.org
        SendTicketEmail(ComposeEmail(user, ticket, true, false));
        var dbTicket = await GetTicketById(ticket.Id, token);
        return Result<TicketDto>.Success(dbTicket.Data, Messages.Added);
    }
    #endregion

    #region Get ticket by it's Id
    public async Task<Result<TicketDto>> GetTicketById(Guid ticketId, CancellationToken token = default)
    {
        //get the Tickect by it's Id record
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId, token);
        if (ticket is null)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error : {new ServicesErrorMessages(ticketId).IdNotFound}");
            return Result<TicketDto>.Failure(new ServicesErrorMessages(ticketId).IdNotFound);
        }
        return Result<TicketDto>.Success(ticket.MapTicketDto(), string.Empty);
    }
    #endregion

    #region Get all Tickets
    public async Task<Result<IEnumerable<TicketDto>>> GetTickets(CancellationToken token = default)
    {
        //get user role and user Id
        var role = _http.GetUserRole()!;
        var userId = _http.GetUserId();

        //get all the existing tickets
        var tickets = await _unitOfWork.Tickets.GetAllAsync(token);

        if (!tickets.Any())
        {
            return Result<IEnumerable<TicketDto>>.Failure(Messages.NoData);
        }

        if (!role.Equals(AuthConstants.AdminRole))
        {
            return Result<IEnumerable<TicketDto>>
                .Success(tickets.Where(u => u.UserId.Equals(userId)).Select(x => x.MapTicketDto()), string.Empty);
        }

        return Result<IEnumerable<TicketDto>>
            .Success(tickets.Select(x => x.MapTicketDto()), string.Empty);
    }
    #endregion

    #region Get all tickets by deviceId
    public async Task<Result<IEnumerable<TicketDto>>> GetTicketsOfDevice(Guid deviceId, CancellationToken token = default)
    {
        //get all the existing tickets
        var tickets = await _unitOfWork.Tickets.GetTicketsOfDeviceAsync(deviceId, token);
        if (!tickets.Any())
        {
            return Result<IEnumerable<TicketDto>>.Failure(Messages.NoData);
        }

        return Result<IEnumerable<TicketDto>>
            .Success(tickets.Select(x => x.MapTicketDto()), string.Empty);
    }
    #endregion
    #region Ticket being attended
    public async Task<Result<TicketDto>> TicketAcknowledge(TicketRequest request, Guid ticketId, CancellationToken token = default)
    {
        //get the Tickect by it's Id record
        var dbTicket = await _unitOfWork.Tickets.GetByIdAsync(ticketId, token);
        if (dbTicket is null)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error Ticket Id: {new ServicesErrorMessages(ticketId).IdNotFound}");
            return Result<TicketDto>.Failure(new ServicesErrorMessages(ticketId).IdNotFound);
        }

        //map to ticket model
        var ticket = request.MapToTicket(ticketId, dbTicket);
        //Tech member acknowledging the ticket
        await _unitOfWork.Tickets.UpdateAsync(ticket, token);
        var updated = await _unitOfWork.SaveChangesAsync(token);
        //checking if it was successfuly
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error Ticket Id: {updated.Message}");
            return Result<TicketDto>.Failure(updated.Message);
        }

        var updatedTicket = await GetTicketById(ticketId, token);
        return Result<TicketDto>.Success(updatedTicket.Data, Messages.Updated);
    }
    #endregion

    #region Ticket issuse resolved
    public async Task<Result<TicketDto>> TicketArchived(TicketRequest request, Guid ticketId, CancellationToken token = default)
    {
        //get user Id 
        var userId = _http.GetUserId();

        //get the Tickect by it's Id record
        var dbTicket = await _unitOfWork.Tickets.GetByIdAsync(ticketId, token);
        if (dbTicket is null)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error Ticket Id: {new ServicesErrorMessages(ticketId).IdNotFound}");
            return Result<TicketDto>.Failure(new ServicesErrorMessages(ticketId).IdNotFound);
        }

        //map to ticket model
        var ticket = request.MapToTicket(ticketId, dbTicket);
        //Tech member acknowledging the ticket
        await _unitOfWork.Tickets.UpdateAsync(ticket);
        var updated = await _unitOfWork.SaveChangesAsync(token);
        //checking if it was successfuly
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error Ticket Id: {updated.Message}");
            return Result<TicketDto>.Failure(updated.Message);
        }

        //get user by the user Id
        var user = await _authRepository.GetUserById(userId, token);
        if (user is null)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                $"Error Ticket Id: {new ServicesErrorMessages(userId).IdNotFound}");
            return Result<TicketDto>.Failure(new ServicesErrorMessages(userId).IdNotFound);
        }
        //send email address to the techservices@axiumeducation.org
        SendTicketEmail(ComposeEmail(user, ticket, false, true));

        var updatedTicket = await GetTicketById(ticketId, token);
        return Result<TicketDto>.Success(updatedTicket.Data, Messages.Updated);
    }
    #endregion

    #region Delete ticket
    public async Task<Result<bool>> DeleteTicket(Guid ticketId, CancellationToken token = default)
    {
        //get the Tickect by it's Id record
        var dbTicket = await _unitOfWork.Tickets.GetByIdAsync(ticketId, token);
        if (dbTicket is null)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error : {new ServicesErrorMessages(ticketId).IdNotFound}");
            return Result<bool>.Failure(new ServicesErrorMessages(ticketId).IdNotFound);
        }
        await _unitOfWork.Tickets.DeleteAsync(dbTicket, token);
        var deleted = await _unitOfWork.SaveChangesAsync(token);
        if (!deleted.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(TicketService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                $"Error Ticket Id: {deleted.Message}");
            return Result<bool>.Failure(deleted.Message);
        }
        return Result<bool>.Success(deleted.IsSuccess, Messages.Deleted);
    }
    #endregion

    #region Compossing the email Body
    private EmailResponse ComposeEmail(AppUser? user, Ticket ticket, bool? create, bool? archieve)
    {
        var EmailContext = new EmailResponse();
        var employee = _employeeService.GetByIdAsync(user!.EmployeeId).Result.Data;
        var device = _deviceService.GetByIdAsync(ticket.DeviceId).Result.Data;

        if (create.Equals(true))
        {

            if (employee is not null && device is not null)
            {
                EmailContext.Subject = ticket.TicketTitle!;
                EmailContext.Body = $"<p>Good day Tech Team</p> <br/>" +
                                    $"<p><u>I have created a Ticket about work device :</u><br/>" +
                                    $"<b>Device Name:</b> {device.DeviceName}<br/>" +
                                    $"<b>Identity Number:</b> {device.IdentityNumber}</p><br/>" +
                                    $"<p><u>Here is the Issue:</u><br/><br/><b>{ticket.TicketIssue}</b></p>" +
                                    $"<p><b>Best regards</b><br/>{employee!.FullName}</p>";

                EmailContext.Email = "techservices@axiumeducation.org";

                return EmailContext;
            }
        }
        else if (archieve.Equals(true))
        {
            var UserCreator = _authService.GetUserById(ticket.UserId).Result.Data!;

            EmailContext.Subject = ticket.TicketTitle!;
            EmailContext.Body = $"<p> Good Day {UserCreator.EmployeeName}</p> " +
                                $"<p>I have looked at the your ticket with title: <b>{ticket.TicketTitle}</b></p>" +
                                $"<p><u>The device issue that was submitted is :</u><br/><br/><b>{ticket.TicketIssue}</b></p>" +
                                $"<hr />" +
                                $"<p><u>Here are my findings and solution to the problem:</u><br/><br/>" +
                                $"<b>{ticket.TicketSolution}</b></p>" +
                                $"<p><b>Best regards</b><br />{employee!.FullName}</p>";

            EmailContext.Email = UserCreator.UserName;

            return EmailContext;
        }

        return EmailContext;


    }
    #endregion

    #region Sending Email about the Ticket 
    private void SendTicketEmail(EmailResponse request)
    {
        var _settings = _config.GetSection(nameof(EmailSettings))
                        .Get<EmailSettings>()!;

        var email = new MimeMessage();
        //email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
        email.From.Add(MailboxAddress.Parse(_settings.EmailUsername));
        email.To.Add(MailboxAddress.Parse(request.Email));
        email.Subject = request.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

        try
        {
            using var smtp = new SmtpClient();
            //smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Connect(_settings.EmailHost, 587, SecureSocketOptions.StartTls);
            //smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Authenticate(_settings.EmailUsername, _settings.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);

            // Log success
            _logger.LogInformation("Email sent successfully to {Email}", request.Email);
        }
        catch (Exception ex)
        {
            // Log failure
            _logger.LogError(ex, "Failed to send email to {Email}", request.Email);

            // Optionally, rethrow the exception or handle it as needed
            throw;
        }
    }

    #endregion
}

using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementSystem.Web.Controllers;

public class TicketsController(
    ITicketService _ticketService,
    IDeviceService _deviceService) : BaseController
{
    public async Task<IActionResult> Index(CancellationToken token = default)
    {
        var result = await _ticketService.GetTickets(token);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
            return View(result.Data);
        }
        return View(result.Data);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }
        var result = await _ticketService.GetTicketById(id);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }
    public ActionResult Create(Guid id)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        var devices = _deviceService.GetAllAsync().Result.Data;
        ViewData["DeviceId"] = devices!.Where(d => d.Id.Equals(id))
                                        .Select(d => new SelectListItem
                                        {
                                            Value = d.Id.ToString(),
                                            Text = d.DeviceName,
                                        });
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid id, [FromForm] TicketRequest request, CancellationToken token = default)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        if (ModelState.IsValid)
        {
            var result = await _ticketService.AddTicket(request, token);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }
            return RedirectToAction("Index", "Tickets");
        }
        return View(request);
    }

    public async Task<IActionResult> AcknowledgeTicket(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        var result = await _ticketService.GetTicketById(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcknowledgeTicket(Guid id, [FromForm] TicketRequest request,
        CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        var result = await _ticketService.TicketAcknowledge(request, id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> ArchiveTicket(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        var result = await _ticketService.GetTicketById(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ArchiveTicket(Guid id, [FromForm] TicketRequest request, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        var result = await _ticketService.TicketArchived(request, id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }
        return RedirectToAction(nameof(Index));
    }


    //Tickets/delete/{id}
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }

        var result = await _ticketService.GetTicketById(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }
        return View(result.Data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }
        var result = await _ticketService.DeleteTicket(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Ticket"));
            return View();
        }
        return RedirectToAction(nameof(Index));
    }
}

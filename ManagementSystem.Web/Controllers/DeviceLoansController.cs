using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementSystem.Web.Controllers;

[Authorize]
public class DeviceLoansController(
    IDeviceLoanService _deviceLoanService,
    IDeviceService _deviceService,
    IEmployeeService _employeeService,
    ITicketService _ticketService) : BaseController
{
   
    [ActionName("Details")]
    public async Task<IActionResult> DeviceDetailsAndHistory(Guid id, CancellationToken token)
    {

        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }

        var deviceResult = await _deviceService.GetByIdAsync(id, token);
        ViewData["Device"] = deviceResult.Data;

        var ticketResult = await _ticketService.GetTicketsOfDevice(id, token);
        if (ticketResult.IsSuccess)
        {
            ViewBag.Ticket = ticketResult.Data;
        }

        var LoanResult = await _deviceLoanService.GetAllDeviceLoansById(id, token);
        if (!LoanResult.IsSuccess)
        {
            ViewBag.Error = LoanResult.Message;
            return View();
        }
        return View(LoanResult.Data);
    }

    // GET: DeviceLoansController/Create
    public IActionResult Assign(Guid id, Guid departId, Guid typeId, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) ||
            departId == Guid.Empty || 
            typeId == Guid.Empty || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }
        //do not mistake the id as Loan Id this id is Device Id
        LoanFormSelection(id, token, Guid.Empty);

        return View();
    }

    // POST: DeviceLoansController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(Guid id, Guid departId, Guid typeId,
        [FromForm] LoanRequest request, CancellationToken token = default)
    {

        if (!Guid.TryParse(id.ToString(), out _) ||
           departId == Guid.Empty ||
           typeId == Guid.Empty || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }
        var result = await _deviceLoanService.AssignDevice(request, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }

        return RedirectToAction("Index", "Devices", new { Id = departId, typeId = typeId });
    }

    // GET: DeviceLoansController/Edit/5
    public ActionResult Unassign(Guid id, Guid departId, Guid typeId, CancellationToken token = default)
    {
        if (!Guid.TryParse(id.ToString(), out _) ||
           departId == Guid.Empty ||
           typeId == Guid.Empty || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }
        //do not mistake this id paramenter is DeviceId in DeviceLoan table
        var result = _deviceLoanService.GetLoanByDeviceId(id, token).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }

        LoanFormSelection(id, token, result.Data!.EmployeeId);

        return View(result.Data);
    }

    // POST: DeviceLoansController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unassign(Guid id, Guid departId, Guid typeId, 
        [FromForm] LoanRequest request, CancellationToken token = default)
    {

        if (!Guid.TryParse(id.ToString(), out _) ||
           departId == Guid.Empty ||
           typeId == Guid.Empty || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }
        //do not mistake this id paramenter is DeviceId in DeviceLoan table
        var result = await _deviceLoanService.UnassignedDevice(id, request, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction("Index", "Devices", new { Id = departId, typeId = typeId });
    }

    #region Form selection item
    private void LoanFormSelection(Guid id, CancellationToken token, Guid employeeId)
    {
        var devices = _deviceService.GetAllAsync(token).Result.Data;
        var employees = _employeeService.GetAllAsync(token).Result.Data;

        ViewData["DeviceId"] = devices!.Where(d => d.Id.Equals(id)).Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.DeviceName,
        });

        if (employeeId == Guid.Empty)
        {
            ViewData["EmployeeId"] = employees!.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.FullName
            });
        }
        else
        {
            ViewData["EmployeeId"] = employees!.Where(e => e.Id.Equals(employeeId))!.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.FullName
            });
        }

    }
    #endregion
}

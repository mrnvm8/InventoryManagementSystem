using ManagementSystem.Application.Helpers;
using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementSystem.Web.Controllers;

[Authorize(AuthenticationConstants.EveryUserPolicyName)]
public class DevicesController(
        IDeviceService _deviceService,
        IDepartmentService _departmentService,
        IDeviceTypeService _deviceTypeService,
        IHttpContextService _http)
        : BaseController
{


    //get devices by device type and department
    public async Task<IActionResult> Index(Guid Id, Guid typeId, CancellationToken token)
    {

        if (!Guid.TryParse(Id.ToString(), out _) ||
            !Guid.TryParse(typeId.ToString(), out _)
            || Id == Guid.Empty && typeId == Guid.Empty)
        {
            return RedirectToAction("DeviceSummary", "Devices");
        }
        var result = await _deviceService.GetDevicesByDepartmentAndDeviceType(Id, typeId, token);
        return View(result.Data);

    }

    //GET Device/DeviceSummary
    public async Task<IActionResult> DeviceSummary(CancellationToken token)
    {
        var result = await _deviceService.Summary(token);

        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
        }
        return View(result.Data);
    }

    //Get all Devices
    public async Task<IActionResult> AllDevices(CancellationToken token)
    {
        var result = await _deviceService.GetAllAsync(token);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
        }
        return View(result.Data);
    }

    // GET: Devices/Create
    public IActionResult Create(CancellationToken token)
    {
        var departmentId = _http.GetDepartmentIdentifier();
        var role = _http.GetUserRole();
        DeviceFormSelectItems(departmentId, role, token);
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] DeviceRequest request, CancellationToken token)
    {
        // Your logic to handle the form submission
        if (string.IsNullOrEmpty(request.SerialNo) && string.IsNullOrEmpty(request.IMEINo))
        {
            ModelState.AddModelError(string.Empty, "Please fill in either Serial Number or IMEI Number.");
            return View();
        }

        if (ModelState.IsValid)
        {
            var result = await _deviceService.CreateAsync(request, token);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }

            return RedirectToAction("Index", "Devices",
                new { id = result.Data!.DepartId, typeId = result.Data.TypeId });
        }

        return View();
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }

        var result = await _deviceService.GetByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }

        var departmentId = _http.GetDepartmentIdentifier();
        var role = _http.GetUserRole();

        DeviceFormSelectItems(departmentId, role, token);

        return View(result.Data);
    }

    // POST: Devices/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] DeviceRequest request, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }

        // Your logic to handle the form submission
        if (string.IsNullOrEmpty(request.SerialNo) && string.IsNullOrEmpty(request.IMEINo))
        {
            ModelState.AddModelError(string.Empty, "Please fill in either Serial Number or IMEI Number.");
            return View();
        }


        if (ModelState.IsValid)
        {
            var result = await _deviceService.UpdateAsync(request, id, token);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }
            return RedirectToAction("Index", "Devices",
                    new { id = result.Data!.DepartId, typeId = result.Data.TypeId });
        }

        ModelState.AddModelError(string.Empty, ControllersErrors.Resubmit);
        return View(request);
    }

    // GET: Devices/Delete/5
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }

        var result = await _deviceService.GetByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }

    // POST: Devices/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Device"));
            return View();
        }

        var result = await _deviceService.DeleteByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }

    #region Form selection items
    private void DeviceFormSelectItems(Guid departmentId, string? role, CancellationToken token)
    {
        var Departments = _departmentService.GetAllAsync(token).Result.Data;
        var DeviceTypes = _deviceTypeService.GetAllAsync(token).Result.Data;
        if (role!.Equals(AuthConstants.AdminRole))
        {

            ViewData["DepartmentId"] = Departments!.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.DepartmentOfficeName} => {d.DepartmentName}"
            });
        }
        else
        {
            ViewData["DepartmentId"] = Departments!
                .Where(d => d.Id.Equals(departmentId)).Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.DepartmentOfficeName} => {d.DepartmentName}"
                });
        }

        ViewData["DeviceTypeId"] = DeviceTypes!.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"{d.Name}"
        });
    }
    #endregion
}
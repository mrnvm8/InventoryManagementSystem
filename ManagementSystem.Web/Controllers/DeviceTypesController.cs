using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Web.Controllers;

[Authorize(AuthenticationConstants.AdminPolicyName)]
public class DeviceTypesController(IDeviceTypeService _deviceTypeService) : BaseController
{
    // GET: All device type records
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var result = await _deviceTypeService.GetAllAsync(token);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
            return View();
        }
        return View(result.Data);
    }


    // GET: Adding record to DB
    public ActionResult Create()
    {
        return View();
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] DeviceTypeRequest request, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, ControllersErrors.Resubmit);
            return View(request);
        }

        var result = await _deviceTypeService.CreateAsync(request, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }

    // GET:Edit/5
    public ActionResult Edit(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("DeviceType"));
            return View();
        }
        //get device type id
        var result = _deviceTypeService.GetByIdAsync(id, token).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Message);
            return View("Error");
        }

        return View(result.Data);
    }

    // POST: Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] DeviceTypeRequest request, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("DeviceType"));
            return View();
        }

        var result = await _deviceTypeService.UpdateAsync(request, id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Delete/5
    public ActionResult Delete(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("DeviceType"));
            return View();
        }

        var result = _deviceTypeService.GetByIdAsync(id, token).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Message);
            return View("Error");
        }
        return View(result.Data);
    }

    // POST: Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("DeviceType"));
            return View();
        }

        var result = await _deviceTypeService.DeleteByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
            return View();
        }
        return RedirectToAction(nameof(Index));
    }
}

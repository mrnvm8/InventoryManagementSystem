using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Web.Controllers;

[Authorize(AuthenticationConstants.AdminPolicyName)]
public class OfficesController(IOfficeService _officeService) : BaseController
{
    // GET: All offices records
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var result = await _officeService.GetAllAsync(token);
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

    // POST: OfficesContoller/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] OfficeRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _officeService.CreateAsync(request);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError(string.Empty, ControllersErrors.Resubmit);
        return View(request);
    }

    // GET: OfficesContoller/Edit/5
    public ActionResult Edit(Guid id)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Office"));
            return View();
        }
        
        //get office b8 id
        var result = _officeService.GetByIdAsync(id).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }

        return View(result.Data);
    }

    // POST: OfficesContoller/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] OfficeRequest request, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Office"));
            return View();
        }

        if (ModelState.IsValid)
        {
            var result = await _officeService.UpdateAsync(request, id, token);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError(string.Empty, ControllersErrors.Resubmit);
        return View(request);
    }

    // GET: OfficesContoller/Delete/5
    public ActionResult Delete(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Office"));
            return View();
        }

        var result = _officeService.GetByIdAsync(id, token).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }

    // POST: OfficesContoller/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Office"));
            return View();
        }

        var result = await _officeService.DeleteByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }
}

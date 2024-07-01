using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementSystem.Web.Controllers;


[Authorize(AuthenticationConstants.AdminPolicyName)]
public class DepartmentsController(
        IDepartmentService _departmentService, IOfficeService _officeService) : BaseController
{

    // GET: get all department record
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var result = await _departmentService.GetAllAsync(token);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
            return View();
        }
        return View(result.Data);
    }

    // GET: adding department record
    public IActionResult Create(CancellationToken token)
    {

        //getting all office record 
        var offices = _officeService.GetAllAsync(token).Result;
        ViewData["OfficeId"] = offices.Data!.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Name}"
        });
        return View();
    }

    // POST: DepartmentsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] DepartmentRequest request, CancellationToken token)
    {
        if (ModelState.IsValid)
        {
            var result = await _departmentService.CreateAsync(request, token);
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
        

    // GET: Editing record fom database
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Department"));
            return View();
        }
        //get the department record
        var result = await _departmentService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        //get all office record
        var offices = _officeService.GetAllAsync(token).Result;
        ViewData["OfficeId"] = offices.Data!.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Name}"
        });

        return View(result.Data);
    }

    // POST: Departments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] DepartmentRequest request, CancellationToken token)
    {

        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Department"));
            return View();
        }

        if (ModelState.IsValid)
        {
            var result = await _departmentService.UpdateAsync(request, id, token);
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

    // GET: Deleting the record
    public ActionResult Delete(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Department"));
            return View();
        }

        var result = _departmentService.GetByIdAsync(id).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }

    // POST: Departments/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Department"));
            return View();
        }
        var result = await _departmentService.DeleteByIdAsync(id);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }
}
using ManagementSystem.Web.Helpers;
using ManagementSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ManagementSystem.Web.Controllers;


[Authorize(AuthenticationConstants.AdminPolicyName)]
public class EmployeesController(
    IEmployeeService _employeeService, 
    IDepartmentService _departmentService) : BaseController
{
    // GET: get all employees record
    public async Task<IActionResult> Index()
    {
        var result = await _employeeService.GetAllAsync();
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
            return View();
        }
        return View(result.Data);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Employee"));
            return View();
        }
        var result = await _employeeService.GetAllLoanDevicesForEmployee(id, token);
        if (!result.IsSuccess)
        {
            ViewBag.Error = result.Message;
            return View();
        }
        return View(result.Data);
    }

    // GET: Adding employee to the DB
    public ActionResult Create(CancellationToken token)
    {
        //get the departments
        var departments = _departmentService.GetAllAsync(token).Result;
        ViewData["DepartmentId"] = departments.Data!.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"{d.DepartmentOfficeName} => {d.DepartmentName}"
        });
        return View();
    }

    // POST: EmployeesController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] EmployeeRequest request, CancellationToken token)
    {
        if (ModelState.IsValid)
        {
            var result = await _employeeService.CreateAsync(request, token);
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

    // GET: Editing employee record
    public ActionResult Edit(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Employee"));
            return View();
        }

        var result = _employeeService.GetByIdAsync(id, token).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }

        var Departments = _departmentService.GetAllAsync(token).Result;

        ViewData["DepartmentId"] = Departments.Data!.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"{d.DepartmentOfficeName} => {d.DepartmentName}"
        });

        return View(result.Data);
    }

    // POST: EmployeesController1/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] EmployeeRequest request, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Employee"));
            return View();
        }

        if (ModelState.IsValid)
        {
            var result = await _employeeService.UpdateAsync(request, id, token);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }
            return RedirectToAction(nameof(Index));
        }
        return View("Error", new ErrorViewModel { Message = ControllersErrors.Resubmit });

    }

    // GET: deleting employee record
    public ActionResult Delete(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Employee"));
            return View();
        }

        var result = _employeeService.GetByIdAsync(id, token).Result;
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return View(result.Data);
    }

    // POST: EmployeesController1/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken token)
    {
        if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
        {
            ModelState.AddModelError("", new ControllersErrors().InvalidID("Employee"));
            return View();
        }

        var result = await _employeeService.DeleteByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Error");
        }
        return RedirectToAction(nameof(Index));
    }
}

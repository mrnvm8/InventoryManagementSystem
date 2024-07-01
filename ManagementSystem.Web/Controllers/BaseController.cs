using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManagementSystem.Web.Controllers;

public abstract class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        // Retrieve the current action and controller names
        string currentAction = ControllerContext.ActionDescriptor.ActionName;
        string currentController = ControllerContext.ActionDescriptor.ControllerName;

        // Store them in ViewBag to pass to the view
        ViewBag.CurrentAction = currentAction;
        ViewBag.CurrentController = currentController;

    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SessionCheckFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Kiểm tra xem Session "User" đã tồn tại chưa
        var userSession = context.HttpContext.Session.GetString("User");
        if (string.IsNullOrEmpty(userSession))
        {
            // Nếu chưa đăng nhập, bắt quay về trang Login
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        base.OnActionExecuting(context);
    }
}
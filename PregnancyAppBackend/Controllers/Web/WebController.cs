using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PregnancyAppBackend.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize]
public class WebController : ControllerBase
{
    protected string GetErrorListFromModelState(ModelStateDictionary modelState)
    {
        var query = from state in modelState.Values
                    from error in state.Errors
                    select error.ErrorMessage;
        var errors = string.Join(" ", query);
        return errors;
    }
}
using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Misc.Core.Models.Api;

namespace Delsami.Plugin.Misc.Core.Bases
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected virtual IActionResult SucceedApiResult()
        {
            return Ok(new ApiResult(true));
        }
    }
}
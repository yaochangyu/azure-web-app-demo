using AspNetCoreApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet(Name = "GetVersion")]
    public VersionInfo Get()
    {
        var env = HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        
        return new VersionInfo
        {
            Version = "1.0.1",
            BuildDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Environment = env.EnvironmentName
        };
    }
}

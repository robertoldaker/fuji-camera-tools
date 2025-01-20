using ImageViewer.server.Services;
using ImageViewer.shared;
using Microsoft.AspNetCore.Mvc;

namespace ImageViewer.server.Controllers;

[ApiController]
[Route("/Configuration")]
public class ConfigurationController : ControllerBase
{
    private Services.Config _config;
    
    public ConfigurationController(Services.Config config) {
        _config = config;
    }

    /// <summary>
    /// Return the config
    /// </summary>
    /// <returns></returns>
    [HttpGet("Config")]
    public Services.Config GetConfig() {
        return _config;
    }

    /// <summary>
    /// Updates config
    /// </summary>
    /// <param name="config"></param>
    [HttpPost("Config")]
    public IActionResult SetConfig([FromBody] Services.Config config) {    
        var errors = config.CheckModel();
        if ( errors.Count > 0 ) {
            return this.ModelErrors(errors);
        } else {
            config.Save();
            return Ok();
        }
    }
}
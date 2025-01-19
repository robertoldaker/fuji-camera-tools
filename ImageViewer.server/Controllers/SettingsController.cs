using ImageViewer.server.Services;
using ImageViewer.shared;
using Microsoft.AspNetCore.Mvc;

namespace ImageViewer.server.Controllers;

[ApiController]
[Route("/Settings")]
public class SettingsController : ControllerBase
{
    private Services.Config _config;
    
    public SettingsController(Services.Config config) {
        _config = config;
    }

    /// <summary>
    /// Return the config
    /// </summary>
    /// <returns></returns>
    [HttpGet("/Config")]
    public Services.Config GetConfig() {
        return _config;
    }

    /// <summary>
    /// Updates config
    /// </summary>
    /// <param name="config"></param>
    [HttpPost("/Config")]
    public void SetConfig([FromBody] Services.Config config) {
        config.Save();
    }
}
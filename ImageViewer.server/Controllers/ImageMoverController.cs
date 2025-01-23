using System.Runtime.CompilerServices;
using ImageViewer.server.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageViewer.server.Controllers;

[ApiController]
[Route("/ImageMover")]
public class ImageMoverController : ControllerBase
{
    private ImageMoverService _imageMoverService;
    public ImageMoverController(ImageMoverService service) {
        _imageMoverService = service;
    }   

    /// <summary>
    /// List files available for import
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    [HttpGet("ListFiles")]
    public IActionResult ListFiles() {
        try {
            return this.Ok(_imageMoverService.ListFiles());
        } catch( Exception e) {
            return this.StatusCode(422,e.Message);
        }
    }

    /// <summary>
    /// Imports the next file available
    /// </summary>
    /// <returns></returns> <summary>
    [HttpGet("MoveNextFile")]
    public IActionResult MoveNextFile() {
        try {
            return this.Ok(_imageMoverService.MoveNextFile());
        } catch( Exception e) {
            return this.StatusCode(422,e.Message);
        }
    }
}
using ImageViewer.server.Services;
using ImageViewer.shared;
using Microsoft.AspNetCore.Mvc;

namespace ImageViewer.server.Controllers;

[ApiController]
[Route("/ImageLibrary")]
public class ImageLibraryController : ControllerBase
{
    private readonly ILogger<ImageLibraryController> _logger;

    private readonly ImageLibraryService _imageLibrary;

    public ImageLibraryController(ILogger<ImageLibraryController> logger, ImageLibraryService imageLibrary)
    {
        _logger = logger;
        _imageLibrary = imageLibrary;
    }

    /// <summary>
    /// Gets all images by year and month
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="month">Month (1-12)</param>
    /// <returns></returns> <summary>
    [HttpGet("ImagesByDate")]
    public List<ImagesByDate> GetImagesByDate(int year, int month)
    {
        return _imageLibrary.GetImagesByDate(year, month);
    }

    /// <summary>
    /// Get a list of the available months sorted by year
    /// </summary>
    /// <returns></returns> <summary>
    [HttpGet("MonthsByYear")]
    public List<MonthsByYear> GetMonthsByYear()
    {
        return _imageLibrary.GetMonthsByYear();
    }

    /// <summary>
    /// Gets an image given by the image id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Image")]
    public IActionResult GetImage(string id)
    {
        var imageData = _imageLibrary.GetImageData(id);
        if (imageData == null)
        {
            return NotFound();
        }
        return File(imageData, "image/jpeg"); // Set the MIME type to image/jpeg
    }

    /// <summary>
    /// Gets the thumbnail image given by the image id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns> <summary>
    [HttpGet("Thumbnail")]
    public IActionResult GetThumbnail(string id)
    {
        var thumbnailData = _imageLibrary.GetThumbnailData(id);
        if (thumbnailData == null)
        {
            return NotFound();
        }
        return File(thumbnailData, "image/jpeg"); // Set the MIME type to image/jpeg
    }

    /// <summary>
    /// Gets the metadata of an image given by the image id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns> <summary>
    [HttpGet("ImageMetadata")]
    public ImageMetadataBase GetImageMetadata(string id)
    {
        return _imageLibrary.GetImageMetadata(id);
    }

    /// <summary>
    /// Re-loads the library
    /// </summary>
    [HttpGet("LoadImages")]
    public void LoadImages() {
        _imageLibrary.LoadImages();
    }

    /// <summary>
    /// Rotate the given image clockwise by 90deg.
    /// </summary>
    /// <param name="id"></param> <summary>
    [HttpGet("RotateClockwise")]
    public void RotateClockwise(string id) {
        _imageLibrary.RotateClockwise(id);
    }

    /// <summary>
    /// Rotate the given image anti-clockwise by 90deg.
    /// </summary>
    /// <param name="id"></param> <summary>
    [HttpGet("RotateAntiClockwise")]
    public void RotateAntiClockwise(string id) {
        _imageLibrary.RotateAntiClockwise(id);        
    }

    /// <summary>
    /// Delete the file
    /// </summary>
    /// <param name="id"></param> <summary>
    [HttpGet("Delete")]
    public void Delete(string id) {
        _imageLibrary.Delete(id);        
    }

    /// <summary>
    /// Open in editor
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("OpenInEditor")]
    public void OpenInEditor(string id) {
        _imageLibrary.OpenInEditor(id);
    }

}

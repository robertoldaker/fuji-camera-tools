using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ImageViewer.server.Controllers;

public static class ControllerBaseMethods {
    public static IActionResult ModelErrors(this ControllerBase c, Dictionary<string,string> errors) {
        return c.StatusCode(422,errors);
    }

}

using System.Data.SqlTypes;
using System.Runtime.InteropServices.Marshalling;

namespace FujiImageMover;

public class FujiImageMover {

    private static string[] _months =  new string[] {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };
    public FujiImageMover(Config config) {

        // see if we can find an attached sd-card
        var sourceFolder = findAttachedSdCard();
        var destinationFolder = config.DestinationFolder;
        if ( !Directory.Exists(destinationFolder) ) {
            throw new System.Exception($"Destination folder [{destinationFolder}] does not exist");
        }
        
        // if we can't find an attached sd-card, throw an exception
        if ( sourceFolder == null ) {
            throw new System.Exception("No attached SD card found");
        }

        // get all fujifilm recipes we know about via the Google Sheets API
        var fujiRecipes = new FujiRecipes(config.ApiKey, config.SpreadsheetId);

        // get all image files from the source folder        
        var imageFiles = ListFilesWithExtension(sourceFolder, ".JPG");
        foreach (var file in imageFiles) {
            var md = new ImageMetadata(file);
            string recipeName = "";
            if ( md.FujiCustomSettings != null ) {
                recipeName = fujiRecipes.FindMatch(md.FujiCustomSettings);
            }
            // mov file to new destination
            moveFile(file, md, config.DestinationFolder, recipeName);
        }
        Console.WriteLine($"[{imageFiles.Count()}] files processed");

    }

    private void moveFile(string file, ImageMetadata md, string destFolder, string recipeName) {
        string year = md.DateTime.Year.ToString();
        string month = _months[md.DateTime.Month-1];
        //
        string destDir = Path.Combine(destFolder, year, month);
        Directory.CreateDirectory(destDir);
        //
        var fileName = Path.GetFileName(file);
        if ( !string.IsNullOrEmpty(recipeName)) {
            var ext = Path.GetExtension(file);
            var fnNoExt = Path.GetFileNameWithoutExtension(file);
            fileName = $"{fnNoExt} ({recipeName}){ext}";
        }
        //
        string destFile = Path.Combine(destDir, fileName);        
        //
        File.Move(file, destFile);
        //
        Console.WriteLine($"Moved {fileName} => {destFile}");
    }

    private string findAttachedSdCard() {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        var mediaDrives = allDrives
            .Where(d => d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable)
            .Where(d=>d.IsReady && d.RootDirectory!=null)
            .Where(d=>d.RootDirectory.FullName.StartsWith("/media/"));
        
        foreach (DriveInfo d in mediaDrives) {
            var dcimFolder = Path.Combine(d.RootDirectory.FullName, "DCIM");
            if ( Directory.Exists(dcimFolder) ) {
                return d.RootDirectory.FullName;
            }
        }
        return null;
    }

    private IEnumerable<string> ListFilesWithExtension(string directory, string extension) {
        return Directory.GetFiles(directory, $"*{extension}", SearchOption.AllDirectories);
    }
}
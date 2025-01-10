using Google.Apis.Sheets.v4.Data;

namespace FujiImageMover;

public interface IImageMover {
    public void Move(string file, ImageMetadata md, string destFolder, string recipeName);
    public List<string> ListFiles();
}

public class ImageMoverFactory {
    public static string[] Months =  new string[] {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    public static IImageMover Create() {
        // check if we have an attached SD card
        var sourceFolder = findAttachedSdCard();
        if ( sourceFolder != null ) {
            return new SdCardImageMover(sourceFolder);
        }
        // check if we have a camera attached
        var camera = findAttachedCamera();
        if ( camera != null ) {
            return new CameraImageMover();
        }
        // no attached SD card or camera found
        throw new Exception("No attached SD card or camera found");
    }

    private static string findAttachedSdCard() {
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

    private static string findAttachedCamera() {
        var exe = new Execute(); 
        var result = exe.Run("gphoto2", "--auto-detect");
        if ( result != 0 ) {
            throw new Exception($"Failed to run gphoto2: {exe.StandardError}");
        }
        var stdout = exe.StandardOutput;
        if ( stdout.Split("\n").Length > 2) {
            return stdout.Split("\n")[2].Split("   ")[0];
        } else {
            return null;
        }
    }

}

public class SdCardImageMover : IImageMover
{
    private string _sourceFolder;
    public SdCardImageMover(string sourceFolder)
    {
        _sourceFolder = sourceFolder;
    }
    public void Move(string file, ImageMetadata md,string destFolder, string recipeName)
    {
        string year = md.DateTime.Year.ToString();
        string month = ImageMoverFactory.Months[md.DateTime.Month-1];
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

    public List<string> ListFiles() {
        var extension = ".JPG";
        return Directory.GetFiles(_sourceFolder, $"*{extension}").ToList();
    }

}

public class CameraImageMover : IImageMover
{
    private Execute _exe = new Execute();
    public List<string> ListFiles()
    {
        var result = _exe.Run("gphoto2", "--list-files");
        if ( result != 0 ) {
            throw new Exception($"Failed to run gphoto2: {_exe.StandardError}");
        }
        var stdout = _exe.StandardOutput;
        var files = new List<string>();
        var lines =stdout.Split("\n");
        foreach( var line in lines) {
            if ( !line.StartsWith("#") ) {
                continue;
            }
            var parts = line.Split("   ");
            if ( parts.Length > 1) {
                files.Add(parts[1].TrimStart());
            }
        }
        return files;
    }

    public void Move(string file, ImageMetadata md, string destFolder, string recipeName)
    {
        throw new NotImplementedException();
    }
}
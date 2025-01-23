using Google.Apis.Sheets.v4.Data;

namespace ImageViewer.server.Services;

public interface IImageMover {
    public List<string> ListFiles();
    public string MoveNextFile(string destFolder, FujiRecipes? fujiRecipes);
    public int MoveFiles(string destFolder, FujiRecipes? fujiRecipes);
}

public class ImageMoverFactory {
    public static string[] Months =  new string[] {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    public static IImageMover Create() {
        // check if we have an attached SD card
        var sourceFolder = findAttachedSdCard();
        if ( sourceFolder != null ) {
            Console.WriteLine($"Found attached SD card at [{sourceFolder}]");
            return new SdCardImageMover(sourceFolder);
        }
        // check if we have a camera attached
        var camera = findAttachedCamera();
        if ( camera != null ) {
            Console.WriteLine($"Found attached camera [{camera}]");
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

    public List<string> ListFiles() {
        return listFiles();
    }

    public string MoveNextFile(string destFolder, FujiRecipes? fujiRecipes) {
        var imageFiles = listFiles();
        string fileMoved = "";
        foreach (var file in imageFiles) {
            fileMoved = moveFile(file,destFolder,fujiRecipes);
            if ( fileMoved!="" ) {
                return fileMoved;
            }
        }
        return fileMoved;
    }

    private string moveFile(string file, string destFolder, FujiRecipes? fujiRecipes) {
        var md = new ImageMetadata(file);
        string recipeName = "";
        string fileMoved = "";
        if ( fujiRecipes!=null && md.FujiCustomSettings != null ) {
            recipeName = fujiRecipes.FindMatch(md.FujiCustomSettings);
        }
        // Only move files with a valid date
        if ( ! (md.DateTime == default) ) {
            fileMoved = moveFile(file, md, destFolder, recipeName);
        }
        return fileMoved;
    }

    public int MoveFiles(string destFolder, FujiRecipes? fujiRecipes)
    {
        var imageFiles = listFiles();
        int moveCount = 0;
        foreach (var file in imageFiles) {
            var movedFile = moveFile(file, destFolder, fujiRecipes);
            if ( movedFile!="" ) {
                moveCount++;
            }
        }
        return moveCount;
    }

    private string moveFile(string file, ImageMetadata md,string destFolder, string recipeName)
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
        return destFile;
    }

    private List<string> listFiles() {
        //var extension = ".JPG";
        return Directory.GetFiles(_sourceFolder, $"*").ToList();
    }

}

public class CameraImageMover : IImageMover
{
    private Execute _exe = new Execute();

    
    public int MoveFiles(string destFolder, FujiRecipes? fujiRecipes)
    {
        var imageFiles = listFiles();
        int movedCount = 0;
        foreach (var file in imageFiles) {
            //
            var movedFile = moveFile(file, destFolder, fujiRecipes);
            if ( movedFile!="") {
                movedCount++;
            }
        }
        return movedCount;
    }

    private List<string> listFiles()
    {
        var result = _exe.Run("gphoto2", "--list-files");
        if ( result != 0 ) {
            if ( _exe.StandardError.Contains("Could not claim the USB device") ) {
                throw new Exception("Please unmount device from file manager or close any app that is using the camera.");
            } else {
                throw new Exception($"Failed to run gphoto2: {_exe.StandardError}");
            }
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

    private void moveFileLocally(string file)
    {
        var result = _exe.Run("gphoto2", $"--get-file 1 --force-overwrite");
        if ( result != 0 ) {
            throw new Exception($"Failed to move file locally: {_exe.StandardError}");
        }
    }

    private void deleteFileFromCamera()
    {
        var result = _exe.Run("gphoto2", $"--delete-file 1 --recurse");
        if ( result != 0 ) {
            throw new Exception($"Failed to delete file from camera: {_exe.StandardError}");
        }
    }

    private string moveFile(string file, ImageMetadata md,string destFolder, string recipeName)
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
        // move the local copy
        File.Move(file, destFile,true);
        // delete the original file from the camera - always #1
        deleteFileFromCamera();
        //
        return destFile;
    }

    public List<string> ListFiles()
    {
        return listFiles();
    }

    public string MoveNextFile(string destFolder, FujiRecipes? fujiRecipes)
    {
        var files = listFiles();
        string movedFile = "";
        foreach( var file in files) {
            movedFile = moveFile(file, destFolder, fujiRecipes);
            if ( movedFile!="") {
                return movedFile;
            }
        }
        return movedFile;
    }

    private string moveFile(string file, string destFolder, FujiRecipes? fujiRecipes) {

        string movedFile = "";
        moveFileLocally(file);
        //
        var md = new ImageMetadata(file);
        string recipeName = "";
        if ( fujiRecipes!=null && md.FujiCustomSettings != null ) {
            recipeName = fujiRecipes.FindMatch(md.FujiCustomSettings);
        }
        // Only move files with a valid date
        if ( ! (md.DateTime == default) ) {
            movedFile = moveFile(file, md, destFolder, recipeName);
       }
        return movedFile;
    }
}

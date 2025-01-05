namespace FujiImageMover;

public class FujiImageMover {
    public FujiImageMover() {
        var sourceFolder = findAttachedSdCard();

        //
        var fujiRecipes = new FujiRecipes();

        if ( sourceFolder == null ) {
            throw new System.Exception("No attached SD card found");
        }

        var imageFiles = ListFilesWithExtension(sourceFolder, ".JPG");
        foreach (var file in imageFiles) {
            var md = new ImageMetadata(file);
            //??Console.WriteLine(md);
        }
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
using ImageViewer.shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageViewer.server.Services;

public class ImageLibraryService
{
    private readonly Config _config;

    private List<ImageItem> _images = new List<ImageItem>();

    private string? _loadedImageFolder = null;

    public ImageLibraryService(Config config)
    {
        _config = config;
        LoadImages();
    }    

    public void LoadImages() {
        // only load in the config has changed
        if ( _loadedImageFolder!=_config.ImageFolder) {
            lock( _images ) {
                var files = getImageFiles();
                createThumbnails(files);
                createImageItems(files);
            }
        }
    }

    public void NewImage(string imagePath) {
        lock( _images ) {
            createThumbnail(imagePath);
            var imageItem = createImageItem(imagePath);
            _images.Add(imageItem);
            // sort by descending date
            _images.Sort((a, b) => b.Metadata.DateTime.CompareTo(a.Metadata.DateTime));
        }
    }

    private void createThumbnails(List<string> files) {
        foreach (var f in files) {
            createThumbnail(f);
        }
    }


    private void createThumbnail(string imagePath, bool overwrite=false) {

        var thumbnailPath = getThumbnailPath(imagePath);
        var thumbnailFolder = Path.GetDirectoryName(thumbnailPath)!;
        if ( overwrite || !File.Exists(thumbnailPath)) {
            try {
                using (var image = Image.Load(imagePath)) {
                    var height = 100;
                    var width = (image.Width * height) / image.Height;
                    image.Mutate(x => x.Resize(width, height));
                    if ( !Directory.Exists(thumbnailFolder)) {
                        Directory.CreateDirectory(thumbnailFolder);
                    }
                    image.Save(thumbnailPath);
                }
            } catch {

            }
        }
    }
    
    private string getThumbnailPath(string imagePath) {
        var folder = Path.GetDirectoryName(imagePath);
        var ext = Path.GetExtension(imagePath);
        var fn = Path.GetFileNameWithoutExtension(imagePath);        
        var thumbnailFolder = Path.Combine(folder!, ".thumbnails");
        return Path.Combine(thumbnailFolder, $"{fn}.thumbnail{ext}");
    }

    private void createImageItems(List<string> files) {
        _images.Clear();
        foreach (var f in files) {
            var imageItem = createImageItem(f);
            _images.Add(imageItem);
        }
        // sort by descending date
        _images.Sort((a, b) => b.Metadata.DateTime.CompareTo(a.Metadata.DateTime));
    }

    private ImageItem createImageItem(string f) {
        var metadata = new ImageMetadata(f);
        // id is the file releative to the image folder
        var id = f.Replace(_config.ImageFolder, "");
        // ensure it does not start with directory separator (Path.Combine does not work!)
        if ( id.StartsWith(Path.DirectorySeparatorChar)) {
            id = id.Substring(1);
        }
        return new ImageItem(id, metadata);
    }

    private List<string> getImageFiles() {
        List<string> files = new List<string>();
        if ( Directory.Exists(_config.ImageFolder)) {
            getImageFiles(files, _config.ImageFolder);
            _loadedImageFolder = _config.ImageFolder;
        } 
        return files;
    }

    private void getImageFiles(List<string> totalFiles, string folder) {
        var files = Directory.GetFiles(folder, "*.JPG").ToList();        
        totalFiles.AddRange(files);
        var folders = Directory.GetDirectories(folder);
        foreach (var f in folders) {
            // ignore thumbnails
            if ( !f.Contains(".thumbnails")) {
                getImageFiles(totalFiles, f);
            }
        }
    }

    private class ImageItem {
        public ImageItem(string id, ImageMetadata metadata) {
            Id = id;
            Metadata = metadata;
        }
        public string Id {get; set;}
        public ImageMetadata Metadata {get; set;}
    } 

    public List<ImagesByDate> GetImagesByDate(int year, int month) {
        var imagesByDateList = new List<ImagesByDate>();
        lock( _images) {
            var images = _images.Where(x => x.Metadata.DateTime.Year == year && x.Metadata.DateTime.Month == month).ToList();
            var curDate= new DateTime();                    
            ImagesByDate? imagesByDate = null;
            foreach (var i in images) {
                var date = i.Metadata.DateTime.Date;
                if ( date != curDate) {
                    curDate = date;
                    imagesByDate = new ImagesByDate(date);
                    imagesByDateList.Add(imagesByDate);
                }
                var imagePath = Path.Combine(_config.ImageFolder,i.Id);
                imagesByDate!.Images.Add(new ImageViewer.shared.ImageInfo(i.Id, imagePath, false));
            }
        }
        return imagesByDateList;
    }  

    public List<MonthsByYear> GetMonthsByYear() {
        var monthsByYearList = new List<MonthsByYear>();
        lock( _images) {
            var years = _images.Select(x => x.Metadata.DateTime.Year).Distinct().ToList();
            foreach (var y in years) {
                var monthsByYear = new MonthsByYear(y);
                monthsByYearList.Add(monthsByYear);
                var months = _images.Where(x => x.Metadata.DateTime.Year == y).Select(x => x.Metadata.DateTime.Month).Distinct().ToList();
                monthsByYear.Months.AddRange(months);
            }
        }
        return monthsByYearList;
    }

    public ImageMetadata GetImageMetadata(string id) {
        lock( _images) {
            var image = _images.FirstOrDefault(x => x.Id == id.ToString());
            if ( image == null) {
                throw new Exception($"Image {id} not found");
            }
            return image.Metadata;
        }
    }

    public byte[] GetImageData(string id) {
        lock( _images) {
            var image = _images.FirstOrDefault(x => x.Id == id.ToString());
            if ( image == null) {
                throw new Exception($"Image {id} not found");
            }
            var imagePath = Path.Combine(_config.ImageFolder,image.Id);
            return File.ReadAllBytes(imagePath);
        }
    }

    public byte[] GetThumbnailData(string id) {
        lock( _images) {
            var image = _images.FirstOrDefault(x => x.Id == id.ToString());
            if ( image == null) {
                throw new Exception($"Image {id} not found");
            }
            var thumbnailPath = getThumbnailPath(id);
            thumbnailPath = Path.Combine( _config.ImageFolder , thumbnailPath);
            return File.ReadAllBytes(thumbnailPath);
        }
    }

    public void RotateClockwise(string id) {
        rotate(id, 90);
    }

    public void RotateAntiClockwise(string id) {
        rotate(id, -90);
    }

    private void rotate(string id, float deg) {
        lock (_images) {
            var imagePath = Path.Combine(_config.ImageFolder,id);
            if ( File.Exists(imagePath)) {
                using (var image = Image.Load(imagePath)) {
                    image.Mutate(x => x.Rotate(deg));
                    image.Save(imagePath);
                }
                //
                createThumbnail(imagePath,true);
            }
        }
    }

    public void Delete(string id) {
        lock (_images) {
            var imagePath = Path.Combine(_config.ImageFolder,id);
            var thumbnailPath = getThumbnailPath(imagePath);
            if ( File.Exists(imagePath)) {
                File.Delete(imagePath);
            }
            if ( File.Exists(thumbnailPath)) {
                File.Delete(thumbnailPath);
            }
            var image = _images.FirstOrDefault(x => x.Id == id.ToString());
            if ( image!=null ) {
                _images.Remove(image);
            }
        }
    }
    public void OpenInEditor(string id) {
        var imagePath = Path.Combine(_config.ImageFolder,id);
        var editor = _config.ExternalEditor;
        System.Diagnostics.Process.Start("nohup",new List<string>() {editor,imagePath});
    }
}


using ImageViewer.shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageViewer.server.Services;

public class ImageLibrary
{
    private readonly Config _config;

    private List<ImageItem> _images = new List<ImageItem>();

    private string? _loadedImageFolder = null;

    public ImageLibrary(Config config)
    {
        _config = config;
    }    

    public void LoadImages() {
        // only load in the config has changed
        if ( _loadedImageFolder!=_config.ImageFolder) {
            var files = getImageFiles();
            createThumbnails(files);
            createImageItems(files);
        }
    }

    private void createThumbnails(List<string> files) {
        foreach (var f in files) {
            createThumbnail(f);
        }
    }


    private void createThumbnail(string imagePath) {

        var thumbnailPath = getThumbnailPath(imagePath);
        var thumbnailFolder = Path.GetDirectoryName(thumbnailPath)!;
        if ( !File.Exists(thumbnailPath)) {
            using (var image = Image.Load(imagePath)) {
                var height = 100;
                var width = (image.Width * height) / image.Height;
                image.Mutate(x => x.Resize(width, height));
                if ( !Directory.Exists(thumbnailFolder)) {
                    Directory.CreateDirectory(thumbnailFolder);
                }
                image.Save(thumbnailPath);
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
        lock( _images) {
            _images.Clear();
            foreach (var f in files) {
                var metadata = new ImageMetadata(f);
                // id is the file releative to the image folder
                var id = f.Replace(_config.ImageFolder, "");
                // ensure it does not start with directory separator (Path.Combine does not work!)
                if ( id.StartsWith(Path.DirectorySeparatorChar)) {
                    id = id.Substring(1);
                }
                _images.Add(new ImageItem(id, metadata));
            }
            // sort by descending date
            _images.Sort((a, b) => b.Metadata.DateTime.CompareTo(a.Metadata.DateTime));
        }
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
        var files = Directory.GetFiles(folder, "*").ToList();
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
                imagesByDate!.Images.Add(new ImageViewer.shared.ImageInfo(i.Id, false));
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
}


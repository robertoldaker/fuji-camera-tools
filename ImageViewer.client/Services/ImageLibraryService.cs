using System.Threading.Tasks;
using ImageViewer.shared;
using static ImageViewer.client.Services.MainDisplayService;

namespace ImageViewer.client.Services;

public class ImageLibraryService {

    public ImageLibraryService(DataAccessService dataAccessService, MainDisplayService mainDisplayService) {
        _dataAccessService = dataAccessService;
        _mainDisplayService = mainDisplayService;
        ReloadAsync();
    }

    private DataAccessService _dataAccessService;
    private MainDisplayService _mainDisplayService;
    public event EventHandler<bool>? LoadImagesStarted;
    private bool _loadingImages = false;    
    public event EventHandler<List<MonthsByYear>>? MonthsByYearLoaded;
    public List<MonthsByYear>? MonthsByYear { get; set; }
    public event EventHandler<List<ImagesByDate>>? ImagesByDateLoaded;
    public List<ImagesByDate>? ImagesByDateList {get; set;}
    public int Year {get; set;}
    public int Month {get; set;}

    public event EventHandler<ImageInfo>? NewImageSelected;

    public event EventHandler<ImageMetadataBase>? NewMetadataLoaded;

    public event EventHandler<ImageInfo>? ImageEdited;

    public async Task ReloadAsync() {
        _loadingImages = true;
        LoadImagesStarted?.Invoke(this,_loadingImages);
        //done now in the server
        await LoadMonthsByYearAsync();
        _loadingImages = false;
        LoadImagesStarted?.Invoke(this,_loadingImages);
    }

    public bool LoadingImages {
        get {
            return _loadingImages;
        }
    }

    public async Task LoadMonthsByYearAsync() {
        MonthsByYear = await _dataAccessService.GetMonthsByYearAsync();
        _mainDisplayService?.SetShowState(ShowStateEnum.Thumbnails);
        MonthsByYearLoaded?.Invoke(this,MonthsByYear);
        int year = 0;
        int month = 0;
        if ( MonthsByYear!=null && MonthsByYear.Count>0 ) {
            year = MonthsByYear[0].Year;
            month = MonthsByYear[0].Months[0];
        } 
        await SelectMonthAsync(year, month);
    }

    public async Task SelectMonthAsync(int year, int month) {
        if ( year==0 ) {
            ImagesByDateList = new List<ImagesByDate>();
        } else {
            ImagesByDateList = await _dataAccessService.GetImagesByDateAsync(year, month);
        }
        Year = year;
        Month = month;
        _mainDisplayService?.SetShowState(ShowStateEnum.Thumbnails);
        ImagesByDateLoaded?.Invoke(this,ImagesByDateList);
    }
    public async Task ReloadMonthAsync() {
        ImagesByDateList = await _dataAccessService.GetImagesByDateAsync(Year, Month);
        ImagesByDateLoaded?.Invoke(this,ImagesByDateList);
    }

    public ImageInfo? SelectedImage {get; set;}

    public ImageMetadataBase? ImageMetadata {get; set;}

    private int _selectedImageIndex = -1;
    private int _selectedDateIndex = -1;

    public void SelectImage(int dateIndex, int imageIndex) {
        _selectedDateIndex = dateIndex;
        _selectedImageIndex = imageIndex;
        selectImage();
    }

    private async Task selectImage() {
        if ( ImagesByDateList!=null ) {
            SelectedImage = ImagesByDateList[_selectedDateIndex].Images[_selectedImageIndex];
            NewImageSelected?.Invoke(this,SelectedImage);
            _mainDisplayService.SetShowState(ShowStateEnum.ImageEditor);
            //
            ImageMetadata = await _dataAccessService!.GetImageMetadataAsync(SelectedImage.Id);
            NewMetadataLoaded?.Invoke(this,ImageMetadata);
        }
    }

    public void SelectNextImage() {
        if ( ImagesByDateList!=null && ImagesByDateList.Count > 0 ) {
            _selectedImageIndex++;
            if ( _selectedImageIndex>=ImagesByDateList[_selectedDateIndex].Images.Count) {
                _selectedImageIndex = 0;
                _selectedDateIndex++;
                if ( _selectedDateIndex>=ImagesByDateList.Count) {
                    _selectedDateIndex = 0;
                }
            }
            selectImage();
        }
    }

    public void ReSelectImage() {
        if ( ImagesByDateList!=null && ImagesByDateList.Count > 0 ) {
            if ( _selectedImageIndex>=ImagesByDateList[_selectedDateIndex].Images.Count) {
                _selectedImageIndex = 0;
                _selectedDateIndex++;
                if ( _selectedDateIndex>=ImagesByDateList.Count) {
                    _selectedDateIndex = 0;
                }
            }
            selectImage();
        } 
    }

    public void SelectPrevImage() {
        if ( ImagesByDateList!=null && ImagesByDateList.Count > 0 ) {
            _selectedImageIndex--;
            if ( _selectedImageIndex<0) {
                _selectedDateIndex--;
                if ( _selectedDateIndex<0) {
                    _selectedDateIndex = ImagesByDateList.Count-1;;
                }
                _selectedImageIndex = ImagesByDateList[_selectedDateIndex].Images.Count-1;
            }
            selectImage();
        }        
    }

    public void SelectedImageEdited() {
        if ( SelectedImage!=null) {
            ImageEdited?.Invoke(this,SelectedImage);
        }
    }
   
}
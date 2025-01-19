using System.Threading.Tasks;
using ImageViewer.shared;
using static ImageViewer.client.Services.MainDisplayService;

namespace ImageViewer.client.Services;

public class ImageLibraryService {

    public ImageLibraryService(DataAccessService dataAccessService, MainDisplayService mainDisplayService) {
        _dataAccessService = dataAccessService;
        _mainDisplayService = mainDisplayService;
        LoadMonthsByYearAsync();
    }

    private DataAccessService _dataAccessService;
    private MainDisplayService _mainDisplayService;
    
    public event EventHandler<List<MonthsByYear>>? MonthsByYearLoaded;
    public List<MonthsByYear>? MonthsByYear { get; set; }

    public event EventHandler<List<ImagesByDate>>? ImagesByDateLoaded;
    public List<ImagesByDate>? ImagesByDateList {get; set;}
    public int Year {get; set;}
    public int Month {get; set;}

    public event EventHandler<ImageInfo>? NewImageSelected;

    public event EventHandler<ImageMetadataBase>? NewMetadataLoaded;

    public async Task LoadNewThumbnailsAsync(int year, int month) {
        ImagesByDateList = await _dataAccessService.GetImagesByDateAsync(year, month);
        Year = year;
        Month = month;
        ImagesByDateLoaded?.Invoke(this,ImagesByDateList);
    }

    public async Task LoadMonthsByYearAsync() {
        MonthsByYear = await _dataAccessService.GetMonthsByYearAsync();
        MonthsByYearLoaded?.Invoke(this,MonthsByYear);
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
   
}
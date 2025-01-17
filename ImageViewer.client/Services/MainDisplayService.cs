using System.Data;
using ImageViewer.shared;

namespace ImageViewer.client.Services;

public class MainDisplayService {
    public enum ShowStateEnum {Thumbnails,ImageEditor}

    public MainDisplayService(DataAccessService dataAccessService) {
        ShowState = ShowStateEnum.Thumbnails;
        _dataAccessService = dataAccessService;
    }

    public event EventHandler<ShowStateEnum>? OnShowStateChanged = null;

    public ShowStateEnum ShowState {get; private set;}

    public ImageInfo? SelectedImage {get; set;}

    private int _selectedImageIndex = -1;
    private int _selectedDateIndex = -1;

    private DateTime _date;

    private DataAccessService _dataAccessService;

    public void SetShowState(ShowStateEnum newState) {
        ShowState = newState;
        OnShowStateChanged?.Invoke(this,newState);
    }

    public void SelectImage(int dateIndex, int imageIndex) {
        _selectedDateIndex = dateIndex;
        _selectedImageIndex = imageIndex;
        if ( _dataAccessService.ImagesByDateList!=null ) {
            SelectedImage = _dataAccessService.ImagesByDateList[_selectedDateIndex].Images[_selectedImageIndex];
        }
        SetShowState(ShowStateEnum.ImageEditor);
    }

    public void SelectNextImage() {
        if ( _dataAccessService.ImagesByDateList!=null && _dataAccessService.ImagesByDateList.Count > 0 ) {
            _selectedImageIndex++;
            if ( _selectedImageIndex>=_dataAccessService.ImagesByDateList[_selectedDateIndex].Images.Count) {
                _selectedImageIndex = 0;
                _selectedDateIndex++;
                if ( _selectedDateIndex>=_dataAccessService.ImagesByDateList.Count) {
                    _selectedDateIndex = 0;
                }
            }
            SelectedImage = _dataAccessService.ImagesByDateList[_selectedDateIndex].Images[_selectedImageIndex];
        }
    }
    public void SelectPrevImage() {
        if ( _dataAccessService.ImagesByDateList!=null && _dataAccessService.ImagesByDateList.Count > 0 ) {
            _selectedImageIndex--;
            if ( _selectedImageIndex<0) {
                _selectedDateIndex--;
                if ( _selectedDateIndex<0) {
                    _selectedDateIndex = _dataAccessService.ImagesByDateList.Count-1;;
                }
                _selectedImageIndex = _dataAccessService.ImagesByDateList[_selectedDateIndex].Images.Count-1;
            }
            SelectedImage = _dataAccessService.ImagesByDateList[_selectedDateIndex].Images[_selectedImageIndex];
        }        
    }
}
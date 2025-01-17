using System.Data;

namespace ImageViewer.client.Services;

public class MainDisplayService {
    public enum ShowStateEnum {Thumbnails,ImageEditor}


    public MainDisplayService() {
        ShowState = ShowStateEnum.Thumbnails;
    }

    public event EventHandler<ShowStateEnum>? OnShowStateChanged = null;

    public ShowStateEnum ShowState {get; private set;}

    public void SetShowState(ShowStateEnum newState) {
        ShowState = newState;
        OnShowStateChanged?.Invoke(this,newState);
    }
}

using ImageViewer.client.Components.Shared;

namespace ImageViewer.client.Services;

public enum ModalDialog {MessageBox, Settings}

public enum MessageBoxIcon {Info,Warning,Error}

public class ModalsService {
    private Dictionary<ModalDialog,ModalBase> _dict = new Dictionary<ModalDialog, ModalBase>();

    public void Register(ModalBase modalBase,ModalDialog modalDialog) {
        _dict.Add(modalDialog,modalBase);
    }

    public void Show(ModalDialog modalDialog) {
        if ( _dict.TryGetValue(modalDialog, out ModalBase? modalBase)) {
            if ( modalBase!=null) {
                modalBase.Modal?.ShowAsync();
            }
        }
    }

    public void ShowMessageBox(string message, MessageBoxIcon icon = MessageBoxIcon.Info) {
        if ( _dict.TryGetValue(ModalDialog.MessageBox, out ModalBase? modalBase)) {
            if ( modalBase!=null && modalBase is MessageBox) {
                //
                ((MessageBox) modalBase).Message = message;
                ((MessageBox) modalBase).SetIcon(icon);
                //
                modalBase.Modal?.ShowAsync();
            }
        }
    }


}
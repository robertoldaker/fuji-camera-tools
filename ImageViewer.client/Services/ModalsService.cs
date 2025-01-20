
using ImageViewer.client.Components.Shared;

namespace ImageViewer.client.Services;

public class ModalsService {
    private Dictionary<string,ModalBase> _dict = new Dictionary<string, ModalBase>();
    public void Register(ModalBase modalBase,string name) {
        Console.WriteLine($"Register modal [{modalBase.Modal?.Title}] [{name}]");
        _dict.Add(name,modalBase);
    }

    public void Show(string name) {
        if ( _dict.TryGetValue(name, out ModalBase? modalBase)) {
            if ( modalBase!=null) {
                Console.WriteLine($"Show [{modalBase.Modal?.Title}]");
                modalBase.Modal?.ShowAsync();
            }
        }
    }


}
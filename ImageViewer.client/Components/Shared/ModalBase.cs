using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace ImageViewer.client.Components.Shared;

public class ModalBase : ComponentBase {
    public Modal? Modal {get; set;}
    protected Dictionary<string,ErrorText?> _errorTexts = new Dictionary<string, ErrorText?>();
    public void UpdateErrors(Dictionary<string,string> errors) {
        // Clear all errors
        foreach( var et in _errorTexts) {
            if ( et.Value!=null ) {
                et.Value.SetError("");
            }
        }
        // Set errors that match keys in errors dictionary
        foreach( var e in errors) {
            if ( _errorTexts.TryGetValue(e.Key,out ErrorText? errorText ) ) {
                if (errorText!=null ) {
                    errorText.SetError(e.Value);
                } 
            } 
        }
    }
}
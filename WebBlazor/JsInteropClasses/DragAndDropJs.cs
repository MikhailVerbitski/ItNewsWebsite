using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using WebBlazor.Services;

namespace WebBlazor.JsInteropClasses
{
    public static class DragAndDropJs
    {
        public static ServiceOfImage serviceOfImage { get; set; }
        public static event Action<string> sendEvent;
        
        public static Task<bool> IncludeJs(int id)
        {
            return JSRuntime.Current.InvokeAsync<bool>("DragAndDrop.Init", id);
        }
        [JSInvokable]
        public static async Task Send(string data, int postId, string Extension)
        {
            if(serviceOfImage != null)
            {
                var result = await serviceOfImage.LoadPostImage(data, postId, Extension);
                sendEvent.Invoke(result);
            }
        }
    }
}
    
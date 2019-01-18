using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace WebBlazor.JsInteropClasses
{
    public class TagCloud
    {
        public static Task<string> StartTagCanvas()
        {
            return JSRuntime.Current.InvokeAsync<string>("StartTagCanvas.start");
        }
    }
}

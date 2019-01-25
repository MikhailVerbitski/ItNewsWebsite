using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace WebBlazor.JsInteropClasses
{
    public class TagCloud
    {
        public static Task<string> StartTagCanvas(string jsonWords)
        {
            return JSRuntime.Current.InvokeAsync<string>("TagCloudStarterFunction.Start", jsonWords);
        }
    }
}

using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace WebBlazor.JsInteropClasses
{
    public class ReloadPage
    {
        public static Task<string> Reload()
        {
            return JSRuntime.Current.InvokeAsync<string>("reload.reloadPage");
        }
    }
}
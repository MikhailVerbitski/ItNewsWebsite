using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace WebBlazor.JsInteropClasses
{
    public class Reload
    {
        public static async Task ReloadPage()
        {
            await JSRuntime.Current.InvokeAsync<object>("reload.ReloadPage");
        }
    }
}

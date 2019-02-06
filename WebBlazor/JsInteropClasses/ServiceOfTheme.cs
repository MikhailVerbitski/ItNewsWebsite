using Blazor.Extensions.Storage;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace WebBlazor.JsInteropClasses
{
    public class ServiceOfTheme
    {
        private readonly LocalStorage localStorage;
        private string ThemeIsDefault { get; set; }

        public ServiceOfTheme(LocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        public string PahtIcon { get { return (ThemeIsDefault == null || ThemeIsDefault == "False") ? "/img/moon.png" : "/img/sun.png"; } }

        public async Task Init()
        {
            ThemeIsDefault = await localStorage.GetItem<string>("ThemeIsDefault");
            if (ThemeIsDefault != null)
            {
                await JSRuntime.Current.InvokeAsync<object>("Theme.ChangeTheme", Convert.ToBoolean(ThemeIsDefault));
            }
        }
        public async Task ChangeTheme()
        {
            ThemeIsDefault = await localStorage.GetItem<string>("ThemeIsDefault");
            if (ThemeIsDefault == null)
            {
                await localStorage.SetItem<string>("ThemeIsDefault", "false");
                await JSRuntime.Current.InvokeAsync<object>("Theme.ChangeTheme", false);
            }
            else
            {
                ThemeIsDefault = (!Convert.ToBoolean(ThemeIsDefault)).ToString();
                await localStorage.SetItem<string>("ThemeIsDefault", ThemeIsDefault);
                await JSRuntime.Current.InvokeAsync<object>("Theme.ChangeTheme", Convert.ToBoolean(ThemeIsDefault));
            }
        }
    }
}

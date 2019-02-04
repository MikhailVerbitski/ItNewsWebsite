using System.ComponentModel;
using System;
using System.Collections;
using System.Linq;

namespace WebBlazor.Models.ViewModels.Account
{
    public class LoginValidationViewModel : IDataErrorInfo, INotifyDataErrorInfo
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }


        public string this[string property]
        {
            get
            {
                return GetErrors(property).Cast<string>().FirstOrDefault<string>();
            }
        }
        public string Error
        {
            get
            {
                return null;
            }
        }
        public bool HasErrors => GetErrors(null).Cast<string>().Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string property)
        {
            if (property == null || property == nameof(this.Login))
            {
                if (string.IsNullOrEmpty(this.Login))
                {
                    yield return $"{nameof(this.Login)} is mandatory";
                }
            }
            if (property == null || property == nameof(this.Password))
            {
                if (string.IsNullOrEmpty(this.Password))
                {
                    yield return $"{nameof(this.Password)} is mandatory";
                }
                if (this.Password != null && this.Password.Length < 6)
                {
                    yield return "minimum password length is 6 characters";
                }
            }
        }
    }
}

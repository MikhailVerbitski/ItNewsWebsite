﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace WebBlazor.Models.ViewModels.Account
{
    public class RegisterValidationViewModel : IDataErrorInfo, INotifyDataErrorInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }


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
            if (property == null || property == nameof(this.FirstName))
            {
                if (string.IsNullOrEmpty(this.FirstName))
                {
                    yield return $"{nameof(this.FirstName)} is mandatory";
                }
            }
            if (property == null || property == nameof(this.LastName))
            {
                if (string.IsNullOrEmpty(this.LastName))
                {
                    yield return $"{nameof(this.LastName)} is mandatory";
                }
            }
            if (property == null || property == nameof(this.Email))
            {
                if (string.IsNullOrEmpty(this.Email))
                {
                    yield return $"{nameof(this.Email)} is mandatory";
                }
            }
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
            if (property == null || property == nameof(this.ConfirmPassword))
            {
                if (string.IsNullOrEmpty(this.ConfirmPassword))
                {
                    yield return $"{nameof(this.ConfirmPassword)} is mandatory";
                }
                if (this.Password != null && this.ConfirmPassword != null && this.Password != this.ConfirmPassword)
                {
                    yield return "passwords do not match";
                }
            }
        }
    }
}

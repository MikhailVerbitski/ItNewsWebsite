﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Models.ViewModels.Account
{
    public class LoginViewModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginResult
    {
        public string? Token { get; set; }
        public string? UserName { get; set; }
        public string? UserRole { get; set; }
    }
}

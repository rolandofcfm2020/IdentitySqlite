using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityRVapi.Models
{
    public class LoginResponse
    {
        public LoginResponse()
        {
            IsAuthenticated = false;
        }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public Claims UserClaims { get; set; }
    }

    public class Claims
    {
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
        public bool isActive { get; set; }
        public string scopes { get; set; }
    }
}

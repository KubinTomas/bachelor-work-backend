using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models
{
    public class AuthenticationResult
    {

        public string TokenString { get; set; }
        public AuthorizationResultEnum Result { get; set; }

        public AuthenticationResult(string tokenString, AuthorizationResultEnum result = AuthorizationResultEnum.ok)
        {
            this.TokenString = tokenString;
            this.Result = result;
        }

        public AuthenticationResult(AuthorizationResultEnum result = AuthorizationResultEnum.failed)
        {
            this.Result = result;
        }
    }

    public enum AuthorizationResultEnum
    {
        ok,
        failed
    }
}

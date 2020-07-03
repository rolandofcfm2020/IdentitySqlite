using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityRVapi.Backend;
using IdentityRVapi.DataAccess;
using IdentityRVapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IdentityRVapi.Controllers
{
    [Route("[controller]/[action]")]
    public class RvUserController : ControllerBase
    {

        [HttpPost]
        public bool CreateUser(string UserName, string Password, string scope = "")
        {
            IdentityRvContext dataContext = new IdentityRvContext();
            var newCredentials = new RvNetUsers();
            newCredentials.UserName = UserName;
            newCredentials.SaltValue = Guid.NewGuid().ToString();
            newCredentials.Id = Guid.NewGuid();
            newCredentials.HashedPassword = new IdentitySC().Encrypt(Password, newCredentials.SaltValue);
            newCredentials.CreationDate = DateTime.Now;
            newCredentials.IsActive = true;

            dataContext.RvNetUsers.Add(newCredentials);
            dataContext.SaveChanges();

            return true;
        }

        [HttpGet]
        public object GetAllUserData()
        {
            IdentityRvContext dataContext = new IdentityRvContext();
            var list = dataContext.RvNetUsers.ToList();
            return list;
        }

        [HttpGet]
        public LoginResponse Login(string UserName, string Password, string scope = "")
        {
            IdentityRvContext dataContext = new IdentityRvContext();
            LoginResponse loginResponse = new LoginResponse();
            var userInDB = dataContext.RvNetUsers.FirstOrDefault(f => f.UserName == UserName);

            if (userInDB == null)
            {
                loginResponse.IsAuthenticated = false;
                loginResponse.Message = "No se ha encontrado el usuario solicitado";
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return loginResponse;
            }

            loginResponse = new IdentitySC().GetUserClaims(UserName, Password, scope);

            if (loginResponse.UserClaims == null)
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return loginResponse;
        }

    }
}
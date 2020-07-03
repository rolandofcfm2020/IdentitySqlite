using IdentityRVapi.DataAccess;
using IdentityRVapi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityRVapi.Backend
{
    public class IdentitySC
    {

        public string Encrypt(string password, string saltValue)
        {
            string secretKey = "0406b130-bd65-11ea-b3de-0242ac130004";
            var saltBuffer = Encoding.UTF8.GetBytes(saltValue);
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, saltBuffer);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }

            return password;
        }

        public static string Decrypt(string cipherText, string saltValue)
        {
            string secretKey = "0406b130-bd65-11ea-b3de-0242ac130004";
            var saltBuffer = Encoding.UTF8.GetBytes(saltValue);
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, saltBuffer);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public LoginResponse GetUserClaims(string userName, string password, string scope)
        {
            LoginResponse lr = new LoginResponse();

            IdentityRvContext dataContext = new IdentityRvContext();
            var user = dataContext.RvNetUsers.FirstOrDefault(f => f.UserName == userName);
            var salt = user.SaltValue;

            var hashedPassword = user.HashedPassword;
            var loginResultSuccess = this.Login(salt, hashedPassword, password);
            if (loginResultSuccess)
            {
                lr.IsAuthenticated = true;
                lr.Message = "Ingreso correcto";
                lr.UserClaims = new Claims();
                lr.UserClaims.UserName = user.UserName;
                lr.UserClaims.CreationDate = user.CreationDate;
                lr.UserClaims.isActive = user.IsActive;
                lr.UserClaims.scopes = user.Scopes;

            }
            else
            {
                lr.IsAuthenticated = false;
                lr.Message = "La contraseña introducida para este usuario es incorrecta";
            }
            return lr;
        }

        private bool Login(string salt, string hashedPassword, string userPassword)
        {
            var decryptedPassword = Decrypt(hashedPassword, salt);
            if (decryptedPassword == userPassword)
                return true;

            else
                return false;
        }
    }
}

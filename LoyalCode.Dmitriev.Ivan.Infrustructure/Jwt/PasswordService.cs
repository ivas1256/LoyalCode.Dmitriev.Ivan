using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt
{
    public interface IPasswordService
    {
        string EncryptPassword(string password);
        bool VerifyPassword(string password, string encryptedPassword);
    }

    internal class PasswordService : IPasswordService
    {
        public string EncryptPassword(string password)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(plainTextBytes);
        }

        public bool VerifyPassword(string password, string encryptedPassword)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(password);
            var userPassword = Convert.ToBase64String(plainTextBytes);

            return userPassword == encryptedPassword;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public interface IEncryptionService
    {

        String Encrypt(String plainText, String key);

        String Decrypt(String encryptedText, String key);

    }
}

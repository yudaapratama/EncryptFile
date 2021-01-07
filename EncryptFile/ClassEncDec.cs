using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EncryptFile
{
    [DebuggerNonUserCode]
    class ClassEncDec
    {

        public ClassEncDec()
        {

        }
        public string Decrypt(string textToDecrypt, string key)
        {
            byte[] bytes = null;
            RijndaelManaged managed = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
            byte[] inputBuffer = Convert.FromBase64String(textToDecrypt);
            byte[] sourceArray = Encoding.UTF8.GetBytes(key);
            byte[] destinationArray = new byte[16];
            int length = checked((int)sourceArray.Length);
            if (length > checked((int)destinationArray.Length))
            {
                length = checked((int)destinationArray.Length);
            }
            Array.Copy(sourceArray, destinationArray, length);
            managed.Key = destinationArray;
            managed.IV = destinationArray;
            bytes = managed.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, checked((int)inputBuffer.Length));
            return Encoding.UTF8.GetString(bytes);

        }

        public string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged managed = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            byte[] destinationArray = new byte[16];
            int length = checked((int)bytes.Length);
            if (length > checked((int)destinationArray.Length))
            {
                length = checked((int)destinationArray.Length);
            }
            Array.Copy(bytes, destinationArray, length);
            managed.Key = destinationArray;
            managed.IV = destinationArray;
            ICryptoTransform transform = managed.CreateEncryptor();
            byte[] inputBuffer = Encoding.UTF8.GetBytes(textToEncrypt);
            string Encrypt = Convert.ToBase64String(transform.TransformFinalBlock(inputBuffer, 0, checked((int)inputBuffer.Length)));
            return Encrypt;
        }
    }

    
}

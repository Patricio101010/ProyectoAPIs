using System;
using System.Security.Cryptography;
using System.Text;
using WebApiRepoteria.AL.Interfaz;

namespace WebApiRepoteria.AL.Seguridad
{
    public class EncriptaEN : IEncriptaEN
    {
        public string EncriptarHash(string CadenaOriginal)
        {
            System.Security.Cryptography.HashAlgorithm hashValue = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(CadenaOriginal);
            byte[] byteHash = hashValue.ComputeHash(bytes);
            hashValue.Clear();
            return (Convert.ToBase64String(byteHash));
        }

        public string Desencriptar(string valor)
        {

            // Recibe el rut del usuario sin digito ni guión
            string StrClave;
            try
            {
                StrClave = Decrypt(valor, "DiMeNsIoN");
                return StrClave;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public string Encriptar(string valor)
        {
            // Recibe el rut del usuario sin digito ni guión
            string StrClave;
            try
            {
                StrClave = Encrypt(valor, "DiMeNsIoN");
                return StrClave;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        private string Decrypt(string encryptedText, string password)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] buffer = Convert.FromBase64String(encryptedText);
            byte[] iv = { 0, 0, 0, 0, 0, 0, 0, 0 };
            SymmetricAlgorithm provider = SymmetricAlgorithm.Create("TripleDES");
            byte[] key = new PasswordDeriveBytes(password, iv).CryptDeriveKey("TripleDES", "MD5", provider.KeySize, iv);
            ICryptoTransform decryptor = provider.CreateDecryptor(key, iv);
            try
            {
                return encoding.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                decryptor.Dispose();
            }
        }

        private string Encrypt(string text, string password)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            byte[] iv = { 0, 0, 0, 0, 0, 0, 0, 0 };
            SymmetricAlgorithm provider = SymmetricAlgorithm.Create("TripleDES");
            byte[] key = new PasswordDeriveBytes(password, iv).CryptDeriveKey("TripleDES", "MD5", provider.KeySize, iv);
            ICryptoTransform encryptor = provider.CreateEncryptor(key, iv);

            try
            {
                return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                encryptor.Dispose();
            }
        }
    }
}
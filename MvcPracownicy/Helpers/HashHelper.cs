using System.Security.Cryptography;
using System.Text;

namespace MvcPracownicy.Helpers
{
    public static class HashHelper
    {
        public static string GetMd5Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                 return "";  
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
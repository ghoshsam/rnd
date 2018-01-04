using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleAppWSSEGenerate
{

    public static class HttpClintExtention
    {
        public static void AddWsseHeader(this HttpClient httpClient,string username,string password)
        {
           
            string noiceencode = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Substring(0, 16)));
            string noincedecode = Encoding.UTF8.GetString(Convert.FromBase64String(noiceencode));
            string createdDate = DateTime.UtcNow.ToString("u");
            string digestString = String.Concat(noincedecode, createdDate, password);
            SHA1 sha = SHA1.Create();
            string digest = Convert.ToBase64String(
               sha.ComputeHash(Encoding.UTF8.GetBytes(digestString)));            
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "WSSE profile=\"UsernameToken\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-WSSE", $"UsernameToken Username=\"{username}\", PasswordDigest=\"{digest}\", Nonce=\"{noiceencode}\", Created=\"{createdDate}\"");
        }
    }
    public class WSSEHeader
    {
        private string _username;
        private string _password;
        
       public WSSEHeader(string userName,string password)
        {
            _username = userName;
            _password = password;
        }

        public string GenerateHeader()
        {

            string noice= Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Substring(0,16)));
            string createdDate = DateTime.UtcNow.ToString();
            string digestString = String.Concat(noice, createdDate, _password);
            SHA1 sha = SHA1.Create();
            string digest = Convert.ToBase64String(
               sha.ComputeHash(Encoding.UTF8.GetBytes(digestString)));
            StringBuilder wsseheader = new StringBuilder();
            wsseheader.Append("Authorization: WSSE profile=\"UsernameToken\"\n");
            wsseheader.AppendFormat($"X-WSSE: UsernameToken Username=\"{_username}\", PasswordDigest=\"{digest}\", Nonce=\"{noice}\", Created=\"{createdDate}\"");


            return wsseheader.ToString();
        }
    }
}

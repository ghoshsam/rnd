using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace InSync.WSSEHeader
{
    public static class HttpClintExtention
    {
        public static void AddWsseHeader(this HttpClient httpClient, string username, string password)
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
}

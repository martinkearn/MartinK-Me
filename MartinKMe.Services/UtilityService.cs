using MartinKMe.Domain.Interfaces;
using System;
using System.Text;

namespace MartinKMe.Services
{
    public class UtilityService : IUtilityService
    {
        public string Base64Encode(string input)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string input)
        {
            if (IsBase64String(input))
            {
                var base64EncodedBytes = Convert.FromBase64String(input);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            else
            {
                throw new ArgumentException("Input string was not a valid Base64 string.");
            }
        }

        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}

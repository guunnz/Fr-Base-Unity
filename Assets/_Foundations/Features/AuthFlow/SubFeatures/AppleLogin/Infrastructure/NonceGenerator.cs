using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using static System.Text.Encoding;

namespace AuthFlow.AppleLogin.Infrastructure
{
    public class NonceGenerator : IAppleNonceGenerator
    {
        readonly SHA256Managed sha = new SHA256Managed();
        readonly List<int> randomNumbers = new List<int>(16);
        readonly byte[] randomNumberHolder = new byte[1];
        readonly RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
        const string Charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";


        public string GenerateAppleNonce(int length = 32) =>
            GenerateSHA256NonceFromRawNonce(GenerateRandomString(length));

        string GenerateRandomString(int length)
        {
            if (length <= 0) throw new Exception("Expected nonce to have positive length");

            var result = string.Empty;
            var remainingLength = length;

            randomNumberHolder[0] = 0;

            while (remainingLength > 0)
            {
                randomNumbers.Clear();
                randomNumbers.Capacity = 16;
                for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
                {
                    cryptoServiceProvider.GetBytes(randomNumberHolder);
                    randomNumbers.Add(randomNumberHolder[0]);
                }

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var number in randomNumbers)
                {
                    if (remainingLength == 0) break;
                    if (number >= Charset.Length) continue;
                    result += Charset[number];
                    remainingLength--;
                }
            }

            return result;
        }

        string GenerateSHA256NonceFromRawNonce(string rawNonce)
        {
            const string format = "x2";            
            var utf8RawNonce = UTF8.GetBytes(rawNonce);
            var hash = sha.ComputeHash(utf8RawNonce);
            return hash.Aggregate(string.Empty, (current, t) => current + t.ToString(format));
        }
    }
}
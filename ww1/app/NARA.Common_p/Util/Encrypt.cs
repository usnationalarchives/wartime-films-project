using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Util
{
    /// <summary>
    /// Hnadling of encryption needed for saving user credentials into database
    /// </summary>
    public class Encrypt
    {
         private readonly Encoding _encoding;
        private PaddedBufferedBlockCipher _cipher;

        public Encrypt()
        {
            _encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Encripts plain text with provided key
        /// </summary>
        /// <param name="plain">Plain text</param>
        /// <param name="key">Key for encryption</param>
        /// <returns>Encrypted text</returns>
        public string EncryptS(string plain, string key)
        {
            byte[] result = BouncyCastleCrypto(true, _encoding.GetBytes(plain), key);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decripts encripted text with provided key
        /// </summary>
        /// <param name="plain">Encrypted text</param>
        /// <param name="key">Key for decryption</param>
        /// <returns>Decrypted text</returns>
        public string Decrypt(string cipher, string key)
        {
            byte[] result = BouncyCastleCrypto(false, Convert.FromBase64String(cipher), key);
            return _encoding.GetString(result, 0, result.Length);
        }

        /// <summary>
        /// Encripts input with provided key
        /// </summary>
        /// <param name="forEncrypt">Flag f it goes for encryption</param>
        /// <param name="input">Byte array of the input</param>
        /// <param name="key">Key for de/encryption</param>
        /// <returns>De/Encrypted byte array of input</returns>
        private byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, string key)
        {
            _cipher = new PaddedBufferedBlockCipher(new AesEngine());
            byte[] keyByte = _encoding.GetBytes(key);
            _cipher.Init(forEncrypt, new KeyParameter(keyByte));
            return _cipher.DoFinal(input);
        }
    }
}

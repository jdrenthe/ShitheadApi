namespace ShitheadApi.Services
{
    public interface ICryptoService
    {
        /// <summary>
        /// Encrypts a sting with an key
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keyString"></param>
        /// <returns>Encrypted string</returns>
        string Encrypt(string text, string keyString);

        /// <summary>
        /// Decrypts a sting with an key
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="keyString"></param>
        /// <returns>Decrypted string</returns>
        string Decrypt(string cipherText, string keyString);

        /// <summary>
        /// Encodes a string to a base64
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Base64Encode(string text);

        /// <summary>
        /// Decodes a base64 to a string
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        string Base64Decode(string base64);
    }
}

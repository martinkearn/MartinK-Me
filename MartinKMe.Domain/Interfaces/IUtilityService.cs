namespace MartinKMe.Domain.Interfaces
{
    public interface IUtilityService
    {
        /// <summary>
        /// Decodes a Base64 string to a plain string.
        /// </summary>
        /// <param name="input">The Base64 string to decode.</param>
        /// <returns>Plain version of the string.</returns>
        string Base64Decode(string input);

        /// <summary>
        /// Encodes a plain string to Base64.
        /// </summary>
        /// <param name="input">The plain string to base64 encode.</param>
        /// <returns>Base64 encoded version of the input string.</returns>
        string Base64Encode(string input);
    }
}

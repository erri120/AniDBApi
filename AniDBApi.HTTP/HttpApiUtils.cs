using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace AniDBApi.HTTP
{
    /// <summary>
    /// Contains Utility functions for <see cref="HttpApi"/>.
    /// </summary>
    [PublicAPI]
    public static class HttpApiUtils
    {
        private static XmlReaderSettings _settings = new()
        {
            CheckCharacters = false,
            IgnoreProcessingInstructions = true,
            IgnoreComments = true,
            ValidationType = ValidationType.None
        };

        /// <summary>
        /// Utility function for verifying the result of any of the <see cref="HttpApi"/> functions.
        /// </summary>
        /// <param name="result">Result of an Api function.</param>
        /// <param name="error">Null if return value is false, otherwise it's the error message.</param>
        /// <returns></returns>
        public static bool IsError(string result, out string? error)
        {
            //<error code="302">client version missing or invalid</error>
            error = null;

            using var sr = new StringReader(result);
            using var xmlReader = XmlReader.Create(sr, _settings);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element) continue;
                if (!xmlReader.Name.Equals("error")) continue;

                error = xmlReader.ReadElementContentAsString();
                return true;
            }

            return false;
        }
    }
}


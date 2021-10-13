using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public sealed class UdpApiResult
    {
        public readonly int ReturnCode;
        public readonly string ReturnString;

        internal UdpApiResult(int returnCode, string returnString)
        {
            ReturnCode = returnCode;
            ReturnString = returnString;
        }

        internal static UdpApiResult CreateInternalError(ILogger logger, string message)
        {
            var res = new UdpApiResult(-1, $"INTERNAL ERROR: {message}");
            logger.LogError("{@Error}", res);
            return res;
        }

        public override string ToString()
        {
            return $"{ReturnCode.ToString()} {ReturnString}";
        }
    }
}

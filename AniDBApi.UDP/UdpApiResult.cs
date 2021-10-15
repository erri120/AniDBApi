using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public sealed class UdpApiResult
    {
        public readonly int ReturnCode;
        public readonly string ReturnString;

        internal readonly List<List<string>> InternalLines = new();
        public IReadOnlyList<IReadOnlyList<string>> Lines => InternalLines;

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

        internal static UdpApiResult CreateMissingSessionError(ILogger logger, string commandName)
        {
            return CreateInternalError(logger, $"Command {commandName} requires a session!");
        }

        public override string ToString()
        {
            return InternalLines.Any()
                ? $"{ReturnCode.ToString()} {ReturnString}: {InternalLines.Count.ToString()} lines"
                : $"{ReturnCode.ToString()} {ReturnString}";
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using WacLexer;

namespace WacParser;

internal static partial class TokenParser
{
    private static bool TryPeekTo(Token[] tokens, int position, [NotNullWhen(returnValue: true)] out Token? token)
    {
        if (position >= tokens.Length || position < 0) token = null;
        else token = tokens[position];

        return token is not null;
    }
}
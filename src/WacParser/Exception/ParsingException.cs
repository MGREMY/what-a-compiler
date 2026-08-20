using WacLexer;

namespace WacParser.Exception;

public class ParsingException : System.Exception
{
    public Token? Token { get; }

    public ParsingException(string message, Token? token = null) : base(message)
    {
        Token = token;
    }
}
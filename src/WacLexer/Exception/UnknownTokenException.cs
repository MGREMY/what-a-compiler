namespace WacLexer.Exception;

public class UnknownTokenException : System.Exception
{
    public UnknownTokenException(TokenPosition position, string token)
        : base($"Unknown token (position: {position}, token: {token})")
    {
    }
}
namespace WacLexer.Exception;

public class InvalidTokenException : System.Exception
{
    public InvalidTokenException(TokenPosition position, string token)
        : base($"Invalid token (position: {position}, token: {token})")
    {
    }
}
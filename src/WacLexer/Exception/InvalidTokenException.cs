namespace WacLexer.Exception;

public sealed class InvalidTokenException : System.Exception
{
    public InvalidTokenException(TokenPosition position, string token) : base("Invalid token")
    {
        Data.Add("token", token);
        Data.Add("position", position);
    }
}
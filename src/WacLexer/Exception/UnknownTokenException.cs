namespace WacLexer.Exception;

public sealed class UnknownTokenException : System.Exception
{
    public UnknownTokenException(TokenPosition position, string token) : base("Unknown token")
    {
        Data.Add("token", token);
        Data.Add("position", position);
    }
}
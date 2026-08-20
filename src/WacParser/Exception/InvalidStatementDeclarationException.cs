namespace WacParser.Exception;

public sealed class InvalidStatementDeclarationException : System.Exception
{
    public InvalidStatementDeclarationException(int kind, string token) : base("Invalid statement declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
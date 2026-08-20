namespace WacParser.Exception;

public sealed class InvalidVariableDeclarationException : System.Exception
{
    public InvalidVariableDeclarationException(int kind, string token) : base("Invalid variable declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
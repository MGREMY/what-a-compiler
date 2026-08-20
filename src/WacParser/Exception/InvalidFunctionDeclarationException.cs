namespace WacParser.Exception;

public sealed class InvalidFunctionDeclarationException : System.Exception
{
    public InvalidFunctionDeclarationException(int kind, string token) : base("Invalid function declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
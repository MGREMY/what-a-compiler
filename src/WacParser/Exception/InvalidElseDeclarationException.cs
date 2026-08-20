namespace WacParser.Exception;

public sealed class InvalidElseDeclarationException : System.Exception
{
    public InvalidElseDeclarationException(int kind, string token) : base("Invalid expression declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
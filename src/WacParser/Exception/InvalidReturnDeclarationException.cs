namespace WacParser.Exception;

public sealed class InvalidReturnDeclarationException : System.Exception
{
    public InvalidReturnDeclarationException(int kind, string token) : base("Invalid expression declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
namespace WacParser.Exception;

public sealed class InvalidIfDeclarationException : System.Exception
{
    public InvalidIfDeclarationException(int kind, string token) : base("Invalid expression declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
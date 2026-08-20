namespace WacParser.Exception;

public sealed class InvalidWhileDeclarationException : System.Exception
{
    public InvalidWhileDeclarationException(int kind, string token) : base("Invalid expression declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
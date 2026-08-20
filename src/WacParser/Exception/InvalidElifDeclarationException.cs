namespace WacParser.Exception;

public sealed class InvalidElifDeclarationException : System.Exception
{
    public InvalidElifDeclarationException(int kind, string token) : base("Invalid expression declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
namespace WacParser.Exception;

public sealed class InvalidExpressionDeclarationException : System.Exception
{
    public InvalidExpressionDeclarationException(int kind, string token) : base("Invalid expression declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
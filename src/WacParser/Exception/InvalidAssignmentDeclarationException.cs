namespace WacParser.Exception;

public sealed class InvalidAssignmentDeclarationException : System.Exception
{
    public InvalidAssignmentDeclarationException(int kind, string token) : base("Invalid assignment declaration")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
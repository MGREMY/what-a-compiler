using System.Collections.Concurrent;

namespace WacParser.Exception;

public sealed class UnknownTokenException : System.Exception
{
    public UnknownTokenException(int kind, string token) : base($"Unknown token")
    {
        Data.Add("kind", kind);
        Data.Add("token", token);
    }
}
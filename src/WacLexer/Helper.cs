namespace WacLexer;

public static class Helper
{
    /// <summary>
    /// Get the next character without moving the current position.
    ///
    /// Return '\0' if we reach the end of string.
    /// </summary>
    /// <returns>The next character.</returns>
    public static char Peek(string source, int position)
    {
        return position < source.Length - 1 ? source[position + 1] : '\0';
    }
}
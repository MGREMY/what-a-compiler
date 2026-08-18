namespace WacLexer;

public struct LexerState
{
    public int Position;
    public int Line;
    public int Column;

    public LexerState(int position, int line, int column)
    {
        Position = position;
        Line = line;
        Column = column;
    }
}
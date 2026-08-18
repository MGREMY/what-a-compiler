namespace WacLexer.Tokenizer;

internal class DoubleQuoteTokenizer : ITokenizer
{
    public bool CanTokenize(string source, ref LexerState state)
    {
        return source[state.Position] == '"';
    }

    public Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var position = state.Position;
        var current = source[state.Position];
        var count = 0;

        while (position < source.Length &&
               count < 2)
        {
            if (current == '"') count++;
            if (Helper.IsEscape(current)) position++;

            position++;
            current = source[position];
        }

        var word = source.Substring(start, position - start);

        var token = new Token(TokenKind.StringLiteral, new TokenPosition(state.Line, state.Column), word);

        state.Position = position;
        state.Column += word.Length;

        return token;
    }
}
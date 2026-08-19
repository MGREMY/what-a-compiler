using WacLexer.Tokenizer;

namespace WacLexer;

public class Lexer
{
    private readonly DigitTokenizer _digitTokenizer = new();
    private readonly LetterTokenizer _letterTokenizer = new();
    private readonly DoubleQuoteTokenizer _doubleQuoteTokenizer = new();
    private readonly QuoteTokenizer _quoteTokenizer = new();
    private readonly OperatorTokenizer _operatorTokenizer = new();

    public IEnumerable<Token> Tokenize(string source)
    {
        var state = new LexerState(0, 1, 1);

        while (state.Position < source.Length)
        {
            // WhiteSpace / NewLine character
            if (char.IsWhiteSpace(source[state.Position]))
            {
                // If return line, increment line and reset col
                if (source[state.Position] == '\n')
                {
                    state.Line++;
                    state.Column = 1;
                }
                else
                {
                    state.Column++;
                }

                state.Position++;
                continue;
            }

            if (source[state.Position] == '/')
            {
                if (Helper.Peek(source, state.Position) == '/')
                {
                    state.Position += 2;
                    state.Column += 2;

                    while (state.Position < source.Length && source[state.Position] != '\n')
                    {
                        state.Position++;
                    }

                    state.Position++;
                    state.Line++;
                    state.Column = 1;

                    continue;
                }

                if (Helper.Peek(source, state.Position) == '*')
                {
                    state.Position += 2;
                    state.Column += 2;
                    var commentEnd = false;

                    while (!commentEnd)
                    {
                        while (source[state.Position] != '*')
                        {
                            if (source[state.Position] == '\n')
                            {
                                state.Line++;
                                state.Column = 1;
                            }
                            else
                            {
                                state.Column++;
                            }

                            state.Position++;
                        }

                        if (state.Position >= source.Length || Helper.Peek(source, state.Position) == '/')
                        {
                            commentEnd = true;
                        }

                        state.Column++;
                        state.Position++;
                    }

                    state.Column++;
                    state.Position++;

                    continue;
                }
            }

            if (_letterTokenizer.CanTokenize(source, ref state))
            {
                yield return _letterTokenizer.Tokenize(source, ref state);
                continue;
            }

            if (_digitTokenizer.CanTokenize(source, ref state))
            {
                yield return _digitTokenizer.Tokenize(source, ref state);
                continue;
            }

            if (_doubleQuoteTokenizer.CanTokenize(source, ref state))
            {
                yield return _doubleQuoteTokenizer.Tokenize(source, ref state);
                continue;
            }

            if (_quoteTokenizer.CanTokenize(source, ref state))
            {
                yield return _quoteTokenizer.Tokenize(source, ref state);
                continue;
            }

            if (_operatorTokenizer.CanTokenize(source, ref state))
            {
                yield return _operatorTokenizer.Tokenize(source, ref state);
                continue;
            }
        }

        yield return new Token(TokenKind.Eof, new TokenPosition(state.Line, state.Column), "EOF");
    }
}
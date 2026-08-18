using WacLexer.Tokenizer;

namespace WacLexer;

public class Lexer
{
    private readonly ITokenizer _digitTokenizer = new DigitTokenizer();
    private readonly ITokenizer _letterTokenizer = new LetterTokenizer();
    private readonly ITokenizer _doubleQuoteTokenizer = new DoubleQuoteTokenizer();
    private readonly ITokenizer _quoteTokenizer = new QuoteTokenizer();
    private readonly ITokenizer _operatorTokenizer = new OperatorTokenizer();

    public IEnumerable<Token> Tokenize(string source)
    {
        var state = new LexerState(0, 1, 1);

        while (state.Position < source.Length)
        {
            // WhiteSpace / NewLine character
            if (char.IsWhiteSpace(source[state.Position]))
            {
                // If return line, increment line and reset col
                if (source[state.Position].Equals('\n'))
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

            Token token;

            if (_letterTokenizer.CanTokenize(source, ref state))
            {
                token = _letterTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (_digitTokenizer.CanTokenize(source, ref state))
            {
                token = _digitTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (_doubleQuoteTokenizer.CanTokenize(source, ref state))
            {
                token = _doubleQuoteTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (_quoteTokenizer.CanTokenize(source, ref state))
            {
                token = _quoteTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (_operatorTokenizer.CanTokenize(source, ref state))
            {
                token = _operatorTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }
        }

        yield return new Token(TokenKind.Eof, new TokenPosition(state.Line, state.Column), "EOF");
    }
}
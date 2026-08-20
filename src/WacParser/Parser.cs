using System.Diagnostics.CodeAnalysis;
using WacLexer;
using WacParser.Exception;
using WacParser.Extension;
using WacParser.NodeType;

namespace WacParser;

public class Parser
{
    public ProgramNode Parse(Token[] tokens)
    {
        var state = new ParserState(0);
        var program = new ProgramNode
        {
            Position = 0,
            Functions = [],
        };

        while (state.Position < tokens.Length)
        {
            var function = ParseFunction(tokens, ref state);
            program.Functions.Add(function);
        }

        return program;
    }

    private FunctionNode ParseFunction(Token[] tokens, ref ParserState state)
    {
        FunctionNode functionNode = new();
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.Void:
                case TokenKind.Int:
                case TokenKind.Float:
                case TokenKind.String:
                case TokenKind.Char:
                {
                    var hasPreviousToken = TryPeekTo(tokens, state.Position - 1, out var previousToken);

                    // If not previous token or previous token is not '('
                    // It's the function declaration
                    if (!hasPreviousToken || previousToken!.Kind is not TokenKind.LeftParenthesis)
                    {
                        if (!string.IsNullOrEmpty(functionNode.Type))
                        {
                            throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
                        }

                        functionNode.Type = currentToken.Text;
                        state.Position++;
                    }
                    // Otherwise, it's the variable declaration between '(' and ')'
                    else if (previousToken.Kind is TokenKind.LeftParenthesis)
                    {
                        functionNode.Parameters.Add(ParseVariableDeclaration(tokens, ref state));
                    }

                    break;
                }
                case TokenKind.Id:
                {
                    // If the function name is defined, then having a name here makes no sense
                    if (string.IsNullOrEmpty(functionNode.Type))
                    {
                        throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    functionNode.Name = currentToken.Text;
                    state.Position++;

                    break;
                }
                // '(' and ')' are just delimiters, do nothing and pass them
                case TokenKind.LeftParenthesis:
                case TokenKind.RightParenthesis:
                {
                    state.Position++;

                    break;
                }
                case TokenKind.Comma:
                {
                    // If the function has no name or type, we are not between '(' and ')'
                    // Therefore, having a comma here makes no sense
                    if (string.IsNullOrEmpty(functionNode.Name) ||
                        string.IsNullOrEmpty(functionNode.Type))
                    {
                        throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    state.Position++;

                    break;
                }
                case TokenKind.LeftBrace:
                {
                    if (string.IsNullOrEmpty(functionNode.Name) ||
                        string.IsNullOrEmpty(functionNode.Type))
                    {
                        throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    // functionNode.Block = ParseBlockStatement(tokens, ref state);
                    done = true;

                    break;
                }

                default: throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return functionNode;
    }

    VariableDeclarationStatementNode ParseVariableDeclaration(Token[] tokens, ref ParserState state)
    {
        VariableDeclarationStatementNode variableNode = new();
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.Int:
                case TokenKind.Float:
                case TokenKind.String:
                case TokenKind.Char:
                {
                    // If the variable already have a type, makes no sense to define it again
                    if (!string.IsNullOrEmpty(variableNode.Type))
                    {
                        throw new InvalidVariableDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    variableNode.Type = currentToken.Text;
                    state.Position++;

                    break;
                }
                case TokenKind.Id:
                {
                    // If the variable has no type, makes no sense to declare its name now
                    if (string.IsNullOrEmpty(variableNode.Type))
                    {
                        throw new InvalidVariableDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    variableNode.Name = currentToken.Text;

                    // If next token is not a '=', the declaration is finished
                    if (TryPeekTo(tokens, state.Position + 1, out var nextToken) && nextToken.Kind != TokenKind.Equal)
                    {
                        done = true;
                    }

                    state.Position++;

                    break;
                }
                case TokenKind.Equal:
                {
                    // If we declare the value of the variable, it means we should have a name and a type
                    // Otherwise, the declaration is malformed
                    if (string.IsNullOrEmpty(variableNode.Name) ||
                        string.IsNullOrEmpty(variableNode.Type))
                    {
                        throw new InvalidVariableDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    // variableNode.Initializer = ParseExpression(tokens, ref state);

                    done = true;

                    break;
                }
                default: throw new InvalidVariableDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return variableNode;
    }

    private bool TryPeekTo(Token[] tokens, int position, [NotNullWhen(returnValue: true)] out Token? token)
    {
        if (position >= tokens.Length || position < 0) token = null;
        else token = tokens[position];

        return token is not null;
    }

    private Token[] GetTokenWhile(Token[] tokens, int position, Func<Token, bool> predicate)
    {
        return position >= tokens.Length || position < 0 ? [] : tokens.Skip(position).TakeWhile(predicate).ToArray();
    }
}
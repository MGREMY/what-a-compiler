using System.Diagnostics.CodeAnalysis;
using WacLexer;
using WacParser.Exception;
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
        FunctionNode functionNode = new()
        {
            Position = state.Position,
        };
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
                    // If we hit the begining of block statement without a name or a type
                    // THe declaration is malformed
                    if (string.IsNullOrEmpty(functionNode.Name) ||
                        string.IsNullOrEmpty(functionNode.Type))
                    {
                        throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    // Don't increase position because the block statement starts at the '{'
                    functionNode.Block = ParseBlockStatement(tokens, ref state);

                    break;
                }
                case TokenKind.RightBrace:
                case TokenKind.Eof:
                {
                    if (functionNode.Block is null)
                    {
                        throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    state.Position++;
                    done = true;

                    break;
                }
                default: throw new InvalidFunctionDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? functionNode : throw new InvalidDeclarationException();
    }

    private VariableDeclarationStatementNode ParseVariableDeclaration(Token[] tokens, ref ParserState state)
    {
        VariableDeclarationStatementNode variableNode = new()
        {
            Position = state.Position,
        };
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

                    var assignment = ParseAssignmentDeclaration(tokens, ref state);

                    variableNode.Name = assignment.Name;
                    variableNode.Initializer = assignment.Expression;

                    done = true;

                    break;
                }
                default: throw new InvalidVariableDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? variableNode : throw new InvalidDeclarationException();
    }

    private AssignmentStatementNode ParseAssignmentDeclaration(Token[] tokens, ref ParserState state)
    {
        AssignmentStatementNode assignmentNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.Id:
                {
                    assignmentNode.Name = currentToken.Text;

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
                    if (string.IsNullOrEmpty(assignmentNode.Name))
                    {
                        throw new InvalidVariableDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    // Increate position because the expression starts at the next tokens
                    state.Position++;

                    assignmentNode.Expression = ParseExpression(tokens, ref state);

                    done = true;

                    break;
                }
                default: throw new InvalidAssignmentDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? assignmentNode : throw new InvalidDeclarationException();
    }

    private ExpressionNode ParseExpression(Token[] tokens, ref ParserState state)
    {
        var done = false;
        ExpressionNode? expressionNode = null;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.IntLiteral:
                {
                    expressionNode = new IntLiteralExpressionNode
                    {
                        Position = state.Position,
                        Value = currentToken.Text,
                    };
                    done = true;

                    break;
                }
                case TokenKind.FloatLiteral:
                {
                    expressionNode = new FloatLiteralExpressionNode
                    {
                        Position = state.Position,
                        Value = currentToken.Text,
                    };
                    done = true;

                    break;
                }
                case TokenKind.CharLiteral:
                {
                    expressionNode = new CharLiteralExpressionNode
                    {
                        Position = state.Position,
                        Value = currentToken.Text,
                    };
                    done = true;

                    break;
                }
                case TokenKind.StringLiteral:
                {
                    expressionNode = new StringLiteralExpressionNode
                    {
                        Position = state.Position,
                        Value = currentToken.Text,
                    };
                    done = true;

                    break;
                }
                case TokenKind.Not:
                {
                    state.Position++;

                    expressionNode = new UnaryExpressionNode
                    {
                        Position = state.Position,
                        Operator = currentToken.Text,
                        Operand = ParseExpression(tokens, ref state),
                    };
                    done = true;

                    break;
                }
                case TokenKind.Id:
                {
                    expressionNode = new IdExpressionNode
                    {
                        Position = state.Position,
                        Name = currentToken.Text,
                    };
                    done = true;

                    break;
                }
                default: throw new InvalidExpressionDeclarationException((int)currentToken.Kind, currentToken.Text);
            }

            if (!TryPeekTo(tokens, state.Position + 1, out var nextToken))
            {
                continue;
            }

            // Check for binary expression node of form 'x operator expressionNode'
            if (done &&
                nextToken.Kind
                    is TokenKind.EqualsTo
                    or TokenKind.NotEqualsTo
                    or TokenKind.LessThan
                    or TokenKind.LessOrEqualThan
                    or TokenKind.MoreThan
                    or TokenKind.MoreOrEqualThan)
            {
                // If we create a binary expression, we need to increase state.Position
                // Otherwise the next currentToken will be the current token
                // ANd if we increase it by only 1, it will be the operator token
                // Used in the binary expression
                state.Position += 2;

                expressionNode = new BinaryExpression
                {
                    Position = state.Position,
                    Left = expressionNode,
                    Right = ParseExpression(tokens, ref state),
                    Operator = nextToken.Text,
                };
            }

            // Check for mixed operators
            if (done &&
                nextToken.Kind
                    is TokenKind.Plus
                    or TokenKind.Minus
                    or TokenKind.Star
                    or TokenKind.Slash)
            {
                state.Position += 2;

                expressionNode = new ArithmeticExpressionNode
                {
                    Position = state.Position,
                    Left = expressionNode,
                    Right = ParseExpression(tokens, ref state),
                    Operator = nextToken.Text,
                };
            }
        }

        return done && expressionNode is not null ? expressionNode : throw new InvalidDeclarationException();
    }

    private BlockStatementNode ParseBlockStatement(Token[] tokens, ref ParserState state)
    {
        BlockStatementNode statementNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.LeftBrace:
                {
                    state.Position++;

                    break;
                }
                case TokenKind.RightBrace:
                {
                    state.Position++;
                    done = true;

                    break;
                }
                case TokenKind.SemiColon:
                {
                    state.Position++;

                    break;
                }
                default:
                {
                    statementNode.Statements.Add(ParseStatement(tokens, ref state));

                    break;
                }
            }
        }

        return done ? statementNode : throw new InvalidDeclarationException();
    }

    private StatementNode ParseStatement(Token[] tokens, ref ParserState state)
    {
        StatementNode? statementNode = null;
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
                    statementNode = ParseVariableDeclaration(tokens, ref state);
                    done = true;

                    if (!TryPeekTo(tokens, state.Position + 1, out var nextStatement) ||
                        nextStatement.Kind is not TokenKind.SemiColon)
                    {
                        throw new InvalidStatementDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    state.Position++;

                    break;
                }
                case TokenKind.Id:
                {
                    statementNode = ParseAssignmentDeclaration(tokens, ref state);
                    done = true;

                    if (!TryPeekTo(tokens, state.Position + 1, out var nextStatement) ||
                        nextStatement.Kind is not TokenKind.SemiColon)
                    {
                        throw new InvalidStatementDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    state.Position++;

                    break;
                }
                case TokenKind.Return:
                {
                    statementNode = ParseReturnStatementDeclaration(tokens, ref state);
                    done = true;

                    if (!TryPeekTo(tokens, state.Position + 1, out var nextStatement) ||
                        nextStatement.Kind is not TokenKind.SemiColon)
                    {
                        throw new InvalidStatementDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    state.Position++;

                    break;
                }
                case TokenKind.If:
                {
                    statementNode = ParseIfStatementDeclaration(tokens, ref state);
                    done = true;

                    break;
                }
                case TokenKind.Elif:
                {
                    statementNode = ParseElifStatementDeclaration(tokens, ref state);
                    done = true;

                    break;
                }
                case TokenKind.Else:
                {
                    statementNode = ParseElseStatementDeclaration(tokens, ref state);
                    done = true;

                    break;
                }
                case TokenKind.While:
                {
                    statementNode = ParseWhileStatementDeclaration(tokens, ref state);
                    done = true;

                    break;
                }
                case TokenKind.SemiColon:
                {
                    state.Position++;
                    done = true;

                    break;
                }
                case TokenKind.LeftBrace:
                {
                    statementNode = ParseBlockStatement(tokens, ref state);
                    done = true;

                    break;
                }
                default: throw new InvalidStatementDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done && statementNode is not null ? statementNode : throw new InvalidDeclarationException();
    }

    private IfStatementNode ParseIfStatementDeclaration(Token[] tokens, ref ParserState state)
    {
        IfStatementNode ifStatementNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.If:
                case TokenKind.RightParenthesis:
                {
                    state.Position++;

                    break;
                }
                case TokenKind.LeftParenthesis:
                {
                    state.Position++;

                    ifStatementNode.Condition = ParseExpression(tokens, ref state);
                    state.Position++;

                    break;
                }
                case TokenKind.LeftBrace:
                {
                    // Can be null if something went wrong
                    if (ifStatementNode.Condition is null)
                    {
                        throw new InvalidIfDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    ifStatementNode.Statement = ParseBlockStatement(tokens, ref state);
                    done = true;

                    break;
                }
                default: throw new InvalidIfDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? ifStatementNode : throw new InvalidDeclarationException();
    }

    private ElifStatementNode ParseElifStatementDeclaration(Token[] tokens, ref ParserState state)
    {
        ElifStatementNode elifStatementNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.Elif:
                case TokenKind.RightParenthesis:
                {
                    state.Position++;

                    break;
                }
                case TokenKind.LeftParenthesis:
                {
                    state.Position++;

                    elifStatementNode.Condition = ParseExpression(tokens, ref state);
                    state.Position++;

                    break;
                }
                case TokenKind.LeftBrace:
                {
                    // Can be null if something went wrong
                    if (elifStatementNode.Condition is null)
                    {
                        throw new InvalidIfDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    elifStatementNode.Statement = ParseBlockStatement(tokens, ref state);
                    done = true;

                    break;
                }
                default: throw new InvalidElifDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? elifStatementNode : throw new InvalidDeclarationException();
    }

    private ElseStatementNode ParseElseStatementDeclaration(Token[] tokens, ref ParserState state)
    {
        ElseStatementNode elseStatementNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.Else:
                {
                    state.Position++;

                    break;
                }
                case TokenKind.LeftBrace:
                {
                    elseStatementNode.Statement = ParseBlockStatement(tokens, ref state);
                    done = true;

                    break;
                }
                default: throw new InvalidElseDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? elseStatementNode : throw new InvalidDeclarationException();
    }

    private WhileStatementNode ParseWhileStatementDeclaration(Token[] tokens, ref ParserState state)
    {
        WhileStatementNode whileStatementNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.While:
                case TokenKind.RightParenthesis:
                {
                    state.Position++;

                    break;
                }
                case TokenKind.LeftParenthesis:
                {
                    state.Position++;

                    whileStatementNode.Condition = ParseExpression(tokens, ref state);
                    state.Position++;

                    break;
                }
                case TokenKind.LeftBrace:
                {
                    // Can be null if something went wrong
                    if (whileStatementNode.Condition is null)
                    {
                        throw new InvalidWhileDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    whileStatementNode.Statement = ParseBlockStatement(tokens, ref state);
                    done = true;

                    break;
                }
                default: throw new InvalidWhileDeclarationException((int)currentToken.Kind, currentToken.Text);
            }
        }

        return done ? whileStatementNode : throw new InvalidDeclarationException();
    }

    private ReturnStatementNode ParseReturnStatementDeclaration(Token[] tokens, ref ParserState state)
    {
        ReturnStatementNode returnStatementNode = new()
        {
            Position = state.Position,
        };
        var done = false;

        while (state.Position < tokens.Length && !done)
        {
            var currentToken = tokens[state.Position];

            switch (currentToken.Kind)
            {
                case TokenKind.Return:
                {
                    state.Position++;

                    break;
                }
                default:
                {
                    if (!TryPeekTo(tokens, state.Position - 1, out var previousToken) ||
                        previousToken.Kind is not TokenKind.Return)
                    {
                        throw new InvalidReturnDeclarationException((int)currentToken.Kind, currentToken.Text);
                    }

                    returnStatementNode.Expression = ParseExpression(tokens, ref state);
                    done = true;

                    break;
                }
            }
        }

        return done ? returnStatementNode : throw new InvalidDeclarationException();
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
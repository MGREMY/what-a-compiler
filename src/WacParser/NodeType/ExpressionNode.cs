namespace WacParser.NodeType;

public abstract class ExpressionNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.Expression;
    public int Position { get; }
}

public class BinaryExpression : ExpressionNode
{
    public ExpressionNode Left { get; }
    public ExpressionNode Right { get; }
    public string Operator { get; }
}

public class UnaryExpressionNode : ExpressionNode
{
    public string Operator { get; }
    public ExpressionNode Operand { get; }
}

public class IntLiteralExpressionNode : ExpressionNode
{
    public string Value { get; }
}

public class FloatLiteralExpressionNode : ExpressionNode
{
    public string Value { get; }
}

public class CharLiteralExpressionNode : ExpressionNode
{
    public string Value { get; }
}

public class StringLiteralExpressionNode : ExpressionNode
{
    public string Value { get; }
}

public class IdExpressionNode : ExpressionNode
{
    public string Name { get; }
}
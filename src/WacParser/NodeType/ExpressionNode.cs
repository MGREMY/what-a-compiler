using System.Text.Json.Serialization;

namespace WacParser.NodeType;

[JsonDerivedType(typeof(BinaryExpression), typeDiscriminator: "BinaryExpression")]
[JsonDerivedType(typeof(UnaryExpressionNode), typeDiscriminator: "UnaryExpressionNode")]
[JsonDerivedType(typeof(ArithmeticExpressionNode), typeDiscriminator: "ArithmeticExpressionNode")]
[JsonDerivedType(typeof(CharLiteralExpressionNode), typeDiscriminator: "CharLiteralExpressionNode")]
[JsonDerivedType(typeof(FloatLiteralExpressionNode), typeDiscriminator: "FloatLiteralExpressionNode")]
[JsonDerivedType(typeof(IdExpressionNode), typeDiscriminator: "IdExpressionNode")]
[JsonDerivedType(typeof(IntLiteralExpressionNode), typeDiscriminator: "IntLiteralExpressionNode")]
[JsonDerivedType(typeof(StringLiteralExpressionNode), typeDiscriminator: "StringLiteralExpressionNode")]
public abstract class ExpressionNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.Expression;
    public int Position { get; set; }
}

public class BinaryExpression : ExpressionNode
{
    public ExpressionNode Left { get; set; } = null!;
    public ExpressionNode Right { get; set; } = null!;
    public string Operator { get; set; } = string.Empty;
}

public class UnaryExpressionNode : ExpressionNode
{
    public string Operator { get; set; } = string.Empty;
    public ExpressionNode Operand { get; set; } = null!;
}

public class ArithmeticExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; set; } = null!;
    public ExpressionNode Right { get; set; } = null!;
    public string Operator { get; set; } = string.Empty;
}

public class IntLiteralExpressionNode : ExpressionNode
{
    public string Value { get; set; } = string.Empty;
}

public class FloatLiteralExpressionNode : ExpressionNode
{
    public string Value { get; set; } = string.Empty;
}

public class CharLiteralExpressionNode : ExpressionNode
{
    public string Value { get; set; } = string.Empty;
}

public class StringLiteralExpressionNode : ExpressionNode
{
    public string Value { get; set; } = string.Empty;
}

public class IdExpressionNode : ExpressionNode
{
    public string Name { get; set; } = string.Empty;
}
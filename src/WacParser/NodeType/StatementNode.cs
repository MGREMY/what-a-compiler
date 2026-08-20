using System.Text.Json.Serialization;

namespace WacParser.NodeType;

[JsonDerivedType(typeof(BlockStatementNode), typeDiscriminator: "BlockStatementNode")]
[JsonDerivedType(typeof(VariableDeclarationStatementNode), typeDiscriminator: "VariableDeclarationStatementNode")]
[JsonDerivedType(typeof(AssignmentStatementNode), typeDiscriminator: "AssignmentStatementNode")]
[JsonDerivedType(typeof(IfStatementNode), typeDiscriminator: "IfStatementNode")]
[JsonDerivedType(typeof(ElifStatementNode), typeDiscriminator: "ElifStatementNode")]
[JsonDerivedType(typeof(ElseStatementNode), typeDiscriminator: "ElseStatementNode")]
[JsonDerivedType(typeof(WhileStatementNode), typeDiscriminator: "WhileStatementNode")]
[JsonDerivedType(typeof(ReturnStatementNode), typeDiscriminator: "ReturnStatementNode")]
public abstract class StatementNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.Statement;
    public int Position { get; set; }
}

public class BlockStatementNode : StatementNode
{
    public ICollection<StatementNode> Statements { get; } = [];
}

public class VariableDeclarationStatementNode : StatementNode
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ExpressionNode? Initializer { get; set; } = null;
}

public class AssignmentStatementNode : StatementNode
{
    public string Name { get; set; } = string.Empty;
    public ExpressionNode? Expression { get; set; } = null;
}

public class IfStatementNode : StatementNode
{
    public ExpressionNode Condition { get; set; } = null!;
    public StatementNode Statement { get; set; } = null!;
}

public class ElifStatementNode : StatementNode
{
    public ExpressionNode Condition { get; set; } = null!;
    public StatementNode Statement { get; set; } = null!;
}

public class ElseStatementNode : StatementNode
{
    public StatementNode Statement { get; set; } = null!;
}

public class WhileStatementNode : StatementNode
{
    public ExpressionNode Condition { get; set; } = null!;
    public StatementNode Statement { get; set; } = null!;
}

public class ReturnStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; } = null!;
}
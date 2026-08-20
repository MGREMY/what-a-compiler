using System.Text.Json.Serialization;

namespace WacParser.NodeType;

[JsonDerivedType(typeof(FunctionNode), typeDiscriminator: "FunctionNode")]
[JsonDerivedType(typeof(ProgramNode), typeDiscriminator: "ProgramNode")]
[JsonDerivedType(typeof(ExpressionNode), typeDiscriminator: "ExpressionNode")]
[JsonDerivedType(typeof(StatementNode), typeDiscriminator: "StatementNode")]
public interface IAstNode
{
    int Position { get; set; }
    AstNodeKind Kind { get; }
}

public class ProgramNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.Program;
    public int Position { get; set; }
    public ICollection<FunctionNode> Functions { get; set; } = [];
}

public class FunctionNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.FunctionDeclaration;
    public int Position { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<VariableDeclarationStatementNode> Parameters { get; set; } = [];
    public BlockStatementNode Block { get; set; } = null!;
}
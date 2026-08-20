namespace WacParser.NodeType;

public interface IAstNode
{
    int Position { get; set; }
    AstNodeKind Kind { get; }
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

public class ProgramNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.Program;
    public int Position { get; set; }
    public ICollection<FunctionNode> Functions { get; set; } = [];
}
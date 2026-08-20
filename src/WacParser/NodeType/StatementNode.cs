namespace WacParser.NodeType;

public abstract class StatementNode : IAstNode
{
    public AstNodeKind Kind => AstNodeKind.Statement;
    public int Position { get; }
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
    public string Name { get; }
    public ExpressionNode Expression { get; }
}

public class IfStatementNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public StatementNode Statement { get; }
}

public class ElseStatementNode : StatementNode
{
    public StatementNode Statement { get; }
}

public class WhileStatementNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public StatementNode Statement { get; }
}

public class ReturnStatementNode : StatementNode
{
    public ExpressionNode Expression { get; }
}
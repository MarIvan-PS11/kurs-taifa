namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Инструкция-выражение, выполняемая ради побочных эффектов.
/// </summary>
public sealed class ExpressionStatement : Statement
{
    /// <summary>
    /// Создаёт инструкцию-выражение.
    /// </summary>
    public ExpressionStatement(Expression expression, SourcePosition position)
        : base(position)
    {
        Expression = expression;
    }

    /// <summary>
    /// Выражение, вычисляемое при выполнении инструкции.
    /// </summary>
    public Expression Expression { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
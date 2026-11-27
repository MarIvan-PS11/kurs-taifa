namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Узел AST, представляющий инструкцию возврата из функции.
/// </summary>
public sealed class ReturnStatement : Statement
{
    /// <summary>
    /// Инициализирует новый экземпляр узла возврата.
    /// </summary>
    public ReturnStatement(Expression? value, SourcePosition position)
        : base(position)
    {
        Value = value;
    }

    /// <summary>
    /// Возвращаемое выражение. Может быть null для пустого return.
    /// </summary>
    public Expression? Value { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
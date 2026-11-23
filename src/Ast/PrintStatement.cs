namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Инструкция вывода значения в стандартный поток вывода.
/// </summary>
public sealed class PrintStatement : Statement
{
    /// <summary>
    /// Создаёт инструкцию вывода.
    /// </summary>
    public PrintStatement(Expression value, SourcePosition position)
        : base(position)
    {
        Value = value;
    }

    /// <summary>
    /// Выражение, значение которого выводится.
    /// </summary>
    public Expression Value { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Целочисленный литерал со значением типа int (64-битное знаковое).
/// </summary>
public sealed class IntegerLiteralExpression : Expression
{
    /// <summary>
    /// Создаёт узел целочисленного литерала.
    /// </summary>
    public IntegerLiteralExpression(long value, SourcePosition position)
        : base(position)
    {
        Value = value;
    }

    /// <summary>
    /// Значение литерала.
    /// </summary>
    public long Value { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
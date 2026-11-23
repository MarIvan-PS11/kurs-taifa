namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Унарное выражение: оператор и единственный операнд.
/// </summary>
public sealed class UnaryExpression : Expression
{
    /// <summary>
    /// Создаёт узел унарного выражения.
    /// </summary>
    public UnaryExpression(UnaryOperator op, Expression operand, SourcePosition position)
        : base(position)
    {
        Operator = op;
        Operand = operand;
    }

    /// <summary>
    /// Унарный оператор.
    /// </summary>
    public UnaryOperator Operator { get; }

    /// <summary>
    /// Операнд унарного выражения.
    /// </summary>
    public Expression Operand { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
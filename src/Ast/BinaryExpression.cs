namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Бинарное выражение: левый операнд, оператор и правый операнд.
/// </summary>
public sealed class BinaryExpression : Expression
{
    /// <summary>
    /// Создаёт узел бинарного выражения.
    /// </summary>
    public BinaryExpression(Expression left, BinaryOperator op, Expression right, SourcePosition position)
        : base(position)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    /// <summary>
    /// Левый операнд.
    /// </summary>
    public Expression Left { get; }

    /// <summary>
    /// Бинарный оператор.
    /// </summary>
    public BinaryOperator Operator { get; }

    /// <summary>
    /// Правый операнд.
    /// </summary>
    public Expression Right { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
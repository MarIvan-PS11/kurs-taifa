namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Узел AST, представляющий чтение значения переменной по имени.
/// </summary>
public sealed class IdentifierExpression : Expression
{
    /// <summary>
    /// Инициализирует новый экземпляр узла идентификатора.
    /// </summary>
    public IdentifierExpression(string name, SourcePosition position)
        : base(position)
    {
        Name = name;
    }

    /// <summary>
    /// Имя переменной.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
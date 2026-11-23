namespace MiniSharp.Ast;

using System.Collections.Generic;

using MiniSharp.Lexer;

/// <summary>
/// Вызов функции: имя функции и список выражений-аргументов.
/// Подходит как для встроенных, так и для пользовательских функций.
/// </summary>
public sealed class CallExpression : Expression
{
    /// <summary>
    /// Создаёт узел вызова функции.
    /// </summary>
    public CallExpression(string functionName, IReadOnlyList<Expression> arguments, SourcePosition position)
        : base(position)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }

    /// <summary>
    /// Имя вызываемой функции.
    /// </summary>
    public string FunctionName { get; }

    /// <summary>
    /// Аргументы вызова в порядке их вычисления.
    /// </summary>
    public IReadOnlyList<Expression> Arguments { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}
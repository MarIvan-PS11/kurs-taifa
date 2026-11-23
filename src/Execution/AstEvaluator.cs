namespace MiniSharp.Execution;

using System;
using System.Collections.Generic;

using MiniSharp.Ast;
using MiniSharp.Lexer;

/// <summary>
/// Вычислитель AST со стеком значений. Реализует паттерн Visitor.
/// </summary>
public sealed class AstEvaluator : IAstVisitor
{
    private readonly Stack<long> _values = new();
    private readonly IEnvironment _env;

    /// <summary>
    /// Создаёт вычислитель с указанным окружением ввода-вывода.
    /// </summary>
    public AstEvaluator(IEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// Выполняет программу.
    /// </summary>
    public void Evaluate(ProgramNode program)
    {
        program.Accept(this);
    }

    /// <inheritdoc />
    public void Visit(ProgramNode node)
    {
        node.Main.Accept(this);
    }

    /// <inheritdoc />
    public void Visit(FunctionDeclaration node)
    {
        node.Body.Accept(this);
    }

    /// <inheritdoc />
    public void Visit(BlockStatement node)
    {
        foreach (Statement statement in node.Statements)
        {
            statement.Accept(this);
        }
    }

    /// <inheritdoc />
    public void Visit(PrintStatement node)
    {
        node.Value.Accept(this);
        long value = _values.Pop();
        _env.Print(value);
    }

    /// <inheritdoc />
    public void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);

        if (_values.Count > 0)
        {
            _values.Pop();
        }
    }

    /// <inheritdoc />
    public void Visit(IntegerLiteralExpression node)
    {
        _values.Push(node.Value);
    }

    /// <inheritdoc />
    public void Visit(UnaryExpression node)
    {
        node.Operand.Accept(this);
        long operand = _values.Pop();

        long result = node.Operator switch
        {
            UnaryOperator.Negate => EvaluateChecked(() => checked(-operand), node.Position),
            _ => throw new InterpreterException($"Неизвестный унарный оператор '{node.Operator}'", node.Position),
        };

        _values.Push(result);
    }

    /// <inheritdoc />
    public void Visit(BinaryExpression node)
    {
        node.Left.Accept(this);
        node.Right.Accept(this);

        long right = _values.Pop();
        long left = _values.Pop();

        long result = node.Operator switch
        {
            BinaryOperator.Add => EvaluateChecked(() => checked(left + right), node.Position),
            BinaryOperator.Subtract => EvaluateChecked(() => checked(left - right), node.Position),
            BinaryOperator.Multiply => EvaluateChecked(() => checked(left * right), node.Position),
            BinaryOperator.Divide => EvaluateDivision(left, right, node.Position),
            _ => throw new InterpreterException($"Неизвестный бинарный оператор '{node.Operator}'", node.Position),
        };

        _values.Push(result);
    }

    /// <inheritdoc />
    public void Visit(CallExpression node)
    {
        if (node.FunctionName == "readInt")
        {
            if (node.Arguments.Count != 0)
            {
                throw new InterpreterException(
                    $"Функция 'readInt': Ожидаемое количество аргументов: 0",
                    node.Position);
            }

            _values.Push(_env.ReadInt());
            return;
        }

        long[] args = new long[node.Arguments.Count];
        for (int i = 0; i < node.Arguments.Count; i++)
        {
            node.Arguments[i].Accept(this);
            args[i] = _values.Pop();
        }

        if (!BuiltinImplementations.TryGet(node.FunctionName, out Func<IReadOnlyList<long>, long>? impl) || impl is null)
        {
            throw new InterpreterException($"Неизвестная встроенная функция '{node.FunctionName}'", node.Position);
        }

        long result = EvaluateChecked(() => impl(args), node.Position);
        _values.Push(result);
    }

    /// <summary>
    /// Обрабатывает деление с отдельной диагностикой деления на ноль.
    /// </summary>
    private static long EvaluateDivision(long left, long right, SourcePosition position)
    {
        if (right == 0)
        {
            throw new InterpreterException("Деление на ноль", position);
        }

        return EvaluateChecked(() => checked(left / right), position);
    }

    /// <summary>
    /// Выполняет вычисление и преобразует OverflowException в ошибку выполнения.
    /// </summary>
    private static long EvaluateChecked(Func<long> evaluation, SourcePosition position)
    {
        try
        {
            return evaluation();
        }
        catch (OverflowException ex)
        {
            throw new InterpreterException($"Арифметическое переполнение: {ex.Message}", position);
        }
    }
}
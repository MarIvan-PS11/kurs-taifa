namespace MiniSharp.Execution;

using System;
using System.Collections.Generic;

using MiniSharp.Ast;
using MiniSharp.Lexer;

/// <summary>
/// Вычислитель AST, реализующий паттерн Visitor и управляющий областями видимости.
/// </summary>
public sealed class AstEvaluator : IAstVisitor
{
    private readonly Stack<Dictionary<string, long>> _scopes = new();
    private readonly Dictionary<string, FunctionDeclaration> _functions = new();
    private readonly IEnvironment _env;
    private long _lastExpressionValue;

    /// <summary>
    /// Инициализирует вычислитель с указанным окружением.
    /// </summary>
    public AstEvaluator(IEnvironment env)
    {
        _env = env;
        EnterScope();
    }

    /// <summary>
    /// Запускает вычисление программы.
    /// </summary>
    public void Evaluate(ProgramNode program)
    {
        foreach (FunctionDeclaration func in program.Functions)
        {
            _functions[func.Name] = func;
        }

        try
        {
            program.Accept(this);
        }
        catch (ReturnException)
        {
        }
    }

    /// <inheritdoc />
    public void Visit(ProgramNode node) => node.Main.Accept(this);

    /// <inheritdoc />
    public void Visit(FunctionDeclaration node)
    {
        node.Body.Accept(this);
    }

    /// <inheritdoc />
    public void Visit(BlockStatement node)
    {
        EnterScope();
        try
        {
            foreach (Statement stmt in node.Statements)
            {
                stmt.Accept(this);
            }
        }
        finally
        {
            ExitScope();
        }
    }

    /// <inheritdoc />
    public void Visit(VariableDeclarationStatement node)
    {
        node.Initializer.Accept(this);

        if (_scopes.Peek().ContainsKey(node.Name))
        {
            throw new InterpreterException($"Переменная '{node.Name}' уже объявлена в этой области", node.Position);
        }

        _scopes.Peek()[node.Name] = _lastExpressionValue;
    }

    /// <inheritdoc />
    public void Visit(ReturnStatement node)
    {
        if (node.Value != null)
        {
            node.Value.Accept(this);
            throw new ReturnException(_lastExpressionValue);
        }

        throw new ReturnException(0);
    }

    /// <inheritdoc />
    public void Visit(PrintStatement node)
    {
        node.Value.Accept(this);
        _env.Print(_lastExpressionValue);
    }

    /// <inheritdoc />
    public void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
    }

    /// <inheritdoc />
    public void Visit(IntegerLiteralExpression node)
    {
        _lastExpressionValue = node.Value;
    }

    /// <inheritdoc />
    public void Visit(IdentifierExpression node)
    {
        if (!TryGetValue(node.Name, out long value))
        {
            throw new InterpreterException($"Переменная '{node.Name}' не объявлена", node.Position);
        }

        _lastExpressionValue = value;
    }

    /// <inheritdoc />
    public void Visit(UnaryExpression node)
    {
        node.Operand.Accept(this);
        long operand = _lastExpressionValue;
        _lastExpressionValue = node.Operator switch
        {
            UnaryOperator.Negate => EvaluateChecked(() => checked(-operand), node.Position),
            _ => throw new InterpreterException("Неизвестный унарный оператор", node.Position),
        };
    }

    /// <inheritdoc />
    public void Visit(BinaryExpression node)
    {
        if (node.Operator == BinaryOperator.Assign)
        {
            if (node.Left is not IdentifierExpression id)
            {
                throw new InterpreterException("Левая часть присваивания должна быть идентификатором", node.Position);
            }

            node.Right.Accept(this);
            SetValue(id.Name, _lastExpressionValue, node.Position);
            return;
        }

        node.Left.Accept(this);
        long left = _lastExpressionValue;
        node.Right.Accept(this);
        long right = _lastExpressionValue;

        _lastExpressionValue = node.Operator switch
        {
            BinaryOperator.Add => EvaluateChecked(() => checked(left + right), node.Position),
            BinaryOperator.Subtract => EvaluateChecked(() => checked(left - right), node.Position),
            BinaryOperator.Multiply => EvaluateChecked(() => checked(left * right), node.Position),
            BinaryOperator.Divide => EvaluateDivision(left, right, node.Position),
            _ => throw new InterpreterException("Неизвестный оператор", node.Position),
        };
    }

    /// <inheritdoc />
    public void Visit(CallExpression node)
    {
        if (node.FunctionName == "readInt")
        {
            _lastExpressionValue = _env.ReadInt();
            return;
        }

        if (_functions.TryGetValue(node.FunctionName, out FunctionDeclaration? funcDecl))
        {
            if (node.Arguments.Count != funcDecl.Parameters.Count)
            {
                throw new InterpreterException($"Функция '{node.FunctionName}': неверное число аргументов", node.Position);
            }

            long[] argValues = new long[node.Arguments.Count];
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                node.Arguments[i].Accept(this);
                argValues[i] = _lastExpressionValue;
            }

            EnterScope();
            try
            {
                for (int i = 0; i < funcDecl.Parameters.Count; i++)
                {
                    _scopes.Peek()[funcDecl.Parameters[i]] = argValues[i];
                }

                try
                {
                    funcDecl.Body.Accept(this);
                    _lastExpressionValue = 0;
                }
                catch (ReturnException ex)
                {
                    _lastExpressionValue = ex.Value;
                }
            }
            finally
            {
                ExitScope();
            }

            return;
        }

        long[] args = new long[node.Arguments.Count];
        for (int i = 0; i < node.Arguments.Count; i++)
        {
            node.Arguments[i].Accept(this);
            args[i] = _lastExpressionValue;
        }

        if (!BuiltinImplementations.TryGet(node.FunctionName, out Func<IReadOnlyList<long>, long>? impl) || impl is null)
        {
            throw new InterpreterException($"Неизвестная функция '{node.FunctionName}'", node.Position);
        }

        _lastExpressionValue = EvaluateChecked(() => impl(args), node.Position);
    }

    private void EnterScope() => _scopes.Push(new Dictionary<string, long>());

    private void ExitScope() => _scopes.Pop();

    private void SetValue(string name, long value, SourcePosition position)
    {
        foreach (Dictionary<string, long> scope in _scopes)
        {
            if (scope.ContainsKey(name))
            {
                scope[name] = value;
                return;
            }
        }

        throw new InterpreterException($"Переменная '{name}' не объявлена", position);
    }

    private bool TryGetValue(string name, out long value)
    {
        foreach (Dictionary<string, long> scope in _scopes)
        {
            if (scope.TryGetValue(name, out value))
            {
                return true;
            }
        }

        value = 0;
        return false;
    }

    private static long EvaluateDivision(long left, long right, SourcePosition position)
    {
        if (right == 0)
        {
            throw new InterpreterException("Деление на ноль", position);
        }

        return EvaluateChecked(() => checked(left / right), position);
    }

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
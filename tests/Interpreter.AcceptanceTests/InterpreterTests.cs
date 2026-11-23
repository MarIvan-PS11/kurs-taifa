namespace MiniSharp.Interpreter.AcceptanceTests;

using System;
using System.Collections.Generic;

using MiniSharp.Execution;
using MiniSharp.Interpreter;
using MiniSharp.Lexer;
using MiniSharp.Parser;
using Xunit;

/// <summary>
/// Приёмочные тесты интерпретатора, проверяющие парсер и вычисления.
/// </summary>
public class InterpreterTests
{
    /// <summary>
    /// Тестовые данные для базовой арифметики и приоритета операций.
    /// </summary>
    public static TheoryData<string, string> ArithmeticData => new()
    {
        { "func main() { print 0; }", "0\n" },
        { "func main() { print 42; }", "42\n" },
        { "func main() { print 10 + 5; }", "15\n" },
        { "func main() { print 10 - 5; }", "5\n" },
        { "func main() { print 10 * 5; }", "50\n" },
        { "func main() { print 10 / 5; }", "2\n" },
        { "func main() { print 2 + 3 * 4; }", "14\n" },
        { "func main() { print (2 + 3) * 4; }", "20\n" },
        { "func main() { print 10 / 3; }", "3\n" },
        { "func main() { print -10 / 3; }", "-3\n" },
        { "func main() { print 10 / -3; }", "-3\n" },
        { "func main() { print -10 / -3; }", "3\n" },
        { "func main() { print 1 + 2 + 3 + 4; }", "10\n" },
        { "func main() { print 2 * 3 * 4; }", "24\n" },
        { "func main() { print 10 - 2 - 3; }", "5\n" },
        { "func main() { print 10 / 2 / 5; }", "1\n" },
        { "func main() { print 10 + 5 * 2 - 3; }", "17\n" },
        { "func main() { print (10 + 5) * (2 - 3); }", "-15\n" },
    };

    /// <summary>
    /// Тестовые данные для унарного минуса.
    /// </summary>
    public static TheoryData<string, string> UnaryMinusData => new()
    {
        { "func main() { print -5; }", "-5\n" },
        { "func main() { print --5; }", "5\n" },
        { "func main() { print ---5; }", "-5\n" },
        { "func main() { print -(-5); }", "5\n" },
        { "func main() { print 10 + -5; }", "5\n" },
        { "func main() { print 10 * -2; }", "-20\n" },
        { "func main() { print -2 * -3; }", "6\n" },
        { "func main() { print -(-(-5)); }", "-5\n" },
        { "func main() { print -(-(-(-5))); }", "5\n" },
    };

    /// <summary>
    /// Тестовые данные для встроенных функций.
    /// </summary>
    public static TheoryData<string, string> BuiltinFunctionsData => new()
    {
        { "func main() { print abs(-15); }", "15\n" },
        { "func main() { print abs(15); }", "15\n" },
        { "func main() { print abs(0); }", "0\n" },
        { "func main() { print sqr(5); }", "25\n" },
        { "func main() { print sqr(-5); }", "25\n" },
        { "func main() { print min(10, 20); }", "10\n" },
        { "func main() { print min(20, 10); }", "10\n" },
        { "func main() { print min(10, 10); }", "10\n" },
        { "func main() { print max(10, 20); }", "20\n" },
        { "func main() { print max(20, 10); }", "20\n" },
        { "func main() { print max(10, 10); }", "10\n" },
        { "func main() { print max(abs(-3), sqr(2)); }", "4\n" },
        { "func main() { print min(sqr(3), max(10, 5)); }", "9\n" },
    };

    /// <summary>
    /// Тестовые данные для функции readInt.
    /// </summary>
    public static TheoryData<string, IEnumerable<long>, string> ReadIntData => new()
    {
        { "func main() { print readInt(); }", new long[] { 42 }, "42\n" },
        { "func main() { print readInt() * 2; }", new long[] { 21 }, "42\n" },
        { "func main() { print readInt() + readInt(); }", new long[] { 10, 32 }, "42\n" },
        { "func main() { print abs(readInt()); }", new long[] { -10 }, "10\n" },
        { "func main() { print min(readInt(), readInt()); }", new long[] { 50, 10 }, "10\n" },
    };

    /// <summary>
    /// Проверяет корректное вычисление арифметических выражений.
    /// </summary>
    [Theory]
    [MemberData(nameof(ArithmeticData))]
    public void ArithmeticExpressions_ShouldEvaluateCorrectly(string source, string expectedOutput)
    {
        AssertProgramOutput(source, Array.Empty<long>(), expectedOutput);
    }

    /// <summary>
    /// Проверяет корректную обработку унарного минуса.
    /// </summary>
    [Theory]
    [MemberData(nameof(UnaryMinusData))]
    public void UnaryMinus_ShouldEvaluateCorrectly(string source, string expectedOutput)
    {
        AssertProgramOutput(source, Array.Empty<long>(), expectedOutput);
    }

    /// <summary>
    /// Проверяет корректную работу всех встроенных функций.
    /// </summary>
    [Theory]
    [MemberData(nameof(BuiltinFunctionsData))]
    public void BuiltinFunctions_ShouldEvaluateCorrectly(string source, string expectedOutput)
    {
        AssertProgramOutput(source, Array.Empty<long>(), expectedOutput);
    }

    /// <summary>
    /// Проверяет чтение данных из входного потока.
    /// </summary>
    [Theory]
    [MemberData(nameof(ReadIntData))]
    public void ReadInt_ShouldReadFromInput(string source, IEnumerable<long> input, string expectedOutput)
    {
        AssertProgramOutput(source, input, expectedOutput);
    }

    /// <summary>
    /// Проверяет обработку максимального значения 64-битного целого.
    /// </summary>
    [Fact]
    public void MaxLongValue_ShouldBeParsedAndPrinted()
    {
        string source = "func main() { print 9223372036854775807; }";
        AssertProgramOutput(source, Array.Empty<long>(), "9223372036854775807\n");
    }

    /// <summary>
    /// Проверяет обработку минимального значения 64-битного целого.
    /// </summary>
    [Fact]
    public void MinLongValue_ShouldBeParsedAndPrinted()
    {
        string source = "func main() { print -9223372036854775808; }";
        AssertProgramOutput(source, Array.Empty<long>(), "-9223372036854775808\n");
    }

    /// <summary>
    /// Проверяет, что положительный литерал 2^63 выходит за диапазон int64.
    /// </summary>
    [Fact]
    public void PositiveLiteralBeyondMaxLong_ShouldThrowParserException()
    {
        string source = "func main() { print 9223372036854775808; }";
        ParserException ex = AssertProgramThrows<ParserException>(source, Array.Empty<long>());
        Assert.Contains("выходит за диапазон", ex.Message);
    }

    /// <summary>
    /// Проверяет, что readInt с правильным регистром распознаётся и выполняется.
    /// </summary>
    [Fact]
    public void ReadIntKeywordWithCorrectCase_ShouldWork()
    {
        AssertProgramOutput("func main() { print readInt(); }", new long[] { 7 }, "7\n");
    }

    /// <summary>
    /// Проверяет, что неправильный регистр readInt не распознаётся как ключевое слово.
    /// </summary>
    [Fact]
    public void ReadIntKeywordWithWrongCase_ShouldBeRejected()
    {
        ParserException ex = AssertProgramThrows<ParserException>(
            "func main() { print readint(); }", Array.Empty<long>());
        Assert.Contains("Неизвестная встроенная функция", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при отсутствии ключевого слова func.
    /// </summary>
    [Fact]
    public void MissingFuncKeyword_ShouldThrowParserException()
    {
        string source = "main() { print 1; }";
        AssertProgramThrows<ParserException>(source, Array.Empty<long>());
    }

    /// <summary>
    /// Проверяет выброс исключения при отсутствии ключевого слова main.
    /// </summary>
    [Fact]
    public void MissingMainKeyword_ShouldThrowParserException()
    {
        string source = "func foo() { print 1; }";
        AssertProgramThrows<ParserException>(source, Array.Empty<long>());
    }

    /// <summary>
    /// Проверяет выброс исключения при отсутствии точки с запятой.
    /// </summary>
    [Fact]
    public void MissingSemicolon_ShouldThrowParserException()
    {
        string source = "func main() { print 42 }";
        AssertProgramThrows<ParserException>(source, Array.Empty<long>());
    }

    /// <summary>
    /// Проверяет выброс исключения при несовпадении скобок.
    /// </summary>
    [Fact]
    public void MismatchedParentheses_ShouldThrowParserException()
    {
        string source = "func main() { print (10 + 5; }";
        AssertProgramThrows<ParserException>(source, Array.Empty<long>());
    }

    /// <summary>
    /// Проверяет выброс исключения при неожиданном токене.
    /// </summary>
    [Fact]
    public void UnexpectedToken_ShouldThrowParserException()
    {
        string source = "func main() { print ; }";
        AssertProgramThrows<ParserException>(source, Array.Empty<long>());
    }

    /// <summary>
    /// Проверяет выброс исключения при делении на ноль.
    /// </summary>
    [Fact]
    public void DivisionByZero_ShouldThrowInterpreterException()
    {
        string source = "func main() { print 10 / 0; }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Деление на ноль", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при переполнении сложения.
    /// </summary>
    [Fact]
    public void OverflowOnAddition_ShouldThrowInterpreterException()
    {
        string source = "func main() { print 9223372036854775807 + 1; }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при переполнении умножения.
    /// </summary>
    [Fact]
    public void OverflowOnMultiplication_ShouldThrowInterpreterException()
    {
        string source = "func main() { print 9223372036854775807 * 2; }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при переполнении унарного минуса.
    /// </summary>
    [Fact]
    public void OverflowOnUnaryMinus_ShouldThrowInterpreterException()
    {
        string source = "func main() { print -(-9223372036854775808); }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при вызове неизвестной функции.
    /// </summary>
    [Fact]
    public void UnknownFunction_ShouldThrowParserException()
    {
        string source = "func main() { print foo(1); }";
        ParserException ex = AssertProgramThrows<ParserException>(source, Array.Empty<long>());
        Assert.Contains("Неизвестная встроенная функция", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при неверном количестве аргументов (слишком много).
    /// </summary>
    [Fact]
    public void TooManyArguments_ShouldThrowParserException()
    {
        string source = "func main() { print abs(1, 2); }";
        ParserException ex = AssertProgramThrows<ParserException>(source, Array.Empty<long>());
        Assert.Contains("Ожидаемое количество аргументов: 1", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при неверном количестве аргументов (слишком мало).
    /// </summary>
    [Fact]
    public void TooFewArguments_ShouldThrowParserException()
    {
        string source = "func main() { print min(1); }";
        ParserException ex = AssertProgramThrows<ParserException>(source, Array.Empty<long>());
        Assert.Contains("Ожидаемое количество аргументов: 2", ex.Message);
    }

    /// <summary>
    /// Проверяет выброс исключения при отсутствии ввода для readInt.
    /// </summary>
    [Fact]
    public void MissingReadIntInput_ShouldThrowInterpreterException()
    {
        string source = "func main() { print readInt(); }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Неожиданный конец потока ввода", ex.Message);
    }

    /// <summary>
    /// Проверяет, что литерал больше 2^63 — лексическая ошибка.
    /// </summary>
    [Fact]
    public void LiteralOutOfLongRange_ShouldThrowLexerException()
    {
        string source = "func main() { print 9223372036854775809; }";
        LexerException ex = Assert.Throws<LexerException>(() =>
        {
            Lexer lexer = new Lexer(source);
            lexer.Tokenize();
        });
        Assert.Contains("выходит за диапазон", ex.Message);
    }

    /// <summary>
    /// Проверяет, что цифра(ы) вплотную к букве без разделителя — лексическая ошибка.
    /// </summary>
    [Fact]
    public void LiteralFollowedByLetter_ShouldThrowLexerException()
    {
        string source = "func main() { print 12abc; }";
        LexerException ex = Assert.Throws<LexerException>(() =>
        {
            Lexer lexer = new Lexer(source);
            lexer.Tokenize();
        });
        Assert.Contains("после цифры", ex.Message);
    }

    /// <summary>
    /// Проверяет переполнение при делении long.MinValue на -1.
    /// </summary>
    [Fact]
    public void DivisionOverflowMinValueByMinusOne_ShouldThrowInterpreterException()
    {
        string source = "func main() { print -9223372036854775808 / -1; }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    /// <summary>
    /// Проверяет переполнение в abs при аргументе long.MinValue.
    /// </summary>
    [Fact]
    public void OverflowInAbsFunction_ShouldThrowInterpreterException()
    {
        string source = "func main() { print abs(-9223372036854775808); }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    /// <summary>
    /// Проверяет переполнение в sqr при большом аргументе.
    /// </summary>
    [Fact]
    public void OverflowInSqrFunction_ShouldThrowInterpreterException()
    {
        string source = "func main() { print sqr(3037000500); }";
        InterpreterException ex = AssertProgramThrows<InterpreterException>(source, Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    /// <summary>
    /// Проверяет выполнение программы с пустым блоком инструкций.
    /// </summary>
    [Fact]
    public void EmptyBlock_ShouldProduceNoOutput()
    {
        string source = "func main() { }";
        AssertProgramOutput(source, Array.Empty<long>(), string.Empty);
    }

    /// <summary>
    /// Проверяет инструкцию-выражение без print: вывод остаётся пустым.
    /// </summary>
    [Fact]
    public void ExpressionStatementWithoutPrint_ShouldProduceNoOutput()
    {
        string source = "func main() { 2 + 2; print 7; }";
        AssertProgramOutput(source, Array.Empty<long>(), "7\n");
    }

    /// <summary>
    /// Проверяет, что однострочные комментарии внутри программы игнорируются.
    /// </summary>
    [Fact]
    public void CommentsInsideProgram_ShouldBeIgnored()
    {
        string source = "func main() { // начало\nprint 3 + 4; // конец\n}";
        AssertProgramOutput(source, Array.Empty<long>(), "7\n");
    }

    /// <summary>
    /// Проверяет унарный минус перед вызовом встроенной функции.
    /// </summary>
    [Fact]
    public void UnaryMinusBeforeFunctionCall_ShouldNegateResult()
    {
        string source = "func main() { print -abs(-5); }";
        AssertProgramOutput(source, Array.Empty<long>(), "-5\n");
    }

    /// <summary>
    /// Проверяет глубоко вложенное выражение со скобками и несколькими функциями.
    /// </summary>
    [Fact]
    public void DeeplyNestedExpression_ShouldEvaluateCorrectly()
    {
        string source = "func main() { print ((min(2, 3) + max(1, 4)) * (sqr(2) - abs(-1))); }";
        AssertProgramOutput(source, Array.Empty<long>(), "18\n");
    }

    /// <summary>
    /// Вспомогательный метод для успешного выполнения программы и проверки вывода.
    /// </summary>
    private static void AssertProgramOutput(string source, IEnumerable<long> input, string expectedOutput)
    {
        FakeEnvironment env = new FakeEnvironment(input);
        MiniSharpInterpreter.Run(source, env);
        Assert.Equal(expectedOutput, env.GetOutput());
    }

    /// <summary>
    /// Вспомогательный метод для проверки выброса ожидаемого исключения.
    /// </summary>
    private static TException AssertProgramThrows<TException>(string source, IEnumerable<long> input)
        where TException : Exception
    {
        FakeEnvironment env = new FakeEnvironment(input);
        return Assert.Throws<TException>(() => MiniSharpInterpreter.Run(source, env));
    }
}
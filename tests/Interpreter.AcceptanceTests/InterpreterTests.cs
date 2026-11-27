namespace MiniSharp.Interpreter.AcceptanceTests;

using System;
using System.Collections.Generic;

using MiniSharp.Execution;
using MiniSharp.Lexer;
using MiniSharp.Parser;
using Xunit;

/// <summary>
/// Приёмочные тесты интерпретатора, проверяющие парсер и вычисления.
/// </summary>
public class InterpreterTests
{
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

    public static TheoryData<string, IEnumerable<long>, string> ReadIntData => new()
    {
        { "func main() { print readInt(); }", new long[] { 42 }, "42\n" },
        { "func main() { print readInt() * 2; }", new long[] { 21 }, "42\n" },
        { "func main() { print readInt() + readInt(); }", new long[] { 10, 32 }, "42\n" },
        { "func main() { print abs(readInt()); }", new long[] { -10 }, "10\n" },
        { "func main() { print min(readInt(), readInt()); }", new long[] { 50, 10 }, "10\n" },
    };

    [Theory]
    [MemberData(nameof(ArithmeticData))]
    public void ArithmeticExpressions_ShouldEvaluateCorrectly(string source, string expectedOutput)
    {
        AssertProgramOutput(source, Array.Empty<long>(), expectedOutput);
    }

    [Theory]
    [MemberData(nameof(UnaryMinusData))]
    public void UnaryMinus_ShouldEvaluateCorrectly(string source, string expectedOutput)
    {
        AssertProgramOutput(source, Array.Empty<long>(), expectedOutput);
    }

    [Theory]
    [MemberData(nameof(BuiltinFunctionsData))]
    public void BuiltinFunctions_ShouldEvaluateCorrectly(string source, string expectedOutput)
    {
        AssertProgramOutput(source, Array.Empty<long>(), expectedOutput);
    }

    [Theory]
    [MemberData(nameof(ReadIntData))]
    public void ReadInt_ShouldReadFromInput(string source, IEnumerable<long> input, string expectedOutput)
    {
        AssertProgramOutput(source, input, expectedOutput);
    }

    [Fact]
    public void MaxLongValue_ShouldBeParsedAndPrinted()
    {
        AssertProgramOutput("func main() { print 9223372036854775807; }", Array.Empty<long>(), "9223372036854775807\n");
    }

    [Fact]
    public void MinLongValue_ShouldBeParsedAndPrinted()
    {
        AssertProgramOutput("func main() { print -9223372036854775808; }", Array.Empty<long>(), "-9223372036854775808\n");
    }

    [Fact]
    public void PositiveLiteralBeyondMaxLong_ShouldThrowParserException()
    {
        ParserException ex = AssertProgramThrows<ParserException>("func main() { print 9223372036854775808; }", Array.Empty<long>());
        Assert.Contains("выходит за диапазон", ex.Message);
    }

    [Fact]
    public void ReadIntKeywordWithCorrectCase_ShouldWork()
    {
        AssertProgramOutput("func main() { print readInt(); }", new long[] { 7 }, "7\n");
    }

    [Fact]
    public void ReadIntKeywordWithWrongCase_ShouldBeRejected()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print readint(); }", Array.Empty<long>());
        Assert.Contains("Неизвестная функция 'readint'", ex.Message);
    }

    [Fact]
    public void MissingFuncKeyword_ShouldThrowParserException()
    {
        AssertProgramThrows<ParserException>("main() { print 1; }", Array.Empty<long>());
    }

    [Fact]
    public void MissingMainKeyword_ShouldThrowParserException()
    {
        AssertProgramThrows<ParserException>("func foo() { print 1; }", Array.Empty<long>());
    }

    [Fact]
    public void MissingSemicolon_ShouldThrowParserException()
    {
        AssertProgramThrows<ParserException>("func main() { print 42 }", Array.Empty<long>());
    }

    [Fact]
    public void MismatchedParentheses_ShouldThrowParserException()
    {
        AssertProgramThrows<ParserException>("func main() { print (10 + 5; }", Array.Empty<long>());
    }

    [Fact]
    public void UnexpectedToken_ShouldThrowParserException()
    {
        AssertProgramThrows<ParserException>("func main() { print ; }", Array.Empty<long>());
    }

    [Fact]
    public void DivisionByZero_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print 10 / 0; }", Array.Empty<long>());
        Assert.Contains("Деление на ноль", ex.Message);
    }

    [Fact]
    public void OverflowOnAddition_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print 9223372036854775807 + 1; }", Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    [Fact]
    public void OverflowOnMultiplication_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print 9223372036854775807 * 2; }", Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    [Fact]
    public void OverflowOnUnaryMinus_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print -(-9223372036854775808); }", Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    [Fact]
    public void UnknownFunction_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print foo(1); }", Array.Empty<long>());
        Assert.Contains("Неизвестная функция 'foo'", ex.Message);
    }

    [Fact]
    public void TooManyArguments_ShouldThrowParserException()
    {
        ParserException ex = AssertProgramThrows<ParserException>("func main() { print abs(1, 2); }", Array.Empty<long>());
        Assert.Contains("Ожидаемое количество аргументов: 1", ex.Message);
    }

    [Fact]
    public void TooFewArguments_ShouldThrowParserException()
    {
        ParserException ex = AssertProgramThrows<ParserException>("func main() { print min(1); }", Array.Empty<long>());
        Assert.Contains("Ожидаемое количество аргументов: 2", ex.Message);
    }

    [Fact]
    public void MissingReadIntInput_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print readInt(); }", Array.Empty<long>());
        Assert.Contains("Неожиданный конец потока ввода", ex.Message);
    }

    [Fact]
    public void LiteralOutOfLongRange_ShouldThrowLexerException()
    {
        LexerException ex = Assert.Throws<LexerException>(() => new Lexer("func main() { print 9223372036854775809; }").Tokenize());
        Assert.Contains("выходит за диапазон", ex.Message);
    }

    [Fact]
    public void LiteralFollowedByLetter_ShouldThrowLexerException()
    {
        LexerException ex = Assert.Throws<LexerException>(() => new Lexer("func main() { print 12abc; }").Tokenize());
        Assert.Contains("после цифры", ex.Message);
    }

    [Fact]
    public void DivisionOverflowMinValueByMinusOne_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print -9223372036854775808 / -1; }", Array.Empty<long>());
        Assert.Contains("Арифметическое переполнение", ex.Message);
    }

    [Fact]
    public void EmptyBlock_ShouldProduceNoOutput()
    {
        AssertProgramOutput("func main() { }", Array.Empty<long>(), string.Empty);
    }

    [Fact]
    public void ExpressionStatementWithoutPrint_ShouldProduceNoOutput()
    {
        AssertProgramOutput("func main() { 2 + 2; print 7; }", Array.Empty<long>(), "7\n");
    }

    [Fact]
    public void CommentsInsideProgram_ShouldBeIgnored()
    {
        AssertProgramOutput("func main() { // начало\nprint 3 + 4; // конец\n}", Array.Empty<long>(), "7\n");
    }

    [Fact]
    public void UnaryMinusBeforeFunctionCall_ShouldNegateResult()
    {
        AssertProgramOutput("func main() { print -abs(-5); }", Array.Empty<long>(), "-5\n");
    }

    [Fact]
    public void DeeplyNestedExpression_ShouldEvaluateCorrectly()
    {
        AssertProgramOutput("func main() { print ((min(2, 3) + max(1, 4)) * (sqr(2) - abs(-1))); }", Array.Empty<long>(), "18\n");
    }

    [Fact]
    public void VariableDeclarationAndUsage_ShouldEvaluateCorrectly()
    {
        string source = "func main() { int x = 10; print x + 5; }";
        AssertProgramOutput(source, Array.Empty<long>(), "15\n");
    }

    [Fact]
    public void VariableReassignment_ShouldEvaluateCorrectly()
    {
        string source = "func main() { int x = 10; x = 20; print x; }";
        AssertProgramOutput(source, Array.Empty<long>(), "20\n");
    }

    [Fact]
    public void NestedAssignment_ShouldEvaluateCorrectly()
    {
        string source = "func main() { int a = 0; int b = 0; a = b = 42; print a; print b; }";
        AssertProgramOutput(source, Array.Empty<long>(), "42\n42\n");
    }

    [Fact]
    public void UserFunctionWithParametersAndReturn_ShouldEvaluateCorrectly()
    {
        string source = @"
            func add(a, b) {
                return a + b;
            }
            func main() {
                print add(10, 32);
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "42\n");
    }

    [Fact]
    public void UserFunctionWithoutReturn_ShouldReturnLastExpression()
    {
        string source = @"
            func calc() {
                int x = 4;
                return x * 5;
            }
            func main() {
                print calc();
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "20\n");
    }

    [Fact]
    public void VariableShadowing_ShouldWorkCorrectly()
    {
        string source = @"
            func main() {
                int x = 10;
                {
                    int x = 20;
                    print x;
                }
                print x;
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "20\n10\n");
    }

    [Fact]
    public void FunctionAndVariableSameName_ShouldWorkCorrectly()
    {
        string source = @"
            func foo() {
                return 100;
            }
            func main() {
                int foo = 5;
                print foo;
                print foo();
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "5\n100\n");
    }

    [Fact]
    public void FunctionArgumentsEvaluatedLeftToRight_ShouldWorkCorrectly()
    {
        string source = @"
            func main() {
                int x = 1;
                print max(x = 10, x = 5);
                print x;
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "10\n5\n");
    }

    [Fact]
    public void CircleSquare_ShouldCalculateCorrectly()
    {
        string source = @"
            func main() {
                int r = readInt();
                int pi = 3;
                print pi * r * r;
            }";
        AssertProgramOutput(source, new long[] { 5 }, "75\n");
    }

    [Fact]
    public void RectSquare_ShouldCalculateCorrectly()
    {
        string source = @"
            func main() {
                int width = readInt();
                int height = readInt();
                print width * height;
            }";
        AssertProgramOutput(source, new long[] { 4, 7 }, "28\n");
    }

    [Fact]
    public void UserFunctionInExpression_ShouldEvaluateCorrectly()
    {
        string source = @"
            func double(x) {
                return x * 2;
            }
            func main() {
                print double(5) + double(3);
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "16\n");
    }

    [Fact]
    public void UserFunctionWithoutParameters_ShouldWorkCorrectly()
    {
        string source = @"
            func getAnswer() {
                return 42;
            }
            func main() {
                print getAnswer();
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "42\n");
    }

    [Fact]
    public void LocalVariableInFunction_ShouldNotAffectOuterScope()
    {
        string source = @"
            func modify() {
                int x = 100;
            }
            func main() {
                int x = 5;
                modify();
                print x;
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "5\n");
    }

    [Fact]
    public void MultipleUserFunctionCalls_ShouldWorkCorrectly()
    {
        string source = @"
            func square(x) {
                return x * x;
            }
            func main() {
                print square(2);
                print square(3);
                print square(4);
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "4\n9\n16\n");
    }

    [Fact]
    public void FunctionCallingAnotherUserFunction_ShouldWorkCorrectly()
    {
        string source = @"
            func double(x) {
                return x * 2;
            }
            func quadruple(x) {
                return double(double(x));
            }
            func main() {
                print quadruple(5);
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "20\n");
    }

    [Fact]
    public void ModifyOuterVariableFromNestedBlock_ShouldWorkCorrectly()
    {
        string source = @"
            func main() {
                int x = 10;
                {
                    x = 20;
                }
                print x;
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "20\n");
    }

    [Fact]
    public void FunctionArgumentsEvaluatedInCallerScope_ShouldWorkCorrectly()
    {
        string source = @"
            func calculate(a, b) {
                return a * 10 + b;
            }
            func main() {
                int x = 1;
                print calculate(x, x = 2);
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "12\n");
    }

    [Fact]
    public void ReturnInMain_ShouldTerminateGracefully()
    {
        string source = @"
            func main() {
                print 10;
                return;
                print 20;
            }";
        AssertProgramOutput(source, Array.Empty<long>(), "10\n");
    }

    [Fact]
    public void UndeclaredVariable_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>("func main() { print y; }", Array.Empty<long>());
        Assert.Contains("не объявлена", ex.Message);
    }

    [Fact]
    public void UserFunctionWrongArity_ShouldThrowInterpreterException()
    {
        InterpreterException ex = AssertProgramThrows<InterpreterException>(
            "func foo(a) { return a; } func main() { print foo(1, 2); }", Array.Empty<long>());
        Assert.Contains("неверное число аргументов", ex.Message);
    }

    private static void AssertProgramOutput(string source, IEnumerable<long> input, string expectedOutput)
    {
        FakeEnvironment env = new FakeEnvironment(input);
        MiniSharpInterpreter.Run(source, env);
        Assert.Equal(expectedOutput, env.GetOutput());
    }

    private static TException AssertProgramThrows<TException>(string source, IEnumerable<long> input)
        where TException : Exception
    {
        FakeEnvironment env = new FakeEnvironment(input);
        return Assert.Throws<TException>(() => MiniSharpInterpreter.Run(source, env));
    }
}
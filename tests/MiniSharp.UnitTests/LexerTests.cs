using MiniSharp.Lexer;

using Xunit;

namespace MiniSharp.Tests;

/// <summary>
/// Модульные тесты для лексического анализатора языка MiniSharp.
/// Покрывают все лексемы, граничные случаи и негативные сценарии.
/// </summary>
public class LexerTests
{
    public static TheoryData<string, TokenType, string> SingleCharData => new()
    {
        { "+", TokenType.Plus, "+" },
        { "-", TokenType.Minus, "-" },
        { "*", TokenType.Star, "*" },
        { "/", TokenType.Slash, "/" },
        { "(", TokenType.LeftParen, "(" },
        { ")", TokenType.RightParen, ")" },
        { "{", TokenType.LeftBrace, "{" },
        { "}", TokenType.RightBrace, "}" },
        { "[", TokenType.LeftBracket, "[" },
        { "]", TokenType.RightBracket, "]" },
        { ";", TokenType.Semicolon, ";" },
        { ",", TokenType.Comma, "," },
        { ".", TokenType.Dot, "." },
        { "=", TokenType.Assign, "=" },
        { "!", TokenType.Not, "!" },
        { "<", TokenType.Less, "<" },
        { ">", TokenType.Greater, ">" },
    };

    public static TheoryData<string, TokenType, string> DoubleCharData => new()
    {
        { "==", TokenType.Equal, "==" },
        { "!=", TokenType.NotEqual, "!=" },
        { "<=", TokenType.LessEqual, "<=" },
        { ">=", TokenType.GreaterEqual, ">=" },
        { "&&", TokenType.And, "&&" },
        { "||", TokenType.Or, "||" },
    };

    public static TheoryData<string, TokenType, string> KeywordsData => new()
    {
        { "let", TokenType.Let, "let" },
        { "if", TokenType.If, "if" },
        { "else", TokenType.Else, "else" },
        { "while", TokenType.While, "while" },
        { "func", TokenType.Func, "func" },
        { "return", TokenType.Return, "return" },
        { "print", TokenType.Print, "print" },
        { "read", TokenType.Read, "read" },
        { "true", TokenType.True, "true" },
        { "false", TokenType.False, "false" },
        { "main", TokenType.Main, "main" },
    };

    public static TheoryData<string, TokenType, string> IdentifiersData => new()
    {
        { "x", TokenType.Identifier, "x" },
        { "myVar", TokenType.Identifier, "myVar" },
        { "_hidden", TokenType.Identifier, "_hidden" },
        { "var123", TokenType.Identifier, "var123" },
        { "MAX_SIZE", TokenType.Identifier, "MAX_SIZE" },
        { "Let", TokenType.Identifier, "Let" },
        { "IF", TokenType.Identifier, "IF" },
    };

    public static TheoryData<string, TokenType, string> NumbersData => new()
    {
        { "0", TokenType.IntegerLiteral, "0" },
        { "42", TokenType.IntegerLiteral, "42" },
        { "9999", TokenType.IntegerLiteral, "9999" },
        { "123456789", TokenType.IntegerLiteral, "123456789" },
        { "999999999999999", TokenType.IntegerLiteral, "999999999999999" },
    };

    public static TheoryData<string, TokenType, string> StringsData => new()
    {
        { "\"\"", TokenType.StringLiteral, "\"\"" },
        { "\"a\"", TokenType.StringLiteral, "\"a\"" },
        { "\"hello world\"", TokenType.StringLiteral, "\"hello world\"" },
        { "\"\\n\"", TokenType.StringLiteral, "\"\\n\"" },
        { "\"\\t\"", TokenType.StringLiteral, "\"\\t\"" },
        { "\"\\\"\"", TokenType.StringLiteral, "\"\\\"\"" },
        { "\"\\\\\"", TokenType.StringLiteral, "\"\\\\\"" },
        { "\"line1\\nline2\"", TokenType.StringLiteral, "\"line1\\nline2\"" },
    };

    [Theory]
    [MemberData(nameof(SingleCharData))]
    public void SingleCharTokens_ShouldBeRecognized(string source, TokenType expectedType, string expectedLexeme)
    {
        // Arrange (Используем полное имя MiniSharp.Lexer.Lexer для избежания конфликта CS0118)
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Theory]
    [MemberData(nameof(DoubleCharData))]
    public void DoubleCharTokens_ShouldBeRecognized(string source, TokenType expectedType, string expectedLexeme)
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
    }

    [Theory]
    [MemberData(nameof(KeywordsData))]
    public void Keywords_ShouldBeRecognized(string source, TokenType expectedType, string expectedLexeme)
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
    }

    [Theory]
    [MemberData(nameof(IdentifiersData))]
    public void Identifiers_ShouldBeRecognized(string source, TokenType expectedType, string expectedLexeme)
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
    }

    [Theory]
    [MemberData(nameof(NumbersData))]
    public void Numbers_ShouldBeRecognized(string source, TokenType expectedType, string expectedLexeme)
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
    }

    [Theory]
    [MemberData(nameof(StringsData))]
    public void Strings_ShouldBeRecognized(string source, TokenType expectedType, string expectedLexeme)
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
    }

    [Fact]
    public void NumberFollowedByIdentifier_ShouldBeSeparateTokens()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("42abc");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.Equal("42", tokens[0].Lexeme);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("abc", tokens[1].Lexeme);
    }

    [Fact]
    public void LongString_ShouldBeRecognized()
    {
        // Arrange
        string longText = new string('a', 1000);
        string source = "\"" + longText + "\"";
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(TokenType.StringLiteral, tokens[0].Type);
        Assert.Equal(source, tokens[0].Lexeme);
    }

    [Fact]
    public void TokensWithoutSpaces_ShouldBeSeparatedCorrectly()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("x=5;");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.Assign, tokens[1].Type);
        Assert.Equal(TokenType.IntegerLiteral, tokens[2].Type);
        Assert.Equal(TokenType.Semicolon, tokens[3].Type);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void EmptyInput_ShouldReturnOnlyEof()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("   \t \n \r\n  ")]
    public void VariousWhitespace_ShouldBeIgnored(string source)
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer(source);

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Fact]
    public void SingleComment_ShouldBeIgnored()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("// это просто комментарий");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Fact]
    public void CommentAfterToken_ShouldIgnoreCommentAndKeepToken()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("let // объявление переменной");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Fact]
    public void SimpleAssignment_ShouldTokenizeCorrectly()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("let x = 42;");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(TokenType.Assign, tokens[2].Type);
        Assert.Equal(TokenType.IntegerLiteral, tokens[3].Type);
        Assert.Equal(TokenType.Semicolon, tokens[4].Type);
        Assert.Equal(TokenType.Eof, tokens[5].Type);
    }

    [Fact]
    public void ArithmeticExpression_ShouldTokenizeCorrectly()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("a + b * c - d / e");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(10, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Eof, tokens[9].Type);
    }

    [Fact]
    public void IfElseBlock_ShouldTokenizeCorrectly()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("if (x == 1) { print true; } else { print false; }");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(TokenType.If, tokens[0].Type);
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
        Assert.Equal(TokenType.Eof, tokens[tokens.Count - 1].Type);
    }

    [Fact]
    public void MultilineProgram_ShouldTrackLineNumbers()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("let x = 1;\nlet y = 2;\nprint x;");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(2, tokens[5].Line);
        Assert.Equal(3, tokens[10].Line);
    }

    [Fact]
    public void MultipleOperatorsInRow_ShouldTokenizeSeparately()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("+-*/");

        // Act
        List<Token> tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
        Assert.Equal(TokenType.Star, tokens[2].Type);
        Assert.Equal(TokenType.Slash, tokens[3].Type);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void UnterminatedString_ShouldThrowLexerException()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("\"unclosed string");

        // Act & Assert ((Action) исправляет CS0619, а полное имя класса исправляет CS0118)
        LexerException exception = Assert.Throws<LexerException>((Action)(() => lexer.Tokenize()));
        Assert.Contains("Незакрытая строковая константа", exception.Message);
    }

    [Fact]
    public void InvalidEscapeSequence_ShouldThrowLexerException()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("\"\\x\"");

        // Act & Assert
        LexerException exception = Assert.Throws<LexerException>((Action)(() => lexer.Tokenize()));
        Assert.Contains("Недопустимая escape-последовательность", exception.Message);
    }

    [Fact]
    public void UnknownCharacter_ShouldThrowLexerException()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("@");

        // Act & Assert
        LexerException exception = Assert.Throws<LexerException>((Action)(() => lexer.Tokenize()));
        Assert.Contains("Неизвестный символ", exception.Message);
    }

    [Fact]
    public void SingleAmpersand_ShouldThrowLexerException()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("a & b");

        // Act & Assert
        LexerException exception = Assert.Throws<LexerException>((Action)(() => lexer.Tokenize()));
        Assert.Contains("&&", exception.Message);
    }

    [Fact]
    public void SinglePipe_ShouldThrowLexerException()
    {
        // Arrange
        MiniSharp.Lexer.Lexer lexer = new MiniSharp.Lexer.Lexer("a | b");

        // Act & Assert
        LexerException exception = Assert.Throws<LexerException>((Action)(() => lexer.Tokenize()));
        Assert.Contains("||", exception.Message);
    }
}
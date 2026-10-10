namespace MiniSharp.Lexer;

/// <summary>
/// Вспомогательный класс для быстрого поиска ключевых слов.
/// </summary>
internal static class Keywords
{
    private static readonly Dictionary<string, TokenType> Map = new()
    {
        { "let", TokenType.Let },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "func", TokenType.Func },
        { "return", TokenType.Return },
        { "print", TokenType.Print },
        { "read", TokenType.Read },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "main", TokenType.Main },
    };

    /// <summary>
    /// Пытается получить тип токена для переданного текста.
    /// </summary>
    public static bool TryGetKeyword(string text, out TokenType type)
    {
        return Map.TryGetValue(text, out type);
    }
}
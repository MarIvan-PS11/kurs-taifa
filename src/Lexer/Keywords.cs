namespace MiniSharp.Lexer;

using System.Collections.Generic;

/// <summary>
/// Реестр ключевых слов языка MiniSharp и соответствующих им типов токенов.
/// </summary>
public static class Keywords
{
    /// <summary>
    /// Словарь соответствий между строковым представлением ключевого слова и его типом токена.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, TokenType> Map = new Dictionary<string, TokenType>
    {
        { "func", TokenType.Func },
        { "main", TokenType.Main },
        { "return", TokenType.Return },
        { "print", TokenType.Print },
        { "readInt", TokenType.ReadInt },
        { "int", TokenType.Int },
        { "let", TokenType.Let },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "true", TokenType.True },
        { "false", TokenType.False },
    };

    /// <summary>
    /// Пытается получить тип токена для указанного слова, если оно является ключевым.
    /// </summary>
    public static bool TryGetKeyword(string word, out TokenType tokenType)
    {
        return Map.TryGetValue(word, out tokenType);
    }
}
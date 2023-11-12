namespace Parser;

public class Token
{
    public TokenType TokenType { get; set; }
    public string Lexeme { get; set; }
    public object Literal { get; set; }
    public int Line { get; set; }

    public Token(TokenType tokenType, string lexeme, object literal, int line)
    {
        TokenType = tokenType;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }
}
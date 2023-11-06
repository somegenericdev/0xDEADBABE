using System.Text.RegularExpressions;

namespace Parser;

public class Scanner
{
    public string Source;
    public int Pointer;
    public int MaxPointer;
    public int Line = 0;
    

    public Scanner(string source)
    {
        Source = source;
    }

    public List<Token> Scan()
    {
        Pointer = 0;
        MaxPointer = Source.Length - 1;
        List<Token> tokens = new List<Token>();

        while (Pointer <= MaxPointer)
        {
            char currentChar = Source[Pointer];
            switch (currentChar)
            {
                case '(':
                    tokens.Add(new Token(TokenType.LEFT_PAREN, "(", null, Line));
                    Pointer++;
                    break;
                case ')':
                    tokens.Add(new Token(TokenType.RIGHT_PAREN, ")", null, Line));
                    Pointer++;
                    break;
                case '{':
                    tokens.Add(new Token(TokenType.LEFT_BRACE, "{", null, Line));
                    Pointer++;
                    break;
                case '}':
                    tokens.Add(new Token(TokenType.RIGHT_BRACE, "}", null, Line));
                    Pointer++;
                    break;
                case ',':
                    tokens.Add(new Token(TokenType.COMMA, ",", null, Line));
                    Pointer++;
                    break;
                case '.':
                    tokens.Add(new Token(TokenType.DOT, ".", null, Line));
                    Pointer++;
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.MINUS, "-", null, Line));
                    Pointer++;
                    break;
                case '+':
                    tokens.Add(new Token(TokenType.PLUS, "+", null, Line));
                    Pointer++;
                    break;
                case ';':
                    tokens.Add(new Token(TokenType.SEMICOLON, ";", null, Line));
                    Pointer++;
                    break;
                case '/':
                    tokens.Add(new Token(TokenType.SLASH, "/", null, Line));
                    Pointer++;
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.STAR, "*", null, Line));
                    Pointer++;
                    break;
                case '!':
                    if (Source[Pointer + 1] == '=')
                    {
                        Pointer = Pointer + 2;
                        tokens.Add(new Token(TokenType.BANG_EQUAL, "!=", null, Line));
                    }
                    else
                    {
                        Pointer++;
                        tokens.Add(new Token(TokenType.BANG, "!", null, Line));
                    }

                    break;
                case '=':
                    if (Source[Pointer + 1] == '=')
                    {
                        Pointer = Pointer + 2;
                        tokens.Add(new Token(TokenType.EQUAL_EQUAL, "==", null, Line));
                    }
                    else
                    {
                        Pointer++;
                        tokens.Add(new Token(TokenType.EQUAL, "=", null, Line));
                    }

                    break;
                case '>':
                    if (Source[Pointer + 1] == '=')
                    {
                        Pointer = Pointer + 2;
                        tokens.Add(new Token(TokenType.GREATER_EQUAL, ">=", null, Line));
                    }
                    else
                    {
                        Pointer++;
                        tokens.Add(new Token(TokenType.GREATER, ">", null, Line));
                    }

                    break;
                case '<':
                    if (Source[Pointer + 1] == '=')
                    {
                        Pointer = Pointer + 2;
                        tokens.Add(new Token(TokenType.LESS_EQUAL, "<=", null, Line));
                    }
                    else
                    {
                        Pointer++;
                        tokens.Add(new Token(TokenType.LESS, "<", null, Line));
                    }

                    break;
                case '\r':
                    if (Source[Pointer + 1] == '\n')
                    {
                        Pointer = Pointer + 2;
                        Line++;
                    }
                    else
                    {
                        Pointer++;
                        Line++;
                    }

                    break;
                case '\n':
                    Pointer++;
                    Line++;
                    break;
                case '\t':
                    Pointer++;
                    break;
                case ' ':
                    Pointer++;
                    break;
                case '"':
                    tokens.Add(HandleString());
                    //TODO string literal
                    break;
                default:
                    if (char.IsDigit(currentChar))
                    {
                        tokens.Add(HandleDigit());
                    }
                    else
                    {
                        tokens.Add(HandleIdentifierOrKeyword());
                    }

                    break;
            }
        }

        return tokens;

    }
    
    public Token HandleDigit()
    {
        string lexeme = "";
        
        var currentChar = Source[Pointer];
        while (currentChar.isDigitOrDot())
        {
            lexeme += currentChar;

            Pointer++;
            currentChar = Source[Pointer];
        }

        return new Token(TokenType.NUMBER_LITERAL, lexeme, double.Parse(lexeme), Line);
    }
    
    
    public Token HandleString()
    {
        int startPointer = Pointer;
        while (true)
        {
            Pointer++;
            var currentChar = Source[Pointer];
            if (currentChar == '"')
            {
                Pointer++;
                break;
            }
        }

        var lexeme= Source.Substring(startPointer, Pointer - startPointer);
        var literal = lexeme.Substring(1, lexeme.Length - 2);

        return new Token(TokenType.STRING_LITERAL, lexeme, literal, Line);
    }
    
    

    public Token HandleIdentifierOrKeyword()
    {
        string lexeme = "";
        
        
        var rgx = new Regex("[a-zA-Z0-9_]");
        var currentChar = Source[Pointer];

        while (rgx.IsMatch(currentChar.ToString()))
        {
            lexeme += currentChar;
            Pointer++;
            currentChar = Source[Pointer];
        }

        if (IsReservedKeyword(lexeme))
        {
            return new Token(GetReservedKeywordToken(lexeme), lexeme, null, Line);

        }
        
        return new Token(TokenType.IDENTIFIER, lexeme, null, Line);
    }

    public bool IsReservedKeyword(string str)
    {
        var arr=new string[]
        {
            TokenType.AND.ToString().ToLower(), TokenType.CLASS.ToString().ToLower(), TokenType.ELSE.ToString().ToLower(),
            TokenType.FALSE.ToString().ToLower(), TokenType.FUNCTION.ToString().ToLower(),
            TokenType.FOR.ToString().ToLower(), TokenType.IF.ToString().ToLower(), TokenType.NULL.ToString().ToLower(),
            TokenType.OR.ToString().ToLower(), TokenType.PRINT.ToString().ToLower(),
            TokenType.RETURN.ToString().ToLower(), TokenType.SUPER.ToString().ToLower(),
            TokenType.THIS.ToString().ToLower(), TokenType.TRUE.ToString().ToLower(),
            TokenType.VAL.ToString().ToLower(), TokenType.WHILE.ToString().ToLower(), TokenType.MUT.ToString().ToLower()
        };

        return arr.Contains(str);
    }
    
    public TokenType GetReservedKeywordToken(string str)
    {
        var dic=new Dictionary<string,TokenType>
        {
            {TokenType.AND.ToString().ToLower(),TokenType.AND }, {TokenType.CLASS.ToString().ToLower(),TokenType.CLASS }, {TokenType.ELSE.ToString().ToLower(),TokenType.ELSE },
            {TokenType.FALSE.ToString().ToLower(),TokenType.FALSE }, {TokenType.FUNCTION.ToString().ToLower(),TokenType.FUNCTION },
            {TokenType.FOR.ToString().ToLower(),TokenType.FOR }, {TokenType.IF.ToString().ToLower(),TokenType.IF }, {TokenType.NULL.ToString().ToLower(),TokenType.NULL },
            {TokenType.OR.ToString().ToLower(),TokenType.OR }, {TokenType.PRINT.ToString().ToLower(),TokenType.PRINT },
            {TokenType.RETURN.ToString().ToLower(),TokenType.RETURN }, {TokenType.SUPER.ToString().ToLower(),TokenType.SUPER },
            {TokenType.THIS.ToString().ToLower(),TokenType.THIS }, {TokenType.TRUE.ToString().ToLower(),TokenType.TRUE },
            {TokenType.VAL.ToString().ToLower(),TokenType.VAL }, {TokenType.WHILE.ToString().ToLower(),TokenType.WHILE },{ TokenType.MUT.ToString().ToLower(), TokenType.MUT }
        };

        return dic[str];
    }
    
    
}
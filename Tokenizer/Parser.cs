using System.Linq.Expressions;
using Parser.Exceptions;

namespace Parser;

public class Parser
{
    private List<Token> Tokens;
    private int Current = 0;

    public Parser(List<Token> tokens)
    {
        this.Tokens = tokens;
    }

    public Expr Parse()
    {
        try
        {
            return expression();
        }
        catch (ParseException)
        {
            return null;
        }
    }

    private Expr expression()
    {
        return equality();
    }

    private Expr equality()
    {
        Expr expr = comparison();
        while (match(TokenType.EQUAL_EQUAL) ||
               match(TokenType
                   .BANG_EQUAL)) //if it doesnt encounter an equality operator, it simply returns comparison()
        {
            Token @operator = Tokens[Current - 1];
            Expr right = comparison();
            expr = new Binary(expr, @operator, right);
        }

        return expr;
    }

    //term ( (">" | ">=", | "<" | "<=") term)*
    private Expr comparison()
    {
        Expr expr = term();
        while (match(TokenType.GREATER) || match(TokenType.GREATER_EQUAL) || match(TokenType.LESS) ||
               match(TokenType.LESS_EQUAL)) //if it doesnt encounter a comparison operator, it simply returns term()
        {
            Token @operator = Tokens[Current - 1];
            Expr right = term();
            expr = new Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr term()
    {
        Expr expr = factor();
        while (match(TokenType.MINUS) ||
               match(TokenType.PLUS)) //if it doesnt encounter a + or - sign, it simply returns factor()
        {
            Token @operator = Tokens[Current - 1];
            Expr right = factor();
            expr = new Binary(expr, @operator, right);
        }

        return expr;
    }


    private Expr factor()
    {
        Expr expr = unary();
        while (match(TokenType.STAR) || match(TokenType.SLASH))
        {
            Token @operator = Tokens[Current - 1];
            Expr right = unary();
            expr = new Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr unary()
    {
        if (match(TokenType.BANG) || match(TokenType.MINUS))
        {
            Token @operator = Tokens[Current - 1];
            Expr right = unary();
            return new Unary(@operator, right);
        }

        return primary(); //if it doesnt encounter a ! or a -, it just returns primary()
    }

    private Expr primary()
    {
        if (match(TokenType.FALSE)) return new Literal(false);
        if (match(TokenType.TRUE)) return new Literal(true);
        if (match(TokenType.NULL)) return new Literal(null);
        if (match(TokenType.STRING_LITERAL) || match(TokenType.NUMBER_LITERAL))
        {
            return new Literal(Tokens[Current - 1].Literal);
        }

        if (match(TokenType.LEFT_PAREN))
        {
            Expr expr = expression();
            consume(TokenType.RIGHT_PAREN, "Expected ) after expression.");
            return new Grouping(expr);
        }

        throw error(Tokens[Current], "Invalid token. Expected primary.");
    }


    private bool match(TokenType tokenType)
    {
        if (Tokens[Current].TokenType == tokenType)
        {
            Current++;
            return true;
        }

        return false;
    }

    private Token consume(TokenType tokenType, String message)
    {
        if (Tokens[Current].TokenType == tokenType)
        {
            Current++;
            return Tokens[Current - 1];
        }


        throw error(Tokens[Current], message);
    }

    private ParseException error(Token token, String message)
    {
        Lox.Error(token, message);
        return new ParseException();
    }

    private void synchronize()
    {
        Current++;
        while (Tokens[Current].TokenType != TokenType.EOF)
        {
            if (Tokens[Current - 1].TokenType == TokenType.SEMICOLON)
            {
                return;
            }

            if (new TokenType[]
                {
                    TokenType.CLASS, TokenType.FOR, TokenType.FUNCTION, TokenType.IF, TokenType.PRINT, TokenType.RETURN,
                    TokenType.VAL, TokenType.WHILE, TokenType.MUT
                }.Contains(Tokens[Current].TokenType))
            {
                return;
            }

            Current++;
        }
    }
}
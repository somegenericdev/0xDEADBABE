namespace Parser.Exceptions;

public class RuntimeException:Exception
{
    public Token Token;

    public RuntimeException(Token token, string message):base(message)
    {
        this.Token = token;
    }
}
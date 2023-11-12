using Parser.Exceptions;

namespace Parser;

public class Lox
{
    public static bool HadError;
    public static bool HadRuntimeError;

    public static void Run()
    {
        var source=File.ReadAllText("/home/ubuntu/RiderProjects/0xDEADBABE/Tokenizer/source_code2.0xb");

        var scanner=new Scanner(source);
        var tokens=scanner.Scan();
        Parser parser = new Parser(tokens);
        Expr expr = parser.Parse();
        Interpreter interpreter = new Interpreter();
        interpreter.Interpret(expr);
    }
    public static void Error(Token token, String message)
    {
        if (token.TokenType == TokenType.EOF)
        {
            Report(token.Line, "at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    public static void RuntimeError(RuntimeException exception)
    {
        Console.WriteLine($"{exception.Message}\n[line {exception.Token.Line}]");
        
    }
    
    public static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error ${where}:${message}");
        HadError = true;
        HadRuntimeError = true;
    }
}
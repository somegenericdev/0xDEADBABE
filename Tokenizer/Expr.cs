using System.Text.RegularExpressions;

namespace Parser;

public abstract class Expr
{
    public abstract T accept<T>(Visitor<T> visitor);
}

public class Binary : Expr
{
    public Expr Left;
    public Token Operator;
    public Expr Right;

    public Binary(Expr left, Token @operator, Expr right)
    {
        this.Left = left;
        this.Operator = @operator;
        this.Right = right;
    }

    public override T accept<T>(Visitor<T> visitor)
    {
        return visitor.visitBinary(this);
    }
}

public class Grouping : Expr
{
    public Expr Expression;

    public Grouping(Expr expression)
    {
        this.Expression = expression;
    }

    public override T accept<T>(Visitor<T> visitor)
    {
        return visitor.visitGrouping(this);
    }
}

public class Literal : Expr
{
    public object Value;

    public Literal(object value)
    {
        Value = value;
    }

    public override T accept<T>(Visitor<T> visitor)
    {
        return visitor.visitLiteral(this);
    }
}

public class Unary : Expr
{
    public Token Operator;
    public Expr Right;

    public Unary(Token @operator, Expr right)
    {
        Operator = @operator;
        Right = right;
    }

    public override T accept<T>(Visitor<T> visitor)
    {
        return visitor.visitUnary(this);
    }
}

public interface Visitor<T>
{
    T visitBinary(Binary expr);
    T visitUnary(Unary expr);
    T visitGrouping(Grouping expr);
    T visitLiteral(Literal expr);
}
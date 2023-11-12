using System.Security.AccessControl;
using Parser.Exceptions;

namespace Parser;

public class Interpreter : Visitor<object>
{

    public void Interpret(Expr expr)
    {
        try
        {
            object value = Evaluate(expr);
            Console.WriteLine(value.ToString());
        }
        catch (RuntimeException e)
        {
            Lox.RuntimeError(e);
        }
    }
    public object visitBinary(Binary expr)
    {
        object left=Evaluate(expr.Left);
        object right = Evaluate(expr.Right);
        switch (expr.Operator.TokenType)
        {
            case TokenType.GREATER:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.MINUS:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left.GetType() == typeof(string)&& right.GetType()==typeof(string))
                {
                    return (string)left + (string)right;
                }

                if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                {
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left + (double)right;
                }

                throw new RuntimeException(expr.Operator, "Operands not valid for string concatenation/addition");
                break;
            case TokenType.SLASH:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
        }

        //unreachable
        return null;
    }

    public object visitUnary(Unary expr)
    {
        object right = Evaluate(expr.Right);
        if (expr.Operator.TokenType == TokenType.BANG)
        {
            return !IsTruthy(right);
        }

        if (expr.Operator.TokenType == TokenType.MINUS)
        {
            checkNumberOperand(expr.Operator, right);
            return -(double)right;
        }
        
        //unreachable
        return null;
    }

    public object visitGrouping(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object visitLiteral(Literal expr)
    {
        return expr.Value;
    }

    private object Evaluate(Expr expr)
    {
        return expr.accept(this);
    }

    private void checkNumberOperand(Token @operator, object operand)
    {
        if (operand.GetType() == typeof(double))
        {
            return;
        }

        throw new RuntimeException(@operator, "Operand must be a number");
    }
    
    private void checkNumberOperands(Token @operator, object left, object right)
    {
        if (left.GetType() == typeof(double) && right.GetType()==typeof(double))
        {
            return;
        }

        throw new RuntimeException(@operator, "Operand must be a number");
    }

    private bool IsTruthy(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() == typeof(Boolean))
        {
            return (bool)obj;
        }

        return true;
    }

    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null)
        {
            return false;
        }

        return a.Equals(b);
    }
}
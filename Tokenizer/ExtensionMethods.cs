namespace Parser;

public static class ExtensionMethods
{
    public static  bool isDigitOrDot(this char c)
    {
        return char.IsDigit(c) || c == '.';
    }
    
    
}
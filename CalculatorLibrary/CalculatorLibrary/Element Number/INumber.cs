namespace CalculatorLibrary.Element_Number;

public interface INumber
{
    public double Get();

    public static double operator +(INumber number1, INumber number2)
        => number1.Get() + number2.Get();

    public static double operator -(INumber number1, INumber number2)
        => number1.Get() - number2.Get();

    public static double operator /(INumber number1, INumber number2)
        => number1.Get() / number2.Get();

    public static double operator *(INumber number1, INumber number2)
        => number1.Get() * number2.Get();

}

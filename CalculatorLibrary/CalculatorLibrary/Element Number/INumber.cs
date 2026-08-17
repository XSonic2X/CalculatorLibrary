using System.Runtime.CompilerServices;

namespace CalculatorLibrary.Element_Number;

public interface INumber
{
    public double Get();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static double operator +(INumber number1, INumber number2)
        => number1.Get() + number2.Get();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static double operator -(INumber number1, INumber number2)
        => number1.Get() - number2.Get();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static double operator /(INumber number1, INumber number2)
        => number1.Get() / number2.Get();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static double operator *(INumber number1, INumber number2)
        => number1.Get() * number2.Get();

}

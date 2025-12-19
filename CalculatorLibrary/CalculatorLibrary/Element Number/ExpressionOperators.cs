using System;
using static CalculatorLibrary.Element_Number.ExpressionOperators;

namespace CalculatorLibrary.Element_Number;

public sealed partial class ExpressionOperators(INumber num1, Select select, INumber num2) : INumber
{

    public INumber num1 = num1, num2 = num2;
    public Select select = select;

    public double Get()
        => match[(int)select](num1, num2);

    public override string ToString()
        => toString[(int)select](num1, num2);

    public static readonly Func<INumber, INumber, double>[] match =
        [
        static (num1, num2) => num1 + num2,
        static (num1, num2) => num1 - num2,
        static (num1, num2) => num1 * num2,
        static (num1, num2) => num1 / num2,
        ];

    public static readonly Func<INumber, INumber, string>[] toString =
        [
        static (num1, num2) => $"{num1}+{num2}",
        static (num1, num2) => $"{num1}-{num2}",
        static (num1, num2) => $"{num1}*{num2}",
        static (num1, num2) => $"{num1}/{num2}",
        ];

    public static INumber? Build(INumber a, Select select, INumber? b)
        => b is not null ? new ExpressionOperators(a, select, b) : a;

}
partial class ExpressionOperators
{

    public enum Select: uint
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

}
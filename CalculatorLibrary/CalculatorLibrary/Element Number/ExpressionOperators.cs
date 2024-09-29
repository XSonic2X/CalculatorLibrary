using static CalculatorLibrary.Element_Number.ExpressionOperators;

namespace CalculatorLibrary.Element_Number;

public sealed class ExpressionOperators(INumber num1, Select select, INumber num2) : INumber
{
    public INumber num1 = num1, num2 = num2;
    public Select select = select;

    public double Get()
        => select switch
        {
            Select.Addition => num1 + num2,
            Select.Subtraction => num1 - num2,
            Select.Multiplication => num1 * num2,
            Select.Division => num1 / num2,
        };

    public enum Select
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    public override string ToString()
        => select switch
        {
            Select.Addition => $"{num1}+{num2}",
            Select.Subtraction => $"{num1}-{num2}",
            Select.Multiplication => $"{num1}*{num2}",
            Select.Division => $"{num1}/{num2}",
        };

}

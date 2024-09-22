namespace CalculatorLibrary.Element_Number.Element_operators;

public sealed class Addition(INumber num1, INumber num2) : Operators(num1, num2)
{

    public override double Get() => num1 + num2;

    public override string ToString() => $"{num1}+{num2}";
}

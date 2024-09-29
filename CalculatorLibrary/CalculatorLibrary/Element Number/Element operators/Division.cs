namespace CalculatorLibrary.Element_Number.Element_operators;

public sealed class Division(INumber num1, INumber num2) : ExpressionOperators(num1, num2)
{

    public override double Get() => num1 / num2;

    public override string ToString() => $"{num1}/{num2}";
}

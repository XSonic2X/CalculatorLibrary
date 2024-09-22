namespace CalculatorLibrary.Element_Number;

public sealed class Number(double values) : INumber
{

    public double values = values;

    public double Get() => values;

    public override string ToString() => values.ToString();
}

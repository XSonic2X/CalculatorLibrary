namespace CalculatorLibrary.Element_Number;

public sealed class Negative(INumber number) : INumber
{

    public INumber number = number;

    public double Get() => -number.Get();

    public override string ToString() => $"-{number}";

}

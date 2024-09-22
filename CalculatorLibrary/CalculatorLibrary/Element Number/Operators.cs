namespace CalculatorLibrary.Element_Number;

public abstract class Operators(INumber num1, INumber num2) : INumber
{
    public INumber num1 = num1, num2 = num2;

    public abstract double Get();

}

# CalculatorLibrary
Solves mathematical problems found in strings.

## Test display

### 1
```csharp
public static void Main1()
{
    Mathematics mathematics = new();
    Demo(mathematics, "5-3-2");
    Demo(mathematics, "3+2-2*2+1+5-3-2");
    Demo(mathematics, "5*2+5");
    Demo(mathematics, "5+5*2");
    Demo(mathematics, "5+5*(2+2)");
    Demo(mathematics, "-2+(5+5)*2+1");
    Demo(mathematics, "2*5+5*4");
    Demo(mathematics, "100/2/2/2");
    Console.WriteLine("End");
    Console.ReadLine();
}

static void Demo(Mathematics m, string txt)
{
    if(m.GetNumber(txt, out INumber? number))
        Console.WriteLine($"{number} = {number.Get()}");
}
```
### Output:
```
5-3-2 = 0
3+2-2*2+1+5-3-2 = 2
5*2+5 = 15
5+5*2 = 15
5+5*(2+2) = 25
-2+(5+5)*2+1 = 19
2*5+5*4 = 30
100/2/2/2 = 12,5
End
```

### 2
```csharp
public static void Main1()
{
    
    Dictionary<string, Mathematics.BuilderNumber> keyValues = [];
    keyValues.Add("^", new BuilderDegree());
    Mathematics mathematics = new(keyValues, @"[0-9]*\.?[0-9]+([0-9]+)?|[\^()+*-/]");
    Demo(mathematics, "2-1*(2^3)+1");
    Demo(mathematics, "2-1+2^3*1");
    Console.WriteLine("End");
    Console.ReadLine();
}
static void Demo(Mathematics m, string txt)
{
    if (m.GetNumber(txt, out INumber? number))
        Console.WriteLine($"{number} = {number.Get()}");
}
public class BuilderDegree : Mathematics.BuilderNumber
{

    public override INumber? Get(INumber? number)
    {
        if (number is null) throw new FormatException($"Invalid number format: {txt}");
        return new Degree(number, Level2());
    }

}

public class Degree(INumber num1, INumber num2) : INumber
{

    public INumber num1 = num1, num2 = num2;

    public double Get()
        => Math.Pow(num1.Get(), num2.Get());

    public override string ToString()
        => $"{num1}^{num2}";

}
```
### Output:
```
2-1*(2^3)+1 = -5
2-1+2^3*1 = 9
End
```

### 3
```csharp
static void Main()
{
    DynamicNumber d = new DynamicNumber();

    Dictionary<string, BuilderNumber> keyValues = [];

    keyValues.Add("[CustomTest]", new CustomBuilderNumber(d));

    Mathematics mathematics = new(keyValues, @"\[(.*?)\]|[0-9]*\.?[0-9]+([0-9]+)?|[()+*-/]");
    if (mathematics.GetNumber("[CustomTest]*0.5", out INumber? number))
    {
        Action a = () => Console.WriteLine($"{number} = {number.Get()}");
        for (int i = 0; i < 5; i++)
            d.Test(a);
    }


    Console.WriteLine("End");
    Console.ReadLine();
}
	
class DynamicNumber
{
    public int i = 0;

    public void Test(Action action)
    {
        i++;
        action();
    }
}

class NumberCustom(DynamicNumber d) : INumber
{

    private DynamicNumber d = d;

    public double Get()
        => d.i;

    public override string ToString()
        => d.i.ToString();

}

class CustomBuilderNumber(DynamicNumber d) : BuilderNumber
{
    private DynamicNumber d = d;

    public override INumber? Get()
    {
        Next();
        return new NumberCustom(d);
    }

}
```
### Output:
```
1*0,5 = 0,5
2*0,5 = 1
3*0,5 = 1,5
4*0,5 = 2
5*0,5 = 2,5
End
```

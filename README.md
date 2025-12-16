# CalculatorLibrary
Solves mathematical problems found in strings.

## Test display

### 1
```csharp
static void Main()
{
    Mathematics mathematics = new();
    Demo(mathematics, "5*2+5");
    Demo(mathematics, "5+5*2");
    Demo(mathematics, "5+5*(2+2)");
    Demo(mathematics, "-2+(5+5)*2+1");
    Demo(mathematics, "2*5+5*4");
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
5*2+5 = 15
5+5*2 = 15
5+5*(2+2) = 25
-2+(5+5)*2+1 = 19
End
```

### 2
```csharp
static void Main()
{
    DynamicNumber d = new DynamicNumber();

    Dictionary<string, BuilderNumber> keyValues = [];

    keyValues.Add("[CastomTest]", new CastomBuilderNumber(d));

    Mathematics mathematics = new(keyValues, @"\[(.*?)\]|[0-9]*\.?[0-9]+([0-9]+)?|[()+*-/]");
    if (mathematics.GetNumber("[CastomTest]*0.5", out INumber? number))
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

class NumberCastom(DynamicNumber d) : INumber
{

    private DynamicNumber d = d;

    public double Get()
        => d.i;

    public override string ToString()
        => d.i.ToString();

}

class CastomBuilderNumber(DynamicNumber d) : BuilderNumber
{
    private DynamicNumber d = d;

    public override INumber? Get()
    {
        Next();
        return new NumberCastom(d);
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

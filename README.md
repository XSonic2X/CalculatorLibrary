# CalculatorLibrary
Solves mathematical problems found in strings.

##Test display

###1
```csharp
static void Main()
{
    Mathematics mathematics = new();
    MatchTest(mathematics, "5*2+5");
    MatchTest(mathematics, "5+5*2");
    MatchTest(mathematics, "5+5*(2+2)");
    MatchTest(mathematics, "-2+(5+5)*2+1");
    Console.WriteLine("End");
    Console.ReadLine();
}

static void MatchTest(Mathematics m, string txt)
{
    INumber number = m.GetNumber(txt);
    Console.WriteLine($"{number} = {number.Get()}");
}
```
###Output:
```
5*2+5 = 15
5+5*2 = 15
5+5*(2+2) = 25
-2+(5+5)*2+1 = 19
End
```

###2
```csharp
static void Main()
{
    DynamicNumber d = new DynamicNumber();

    Dictionary<string, BuilderNumber> keyValues = [];

    keyValues.Add("[CastomTest]", new CastomBuilderNumber(d));

    Mathematics mathematics = new(keyValues, @"\[(.*?)\]|[0-9]*\.?[0-9]+([0-9]+)?|[()+*-/]");
    INumber number = mathematics.GetNumber("[CastomTest]*0.5");
    for (int i = 0; i < 5; i++)
        d.Test(()=> MatchTest(mathematics, number));


    Console.WriteLine("End");
    Console.ReadLine();
}

static void MatchTest(Mathematics m, INumber number)
    => Console.WriteLine($"{number} = {number.Get()}");
	
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
        => $"{d.i}";
}

class CastomBuilderNumber(DynamicNumber d) : BuilderNumber
{
    private DynamicNumber d = d;
    public override INumber Get()
    {
        Next();
        return new NumberCastom(d);
    }
}
```
###Output:
```
1*0,5 = 0,5
2*0,5 = 1
3*0,5 = 1,5
4*0,5 = 2
5*0,5 = 2,5
End
```

using CalculatorLibrary.Element_Number;
using CalculatorLibrary.Element_Number.Element_operators;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CalculatorLibrary;

public class Mathematics
{
    public Mathematics()
        => Initialization();

    public Mathematics(Dictionary<string, IBuilderNumber> keyValues, string regexp)
    {
        _regexp = regexp;
        _keyValues = keyValues;
        Initialization();
    }

    private void Initialization()
    {
        _regex = new Regex(_regexp);
        _keyValues ??= [];
        _keyValues.Add("-", new NegativeBuilder(this));
        _keyValues.Add("(", new StaplesBuilder(this));
    }

    static private string _regexp = @"[0-9]*\.?[0-9]+([0-9]+)?|[()+*-/]";

    public Dictionary<string, IBuilderNumber> _keyValues;
    private string _txt = string.Empty;
    private int _index;
    private Regex _regex;
    private MatchCollection _matches;

    public INumber GetNumber(string txt)
    {
        _matches = _regex.Matches(txt);
        _index = 0;
        return Level1();
    }

    private INumber OperatorBuilderLv1(INumber number)
        => _txt switch
        {
            "+" => new Addition(number, Level1()),
            "-" => new Subtraction(number, Level1()),
            _ => number
        };

    private INumber OperatorBuilderLv2(INumber number)
        => _txt switch
        {
            "*" => new Multiplication(number, Level2()),
            "/" => new Division(number, Level2()),
            _ => number
        };

    private INumber Level1()
        => OperatorBuilderLv1(OperatorBuilderLv2(Level2()));

    private INumber Level2()
    {
        Next();
        if (_txt == string.Empty) return null;
        return Level3() ?? CreateNumber();
    }

    private INumber Level3()
        => _keyValues.TryGetValue(_txt, out IBuilderNumber? value) ? value.Get() : null;

    private INumber CreateNumber()
    {
        INumber number;
        try
        {
            number = new Number(Convert.ToDouble(_txt));
            Next();
        }
        catch
        (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return number;
    }

    private void Next()
        => _txt = _index < _matches.Count ?
        _matches[_index++].Value :
        string.Empty;

    private class NegativeBuilder(Mathematics mathematics) : BuilderNumber(mathematics)
    {
        public override INumber Get()
            => new Negative(Level2());
    }

    private class StaplesBuilder(Mathematics mathematics) : BuilderNumber(mathematics)
    {

        public override INumber Get()
        {
            INumber number = new Staples(Level1());
            if (mathematics._txt is not ")") return null;
            Next();
            return number;
        }
    }

    public abstract class BuilderNumber(Mathematics mathematics) : IBuilderNumber
    {
        protected Mathematics mathematics = mathematics;

        public INumber Level1()
            => mathematics.Level1();

        public INumber Level2()
            => mathematics.Level2();

        public void Next()
            => mathematics.Next();

        public abstract INumber Get();
    }

}

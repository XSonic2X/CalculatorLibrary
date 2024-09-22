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

    private INumber OperatorBuilder(INumber number)
        => _txt switch
        {
            "+" => new Addition(number, Level1()),
            "-" => new Subtraction(number, Level1()),
            "*" => new Multiplication(number, Level1()),
            "/" => new Division(number, Level1()),
            _=> number
        };

    private INumber Level1()
    {
        INumber number = Level2();
        if (number is null) return number;
        return OperatorBuilder(number);
    }

    private INumber Level2()
    {
        Next();
        if (_txt == string.Empty) return null;
        INumber number = Level3();
        number ??= CreateNumber();
        return number;
    }

    private INumber Level3()
    {
        if (_keyValues.TryGetValue(_txt, out IBuilderNumber? value))
            return value.Get();
        return null;
    }

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
        => _txt = _index < _matches.Count ? _matches[_index++].Value : string.Empty;

    private class NegativeBuilder(Mathematics mathematics) : IBuilderNumber
    {
        public Mathematics mathematics = mathematics;

        public INumber Get()
        {
            return new Negative(mathematics.Level2());
        }
    }

    private class StaplesBuilder(Mathematics mathematics) : IBuilderNumber
    {

        public Mathematics mathematics = mathematics;

        public virtual INumber Get()
        {
            INumber number = new Staples(mathematics.Level1());
            if (mathematics._txt is not ")") return null;
            mathematics.Next();
            return number;
        }
    }

}

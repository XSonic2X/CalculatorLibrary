using CalculatorLibrary.Element_Number;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CalculatorLibrary;

public class Mathematics
{
    public Mathematics()
    {
        _regexP = @"[0-9]*\.?[0-9]+([0-9]+)?|[()+*-/]";
        Initialization();
    }

    public Mathematics(Dictionary<string, BuilderNumber> keyValues, string regexp)
    {
        _regexP = regexp;
        _keyValues = keyValues;
        Initialization();
    }

    private void Initialization()
    {
        _regex = new Regex(_regexP);
        _keyValues ??= [];
        _keyValues.Add("-", new NegativeBuilder());
        _keyValues.Add("(", new StaplesBuilder());
        foreach (var key in _keyValues)
            BuilderNumber.Initialization(key.Value,this);
    }

    public Dictionary<string, BuilderNumber> _keyValues;

    private readonly string _regexP;
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
            "+" => new ExpressionOperators(number, ExpressionOperators.Select.Addition, Level1()),
            "-" => new ExpressionOperators(number, ExpressionOperators.Select.Subtraction, Level1()),
            _ => number
        };

    private INumber OperatorBuilderLv2(INumber number)
        => _txt switch
        {
            "*" => new ExpressionOperators(number, ExpressionOperators.Select.Multiplication, Level2()),
            "/" => new ExpressionOperators(number, ExpressionOperators.Select.Division, Level2()),
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
        => _keyValues.TryGetValue(_txt, out BuilderNumber? value) ? value.Get() : null;

    private INumber CreateNumber()
    {
        INumber number;
        try
        {
            number = new Number(double.Parse(_txt, CultureInfo.InvariantCulture));
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

    public override string ToString()
        => $"Mathematics regex pattern {_regexP}";


    private sealed class NegativeBuilder : BuilderNumber
    {

        public override INumber Get()
            => new Negative(Level2());
    }

    private sealed class StaplesBuilder : BuilderNumber
    {

        public override INumber Get()
        {
            INumber number = new Staples(Level1());
            if (txt is not ")") return null;
            Next();
            return number;
        }
    }

    /// <summary>
    /// To create custom solutions
    /// </summary>
    /// <param name="mathematics"></param>
    public abstract class BuilderNumber
    {

        protected Mathematics mathematics;

        protected string txt { get => mathematics._txt; }

        public static void Initialization(BuilderNumber BN, Mathematics m)
            => BN.mathematics = m;

        public INumber Level1()
            => mathematics.Level1();

        public INumber Level2()
            => mathematics.Level2();

        public void Next()
            => mathematics.Next();

        public abstract INumber Get();
    }

}

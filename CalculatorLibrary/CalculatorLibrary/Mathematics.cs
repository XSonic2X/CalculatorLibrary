using CalculatorLibrary.Element_Number;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CalculatorLibrary;

public partial class Mathematics
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

    public bool GetNumber(string txt, out INumber? number)
    {
        _matches = _regex.Matches(txt);
        _index = 0;
        number = Level1();
        return number is not null;
    }

    private INumber? OperatorBuilder(INumber? number)
        => number is not null? _txt switch
        {
            "+" => ExpressionOperators.Build(number, ExpressionOperators.Select.Addition, Level1()),
            "-" => ExpressionOperators.Build(number, ExpressionOperators.Select.Subtraction, Level1()),
            "*" => ExpressionOperators.Build(number, ExpressionOperators.Select.Multiplication, Level2()),
            "/" => ExpressionOperators.Build(number, ExpressionOperators.Select.Division, Level2()),
            _ => number
        } : null;


    private INumber? Level1()
        => OperatorBuilder(OperatorBuilder(Level2()));

    private INumber? Level2()
    {
        Next();
        return _txt == string.Empty? null:
            _keyValues.TryGetValue(_txt, out BuilderNumber? value) ? value.Get() :
            CreateNumber();
    }

    private INumber CreateNumber()
    {
        if (double.TryParse(_txt, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            Next();
            return new Number(value);
        }
        throw new FormatException($"Invalid number format: {_txt}");
    }

    private void Next()
        => _txt = _index < _matches.Count ?
        _matches[_index++].Value :
        string.Empty;

    public override string ToString()
        => $"Mathematics regex pattern {_regexP}";




}

partial class Mathematics
{
    private sealed class NegativeBuilder : BuilderNumber
    {

        public override INumber? Get()
        {
            INumber? number = Level2();
            return number is not null ? new Negative(number): null;
        }

    }

    private sealed class StaplesBuilder : BuilderNumber
    {

        public override INumber? Get()
        {
            INumber? number = Level1();
            if (number is null) return null;
            number = new Staples(number);
            if (txt is not ")") throw new InvalidOperationException("Expected closing parenthesis");
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

        protected string txt { get => mathematics._txt; }

        private Mathematics mathematics;

        public static void Initialization(BuilderNumber BN, Mathematics m)
            => BN.mathematics = m;

        protected INumber? Level1()
            => mathematics.Level1();

        protected INumber? Level2()
            => mathematics.Level2();

        protected void Next()
            => mathematics.Next();

        public abstract INumber? Get();

    }

}
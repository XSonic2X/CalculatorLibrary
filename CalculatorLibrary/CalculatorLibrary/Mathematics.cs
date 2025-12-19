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
        _keyValues.Add("(", new ParenthesesBuilder());
        foreach (var key in _keyValues)
            BuilderNumber.Initialization(key.Value, this);
    }

    private readonly string _regexP;
    private string _txt = string.Empty;
    private int _index;

    private Dictionary<string, BuilderNumber> _keyValues;
    private Regex _regex;
    private MatchCollection _matches;

    public bool GetNumber(string txt, out INumber? number)
    {
        _matches = _regex.Matches(txt);
        _index = 0;
        number = Level1();
        return number is not null;
    }

    public bool GetOperator(out ExpressionOperators.Select? select)
    {
        select = _txt switch
        {
            "+" => ExpressionOperators.Select.Addition,
            "-" => ExpressionOperators.Select.Subtraction,
            "*" => ExpressionOperators.Select.Multiplication,
            "/" => ExpressionOperators.Select.Division,
            _ => null
        };
        return select is not null;
    }

    public INumber? Level1()
        => Level1A(Level2());

    public INumber? Level1A(INumber? number)
    {
        while (_index < _matches.Count && BuilderINumberA(ref number)) ;
        return number;
    }

    public INumber? Level1B(INumber? number)
    {
        while (_index < _matches.Count && BuilderINumberB(ref number)) ;
        return number;
    }

    public bool BuilderINumberA(ref INumber? numberA)
    {
        if (numberA is null) return false;
        if (GetOperator(out ExpressionOperators.Select? selectA))
        {
            if(selectA is ExpressionOperators.Select.Multiplication ||
                    selectA is ExpressionOperators.Select.Division)
            {
                numberA = Level1B(numberA);
                return true;
            }
            INumber? numberB = Level2();
            if (numberB is not null)
                if (GetOperator(out ExpressionOperators.Select? selectB) &&
                    selectB is ExpressionOperators.Select.Multiplication ||
                    selectB is ExpressionOperators.Select.Division)
                {
                    numberA = ExpressionOperators.Build(numberA, selectA.Value, Level1B(numberB));
                    return true;
                }
                else if (selectB is null && _keyValues.TryGetValue(_txt, out BuilderNumber? value))
                {
                    numberA = ExpressionOperators.Build(numberA, selectA.Value, value.Get(numberB));
                    return true;
                }

            numberA = ExpressionOperators.Build(numberA, selectA.Value, numberB);
            return true;
        }
        else if (_keyValues.TryGetValue(_txt, out BuilderNumber? value))
        {
            numberA = value.Get(numberA);
            return true;
        }
        return false;
    }

    public bool BuilderINumberB(ref INumber? numberA)
    {
        if (numberA is null) return false;
        if (GetOperator(out ExpressionOperators.Select? selectA))
        {
            INumber? numberB = Level2();
            if (numberB is not null)
                if (GetOperator(out ExpressionOperators.Select? selectB) &&
                    selectB is ExpressionOperators.Select.Addition ||
                    selectB is ExpressionOperators.Select.Subtraction)
                {
                    numberA = ExpressionOperators.Build(numberA, selectA.Value, numberB);
                    return false;
                }
                else if (selectB is null && _keyValues.TryGetValue(_txt, out BuilderNumber? value))
                {
                    numberA = ExpressionOperators.Build(numberA, selectA.Value, value.Get(numberB));
                    return false;
                }

            numberA = ExpressionOperators.Build(numberA, selectA.Value, numberB);
            return true;
        }
        else if (_keyValues.TryGetValue(_txt, out BuilderNumber? value))
        {
            numberA = value.Get(numberA);
            return true;
        }
        return false;
    }

    private INumber? Level2()
    {
        Next();
        return _txt == string.Empty ? null :
            _keyValues.TryGetValue(_txt, out BuilderNumber? value) ? value.Get(null) :
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

        public override INumber? Get(INumber? number)
        {
            if (number is not null) throw new FormatException($"Invalid number format: {txt}");
            number = Level2();
            return number is not null ? new Negative(number) : null;
        }

    }

    private sealed class ParenthesesBuilder : BuilderNumber
    {

        public override INumber? Get(INumber? number)
        {
            if (number is not null) throw new FormatException($"Invalid number format: {txt}");
            number = Level1();
            if (number is null) return null;
            number = new Parentheses(number);
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

        protected string txt { get => mathematics is null ? string.Empty : mathematics._txt; }

        private Mathematics? mathematics = null;

        public static void Initialization(BuilderNumber BN, Mathematics m)
        {
            if (BN.mathematics is not null) throw new Exception("BuilderNumber has been initialized, it cannot be reused in other Mathematics");
            BN.mathematics = m;
        }

        protected INumber? Level1()
            => mathematics.Level1();

        protected INumber? Level2()
            => mathematics.Level2();

        protected void Next()
            => mathematics.Next();

        public abstract INumber? Get(INumber? number);

    }

}
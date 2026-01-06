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
        сache = new Dictionary<string, INumber>();
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
    private Dictionary<string, INumber> сache;
    private Regex _regex;
    private MatchCollection _matches;

    public bool GetNumber(string txt, out INumber? number)
    {
        lock (сache)
        {
            _matches = _regex.Matches(txt);
            _index = 0;
            number = Level1();
            сache.Clear();
            return number is not null;
        }
    }

    public INumber? Level1()
    {
        INumber? num = Level2();
        while (_index < _matches.Count && InfoOperator())
            num = BuilderINumberLowLevel(num);
        return num;
    }

    public INumber? BuilderINumberLowLevel(INumber? num_)
    {
        if (GetOperator(out ExpressionOperators.Select? selectA) &&
                selectA is not ExpressionOperators.Select.Multiplication &&
                selectA is not ExpressionOperators.Select.Division)
        {
            INumber? num = Level2();
            if (GetOperator(out ExpressionOperators.Select? selectB))
                if (selectB is ExpressionOperators.Select.Multiplication || selectB is ExpressionOperators.Select.Division)
                    return BuilderINumberLowLevel(
                        new ExpressionOperators(num_, selectA.Value, BuilderINumberHighLevel(
                            new ExpressionOperators(num, selectB.Value, Level2())
                            )));
                else return BuilderINumberLowLevel(new ExpressionOperators(num_, selectA.Value, num));
            else return BuilderINumberLowLevel(new ExpressionOperators(num_, selectA.Value, BuilderINumberHighLevel(num)));
        }
        return BuilderINumberHighLevel(num_);
    }

    public INumber? BuilderINumberHighLevel(INumber? num_)
    {
        if (GetOperator(out ExpressionOperators.Select? selectA) &&
                selectA is ExpressionOperators.Select.Multiplication ||
                selectA is ExpressionOperators.Select.Division)
        {
            INumber? num = Level2();
            return BuilderINumberHighLevel(new ExpressionOperators(num_, selectA.Value, num));
        }
        else if (_keyValues.TryGetValue(_txt, out BuilderNumber? value))
            return value.Get(num_);
        return num_;
    }

    private INumber? Level2()
    {
        Next();
        if (_txt == string.Empty) return null;
        INumber number;
        if (сache.TryGetValue(_txt, out number))
        {
            Next();
            return number;
        }
        if (_keyValues.TryGetValue(_txt, out BuilderNumber? value))
        {
            number = value.Get(null);
            сache.Add(_txt, number);
            return number;
        }
        return CreateNumber();
    }

    private INumber CreateNumber()
    {
        if (double.TryParse(_txt, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            INumber number = new Number(value);
            сache.Add(_txt, number);
            Next();
            return number;
        }
        throw new FormatException($"Invalid number format: {_txt}");
    }

    private bool GetOperator(out ExpressionOperators.Select? select)
        => (select = _txt switch
        {
            "+" => ExpressionOperators.Select.Addition,
            "-" => ExpressionOperators.Select.Subtraction,
            "*" => ExpressionOperators.Select.Multiplication,
            "/" => ExpressionOperators.Select.Division,
            _ => null
        }) is not null;

    private bool InfoOperator()
        => _txt switch
        {
            "+" => true,
            "-" => true,
            "*" => true,
            "/" => true,
            _ => _keyValues.TryGetValue(_txt, out _)
        };

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
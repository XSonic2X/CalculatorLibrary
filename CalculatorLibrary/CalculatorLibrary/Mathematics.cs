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
        cache = new Dictionary<string, INumber>();
        _regex = new Regex(_regexP);
        _keyValues ??= [];
        _keyValues.Add("-", new NegativeBuilder());
        _keyValues.Add("(", new ParenthesesBuilder());
        foreach (KeyValuePair<string, BuilderNumber> key in _keyValues)
            BuilderNumber.Initialization(key.Value, this);
    }

    private readonly string _regexP;
    private string _currentToken = string.Empty;
    private int _index;

    private Dictionary<string, BuilderNumber> _keyValues;
    private Dictionary<string, INumber> cache;
    private Regex _regex;
    private MatchCollection _matches;

    public bool GetNumber(string txt, out INumber? number)
    {
        lock (cache)
        {
            _matches = _regex.Matches(txt);
            _index = 0;
            number = Level1();
            cache.Clear();
            return number is not null && _currentToken.Length == 0;
        }
    }

    private INumber? Level1()
    {
        INumber? num = Level2();
        while (_index < _matches.Count && InfoOperator())
        {
            int before = _index;
            num = BuilderINumberLowLevel(num);
            if (_index == before) return null;
        }
        return num;
    }

    private INumber? BuilderINumberLowLevel(INumber? num_)
    {
        if (GetOperator(out ExpressionOperators.Select? selectA) &&
                selectA is not ExpressionOperators.Select.Multiplication &&
                selectA is not ExpressionOperators.Select.Division)
        {
            INumber? num = Level2();
            if (GetOperator(out ExpressionOperators.Select? selectB))
                if (selectB is ExpressionOperators.Select.Multiplication || selectB is ExpressionOperators.Select.Division)
                  return  BuilderINumberLowLevel(
                      new ExpressionOperators(num_, selectA.Value, BuilderINumberHighLevel(
                          new ExpressionOperators(num, selectB.Value, Level2())
                          )));
                else return BuilderINumberLowLevel(new ExpressionOperators(num_, selectA.Value, num));
            else return BuilderINumberLowLevel(new ExpressionOperators(num_, selectA.Value, BuilderINumberHighLevel(num)));
        }
        return BuilderINumberHighLevel(num_);
    }

    private INumber? BuilderINumberHighLevel(INumber? num_)
    {
        if (GetOperator(out ExpressionOperators.Select? selectA))
            if (selectA is ExpressionOperators.Select.Multiplication ||
                            selectA is ExpressionOperators.Select.Division)
            {
                INumber? num = Level2();
                return BuilderINumberHighLevel(new ExpressionOperators(num_, selectA.Value, num));
            }
            else return num_;
        else if (_keyValues.TryGetValue(_currentToken, out var value))
            return value.Get(num_);

        return num_;
    }

    private INumber? Level2()
    {
        Next();
        string token = _currentToken;
        if (token.Length == 0) 
            return null;

        if (token is "-" or "(")
            return _keyValues[token].Get(null);

        INumber? number;

        if (_keyValues.TryGetValue(token, out BuilderNumber? builder))
        {
            if (cache.TryGetValue(token, out number))
                goto Next;

            int start = _index;
            number = builder.Get(null);
            if (number is null) return null;
            if (_index == start) Next();
            if (_index == start + 1) cache[token] = number;
            return number;
        }

        if (cache.TryGetValue(token, out number)) 
            goto Next;

        if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
        {
            number = new Number(v);
            cache[token] = number;
            goto Next;
        }

        return null;

    Next:
        Next();
        return number;
    }

    private bool GetOperator(out ExpressionOperators.Select? select)
        => (select = _currentToken switch
        {
            "+" => ExpressionOperators.Select.Addition,
            "-" => ExpressionOperators.Select.Subtraction,
            "*" => ExpressionOperators.Select.Multiplication,
            "/" => ExpressionOperators.Select.Division,
            _ => null
        }) is not null;

    private bool InfoOperator()
        => _currentToken switch
        {
            "+" => true,
            "-" => true,
            "*" => true,
            "/" => true,
            _ => _keyValues.TryGetValue(_currentToken, out _)
        };

    private void Next()
        => _currentToken = _index < _matches.Count ?
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
            if (number is not null) throw new FormatException($"Invalid number format: {_currentToken}");
            number = Level2();
            return number is not null ? new Negative(number) : null;
        }

    }

    private sealed class ParenthesesBuilder : BuilderNumber
    {

        public override INumber? Get(INumber? number)
        {
            if (number is not null) throw new FormatException($"Invalid number format: {_currentToken}");
            number = Level1();
            if (number is null) return null;
            number = new Parentheses(number);
            if (_currentToken is not ")") throw new InvalidOperationException("Expected closing parenthesis");
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

        protected string _currentToken { get => mathematics is null ? string.Empty : mathematics._currentToken; }

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
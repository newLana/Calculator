using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Calculator
{
    /// <summary>
    /// Specifies constants that define priority levels of LexemeBase objects.
    /// </summary>
    public enum LexemePriority
    {
        None,
        Lowest,
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Represent string values as Lexemes for Calculator class. This class cannot be instantiated directly.
    /// </summary>
    public abstract class LexemeBase
    {
        /// <summary>
        /// Gets or sets values of lexeme objects
        /// </summary>
        public char Value { get; protected set; }

        /// <summary>
        /// Gets or sets priority levels of lexeme objects
        /// </summary>
        public LexemePriority Priority { get; protected set; }
        
        public static bool operator <=(LexemeBase lexeme1, LexemeBase lexeme2)
        {
            return lexeme1.Priority <= lexeme2.Priority;
        }
                
        public static bool operator >=(LexemeBase lexeme1, LexemeBase lexeme2)
        {
            return lexeme1.Priority >= lexeme2.Priority;
        }

        /// <summary>
        /// Returns a string that represents the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// Specifies an object as open bracket lexeme with "(" value and lowest priority. Derived from Calculator.LexemeBase class. This class cannot be inherited.
    /// </summary>
    public sealed class OpenBracket : LexemeBase
    {
        public OpenBracket()
        {
            Value = '(';
            Priority = LexemePriority.Lowest;
        }
    }

    /// <summary>
    /// Specifies an object as close bracket lexeme with ")" value and lowest priority. Derived from Calculator.LexemeBase. This class cannot be inherited.
    /// </summary>
    public sealed class CloseBracket : LexemeBase
    {
        public CloseBracket()
        {
            Value = ')';
            Priority = LexemePriority.Lowest;
        }
    }

    /// <summary>
    /// Specifies an object as operator lexeme with one of math sign value. Also specifies its priority from Low to High. Derived from Calculator.LexemeBase. This class cannot be inherited.
    /// </summary>
    public sealed class Operator : LexemeBase
    {
        /// <summary>
        /// Initializes a new instance of Calculator.Operator class with the specified value and priority that depends on input value
        /// </summary>
        /// <param name="value">The value of lexeme, that represents operator as math sign symbol</param>
        public Operator(char value)
        {
            Value = value;
            if (value == '+' || value == '-')
                Priority = LexemePriority.Low;
            else if (value == '*' || value == '/')
                Priority = LexemePriority.Medium;
            else
                Priority = LexemePriority.High;
        }
    }

    /// <summary>
    /// Specifies an object as operand lexeme and represent it as a double value. Also specifies its priority as None. Derived from Calculator.LexemeBase. This class cannot be inherited.
    /// </summary>
    public sealed class Operand : LexemeBase
    {
        /// <summary>
        /// Gets or sets double value of operand.
        /// </summary>
        public new double Value { get; private set; }

        /// <summary>
        /// Initialize new instance of Calculator.Operand class and specifies priority and its double value by parsing string value argument.  
        /// </summary>
        /// <param name="value">String value of operand.</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public Operand(string value) : this(ToDouble(value))
        {                      
        }

        /// <summary>
        /// Initialize new instance of Calculator.Operand class and specifies its priority and value.
        /// </summary>
        /// <param name="value">Double value of operand.</param>
        public Operand(double value)
        {
            Value = value;
            Priority = LexemePriority.None;
        }

        /// <summary>
        /// Convert string in double value.
        /// </summary>
        /// <param name="value">Input string representation.</param>
        /// <returns>Double value.</returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        private static double ToDouble(string value)
        {
            if (value.Contains("e"))
            {
                int index = value.IndexOf("e");
                string operandPart = value.Substring(0, index);
                double result = double.Parse(operandPart, NumberStyles.Float, CultureInfo.InvariantCulture); //allow decimal point end leading sign
                operandPart = value.Substring(index, value.Length - operandPart.Length);
                result *= Math.Pow(10, double.Parse(Regex.Replace(operandPart, @"\D", "")));
                return result;
            }
            else
                return double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }        
        
        public static double operator +(Operand operand1, Operand operand2)
        {           
            return operand1.Value + operand2.Value;
        }
        
        public static double operator -(Operand operand1, Operand operand2)
        {
            return operand1.Value - operand2.Value;
        }

        public static double operator *(Operand operand1, Operand operand2)
        {
            return operand1.Value * operand2.Value;
        }

       public static double operator /(Operand operand1, Operand operand2)
        {
            return operand1.Value / operand2.Value;
        }

       public static double operator ^(Operand operand1, Operand operand2)
        {
            return Math.Pow(operand1.Value, operand2.Value);
        }
    }
}

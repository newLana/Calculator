using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator
{
    public class Tokenizer
    {
        /// <summary>
        /// Gets an array of lexemes.
        /// </summary>
        public LexemeBase[] TokenArray { get; private set; }

        /// <summary>
        /// Represents a set of error messages about input string format errors. If its not empty then assigned to FormatException message property.
        /// </summary>
        private string syntaxErrorMessage;

        /// <summary>
        /// Initialize new instance of Calculator.Tokenizer class with specifing lexeme array if it's possible, otherwise throws FormatException with specifing message.
        /// </summary>
        /// <param name="inputString">Input string with expressions.</param>
        /// <exception cref="FormatException"></exception>
        public Tokenizer(string inputString)
        {
            syntaxErrorMessage = String.Empty;
            if (IsValid(ref inputString, out syntaxErrorMessage))
                TokenArray = Tokenizing(inputString);
            else
                throw new FormatException(syntaxErrorMessage);
        }

        /// <summary>
        /// Returns array of lexemes if it's possible, otherwise base.ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (TokenArray != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in TokenArray)
                    sb.Append(item + "\n");
                return sb.ToString();
            }
            return base.ToString();
        }

        /// <summary>
        /// Split input string and represent its parts as LexemeBase objects
        /// </summary>
        /// <returns>Array of LexemeBase objects like operators, brackets and operands</returns>
        private static LexemeBase[] Tokenizing(string inputString)
        {
            List<LexemeBase> LexemeBases = new List<LexemeBase>();
            int i = 0;
            do
            {
                string operand = "";
                while (i < inputString.Length && !((inputString[i] == '(' || inputString[i] == ')' || inputString[i] == '-' ||
                    inputString[i] == '*' || inputString[i] == '/' || inputString[i] == '^' || inputString[i] == '+') && (i > 0 && inputString[i - 1] != 'e')))
                {
                    operand += inputString[i++];
                }

                if (operand != "")
                {
                    LexemeBases.Add(new Operand(operand));
                }
                else
                {
                    if (inputString[i] == '(')
                        LexemeBases.Add(new OpenBracket());
                    else if (inputString[i] == ')')
                        LexemeBases.Add(new CloseBracket());
                    else
                        LexemeBases.Add(new Operator(inputString[i]));
                    i++;
                }
            } while (i < inputString.Length);

            return LexemeBases.ToArray();
        }

        #region SurfaceSyntaxAnalysis

        /// <summary>
        /// Input string validation
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Message about validation errors</param>
        /// <returns>True if its valid, otherwise False</returns>
        private static bool IsValid(ref string inputString, out string message)
        {
            //remove whitespaces
            inputString = inputString.Replace(" ", "");
            inputString = inputString.Replace(",", ".");
            //replace square brackets to round brackets
            inputString = Regex.Replace(inputString, @"\[", "(");
            inputString = Regex.Replace(inputString, @"\]", ")");
            //if there are negative number add zero befor minus
            inputString = Regex.Replace(inputString, @"\(\-", "(0-");
            inputString = Regex.Replace(inputString, @"^\-", "0-");
            //value to lowercase
            inputString = inputString.ToLower();
            StringBuilder errorMessageBuilder = new StringBuilder("");
            bool isValid = (IsNullOrEmptyLexemeBase(inputString, errorMessageBuilder)) ? false :
                (IsRoundBracketsPaired(inputString, errorMessageBuilder) &
                IsSquareBracketsPaired(inputString, errorMessageBuilder) &
                !IsDividingByZero(inputString, errorMessageBuilder) &
                !IsCharacterThere(inputString, errorMessageBuilder) &
                !IsRepitionOperator(inputString, errorMessageBuilder) &
                !HasMissedOperator(inputString, errorMessageBuilder));
            message = errorMessageBuilder.ToString();
            return isValid;
        }

        /// <summary>
        /// Check for round brackets pairing
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns>True if they are paired, otherwise False.</returns>
        private static bool IsRoundBracketsPaired(string inputString, StringBuilder message)
        {
            int openBrackets = inputString.Count(c => c == '(');
            int closeBrackets = inputString.Count(c => c == ')');
            if (openBrackets != closeBrackets)
            {
                message.Append(String.Format("Error. There is {0} missing.", (openBrackets < closeBrackets) ? '(' : ')') + Environment.NewLine);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check for square brackets pairing
        /// /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns>True if they are paired, otherwise False.</returns>
        private static bool IsSquareBracketsPaired(string inputString, StringBuilder message)
        {
            int openBrackets = inputString.Count(c => c == '[');
            int closeBrackets = inputString.Count(c => c == ']');
            if (openBrackets != closeBrackets)
            {
                message.Append(String.Format("Error. There is {0} missing.", (openBrackets < closeBrackets) ? '[' : ']') + Environment.NewLine);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check for divided by zero
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns>True if divided by zero find, otherwise False</returns>
        private static bool IsDividingByZero(string inputString, StringBuilder message)
        {
            if (Regex.IsMatch(inputString, @"/\(*0\)*") || Regex.IsMatch(inputString, @"/\(\p{N}\*0\)")
                || Regex.IsMatch(inputString, @"/\(0\*\p{N}\)") || Regex.IsMatch(inputString, @"/\(+0(\+|\-|\*)+0\)+"))
            {
                message.Append("Error. Input string contains dividing by zero operation." + Environment.NewLine);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check for unsupported symbols
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns>True - if they are, otherwise False</returns>
        private static bool IsCharacterThere(string inputString, StringBuilder message)
        {
            bool result = false;

            if (Regex.IsMatch(inputString, @"[\p{L}-[Ee]]+"))
            {
                message.Append("Error. Input string contains some unsupported text symbols." + Environment.NewLine);
                result = true;
            }
            if (Regex.IsMatch(inputString, @"[\p{P}-[\.\,\/\]\)\(\[\*\/\-]]+"))
            {
                message.Append("Error. Input string contains some unsupported punctuation symbols." + Environment.NewLine);
                result = true;
            }
            if (Regex.IsMatch(inputString, @"[\p{Sk}-[\^]]+"))
            {
                message.Append("Error. Input string contains some unsupported modifier symbols." + Environment.NewLine);
                result = true;
            }
            if (Regex.IsMatch(inputString, @"[\p{Sm}-[\+]]+"))
            {
                message.Append("Error. Input string contains some unsupported math symbols." + Environment.NewLine);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Check for plural operands stand together
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns>True - if plural operands together, otherwise - False</returns>
        private static bool IsRepitionOperator(string inputString, StringBuilder message)
        {
            if (Regex.IsMatch(inputString, @"(\*|\+|\-|\/|\^){2,}"))
            {
                message.Append("Error. There are two or more operators together." + Environment.NewLine);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check for missed sign in expression
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns></returns>
        private static bool HasMissedOperator(string inputString, StringBuilder message)
        {
            bool result = false;
            if (Regex.IsMatch(inputString, @"\)\("))
            {
                message.Append("Error. There is a missing operator between )(." + Environment.NewLine);
                result = true;
            }
            if (Regex.IsMatch(inputString, @"\p{Nd}\(") || Regex.IsMatch(inputString, @"\)\p{Nd}"))
            {
                message.Append("Error. There is a missing operator between bracket and number." + Environment.NewLine);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Check for null reference and for empty string
        /// </summary>
        /// <param name="inputString">Input string with expression</param>
        /// <param name="message">Error message</param>
        /// <returns>True - if input string is null or empty</returns>
        private static bool IsNullOrEmptyLexemeBase(string inputString, StringBuilder message)
        {
            if (inputString == null)
            {
                message.Append("Error. Input string is null." + Environment.NewLine);
                return true;
            }
            if (inputString == "")
            {
                message.Append("Error. Input string is empty." + Environment.NewLine);
                return true;
            }
            return false;
        }
        #endregion
    }
}

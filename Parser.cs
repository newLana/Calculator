using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    public class Parser
    {
        /// <summary>
        /// Gets an instance of Calculator.Tokenizer class which provided lexemes array
        /// </summary>
        private Tokenizer tokenizer;

        /// <summary>
        /// Sorted in reverse polish notation Tokens Array 
        /// </summary>
        private LexemeBase[] rpnLexemeBases;

        /// <summary>
        /// Length of inner RPN Tokens Array
        /// </summary>
        public int Length { get;}

        /// <summary>
        /// Indexer for inner RPN Tokens Array
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>LexemeBase element</returns>
        public LexemeBase this[int index]
        {
            get
            {
                return rpnLexemeBases[index];
            }
        }

        /// <summary>
        /// Gets input string - check it on syntax errors and converts it to token array by using instance of Calulator.Tokenizer class
        /// </summary>
        /// <param name="inputString">Input string with expressions</param>
        /// <exception cref="FormatException"></exception>
        public Parser(string inputString)
        {
            try
            {
                tokenizer = new Tokenizer(inputString);
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            rpnLexemeBases = GetReversePolishNotation(tokenizer.TokenArray);
            Length = rpnLexemeBases.Length;
        }

        /// <summary>
        /// Converts token array to Reverse Polish Notation token array
        /// </summary>
        /// <param name="LexemeBases"></param>
        /// <returns>TokenArray in Reverse Polish Notation representation</returns>
        public static LexemeBase[] GetReversePolishNotation(LexemeBase[] LexemeBases)
        {
            List<LexemeBase> rpnOutput = new List<LexemeBase>(); //output array
            Stack<LexemeBase> operators = new Stack<LexemeBase>();  //operators stack
            for (int i = 0; i < LexemeBases.Length; i++)
            {
                if (LexemeBases[i] is Operand)
                    rpnOutput.Add(LexemeBases[i]);
                else if (LexemeBases[i] is Operator)
                {
                    while (operators.Count != 0 && LexemeBases[i].Priority != LexemePriority.High && LexemeBases[i] <= operators.Peek())
                    {
                        rpnOutput.Add(operators.Pop());
                    }
                    operators.Push(LexemeBases[i]);
                }
                else if (LexemeBases[i] is OpenBracket)
                    operators.Push(LexemeBases[i]);
                else
                {
                    //if bracket is close
                    while (!(operators.Peek() is OpenBracket))
                    {
                        rpnOutput.Add(operators.Pop());
                    }
                    if (operators.Count > 0)
                        operators.Pop();
                }
            }
            while (operators.Count > 0)
            {
                rpnOutput.Add(operators.Pop());
            }
            return rpnOutput.ToArray<LexemeBase>();
        }
    }
}

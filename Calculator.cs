using System;
using System.Collections.Generic;

namespace Calculator
{
    public static class CalculatorBase
    {
        /// <summary>
        /// Returns double result of calculating inputed expression
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        public static double GetResult(string inputString)
        {
            try
            {
                var tokenArray = new Parser(inputString);

                Stack<Operand> operands = new Stack<Operand>();

                for (int i = 0; i < tokenArray.Length;)
                {
                    while (tokenArray[i] is Operand)
                    {
                        operands.Push(tokenArray[i++] as Operand);
                    }
                    Operand op2 = operands.Pop();
                    Operand op1 = operands.Pop();
                    try
                    {
                        var res = Calculating(op1, op2, tokenArray[i++] as Operator);
                        operands.Push(res);
                    }
                    catch (DivideByZeroException)
                    {
                        throw;
                    }
                    catch (OverflowException e)
                    {
                        throw e;
                    }
                }
                return operands.Pop().Value;
            }
            catch (FormatException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Returns new instance of Calculator.Operand class which are result of calculating
        /// </summary>
        /// <param name="op1">First operand</param>
        /// <param name="op2">Second operand</param>
        /// <param name="operatorSign">Operator</param>
        /// <returns>New calculated operand</returns>
        /// <exception cref="DivideByZeroException"></exception>
        /// <exception cref="OverflowException"></exception>
        private static Operand Calculating(Operand op1, Operand op2, Operator operatorSign)
        {
            Operand op;
            double result;
            switch (operatorSign.Value)
            {
                case '+':
                    result = op1 + op2;
                    break;
                case '-':
                    result = op1 - op2;
                    break;
                case '*':
                    result = op1 * op2;
                    break;
                case '/':                    
                    result = op1 / op2;
                    break;
                case '^':
                    result = op1 ^ op2;
                    break;
                default:
                    result = double.PositiveInfinity;
                    break;
            }
            if (!double.IsInfinity(result))
            {
                op = new Operand(result);
                return op;
            }
            else
                throw new OverflowException("Result is infinity.");
        }
    }
}

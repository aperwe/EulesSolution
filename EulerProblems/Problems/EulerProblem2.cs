﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QBits.Intuition.Mathematics.Fibonacci;
using QBits.Intuition.Mathematics;

namespace EulerProblems.Problems
{
    /// <summary>
    /// Each new term in the Fibonacci sequence is generated by adding the previous two terms. By starting with 1 and 2, the first 10 terms will be:
    /// 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, ...
    /// By considering the terms in the Fibonacci sequence whose values do not exceed four million, find the sum of the even-valued terms.
    /// </summary>
    [ProblemSolver("Problem 2", displayName = "Problem 2")]
    public class EulerProblem2 : AbstractEulerProblem
    {
        protected override void Solve(out string answer)
        {
            FibonacciSequence fib = new FibonacciSequence();
            Int64 limitValue = 4000000;
            int iterator = 0;
            Int64 currentValue = 0;

            Int64 resultingSum = 0;

            while (currentValue < limitValue)
            {
                currentValue = fib.Get(iterator);
                if (MoreMath.IsEven(currentValue)) resultingSum += currentValue;

                iterator++;
            }

            answer = string.Format("Sum of even valued fibonacci numbers under 1 million is: {0}.", resultingSum);
        }
    }
}

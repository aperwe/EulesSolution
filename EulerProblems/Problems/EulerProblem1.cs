﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EulerProblems.Problems
{
    /// <summary>
    /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.
    /// Find the sum of all the multiples of 3 or 5 below 1000.
    /// </summary>
    public class EulerProblem1 : AbstractEulerProblem
    {
        public override void Solve()
        {
            var inList = Enumerable.Range(1, 999);
            if (inList.Last() != 999)
            {
                throw new Exception("Max must be 999");
            }
            var listOfMultiplesOf3_or_5 = from item in inList
                                          where (item % 3 == 0) || (item % 5 == 0)
                                          select item;
            var sumOfMultiples = listOfMultiplesOf3_or_5.Aggregate((workingSum, nextNumber) => workingSum + nextNumber);
            var result = sumOfMultiples;
        }
    }
}

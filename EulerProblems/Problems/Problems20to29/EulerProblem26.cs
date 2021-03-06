﻿using QBits.Intuition.Mathematics;
using QBits.Intuition.Mathematics.Fibonacci;
using QBits.Intuition.Mathematics.Primes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EulerProblems.Problems.Problems20to29
{
    /// <summary/>
    [ProblemSolver("Reciprocal cycles", displayName = "Problem 26", problemDefinition =
@"A unit fraction contains 1 in the numerator. The decimal representation of the unit fractions with denominators 2 to 10 are given:

   1/2	= 	0.5
   1/3	= 	0.(3)
   1/4	= 	0.25
   1/5	= 	0.2
   1/6	= 	0.1(6)
   1/7	= 	0.(142857)
   1/8	= 	0.125
   1/9	= 	0.(1)
   1/10	= 	0.1
Where 0.1(6) means 0.166666..., and has a 1-digit recurring cycle. It can be seen that 1/7 has a 6-digit recurring cycle.

Find the value of d < 1000 for which 1/d contains the longest recurring cycle in its decimal fraction part."
        )]
    public class EulerProblem26 : AbstractEulerProblem
    {
        protected override void Solve(out string answer)
        {
            var maxDenominator = 1_000;
            var fractionCollection = new List<FractionSolver>();
            //Create 1000 fractions and store them in collection.
            fractionCollection.AddRange(
                from d in Enumerable.Range(1, maxDenominator)
                select new FractionSolver(denominator: d)
                );
            //Solve all fractions and find their patterns
            //parallel
            fractionCollection.AsParallel().ForAll(fraction => fraction.Solve(nominator: 1)); //Nominator is always 1 for this excercise
            //single CPU
            //elementCollection.ForEach(fraction => fraction.Solve(nominator: 1));
            var sortedFractions = fractionCollection.OrderByDescending(fraction => fraction.PatternLength);
            var first = sortedFractions.First();
            var three = sortedFractions.Where(f => f.Denominator == 3).First();
            answer = string.Format("The value of 1 < d < 1000 containing the longest recurring cycle in its decimal fraction point is: 1/{0}. Cycle is: {1}. Cycle length: {2}. Fraction is: {3}", first.Denominator, first.Pattern, first.PatternLength, first.Fraction);
        }
        protected void SolveSingleThreaded(out string answer)
        {
            Test();
            var maxD = 1000;

            var elementCollection = new List<FractionSolver>();
            
            foreach (int d in Enumerable.Range(1, maxD))
            {
                FractionSolver m = new FractionSolver(d);
                if (d == 6)
                {
                    var test = d;
                }
                m.Solve(nominator: 1);

            }
            answer = string.Format("Calculating...");
        }

        private void Test()
        {
            string pattern = "0.142857142857";
            FractionSolver element = new FractionSolver(denominator: 7);
            element.TestPattern(pattern);
        }
    }

    internal class FractionSolver
    {
        private int nominator;
        private int denominator;
        StringBuilder decimalRepresentation;
        int patternLength;
        string pattern;

        /// <summary>
        /// String representation of this element.
        /// </summary>
        public override string ToString()
        {
            return string.Format("1/{0} => {1} ({2})[{3}]", denominator, decimalRepresentation.ToString(), pattern, patternLength);
        }

        public FractionSolver(int denominator)
        {
            this.denominator = denominator;
        }
        internal int PatternLength { get { return patternLength; } }
        internal string Pattern { get { return pattern; } }
        internal int Denominator { get { return denominator; } }
        internal string Fraction { get { return decimalRepresentation.ToString(); } }

        /// <summary>
        /// TODO: Need to ensure pattern is of required minimal length (or longer) (not always 1)
        /// </summary>
        /// <param name="nominator"></param>
        internal void Solve(int nominator)
        {
            this.nominator = nominator;
            int divisionRemainder = nominator; //division remainder
            const int maxIterations = 10_000; //Long enough to be sure
            decimalRepresentation = new StringBuilder();
            bool endComputationFlag = false; //Flag that - when set to true - indicates to exit the computation loop
            var division = divisionRemainder / denominator;

            decimalRepresentation.Append(division);
            if (division == 0) decimalRepresentation.Append("."); //Decimal place if first iteration

            int iterations = 0;
            while (!endComputationFlag)
            {
                var diff = division * denominator;
                var carryOn = divisionRemainder - diff;
                var newRemainder = carryOn * 10; //Move 1 decimal place
                divisionRemainder = newRemainder;
                if (divisionRemainder == 0) //End computation if there is nothing to end
                    endComputationFlag = true;

                division = divisionRemainder / denominator;
                decimalRepresentation.Append(division);
                iterations++;
                if (iterations > maxIterations)
                    endComputationFlag = true;
            }
            DetectPattern(decimalRepresentation.ToString().Reverse(), minPatternLength: 7);
        }

        internal void TestPattern(string pattern)
        {
            var reversedString = pattern.Reverse();
            bool success = DetectPattern(reversedString, 1);
            
        }

        /// <summary>
        /// Searches for pattern and returns true if one is detected.
        /// </summary>
        /// <param name="reversedString"></param>
        /// <param name="minPatternLength"></param>
        private bool DetectPattern(IEnumerable<char> reversedString, int minPatternLength)
        {
            bool patternDetected = false; //Will set to true only when a pattern has been detected.
            List<char> originalString = new List<char>(reversedString);
            List<char> repeatablePatternCandidate = new List<char>();
            var maxLen = reversedString.Count();

            var current = 0;

            while (current < maxLen)
            {
                char currentChar = originalString[0];
                repeatablePatternCandidate.Add(currentChar);
                originalString.RemoveAt(0);
                int patternLength = GetRecurringPatternLength(repeatablePatternCandidate, originalString);
                if (patternLength >= minPatternLength)
                {
                    //Detected pattern of minimum length
                    pattern = new string(repeatablePatternCandidate.ToArray().Reverse().ToArray());
                    this.patternLength = patternLength;
                    patternDetected = true;
                    break;
                }
                current++;
            }
            return patternDetected;
        }

        /// <summary>
        /// Compares the candidate with origial string to see if the candidate is contained in original string.
        /// </summary>
        /// <param name="repeatablePatternCandidate">String that could be repeated in <paramref name="originalString"/>.</param>
        /// <param name="originalString">String to check if <paramref name="repeatablePatternCandidate"/> is contained within.</param>
        /// <returns>If candidate is a repeated pattern, length of the pattern is returned. Otherwise 0.</returns>
        private int GetRecurringPatternLength(List<char> repeatablePatternCandidate, List<char> originalString)
        {
            //string debugString = new string(originalString.ToArray());
            //string debugString2 = new string(repeatablePatternCandidate.ToArray());

            int detectedPatternLength = 0;
            foreach (int index in Enumerable.Range(0, Math.Min(repeatablePatternCandidate.Count, originalString.Count)))
            {
                var currentChar = repeatablePatternCandidate[index];
                if (repeatablePatternCandidate[index] == originalString[index])
                {
                    detectedPatternLength++;
                    continue;
                }
                break;
            }
            if (detectedPatternLength != repeatablePatternCandidate.Count) detectedPatternLength = 0;
            return detectedPatternLength;
        }
    }
}

﻿using QBits.Intuition.Collections;
using QBits.Intuition.Mathematics;
using QBits.Intuition.Mathematics.Primes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EulerProblems.Problems
{
    /// <summary/>
    [ProblemSolver("Pentagon numbers", displayName = "Problem 44", problemDefinition =
@"Pentagonal numbers are generated by the formula, Pn=n(3n−1)/2. The first ten pentagonal numbers are:

1, 5, 12, 22, 35, 51, 70, 92, 117, 145, ...

It can be seen that P4 + P7 = 22 + 70 = 92 = P8. However, their difference, 70 − 22 = 48, is not pentagonal.

Find the pair of pentagonal numbers, Pj and Pk, for which their sum and difference are pentagonal and D = |Pk − Pj| is minimised; what is the value of D?"
        )]
    public class EulerProblem44 : AbstractEulerProblem
    {
        protected override void Solve(out string answer)
        {
            PentagonManager pentagonManager = new PentagonManager();

            //Generate numbers up to 5000
            long maxCount = 5000;
            foreach (long j in Enumerable64.Range(1, maxCount))
            {
                PentagonNumber pj = new PentagonNumber(j);
                pentagonManager.Add(pj);
            }
            long count = maxCount * maxCount;
            long progressDone = 0;

            //Create all pairs 5000x5000
            Parallelization.GetParallelRanges(1, maxCount, 50).ForAll(sequence =>
            {
                foreach (long j in sequence)
                {
                    foreach (long k in Enumerable64.Range(1, maxCount))
                    {
                        PentagonPair pjk = pentagonManager.CreatePair(j, k);
                        if (pentagonManager.IsSumPentagonal(pjk))
                        {
                            if (pentagonManager.IsDifPentagonal(pjk))
                            {
                                lock (pentagonManager)
                                {
                                    pentagonManager.StorePair(pjk);
                                }
                            }

                        }
                        lock (this)
                        {
                            progressDone++;
                            if (progressDone % 100_000 == 0)
                            {
                                int percent = (int)(progressDone * 100.0 / count);
                                UpdateProgress($"Pairs checked out of {count}: Done {percent}%. Hits: {pentagonManager.storedPairs.Count}...");
                            }
                        }
                    }
                }
            });
            var solution = pentagonManager.storedPairs.OrderBy(pair => pair.AbsDifValue).First();
            answer = $"Candiates = {pentagonManager.storedPairs.Count}, D = {solution.AbsDifValue}.";
        }
    }

    internal class PentagonManager
    {
        private Dictionary<long, PentagonNumber> dictionary;
        /// <summary>Stores the pentagon pairs with required property (sum and dif are also pentagons).</summary>
        internal List<PentagonPair> storedPairs;

        public PentagonManager()
        {
            dictionary = new Dictionary<long, PentagonNumber>();
            storedPairs = new List<PentagonPair>();
        }

        internal void Add(PentagonNumber pj)
        {
            dictionary.Add(pj.Number, pj);
        }

        internal PentagonPair CreatePair(long j, long k)
        {
            return new PentagonPair(dictionary[j], dictionary[k]);
        }

        /// <summary>
        /// Tests whether the sum of pentagons in the pair is also a pentagonal.
        /// </summary>
        /// <param name="pair">Pair whose sum is to be checked for pentagonality.</param>
        /// <returns>True if the sum is a pentagonal number</returns>
        internal bool IsSumPentagonal(PentagonPair pair)
        {
            var sumValue = pair.SumValue;
            return dictionary.Values.Any(pentagon => pentagon.Value == sumValue);
        }

        /// <summary>
        /// Tests whether the absolute difference of pentagons in the pair is also a pentagonal.
        /// </summary>
        /// <param name="pair">Pair whose diff |Pj-Pk| is to be checked for pentagonality.</param>
        /// <returns>True if the absolute value of difference is a pentagonal number</returns>
        internal bool IsDifPentagonal(PentagonPair pair)
        {
            var difValue = pair.AbsDifValue;
            return dictionary.Values.Any(pentagon => pentagon.Value == difValue);
        }

        /// <summary>
        /// Records a pair for later use.
        /// </summary>
        /// <param name="pair">Pair to be stored.</param>
        internal void StorePair(PentagonPair pair)
        {
            storedPairs.Add(pair);
        }
    }

    internal class PentagonPair
    {
        internal PentagonNumber Pj { get; private set; }
        internal PentagonNumber Pk { get; private set; }
        public long SumValue => Pj.Value + Pk.Value;
        public long AbsDifValue => Math.Abs(Pj.Value - Pk.Value);

        public PentagonPair(PentagonNumber pj, PentagonNumber pk)
        {
            Pj = pj;
            Pk = pk;
        }
    }

    /// <summary>
    /// Representation of a pentagon number
    /// </summary>
    class PentagonNumber
    {
        public PentagonNumber(long number)
        {
            Number = number;
        }

        public long Number { get; }
        public long Value { get => Number * (3 * Number - 1) / 2; }
    }
}

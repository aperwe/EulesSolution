﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EulerProblems.Problems
{
    /// <summary>
    /// We shall say that an n-digit number is pandigital if it makes use of all the digits 1 to n exactly once; for example, the 5-digit number, 15234, is 1 through 5 pandigital.
    /// The product 7254 is unusual, as the identity, 39 × 186 = 7254, containing multiplicand, multiplier, and product is 1 through 9 pandigital.
    /// Find the sum of all products whose multiplicand/multiplier/product identity can be written as a 1 through 9 pandigital.
    /// HINT: Some products can be obtained in more than one way so be sure to only include it once in your sum.
    /// </summary>
    [ProblemSolverClass("Problem 32", DisplayName = "Problem 32")]
    public class EulerProblem32 : AbstractEulerProblem
    {
        protected override void Solve(out string answer)
        {
            int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Permutations<int> permutations = new Permutations<int>();
            var allSequencesToCheck = permutations.GeneratePermutations(new List<int>(numbers));
            long iterationCounter = 0;
            List<PandigitCandidate> allCombinations = new List<PandigitCandidate>();

            //All sequences of length 9 are ready.
            foreach (var sequence in allSequencesToCheck)
            {
                // Now break each sequence into lengths of 2 through 9
                foreach (int i in Enumerable.Range(1, 9))
                {
                    foreach (int j in Enumerable.Range(1, 9))
                    {
                        int totalLength = i + j;
                        if (totalLength > 9) continue;
                        iterationCounter++;
                        var fistSequence = sequence.Take(i);
                        var secondSequence = sequence.Skip(i).Take(j);
                        var tester = new PandigitCandidate(fistSequence, secondSequence);
                        allCombinations.Add(tester);
                    }
                }
            }

            //All combinations to check for pandigitability are in allCombinations collection.
            //Select only those that are pandigital.
            var newCollection = from candidate in allCombinations
                                where candidate.IsATruePandigit()
                                select candidate;
            var finalList = newCollection.ToList();
            //Since there are multiple results in the list, group them by product to make sure we include only one distinct product.
            var uniqueList = finalList.GroupBy(candidate => candidate.Product);
            StringBuilder answerPart = new StringBuilder().AppendLine();
            long sumProducts = 0;
            foreach (var group in uniqueList)
            {
                answerPart.AppendLine(group.First().ToString());
                sumProducts += group.Key;
            }

            answer = string.Format("All sequences = {0}. Loop iterations: {1}. Results: {2}. Sum of unique products: {3}. List of results: {4}", allSequencesToCheck.Count, iterationCounter, finalList.Count, sumProducts, answerPart);
        }
    }
    /// <summary>
    /// Single instance describing a combination of multiplicand*multiplier=product.
    /// </summary>
    internal class PandigitCandidate
    {
        static char[] digits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        internal long Multiplicand { get; private set; }
        internal long Multiplier { get; private set; }
        internal long Product => Multiplicand * Multiplier;
        /// <summary>Creates an instance of the candidate with the specified multiplicand and multiplier.</summary>
        /// <param name="multiplicand">Value of the multiplicand.</param>
        /// <param name="multiplier">Value of the multiplier.</param>
        public PandigitCandidate(IEnumerable<int> multiplicand, IEnumerable<int> multiplier)
        {
            int tenM = 1; Multiplicand = 0;
            foreach (var m in multiplicand)
            {
                Multiplicand += tenM * m;
                tenM *= 10;
            }
            int tenM2 = 1; Multiplier = 0;
            foreach (var m in multiplier)
            {
                Multiplier += tenM2 * m;
                tenM2 *= 10;
            }
        }
        /// <summary>Displays info aout this <seealso cref="PandigitCandidate"/>.</summary>
        public override string ToString()
        {
            return string.Format("{0} * {1} = {2}", Multiplicand, Multiplier, Product);
        }

        /// <summary>Checks if the specified combination of a,b, and a*b is a Pandigit combination.</summary>
        /// <returns>True if it is, false if it is not.</returns>
        internal bool IsATruePandigit()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Multiplicand);
            sb.Append(Multiplier);
            sb.Append(Product);
            if (sb.Length != 9) return false; //Required but not sufficient
            if (Multiplicand == 17)
            {
                var mmm = Multiplicand.ToString();
            }
            var array = sb.ToString().ToCharArray();
            var result = digits.All(digit => array.Contains(digit)); //Check that all digits 1-9 are contained within the array.
            return result;
        }
        /// <summary>
        /// Returns all digits as a sorted string.
        /// </summary>
        /// <returns></returns>
        internal string AllDigitsSorted()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Multiplicand);
            sb.Append(Multiplier);
            sb.Append(Product);
            return sb.ToString().OrderBy(c => c).ToString();
        }

    }
    /// <summary>
    /// Class used to generate permutations
    /// </summary>
    /// <typeparam name="T">Type of items in the permutations.</typeparam>
    public class Permutations<T>
    {
        /// <summary>Generate permutations of all the items in the <paramref name="items"/> collection.</summary>
        /// <param name="items"></param>
        /// <returns>List of permutations.</returns>
        public List<List<T>> GeneratePermutations(List<T> items)
        {
            // Make an array to hold the
            // permutation we are building.
            T[] current_permutation = new T[items.Count];

            // Make an array to tell whether
            // an item is in the current selection.
            bool[] in_selection = new bool[items.Count];

            // Make a result list.
            List<List<T>> results = new List<List<T>>();

            // Build the combinations recursively.
            PermuteItems(items, in_selection, current_permutation, results, 0);
            return results;
        }

        /// <summary>
        /// Recursively permute the items that are not yet in the current selection.
        /// </summary>
        /// <param name="items">List of permutations</param>
        /// <param name="in_selection"></param>
        /// <param name="current_permutation"></param>
        /// <param name="results"></param>
        /// <param name="next_position"></param>
        private void PermuteItems(List<T> items, bool[] in_selection, T[] current_permutation, List<List<T>> results, int next_position)
        {
            // See if all of the positions are filled.
            if (next_position == items.Count)
            {
                // All of the positioned are filled.
                // Save this permutation.
                results.Add(current_permutation.ToList());
            }
            else
            {
                // Try options for the next position.
                for (int i = 0; i < items.Count; i++)
                {
                    if (!in_selection[i])
                    {
                        // Add this item to the current permutation.
                        in_selection[i] = true;
                        current_permutation[next_position] = items[i];

                        // Recursively fill the remaining positions.
                        PermuteItems(items, in_selection, current_permutation, results, next_position + 1);

                        // Remove the item from the current permutation.
                        in_selection[i] = false;
                    }
                }
            }
        }
    }
}

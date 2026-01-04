using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace FirMath
{
    public static class GameMath
    {
        public static T Range<T>(T incomeValue, T lowerBound, T upperBound) where T : IComparable
        {
            if (incomeValue.CompareTo(lowerBound) < 0)
                return lowerBound;

            if (incomeValue.CompareTo(upperBound) > 0)
                return upperBound;

            return incomeValue;
        }

        public static int RandomCardFromTheDeck(int DeckLength)
        {
            return RandomCards(1, DeckLength)[0];
        }
        public static List<int> AFewCardsFromTheDeck(int NumberOfCardsDrawn, int DeckLength, bool WithoutDuplicates = true)
        {
            if (NumberOfCardsDrawn <= 0 || DeckLength <= 0)
            {
                return null;
            }

            if (NumberOfCardsDrawn >= DeckLength)
            {
                return Full(DeckLength);
            }

            return RandomCards(NumberOfCardsDrawn, DeckLength, WithoutDuplicates);
        }
        private static List<int> RandomCards(int NumberOfCardsDrawn, int DeckLength, bool WithoutDuplicates = true)
        {
            List<int> result = new List<int>(new int[NumberOfCardsDrawn]);
            while (NumberOfCardsDrawn > 0)
            {
                result[NumberOfCardsDrawn - 1] = Random.Range(0, DeckLength);
                NumberOfCardsDrawn--;
            }
            if (WithoutDuplicates)
            {
                RemoveDuplicate(result);
            }
            return result;
        }

        private static void RemoveDuplicate(List<int> result)
        {
            List<int> check = new List<int>();
            int resultLength = result.Count;
            for (int i = 0; i < resultLength; i++)
            {
                while (check.Contains(result[i]))
                {
                    result[i]++;
                    if(result[i] >= resultLength)
                    {
                        result[i] = 0;
                    }
                }
                check.Add(result[i]);
            }
        }
        private static List<int> Full(int i)
        {
            List<int> result = new List<int>(new int[i]);
            while (i > 0)
            {
                i--;
                result[i] = i;
            }
            return result;
        }
        public static List<T> ShuffleTheCards<T>(List<T> cards)
        {
            for (int i = cards.Count - 1; i >= 1; i--)
            {
                int j = Random.Range(0, i + 1);

                T temporary = cards[j];
                cards[j] = cards[i];
                cards[i] = temporary;
            }

            return cards;
        }

        public static bool HeadsOrTails()
        {
            return Random.Range(0, 2)==0 ? true: false;
        }

        public static bool IsSetsOfCardsMatch(in IReadOnlyCollection<int> Hand1, in IReadOnlyCollection<int> Hand2,
            bool countMustBeSame = true, bool checkOnlyFirstHand = false)
        {
            if (Hand1 == null && Hand2 == null)
                return true;
            if (Hand1 == null || Hand2 == null)
                return false;
            if (countMustBeSame && Hand1.Count != Hand2.Count)
                return false;
            if (Hand1.Count == 0 && Hand2.Count == 0)
                return true;
            if (Hand1.Count == 0 || Hand2.Count == 0)
                return false;

            List<int> hand1 = new List<int>(Hand1);
            List<int> hand2 = new List<int>(Hand2);

            hand1.Sort();
            hand2.Sort();

            if (countMustBeSame)
            {
                for (int i = 0; i < hand1.Count; i++)
                {
                    if(hand1[i] != hand2[i])
                        return false;
                }
            }
            else
            {
                for (int i = 0; i < hand1.Count; i++)
                {
                    if (!hand2.Contains(hand1[i]))
                        return false;
                }

                if (!checkOnlyFirstHand)
                {
                    for (int i = 0; i < hand2.Count; i++)
                    {
                        if (!hand1.Contains(hand2[i]))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
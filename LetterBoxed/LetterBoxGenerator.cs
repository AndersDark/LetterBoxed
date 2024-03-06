using System.Windows;

namespace LetterBoxed
{
    internal static class LetterBoxGenerator
    {
        private static readonly int sumProbablitity = 10000;

        private static readonly Dictionary<string, int> norwegianLetterDistribution = new Dictionary<string, int>
        {
            { "A", 608 },
            { "B", 147 },
            { "C", 34  },
            { "D", 421 },
            { "E", 1538},
            { "F", 202 },
            { "G", 390 },
            { "H", 164 },
            { "I", 616 },
            { "J", 102 },
            { "K", 383 },
            { "L", 526 },
            { "M", 340 },
            { "N", 781 },
            { "O", 503 },
            { "P", 206 },
            { "Q", 2   },
            { "R", 853 },
            { "S", 636 },
            { "T", 784 },
            { "U", 180 },
            { "V", 240 },
            { "W", 15  },
            { "X", 5   },
            { "Y", 74  },
            { "Z", 4   },
            { "Æ", 20  },
            { "Ø", 76  },
            { "Å", 150 }
        };
        // Frquency found at "https://no.wikipedia.org/wiki/Frekvensanalyse_(kryptografi)"

        private static readonly Dictionary<string, int> englishLetterDistribution = new Dictionary<string, int>
        {
            { "A", 812 },
            { "B", 149 },
            { "C", 271 },
            { "D", 432 },
            { "E", 1202},
            { "F", 230 },
            { "G", 203 },
            { "H", 592 },
            { "I", 731 },
            { "J", 10  },
            { "K", 69  },
            { "L", 398 },
            { "M", 261 },
            { "N", 696 },
            { "O", 768 },
            { "P", 182 },
            { "Q", 11  },
            { "R", 602 },
            { "S", 628 },
            { "T", 910 },
            { "U", 288 },
            { "V", 111 },
            { "W", 209 },
            { "X", 17  },
            { "Y", 211 },
            { "Z", 7   }
        };
        // Frquency found at https://pi.math.cornell.edu/~mec/2003-2004/cryptography/subs/frequencies.html


        public static string[,] populateLetterBox(string[,] letterBox, bool playDaily, bool playNorwegian)
        {

            Random rnd;
            
            if (playDaily)
            {
                DateTime date = DateTime.Now;
                int seed = date.Year + date.Month + date.Day;
                rnd = new Random(seed);
            }
            else
            {
                rnd = new Random();
            }

            List<string> pickedLetters = new List<string>();

            for (int side = 0; side < MainWindow.n_side; side++)
            {
                for (int idx = 0; idx < MainWindow.n_idx; idx++)
                {
                    while (true)
                    {
                        int randomValue = rnd.Next(sumProbablitity);
                        string letter = pickLetterFromDistribution(randomValue, playNorwegian);

                        if (!pickedLetters.Contains(letter))
                        {
                            pickedLetters.Add(letter);
                            letterBox[side, idx] = letter;
                            break;
                        }
                    }
                }
            }

            List<string> vowels;

            if (playNorwegian)
            {
                vowels = new List<string> { "A", "E", "I", "O", "U", "Y", "Æ", "Ø", "Å" };
            }
            else
            {
                vowels = new List<string> { "A", "E", "I", "O", "U", "Y" };
            }

            List<string> pickedVowels = vowels.Intersect(pickedLetters).ToList();

            if (pickedVowels.Count < 2)
            {
                return populateLetterBox(letterBox, playDaily, playNorwegian);
            }
            

            return letterBox;
        }

        private static string pickLetterFromDistribution(int randomValue, bool playNorwegian)
        {
            if (randomValue < 0 || randomValue > sumProbablitity)
            {
                throw(new ArgumentOutOfRangeException("value is outside range"));
            }

            int sum = 0;

            if (playNorwegian)
            {
                foreach (var letter in norwegianLetterDistribution)
                {
                    sum += letter.Value;
                    if (randomValue <= sum)
                    {
                        return letter.Key;
                    }
                }
            }
            else
            {
                foreach (var letter in englishLetterDistribution)
                {
                    sum += letter.Value;
                    if (randomValue <= sum)
                    {
                        return letter.Key;
                    }
                }
            }

            throw (new Exception("Did not find any letter"));
        }
    }
}

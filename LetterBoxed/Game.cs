namespace LetterBoxed
{
    internal class Game
    {
        private string[,] letterBox = new string[4, 3];
        private int currentSide = -1;
        private string currentWord = string.Empty;
        private List<string> enteredWords = new List<string>();
        private Dictionary<string, int> letterCount = new Dictionary<string, int>();

        private readonly bool playNorwegian;
        private bool win = false;
        public Game(bool _playNorwegian,bool playDaily)
        {
            playNorwegian = _playNorwegian;

            letterBox = LetterBoxGenerator.populateLetterBox(letterBox, playDaily, playNorwegian);

            for (int side = 0; side < MainWindow.n_side; side++)
            {
                for (int idx = 0; idx < MainWindow.n_idx; idx++)
                {
                    letterCount[letterBox[side, idx]] = 0;
                }
            }
        }

        public string getBoxLetter(int side, int idx)
        {
            return letterBox[side, idx];
        }

        public string getCurrentWord()
        {
            return currentWord;
        }

        public List<string> getEnteredWords()
        {
            return enteredWords;
        }

        public void appendCurrentWord(string letter)
        {
            int side = getSide(letter);

            if (side == currentSide)
                return;
            else if (side == -1)
                return;

            currentSide = side;
            currentWord += letter;
            letterCount[letter] += 1;

        }

        public void removeLastLetter()
        {
            if (currentWord.Length == 0)
                return;

            letterCount[currentWord.Last().ToString()] -= 1;
            currentWord = currentWord.Remove(currentWord.Length - 1, 1);

            if (currentWord.Length > 0)
            {
                currentSide = getSide(currentWord.Last().ToString());
                return;
            }

            if (enteredWords.Count > 0)
            {
                currentWord = enteredWords.Last();
                enteredWords.RemoveAt(enteredWords.Count - 1);
                currentSide = getSide(currentWord.Last().ToString());
            }
            else
            {
                currentSide = -1;
            }
        }

        public bool wordComplete()
        {
            if (playNorwegian && !WordChecker.isNorwegianWord(currentWord))
                return false;

            if (!playNorwegian && !WordChecker.isEnglishWord(currentWord))
                return false;

            enteredWords.Add(currentWord);

            string lastLetter = currentWord.Last().ToString();
            letterCount[lastLetter] += 1;
            currentWord = lastLetter;

            if (letterCount.Values.All(count => count > 0))
            {
                currentWord = string.Empty;
                win = true;
            }
            return true;
        }

        public int getLetterCount(string letter)
        {
            return letterCount[letter];
        }

        private int getSide(string letter)
        {
            for (int side = 0; side < MainWindow.n_side; side++)
            {
                for (int idx = 0; idx < MainWindow.n_idx; idx++)
                {
                    if (letterBox[side, idx] == letter)
                    {
                        return side;
                    }
                }
            }
            return -1; // not contained
        }

        public (int, int) getLetterPosition(string letter)
        {
            for (int side = 0; side < MainWindow.n_side; side++)
            {
                for (int idx = 0; idx < MainWindow.n_idx; idx++)
                {
                    if (letterBox[side, idx] == letter)
                    {
                        return (side, idx);
                    }
                }
            }
            return (-1,-1); // not contained
        }

        public bool checkWin()
        {
            return win;
        }
    }
}

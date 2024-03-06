using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LetterBoxed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public const int n_side = 4;
        public const int n_idx = 3;

        private struct LineObject
        {
            public string id;
            public Line line;
            public bool isGray;
        }

        private Game game = new Game(true, false);

        private List<LineObject> drawnLines = new List<LineObject>();

        private TextBox[,] letterBoxArray = new TextBox[4, 3];

        private Ellipse[,] EllipseArray = new Ellipse[4,3];

        private bool gameInitialized = false;

        private bool playNorwegian = true;

        public MainWindow()
        {
            InitializeComponent();

            letterBoxArray[0, 0] = LetterBox00; letterBoxArray[0, 1] = LetterBox01; letterBoxArray[0, 2] = LetterBox02;
            letterBoxArray[1, 0] = LetterBox10; letterBoxArray[1, 1] = LetterBox11; letterBoxArray[1, 2] = LetterBox12;
            letterBoxArray[2, 0] = LetterBox20; letterBoxArray[2, 1] = LetterBox21; letterBoxArray[2, 2] = LetterBox22;
            letterBoxArray[3, 0] = LetterBox30; letterBoxArray[3, 1] = LetterBox31; letterBoxArray[3, 2] = LetterBox32;

            EllipseArray[0, 0] = Circle00; EllipseArray[0, 1] = Circle01; EllipseArray[0, 2] = Circle02;
            EllipseArray[1, 0] = Circle10; EllipseArray[1, 1] = Circle11; EllipseArray[1, 2] = Circle12;
            EllipseArray[2, 0] = Circle20; EllipseArray[2, 1] = Circle21; EllipseArray[2, 2] = Circle22;
            EllipseArray[3, 0] = Circle30; EllipseArray[3, 1] = Circle31; EllipseArray[3, 2] = Circle32;

            var assembly = Assembly.GetExecutingAssembly();

            foreach(string res in assembly.GetManifestResourceNames())
            {
                Trace.WriteLine(res);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement? fe = sender as FrameworkElement;

            if (fe == null)
                return;
            

            if (fe.Name == "PlayDailyBtn")
                startGame(true);
            else if (fe.Name == "PlayRandomBtn")
                startGame(false);
            else if (fe.Name == "RestartGameBtn")
                restartGame();
            else if (fe.Name == "LanguageToggleBtn")
            {
                CheckBox? cb = fe as CheckBox;

                if (cb == null || cb.IsChecked == null)
                    return;
                
                playNorwegian = cb.IsChecked.Value;
            }
        }

        private void startGame(bool playDaily)
        {
            StartScreen.Visibility = Visibility.Hidden;

            game = new Game(playNorwegian, playDaily);

            initialize_window();

            updateScreen();

            gameInitialized = true;
        }
        private void initialize_window()
        {
            WordString.Text = string.Empty;

            for (int side = 0; side < 4; side++)
            {
                for (int idx = 0; idx < 3; idx++)
                {
                    letterBoxArray[side,idx].Text = game.getBoxLetter(side,idx);
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameInitialized)
                return;

            if (game.checkWin())
                return;

            if (e.Key == Key.Enter)
            {
                bool wordAccepted = game.wordComplete();

                if (!wordAccepted)
                    doShakeAnimation();

                if (game.checkWin())
                    endGame();
            }
            else if (e.Key == Key.Back)
            {
                game.removeLastLetter();
            }
            else if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                game.appendCurrentWord(e.Key.ToString());
            }
            else if (e.Key == Key.OemQuotes)
            {
                game.appendCurrentWord("Æ");
            }
            else if (e.Key == Key.Oem3)
            {
                game.appendCurrentWord("Ø");
            }
            else if (e.Key == Key.Oem6)
            {
                game.appendCurrentWord("Å");
            }
            else
            {
                return;
            }
            updateScreen();
        }

        private void endGame()
        {
            EndScreen.Visibility = Visibility.Visible;
            int tries = game.getEnteredWords().Count();
            FinalScoreTextBox.Text = string.Format("You used {0} tries",tries);
        }

        private void restartGame()
        {
            gameInitialized = false;
            StartScreen.Visibility = Visibility.Visible;
            EndScreen.Visibility = Visibility.Hidden;
        }
        private void updateScreen()
        {
            WordString.Text = game.getCurrentWord();
            EnteredWords.Text = string.Empty;

            EnteredWords.Text = string.Join(" - ", game.getEnteredWords());

            for (int side = 0; side < n_side; side++)
            {
                for (int idx = 0; idx < n_idx;  idx++)
                {
                    string letter = game.getBoxLetter(side, idx);
                    if (game.getLetterCount(letter) > 0)
                    {
                        EllipseArray[side, idx].Fill = (Brush)Application.Current.Resources["BoxCircleUsedColor"];
                    }
                    else
                    {
                        EllipseArray[side, idx].Fill = (Brush)Application.Current.Resources["BoxCircleUnusedColor"]; ;
                    }
                }
            }

            updateLines();
        }

        private void updateLines()
        {
            string currentWord = game.getCurrentWord();
            List<string> enteredWords = game.getEnteredWords();

            int idx = 0;

            foreach (string word in enteredWords)
            {
                for(int i = 0; i < word.Length-1; i++)
                {
                    if (drawnLines.Count <= idx)
                    {
                        drawLine(word[i].ToString(), word[i + 1].ToString(), true);
                        idx++;
                    }
                    else if (drawnLines[idx].id != word.Substring(i, 2) || !drawnLines[idx].isGray)
                    {
                        while(idx <  drawnLines.Count)
                        {
                            removeLastLine();
                        }
                        i--;
                    }
                    else
                    {
                        idx++;
                    }
                }
            }

            for (int i = 0; i < currentWord.Length - 1; i++)
            {
                if (drawnLines.Count <= idx)
                {
                    drawLine(currentWord[i].ToString(), currentWord[i + 1].ToString(), false);
                    idx++;
                }
                else if (drawnLines[idx].id != currentWord.Substring(i, 2) || drawnLines[idx].isGray)
                {
                    while (idx < drawnLines.Count)
                    {
                        removeLastLine();
                    }
                    i--;
                }
                else
                {
                    idx++;
                }
            }

            while (idx < drawnLines.Count)
            {
                removeLastLine();
            }

        }

        private void drawLine(string from_letter, string to_letter, bool isGray)
        {
            (int from_side, int from_idx) = game.getLetterPosition(from_letter);
            (int to_side, int to_idx) = game.getLetterPosition(to_letter);
            double x1 = Int32.Parse(Canvas.GetLeft(EllipseArray[from_side, from_idx]).ToString())+5.5;
            double y1 = Int32.Parse(Canvas.GetTop(EllipseArray[from_side, from_idx]).ToString())+5.5;
            double x2 = Int32.Parse(Canvas.GetLeft(EllipseArray[to_side, to_idx]).ToString())+5.5;
            double y2 = Int32.Parse(Canvas.GetTop(EllipseArray[to_side, to_idx]).ToString())+5.5;


            SolidColorBrush stroke;
            
            if (isGray)
            {
                stroke = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                stroke = new SolidColorBrush(Colors.Black);
            }

            LineObject line;

            line.id = from_letter + to_letter;
            line.line = new Line() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = stroke, StrokeThickness = 2 };
            line.isGray = isGray;

            drawnLines.Add(line);
            canvasArea.Children.Add(line.line);
        }

        private void removeLastLine()
        {
            if (drawnLines.Count == 0) 
            {
                return;
            }
            LineObject line = drawnLines.Last();
            canvasArea.Children.Remove(line.line);
            drawnLines.Remove(line);
        }

        private void doShakeAnimation()
        {
            Storyboard? sb = FindResource("ShakeAnimation") as Storyboard;
            if (sb == null) return;
            sb.Begin();
        }

    }
}
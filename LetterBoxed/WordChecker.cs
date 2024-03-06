using System.IO;
using System.Reflection;
using System.Diagnostics;


namespace LetterBoxed
{
    internal static class WordChecker
    {

        static private IEnumerable<string> norwegianWords;
        static private IEnumerable<string> englishWords; 
        static WordChecker()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Stream? noStream = assembly.GetManifestResourceStream("LetterBoxed.Assets.norwegianWords.txt");
            Stream? enStream = assembly.GetManifestResourceStream("LetterBoxed.Assets.englishWords.txt");

            if (noStream == null || enStream == null) {
                throw new Exception("Could not find word data base");
            }

            StreamReader norwegianWordsStream = new StreamReader(noStream);
            StreamReader englishWordsStream = new StreamReader(enStream);

            norwegianWords = norwegianWordsStream.ReadToEnd().Split("\n");
            englishWords = englishWordsStream.ReadToEnd().Split("\n");
        }



        public static bool isNorwegianWord(string word)
        {
            return norwegianWords.Contains(word.ToLower());
        }

        public static bool isEnglishWord(string word)
        {
            return englishWords.Contains(word.ToLower());
        }
    }
}

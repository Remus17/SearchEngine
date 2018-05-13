using SearchEngine.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SearchEngine.IndexingConsole
{
  public class WordUtils
  {

    private static HashSet<string> _stopWords;
    private static HashSet<string> _exceptionWords;
    private static char[] _delimiters = ".,\"'?!-|/:\\({[]});”“’=_@#$%^&*~`+-–—<>\r\n\t".ToCharArray();

    public static Dictionary<string, int> GetWordFrequency(string path, PorterStemming porter)
    {
      Init();
      var dic = new Dictionary<string, int>();

      using (StreamReader reader = new StreamReader(path))
      {
        string word = string.Empty;
        do
        {
          char c = (char)reader.Read();
          if (Array.IndexOf(_delimiters, c) == -1 && !Char.IsWhiteSpace(c))
          {
            word += c;
            continue;
          }
          word = word.ToLower();

          if (_exceptionWords.Contains(word) || _stopWords.Contains(word) || word.Length < 3)
          {
            word = string.Empty;
            continue;
          }

          word = porter.Stem(word);
          if (dic.ContainsKey(word))
          {
            dic[word] = dic[word] + 1;
          }
          else
          {
            dic[word] = 1;
          }
          word = string.Empty;

        } while (!reader.EndOfStream);
      }
      return dic;
    }

    private static void Init()
    {
      if (_stopWords == null)
      {
        _stopWords = GetFileWords("stopwords.txt");
      }

      if (_exceptionWords == null)
      {
        _exceptionWords = GetFileWords("exceptionwords.txt");
      }
    }

    private static HashSet<string> GetFileWords(string path)
    {
      var set = new HashSet<string>();
      using (StreamReader reader = new StreamReader(path))
      {
        while (!reader.EndOfStream)
        {
          set.Add(reader.ReadLine().Trim());

        }
      }
      return set;
    }

  }
}

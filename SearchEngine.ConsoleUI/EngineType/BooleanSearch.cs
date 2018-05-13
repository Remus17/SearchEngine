//using SearchEngine.Database.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace SearchEngine.ConsoleUI.EngineType
//{
//  public class BooleanSearch
//  {
    
//    public List<ReducedWordDocument> Search(string search)
//    {
//      var resultedItems = new List<ReducedWordDocument>();
//      var searchItems = search.Split(' ');
//      var operands = new List<string>();
//      var operators = new List<string>();
//      for (int i = 0; i < searchItems.Length; i++)
//      {
//        if (i % 2 == 0)
//        {
//          operands.Add(searchItems[i]);
//        }
//        else
//        {
//          operators.Add(searchItems[i]);
//        }
//      }
//      var allIndexedWords = new Queue<List<ReducedWordDocument>>();
//      foreach (var operand in operands)
//      {
//        allIndexedWords.Enqueue(GetIndexedWords(operand).ToList());
//      }

//      foreach (var searchOperator in operators)
//      {
//        if (!resultedItems.Any())
//        {
//          resultedItems = DoOperation(searchOperator, allIndexedWords.Dequeue(), allIndexedWords.Dequeue());
//        }
//        else
//        {
//          resultedItems = DoOperation(searchOperator, resultedItems, allIndexedWords.Dequeue());
//        }
//      }

//      return resultedItems;

//    }

//    public List<ReducedWordDocument> DoOperation(string searchOperator, IEnumerable<ReducedWordDocument> firstList,
//      IEnumerable<ReducedWordDocument> secondList)
//    {
//      var returnedDocuments = new List<ReducedWordDocument>();
//      IEnumerable<ReducedWordDocument> largerList, smallerList;
//      if (firstList.Count() > secondList.Count())
//      {
//        largerList = firstList;
//        smallerList = secondList;
//      }
//      else
//      {
//        largerList = secondList;
//        smallerList = firstList;
//      }



//      if (searchOperator == "AND")
//      {
//        foreach (var indexedWord in largerList)
//        {
//          if (smallerList.Any(x => x. == indexedWord.FilePath))
//          {
//            returnedDocuments.Add(indexedWord);
//          }
//        }
//      }
//      else if (searchOperator == "OR")
//      {
//        returnedDocuments = largerList.Union(smallerList).GroupBy(x => x.FilePath).Select(x => x.First()).ToList();
//      }
//      else if (searchOperator == "NOT")
//      {
//        foreach (var indexedWord in firstList)
//        {
//          if (!secondList.Any(x => x.FilePath == indexedWord.FilePath))
//          {
//            returnedDocuments.Add(indexedWord);
//          }
//        }
//      }
//      return returnedDocuments;
//    }

//    public IEnumerable<ReducedWordDocument> GetIndexedWords(string word)
//    {
//      var foundFirst = false;
//      var foundLast = false;
//      using (var sr = new StreamReader(_reversedIndexFilePath))
//      {
//        while (!sr.EndOfStream && !foundLast)
//        {
//          var line = sr.ReadLine();
//          if (!string.IsNullOrEmpty(line))
//          {
//            var members = line.Split(' ');
//            if (members[0] == word)
//            {
//              if (!foundFirst)
//              {
//                foundFirst = true;
//              }
//              yield return new ReducedWordDocument()
//              {
//                Word = members[0],
//                Count = Convert.ToInt32(members[1]),
//                FilePath = members[2]
//              };
//            }
//            else if (foundFirst)
//            {
//              foundLast = true;
//            }

//          }

//        }

//      }

//    }
//  }
//}

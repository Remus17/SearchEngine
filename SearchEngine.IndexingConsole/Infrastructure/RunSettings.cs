using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.IndexingConsole
{
  public class RunSettings
  {
    public static int MaxDegreeOfParallelism
    {
      get
      {
        return Environment.ProcessorCount; // we want to start as many threads as the number of available processors
      }
    }

    public static int DocumentNumber { get; set; } = -1;
    public static string DatasetPath { get; set; }

    public static LogType LogType { get; set; } = LogType.Default;

    public static int DocumentBatchSize { get; set; } = 5;
    public static int GroupedWordsBatchSize { get; set; } = 30;
  }

  public enum LogType
  {
    Debug,
    Default
  }
}

﻿using SearchEngine.IndexingConsole.Indexing.Workers;
using SearchEngine.IndexingConsole.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine.IndexingConsole.Indexing
{
  public class DirectIndexCreator
  {
    public static void Start()
    {

      //master
      RunSettings.DocumentNumber = RunSettings.FilePaths.Count;

      var parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = RunSettings.MaxDegreeOfParallelism
      };

      Parallel.ForEach(RunSettings.FilePaths, parallelOptions, Slave.BuildDirectIndex);
    }
  }
}

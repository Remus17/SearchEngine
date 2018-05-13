using SearchEngine.IndexingConsole.Indexing.Workers;
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
      var files = FileTreeTools.GetFileLinkRelations(RunSettings.DatasetPath);
      RunSettings.DocumentNumber = files.Count;

      var parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = RunSettings.MaxDegreeOfParallelism
      };

      Parallel.ForEach(files, parallelOptions, Slave.BuildDirectIndex);
    }
  }
}

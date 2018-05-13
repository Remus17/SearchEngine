using SearchEngine.IndexingConsole.Indexing.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.IndexingConsole.Indexing
{
  public class DocumentModuleComputer
  {
    public static void Start()
    {
      var locations = RunSettings.FilePaths.Select(x => x.Uri);

      var parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = RunSettings.MaxDegreeOfParallelism
      };
      Parallel.ForEach(Master.Batch(locations, RunSettings.DocumentBatchSize), parallelOptions, Slave.ComputeDocumentsModule);

    }
  }
}

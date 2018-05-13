using Microsoft.Extensions.DependencyInjection;
using SearchEngine.IndexingConsole.Indexing;
using SearchEngine.IndexingConsole.Indexing.Interfaces;
using System;

namespace SearchEngine.IndexingConsole
{
  public static class DependencyResolver
  {

    public static IServiceProvider BuildDi()
    {
      var services = new ServiceCollection();

      //services.AddScoped<IIndexer, DbMapReduceIndexer>();

      return services.BuildServiceProvider();
    }

  }
}

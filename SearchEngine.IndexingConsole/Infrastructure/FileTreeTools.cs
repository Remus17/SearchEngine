using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SearchEngine.IndexingConsole.Infrastructure
{
  public class FileTreeTools
  {
    public static List<LinkToFileRelation> GetFileLinkRelations(string folderPath)
    {
      var files = new List<LinkToFileRelation>();
      if (!Directory.Exists(folderPath))
      {
        return files;
      }
      foreach (var path in Directory.EnumerateDirectories(folderPath))
      {
        var domain = new DirectoryInfo(path).Name;
        var txtFiles = Directory.GetFiles(path, "*txt", SearchOption.AllDirectories);
        files.AddRange(txtFiles.Select(x => new LinkToFileRelation()
        {
          FilePath = x,
          Uri = Path.GetFullPath(Path.Combine(x,@"..\")).Replace(path, domain)
        }));
      }

      return files;
    }

    public static Queue<string> GetFilesFromDirectory(string path)
    {
      var directoryQueue = new Queue<string>();
      var filesQueue = new Queue<string>();
      if (!Directory.Exists(path))
      {
        return filesQueue;
      }
      directoryQueue.Enqueue(path);

      while (directoryQueue.Count != 0)
      {
        var directory = directoryQueue.Dequeue();
        foreach (var dir in Directory.EnumerateDirectories(directory))
        {
          directoryQueue.Enqueue(dir);
        }
        foreach (var file in Directory.EnumerateFiles(directory))
        {
          filesQueue.Enqueue(file);
        }
      }

      return filesQueue;
    }

    public static void CreateDirectory(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
    }

    public static void CleanDirectory(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
      System.IO.DirectoryInfo di = new DirectoryInfo(path);

      foreach (FileInfo file in di.GetFiles())
      {
        file.Delete();
      }
      foreach (DirectoryInfo dir in di.GetDirectories())
      {
        dir.Delete(true);
      }
    }
  }
}

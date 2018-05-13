using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Database.Models
{
  public class ModuleDocument
  {
    public ObjectId Id { get; set; }
    public string Location { get; set; }
    public float Module { get; set; }
  }
}

using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Database.Models
{
  public class ReducedWordDocument
  {
    public ObjectId Id { get; set; }
    public string Word { get; set; }
    public List<TermLocation> Locations {get;set;}
  }
}

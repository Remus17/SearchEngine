using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Database.Models
{
  public class DirectIndexDocument
  {
    public ObjectId Id { get; set; }
    public string Url { get; set; }
    public List<Term> WordFrequencies { get; set; }

  }
}

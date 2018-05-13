using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Database.Models
{
  public class TermLocation
  {
    public string Location { get; set; }
    public int Count { get; set; }
    public float FinalWeight { get; set; }
  }
}

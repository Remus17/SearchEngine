using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Database.Models
{
  public class Term
  {
    public string Word { get; set; }
    public int Count { get; set; }
    public float Frequency { get; set; }
  }
}

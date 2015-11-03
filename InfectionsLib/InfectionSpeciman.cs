using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public class InfectionSpeciman
  {
    private Infection type;
    private int balance = 0;
    private int spreadCounter = 0;

    public InfectionSpeciman(Infection type)
    {
      this.type = type;
    }

    public Infection Type
    {
      get { return this.type; }
    }

    public int Balance
    {
      get { return this.balance; }
      set { if (!IsDead) { this.balance = value; } }
    }

    public bool IsDead
    {
      get { return this.balance < 0; }
    }

    public int SpreadCounter
    {
      get { return this.spreadCounter; }
      set {
        if (value >= this.type.SpreadSpeed)
        {
          this.spreadCounter = 0;
        }
        else
        {
          this.spreadCounter = value;
        }
      }
    }

    public bool IsSpreadTime
    {
      get { return this.spreadCounter == 0; }
    }
  }
}

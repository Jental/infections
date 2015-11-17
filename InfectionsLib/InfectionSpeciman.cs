using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public class InfectionSpeciman
  {
    public delegate void DeadEventHandler();
    public event DeadEventHandler DeadEvent;

    private Infection type;
    private int balance = 0;
    private int spreadCounter = 0;
    private Guid id = Guid.NewGuid();

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
      set
      {
        if (!IsDead)
        {
          if (this.balance >= 0 && value < 0)
          {
            Logger.Instance.Add("infection", this.GetHashCode().ToString(), "Dead");
            if (this.DeadEvent != null)
            {
              this.DeadEvent();
            }
          }
          Logger.Instance.Add("infection", this.GetHashCode().ToString(), "New Balance: " + value);
          this.balance = value;
        }
      }
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

    public Guid Id
    {
      get { return this.id; }
    }
  }
}

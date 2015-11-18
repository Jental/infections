using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
  public class Result : IComparable
  {
    public Result(ulong steps, TimeSpan time, ulong infected, ulong killed)
    {
      this.steps = steps;
      this.time = time;
      this.killed = killed;
      this.infected = infected;
    }

    public ulong steps;
    public TimeSpan time;
    public ulong infected;
    public ulong killed;

    public override string ToString()
    {
      return String.Format("steps: {0}\t|\ttime: {1}\t|\tinfected: {3}\t|\tkilled: {2}", this.steps, this.time.ToString(), this.infected, this.killed);
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;

      Result casted = obj as Result;
      if (casted == null)
        throw new ArgumentException("Object is not Result");

      if (this.infected == 4 && casted.infected > 4)
      {
        return -1;
      }
      if (this.infected > 4 && casted.infected == 4)
      {
        return 1;
      }

      // return this.steps.CompareTo(casted.steps);
      // return this.infected.CompareTo(casted.infected);
      // return this.infected.CompareTo(casted.killed);
      return this.steps.CompareTo(casted.steps);
    }
  }
}

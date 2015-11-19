using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public static class Consumption
  {
    public const int POWER= 50;

    public static int ofSize(Infection inf)
    {
      return inf.Size;
    }

    public static int ofStore(Infection inf)
    {
      return inf.StoreSize;
    }

    public static int ofSpread(Infection inf, double distance)
    {
      // return inf.Size;
      return inf.Size * 2 + (int)Math.Ceiling(inf.Size * 0.5 * distance);
    }
  }
}

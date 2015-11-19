using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public static class Misc
  {
    public static IEnumerable<T> ToEnumerable<T>(this Array target)
    {
      foreach (var item in target)
        yield return (T)item;
    }

    public static IEnumerable<T> ShuffleNeighbours<T>(this IEnumerable<T> olist, Func<T, int> getHealth, float strengthPref)
    {
      if (olist.Count() == 0) {
        return olist;
      }

      int maxHealth = olist.Max (getHealth);
      int minHealth = olist.Min (getHealth);
      double targetHealth = minHealth + (maxHealth - minHealth) * strengthPref;

      return 
        olist
          .Select ((v) => new KeyValuePair<T, double>(v, Math.Abs (getHealth(v) - targetHealth)))
          .OrderBy ((kvp) => kvp.Value)
          .Select ((kvp) => kvp.Key);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public static class Misc
  {
    public static List<T> Shuffle<T>(this List<T> olist)
    {
      Random rng = new Random();
      List<T> list = olist.ToList();

      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }

      return list;
    }

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

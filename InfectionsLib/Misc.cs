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
  }
}

using InfectionsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections;

namespace Genetic
{
  class Population : List<Infection>
  {
    private const int MIN_AGGRESSION = 0;
    private const int MAX_AGGRESSION = 30;
    private const int MIN_SIZE = 0;
    private const int MAX_SIZE = 10;
    private const int MIN_SPREADAREA = 0;
    private const int MAX_SPREADAREA = 50;
    private const int MIN_SPREADDISTANCE = 0;
    private const int MAX_SPREADDISTANCE = 20;
    private const int MIN_SPREADSPEED = 1;
    private const int MAX_SPREADSPEED = 50;
    private const int MIN_STORESIZE = 0;
    private const int MAX_STORESIZE = 30;

    private int size;
    private Random rnd = new Random();

    public Population(int size)
    {
      this.size = size;
      this.generate();
    }

    public Population(int size, string file)
    {
      this.size = size;

      using (FileStream f = File.OpenRead(file))
      {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Infection>));
        List<Infection> data = (List<Infection>)serializer.ReadObject(f);
        this.AddRange(data);
      }

      this.generate();
    }

    public Population(int size, List<Infection> previous)
    {
      this.size = size;

      // this.AddRange(previous);
      var newGeneration = this.crossover(previous); // crossover
      this.mutate(newGeneration);                   // mutate
      this.AddRange(newGeneration);
      this.generate();                              // if places left - generate random
    }

    public void Generate()
    {
      this.Clear();
      this.generate();
    }

    private void generate()
    {
      for (int i = this.Count; i < this.size; i++)
      {
        Infection inf = new Infection()
        {
          Aggression = rnd.Next(MIN_AGGRESSION, MAX_AGGRESSION),
          Size = rnd.Next(MIN_SIZE, MAX_SIZE),
          SpreadArea = rnd.Next(MIN_SPREADAREA, MAX_SPREADAREA),
          SpreadDistance = rnd.Next(MIN_SPREADDISTANCE, MAX_SPREADDISTANCE),
          SpreadSpeed = rnd.Next(MIN_SPREADSPEED, MAX_SPREADSPEED),
          StoreSize = rnd.Next(MIN_STORESIZE, MAX_STORESIZE)
        };

        this.Add(inf);
      }
    }

    private void mutate(IEnumerable<Infection> infections)
    {
      foreach (Infection inf in infections)
      {
        inf.Aggression += getRandomDiff();
        inf.Size += getRandomDiff();
        inf.SpreadArea += getRandomDiff();
        inf.SpreadDistance += getRandomDiff();
        inf.SpreadSpeed += getRandomDiff();
        inf.StoreSize += getRandomDiff();
      }
    }

    private int getRandomDiff()
    {
      int coeff = 1;
      int maxDiff = 8;
      int maxRnd = (int)Math.Pow(2, maxDiff + 1) - 1;

      BitArray b = new BitArray(new byte[] { (byte)Math.Floor((double)(rnd.Next(0, maxRnd) / coeff)) });
      IEnumerable<bool> casted = b.Cast<bool>();
      for (int i = 0; i < casted.Count(); i++)
      {
        if (casted.ElementAt(i))
        {
          if (i != 0)
          {
            int sign = (rnd.Next(0, 1) == 0) ? 1 : -1;
            return sign * (i + 1) * coeff;
          }
          else
            return (i + 1) * coeff;
        }
      }

      return this.rnd.Next(-8, 8);
    }

    private IEnumerable<Infection> crossover(IEnumerable<Infection> infections)
    {
      int count = infections.Count();

      if (count == 0)
      {
        yield break;
      }

      foreach (Infection inf0 in infections.OrderBy((i) => Guid.NewGuid()))
        foreach (Infection inf1 in infections.OrderBy((i) => Guid.NewGuid()))
          if (inf0 != inf1) 
          {
            if (count >= this.size)
            {
              yield break;
            }

            count++;
            yield return crossover1(inf0, inf1);
          }

      foreach (Infection inf0 in infections.OrderBy((i) => Guid.NewGuid()))
        foreach (Infection inf1 in infections.OrderBy((i) => Guid.NewGuid()))
          if (inf0 != inf1)
          {
            if (count >= this.size)
            {
              yield break;
            }

            count++;
            yield return crossover0(inf0, inf1);
          }
    }

    /// <summary>
    /// Crossover: takes property values randomly from each parent
    /// </summary>
    /// <param name="inf0"></param>
    /// <param name="inf1"></param>
    /// <returns></returns>
    private Infection crossover0(Infection inf0, Infection inf1)
    {
      return new Infection()
      {
        Aggression = (rnd.Next(0, 1) == 0) ? inf0.Aggression : inf1.Aggression,
        Size = (rnd.Next(0, 1) == 0) ? inf0.Size : inf1.Size,
        SpreadArea = (rnd.Next(0, 1) == 0) ? inf0.SpreadArea : inf1.SpreadArea,
        SpreadDistance = (rnd.Next(0, 1) == 0) ? inf0.SpreadDistance : inf1.SpreadDistance,
        SpreadSpeed = (rnd.Next(0, 1) == 0) ? inf0.SpreadSpeed : inf1.SpreadSpeed,
        StoreSize = (rnd.Next(0, 1) == 0) ? inf0.StoreSize : inf1.StoreSize
      };
    }

    /// <summary>
    /// Crossover: takes property values closer to half-sum
    /// </summary>
    /// <param name="inf0"></param>
    /// <param name="inf1"></param>
    /// <returns></returns>
    private Infection crossover1(Infection inf0, Infection inf1)
    {
      int mAggression = (inf0.Aggression + inf1.Aggression) / 2;
      int mSize = (inf0.Size + inf1.Size) / 2;
      int mSpreadArea = (inf0.SpreadArea + inf1.SpreadArea) / 2;
      int mSpreadDistance = (inf0.SpreadDistance + inf1.SpreadDistance) / 2;
      int mSpreadSpeed = (inf0.SpreadSpeed + inf1.SpreadSpeed) / 2;
      int mStoreSize = (inf0.StoreSize + inf1.StoreSize) / 2;

      return new Infection()
      {
        Aggression = (Math.Abs(inf0.Aggression - mAggression) < Math.Abs(inf1.Aggression - mAggression)) ? inf0.Aggression : inf1.Aggression,
        Size = (Math.Abs(inf0.Size - mSize) < Math.Abs(inf1.Size - mSize)) ? inf0.Size : inf1.Size,
        SpreadArea = (Math.Abs(inf0.SpreadArea - mSpreadArea) < Math.Abs(inf1.SpreadArea - mSpreadArea)) ? inf0.SpreadArea : inf1.SpreadArea,
        SpreadDistance = (Math.Abs(inf0.SpreadDistance - mSpreadDistance) < Math.Abs(inf1.SpreadDistance - mSpreadDistance)) ? inf0.SpreadDistance : inf1.SpreadDistance,
        SpreadSpeed = (Math.Abs(inf0.SpreadSpeed - mSpreadSpeed) < Math.Abs(inf1.SpreadSpeed - mSpreadSpeed)) ? inf0.SpreadSpeed : inf1.SpreadSpeed,
        StoreSize = (Math.Abs(inf0.StoreSize - mStoreSize) < Math.Abs(inf1.StoreSize - mStoreSize)) ? inf0.StoreSize : inf1.StoreSize
      };
    }
  }
}

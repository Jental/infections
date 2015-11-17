using InfectionsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

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

      // TODO: replace with crossover+mutation
      this.generate();
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
  }
}

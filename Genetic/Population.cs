﻿using InfectionsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      this.Generate();
    }

    public void Generate()
    {
      this.Clear();

      for (int i = 0; i < size; i++)
      {
        Infection inf = new Infection()
        {
          Agression = rnd.Next(MIN_AGGRESSION, MAX_AGGRESSION),
          Size = rnd.Next(MIN_SIZE, MAX_SIZE),
          SpeadArea = rnd.Next(MIN_SPREADAREA, MAX_SPREADAREA),
          SpreadDistance = rnd.Next(MIN_SPREADDISTANCE, MAX_SPREADDISTANCE),
          SpreadSpeed = rnd.Next(MIN_SPREADSPEED, MAX_SPREADSPEED),
          StoreSize = rnd.Next(MIN_STORESIZE, MAX_STORESIZE)
        };

        this.Add(inf);
      }
    }
  }
}
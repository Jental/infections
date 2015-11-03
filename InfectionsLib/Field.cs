using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public class Field
  {
    public const int SIZE_X = 50;
    public const int SIZE_Y = 50;
    public const int MIN_HEALTH = 20;
    public const int MAX_HEALTH = 100;

    private const int STEP_TIME = 1000;

    private Victim[,] data = new Victim[Field.SIZE_X, Field.SIZE_Y];
    private AutoResetEvent stopEvent = new AutoResetEvent(false);

    public delegate void FieldProgressEventhandler();
    public event FieldProgressEventhandler FieldProgressEvent;

    public Field()
    {
      this.Generate();
    }

    public Victim[,] Data
    {
      get { return this.data; }
    }

    public void Generate()
    {
      Random r = new Random();
      for (int i = 0; i < SIZE_X; i++)
      {
        for (int j = 0; j < SIZE_Y; j++)
        {
          int val = r.Next(Field.MIN_HEALTH, Field.MAX_HEALTH);
          data[i, j] = new Victim(val);
        }
      }
    }

    public void Start()
    {
      Thread t = new Thread(() =>
      {
        while (true)
        {
          for (int i = 0; i < SIZE_X; i++)
          {
            for (int j = 0; j < SIZE_Y; j++)
            {
              Victim v = this.data[i, j];
              if (v.IsInfected && !v.IsDead)
              {
                foreach (InfectionSpeciman inf in v.Infections)
                {
                  int eaten = Math.Min(v.Health, inf.Type.Agression);
                  v.Health -= eaten;
                  inf.Balance += eaten;

                  inf.Balance -= Consumption.ofSize(inf.Type);  // self-feeding
                  inf.Balance -= Consumption.ofStore(inf.Type); // each store slot consumes one point (doesn't store, just consume)

                  inf.SpreadCounter++;
                  if (!inf.IsDead && inf.IsSpreadTime) // infecting random neighbours
                  {
                    List<KeyValuePair<Victim, double>> nb = this.getNeighbours(i, j, inf.Type.SpreadDistance).Where((v1) => !v1.Key.IsInfected && !v1.Key.IsDead && v1.Key.Health < inf.Type.Size * 15).ToList().Shuffle();
                    for (int k = 0; k < inf.Type.SpeadArea; k++)
                    {
                      if (k < nb.Count)
                      {
                        inf.Balance -= Consumption.ofSpread(inf.Type, nb[k].Value);
                        if (!inf.IsDead)
                        {
                          nb[k].Key.Infect(new InfectionSpeciman(inf.Type));
                        }
                        else
                        {
                          break;
                        }
                      }
                    }
                  }

                  if (!inf.IsDead && inf.Balance > 0) // storing left
                  {
                    inf.Balance = Math.Min(inf.Balance, inf.Type.StoreSize);
                  }
                }
              }
            }
          }

          if (this.FieldProgressEvent != null)
          {
            this.FieldProgressEvent();
          }

          if (this.stopEvent.WaitOne(Field.STEP_TIME)) {
            break;
          }
        }
      });
      t.Start();
    }

    public void Stop()
    {
      this.stopEvent.Set();
    }

    private IEnumerable<KeyValuePair<Victim, double>> getNeighbours(int x, int y, int dist)
    {
      double dist2 = Math.Pow(dist, 2);
      for (int i = Math.Max(0, x - dist); i <= Math.Min(Field.SIZE_X, x + dist); i++) {
        for (int j = Math.Max(0, y - dist); j <= Math.Min(Field.SIZE_Y, y + dist); j++)
        {
          double adist = Math.Pow(x - i, 2) + Math.Pow(y - j, 2);
          if (adist <= dist2)
          {
            yield return new KeyValuePair<Victim, double>(this.data[i, j], Math.Sqrt(adist));
          }
        }
      }
    }
  }
}

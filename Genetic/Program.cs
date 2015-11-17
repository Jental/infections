using InfectionsLib;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genetic
{
  class Program
  {
    private struct Point {
      public Point(int x, int y) {
        this.X = x;
        this.Y = y;
      }

      public int X;
      public int Y;
    }

    const int POPULATION_SIZE = 100;
    const int SIZE_X = 20;
    const int SIZE_Y = 20;
    const int MIN_HEALTH = 1;
    const int MAX_HEALTH = 100;
    static Point[] START = new Point[4]
    {
      new Point(SIZE_X / 4, SIZE_Y / 4),
      new Point(SIZE_X * 3 / 4, SIZE_Y / 4),
      new Point(SIZE_X * 3 / 4, SIZE_Y * 3 / 4),
      new Point(SIZE_X / 4, SIZE_Y * 3 / 4)
    };
    const int TOP_SIZE = 10;

    static void start(Field field, Population infections)
    {
      Random rnd = new Random();

      Dictionary<Infection, TimeSpan> top = new Dictionary<Infection,TimeSpan>();
      AutoResetEvent infectionLifeEnded = new AutoResetEvent(false);
      Field.State oldState = field.FieldState;
      field.FieldProgressEvent += () =>
      {
        lock (field)
        {
          Field.State newState = field.FieldState;
          if (newState == Field.State.Stopped && oldState != Field.State.Stopped)
          {
            infectionLifeEnded.Set();
          }
          oldState = field.FieldState;
        }
      };


      foreach (Infection inf in infections)
      {
        field.Reset();
        DateTime startTime = DateTime.Now;

        Console.WriteLine("{0} : Next infection: {1}: \n{2}", startTime.ToString(), inf.Id.ToString(), inf.ToString());

        foreach (Point s in START)
        {
          InfectionSpeciman infs = new InfectionSpeciman(inf);
          infs.DeadEvent += () =>
          {
            DateTime endTime = DateTime.Now;
            Console.WriteLine("{0} : {1} : Dead : Lifespan: {2}", endTime.ToString(), infs.Id.ToString(), endTime - startTime);
          };
          Console.WriteLine("{0} : {1} : Born", startTime.ToString(), infs.Id.ToString());
          field.Data[s.X, s.Y].Infect(infs);
        }

        field.Start();

        infectionLifeEnded.WaitOne();

        DateTime endTime2 = DateTime.Now;
        TimeSpan span = endTime2 - startTime;
        Console.WriteLine("{0} : {1}: End : Lifespan: {2}", endTime2.ToString(), inf.Id.ToString(), span);

        if (top.Count >= TOP_SIZE)
        {
          // Replacing last top entry

          IEnumerable<KeyValuePair<Infection, TimeSpan>> foundInTop = top.Where(((kvp) => kvp.Value < span));
          int count = foundInTop.Count();
          if (count > 0)
          {
            int idx = rnd.Next(0, count - 1);
            KeyValuePair<Infection, TimeSpan> found = foundInTop.ElementAt(idx);
            top.Remove(found.Key);
            top[inf] = span;
          }
        }
        else
        {
          // Top is not filled yet - just adding

          top[inf] = span;
        }
      }

      Console.WriteLine("Top:");
      foreach (KeyValuePair<Infection, TimeSpan> kvp in top)
      {
        Console.WriteLine(kvp.Key);
        Console.WriteLine("\t{0}", kvp.Value);
      }

      Console.ReadLine();
    }

    static void Main(string[] args)
    {
      Field field = null;
      string infectionsFile = null;
      bool showHelp = false;

      OptionSet p = new OptionSet()
           .Add("h|?|help", (v) =>
            {
              showHelp = v != null;
            })
           .Add("f|field=", (v) =>
            {
              if (v != null)
              {
                field = new Field(v);
              }
            })
           .Add("i|infections=", (v) => {
             if (v != null)
             {
               infectionsFile = v;
             }
           })
           .Add("nolog", (v) => {
             Logger.Instance.Disabled = true;
           });
      List<string> extra = p.Parse(args);

      if (showHelp)
      {
        Console.WriteLine(p.ToString());
      }
      else
      {
        if (field == null)
        {
          field = new Field(SIZE_X, SIZE_Y, MIN_HEALTH, MAX_HEALTH);
        }

        Population infections =
          (infectionsFile == null)
          ? new Population(POPULATION_SIZE)
          : new Population(POPULATION_SIZE, infectionsFile);

        start(field, infections);
      }
    }
  }
}

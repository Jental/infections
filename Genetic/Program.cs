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
    const int SIZE_X = 100;
    const int SIZE_Y = 100;
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
    const ulong STEP_LIMIT = UInt64.MaxValue;

    static Infection start(Field field, Population infections)
    {
      List<Infection> previous = null;
      Dictionary<Infection, Result> top = new Dictionary<Infection, Result>();

      while (true)
      {
        Dictionary<Infection, Result> res;
        if (previous == null) {
           res = startRound(field, infections, top);
        }
        else {
          Population newInfections = new Population(infections.Count, previous);
          res = startRound(field, newInfections, top);
        }

        KeyValuePair<Infection, Result> maxResult = res.OrderByDescending((kvp) => kvp.Value).First();
        if (maxResult.Value.steps >= STEP_LIMIT)
        {
          Infection found = maxResult.Key;
          return found;
        }

        previous = 
          res
          // .Where((kvp) => kvp.Value >= previousMax)
          .Select((kvp) => kvp.Key).ToList();
      }
    }

    static Dictionary<Infection, Result> startRound(Field fieldO, Population infections, Dictionary<Infection, Result> top)
    {
      Random rnd = new Random();

      AutoResetEvent allEnded = new AutoResetEvent(false);
      int counter = 0;

      foreach (Infection inf in infections)
        ThreadPool.QueueUserWorkItem((state) =>
      {
        Field field = (Field)fieldO.Clone();
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

        // field.Reset();
        DateTime startTime = DateTime.Now;

        // Console.WriteLine("{0} : Next infection: {1}: \n{2}", startTime.ToString(), inf.Id.ToString(), inf.ToString());

        foreach (Point s in START)
        {
          InfectionSpeciman infs = new InfectionSpeciman(inf);
          //infs.DeadEvent += () =>
          //{
          //  DateTime endTime = DateTime.Now;
          //  Console.WriteLine("{0} : {1} : Dead : Lifespan: {2}", endTime.ToString(), infs.Id.ToString(), endTime - startTime);
          //};
          //Console.WriteLine("{0} : {1} : Born", startTime.ToString(), infs.Id.ToString());
          field.Data[s.X, s.Y].Infect(infs);
        }

        field.Start();

        infectionLifeEnded.WaitOne();

        Result singleResult = new Result(field.Step, DateTime.Now - startTime, field.InfectedCount, field.DeadCount);
        // Console.WriteLine("{0} : {1}: End : Lifespan: {2}", endTime2.ToString(), inf.Id.ToString(), span);

        lock (top)
        {
          if (top.Count((kvp) => kvp.Key.Equals(inf)) == 0) // Top should contain unique elements
          {
            if (top.Count >= TOP_SIZE)
            {
              // Replacing worst top entry

              IEnumerable<KeyValuePair<Infection, Result>> foundInTop = top.Where(((kvp) => kvp.Value.CompareTo(singleResult) <= 0));
              int count = foundInTop.Count();
              if (count > 0)
              {
                int idx = rnd.Next(0, count - 1);
                KeyValuePair<Infection, Result> found = foundInTop.ElementAt(idx);
                top.Remove(found.Key);
                top[inf] = singleResult;
              }
            }
            else
            {
              // Top is not filled yet - just adding

              top[inf] = singleResult;
            }
          }

          counter++;
          if (counter == infections.Count)
          {
            allEnded.Set();
          }
        }
      });

      allEnded.WaitOne();

      Console.Clear();
      Console.WriteLine("Top {0}:", DateTime.Now.ToString());
      foreach (KeyValuePair<Infection, Result> kvp in top)
      {
        Console.WriteLine(kvp.Key);
        Console.WriteLine("\t{0}", kvp.Value.ToString());
      }

      return top;
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

        Infection res = start(field, infections);

        Console.WriteLine("Winner: {0}", res.ToString());
        Console.ReadLine();
      }
    }
  }
}

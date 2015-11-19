using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfectionsLib
{
  [Serializable()]
  public class Field: ISerializable, ICloneable
  {
    private int sizeX = 50;
    private int sizeY = 50;

    private Guid id = Guid.NewGuid();

    private const int STEP_TIME = 0;

    private Victim[,] data;
    private AutoResetEvent stopEvent = new AutoResetEvent(false);
    private ulong step = 0;

    public enum State {
      Started,
      Stopped
    }
    private State state = State.Stopped;

    public delegate void FieldProgressEventhandler();
    public event FieldProgressEventhandler FieldProgressEvent;

    private Field()
    {
    }

    public Field(int sizeX, int sizeY, int minHealth, int maxHealth)
    {
      this.sizeX = sizeX;
      this.sizeY = sizeY;

      this.data = new Victim[this.sizeX, this.sizeY];

      this.Generate(minHealth, maxHealth);
    }

    public Field(string path)
    {
      this.Load(path);
    }

    public Field(SerializationInfo info, StreamingContext context)
    {
      this.data = (Victim[,])info.GetValue("field", typeof(Victim[,]));
    }

    public Victim[,] Data
    {
      get { return this.data; }
    }

    public ulong Step {
      get { return this.step; }
    }

    public State FieldState {
      get { return this.state; }
    }

    public ulong DeadCount
    {
      get
      {
        ulong result = 0;

        for (int i = 0; i < this.sizeX; i++)
          for (int j = 0; j < this.sizeY; j++)
            if (this.data[i, j].IsDead)
              result++;

        return result;
      }
    }

    public ulong InfectedCount
    {
      get
      {
        ulong result = 0;

        for (int i = 0; i < this.sizeX; i++)
          for (int j = 0; j < this.sizeY; j++)
            if (this.data[i, j].IsInfected)
              result++;

        return result;
      }
    }

    public ulong InfectedAndDeadCount
    {
      get
      {
        ulong result = 0;

        for (int i = 0; i < this.sizeX; i++)
          for (int j = 0; j < this.sizeY; j++)
            if (this.data[i, j].IsInfected && this.data[i, j].IsDead)
              result++;

        return result;
      }
    }

    public void Generate(int minHealth, int maxHealth)
    {
      if (this.state == State.Stopped)
      {
        Random r = new Random();
        for (int i = 0; i < sizeX; i++)
        {
          for (int j = 0; j < sizeY; j++)
          {
            int val = r.Next(minHealth, maxHealth);
            data[i, j] = new Victim(val);
          }
        }

        if (this.FieldProgressEvent != null)
        {
          this.FieldProgressEvent();
        }
      }
    }

    public void Reset()
    {
      if (this.state == State.Stopped)
      {
        for (int i = 0; i < sizeX; i++)
        {
          for (int j = 0; j < sizeY; j++)
          {
            data[i, j].Reset();
          }
        }
      }

    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("field", this.data, typeof(Victim[,]));
    }

    public void Save(string path)
    {
      string fullPath = Path.GetFullPath(path);
      string dirName = Path.GetDirectoryName(fullPath);
      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }

      using (FileStream f = File.OpenWrite(path))
      {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(f, this);
      }
    }

    public void Load(string path)
    {
      if (File.Exists(path) && this.state == State.Stopped)
      {
        using (FileStream f = File.OpenRead(path))
        {
          BinaryFormatter bf = new BinaryFormatter();
          Field loadedField = (Field)bf.Deserialize(f);
          this.data = loadedField.data;
          this.step = 0;
          this.sizeX = this.data.GetLength(0);
          this.sizeY = this.data.GetLength(1);
        }

        if (this.FieldProgressEvent != null)
        {
          this.FieldProgressEvent();
        }
      }
    }

    public void Start()
    {
      if (this.state != State.Stopped) {
        return;
      }

      this.state = State.Started;

      Thread t = new Thread(() =>
      {
        Logger.Instance.Add("global", "", "----Start----");
        bool activityPresent = true;
        while (activityPresent)
        {
          Logger.Instance.Add("global", "", "-----");

          activityPresent = false;

          for (int i = 0; i < sizeX; i++)
          {
            for (int j = 0; j < sizeY; j++)
            {
              Victim v = this.data[i, j];
              if (v.IsInfected && !v.IsDead)
              {
                foreach (InfectionSpeciman inf in v.Infections)
                {
                  if (inf.IsDead)
                  {
                    continue;
                  }
                  activityPresent = true;

                  Logger.Instance.Add("infection", inf.GetHashCode().ToString(), String.Format("On victim cell: {0}x{1}", i, j));

                  int eaten = Math.Min(v.Health, inf.Type.Aggression);
                  v.Health -= eaten;
                  inf.Balance += eaten;
                  Logger.Instance.Add("infection", inf.GetHashCode().ToString(), "Eaten: " + eaten);

                  inf.Balance -= Consumption.ofSize(inf.Type);  // self-feeding
                  inf.Balance -= Consumption.ofStore(inf.Type); // each store slot consumes one point (doesn't store, just consume)
                  Logger.Instance.Add("infection", inf.GetHashCode().ToString(), "Spent on self: " + Consumption.ofSize(inf.Type));
                  Logger.Instance.Add("infection", inf.GetHashCode().ToString(), "Spent on store: " + Consumption.ofStore(inf.Type));

                  inf.SpreadCounter++;
                  if (!inf.IsDead && inf.IsSpreadTime)
                  {
                    List<KeyValuePair<Victim, double>> nb = 
                      this.getNeighbours(i, j, inf.Type.SpreadDistance)
                          .Where((v1) => !v1.Key.IsInfected && !v1.Key.IsDead && v1.Key.Health <= inf.Type.Size * Consumption.POWER)
                          .ShuffleNeighbours((kvp) => kvp.Key.Health, inf.Type.StrengthPref)
                          .ToList();
                    for (int k = 0; k < inf.Type.SpreadArea; k++)
                    {
                      if (k < nb.Count)
                      {
                        inf.Balance -= Consumption.ofSpread(inf.Type, nb[k].Value);
                        Logger.Instance.Add("infection", inf.GetHashCode().ToString(), "Spent on spread: " + Consumption.ofSpread(inf.Type, nb[k].Value));
                        if (!inf.IsDead)
                        {
                          InfectionSpeciman newSpec = inf.Clone();
                          nb[k].Key.Infect(newSpec);
                          Logger.Instance.Add("infection", inf.GetHashCode().ToString(), String.Format("Spread to victim with health: {0}", nb[k].Key.Health));
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

                if (!v.IsDead)
                {
                  List<InfectionSpeciman> dead = v.Infections.Where((inf) => inf.IsDead).ToList();
                  foreach (var inf in dead)
                  {
                    v.Cure(inf);
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
          this.step++;
        }

        Logger.Instance.Add("global", "", "-----End-----");
        this.state = State.Stopped;

        if (this.FieldProgressEvent != null)
        {
          this.FieldProgressEvent();
        }
      });
      t.Start();
    }

    public void Stop()
    {
      Logger.Instance.Add("global", "", "----Stop----");
      this.stopEvent.Set();
    }

    private IEnumerable<KeyValuePair<Victim, double>> getNeighbours(int x, int y, int dist)
    {
      double dist2 = Math.Pow(dist, 2);
      for (int i = Math.Max(0, x - dist); i <= Math.Min(this.sizeX - 1, x + dist); i++) {
        for (int j = Math.Max(0, y - dist); j <= Math.Min(this.sizeY - 1, y + dist); j++)
        {
          double adist = Math.Pow(x - i, 2) + Math.Pow(y - j, 2);
          if (adist <= dist2)
          {
            yield return new KeyValuePair<Victim, double>(this.data[i, j], Math.Sqrt(adist));
          }
        }
      }
    }

    public object Clone()
    {
      Victim[,] newData = (Victim[,])this.data.Clone();
      for (int i =0; i< this.sizeX; i++ )
        for(int j=0; j<this.sizeY; j++)
          newData[i, j] = (Victim)newData[i, j].Clone();

      Field newField = new Field()
      {
        sizeX = this.sizeX,
        sizeY = this.sizeY,
        data = newData,
        state = State.Stopped,
        step = 0
      };

      // newField.FieldProgressEvent += this.FieldProgressEvent;

      return newField;
    }
  }
}

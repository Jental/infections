using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  [Serializable()]
  public class Victim : ISerializable, ICloneable
  {
    private int maxHealth;
    private int health;

    private HashSet<InfectionSpeciman> infections = new HashSet<InfectionSpeciman>();

    public Victim(int maxHealth)
    {
      this.maxHealth = maxHealth;
      this.health = maxHealth;
    }

    public Victim(SerializationInfo info, StreamingContext context)
    {
      this.maxHealth = (int)info.GetValue("max_health", typeof(int));
      this.health = maxHealth;
    }

    public int MaxHealth
    {
      get { return this.maxHealth; }
    }

    public int Health
    {
      get { return this.health; }
      set
      {
        if (value <= 0)
        {
          if (this.health > 0)
          {
            Logger.Instance.Add("victim", this.GetHashCode().ToString(), "Dead");
            foreach (InfectionSpeciman inf in this.infections)
            {
              inf.Balance = -1; // Killing all infections
            }
          }
          this.health = 0;
        }
        else
        {
          this.health = value;
        }
      }
    }

    public void Infect(InfectionSpeciman inf)
    {
      Logger.Instance.Add("victim", this.GetHashCode().ToString(), String.Format("Infected with {0}", inf.GetHashCode()));
      this.infections.Add(inf);
    }

    public void Cure(InfectionSpeciman inf)
    {
      Logger.Instance.Add("victim", this.GetHashCode().ToString(), String.Format("Cured from {0}", inf.GetHashCode()));
      this.infections.Remove(inf);
    }

    public void Reset()
    {
      // Logger.Instance.Add("victim", this.GetHashCode().ToString(), "Resetted");
      this.infections.Clear();
      this.health = this.maxHealth;
    }

    public IEnumerable<InfectionSpeciman> Infections
    {
      get { return this.infections; }
    }

    public bool IsInfected
    {
      get { return this.infections.Count > 0; }
    }

    public bool IsDead
    {
      get { return this.health <= 0; }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("max_health", this.maxHealth);
    }

    public object Clone()
    {
      return new Victim(this.maxHealth);
    }
  }
}

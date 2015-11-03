using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public class Victim
  {
    private int maxHealth;
    private int health;

    private HashSet<InfectionSpeciman> infections = new HashSet<InfectionSpeciman>();

    public Victim(int maxHealth)
    {
      this.maxHealth = maxHealth;
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
        if (value < 0)
        {
          if (this.health > 0)
          {
            Logger.Instance.Add("victim", this.GetHashCode().ToString(), "Dead");
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
  }
}

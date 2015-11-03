using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public class Infection
  {
    private int size = 1;
    private int storeSize = 1;
    private int agression = 1;
    private int spreadDistance = 1;
    private int speadArea = 1;

    public int Size
    {
      get { return size; }
      set { size = value; }
    }

    public int StoreSize
    {
      get { return storeSize; }
      set { storeSize = value; }
    }

    public int Agression
    {
      get { return agression; }
      set { agression = value; }
    }
    private int spreadSpeed = 1;

    public int SpreadSpeed
    {
      get { return spreadSpeed; }
      set { spreadSpeed = value; }
    }

    public int SpreadDistance
    {
      get { return spreadDistance; }
      set { spreadDistance = value; }
    }

    public int SpeadArea
    {
      get { return speadArea; }
      set { speadArea = value; }
    }

  }
}

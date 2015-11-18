using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace InfectionsLib
{
  [DataContract]
  public class Infection
  {
    private int size = 1;
    private int storeSize = 1;
    private int aggression = 1;
    private int spreadDistance = 1;
    private int spreadArea = 1;
    private Guid id = Guid.NewGuid();

    [OnDeserializing]
    private void onDeserializing(StreamingContext c)
    {
      this.id = Guid.NewGuid();
    }

    [DataMember]
    public int Size
    {
      get { return size; }
      set { size = value; }
    }

    [DataMember]
    public int StoreSize
    {
      get { return storeSize; }
      set { storeSize = value; }
    }

    [DataMember]
    public int Aggression
    {
      get { return aggression; }
      set { aggression = value; }
    }
    private int spreadSpeed = 1;

    [DataMember]
    public int SpreadSpeed
    {
      get { return spreadSpeed; }
      set { spreadSpeed = value; }
    }

    [DataMember]
    public int SpreadDistance
    {
      get { return spreadDistance; }
      set { spreadDistance = value; }
    }

    [DataMember]
    public int SpreadArea
    {
      get { return spreadArea; }
      set { spreadArea = value; }
    }

    [IgnoreDataMember]
    public Guid Id
    {
      get
      {
        return this.id;
      }
    }

    public override bool Equals(object obj)
    {
      if (obj.GetType() != typeof(Infection))
        return false;

      Infection casted = (Infection)obj;
      return
        this.aggression == casted.aggression
        && this.size == casted.size
        && this.storeSize == casted.storeSize
        && this.spreadArea == casted.spreadArea
        && this.spreadDistance == casted.spreadDistance
        && this.spreadSpeed == casted.spreadSpeed;
    }

    public override string ToString()
    {
      return String.Format(
        "{{Id: {0}, Size: {1}, StoreSize: {2}, Aggression: {3}, SpreadSpeed: {4}, SpreadDistance: {5}, SpreadArea: {6}}}",
        this.id,
        this.Size,
        this.StoreSize,
        this.Aggression,
        this.SpreadSpeed,
        this.SpreadDistance,
        this.SpreadArea
        );
    }

  }
}

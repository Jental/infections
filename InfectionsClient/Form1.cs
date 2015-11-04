using InfectionsLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infections
{
  public partial class Form1 : Form
  {
    private const int FIELD_ITEM_SIZE = 10;

    Field field;
    Dictionary<Infection, Color> infections = new Dictionary<Infection, Color>();

    public Form1()
    {
      InitializeComponent();
      Random rand = new Random();

      this.field = new Field();
      Infection inf1 = new Infection()
      {
        Size = 5,
        StoreSize = 2,
        Agression = 12,
        SpreadSpeed = 3,
        SpreadDistance = 1,
        SpeadArea = 1
      };
      InfectionSpeciman s1 = new InfectionSpeciman(inf1);
      this.infections.Add(inf1, Color.Red);
      Victim v = field.Data[rand.Next(0, Field.SIZE_X), rand.Next(0, Field.SIZE_Y)];
      //v.Health = Field.MAX_HEALTH;
      v.Infect(s1);

      Infection inf2 = new Infection()
      {
        Size = 1,
        StoreSize = 4,
        Agression = 9,
        SpreadSpeed = 5,
        SpreadDistance = 2,
        SpeadArea = 2
      };
      this.infections.Add(inf2, Color.Green);
      InfectionSpeciman s2 = new InfectionSpeciman(inf2);
      field.Data[rand.Next(0, Field.SIZE_X), rand.Next(0, Field.SIZE_Y)].Infect(s2);

      field.FieldProgressEvent += field_FieldProgressEvent;

      Infection inf3 = new Infection()
      {
          Size = 1,
          StoreSize = 2,
          Agression = 5,
          SpreadSpeed = 5,
          SpreadDistance = 1,
          SpeadArea = 1
      };
      this.infections.Add(inf3, Color.DarkOrange);
      InfectionSpeciman s3 = new InfectionSpeciman(inf3);
      field.Data[rand.Next(0, Field.SIZE_X), rand.Next(0, Field.SIZE_Y)].Infect(s3);
    }

    private void field_FieldProgressEvent()
    {
      this.drawField();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.drawField();
      this.field.Start();
    }

    private void drawField()
    {
      int infectionOffset = 3;

      if (this.IsDisposed || this.panel1.IsDisposed)
      {
        return;
      }

      using (Graphics g = this.panel1.CreateGraphics())
      {
        // g.Clear(Color.White);

        for (int i = 0; i < Field.SIZE_X; i++)
        {
          for (int j = 0; j < Field.SIZE_Y; j++)
          {
            Victim v = this.field.Data[i, j];
            int colorComponent = 255 * v.Health / (Field.MAX_HEALTH + 1);

            if (v.IsDead)
            {
              using (Pen p = new Pen(new SolidBrush(Color.Black)))
              using (Brush b = new SolidBrush(Color.White))
              {
                g.FillRectangle (b, i * FIELD_ITEM_SIZE, j * FIELD_ITEM_SIZE, FIELD_ITEM_SIZE, FIELD_ITEM_SIZE);
                g.DrawLine(p, i * FIELD_ITEM_SIZE, j * FIELD_ITEM_SIZE, i * FIELD_ITEM_SIZE + FIELD_ITEM_SIZE - 1, j * FIELD_ITEM_SIZE + FIELD_ITEM_SIZE -1);
                g.DrawLine(p, i * FIELD_ITEM_SIZE, j * FIELD_ITEM_SIZE + FIELD_ITEM_SIZE - 1, i * FIELD_ITEM_SIZE + FIELD_ITEM_SIZE -1, j * FIELD_ITEM_SIZE);
              }
            }
            else
            {
              using (Brush b = new SolidBrush(Color.FromArgb(colorComponent, colorComponent, colorComponent)))
              {
                g.FillRectangle(b, i * FIELD_ITEM_SIZE, j * FIELD_ITEM_SIZE, FIELD_ITEM_SIZE, FIELD_ITEM_SIZE);
              }
            }

            if (v.IsInfected)
            {
              using (Brush b = new SolidBrush(this.infections[v.Infections.ElementAt(0).Type]))
              {
                g.FillRectangle(b, i * FIELD_ITEM_SIZE + infectionOffset, j * FIELD_ITEM_SIZE + infectionOffset, FIELD_ITEM_SIZE - 2 * infectionOffset, FIELD_ITEM_SIZE - 2 * infectionOffset);
              }
            }
          }
        }
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.field.Stop();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.field.Stop();
    }
  }
}

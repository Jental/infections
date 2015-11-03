using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfectionsLib
{
  public class Logger
  {
    private const string LOG_PATH = "infections.log";
    private const string LOG_FORMAT = "{0} : {1} : {2} : {3}";

    private static Logger instance = new Logger();

    private Logger()
    {
    }

    public static Logger Instance
    {
      get { return Logger.instance; }
    }

    public void Add(string type, string id, string str)
    {
      using (StreamWriter f = new StreamWriter(Logger.LOG_PATH, true))
      {
        f.WriteLine(String.Format(Logger.LOG_FORMAT, DateTime.Now, type, id, str));
      }
    }
  }
}

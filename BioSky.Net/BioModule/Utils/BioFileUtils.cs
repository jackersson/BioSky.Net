using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BioModule.Utils
{
  public class BioFileUtils
  {
    private string GetParametr(string parametr)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory + "config.txt";
      FileInfo configFile = new FileInfo(path);

      using (StreamReader sr = new StreamReader(path))
      {
        bool hasParametr = false;
        string sub;
        while (!sr.EndOfStream)
        {
          var line = sr.ReadLine();
          if (!hasParametr)
          {
            if (line.StartsWith(parametr))
            {
              hasParametr = true;
              if (line.Length == parametr.Length)
              {
                Console.WriteLine("Path is not set");
                return null;
              }

              sub = line.Substring(parametr.Length, line.Length - parametr.Length);
              Console.WriteLine(sub);
              return (sub);
            }
          }
        }
      }
      return null;
    }

    private void GetConfigFile(string[] allParametrs)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory + "config.txt";
      FileInfo configFile = new FileInfo(path);

      if (!configFile.Exists)
      {
        foreach (string parametr in allParametrs)
        {
          using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
          using (StreamWriter sw = new StreamWriter(fs))
          {
            sw.WriteLine(parametr);
          }
        }
      }
    }




  }
}

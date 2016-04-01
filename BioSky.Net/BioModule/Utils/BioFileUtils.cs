using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace BioModule.Utils
{
  public class BioFileUtils
  {

    public OpenFileDialog OpenFileDialog()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "All files (*.*)|*.*";
      openFileDialog.InitialDirectory = Environment.CurrentDirectory;

      return openFileDialog;
    }

    public OpenFileDialog OpenFileDialogWithMultiselect(string directoryPath)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = true;
      openFileDialog.Filter = "Txt files (*.txt*)|*.txt*";
      openFileDialog.InitialDirectory = directoryPath;

      return openFileDialog;
    }
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

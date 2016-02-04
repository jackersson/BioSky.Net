using BioContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

namespace BioData
{
  public class BioLocalStorage : PropertyChangedBase, ILocalStorage
  {

    public BioLocalStorage()
    {
      string mediaParametr = "MEDIA_PATHWAY:";
      string mediaPath = GetConfigFile(mediaParametr);

      if (mediaPath == null)      
        LocalStoragePath = "F:\\GRPCs\\ClientFolder";

      LocalStoragePath = mediaPath;
    }

    private string _localStoragePath;
    public string LocalStoragePath
    {
      get { return _localStoragePath; }
      set
      {
        if (_localStoragePath != value)
        {
          _localStoragePath = value;
          NotifyOfPropertyChange(() => LocalStoragePath); 
        }
      }
    }
  
    //TODO to Utils not Repository
    private string GetConfigFile(string parametr)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory + "config.txt";
      FileInfo configFile = new FileInfo(path);

      if (!configFile.Exists)
      {   
        using (StreamWriter streamWriter = configFile.CreateText())        
          streamWriter.WriteLine(parametr);       
      }

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
  }
}

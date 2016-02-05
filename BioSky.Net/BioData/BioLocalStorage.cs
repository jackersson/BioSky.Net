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
      string mediaParametr           = "MEDIA_PATHWAY:"   ;
      string faceServiceParametr     = "FACE_SERVICE:"    ;
      string databaseServiceParametr = "DATABASE_SERVICE:";

      string[] allParametrs = { mediaParametr, faceServiceParametr, databaseServiceParametr };

      GetConfigFile(allParametrs);
      string mediaPath           = GetParametr(mediaParametr)          ;
      string faceServicePath     = GetParametr(faceServiceParametr)    ;
      string databaseServicePath = GetParametr(databaseServiceParametr);

      if (mediaPath == null)      
        LocalStoragePath = "F:\\GRPCs\\ClientFolder";

      if (mediaPath == null)
        LocalStoragePath = "";

      if (mediaPath == null)
        LocalStoragePath = "";

      LocalStoragePath           = mediaPath          ;
      FaceServiceStoragePath     = faceServicePath    ;
      DatabaseServiceStoragePath = databaseServicePath;
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

    private string _faceServiceStoragePath;
    public string FaceServiceStoragePath
    {
      get { return _faceServiceStoragePath; }
      set
      {
        if (_faceServiceStoragePath != value)
        {
          _faceServiceStoragePath = value;
          NotifyOfPropertyChange(() => FaceServiceStoragePath);
        }
      }
    }

    private string _databaseServiceStoragePath;
    public string DatabaseServiceStoragePath
    {
      get { return _databaseServiceStoragePath; }
      set
      {
        if (_databaseServiceStoragePath != value)
        {
          _databaseServiceStoragePath = value;
          NotifyOfPropertyChange(() => DatabaseServiceStoragePath);
        }
      }
    }

    //TODO Make in Utils

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

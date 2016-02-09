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
    private static readonly string mediaParametr           = "MEDIA_PATHWAY:"   ;
    private static readonly string faceServiceParametr     = "FACE_SERVICE:"    ;
    private static readonly string databaseServiceParametr = "DATABASE_SERVICE:";
    private static readonly string languageParametr        = "LANGUAGE:"        ;
    private string[] allParametrs = { mediaParametr, faceServiceParametr, databaseServiceParametr, languageParametr };


    public BioLocalStorage()
    {
      GetConfigFile(allParametrs);

      string mediaPath           = GetParametr(mediaParametr)          ;
      string faceServicePath     = GetParametr(faceServiceParametr)    ;
      string databaseServicePath = GetParametr(databaseServiceParametr);
      string language            = GetParametr(languageParametr);

      LocalStoragePath = mediaPath;
      FaceServiceStoragePath = faceServicePath;
      DatabaseServiceStoragePath = databaseServicePath;
      Language = language;

      if (mediaPath == null)      
        LocalStoragePath = "F:\\GRPCs\\ClientFolder\\";

      if (faceServicePath == null)
        FaceServiceStoragePath = "192.168.1.127:50051";

      if (databaseServicePath == null)
        DatabaseServiceStoragePath = "192.168.1.178:50051";

      if (Language == null)
        Language = "en";


    }
    public void SaveGeneralSettings(string local, string face, string service, string language)
    {
      LocalStoragePath           = local   ;
      FaceServiceStoragePath     = face    ;
      DatabaseServiceStoragePath = service ;
      Language                   = language;

      string[] newData = { local, face, service, language };
      
      SetParametr(newData, allParametrs);

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

    private string _language;
    public string Language
    {
      get { return _language; }
      set
      {
        if (_language != value)
        {
          _language = value;
          NotifyOfPropertyChange(() => Language);
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

    private void SetParametr(string[] data, string[] allParametrs)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory + "config.txt";
      FileInfo configFile = new FileInfo(path);

      if (configFile.Exists)
      {
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
          using (StreamWriter sw = new StreamWriter(fs))
          {
            for (int i = 0; i != 4; ++i)            
              sw.WriteLine(allParametrs[i] + data[i]);          
          }
        }
      }
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

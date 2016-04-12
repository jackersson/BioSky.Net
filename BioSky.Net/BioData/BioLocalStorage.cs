using BioContracts;
using System;
using System.IO;

using Caliburn.Micro;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace BioData
{
  public class BioLocalStorage : PropertyChangedBase, ILocalStorage
  {

    public BioLocalStorage()
    {
      LoadConfiguration();

      //"192.168.1.127:50052" Taras
      //"192.168.1.178:50051"  Serg
    }

    public string GetParametr(ConfigurationParametrs parametr)
    {
      if (Configuration == null)
        return string.Empty;

      ConfigurationValue value = null;
      if (Configuration.TryGetValue(parametr, out value))
        return value.Current;

      return string.Empty;
    }

    public void UpdateParametr(ConfigurationParametrs parametr, string value)
    {
      if (Configuration.ContainsKey(parametr))
      {
        Configuration[parametr].Current = value;
        SaveConfiguration();
      }
    }


    private void LoadConfiguration()
    {
      if (File.Exists(ConfigurationFilePath))
      {
        GenerateDefault();
        return;
      }

      try
      {
        using (FileStream fs = new FileStream(ConfigurationFilePath, FileMode.Open, FileAccess.Read))
        {
          Configuration = Deserialize(fs);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private void SaveConfiguration()
    {
      if (_configuration.Count <= 0)
        GenerateDefault();

      try
      {
        using (FileStream fs = new FileStream(ConfigurationFilePath, FileMode.Create, FileAccess.Write))
        {
          Serialize(Configuration, fs);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private void GenerateDefault()
    {
      Configuration.Clear();

      Configuration.Add(ConfigurationParametrs.MediaPathway, new ConfigurationValue()
      {
          Current = "F:\\Biometric Software\\Client\\"
        , Default = AppDomain.CurrentDomain.BaseDirectory + "\\"
      });

      Configuration.Add(ConfigurationParametrs.FaceServiceAddress, new ConfigurationValue()
      {
         Current = "192.168.1.178:50052"
        , Default = "127.0.0.1:50051"
      });

      Configuration.Add(ConfigurationParametrs.DatabaseServiceAddress, new ConfigurationValue()
      {
          Current = "127.0.0.1:50051"
        , Default = "127.0.0.1:50051"
      });

      Configuration.Add(ConfigurationParametrs.Language, new ConfigurationValue()
      {
          Current = "en"
        , Default = "en"
      });

      Configuration.Add(ConfigurationParametrs.LogsFilePathway, new ConfigurationValue()
      {
          Current = AppDomain.CurrentDomain.BaseDirectory + "Log\\"
        , Default = AppDomain.CurrentDomain.BaseDirectory + "Log\\"
      });
    }

    private void Serialize(Dictionary<ConfigurationParametrs, ConfigurationValue> dictionary, Stream stream)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(stream, Configuration);
    }

    private Dictionary<ConfigurationParametrs, ConfigurationValue> Deserialize(Stream stream)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      var key = formatter.Deserialize(stream);
      Dictionary<ConfigurationParametrs, ConfigurationValue> value = (Dictionary<ConfigurationParametrs, ConfigurationValue>)key;
      return value;
    }

    private string ConfigurationFilePath
    {
      get { return AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_FILE_NAME; }
    }
    public string LogFileFormat { get { return ".txt"; } }

    [Serializable]
    public class ConfigurationValue
    {
      public string Default { get; set; }
      public string Current { get; set; }
    }

    private Dictionary<ConfigurationParametrs, ConfigurationValue> _configuration;
    private Dictionary<ConfigurationParametrs, ConfigurationValue> Configuration
    {
      get
      {
        if (_configuration == null)
          _configuration = new Dictionary<ConfigurationParametrs, ConfigurationValue>();

        return _configuration;
      }
      set
      {
        if (_configuration != value)
          _configuration = value;
      }
    }

    private const string CONFIGURATION_FILE_NAME = "config.txt";
  }
}

namespace BioContracts
{
  public enum ConfigurationParametrs
  {
      MediaPathway
     , FaceServiceAddress
     , DatabaseServiceAddress
     , Language
     , LogsFilePathway
  }

  public interface ILocalStorage
  {
    string GetParametr(ConfigurationParametrs parametr);

    void UpdateParametr(ConfigurationParametrs parametr, string value);

    string LogFileFormat { get; }
  }
}

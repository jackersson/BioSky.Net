using BioContracts;
using System;
using System.IO;

namespace BioData.Holders.Utils
{
  public class IOUtils
  {
    public IOUtils(ILocalStorage localStorage)
    {
      _localStorage = localStorage;
    }

    public bool FileExists( string localPath)
    {
      return File.Exists(_localStorage.GetParametr(ConfigurationParametrs.MediaPathway) + localPath );
    }

    public void SaveFile( string localPath, byte[] bytes )
    {
      try {

        if (FileExists(localPath))
          return;

        string destinationPath = _localStorage.GetParametr(ConfigurationParametrs.MediaPathway) + localPath;

        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

        var fs = new BinaryWriter(new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write));
        fs.Write(bytes);
        fs.Close();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      
    }

    private readonly ILocalStorage _localStorage;
  }
}

using BioContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      return File.Exists( _localStorage.LocalStoragePath + localPath );
    }

    public void SaveFile( string localPath, byte[] bytes )
    {
      try {
        string destinationPath = _localStorage.LocalStoragePath + localPath;

        if (FileExists(destinationPath))
          return;

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

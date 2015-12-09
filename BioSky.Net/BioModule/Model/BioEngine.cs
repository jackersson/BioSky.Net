using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioData;

namespace BioModule.Model
{
  public class BioEngine : IBioEngine
  {
    public BioEngine( IBioSkyNetRepository data )
    {
      _data = data;
      System.Console.WriteLine("Here");

      foreach (User u in data.getAllUsers())
      {
        Console.WriteLine("{0}.{1} - {2}", u.UID, u.First_Name_, u.Last_Name_);
      }
    }

    public IBioSkyNetRepository Database()
    {
      return _data;
    }

    private readonly IBioSkyNetRepository _data;
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BioData;

namespace BioModule.Model
{
  public interface IBioEngine
  {

    IBioSkyNetRepository Database();
  }
}

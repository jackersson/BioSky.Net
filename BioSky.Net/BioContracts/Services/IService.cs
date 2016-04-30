using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IService
  {
    void Start();
    void Start(string address);
    void Stop();
  
  }
}

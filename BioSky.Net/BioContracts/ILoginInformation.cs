using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface ILoginInformation
  {
    void UpdateUser(Person user);

    object LoginInformation { get; }
  }
}

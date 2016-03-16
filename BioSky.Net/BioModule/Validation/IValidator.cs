using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Validation
{
  public interface IValidator
  {
    IEnumerable<Error> Validate(object instance);
    IEnumerable<Error> Validate(object instance, string propertyName);
  }
}

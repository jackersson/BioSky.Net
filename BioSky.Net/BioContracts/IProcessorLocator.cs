using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IProcessorLocator
  {
    void Init(IWindsorContainer container);

    T GetProcessor<T>();

    object GetProcessor(Type type);
  }
}

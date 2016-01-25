using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BioContracts
{
  public class ProcessorLocator : IProcessorLocator
  {
    public void Init(IWindsorContainer container)
    {
      _container = container;
    }

    T IProcessorLocator.GetProcessor<T>()
    {
      return _container.Resolve<T>();
    }

    public object GetProcessor(Type type)
    {
      return _container.Resolve(type);
    }

    private IWindsorContainer _container;
  }
}

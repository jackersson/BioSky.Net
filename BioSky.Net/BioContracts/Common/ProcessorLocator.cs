using Castle.Windsor;
using System;

namespace BioContracts
{
  public class ProcessorLocator : IProcessorLocator
  {
    public void Init(IWindsorContainer container) {
      _container = container;
    }

    T IProcessorLocator.GetProcessor<T>() {
      return _container.Resolve<T>();
    }

    public object GetProcessor(Type type) {
      return _container.Resolve(type);
    }

    private IWindsorContainer _container;
  }
}

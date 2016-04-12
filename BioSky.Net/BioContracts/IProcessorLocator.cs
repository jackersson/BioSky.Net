using Castle.Windsor;
using System;

namespace BioContracts
{
  public interface IProcessorLocator
  {
    void Init(IWindsorContainer container);

    T GetProcessor<T>();

    object GetProcessor(Type type);
  }
}

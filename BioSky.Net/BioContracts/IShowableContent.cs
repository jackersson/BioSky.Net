using System;

namespace BioContracts
{
  public interface IShowableContent
  {
    void ShowContent(Type flyoutPage, object[] args = null);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IProgressShowable
  {
    void ShowProgress(int progress, bool status);

    void ShowWaiting(string message);

    void Hide(int delay);

    IProgressShowable GetLoadingViewModel();
  }
}

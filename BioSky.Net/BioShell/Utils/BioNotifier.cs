using BioContracts;
using BioModule.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System;

namespace BioShell.Utils
{
  public class BioNotifier : INotifier
  {
    public BioNotifier()
    {
      _progressRing = new ProgressRingViewModel();
    }
    public void Notify(_Exception exception)
    {
      if (exception == null)
        return;

      Notify(exception.Message, WarningLevel.Error);
    }

    public void Notify(string message, WarningLevel level)
    {
      MessageBox.Show(message, "Exception");
    }

    public void Notify(int progress, bool status, double pointX, double pointY)
    {
      _progressRing.ShowProgress(progress, status, pointX, pointY);
      
    }

    private ProgressRingViewModel _progressRing;

    public object LoadingViewModel
    {
      get
      {
        return _progressRing;
      }
    }
  }
}

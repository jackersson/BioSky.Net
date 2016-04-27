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
      _bioLogger    = new BioLogger();
    }
    public void Notify(_Exception exception)
    {      
      if (exception == null)
        return;

      _bioLogger.LogException(exception);     

      Notify(exception.Message, WarningLevel.Error);
    }

    public void Notify(string message, WarningLevel level)
    {
      MessageBox.Show(message, "Exception");
    }    

    //**********************8Progress ring***********************************
    public void Notify(int progress, bool status, double pointX, double pointY)
    {
      _progressRing.ShowProgress(progress, status);      
    }

    public void ShowInformation(string message)
    {
      _progressRing.ShowWaiting(message);
    }

    public void Hide()
    {
      _progressRing.Hide();
    }

    public object LoadingViewModel
    {
      get {  return _progressRing;  }
    }

    private ProgressRingViewModel _progressRing;
    private BioLogger             _bioLogger   ;

  }
}

using BioContracts;
using BioModule.ResourcesLoader;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioShell.ViewModels
{
  public class TestViewModel : Screen, IAccessDeviceObserver
  {
    public void OnCardDetected(string cardNumber)
    {
      //throw new NotImplementedException();
    }

    public void UpdateFromSource(BitmapSource img)
    {
      try
      {
        img.Freeze();
        CurrentImageSource = img;
      }
      catch (Exception ex)
      {

      }
    }

    public void UpdateFromSource2(BitmapSource img)
    {
      try
      {
        img.Freeze();
        CurrentImageSource2 = img;
      }
      catch (Exception ex)
      {

      }
    }


    public void OnError(Exception ex)
    {
      System.Action act = delegate { UpdateFromSource(NotOk); UpdateFromSource2(NotOk);  };
      System.Windows.Application.Current.Dispatcher.Invoke(act);
     // UpdateFromSource(NotOk);
    //  UpdateFromSource2(NotOk);
      //throw new NotImplementedException();
    }

    public void OnReady(bool isReady)
    {
      System.Action act = delegate { UpdateFromSource(Ok); UpdateFromSource2(Ok); };
      System.Windows.Application.Current.Dispatcher.Invoke(act);
     // UpdateFromSource(Ok);
    //  UpdateFromSource2(Ok);
      //throw new NotImplementedException();
    }

    private BitmapSource _ok;
    private BitmapSource _notOk;

    public BitmapSource Ok
    {
      get
      {
        if (_ok == null)
          _ok = ResourceLoader.OkIconSource;
        return _ok;
      }
    }


    public BitmapSource NotOk
    {
      get
      {
        if (_notOk == null)
          _notOk = ResourceLoader.WarningIconSource;
        return _notOk;
      }
    }

    private BitmapSource _currentImageSource2;
    public BitmapSource CurrentImageSource2
    {
      get { return _currentImageSource2; }
      protected set
      {
        try
        {
          if (_currentImageSource2 != value)
          {
            _currentImageSource2 = value;
            NotifyOfPropertyChange(() => CurrentImageSource2);
          }
        }
        catch (TaskCanceledException ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }

    private BitmapSource _currentImageSource;
    public BitmapSource CurrentImageSource
    {
      get { return _currentImageSource; }
      protected set
      {
        try
        {
          if (_currentImageSource != value)
          {
            _currentImageSource = value;
            NotifyOfPropertyChange(() => CurrentImageSource);
          }
        }
        catch (TaskCanceledException ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }
  }
}

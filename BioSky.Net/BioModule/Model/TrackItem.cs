using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioModule.ViewModels;

namespace BioModule.Model
{
  public class TrackItem : IObserver<AccessDeviceActivity>
  {
    public TrackItem( IBioEngine bioEngine, string portName )
    {
      _portName = portName;
      _bioEngine = bioEngine;
    }

    public void start()
    {
      _bioEngine.AccessDeviceEngine().Add(_portName);
      _bioEngine.AccessDeviceEngine().Subscribe(this, _portName);
    }

    public void OnNext(AccessDeviceActivity value)
    {
      TrackControlItemViewModel currentViewModel = (TrackControlItemViewModel)ScreenViewModel;
      currentViewModel.AccessDeviceOK = true;
    }

    public void OnError(Exception error)
    {
      Console.WriteLine(error.Message);
      TrackControlItemViewModel currentViewModel = (TrackControlItemViewModel)ScreenViewModel;
      currentViewModel.AccessDeviceOK = false;
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    public string Caption { get; set; }
    public object ScreenViewModel { get; set; }

    private readonly IBioEngine _bioEngine;
    private string _portName;
  }
}

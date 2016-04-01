using BioService;
using System;

namespace BioContracts.Common
{
  /*
  public delegate void StateEventHandler  (bool status   );
  public delegate void CardEventHandler   (TrackLocationAccessDeviceObserver sender, string cardNumber);

  public class TrackLocationAccessDeviceObserver : IObserver<AccessDeviceActivity>
  {
    public event StateEventHandler AccessDeviceState;
    public event CardEventHandler  CardDetected     ;

    public TrackLocationAccessDeviceObserver(IProcessorLocator locator)
    {
      Init(locator);      
    }

    public TrackLocationAccessDeviceObserver(IProcessorLocator locator, AccessDevice accessDevice)
    {
      Init(locator);
      Update(accessDevice.Portname);
    }

    public TrackLocationAccessDeviceObserver(IProcessorLocator locator, string deviceName)
    {
      Init  (locator);
      Update(deviceName);
    }

    public void Update(string deviceName)
    {
      DeviceName = deviceName;
           
      _accessDeviceEngine.Add(DeviceName);
      _accessDeviceEngine.Subscribe(this, DeviceName);

      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, DeviceName);
    }

    private void Init(IProcessorLocator locator)
    {
      _locator      = locator;
     
      _accessDeviceEngine = _locator.GetProcessor<IAccessDeviceEngine>();
      _database           = _locator.GetProcessor<IBioSkyNetRepository>();      
    }

    public void Success()
    {
      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandAccess, DeviceName);
    }

    public void Reset()
    {
      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReset, DeviceName);
    }

    public void Failed()
    {
      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, DeviceName);
    }

    private void OnAccessDeviceStateChanged( bool status = true)
    {
      if (AccessDeviceState != null)
        AccessDeviceState(status);
    }

    private void OnCardDetected(string cardNumber)
    {
      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReset, DeviceName);

      if (CardDetected != null)
        CardDetected(this, cardNumber);
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
      OnAccessDeviceStateChanged(false);
    }

    public void OnNext(AccessDeviceActivity value)
    {
      OnAccessDeviceStateChanged();

      if (value.Data == null)
        return;
      
      string cardNumber = "";
        
      for (int i = 0; i < value.Data.Length; ++i)
        cardNumber += value.Data[i];

      OnCardDetected(cardNumber);     
    }

    public void Stop()
    {
      _accessDeviceEngine.Unsubscribe(this, DeviceName);      
      _accessDeviceEngine.Remove(DeviceName);    
    }

   
    private string _deviceName;
    public string DeviceName
    {
      get { return _deviceName; }
      set
      {
        if (_deviceName != value)
          _deviceName = value;
      }
    }

    private IProcessorLocator    _locator;
    private IAccessDeviceEngine  _accessDeviceEngine;     
    private IBioSkyNetRepository _database;
  }
  */
}

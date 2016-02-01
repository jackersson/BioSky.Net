using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public delegate void StateEventHandler  (bool status   );
  public delegate void CardEventHandler(string visitor);

  public class TrackLocationAccessDeviceObserver : IObserver<AccessDeviceActivity>
  {

    public event StateEventHandler AccessDeviceState;
    public event CardEventHandler  CardDetected;

    public TrackLocationAccessDeviceObserver(IProcessorLocator locator, AccessDevice accessDevice)
    {
      _locator      = locator;
      _accessDevice = accessDevice;

      _accessDeviceEngine = _locator.GetProcessor<IAccessDeviceEngine>();
      _database           = _locator.GetProcessor<IBioSkyNetRepository>();

      _accessDeviceEngine.Add(accessDevice.Portname);
      _accessDeviceEngine.Subscribe(this, accessDevice.Portname);
      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, _accessDevice.Portname);
    }

    private void OnAccessDeviceStateChanged( bool status = true)
    {
      if (AccessDeviceState != null)
        AccessDeviceState(status);
    }

    private void OnCardDetectedd(string cardNumber)
    {
      if (CardDetected != null)
        CardDetected(cardNumber);
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

      CardDetected(cardNumber);

      /*
      Card card = _database.GetCardByNumber(cardNumber);
      if (card != null)
      {
        Person person = _database.GetPersonByID(card.Personid);
        if (person != null)
        {
          _accessDeviceEngine.Execute(AccessDeviceCommands.CommandAccess, _accessDevice.Portname);
         
        }        

      }
      */
      //_accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, _accessDevice.Portname);      
    }

    public void Stop()
    {
      _accessDeviceEngine.Unsubscribe(this, _accessDevice.Portname);
      _accessDeviceEngine.Remove(_accessDevice.Portname);    
    }


    private readonly AccessDevice _accessDevice;
    private readonly IProcessorLocator _locator;

    private readonly IAccessDeviceEngine  _accessDeviceEngine;     
    private readonly IBioSkyNetRepository _database;
  }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;

using BioAccessDevice.Abstract;
using BioAccessDevice.Interfaces;
using BioAccessDevice.Commands;
using System.Threading;
using System.IO.Ports;
using BioContracts.Abstract;
using System.Diagnostics;

namespace BioAccessDevice
{
  public class AccessDeviceListener : Threadable, IObservable<AccessDeviceActivity>
  {  
    public AccessDeviceListener( string portName ) : base()
    {
      _serialPort          = new SerialPort();
      _serialPort.PortName = portName;
      _serialPort.BaudRate = ACCESS_DEVICE_BAUD_RATE;

      _serialPort.ReadTimeout  = 1000;
      _serialPort.WriteTimeout = 1000;


      _commandFactory = new AccessDeviceCommandFactory(); 
      _commands       = new ConcurrentQueue<ICommand> ();

      _observers      = new List<IObserver<AccessDeviceActivity>>();     
    }   
       
    public void Enqueque( ICommand command )
    {
      if (command == null)
        return;
      lock (_commands)      
        _commands.Enqueue(command);      
    }

    public void Enqueque(AccessDeviceCommands commandName)
    {
     // string type = "BioAccessDevice.Commands." + commandName;      
      Enqueque((AccessDeviceCommand)_commandFactory.GetCommand(commandName));      
    }

    public void Clear()
    {
      lock (_commands)
      {
        while (!_commands.IsEmpty)
        {
          ICommand command;
          Dequeue(out command);
        }
      }
    }

    public bool IsActive()
    {
      return _serialPort.IsOpen && Active;
    }

    public bool Open()
    {

      if (_serialPort.IsOpen)
        return true;

      try
      {
        _serialPort.Open();

        Enqueque(AccessDeviceCommands.CommandReset);
        Enqueque(AccessDeviceCommands.CommandReady);

        AccessDeviceActivity notification = new AccessDeviceActivity();
        Notify(new AccessDeviceActivity() { CommandID = AccessDeviceCommands.CommandReady });



        return true;
      }
      catch (Exception exeption)
      {
        Notify(exeption);
        return false;      
      }

    }

    public override void Stop()
    {
      Clear();
      Enqueque(AccessDeviceCommands.CommandReset);    
      base.Stop();
    }

    public override void Run()
    {
      Open();
      Active = true; 

      while (Active)
      {
        ICommand command;       
        if (Dequeue(out command))
        {
         
          Thread.Sleep(200);

         
          if ( command.Execute(ref _serialPort) )
          {            
            AccessDeviceCommands commandID;
            Enum.TryParse(command.GetType().Name, out commandID);

            AccessDeviceActivity notification = new AccessDeviceActivity();          
            Notify(new AccessDeviceActivity() { CommandID = commandID, Data = command.Message() });          
          }
          else
          {
            if ( !IsActive() )
            {
              Notify(command.ErrorMessage());
              Thread.Sleep(1000);
              Open();
            }            
          }
        }  
        
        if ( CancellationTokenResult.IsCancellationRequested)
          break;                    
      }

      _serialPort.Close();
      Clear();


    }

    private bool Dequeue( out ICommand command )
    {
      lock (_commands)
      {
        while (_commands.IsEmpty)
        {
          Thread.Sleep(100);
          Enqueque(AccessDeviceCommands.CommandDallasKey);          
        }     
        
        return _commands.TryDequeue(out command);
      }
    }

    private void Notify(AccessDeviceActivity notification)
    {
      if (notification == null)
        return;

      foreach (var observer in _observers)
      {
        if (notification != null)        
          observer.OnNext(notification);       
      }
    }

    private void Notify( Exception exception )
    {
      if (exception == null)
        return;

      foreach (var observer in _observers)
      {
        if (exception != null)
          observer.OnError(exception);     
      }
    }


    //Method required by IObservable interface
    public IDisposable Subscribe(IObserver<AccessDeviceActivity> observer)
    {
      if (!HasObserver(observer))
        _observers.Add(observer);

      return new Unsubscriber(_observers, observer);
    }

    public IDisposable Unsubscribe(IObserver<AccessDeviceActivity> observer)
    {
      if (HasObserver(observer))
        _observers.Remove(observer);

      return new Unsubscriber(_observers, observer);
    }

    public bool HasObserver(IObserver<AccessDeviceActivity> observer)
    {
      return _observers.Contains(observer);
    }

    private ICommandFactory           _commandFactory;
    private SerialPort                _serialPort    ;   
    private ConcurrentQueue<ICommand> _commands      ;  
      

    private const int ACCESS_DEVICE_BAUD_RATE = 4800;

    //************************** For Observable Functions ***********************************
    private List<IObserver<AccessDeviceActivity>> _observers;
    private class Unsubscriber : IDisposable
    {
      private List<IObserver<AccessDeviceActivity>> _observers;
      private IObserver<AccessDeviceActivity> _observer;

      public Unsubscriber(List<IObserver<AccessDeviceActivity>> observers, IObserver<AccessDeviceActivity> observer)
      {
        this._observers = observers;
        this._observer = observer;
      }

      public void Dispose()
      {        
        if (_observer != null && _observers.Contains(_observer))
          _observers.Remove(_observer);
      }
    }

  }
}

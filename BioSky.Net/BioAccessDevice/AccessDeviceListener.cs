using System;
using System.Collections.Concurrent;

using BioContracts;

using BioAccessDevice.Abstract;
using BioAccessDevice.Interfaces;
using System.Threading;
using System.IO.Ports;
using BioContracts.Abstract;
using BioContracts.Common;
using System.Collections.Generic;
using BioContracts.Locations;

namespace BioAccessDevice
{  
  public class AccessDeviceListener : Threadable, IBioObservable<IAccessDeviceObserver>
  {  
    public AccessDeviceListener( string portName ) : base()
    {
      _serialPort          = new SerialPort();
      _serialPort.PortName = portName;
      _serialPort.BaudRate = ACCESS_DEVICE_BAUD_RATE;

      _serialPort.ReadTimeout  = READ_WRITE_TIMEOUT;
      _serialPort.WriteTimeout = READ_WRITE_TIMEOUT;

      _commandFactory = new AccessDeviceCommandFactory(); 
      _commands       = new ConcurrentQueue<ICommand> ();

      _observer       = new BioObserver<IAccessDeviceObserver>();     
    }

    #region observer
    public void Subscribe(IAccessDeviceObserver observer) {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IAccessDeviceObserver observer) {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll()  {
      _observer.UnsubscribeAll();
    }

    public bool HasObserver(IAccessDeviceObserver observer) {
      return _observer.HasObserver(observer);
    }
    #endregion
    public override void Stop()
    {
      Clear();
      Execute(AccessDeviceCommands.CommandReset);
      base.Stop();
    }

    public void Execute(AccessDeviceCommands commandName)
    {
      Execute((AccessDeviceCommand)_commandFactory.GetCommand(commandName));
    }

    public bool IsActive()
    {
      return _serialPort.IsOpen && Active;
    }

    private void Execute( ICommand command )
    {
      if (command == null)
        return;
      lock (_commands)      
        _commands.Enqueue(command);      
    }    

    private void Clear()
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
      
    private bool Open()
    {
      if (_serialPort.IsOpen)
        return true;

      try
      {
        _serialPort.Open();

        Execute(AccessDeviceCommands.CommandReset    );
        Execute(AccessDeviceCommands.CommandReady    );

        OnReady(true);        
        return true;
      }
      catch (Exception exeption)
      {
        OnError(exeption);
        return false;      
      }

    }    

    protected override void Run()
    {
      Open();
      Active = true; 

      while (Active)
      {
        ICommand command;       
        if (Dequeue(out command))
        {         
          Thread.Sleep(DELAY_BETWEEN_COMMANDS);
         
          if ( command.Execute(ref _serialPort) )
          {            
            AccessDeviceCommands commandID;
            Enum.TryParse(command.GetType().Name, out commandID);

            if (commandID == AccessDeviceCommands.CommandDallasKey)
              OnCardDetected(command.Message());         
          }
          else
          {
            Exception errorMesage = command.ErrorMessage();
            if ( !IsActive() )
            {
              OnError(errorMesage);
              Thread.Sleep(DELAY_BETWEEN_CONNECTION);
              Open();
            }     
            else if (errorMesage != null)            
              OnError(errorMesage);            
            else            
              OnReady(true);       

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
          Execute(AccessDeviceCommands.CommandDallasKey);          
        }     
        
        return _commands.TryDequeue(out command);
      }
    }
         
    private void OnCardDetected(byte[] data)
    {
      if (data == null)
        return;

      string cardNumber = "";

      for (int i = 0; i < data.Length; ++i)
        cardNumber += data[i];

      foreach (KeyValuePair<int, IAccessDeviceObserver> observer in _observer.Observers)
        observer.Value.OnCardDetected(cardNumber);
    }

    private void OnError(Exception exception)
    {
      if (exception == null)
        return;

      foreach (KeyValuePair<int, IAccessDeviceObserver> observer in _observer.Observers)
        observer.Value.OnError(exception, LocationDevice.AccessDevice);
    }

    private void OnReady(bool isOk)
    {
      foreach (KeyValuePair<int, IAccessDeviceObserver> observer in _observer.Observers)
        observer.Value.OnReady(isOk);
    }

    private ICommandFactory _commandFactory;
    private SerialPort      _serialPort    ;

    private ConcurrentQueue<ICommand> _commands;

    private BioObserver<IAccessDeviceObserver> _observer;

    private const int ACCESS_DEVICE_BAUD_RATE  = 4800;
    private const int DELAY_BETWEEN_COMMANDS   = 200 ;
    private const int DELAY_BETWEEN_CONNECTION = 1000;
    private const int DELAY_BETWEEN_ASK_DEVICE = 100 ;
    private const int READ_WRITE_TIMEOUT       = 1000;

  }
}

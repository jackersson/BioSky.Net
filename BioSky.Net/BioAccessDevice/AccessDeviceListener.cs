using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BioAccessDevice.Interfaces;
using System.Threading;

namespace BioAccessDevice
{
  public class AccessDeviceListener
  {

    public AccessDeviceListener()
    {

    }


    public void Enqueque( ICommand command )
    {
      lock (_commands)      
        _commands.Enqueue(command);      
    }

    public void Start ()
    {
      _active = true;
    }

    public void Stop()
    {
      _active = false;
    }

    public void Run()
    {
      while (_active)
      {
        ICommand command;

        if (Dequeue(out command))
        {
          Thread.Sleep(50);
          command.Execute();
        }
      }
    }

    private bool Dequeue( out ICommand command )
    {
      lock (_commands)
      {
        while (_commands.IsEmpty)        
          Monitor.Wait(_commands);
        
        return _commands.TryDequeue(out command);
      }
    }

    private ConcurrentQueue<ICommand> _commands;
    private bool _active = false;
  }
}

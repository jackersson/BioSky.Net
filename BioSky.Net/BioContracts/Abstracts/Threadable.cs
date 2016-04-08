﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BioContracts.Abstract
{
  public abstract class Threadable
  {
    public Threadable()
    {
      _active = false;
      _cancelationToken = new CancellationTokenSource();
    }

    public bool Active
    {
      get { return _active; }
      set
      {
        if (_active != value)
          _active = value;
      }
    }



    public void Start()
    {
      //Task task = new Task(ThreadProcedure, this);
      //task.Start();
      //ParameterizedThreadStart st = new ParameterizedThreadStart(ThreadProcedure);
     // st.

      Thread th = new Thread((() => ThreadProcedure(this)));
      th.Start();
      //ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProcedure), this);
    }

    public virtual void Stop()
    {
      _cancelationToken.Cancel();

    }

    protected abstract void Run();

    protected CancellationTokenSource CancellationTokenResult
    {
      get { return _cancelationToken; }
    }

    protected void ThreadProcedure(object threadContext)
    {
      Threadable threadable = (Threadable)threadContext;

      if (threadable != null)
      {
        threadable.Active = true;
        threadable.Run();
        threadable.Active = false;
      }
    }

    private bool _active;
    private CancellationTokenSource _cancelationToken;
  }
}

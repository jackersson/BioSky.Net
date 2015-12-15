﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioAccessDevice.Interfaces
{
  public interface ICommand
  { 
    bool   Execute( ref SerialPort serialPort );
    byte[] Message();

    Exception ErrorMessage();
  }
}

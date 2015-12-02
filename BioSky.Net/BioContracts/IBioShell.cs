﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BioContracts
{
  public interface IBioShell
  {
    ShellFlyoutControl FlyoutControl { get; }
    ShellTabControl TabControl { get; }
    IBioModule LoadModule(Assembly assembly);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

using Microsoft.Win32;
using System.IO;
using System.Drawing;
using MahApps.Metro.Controls;
using BioData;
using BioService;
using System.Windows.Threading;
using BioModule.Utils;
using BioContracts;
using System.Windows;


namespace BioModule.ViewModels
{
  public class PhotoImageViewModel : ImageViewModel
  {
    PhotoImageViewModel(IProcessorLocator locator, IWindowManager windowManager)
      : base(locator, windowManager)
    {

    }

  }
}

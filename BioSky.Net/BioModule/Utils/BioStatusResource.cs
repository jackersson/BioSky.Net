﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.ResourcesLoader;
using System.Drawing;
using System.Windows.Media.Imaging;

using BioContracts;
using BioData;



namespace BioModule.Utils
{
  public class BioStatusResource
  {
    public BioStatusResource()
    {
      _resource = new Dictionary<string, BitmapSource>();


      _resource.Add(BioService.ResultStatus.Success.ToString(), ResourceLoader.VerificationIconSource);
      _resource.Add(BioService.ResultStatus.Failed.ToString(), ResourceLoader.VerificationFailedIconSource); 
      //_resource.Add( NotificationStatus.Success.ToString()       , ResourceLoader.OkIconSource);
      //_resource.Add( NotificationStatus.Failure.ToString()       , ResourceLoader.CancelIconSource);
      //_resource.Add( NotificationStatus.Warning.ToString()       , ResourceLoader.ErrorIconSource);
    }

    public BitmapSource GetBitmapSource(string status)
    {
      BitmapSource source;
      if (_resource.TryGetValue(status, out source))
        return source;

      return ResourceLoader.ErrorIconSource;
    }

    private Dictionary<string, BitmapSource> _resource;

  }
}

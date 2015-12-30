using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.ResourcesLoader;
using System.Drawing;
using System.Windows.Media.Imaging;

using BioContracts;
using BioData;
using BioModule.Model;

namespace BioModule.Utils
{
  public class BioStatusResource
  {
    public BioStatusResource()
    {
      _resource = new Dictionary<string, BitmapSource>();

      _resource.Add( VisitorStatus.VerificationSuccess.ToString(), ResourceLoader.VerificationIconSource);
      _resource.Add( VisitorStatus.VerificationFailed.ToString() , ResourceLoader.VerificationFailedIconSource);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using BioData;
using BioContracts;

using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using AForge.Video.DirectShow;

namespace BioModule.ViewModels
{
  public class UserPhotoViewModel : Screen
  {
    public UserPhotoViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;

      DisplayName = "Photo";

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;

      CaptureDeviceConnected = false;
    }

    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    private AsyncObservableCollection<FilterInfo> _captureDevicesNames;
    public AsyncObservableCollection<FilterInfo> CaptureDevicesNames
    {
      get { return _captureDevicesNames; }
      set
      {
        if (_captureDevicesNames != value)
        {
          _captureDevicesNames = value;

          NotifyOfPropertyChange(() => CaptureDevicesNames  );          
        }
      }
    }

    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _captureDevicesNames.Count); }
    }

    private string _selectedAccessDevice;
    public string SelectedCaptureDevice
    {
      get { return _selectedAccessDevice; }
      set
      {
        if (_selectedAccessDevice != value)
        {
          _selectedAccessDevice = value;
          NotifyOfPropertyChange(() => SelectedCaptureDevice);          
        }
      }
    }

    private bool _captureDeviceConnected;
    private bool CaptureDeviceConnected
    {
      get { return _captureDeviceConnected; }
      set
      {
        if (_captureDeviceConnected != value)
        {
          _captureDeviceConnected = value;
          NotifyOfPropertyChange(() => CaptureDeviceConnectedIcon);
        }
      }
    }

    public BitmapSource CaptureDeviceConnectedIcon
    {
      get { return CaptureDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }  

    private readonly IBioEngine _bioEngine;
  }
}

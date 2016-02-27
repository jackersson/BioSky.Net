using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using BioContracts;
using System.Drawing;
using System.Reflection;

namespace BioModule.ViewModels
{
  public class CameraDialogResult
  {
    public string selectedCaptureDevice ;
    public bool   captureDeviceConnected;
  }
  public class CameraDialogViewModel : Screen
  {
    public CameraDialogViewModel(IProcessorLocator locator , IWindowManager windowManager, string title = "CameraDialog")
    {
      _locator             = locator      ;
      _windowManager       = windowManager;


      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
   
      CaptureDevicesNames = _captureDeviceEngine.GetCaptureDevicesNames();

      CaptureDeviceConnected = false;

      Update(title);
    }
    #region Update
    public void Update(string title = "CameraDialog")
    {
      DisplayName = title;
    }
    public void Show()
    {
      _windowManager.ShowDialog(this);
    }
    #endregion

    #region Interface
    public void Apply()
    {
      DialogResult = new CameraDialogResult()
      { 
        captureDeviceConnected = CaptureDeviceConnected
        , selectedCaptureDevice = SelectedCaptureDevice
      };

      this.TryClose(true); 
      TryClose(true);
    }

    public void Cancel()
    {     
      TryClose(false);
    }

    public CameraDialogResult GetDialogResult()
    {
      return DialogResult;
    }
    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);    
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }
    protected override void OnActivate()
    {
      CaptureDeviceConnected = false;
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
     
      base.OnActivate();
    }
    protected override void OnDeactivate(bool close)
    {
      CaptureDeviceConnected = false;
      SelectedCaptureDevice  = null ;     

      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }
       
    public void Subscribe()
    {
      if (SelectedCaptureDevice == null || _captureDeviceEngine == null)
        return;
          
      _captureDeviceEngine.Add(SelectedCaptureDevice);

      CaptureDeviceConnected = _captureDeviceEngine.CaptureDeviceActive(SelectedCaptureDevice);
    }
    #endregion
    
    #region UI
   
    public BitmapSource CaptureDeviceConnectedIcon
    {
      get { return CaptureDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }
    public string AvaliableDevicesCount
    {
      get { return string.Format("Available Devices ({0})", _captureDevicesNames.Count); }
    }

    private AsyncObservableCollection<string> _captureDevicesNames;
    public AsyncObservableCollection<string> CaptureDevicesNames
    {
      get { return _captureDevicesNames; }
      set
      {
        if (_captureDevicesNames != value)
        {
          _captureDevicesNames = value;

          NotifyOfPropertyChange(() => CaptureDevicesNames);
        }
      }
    }

    private bool _captureDeviceConnected;
    public bool CaptureDeviceConnected
    {
      get { return _captureDeviceConnected; }
      set
      {
        if (_captureDeviceConnected != value)
        {
          _captureDeviceConnected = value;
          NotifyOfPropertyChange(() => CaptureDeviceConnected);
          NotifyOfPropertyChange(() => CaptureDeviceConnectedIcon);
        }
      }
    }

    private string _selectedCaptureDevice;
    public string SelectedCaptureDevice
    {
      get { return _selectedCaptureDevice; }
      set
      {
        if (_selectedCaptureDevice != value)
        {
          _selectedCaptureDevice = value;
          NotifyOfPropertyChange(() => SelectedCaptureDevice);

          Subscribe();
        }
      }
    }

    private CameraDialogResult _dialogResult;
    public CameraDialogResult DialogResult
    {
      get { return _dialogResult; }
      set
      {
        if (_dialogResult != value)
        {
          _dialogResult = value;
        }
      }
    }

   
    #endregion

    #region Global Variables
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IProcessorLocator    _locator            ;
    private          IWindowManager       _windowManager      ;


    #endregion

  }
}

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

namespace BioModule.ViewModels
{
  public class CameraDialogViewModel : Screen
  {
    public CameraDialogViewModel(IBioEngine bioEngine, IProcessorLocator locator , string title = "CameraDialog")
    {
      _locator   = locator;
      _bioEngine = bioEngine;

      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();


      CaptureDeviceConnected = false;
      Update(title);
    }
    #region Update
    public void Update(string title = "CameraDialog")
    {
      DisplayName = title;
    }
    #endregion

    #region Interface
    public void Apply()
    {
      DialogResult = true;
      this.TryClose(DialogResult);
    }

    public void Cancel()
    {
      DialogResult = false;
      this.TryClose(DialogResult);
    }

    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);
      if (ActiveCaptureDevice == null)
        NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }
    protected override void OnActivate()
    {
      CaptureDeviceConnected = false;
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
      //RefreshData();
      base.OnActivate();
    }
    protected override void OnDeactivate(bool close)
    {
      CaptureDeviceConnected = false;

      if (ActiveCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, ActiveCaptureDevice);

        SelectedCaptureDevice = null;
      //_imageViewer.Clear();

      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      CaptureDeviceConnected = true;

      //_imageViewer.UpdateImage(ref bitmap);
    }
    public void Subscribe()
    {
      CaptureDeviceConnected = false;
      if (SelectedCaptureDevice == null)
        return;

      if (ActiveCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, ActiveCaptureDevice);

      ActiveCaptureDevice = SelectedCaptureDevice;

      if (!_captureDeviceEngine.CaptureDeviceActive(ActiveCaptureDevice))
        _captureDeviceEngine.Add(ActiveCaptureDevice);

      _captureDeviceEngine.Subscribe(OnNewFrame, ActiveCaptureDevice);
    }
    #endregion




    #region UI

    private bool _dialogResult;
    public bool DialogResult
    {
      get { return _dialogResult; }
      set
      {
        if (_dialogResult != value)
        {
          _dialogResult = value;
          NotifyOfPropertyChange(() => DialogResult);
        }
      }
    }
    public BitmapSource CaptureDeviceConnectedIcon
    {
      get { return CaptureDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }
    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _captureDevicesNames.Count); }
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

    private string _activeCaptureDevice;
    public string ActiveCaptureDevice
    {
      get { return _activeCaptureDevice; }
      set
      {
        if (_activeCaptureDevice != value)
        {
          _activeCaptureDevice = value;

          NotifyOfPropertyChange(() => ActiveCaptureDevice);
        }
      }
    }
    #endregion

    #region Global Variables
    private readonly IBioEngine           _bioEngine          ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IProcessorLocator    _locator            ;

    #endregion

  }
}

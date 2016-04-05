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
  public class CameraDialogViewModel : Screen
  {
    public CameraDialogViewModel(IProcessorLocator locator, string title = "CameraDialog")
    {
      _locator             = locator      ;
      _windowManager       = locator.GetProcessor<IWindowManager>();

      ICaptureDeviceEngine captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
   
      CaptureDevicesNames = captureDeviceEngine.GetDevicesNames();
      
      Update(title);
    }
    #region Update
    public void Update(string title = "CameraDialog")
    {
      DisplayName = title;
    }
    public bool? Show()
    {
      return _windowManager.ShowDialog(this);
    }
    #endregion

    #region Interface
    public void Apply()
    {
      this.TryClose(true); 
      TryClose(true);
    }

    public void Cancel()
    {     
      TryClose(false);
    }

    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);    
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }
    protected override void OnActivate()
    {
      SelectedCaptureDevice = null;
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
     
      base.OnActivate();
    }
    protected override void OnDeactivate(bool close)
    {     
      SelectedCaptureDevice  = null ;     
      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }
  
    #endregion
    
    #region UI   
   
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
        }
      }
    }       
    #endregion

    #region Global Variables
    //private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IProcessorLocator    _locator            ;
    private          IWindowManager       _windowManager      ;


    #endregion

  }
}

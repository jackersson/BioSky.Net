using AForge.Video.DirectShow;
using BioContracts;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{

  public class CaptureDevicePropertiesViewModel : Screen, IPropertiesShowable
  {
    public CaptureDevicePropertiesViewModel(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      Resolution = new AsyncObservableCollection<string>();
    }

    public void Show(AForge.Video.DirectShow.VideoCaptureDevice videoDevice)
    {
      Update(videoDevice);
      _windowManager.ShowDialog(this);
    }

    public void Apply()
    {
      TryClose(true);
    }

    public void Close()
    {
      TryClose(false);
    }

    private void Update(AForge.Video.DirectShow.VideoCaptureDevice videoDevice)
    {      
      _videoDevice = videoDevice;

      if (videoDevice == null)
        return;

      Resolution.Clear();
      int i = 0;
      foreach (VideoCapabilities vc in _videoDevice.VideoCapabilities)
      {
        string item = string.Format("{0}x{1}, {2} fps"
                                    , vc.FrameSize.Width
                                    , vc.FrameSize.Height
                                    , vc.AverageFrameRate);
        Resolution.Add(item);

        if (vc == _videoDevice.VideoResolution)
          SelectedResolution = i;

        i++;
      }
      
      NotifyOfPropertyChange(() => Resolution);
    }

    private int _selectedResolution;
    public int SelectedResolution
    {
      get { return _selectedResolution; }
      set
      {
        if (_selectedResolution != value)
        {
          _selectedResolution = value;
          NotifyOfPropertyChange(() => SelectedResolution);

          ApplyVideoDeviceCapability();
        }
      }
    }

    private void ApplyVideoDeviceCapability()
    {
      if (_videoDevice.VideoCapabilities.Length <= SelectedResolution)
        return;

      VideoCapabilities selected = _videoDevice.VideoCapabilities[SelectedResolution];

      if (selected != _videoDevice.VideoResolution)
      {
        _videoDevice.VideoResolution = selected;       
      }  
       
    }

    private AsyncObservableCollection<string> _resolution;
    public AsyncObservableCollection<string> Resolution
    {
      get { return _resolution; }
      set
      {
        if (_resolution != value)
        {
          _resolution = value;
          NotifyOfPropertyChange(() => Resolution);
        }
      }
    }


    private VideoCapabilities[] _defaultVideoCapabilities = new VideoCapabilities[0];
    AForge.Video.DirectShow.VideoCaptureDevice _videoDevice;
    private readonly IWindowManager _windowManager;
  }
}

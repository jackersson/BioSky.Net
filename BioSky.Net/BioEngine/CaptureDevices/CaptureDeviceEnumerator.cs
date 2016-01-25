using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BioEngine.CaptureDevices
{
  
  public class CaptureDeviceEnumerator : Threadable
  {

    public CaptureDeviceEnumerator() : base()
    {
      _captureDevicesNames = new AsyncObservableCollection<FilterInfo>();
    }

    public override void Run()
    {
      Active = true;

      while (Active)
      {       
        ActualCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);      
        ActualCaptureDevices.Clear();
        Thread.Sleep(2000);
      }
    }

   
    private FilterInfoCollection _actualCaptureDevices;
    public FilterInfoCollection ActualCaptureDevices
    {
      get { return _actualCaptureDevices; }
      set
      {
        if (_actualCaptureDevices != value)
        {
        _actualCaptureDevices = value;
          Update();
        }
      }
    }
    
    private void Update()
    {
      if (_actualCaptureDevices.Count > 0)
      {
        foreach (FilterInfo deviceName in _captureDevicesNames.ToArray())
        {
          bool exists = false;
          foreach (FilterInfo dN in _actualCaptureDevices)
          {
            if (deviceName == dN)
            {
              exists = true;
              break;
            }             
          }
          if (!exists)
            _captureDevicesNames.Remove(deviceName);
        }
      }
      foreach (FilterInfo deviceName in _actualCaptureDevices)
      {
        if (!_captureDevicesNames.Contains(deviceName))
          _captureDevicesNames.Add(deviceName);
      }
    }


    public bool CaptureDeviceConnected(FilterInfo deviceName)
    {
      return _captureDevicesNames.Contains(deviceName);
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

        }
      }
    }

  }
}

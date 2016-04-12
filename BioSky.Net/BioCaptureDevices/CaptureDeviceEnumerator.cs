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

namespace BioCaptureDevices
{
  
  public class CaptureDeviceEnumerator : Threadable, ICaptureDeviceConnectivity
  {
     public CaptureDeviceEnumerator() : base()
    {
      _captureDevicesNames = new AsyncObservableCollection<string>();
    }

    protected override void Run()
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
        foreach (string deviceName in _captureDevicesNames.ToArray())
        {
          bool exists = false;
          foreach (FilterInfo dN in _actualCaptureDevices)
          {
            if (deviceName == dN.Name)
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
        if (!_captureDevicesNames.Contains(deviceName.Name))
          _captureDevicesNames.Add(deviceName.Name);
      }
    }


    public FilterInfo CaptureDeviceConnected(string deviceName)
    {
      try
      {
        ActualCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        foreach (FilterInfo fi in ActualCaptureDevices)
        {
          if (fi.Name == deviceName)
            return fi;
        }
      }
      catch (Exception e)  {
        Console.WriteLine(e);
      }    
      return null;
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

        }
      }
    }

  }
}

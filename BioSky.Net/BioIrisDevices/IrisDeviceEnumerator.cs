using BioContracts;
using BioContracts.Abstract;
using BioIrisDevices.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BioIrisDevices
{ 
  public class IrisDeviceEnumerator : Threadable, IDeviceConnectivity<string>
  {   
    public IrisDeviceEnumerator()
    {
      ActualDevicesNames = new List<string>();
      DevicesNames       = new AsyncObservableCollection<string>();
      _deviceConnector   = new IrisDeviceConnector();      
    }

    private void Refresh()
    {
      _actualDevicesNames = _deviceConnector.GetDevices();     

       Update();
    }

    private void Update()
    {
      if (_actualDevicesNames.Count > 0)
      {
        foreach (string deviceName in _devicesNames)
        {
          if (!_actualDevicesNames.Contains(deviceName))
            _actualDevicesNames.Remove(deviceName);
        }
      }
      else
      {
        _devicesNames.Clear();
        return;
      }

      foreach (string deviceName in _actualDevicesNames)
      {
        if (!_devicesNames.Contains(deviceName))
          _devicesNames.Add(deviceName);
      }
    }
      
    protected override void Run()
    {
      Active = true;

      while (Active)
      {
        Refresh();
        Thread.Sleep(DELAY_BETWEEN_CONNECTION);
      }
    }

    public string GetDeviceInfo(string deviceName)
    {
      return IsDeviceConnected(deviceName) ? deviceName : string.Empty;
    }

    public bool IsDeviceConnected(string deviceName)
    {
      return DevicesNames.Contains(deviceName);
    }



    private List<string> _actualDevicesNames;
    private List<string> ActualDevicesNames
    {
      get { return _actualDevicesNames; }
      set
      {
        if (_actualDevicesNames != value)
          _actualDevicesNames = value;
      }
    }

    private AsyncObservableCollection<string> _devicesNames;
    public AsyncObservableCollection<string> DevicesNames
    {
      get { return _devicesNames; }
      set
      {
        if (_devicesNames != value)
          _devicesNames = value;
      }
    }

    private IrisDeviceConnector _deviceConnector;

    private const int DELAY_BETWEEN_CONNECTION = 5000;
  }
}


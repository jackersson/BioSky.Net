using BioContracts;
using BioContracts.Abstract;
using BioContracts.FingerprintDevices;
using BioFingerprintDevices.Common;
using ScanAPIHelper;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BioFingerprintDevices
{
  public class FingerprintDeviceEnumerator : Threadable, IDeviceConnectivity<FingerprintDeviceInfo>
  {
    public FingerprintDeviceEnumerator()
    {
      ActualDevicesNames = new List<FingerprintDeviceInfo>();
      DevicesNames       = new AsyncObservableCollection<FingerprintDeviceInfo>();
    }

    private void Refresh()
    {
      try
      {
        FingerprintDeviceInfo.BaseInterface = Device.BaseInterface;
        FTRSCAN_INTERFACE_STATUS[] status   = Device.GetInterfaces();

        ActualDevicesNames.Clear();

        for (int interfaceNumber = 0; interfaceNumber < status.Length; interfaceNumber++)
        {
          if (status[interfaceNumber] == FTRSCAN_INTERFACE_STATUS.FTRSCAN_INTERFACE_STATUS_CONNECTED)
          {
            FingerprintDeviceInfo info = GetScannerInfo(interfaceNumber);
            if (info != null)
              ActualDevicesNames.Add(info);
          }
        }
      }
      catch (ScanAPIException ex)
      {
        Console.WriteLine(FingerprintDeviceErrorInfo.Instance.GetErrorMessage(ex));
      }

      Update();
    }

    private void Update()
    {
      if (_actualDevicesNames.Count > 0)
      {
        foreach (FingerprintDeviceInfo deviceName in _devicesNames)
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

      foreach (FingerprintDeviceInfo deviceName in _actualDevicesNames)
      {
        if (!_devicesNames.Contains(deviceName))
          _devicesNames.Add(deviceName);
      }
    }
    
  

    public FingerprintDeviceInfo GetScannerInfo(int interfaceNumber)
    {
      FingerprintDeviceInfo info = null;
      try
      {
        int temp = Device.BaseInterface;
        Device.BaseInterface = interfaceNumber;

        Device tempDevice = new Device();
        tempDevice.Open();

        DeviceInfo dinfo = tempDevice.Information;
        info = FutronicDevicesInfo.Instance.GetScannerInfoByCompatibility(dinfo.DeviceCompatibility);

        if (info != null)
          info.InterfaceNumber = interfaceNumber;

        tempDevice.Dispose();
        tempDevice = null;

        Device.BaseInterface = temp;

        return info;
      }
      catch (ScanAPIException ex)
      {
        Console.WriteLine(FingerprintDeviceErrorInfo.Instance.GetErrorMessage(ex));
        return info;
      }
    }

    #region threadable
    protected override void Run()
    {
      Active = true;

      while (Active)
      {
        Refresh();
        Thread.Sleep(CONNECTION_DELAY);
      }
    }
    #endregion

    #region deviceConnectivity
    public bool IsDeviceConnected(string name) { return GetDeviceInfo(name) == null ? false : true;  }
    
    public FingerprintDeviceInfo GetDeviceInfo(string deviceName)
    {
      foreach (FingerprintDeviceInfo deviceInfo in DevicesNames)
      {
        if (deviceInfo.Name == deviceName)
          return deviceInfo;
      }
      return null;
    }
    #endregion

    #region UI
    private List<FingerprintDeviceInfo> _actualDevicesNames;
    private List<FingerprintDeviceInfo> ActualDevicesNames
    {
      get { return _actualDevicesNames; }
      set
      {
        if (_actualDevicesNames != value)
          _actualDevicesNames = value;
      }
    }

    private AsyncObservableCollection<FingerprintDeviceInfo> _devicesNames;
    public AsyncObservableCollection<FingerprintDeviceInfo> DevicesNames
    {
      get { return _devicesNames; }
      set
      {
        if (_devicesNames != value)
          _devicesNames = value;
      }
    }
    #endregion

    public const int CONNECTION_DELAY = 5000;
  }
}

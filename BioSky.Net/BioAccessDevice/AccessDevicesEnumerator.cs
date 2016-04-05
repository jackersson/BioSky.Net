using BioContracts;
using BioContracts.Abstract;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace BioAccessDevice
{
  public class AccessDevicesEnumerator : Threadable
  {

    public AccessDevicesEnumerator() : base()
    {
      _accessDevicesNames = new AsyncObservableCollection<string>();
    }

    protected override void Run()
    {
      Active = true;

      while ( Active )
      {
        string[] serialPortNames = SerialPort.GetPortNames();
        SerialPortNames = serialPortNames;

        Thread.Sleep(1000);
      }      
    }


    private string[] _serialPortNames;
    public string[] SerialPortNames
    {
      get { return _serialPortNames; }
      set
      {
        if ( _serialPortNames != value )
        {
          _serialPortNames = value;
          Update();
        }
      }
    }

    private void Update()
    {
      if (_accessDevicesNames.Count > 0)
      {
        foreach (string portName in _accessDevicesNames.ToArray())
        {
          if (!_serialPortNames.Contains(portName))
            _accessDevicesNames.Remove(portName);
        }
      }
      foreach (string portName in _serialPortNames)
      {
        if (!_accessDevicesNames.Contains(portName))
          _accessDevicesNames.Add(portName);
      }
    }


    public bool AccessDeviceConnected(string portName)
    {
      return _accessDevicesNames.Contains(portName);
    }

    private AsyncObservableCollection<string> _accessDevicesNames;
    public AsyncObservableCollection<string> DevicesNames
    {
      get { return _accessDevicesNames; }
      set
      {
        if (_accessDevicesNames != value)
        {
          _accessDevicesNames = value;

        }
      }
    }

  }
}

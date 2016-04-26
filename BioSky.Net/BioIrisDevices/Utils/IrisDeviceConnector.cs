using Iddk2000DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioIrisDevices.Utils
{
  public class IrisDeviceConnector
  {
    private IddkConfig _config = new IddkConfig();
    private List<string> _deviceDescriptions = new List<string>();

    public IrisDeviceConnector()
    {
      UpdateSDK();
    }


    public List<string> GetDevices()
    {
      IddkResult ret = IddkResult.OK;

      _deviceDescriptions.Clear();
      if (_config.CommStd == IddkCommStd.Usb)
      {
        ret = Iddk2000APIs.ScanDevices(_deviceDescriptions);
        if (ret != IddkResult.OK)
          IrisUtils.Instance.GetErrorMessage(ret);
      }

      return _deviceDescriptions;
    }

    private void UpdateSDK()
    {
      IddkResult ret = IddkResult.OK;
      ret = Iddk2000APIs.GetSdkConfig(_config);
      if (ret != IddkResult.OK)
      {
        IrisUtils.Instance.GetErrorMessage(ret);
        return;
      }

      _config.CommStd = IddkCommStd.Usb;
      _config.ResetOnOpenDevice = true;

      ret = Iddk2000APIs.SetSdkConfig(_config);
      if (ret != IddkResult.OK)
      {
        Console.Out.WriteLine("\nFailed to set new configuration !");
        IrisUtils.Instance.GetErrorMessage(ret);
      }
    }
  }
}

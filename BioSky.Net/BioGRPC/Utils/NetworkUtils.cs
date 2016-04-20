using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BioGRPC.Utils
{
  public class NetworkUtils
  {
    public string GetMACAddress()
    {
      NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
      string sMacAddress = string.Empty;
      foreach (NetworkInterface adapter in nics)
      {
        if (string.IsNullOrEmpty(sMacAddress))        
          sMacAddress = adapter.GetPhysicalAddress().ToString();        
      }
      return sMacAddress;
    }

    public string GetLocalIPAddress()
    {
      var host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (var ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork)        
          return ip.ToString();        
      }
      return "";
    }
  }
}

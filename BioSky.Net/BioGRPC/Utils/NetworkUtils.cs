using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        if (sMacAddress == String.Empty)// only return MAC Address from first card  
        {
          //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
          sMacAddress = adapter.GetPhysicalAddress().ToString();
        }
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

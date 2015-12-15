using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioAccessDevice
{
  public class SerialPortUtils
  {

    public bool Compare(byte[] command, byte[] response )
    {
      if (command.Length != response.Length)
        return false;
      for (int i = 0; i < command.Length; i++)
      {
        if (command[i] != response[i])
          return false;
      }

      return true;
    }

    public bool CheckResponseSum(byte[] response)
    {
      if (response.Length < 2)
        return false;

      uint sumBytesOfResponse = 0;
      for (int i = 0; i < response.Length - 2; i++)
        sumBytesOfResponse += response[i];  // calculate sum of all received bytes (exept last two - check sum from com port)

      uint mask = CreateBitMask(0, MAX_CHECK_SUM_BITS_COUNT - 1); // checksum can be only 8 bits
      uint commandCheckSum = mask & sumBytesOfResponse;

      uint portCheckSum = CombineBits(response[response.Length - 2], response[response.Length - 1]);

      return (commandCheckSum == portCheckSum);
    }

    public uint CombineBits(uint a, uint b)
    {
      uint result = a;
      result = (result << 4) | b; // 0000 0000 // two parts in 4 bits
      return result;
    }

    public uint CreateBitMask(int from, int to)
    {
      uint r = 0;
      for (int i = from; i <= to; i++)
        r |= (uint)(1) << i;

      return r;
    }

    private const int MAX_CHECK_SUM_BITS_COUNT = 8;
  }
}

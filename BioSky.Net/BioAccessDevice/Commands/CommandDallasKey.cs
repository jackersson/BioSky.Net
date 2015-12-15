using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioAccessDevice.Abstract;


namespace BioAccessDevice.Commands
{
  public class CommandDallasKey : AccessDeviceCommand
  {
    public CommandDallasKey() : base()
    {
      _command                = new byte[] { (byte)AccessDeviceCommandID.READ_DALLAS_KEY, 0, 0, 8, 4 };
      _actualResponse         = new byte[DALLASKEY_COMMAND_RESPONSE_BYTES_COUNT];
      _noCardDetectedResponse = new byte[] { (byte)AccessDeviceCommandID.READ_DALLAS_KEY, 0, 0, 15, 15, 15, 15
                                                                                              , 15, 15, 15, 15
                                                                                              , 15, 15, 15, 15
                                                                                                      ,  3,  8 };
    }   

    public override bool Validate()
    {
      bool checkResponseSumValid =  _utils.CheckResponseSum(_actualResponse);
      bool commandEquality       = !_utils.Compare(_noCardDetectedResponse, _actualResponse);


      bool flag = commandEquality && checkResponseSumValid;
      if (flag)
      {
        byte[] dallasKey = GetDallayKey();

        _response = dallasKey;

       /* Console.WriteLine();
        for (int i = 0; i < dallasKey.Length; ++i)
          Console.Write(dallasKey[i] + " ");*/
      }
     
      return flag;
    }

    private byte[] GetDallayKey()
    {
      byte[] dallasKey = new byte[12];

      short beginOffset = 3;
      short endOffset   = 2;

      for (int i = beginOffset; i < _actualResponse.Length - endOffset; ++i)      
        dallasKey[i - beginOffset] = _actualResponse[i];      

      return dallasKey;
    }

    private byte[] _noCardDetectedResponse;

    private const short DALLASKEY_COMMAND_RESPONSE_BYTES_COUNT = 17;
  }
}

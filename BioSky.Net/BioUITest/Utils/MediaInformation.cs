using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;

namespace BioUITest.Utils
{
  [ProtoContract]
  public class MediaInformation
  {
    [ProtoMember(1)]
    public int Width { get; set; }

    [ProtoMember(2)]
    public int Height { get; set; }

    [ProtoMember(3)]
    public byte[] Bytes { get; set; }
  }
}

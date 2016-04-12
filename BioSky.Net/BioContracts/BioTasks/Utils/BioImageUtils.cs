using System.Drawing;
using System.IO;

namespace BioContracts.BioTasks.Utils
{
  public class BioImageUtils
  {
    public byte[] ImageToByte(Image img)
    {
      byte[] byteArray = new byte[0];
      using (MemoryStream stream = new MemoryStream())
      {
        img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        stream.Close();


        byteArray = stream.ToArray();
      }
      return byteArray;
    }

    public byte[] ImageToByte(Stream stream)
    {

      byte[] buffer = null;
      if (stream != null && stream.Length > 0)
      {
        using (BinaryReader br = new BinaryReader(stream))
        {
          buffer = br.ReadBytes((int)stream.Length);
        }
      }
      return buffer;
    }

    public Google.Protobuf.ByteString ImageToByteString(Image img)
    {
      byte[] bytes = ImageToByte(img);
      return Google.Protobuf.ByteString.CopyFrom(bytes);
    }

    public Google.Protobuf.ByteString ImageToByteString(Stream img)
    {
      byte[] bytes = ImageToByte(img);
      return Google.Protobuf.ByteString.CopyFrom(bytes);
    }

  }
}

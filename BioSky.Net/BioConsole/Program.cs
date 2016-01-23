
using System;
using Grpc.Core;
using BioFaceService;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace GreeterClient
{  
  class Program
  {
  
    public class BioFaceServiceClient
    {
      readonly BioFaceDetector.IBioFaceDetectorClient client;

      public BioFaceServiceClient(BioFaceDetector.IBioFaceDetectorClient client)
      {
        this.client = client;
      }
      
      /// <summary>
      /// Server-streaming example. Calls listFeatures with a rectangle of interest. Prints each response feature as it arrives.
      /// </summary>
      public async Task DetectFace(byte[] bytes)
      {
        try
        {
          Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);

          BioImage image = new BioImage() { Description = bs };

          var requests = new List<BioImage>
                    {
                       image
                    };

          System.Threading.CancellationToken token = new System.Threading.CancellationToken();
          using (var call = client.DetectFace())
          {

            var responseReaderTask = Task.Run(async () =>
            {
              while (await call.ResponseStream.MoveNext(token))
              {
                var note = call.ResponseStream.Current;

                foreach (ObjectInfo oi in note.Objects)                
                  Log("Got objects info \"{0}\"  {1}", oi.Confidence, oi.RotationAngle);               
              }
            });

            foreach (BioImage request in requests)
            {
              Log("Sending image ");

              await call.RequestStream.WriteAsync(request);
            }


            await call.RequestStream.CompleteAsync();
            await responseReaderTask;

            Log("Finished RouteChat");           
          }
        }
        catch (RpcException e)
        {
          Log("RPC failed " + e);
          throw;
        }
      }

      private void Log(string s, params object[] args)
      {
        Console.WriteLine(string.Format(s, args));
      }

      private void Log(string s)
      {
        Console.WriteLine(s);
      }        
    }

    public static byte[] ImageToByte2(Image img)
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


    public static void Main(string[] args)
    {
      Channel channel = new Channel("127.0.0.1:50051", Credentials.Insecure);

     
      var client = new BioFaceServiceClient(BioFaceDetector.NewClient(channel));
      //string name = "Taras";

      Image newFrame = Bitmap.FromFile("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\1.jpg");
      byte[] bytes = ImageToByte2(newFrame);

      client.DetectFace(bytes).Wait();

      channel.ShutdownAsync().Wait();
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }      
  }
}

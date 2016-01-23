using BioFaceService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC
{
  public class BioFaceClient
  {
    readonly BioFaceDetector.IBioFaceDetectorClient client;
    public delegate void FaceDetectedEventHandler  ( object sender, DetectedObjectsInfo objectsInfo );
    public delegate void EnrollFeedbackEventHandler( object sender, EnrollmentFeedback  feedback    );

    public event FaceDetectedEventHandler   FaceDetected  ;
    public event EnrollFeedbackEventHandler EnrollFeedback;

    protected virtual void OnFaceDetected(DetectedObjectsInfo objectsInfo)
    {
      if (FaceDetected != null)
        FaceDetected(this, objectsInfo);
    }

    public BioFaceClient(BioFaceDetector.IBioFaceDetectorClient client)
    {
      this.client = client;

      _imageRequests = new List<BioImage>();
    }

    public async Task Identify(BioImagesList image_list)
    {
      try
      {
        using (var call = client.IdentifyFace(image_list))
        {
          var responseStream = call.ResponseStream;
          StringBuilder responseLog = new StringBuilder("Result: ");

          while (await responseStream.MoveNext())
          {
            IdentificationFeedback feature = responseStream.Current;
            //responseLog.Append(feature.ToString());
            Console.WriteLine("");
            Console.WriteLine(feature.EnrollmentFeedback.ToString());
            Console.WriteLine(feature.ToString());

          }
          // Log(responseLog.ToString());
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task CaptureDeviceRequest(CommandCaptureDevice command)
    {
      try
      {
        CaptureDeviceList call = await client.CaptureDeviceSelectAsync(command);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task AccessDeviceRequest(CommandAccessDevice command)
    {
      try
      {
        AccessDeviceList call = await client.AccessDeviceSelectAsync(command);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task LocationRequest(CommandLocation command)
    {
      try
      {
        LocationList call = await client.LocationSelectAsync(command);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PhotoRequest(CommandPhoto command)
    {
      try
      {
        PhotoList call = await client.PhotoSelectAsync(command);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task CardRequest(CommandCard command)
    {
      try
      {
        CardList call = await client.CardSelectAsync(command);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task VisitorRequest(CommandVisitor command)
    {
      try
      {
        VisitorList call = await client.VisitorSelectAsync(command);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PersonRequest(CommandPerson command)
    {
      try
      {
        PersonList call = await client.PersonSelectAsync(command);
        Console.WriteLine(call.ToString());
        
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task VerifyImages(VerificationData verificationData)
    {
      try
      {
        using (var call = client.VerifyFace(verificationData))
        {
          var responseStream = call.ResponseStream;
          StringBuilder responseLog = new StringBuilder("Result: ");

          while (await responseStream.MoveNext())
          {
            VerificationFeedback feature = responseStream.Current;
            //responseLog.Append(feature.ToString());
            Console.WriteLine("");
            Console.WriteLine(feature.EnrollmentFeedback.ToString());
            Console.WriteLine(feature.ToString());

          }
          // Log(responseLog.ToString());
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task EnrollFace(BioImagesList imageList)
    {      
      try
      {
        using (var call = client.EnrollFace(imageList))
        {
          var responseStream = call.ResponseStream;
          StringBuilder responseLog = new StringBuilder("Result: ");

          while (await responseStream.MoveNext())
          {
            EnrollmentFeedback feature = responseStream.Current;
            //responseLog.Append(feature.ToString());
            Console.WriteLine("");
            Console.WriteLine(feature.ToString());

            EnrollFeedback(this, feature);

          }
         // Log(responseLog.ToString());
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }
    
    public async Task DetectFace(byte[] bytes)
    {
      try
      {
        Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);

        BioImage imageRequest = new BioImage() { Description = bs };
        _imageRequests.Clear();

        _imageRequests.Add(imageRequest);

        System.Threading.CancellationToken token = new System.Threading.CancellationToken();
        using (var call = client.DetectFace())
        {

          var responseReaderTask = Task.Run(async () =>
          {
            while (await call.ResponseStream.MoveNext(token))
            {
              var note = call.ResponseStream.Current;
              OnFaceDetected(note);
              foreach (ObjectInfo oi in note.Objects)
                Log("Got objects info \"{0}\"  {1}", oi.Confidence, oi.RotationAngle);
            }
          });

          foreach (BioImage image in _imageRequests)
          {
            Log("Sending image ");

            await call.RequestStream.WriteAsync(image);            
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

    private List<BioImage> _imageRequests;
                   
  }

}

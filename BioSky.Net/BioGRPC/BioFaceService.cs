using BioFaceService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioContracts.Services;

namespace BioGRPC
{
  public class BioFaceService : IFaceService
  {
    public BioFaceService( IProcessorLocator locator, BioFaceDetector.IBioFaceDetectorClient client )
    {
      _locator = locator;
      _client = client;
    }

    public async Task Identify(BioImagesList image_list)
    {
      try
      {
        
        using (var call = _client.IdentifyFace(image_list))
        {
          var responseStream = call.ResponseStream;
         
          while (await responseStream.MoveNext())
          {
            IdentificationFeedback feature = responseStream.Current;           
            Console.WriteLine("");           
            Console.WriteLine(feature.ToString());
          }         
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }
    
    public async Task Verify(VerificationData verificationData)
    {
      try
      {
       
        using (var call = _client.VerifyFace(verificationData))
        { 
          var responseStream = call.ResponseStream;
          
          while (await responseStream.MoveNext())
          {
            VerificationFeedback feature = responseStream.Current;          
            Console.WriteLine("");           
            Console.WriteLine(feature.ToString());
          }          
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task Enroll(BioImagesList imageList)
    {    
      /*  
      try
      {
       
        using (var call = _client.EnrollFace(imageList))
        {
          var responseStream = call.ResponseStream;        

          while (await responseStream.MoveNext())
          {
            EnrollmentFeedback feature = responseStream.Current;            
            Console.WriteLine("");
            Console.WriteLine(feature.ToString());
            EnrollFeedback(this, feature);
          }      
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
      */
    }

    /*
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
    */

    public delegate void FaceDetectedEventHandler  (object sender, DetectedObjectsInfo objectsInfo);
    public delegate void EnrollFeedbackEventHandler(object sender, EnrollmentFeedback  feedback   );

    public event FaceDetectedEventHandler   FaceDetected;
    public event EnrollFeedbackEventHandler EnrollFeedback;

    protected virtual void OnFaceDetected(DetectedObjectsInfo objectsInfo)
    {
      if (FaceDetected != null)
        FaceDetected(this, objectsInfo);
    }

    protected virtual void OnEnrollFeedback(EnrollmentFeedback objectsInfo)
    {
      if (EnrollFeedback != null)
        EnrollFeedback(this, objectsInfo);
    }


    private void Log(string s, params object[] args)
    {
      Console.WriteLine(string.Format(s, args));
    }

    private void Log(string s)
    {
      Console.WriteLine(s);
    }

    private readonly BioFaceDetector.IBioFaceDetectorClient _client;
    private readonly IProcessorLocator _locator;

  }

}

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

    public async Task Enroll(EnrollmentData enrollmentData)
    {          
      try
      {
        //TODO try to use only images

        using (var call = _client.EnrollFace(enrollmentData))
        {
          var responseStream = call.ResponseStream;        

          while (await responseStream.MoveNext())
          {
            EnrollmentFeedback feature = responseStream.Current;            
            Console.WriteLine("");
            Console.WriteLine(feature.ToString());
            OnEnrollFeedback(feature);
          }      
        }
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }     
    }
    

    
   // public delegate void EnrollFeedbackEventHandler(object sender, EnrollmentFeedback  feedback   );

 
    public event EnrollFeedbackEventHandler EnrollFeedbackChanged;


    protected virtual void OnEnrollFeedback(EnrollmentFeedback feedback)
    {
      if (EnrollFeedbackChanged != null)
        EnrollFeedbackChanged(this, feedback);
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

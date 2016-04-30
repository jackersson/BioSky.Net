using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioContracts.Services;
using BioContracts.Services.Common;

namespace BioGRPC
{
  public class BioFacialService : ServiceBase, IFaceService
  {
    public BioFacialService( IProcessorLocator locator  )
    {
      _locator = locator;      
    }

    public BioFacialService(IProcessorLocator locator, string address)
    {
      _locator = locator;
      Address  = address;
    }

    public async Task Configurate( IServiceConfiguration configuration )
    {
      try
      {
        SocketConfiguration config = new SocketConfiguration();
        config.Address             = configuration.DatabaseService;

        Response call = await _client.AddSocketAsync(config);
        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }
  

    /*
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
    */
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
           // Console.WriteLine("");           
           // Console.WriteLine(feature.ToString());
            OnVerifyFeedback(feature);
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
           // Console.WriteLine("");
           // Console.WriteLine(feature.ToString());
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
    
        
    public event EnrollFeedbackEventHandler       EnrollFeedbackChanged;
    public event VerificationFeedbackEventHandler VerifyFeedbackChanged;

    protected virtual void OnEnrollFeedback(EnrollmentFeedback feedback)
    {
      if (EnrollFeedbackChanged != null)
        EnrollFeedbackChanged(this, feedback);
    }

    protected virtual void OnVerifyFeedback(VerificationFeedback feedback)
    {
      if (VerifyFeedbackChanged != null)
        VerifyFeedbackChanged(this, feedback);
    }

    private void Log(string s, params object[] args)
    {
      Console.WriteLine(string.Format(s, args));
    }

    private void Log(string s)
    {
      Console.WriteLine(s);
    }

    protected override void CreateClient()
    {
      _client = BiometricFacialSevice.NewClient(Channel);
    }

    private BiometricFacialSevice.IBiometricFacialSeviceClient _client;
    private readonly IProcessorLocator _locator;

  }

}

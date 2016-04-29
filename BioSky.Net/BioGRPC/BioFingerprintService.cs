using BioContracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioService;
using Grpc.Core;
using BioContracts;
using BioContracts.Services.Common;
using BioContracts.BioTasks.Fingers;
using BioContracts.Common;

namespace BioGRPC
{
  public class BioFingerprintService : ServiceBase,  IFingerprintService
  {
    public BioFingerprintService(IProcessorLocator locator)
    {
      _locator  = locator;
      _notifier = locator.GetProcessor<INotifier>();

      _observer = new BioObserver<IFingerprintServiceObserver>();
    }

    public BioFingerprintService(IProcessorLocator locator, string address )
    {
      _locator = locator;
      Address  = address;

      _observer = new BioObserver<IFingerprintServiceObserver>();

    }

    public async Task Enroll(FingerprintData fingerprintData)
    {
      try
      {
        using (var call = _client.EnrollFingerprint(fingerprintData))
        {
          var responseStream = call.ResponseStream;
          while (await responseStream.MoveNext())
          {
            EnrollmentFeedback feedback = responseStream.Current;
            Console.WriteLine(feedback);
            OnFeedback(feedback);
            Console.WriteLine();
           // OnEnrollFeedback(feature);
          }
        }
       
        //Console.WriteLine(call);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    private void  OnFeedback( EnrollmentFeedback feedback)
    {
      foreach (KeyValuePair<int, IFingerprintServiceObserver> observer in _observer.Observers)
        observer.Value.OnFeedback(feedback);
    }

    public Task Verify(VerificationData verificationData)
    {
      throw new NotImplementedException();
    }

    protected override void CreateClient()
    {
      _client = BiometricFingerprintSevice.NewClient(Channel);
    }

    public void Subscribe(IFingerprintServiceObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IFingerprintServiceObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll()
    {
      _observer.UnsubscribeAll();
    }

    private BiometricFingerprintSevice.IBiometricFingerprintSeviceClient _client;

    private readonly IProcessorLocator _locator ;
    private readonly INotifier         _notifier;
    private BioObserver<IFingerprintServiceObserver> _observer;

  }
}

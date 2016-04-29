using BioContracts.BioTasks.Fingers;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IFingerprintService : IService, IBioObservable<IFingerprintServiceObserver>
  {
    Task Verify(VerificationData verificationData);
    Task Enroll(FingerprintData enrollmentData);
  }
}

using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public delegate void EnrollFeedbackEventHandler      (object sender, EnrollmentFeedback   feedback);
  public delegate void VerificationFeedbackEventHandler(object sender, VerificationFeedback feedback);
  public interface IFaceService : IService
  {
    event EnrollFeedbackEventHandler       EnrollFeedbackChanged;
    event VerificationFeedbackEventHandler VerifyFeedbackChanged;

    Task Configurate(IServiceConfiguration configuration);

    //Task Identify(BioImagesList    image_list);
    Task Verify  (VerificationData verificationData);
    Task Enroll  (EnrollmentData enrollmentData);    
  }
}

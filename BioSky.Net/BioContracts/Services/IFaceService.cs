using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public delegate void EnrollFeedbackEventHandler(object sender, EnrollmentFeedback feedback);
  public interface IFaceService
  {
    event EnrollFeedbackEventHandler EnrollFeedbackChanged;

    Task Identify(BioImagesList    image_list);
    Task Verify  (VerificationData verificationData);
    Task Enroll  (EnrollmentData enrollmentData);    
  }
}

using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IFaceService
  {
    Task Identify(BioImagesList    image_list);
    Task Verify  (VerificationData verificationData);
    Task Enroll  (BioImagesList    imageList);    
  }
}

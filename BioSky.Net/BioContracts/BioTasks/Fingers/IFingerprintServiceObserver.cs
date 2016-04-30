using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.BioTasks.Fingers
{
  public interface IFingerprintServiceObserver
  {
    void OnFeedback(EnrollmentFeedback feedback);
  }
}

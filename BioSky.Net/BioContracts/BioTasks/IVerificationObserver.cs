using System;

namespace BioContracts.BioTasks
{
  public interface IVerificationObserver
  {
    void OnVerificationFailure(Exception ex);

    void OnVerificationSuccess(bool state);

    void OnVerificationProgress(int progress);
  }
}

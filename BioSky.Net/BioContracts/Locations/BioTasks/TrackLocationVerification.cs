using BioContracts.BioTasks;
using BioContracts.Common;
using BioService;
using System;
using System.Collections.Generic;

namespace BioContracts.Locations.BioTasks
{
  public class TrackLocationVerification : IBioObservable<IVerificationObserver>
  {
    public TrackLocationVerification(IProcessorLocator locator)
    {
      _locator = locator;

      _database = locator.GetProcessor<IBioSkyNetRepository>();
      _bioService = locator.GetProcessor<IServiceManager>();

      _observer = new BioObserver<IVerificationObserver>();

    }

    public void StartByCard(string cardNumber, Location location)
    {
      Console.WriteLine(cardNumber);

      foreach (KeyValuePair<int, IVerificationObserver> observer in _observer.Observers)
        observer.Value.OnVerificationProgress(0);

      Person pp = _database.Persons.GetPersonByCardNumber(cardNumber);

      _visitor = new Visitor();
      _visitor.CardNumber = cardNumber;
      _visitor.Locationid = location.Id;
      _visitor.Time = DateTime.Now.Ticks;

      if (pp != null)
        _visitor.Personid = pp.Id;

      _visitor.Status = pp == null ? Result.Failed : Result.Success;

      _bioService.DatabaseService.VisitorDataClient.Add(_visitor);

      if (_visitor.Status == Result.Success)
        OnVerificationSuccess();
      else
        OnVerificationFailed();   
    }

    private void OnVerificationSuccess()
    {
      foreach (KeyValuePair<int, IVerificationObserver> observer in _observer.Observers)
        observer.Value.OnVerificationSuccess(true);
    }

    private void OnVerificationFailed()
    {
      foreach (KeyValuePair<int, IVerificationObserver> observer in _observer.Observers)
        observer.Value.OnVerificationFailure(new Exception("Test"));
    }

    public void Subscribe(IVerificationObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IVerificationObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll()
    {
      _observer.UnsubscribeAll();
    }

    private BioObserver<IVerificationObserver> _observer;

    private readonly IBioSkyNetRepository _database;
    private readonly IProcessorLocator _locator;
    private readonly IServiceManager _bioService;
    private Visitor _visitor;
  }

}

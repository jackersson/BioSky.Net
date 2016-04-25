using System;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using System.Collections.ObjectModel;
using BioContracts;
using BioService;
using BioModule.Utils;
using System.Windows;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Grpc.Core;
using BioContracts.Services;
using BioContracts.Common;
using BioContracts.Locations;

namespace BioModule.ViewModels
{
  public class UserContactlessCardViewModel : Conductor<IScreen>.Collection.OneActive, IUpdatable, IAccessDeviceObserver
  {
    public UserContactlessCardViewModel(IProcessorLocator locator, IUserBioItemsUpdatable imageViewer)
    {
      DisplayName = "Cards";
      CardState   = "Card number";

      _locator     = locator;
      _imageViewer = imageViewer;
      _bioService  = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database    = _locator.GetProcessor<IBioSkyNetRepository>();
      _dialogs     = _locator.GetProcessor<DialogsHolder>();
      
      //TODO put in locator
      CardEnrollment = new CardEnrollmentBarViewModel(locator);
      
      _userCards = new AsyncObservableCollection<Card>();
      
      IsEnabled = true;      
    }

    private void ActivateCardEnrollment(bool flag)
    {
      if (flag)      
        ActivateItem(CardEnrollment);         
      else      
        DeactivateItem(CardEnrollment, false);            
    }    
   
    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
      CardEnrollment.Subscribe(this);
      _imageViewer.ChangeBioImageModel( BioImageModelEnum.Faces);
    }

    protected override void OnDeactivate(bool close)
    {
      CardEnrollment.UnsubscribeAll();
      base.OnDeactivate(close);
    }

    public void CheckCard()
    {
      CanAddCard = false;

      //TODO make as validator
      if (string.IsNullOrEmpty(CardNumber))
      {
        CardState = "CardNumber";
        return;
      }

      Person person = _database.Persons.CardDataHolder.GetPersonByCardNumber(CardNumber);
      if (person != null)
        CardState = "Card is already used" + " " + person.Firstname + " " + person.Lastname;
      else
      {
        CardState = "Card is avaliable to use";
        CanAddCard = true;
      }
      
    }

    #region Update
    public void RefreshData()
    {
      if (!IsActive || _user == null)
        return;

      _userCards.Clear();
      foreach (Card card in _user.Cards)      
        _userCards.Add(card);      
    }

    public void Update(Person user)
    {
      if (user == null || ( user != null && user.Id <= 0))
        return;      
          
      _user = user;
      RefreshData();

      IsEnabled = true;
    }

    public async void AddCard()
    {
      if (!CanAddCard)
        return;

      var result = _dialogs.AreYouSureDialog.Show();

      if (!result.Value)
        return;

      Card card = new Card() { UniqueNumber = CardNumber
                             , Personid = _user.Id };    
      
      CanAddCard = false;

      try  {
        await _bioService.CardsDataClient.Add(_user.Id, card);
      }
      catch (RpcException e)  {
        _notifier.Notify(e);
      }         
    }
    #endregion  
    
    #region Interface
    public async void OnDeleteCards()
    {
      if (!CanDeleteCard)
        return;

      var result = _dialogs.AreYouSureDialog.Show();

      if (!result.Value)
        return;

      Card card = new Card() { Id = SelectedCard.Id };     

      try {
        await _bioService.CardsDataClient.Remove(_user.Id, card);
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }      
    }
    public void Apply() {}
    #endregion

    #region Observer
    public void OnCardDetected(string cardNumber)  {  CardNumber = cardNumber; }
        
    public void OnError(Exception ex, LocationDevice device) {}

    public void OnReady(bool isReady) {}
    #endregion

    #region UI
    public bool CanDeleteCard { get { return SelectedCard != null; } }

    private CardEnrollmentBarViewModel _cardEnrollment;
    public CardEnrollmentBarViewModel CardEnrollment
    {
      get { return _cardEnrollment; }
      set
      {
        if (_cardEnrollment != value)
        {
          _cardEnrollment = value;
          NotifyOfPropertyChange(() => CardEnrollment);
        }
      }
    }

    private Card selectedCard;
    public Card SelectedCard
    {
      get { return selectedCard; }
      set
      {
        if (selectedCard != value)
        {
          selectedCard = value;
          NotifyOfPropertyChange(() => SelectedCard);
          NotifyOfPropertyChange(() => CanDeleteCard);
        }
      }
    }

    private bool _canAddCard;
    public bool CanAddCard
    {
      get { return _canAddCard; }
      set
      {
        if (_canAddCard != value)
        {
          _canAddCard = value;
          NotifyOfPropertyChange(() => CanAddCard);
        }
      }
    }

    private AsyncObservableCollection<Card> _userCards;
    public AsyncObservableCollection<Card> UserCards
    {
      get { return _userCards; }
      set
      {
        if (_userCards != value)
        {
          _userCards = value;
          NotifyOfPropertyChange(() => UserCards);
        }
      }
    }
    
    private string _cardNumber;
    public string CardNumber
    {
      get { return _cardNumber; }
      set
      {
        if (_cardNumber != value)
        {
          _cardNumber = value;
          CheckCard();
          NotifyOfPropertyChange(() => CardNumber);
        }
      }
    }

    private bool _isEnabled;
    public bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        if (_isEnabled != value)
        {
          _isEnabled = value;
          NotifyOfPropertyChange(() => IsEnabled);
        }
      }
    }

    private string _cardState;
    public string CardState
    {
      get { return _cardState; }
      set
      {
        if (_cardState != value)
        {
          _cardState = value;
          NotifyOfPropertyChange(() => CardState);
        }
      }
    }

    private bool _cardEnrollmentExpanded;
    public bool CardEnrollmentExpanded
    {
      get { return _cardEnrollmentExpanded; }
      set
      {
        if (_cardEnrollmentExpanded != value)
        {
          _cardEnrollmentExpanded = value;
          ActivateCardEnrollment(_cardEnrollmentExpanded);
        }
      }
    }
    #endregion

    #region Global Variables
    private readonly DialogsHolder          _dialogs      ;
    private          Person                 _user         ;
    private readonly IProcessorLocator      _locator      ; 
    private readonly IDatabaseService       _bioService   ;
    private readonly INotifier              _notifier     ;
    private readonly IBioSkyNetRepository   _database     ;
    private          IUserBioItemsUpdatable _imageViewer  ;

    #endregion
  }
}

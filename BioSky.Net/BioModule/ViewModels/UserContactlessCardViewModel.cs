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

namespace BioModule.ViewModels
{
  public class UserContactlessCardViewModel : Screen, IUpdatable
  {
    public UserContactlessCardViewModel(IProcessorLocator locator)
    {
      DisplayName = "Cards";

      _locator    = locator;    

      _bioService    = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>(); 
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();


      CardState = "Card number";

      _observer  = new TrackLocationAccessDeviceObserver(locator);
      _observer.CardDetected += OnCardDetected;
      _userCards = new AsyncObservableCollection<Card>();


      _database.Persons.DataChanged += RefreshData;

      IsEnabled = false;
    }

    private void OnCardDetected(TrackLocationAccessDeviceObserver sender, string cardNumber)
    {
      CardNumber = cardNumber;
      CheckCard(cardNumber);
    }

    #region Update

    public void RefreshData()
    {
      if (!IsActive || _user == null)
        return;      

      IEnumerable<Card> cards = _database.CardHolder.Data.Where(x => x.Personid == _user.Id);

      if (cards == null)
        return;

      _userCards.Clear();
      foreach (Card card in cards)      
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

      Card card = new Card() { UniqueNumber = CardNumber
                             , EntityState = EntityState.Added
                             , Personid = _user.Id };
                
      Person person = new Person() { Id = _user.Id };
      person.Cards.Add(card);
      
      CanAddCard = false;

      try
      {
        await _bioService.PersonDataClient.Update(person);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }         
    }
    #endregion

    #region Database

    public void CheckCard(string cardNumber)
    {
      CanAddCard = false; 

      Card card = _bioEngine.Database().CardHolder.GetValue(cardNumber);
      if (card != null)
      {
        Person person = _bioEngine.Database().PersonHolder.GetValue(card.Personid);
        //TODO show in right way
        if (person != null)        
          CardState = "Card is already used" + " " + person.Firstname + " " + person.Lastname;             
      }      
      
      CardState = "Card is avaliable to use";
      CanAddCard = true;                  
    }
    #endregion
    
    #region Interface

    public async void OnDeleteCards()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (!result)
        return;

      Person person = new Person() { Id = _user.Id };

      Card card = SelectedCard;
      card.EntityState = EntityState.Deleted;

      person.Cards.Add(card);

      try
      {
        await _bioService.PersonDataClient.Update(person);
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }      
    }
    public void Apply() {}
    #endregion

    #region UI

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
      get
      {
        return _userCards;
      }
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
    #endregion

    #region Global Variables

    private          Person                            _user         ;
    private readonly IBioEngine                        _bioEngine    ;
    private readonly IProcessorLocator                 _locator      ; 
    private readonly IDatabaseService                  _bioService   ;
    private readonly INotifier                         _notifier     ;
    private readonly IBioSkyNetRepository              _database     ;
    private readonly TrackLocationAccessDeviceObserver _observer     ;
    private readonly DialogsHolder                     _dialogsHolder;


    #endregion

    #region AccessDevices
    /*private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }*/
    /*
    public void Subscribe()
    {
      if (_selectedAccessDevice == null)
      {
        AccessDeviceConnected = false;
        return;
      }

      IAccessDeviceEngine accessDeviceEngine = _bioEngine.AccessDeviceEngine();

      accessDeviceEngine.Add(_selectedAccessDevice);

      if (!accessDeviceEngine.HasObserver(this, _selectedAccessDevice))
        accessDeviceEngine.Subscribe(this, _selectedAccessDevice);

      AccessDeviceConnected = accessDeviceEngine.AccessDeviceActive(_selectedAccessDevice);
    }
    public void OnNext(AccessDeviceActivity value)
    {
      AccessDeviceConnected = true;

      if (value.Data != null)
      {
        _detectedCard.UniqueNumber = "";
        for (int i = 0; i < value.Data.Length; ++i)
          _detectedCard.UniqueNumber += value.Data[i];

        CanAddCard = true;

        TryToAddNewCard(_detectedCard.UniqueNumber);
      }
    }
    
    public void OnError(Exception error)
    {
      AccessDeviceConnected = false;
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }
    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _accessDevicesNames.Count); }
    }

    private string _selectedAccessDevice;
    public string SelectedAccessDevice
    {
      get { return _selectedAccessDevice; }
      set
      {
        if (_selectedAccessDevice != value)
        {
          _selectedAccessDevice = value;
          NotifyOfPropertyChange(() => SelectedAccessDevice);

          Subscribe();
        }
      }
    }
    public BitmapSource AccessDeviceConnectedIcon
    {
      get { return AccessDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

    private AsyncObservableCollection<string> _accessDevicesNames;
    public AsyncObservableCollection<string> AccessDevicesNames
    {
      get { return _accessDevicesNames; }
      set
      {
        if (_accessDevicesNames != value)
        {
          _accessDevicesNames = value;

          NotifyOfPropertyChange(() => AccessDevicesNames);
        }
      }
    }

    public bool AnyCardDetected
    {
      get { return _detectedCard != null; }
    }

    private bool _accessDeviceConnected;
    private bool AccessDeviceConnected
    {
      get { return _accessDeviceConnected; }
      set
      {
        if (_accessDeviceConnected != value)
        {
          _accessDeviceConnected = value;
          NotifyOfPropertyChange(() => AccessDeviceConnectedIcon);
        }
      }
    }
    */

    #endregion


    // private bool _dataChanged;
  }
}

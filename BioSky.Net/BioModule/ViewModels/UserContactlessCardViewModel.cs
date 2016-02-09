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

namespace BioModule.ViewModels
{

  public class UserContactlessCardViewModel : Screen, IObserver<AccessDeviceActivity>, IUpdatable
  {
    public UserContactlessCardViewModel(IBioEngine bioEngine, IProcessorLocator locator)
    {
      DisplayName = "Cards";

      _locator = locator;
      _bioEngine = bioEngine;

      _selector = locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();


      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();
      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;

      AccessDeviceConnected = false;

      _detectedCard = new Card();

      CardState = "Card number";

      _userCards = new ObservableCollection<Card>();

      IsEnabled = false;    
    }

    #region Update

    public void Update(Person user)
    {
      if (user == null)
        return;

      if (user.EntityState == EntityState.Added)
        return;

      IsEnabled = true;

      _user = user;    

      _userCards.Clear();

      _detectedCard.Personid = _user.Id;


      foreach (Card card in _bioEngine.Database().CardHolder.Data)
      {
        if (card.Personid == _user.Id)
          _userCards.Add(card);
      }
    }
    public void AddNewCard()
    {
      if (!CanAddNewCard)
        return;

      DetectedCard.EntityState = EntityState.Added;

      Card newCard = new Card(DetectedCard);
      UserCards.Add(newCard);

      //_dataChanged = true;

      NotifyOfPropertyChange(() => UserCards);

      CanAddNewCard = false;
    }
    #endregion

    #region Database

    public void AddNewCard(string cardNumber)
    {
      Card card;
      bool cardFound = _bioEngine.Database().CardHolder.DataSet.TryGetValue(cardNumber, out card);
      if (cardFound)
      {
        Person person;
        bool personFound = _bioEngine.Database().PersonHolder.DataSet.TryGetValue(card.Personid, out person);
        if (personFound)
          CardState = "Card is avaliable to use";
        else
        {
          CardState = "Card is already used" + " " + person.Firstname + " " + person.Lastname;
          CanAddNewCard = false;
        }
      }

      DetectedCard = _detectedCard;

      if (UserCards.Contains(DetectedCard))
        CanAddNewCard = false;
    }
    #endregion

    #region BioService
    public async Task CardUpdatePerformer(EntityState dbState)
    {
      PersonList personChangedList = new PersonList();

      if (dbState == EntityState.Unchanged)
      {
        foreach (Card card in UserCards)
        {
          if (card.EntityState != EntityState.Unchanged)
            _user.Cards.Add(card);
        }
      }
      else if (dbState == EntityState.Deleted)
      {
        ///selectedCard.EntityState = EntityState.Deleted;
        //cardList.Cards.Add(selectedCard);       
      }

     // _bioEngine.Database().Persons.DataChanged += CardHolder_DataUpdated; 

     // await _bioService.DatabaseService.CardUpdateRequest(cardList);
    }
    /*
    private void CardHolder_DataUpdated(System.Collections.Generic.IList<Card> list, Result result)
    {      
      //_bioEngine.Database().CardHolder.DataUpdated -= CardHolder_DataUpdated; 

      Card card = null;
      foreach (ResultPair currentResult in result.Status)
      {
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Remove)
          {
            card = currentResult.Card;
            Console.WriteLine("Card successfully Removed");
          }
          else
            Console.WriteLine("Data Updated");
        }
      }
    }
    */

    #endregion

    #region Interface
    public async void Apply()
    {
      await CardUpdatePerformer(EntityState.Unchanged);
    }

    public async void Remove(bool all)
    {
      if(!all)
      {
        if (selectedCard != null)
          await CardUpdatePerformer(EntityState.Deleted);
      }
      else if(all)
      {
        /*
        CardList cardList = new CardList();
        foreach(Card card in UserCards)
        {
          card.Dbstate = DbState.Remove;
          cardList.Cards.Add(card);
        }

        _bioEngine.Database().CardHolder.DataUpdated += CardHolder_DataUpdated;

        await _bioService.DatabaseService.CardUpdateRequest(cardList);
        */
      }
    }

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

    private bool _canAddNewCard;
    public bool CanAddNewCard
    {
      get { return _canAddNewCard; }
      set
      {
        if (_canAddNewCard != value)
        {
          _canAddNewCard = value;
          NotifyOfPropertyChange(() => CanAddNewCard);
        }
      }
    }

    private ObservableCollection<Card> _userCards;
    public ObservableCollection<Card> UserCards
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

    private Card _detectedCard;
    public Card DetectedCard
    {
      get { return _detectedCard; }
      set
      {
        if (_detectedCard == value)
        {
          _detectedCard = value;
          NotifyOfPropertyChange(() => DetectedCard);
          NotifyOfPropertyChange(() => AnyCardDetected);
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

    private Person _user;
    private readonly IBioEngine _bioEngine;
    private readonly IProcessorLocator _locator;
    private readonly ViewModelSelector _selector;
    private readonly IServiceManager _bioService;

    #endregion

    #region AccessDevices
    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }
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

        CanAddNewCard = true;

        AddNewCard(_detectedCard.UniqueNumber);
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

    #endregion


    // private bool _dataChanged;
  } 
}

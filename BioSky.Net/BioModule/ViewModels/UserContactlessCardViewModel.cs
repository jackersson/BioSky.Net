using System;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using System.Collections.ObjectModel;
using BioContracts;
using BioFaceService;
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

      //UserCards = _bioEngine.Database().Cards;

     
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


/*
    private void OnCardsChanged(CardList cards)
    {
      UserCards.Clear();

      if (_user == null)
        return;

      foreach (Card item in cards.Cards)
      {
        if (item.Personid != _user.Id)
          continue;

        if (UserCards.Contains(item))
          continue;

        UserCards.Add(item);
      }
    }*/

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

   // private bool _dataChanged;
    
    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
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
    
    public string AvaliableDevicesCount
    {
      get { return String.Format( "Available Devices ({0})", _accessDevicesNames.Count); }      
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

    private string _cardState;
    public string CardState
    {
      get { return _cardState; }
      set
      {
        if ( _cardState != value)
        {
          _cardState = value;
          NotifyOfPropertyChange(() => CardState);
        }
      }
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


    private bool _accessDeviceConnected;
    private bool AccessDeviceConnected
    {
      get {  return _accessDeviceConnected;  }
      set
      {
        if ( _accessDeviceConnected != value)
        {
          _accessDeviceConnected = value;
          NotifyOfPropertyChange(() => AccessDeviceConnectedIcon);
        }
      }
    }

    public BitmapSource AccessDeviceConnectedIcon
    {
      get  { return AccessDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }


    public void Update(Person user)
    {
      if (user == null)
        return;    

      if (user.Dbstate == DbState.Insert)
        return;

      IsEnabled = true;

      _user = user;

      //_dataChanged = false;

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

      DetectedCard.Dbstate = DbState.Insert;

      Card newCard = new Card(DetectedCard);
      UserCards.Add(newCard);

      //_dataChanged = true;
     
      NotifyOfPropertyChange(() => UserCards);

      CanAddNewCard = false;

    }

    public bool AnyCardDetected
    {
      get { return _detectedCard != null;  }
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
        if ( _userCards != value )
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
        if ( _detectedCard == value )
        {
          _detectedCard = value;
          NotifyOfPropertyChange(() => DetectedCard);
          NotifyOfPropertyChange(() => AnyCardDetected);          
        }
      }
    }

    public async Task CardUpdatePerformer()
    {             
      CardList cardList = new CardList();
      foreach (Card card in UserCards)
      {
        if (card.Dbstate != DbState.None)
          cardList.Cards.Add(card);
      }       

       _bioService.DatabaseService.CardUpdated += DatabaseService_CardUpdated;

       await _bioService.DatabaseService.CardUpdateRequest(cardList);      
    }

    private void DatabaseService_CardUpdated(CardList list, Result result)
    {
      PersonUpdateResultProcessing(list, result);

    }

    private void PersonUpdateResultProcessing(CardList list, Result result)
    {
      /*
      _bioService.DatabaseService.CardUpdated -= DatabaseService_CardUpdated;
      
      IBioSkyNetRepository database = _locator.GetProcessor<IBioSkyNetRepository>();
        
      string message = "";

      foreach (ResultPair rp in result.Status)
      {
        Card card = null;
        if (rp.Status == ResultStatus.Success)
        {
          if (rp.State == DbState.Insert)
            card = rp.Card;
          else
            card = list.Cards.Where(x => x.Id == rp.Id).FirstOrDefault();

          database.UpdateCardFromServer(card);

        }
        else
        {
          if (rp.State == DbState.Insert)
            message += rp.Status.ToString() + " " + rp.State.ToString() + " " + card.UniqueNumber + "\n";
        }

        if (card != null)
          message += rp.Status.ToString() + " " + rp.State.ToString() + " " + card.UniqueNumber + "\n";        
      }
      */
      //MessageBox.Show(message);      
    }

    public async void Apply()
    {
      await CardUpdatePerformer();

    }

    public async void Remove()
    {
      await CardUpdatePerformer();
    }

    public void OnNext(AccessDeviceActivity value)
    {
      AccessDeviceConnected = true;
     
      if (value.Data != null)
      {
        _detectedCard.UniqueNumber = "";
        for ( int i = 0; i < value.Data.Length; ++i )        
          _detectedCard.UniqueNumber += value.Data[i];

        CanAddNewCard = true;

        Card card;
        bool cardFound = _bioEngine.Database().CardHolder.DataSet.TryGetValue(_detectedCard.UniqueNumber, out card);       
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

    public void OnError(Exception error)
    {
      AccessDeviceConnected = false;
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    private Person                     _user      ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IProcessorLocator _locator   ;
    private readonly ViewModelSelector _selector  ;
    private readonly IServiceManager   _bioService;
  }

 
}

using System;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using System.Collections.ObjectModel;
using BioContracts;
using BioFaceService;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class UserContactlessCardViewModel : Screen, IObserver<AccessDeviceActivity>
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

      _bioEngine.Database().DataChanged += UserContactlessCardViewModel_DataChanged; 
    }

    protected async override void OnActivate()
    {
      await _bioService.DatabaseService.CardRequest(new CommandCard());
    }

    public void UserContactlessCardViewModel_DataChanged(object sender, EventArgs args)
    {
      OnCardsChanged(_bioEngine.Database().Cards);
    }

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

    private bool _dataChanged;
    
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
      _user = user;

      //_detectedCard.Personid = _user.;

      _dataChanged = false;
      OnCardsChanged(_bioEngine.Database().Cards);
      /*
      _userCards.Clear();

      IEnumerable<Card> cards = _bioEngine.Database().GetCards().Where(x => x.User == _user);

      foreach (Card card in cards)
        _userCards.Add(card);
      */
    }

    public void AddNewCard()
    {
      /*
      if (UserCards.Contains(DetectedCard))
      {
        MessageBox.Show("Card is Already in Use");
        return;
      }

      Card newCard = new Card() { CardID = DetectedCard.CardID, UserID = DetectedCard.UserID };
      UserCards.Add(DetectedCard);

      _dataChanged = true;
      //_bioEngine.Database().AddCard(newCard);
      NotifyOfPropertyChange(() => UserCards);
      */
    }

    public bool AnyCardDetected
    {
      get { return _detectedCard != null;  }
    }

    private ObservableCollection<Card> _userCards;
    public ObservableCollection<Card> UserCards
    {
      get { return _userCards; }
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

    public void Apply()
    {

      if (!_dataChanged)
        return;

      //_bioEngine.Database().UpdateCards(UserCards, _user);

      //foreach ( Card card in UserCards)
        //_bioEngine.Database().AddCard(card);
      /*string cardNumbers = "";
      foreach (Card cardNumber in UserCards )      
        cardNumbers += cardNumber + ",";

      return cardNumbers;*/
    }

    public void OnNext(AccessDeviceActivity value)
    {
      AccessDeviceConnected = true;
     
      if (value.Data != null)
      {
        _detectedCard.UniqueNumber = "";
        for ( int i = 0; i < value.Data.Length; ++i )        
          _detectedCard.UniqueNumber += value.Data[i];

/*
        Card card = _bioEngine.Database().GetCards().Where(x => x.User == _user).First();
        if (card != null)
          CardState = "Card is already used";
        else
          CardState = "Card is avaliable to use";
          */
        DetectedCard = _detectedCard;       
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

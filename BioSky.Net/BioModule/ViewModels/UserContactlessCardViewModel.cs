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

namespace BioModule.ViewModels
{

  public class UserContactlessCardViewModel : Screen, IObserver<AccessDeviceActivity>, IUpdatable
  {
    public UserContactlessCardViewModel(IBioEngine bioEngine, IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = "Cards";

      _locator       = locator      ;
      _bioEngine     = bioEngine    ;
      _windowManager = windowManager;

      _selector   = locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();
      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;

      AccessDeviceConnected = false;

      _detectedCard = new Card();

      CardState = "Card number";

      _userCards = new AsyncObservableCollection<Card>();

      _bioEngine.Database().Persons.DataChanged += RefreshData;

      IsEnabled = false;
    }

    #region Update

    public void RefreshData()
    {
      if (_user == null)
        return;

      IEnumerable<Card> cards = _bioEngine.Database().CardHolder.Data.Where(x => x.Personid == _user.Id);

      if (cards == null)
        return;

      _userCards.Clear();

      foreach (Card card in cards)      
        _userCards.Add(card);      

    }


    public void Update(Person user)
    {
      if (user == null)
        return;

      if (user.Id <= 0)
        return;

      IsEnabled = true;

      _user = user;


      RefreshData();

    }
    public async void AddNewCard()
    {
      if (!CanAddNewCard)
        return;

      DetectedCard.EntityState = EntityState.Added;
      DetectedCard.Personid = _user.Id;

      PersonList personList = new PersonList();
     
      Person personWithPhoto = new Person() { Id = _user.Id };
      personWithPhoto.Cards.Add(DetectedCard);

      personList.Persons.Add(personWithPhoto);

      CanAddNewCard = false;

      try
      {
        await _bioService.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }

    

      CanAddNewCard = false;
    }
    #endregion

    #region Database

    public void TryToAddNewCard(string cardNumber)
    {
      CanAddNewCard = false;

      Card card = _bioEngine.Database().CardHolder.GetValue(cardNumber);
      if (card != null)
      {
        Person person = _bioEngine.Database().PersonHolder.GetValue(card.Personid);
        if (person != null)
        {
          CardState = "Card is already used" + " " + person.Firstname + " " + person.Lastname;
        }
        else
        {
          CardState = "Card is avaliable to use";
          CanAddNewCard = true;
        }
      }
      else
      {
        CardState = "Card is avaliable to use";
        CanAddNewCard = true;
      }

      DetectedCard = _detectedCard;
      
    }
    #endregion

    #region BioService

    public async Task CardDeletePerformer()
    {
      PersonList personList = new PersonList();

      Person person = new Person()
      {
          EntityState = EntityState.Unchanged
        , Id = _user.Id
      };

      Card card = SelectedCard;
      card.EntityState = EntityState.Deleted;

      person.Cards.Add(card);
      personList.Persons.Add(person);

      try
      {
        _database.Persons.DataUpdated += UpdateData;
        await _bioService.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          if (person.Cards.Count > 1)
            MessageBox.Show(person.Cards.Count + " cards successfully Deleted");
          else
            MessageBox.Show("Card successfully Deleted");
        }
      }
    }

    #endregion

    #region Interface

    public async void OnDeleteCards()
    {
      var result = _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (result == true)
      {
        try
        {
          await CardDeletePerformer();
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    }
    public void Apply()
    {
      //await CardUpdatePerformer(EntityState.Unchanged);
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

    private          Person               _user         ;
    private readonly IBioEngine           _bioEngine    ;
    private readonly IProcessorLocator    _locator      ;
    private readonly ViewModelSelector    _selector     ;
    private readonly IServiceManager      _bioService   ;
    private readonly IWindowManager       _windowManager;
    private readonly IBioSkyNetRepository _database     ; 



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

    #endregion


    // private bool _dataChanged;
  }
}

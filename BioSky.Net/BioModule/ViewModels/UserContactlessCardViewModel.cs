using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using BioData;
using BioModule.Model;
using System.Collections.ObjectModel;
using BioContracts;
using System.Windows;

using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;

namespace BioModule.ViewModels
{
  public class UserContactlessCardViewModel : Screen, IObserver<AccessDeviceActivity>
  {
    public UserContactlessCardViewModel()
    {
      CardNumber = "123498767456345";
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

    public UserContactlessCardViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;
      DisplayName = "Cards";

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();

      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;

      AccessDeviceConnected = false;

      _detectedCard = new Card();

      CardState = "Card number";

      _userCards = new ObservableCollection<Card>();
    }

    override protected void OnActivate()
    {
      Console.WriteLine("Activated " + DisplayName);
    }

    override protected void OnDeactivate(bool canClose)
    {
      Console.WriteLine("Deactivated " + DisplayName);
    }

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


    public void Update(User user)
    {
      _user = user;

      _detectedCard.UserID = _user.UID;

      _dataChanged = false;

      _userCards.Clear();

      IEnumerable<Card> cards = _bioEngine.Database().GetCards().Where(x => x.User == _user);

      foreach (Card card in cards)
        _userCards.Add(card);
      
    }

    public void AddNewCard()
    {
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

      _bioEngine.Database().UpdateCards(UserCards, _user);

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
        _detectedCard.CardID = "";
        for ( int i = 0; i < value.Data.Length; ++i )        
          _detectedCard.CardID += value.Data[i];

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

    private User _user;
    private readonly IBioEngine _bioEngine;
  }

  public class ConvertToFormatedNumber : IValueConverter
  {
    public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var tb = new TextBlock();

      tb.Inlines.Add(new Run() { Text = (string)values, Background = System.Windows.Media.Brushes.Yellow });

      int chunkSize = 4;
      string number = (string)(values);
      string result = "";
      int stringLength = number.Length;
      for (int i = 0; i < stringLength ; i += chunkSize)
      {
        if (i + chunkSize > stringLength) chunkSize = stringLength - i;

        result += number.Substring(i, chunkSize) + " ";
      }        

      return result;
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

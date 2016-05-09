using System;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using BioModule.BioModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using BioContracts;
using System.Linq;

namespace BioModule.ViewModels
{

  public delegate void FingerChangedEventHandler(Finger finger);
  public interface IFingerSelector
  {
    Finger SelectedFinger { get; set; }

    event FingerChangedEventHandler SelectedFingerChanged; 
  }
  
  public class UserFingerViewModel : Screen, IUserBioItemsController, IFingerSelector
  {
    public UserFingerViewModel(IUserBioItemsUpdatable imageViewer)
    {
      DisplayName = "Fingers";

      _imageViewer = imageViewer;
      _imageViewer.UpdateBioItemsController(this);

      Images    = new AsyncObservableCollection<FingerprintItem>();
      _imageSet = new Dictionary<Finger, FingerprintItem>();

      Array fingers = Enum.GetValues(typeof(Finger));
      foreach (Finger fn in fingers)
      {
        FingerprintItem fi = new FingerprintItem(fn);
        _imageSet.Add(fn, fi);
        Images.Add(fi);
      }

      IsEnabled = true;
    }
   
    public void SelectFinger(Finger finger)
    {
      SelectedFinger = finger;      
    }

    public void Update(Person user)
    {
      if (user == null || (user != null && user.Id <= 0))
      {
        IsEnabled = false;
        return;
      }

      _user = user;
      RefreshData();

      IsEnabled = true;
    }

    private void RefreshData()
    {
      if (!IsActive || _user == null)
        return;

      ResetImages();

      BiometricData bioData = _user.BiometricData;
      Google.Protobuf.Collections.RepeatedField<FingerprintCharacteristic> fingerprints = bioData.Fingerprints;
      if (bioData != null && fingerprints != null && fingerprints.Count > 0)
      {
        foreach (FingerprintCharacteristic fc in fingerprints)
        {
          Finger pos = fc.Position;
          if (_imageSet.ContainsKey(pos))
          {
            _imageSet[pos].PhotoID = fc.Photoid;
            SelectedFinger = pos;
          }
        }          
      }
            
     // NotifyOfPropertyChange(() => Images);

      //if (Images.Count > 0)
    //  {       
        //PhotoAvailableText = LocExtension.GetLocalizedValue<string>("BioModule:lang:YourPhotos");
   //   }
      //else
       // PhotoAvailableText = LocExtension.GetLocalizedValue<string>("BioModule:lang:NoAvailablePhotos");
    }

    private void ResetImages()
    {
      foreach (FingerprintItem item in Images)
        item.Reset();
    }

    protected override void OnActivate()
    {
      base.OnActivate();

      _imageViewer.SetBioImageModel(BioImageModelType);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
    }

    public void Add(Photo photo)
    {
      throw new NotImplementedException();
    }

    public void Remove(Photo photo)
    {
      throw new NotImplementedException();
    }

    public void Next()
    {
      throw new NotImplementedException();
    }

    public void Previous()
    {
      throw new NotImplementedException();
    }


    #region UI

    private Finger _selectedFinger;
    public Finger SelectedFinger
    {
      get { return _selectedFinger; }
      set
      {
        if (_selectedFinger != value)
        {
          _selectedFinger = value;
          OnFingerChanged();
          NotifyOfPropertyChange(() => SelectedFinger);
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

    private AsyncObservableCollection<FingerprintItem> _images;
    public AsyncObservableCollection<FingerprintItem> Images
    {
      get { return _images; }
      set
      {
        if (_images != value)
        {
          _images = value;
          NotifyOfPropertyChange(() => Images);
        }
      }
    }

    private Dictionary<Finger, FingerprintItem> _imageSet;
    

    public bool CanNext
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool CanPrevious
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    private long _selectedItem;
    public long SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;

         // Photo photo = _database.Photos.GetValue(_selectedItem);
        //  _imageViewer.UpdateFromPhoto(photo);
        //  CurrentPhotoIndex = UserImages.IndexOf(_selectedItem);

          NotifyOfPropertyChange(() => SelectedItem);
          //NotifyOfPropertyChange(() => CanDeleteItem);
          NotifyOfPropertyChange(() => CanNext);
          NotifyOfPropertyChange(() => CanPrevious);
        }
      }
    }

    private void OnFingerChanged()
    {
      if (SelectedFingerChanged != null)
        SelectedFingerChanged(_selectedFinger);
    }
    public event FingerChangedEventHandler SelectedFingerChanged;

    #endregion

    public BioImageModelType BioImageModelType { get { return BioImageModelType.Fingers; }}
    public Person User { get { return _user; }}

    private Person                            _user            ;
    private IUserBioItemsUpdatable            _imageViewer     ;    
  }

  public class FingerprintItem : PropertyChangedBase
  {
    public FingerprintItem(Finger fingerType)
    {
      _fingerType = fingerType;
      PhotoID = ID_RESET_VALUE;
    }

    private long _photoId;
    public long PhotoID
    {
      get { return _photoId; }
      set
      {
        if (_photoId != value)
        {
          _photoId = value;
          NotifyOfPropertyChange(() => PhotoID);
        }
      }
    }    

    private Finger _fingerType;
    public Finger FingerType { get { return _fingerType; } }

    public void Reset() { PhotoID = ID_RESET_VALUE; }

    public const int ID_RESET_VALUE = -1;
  }
}

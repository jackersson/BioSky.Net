using BioContracts;
using BioService;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using BioContracts.Holders;
using BioData.Holders.Utils;

namespace BioData.Holders.Grouped
{
  public class FullLocationHolder : PropertyChangedBase, IFullLocationHolder, ILocationGetAble
  {
    public FullLocationHolder(IProcessorLocator locator)
    {
      DataSet = new Dictionary<long, Location>();     
      Data    = new AsyncObservableCollection<Location>();

      _accessDeviceHolder      = new AccessDeviceHolder     (this);
      _captureDeviceHolder     = new CaptureDeviceHolder    (this);
      _fingerprintDeviceHolder = new FingerprintDeviceHolder(this);
      _irisDeviceHolder        = new IrisDeviceHolder       (this);

      _fieldsUtils  = new ProtoFieldsUtils();

      _dialogsHolder = locator.GetProcessor<IDialogsHolder>();

      Google.Protobuf.Collections.RepeatedField<Location> data = new Google.Protobuf.Collections.RepeatedField<Location>();      
    }

    public void Init(Google.Protobuf.Collections.RepeatedField<Location> data)
    {
      Data = new AsyncObservableCollection<Location>(data);

      foreach (Location location in data)
      {
        if (location == null)
          continue;

        long id = location.Id;
        _dataSet.Add(id, location);

        _accessDeviceHolder     .Add(id, location.AccessDevice     );
        _captureDeviceHolder    .Add(id, location.CaptureDevice    );
        _fingerprintDeviceHolder.Add(id, location.FingerprintDevice);
        _irisDeviceHolder       .Add(id, location.IrisDevice       );
      }

      OnDataChanged();     
    }

    public void Add(Location requested, Location responded)
    {
      if (responded != null && responded.Dbresult == Result.Success && !_dataSet.ContainsKey(responded.Id))
      {
        requested.Id                    = responded.Id;

        if (responded.AccessInfo != null)
          requested.AccessInfo.AccessType = responded.AccessInfo.AccessType;

        _accessDeviceHolder     .UpdateFromResponse(requested, responded.AccessDevice     , null);
        _captureDeviceHolder    .UpdateFromResponse(requested, responded.CaptureDevice    , null);
        _fingerprintDeviceHolder.UpdateFromResponse(requested, responded.FingerprintDevice, null); 
        _irisDeviceHolder       .UpdateFromResponse(requested, responded.IrisDevice       , null);

        Data.Add(requested);
        _dataSet.Add(requested.Id, requested);        
      }

      OnDataChanged();

      ShowLocationResult(requested, responded);
    }

    public void Update( Location requested
                      , Location responded)
    {
      if (responded != null && responded.Dbresult == Result.Success)
      {
        Location oldItem = GetValue(requested.Id);
     
        if (oldItem != null)
        {         
          _accessDeviceHolder     .UpdateFromResponse(oldItem, responded.AccessDevice     , requested.AccessDevice     );
          _captureDeviceHolder    .UpdateFromResponse(oldItem, responded.CaptureDevice    , requested.CaptureDevice    );
          _fingerprintDeviceHolder.UpdateFromResponse(oldItem, responded.FingerprintDevice, requested.FingerprintDevice);
          _irisDeviceHolder       .UpdateFromResponse(oldItem, responded.IrisDevice       , requested.IrisDevice       );

          CopyFrom(responded, oldItem, requested);
        }        
      }

      OnDataChanged();
      ShowLocationResult(requested, responded);
    }

    public void Remove( Location requested
                      , Location responded)
    {   
      
      if (responded != null && responded.Dbresult == Result.Success)
      {
        _dataSet.Remove(requested.Id);
        var item = Data.Where(x => x.Id == requested.Id).FirstOrDefault();
        if (item != null)
        {
          _accessDeviceHolder     .UpdateFromResponse(item, responded.AccessDevice     , null);
          _captureDeviceHolder    .UpdateFromResponse(item, responded.CaptureDevice    , null);
          _fingerprintDeviceHolder.UpdateFromResponse(item, responded.FingerprintDevice, null);    
          _irisDeviceHolder       .UpdateFromResponse(item, responded.IrisDevice       , null);

          Data.Remove(item);
        }
      }      
      
      OnDataChanged();
      ShowLocationResult(requested, responded);
    }
      
    
    private void CopyFrom(Location from, Location to, Location requested)
    {   
      if (!string.IsNullOrEmpty(requested.LocationName))        
        to.LocationName = from.LocationName;

      bool hasDescription          = !string.IsNullOrEmpty(requested.LocationName);
      bool needToDeleteDescription = _fieldsUtils.IsDeleteState(from.Description);
      if (needToDeleteDescription)
        to.Description = string.Empty;
      else
      {
        if (hasDescription )
          to.Description = from.Description;
      }

      if (!string.IsNullOrEmpty(requested.MacAddress))
        to.MacAddress = from.MacAddress;

      #region personAccess 

      bool fromHasAccessInfo = from.AccessInfo != null;
      if (!fromHasAccessInfo)
        return;

      if (from.AccessInfo.EntityState == EntityState.Unchanged || from.AccessInfo.Dbresult == Result.Failed)
        return;
           
      bool toHasAccessInfo   = to.AccessInfo != null;     
      if (!toHasAccessInfo)
        to.AccessInfo = new AccessInfo();

      to.AccessInfo.AccessType = from.AccessInfo.AccessType;

      bool accessTypeChanged         = toHasAccessInfo && fromHasAccessInfo && to.AccessInfo.AccessType != from.AccessInfo.AccessType;
     
      switch (to.AccessInfo.AccessType)
      {
        case AccessInfo.Types.AccessType.None:
        case AccessInfo.Types.AccessType.All:
          to.AccessInfo.Persons.Clear();
          break;

        case AccessInfo.Types.AccessType.Custom:
          to.AccessInfo.Persons.Clear();
          to.AccessInfo.Persons.Add(from.AccessInfo.Persons);
          break;
      }
      #endregion
    }
    #region DisplayResults
    private void ShowLocationResult(Location requested, Location responded)
    {
      return;
      LocationItems.Clear();

      if (responded == null)
        responded = new Location() { Id = requested.Id, Dbresult = Result.Failed };

      TreeItem locationItem = new TreeItem()
      {
          Name = string.Format("Location: {0} ({1})", responded.LocationName, responded.Id)
        , IsSuccess = (responded.Dbresult == Result.Success) ? true : false
      };

      if (responded.AccessDevice != null)
      {
        AccessDevice accessDevice = responded.AccessDevice;
        string state = requested.AccessDevice.EntityState.ToString();
        locationItem.Members.Add(new TreeItem
        {
            Name = string.Format("Access Device: {0} ({1}) {2}", accessDevice.Portname, state)
          , IsSuccess = (accessDevice.Dbresult == Result.Success) ? true : false
        });
      }

      if (responded.CaptureDevice != null)
      {
        CaptureDevice captureDevice = responded.CaptureDevice;
        string state = requested.CaptureDevice.EntityState.ToString();
        locationItem.Members.Add(new TreeItem
        {
            Name = string.Format("Capture Device: {0} ({1}) {2}", captureDevice.Devicename, state)
          , IsSuccess = (captureDevice.Dbresult == Result.Success) ? true : false
        });
      }

      if (responded.AccessInfo.Persons != null)
      {
        if(responded.AccessInfo != null && responded.AccessInfo.Persons != null && responded.AccessInfo.Persons.Count > 0)
        {
          TreeItem personsItem = new TreeItem() { Name = "Persons", IsSuccess = true };

          foreach (Person person in responded.AccessInfo.Persons)
          {
            personsItem.Members.Add(new TreeItem
            {
                Name = string.Format("Person: {0} {1} ({2})", person.Firstname, person.Lastname, person.Id)
              , IsSuccess = (person.Dbresult == Result.Success) ? true : false
            });
          }

          locationItem.Members.Add(personsItem);
        }
      }

      LocationItems.Add(locationItem);

      _dialogsHolder.NotificationDialog.Update(LocationItems, "LocationNotificationDialog");
      _dialogsHolder.NotificationDialog.Show();
    }

    private List<TreeItem> _locationItems;
    public List<TreeItem> LocationItems
    {
      get
      {
        return (_locationItems == null) ? _locationItems = new List<TreeItem>()
                                     : _locationItems;
      }
    }


    private IDialogsHolder _dialogsHolder;


    #endregion

    public Location GetValue(long Id)
    {
      Location location = null;
      DataSet.TryGetValue(Id, out location);
      return location;
    }

    public Location GetValue(Location location)
    {
      if (location == null)
        return null;

      Location response = Data.Where(x => x.LocationName == location.LocationName && x.MacAddress == location.MacAddress).FirstOrDefault();
      return response;
    }
    
    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    #region Collections

    private AsyncObservableCollection<Location> _data;
    public AsyncObservableCollection<Location> Data
    {
      get { return _data; }
      private set
      {
        if (_data != value)
        {
          _data = value;
          NotifyOfPropertyChange(() => Data);
        }
      }
    } 

    private Dictionary<long, Location> _dataSet;
    public Dictionary<long, Location> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    public ICollection<string> AccessDevices{
      get { return _accessDeviceHolder.DataSet.Keys; }
    }

    public ICollection<string> CaptureDevices  {
      get { return _captureDeviceHolder.DataSet.Keys; }
    }

    public ICollection<string> FingerprintDevices {
      get { return _fingerprintDeviceHolder.DataSet.Keys; }
    }
    public ICollection<string> IrisDevices
    {
      get { return _irisDeviceHolder.DataSet.Keys; }
    }

    #endregion

    #region global variables
    private readonly AccessDeviceHolder      _accessDeviceHolder     ;
    private readonly CaptureDeviceHolder     _captureDeviceHolder    ;
    private readonly FingerprintDeviceHolder _fingerprintDeviceHolder;
    private readonly IrisDeviceHolder        _irisDeviceHolder       ;


    private readonly ProtoFieldsUtils        _fieldsUtils;

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Location>> DataUpdated;
    #endregion
  }
}

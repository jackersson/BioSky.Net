using BioContracts;
using BioService;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System;
using BioContracts.Holders;

namespace BioData.Holders.Grouped
{
  public class FullLocationHolder : PropertyChangedBase, IFullLocationHolder
  {
    public FullLocationHolder()
    {
      DataSet = new Dictionary<long, Location>();     
      Data = new AsyncObservableCollection<Location>();

      AccessDevices  = new List<AccessDevice>();
      CaptureDevices = new List<CaptureDevice>();
    }

    public void Init(Google.Protobuf.Collections.RepeatedField<Location> data)
    {      
      Data = new AsyncObservableCollection<Location>(data);
      
      foreach (Location location in data)
      {
        _dataSet.Add(location.Id, location);
        AccessDevices .Add(location.AccessDevice);
        CaptureDevices.Add(location.CaptureDevice);
      }     
      
      
      OnDataChanged();    
    }

    public void Add(Location requested, Location responded)
    {
      if (responded.Dbresult == Result.Success && !_dataSet.ContainsKey(responded.Id))
      {
        requested.Id         = responded.Id        ;
        requested.AccessType = responded.AccessType;

        AddAccessDevice (requested, requested.AccessDevice , responded.AccessDevice);
        AddCaptureDevice(requested, requested.CaptureDevice, responded.CaptureDevice);
 
        if (requested.AccessType == Location.Types.AccessType.Custom)
          requested.Persons.Add(responded.Persons);

        Data.Add(requested);
        _dataSet.Add(requested.Id, requested);        
      }
      OnDataChanged();
    }

    public void Update( Location requested
                      , Location responded)
    {
      if (responded.Dbresult == Result.Success)
      {
        Location oldItem = GetValue(requested.Id);

        if (oldItem != null)
          CopyFrom(responded, oldItem);
      }
    }

    private void CopyFrom(Location from, Location to)
    {
      if (from.LocationName != "")
        to.LocationName = from.LocationName;

      if (from.Description != "")
        to.Description = from.Description;

      if (from.AccessType != to.AccessType)
        to.AccessType = from.AccessType;

      if (from.AccessDevice != null)
        AddAccessDevice(to, to.AccessDevice, from.AccessDevice);

      if (from.CaptureDevice != null)
        AddAccessDevice(to, to.AccessDevice, from.AccessDevice);

      to.Persons.Clear();
      to.Persons.Add(from.Persons);
    }

    public void Remove( Location requested
                      , Location responded )
    {
      if (responded.Dbresult == Result.Success)
      {
        _dataSet.Remove(requested.Id);
        var item = Data.Where(x => x.Id == requested.Id).FirstOrDefault();
        if (item != null)
          Data.Remove(item);
      }
    }

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

      Location response = Data.Where(x => x.LocationName == location.LocationName && x.Description == location.Description).FirstOrDefault();
      return response;
    }

    public bool HasUserAccess(long locationID, long userID)
    {

      return true;
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    private void OnDataUpdated(Google.Protobuf.Collections.RepeatedField<Location> list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }

    private void AddAccessDevice (Location owner, AccessDevice requested, AccessDevice responded )
    {
      if (responded == null || requested == null)
        return;
            
       requested.Id         = responded.Id;
       requested.Locationid = owner.Id    ;      
    }

    private void AddCaptureDevice(Location owner, CaptureDevice requested, CaptureDevice responded)
    {
      if (responded == null || requested == null)
        return;

      requested.Id = responded.Id;
      requested.Locationid = owner.Id;
    }

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

    private List<AccessDevice> _accessDevices;
    public List<AccessDevice> AccessDevices
    {
      get { return _accessDevices; }
      private set
      {
        if (_accessDevices != value)
          _accessDevices = value;
      }
    }

    private List<CaptureDevice> _captureDevices;
    public List<CaptureDevice> CaptureDevices
    {
      get { return _captureDevices; }
      private set
      {
        if (_captureDevices != value)
          _captureDevices = value;
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

    private Dictionary<long, HashSet<long>> _allowedUsersDataSet;
    public Dictionary<long, HashSet<long>> AllowedUsersDataSet
    {
      get { return _allowedUsersDataSet; }
      private set
      {
        if (_allowedUsersDataSet != value)
          _allowedUsersDataSet = value;
      }
    }

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Location>> DataUpdated;    
  }
}

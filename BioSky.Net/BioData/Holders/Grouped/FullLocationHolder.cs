using BioContracts;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace BioData.Holders.Grouped
{
  public class FullLocationHolder : PropertyChangedBase, IFullHolder<Location>
  {
    public FullLocationHolder()
    {
      DataSet = new Dictionary<long, Location>();     
      Data = new AsyncObservableCollection<Location>();
    }


    public void Init(Google.Protobuf.Collections.RepeatedField<Location> data)
    {
    
      foreach (Location location in data)
      {
       // foreach (AccessDevice accessDevice in location.AccessDevices)
         // _accessDevices.Add(accessDevice, accessDevice.Id);

        //foreach (CaptureDevice captureDevice in location.CaptureDevices)
        //  _captureDevices.Add(captureDevice, captureDevice.Id);
        //TODO clear location list

       // _locations.Add(location, location.Id);
      }

      OnDataChanged();
    }

    public void Update( Google.Protobuf.Collections.RepeatedField<Location> updated
                      , Google.Protobuf.Collections.RepeatedField<Location> results)
    {
     
      bool success = false;
      
      foreach (Location location in results)
      {
        Clear(location.Id);
        //foreach (AccessDevice accessDevice in location.AccessDevices)
          //_accessDevices.UpdateItem(accessDevice, accessDevice.Id, accessDevice.EntityState, accessDevice.Dbresult);

        //foreach (CaptureDevice captureDevice in location.CaptureDevices)
          //_captureDevices.UpdateItem(captureDevice, captureDevice.Id, captureDevice.EntityState, captureDevice.Dbresult);

        //success = location.Dbresult == ResultStatus.Success;

        //_locations.UpdateItem(location, location.Id, location.EntityState, location.Dbresult);
      }

      //Console.WriteLine(_accessDevices.Data);

      if (success)
        OnDataUpdated(results);

      OnDataChanged();    
    }

    private void Clear(long locationid )
    {


      try
      {
        /*
        IEnumerable<AccessDevice> tempAccessDevices = _accessDevices.Data.Where(x => x.Locationid == locationid);
        if (tempAccessDevices != null)
        {
          //foreach (AccessDevice accessDevice in tempAccessDevices.ToList())
           // _accessDevices.UpdateItem(accessDevice, accessDevice.Id, EntityState.Deleted, accessDevice.Dbresult);
        }

        IEnumerable<CaptureDevice> tempCaptureDevices = _captureDevices.Data.Where(x => x.Locationid == locationid);
        if (tempCaptureDevices != null)
        {
         // foreach (CaptureDevice captureDevice in tempCaptureDevices.ToList())
           // _captureDevices.UpdateItem(captureDevice, captureDevice.Id, EntityState.Deleted, captureDevice.Dbresult);
        }
        */
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
   

      //Console.WriteLine(_locations.DataSet);
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

    public Location GetValue(long Id)
    {
      Location location = null;
      DataSet.TryGetValue(Id, out location);

      return location;
    }

    public void Add(Location requested, Location responded)
    {
      throw new NotImplementedException();
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

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Location>> DataUpdated;
    
  }
}

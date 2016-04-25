using BioService;
using System.Collections.Generic;

namespace BioData.Holders
{
  internal class CaptureDeviceHolder
  {
    public CaptureDeviceHolder(ILocationGetAble locationHolder)
    {
      _locationHolder = locationHolder;

      DataSet = new Dictionary<string, long>();
    }

    public void UpdateFromResponse(Location owner, CaptureDevice responded, CaptureDevice requested)
    {
      if (responded == null ||  responded.Dbresult != Result.Success )
        return;

      switch (responded.EntityState)
      {
        case EntityState.Added:
          if (owner.CaptureDevice == null)
            owner.CaptureDevice = new CaptureDevice();

          if (requested != null)
            owner.CaptureDevice.Devicename = requested.Devicename;

          Add(owner.Id, owner.CaptureDevice);
          break;

        case EntityState.Deleted:
          Remove(owner.CaptureDevice);
          owner.CaptureDevice = null;          
          break;
      }
    }

    private void Remove(CaptureDevice device)
    {
      if (device == null)
        return;

      DataSet.Remove(device.Devicename);
    }

    public void Add(long locationId, CaptureDevice device)
    {
      if (device == null)
        return;

      string deviceName = device.Devicename;
      if (!ContainesKey(deviceName))
        DataSet.Add(deviceName, locationId);
      else
      {
        Location location = _locationHolder.GetValue(DataSet[deviceName]);
        location.CaptureDevice = null;
        DataSet[deviceName] = locationId;
      }
    }

    private bool ContainesKey(string key)
    {
      long result;
      return DataSet.TryGetValue(key, out result);
    }

    private Dictionary<string, long> _dataSet;
    public Dictionary<string, long> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    private readonly ILocationGetAble _locationHolder;
  }
}

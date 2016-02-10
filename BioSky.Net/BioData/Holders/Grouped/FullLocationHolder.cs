﻿using BioContracts;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders.Grouped
{
  public class FullLocationHolder : IFullHolder<LocationList>
  {
    public FullLocationHolder( LocationHolder      locations
                             , AccessDeviceHolder  accessDevices
                             , CaptureDeviceHolder captureDevices)
    {
      _locations      = locations;
      _accessDevices  = accessDevices;
      _captureDevices = captureDevices;
    }


    public void Init(LocationList list)
    {
      Google.Protobuf.Collections.RepeatedField<Location> data = list.Locations;

      foreach (Location location in data)
      {
        foreach (AccessDevice accessDevice in location.AccessDevices)
          _accessDevices.Add(accessDevice, accessDevice.Id);

        foreach (CaptureDevice captureDevice in location.CaptureDevices)
          _captureDevices.Add(captureDevice, captureDevice.Id);
        //TODO clear location list

        _locations.Add(location, location.Id);
      }

      OnDataChanged();
    }

    public void Update(LocationList updated, LocationList results)
    {
      Google.Protobuf.Collections.RepeatedField<Location> data = results.Locations;

      bool success = false;
      foreach (Location location in data)
      {
        foreach (AccessDevice accessDevice in location.AccessDevices)
          _accessDevices.UpdateItem(accessDevice, accessDevice.Id, accessDevice.EntityState, accessDevice.Dbresult);

        foreach (CaptureDevice captureDevice in location.CaptureDevices)
          _captureDevices.UpdateItem(captureDevice, captureDevice.Id, captureDevice.EntityState, captureDevice.Dbresult);

        success = location.Dbresult == ResultStatus.Success;

        _locations.UpdateItem(location, location.Id, location.EntityState, location.Dbresult);
      }

      if (success)
        OnDataUpdated(results);

      OnDataChanged();    
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    private void OnDataUpdated(LocationList list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<LocationList> DataUpdated;

    private readonly LocationHolder      _locations     ;
    private readonly AccessDeviceHolder  _accessDevices ;
    private readonly CaptureDeviceHolder _captureDevices;
  }
}

﻿using BioData.Holders.Base;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts.Holders;
using System.IO;
using BioData.Holders.Utils;

namespace BioData.Holders
{
  public class PhotoHolder : HolderBase<Photo, long>, IPhotoHolder
  {
    public PhotoHolder(IOUtils ioUtils) : base() 
    {    
      _noDescriptionPhotos = new PhotoList();
      _checkedPhotos       = new HashSet<long>();
      _ioUtils = ioUtils;
    }
       
    protected override void UpdateDataSet(IList<Photo> list)
    {
      foreach (Photo photo in list)
      {
        Update(photo, photo.Id);
        SavePhoto(photo);
        PhotoExists(photo);
      }

      OnDataChanged();
      CheckPhotos();
    }

    public override void UpdateItem(Photo obj, long key, EntityState state, ResultStatus result)
    {
      if (result != ResultStatus.Success)
        return;
      base.UpdateItem(obj, key, state, result);
      SavePhoto  (obj);
      PhotoExists(obj);
      
    }


    private void SavePhoto ( Photo obj )
    {            
      if (obj.Description != null && obj.Description.Length > 0)
      {
        byte[] bytes = obj.Description.ToByteArray();
        _ioUtils.SaveFile(obj.FileLocation, bytes);
      }     
    }

    private bool PhotoExists( Photo obj )
    {
      if (_ioUtils.FileExists(obj.FileLocation) )
        return true;

      if (!_checkedPhotos.Contains(obj.Id))
      {
        _noDescriptionPhotos.Photos.Add(obj);
        _checkedPhotos.Add(obj.Id);
      }
      
      return false;
    }

    protected override void CopyFrom(Photo from, Photo to)
    {
      to.MergeFrom(from);
    }

    public override void Remove(long key)
    {
      base.Remove(key);
      var item = Data.Where(x => x.Id == key).FirstOrDefault();
      if (item != null)
      {
        Data.Remove(item);
      }
    }

    public void CheckPhotos()
    {
      if (_noDescriptionPhotos.Photos.Count > 0)
      {
        OnFullPhotoRequested(_noDescriptionPhotos);
        _noDescriptionPhotos.Photos.Clear();
      }
    } 

    private void OnFullPhotoRequested( PhotoList list)
    {
      if ( FullPhotoRequested != null )
        FullPhotoRequested(list);
    }

    public Photo GetValue(long id)
    {
      Photo photo = null;
      bool photoExist = DataSet.TryGetValue(id, out photo);
      return photo;
    }

   
    PhotoList _noDescriptionPhotos;

    HashSet<long> _checkedPhotos;

    private readonly IOUtils _ioUtils;

    public event FullPhotoRequest FullPhotoRequested;
  }
}

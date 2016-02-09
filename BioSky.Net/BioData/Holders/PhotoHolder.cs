using BioData.Holders.Base;
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

    public override void UpdateItem(Photo obj, long key, EntityState state)
    {
      base.UpdateItem(obj, key, state);
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
      //OnFullPhotoRequested(_noDescriptionPhotos);
      return false;
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
        

    PhotoList _noDescriptionPhotos;

    HashSet<long> _checkedPhotos;

    private readonly IOUtils _ioUtils;

    public event FullPhotoRequest FullPhotoRequested;
  }
}

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
     // _photos = new Dictionary<long, List<Photo>>();

      _noDescriptionPhotos = new PhotoList();

      _ioUtils = ioUtils;
    }
       
    /*
    public void UpdateDescription(IList<Photo> list)
    {
      foreach (Photo ph in list)
      {
        UpdateItem(ph, ph.Id, ph.EntityState);
        SavePhoto(ph);
      }      
    }
  
    protected override void AddToDataSet(Photo obj, long key)
    {      

     // long person_idkey = obj.Personid;
      /*
      if (_photos.ContainsKey(person_idkey))
      {
        if (obj.OriginType == PhotoOriginType.Loaded)
        {
          if (_photos[person_idkey].Contains(obj.Id))
          {
            Add(obj);
          }
        }
      }
      else
        _photos.Add(person_idkey, new List<Photo>());
        */
      //base.AddToDataSet(obj, obj.Id);
    //}

    public override void Update(Photo obj, long key)
    {
      //TODO check if photo changes person
      /*
      if (_photos.ContainsKey(key))
      {
        IList<Photo> photos = _photos[key];
        for (int i = 0; i < photos.Count; ++i)
        {
          Photo ph = photos[i];
          if (ph.Id == obj.Id)
            photos[i] = obj;
        }
      }
      */
      base.Update(obj, obj.Id);
    }

    public override void Remove(Photo obj, long key)
    {
      /*
      long person_idkey = obj.Personid;
      if (_photos.ContainsKey(person_idkey))
      {
        IList<Photo> photos = _photos[person_idkey];
        photos.Remove(obj);      
      }
      */
      base.Remove(obj, obj.Id);
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

    /*
    public override void Update(IList<Photo> list, Result result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        Photo photo = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            photo = currentResult.Photo;
          else
            photo = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (photo != null)
          {
            photo.Dbstate = DbState.None;
            UpdateItem(photo, photo.Id, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }
    */

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

      _noDescriptionPhotos.Photos.Add(obj);
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

    /*
    public IList<Photo> GetPersonPhoto(long id)
    {
      List<Photo> photos = null;
      _photos.TryGetValue(id, out photos);
      return photos;
    }
    */
    //Dictionary<long, List<Photo>> _photos;

    PhotoList _noDescriptionPhotos;

    private readonly IOUtils _ioUtils;

    public event FullPhotoRequest FullPhotoRequested;
  }
}

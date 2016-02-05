using BioData.Holders.Base;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts.Holders;

namespace BioData.Holders
{
  public class PhotoHolder : HolderBase<Photo, long>, IPhotoHolder
  {
    public PhotoHolder() : base() 
    {
      _photos = new Dictionary<long, List<Photo>>();
    }
       

    protected override void AddToDataSet(Photo obj, long key)
    {
      long person_idkey = obj.Personid;
      if (_photos.ContainsKey(person_idkey))
      {
        //TODO check photo type
        _photos[person_idkey].Add(obj);
      }
      else
        _photos.Add(person_idkey, new List<Photo>());

      base.AddToDataSet(obj, obj.Id);
    }

    protected override void Update(Photo obj, long key)
    {
      //TODO check if photo changes person
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
      base.Update(obj, obj.Id);
    }

    protected override void Remove(Photo obj, long key)
    {
      long person_idkey = obj.Personid;
      if (_photos.ContainsKey(person_idkey))
      {
        IList<Photo> photos = _photos[person_idkey];
        photos.Remove(obj);      
      }
      base.Remove(obj, obj.Id);
    }

    protected override void UpdateDataSet(IList<Photo> list)
    {
      foreach (Photo photo in list)      
        AddToDataSet(photo, photo.Personid);      
    }

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

    public IList<Photo> GetPersonPhoto(long id)
    {
      List<Photo> photos = null;
      _photos.TryGetValue(id, out photos);
      return photos;
    }

    Dictionary<long, List<Photo>> _photos;

  }
}

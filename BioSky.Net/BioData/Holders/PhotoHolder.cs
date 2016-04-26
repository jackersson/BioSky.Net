using BioContracts.Holders;
using BioData.Holders.Utils;
using BioService;
using System.Collections.Generic;
using System;
using static BioService.QueryPhoto.Types;
using System.Linq;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class PhotoHolder : IPhotoHolder
  {
    public PhotoHolder(IOUtils ioUtils) 
    {
      DataSet = new Dictionary<long, Photo>();
   
      _ioUtils = ioUtils;
    }

    public void UpdateFromResponse(IList<Photo> requested, IList<Photo> responded)
    {
      foreach (Photo respondedPhoto in responded)
        UpdateFromResponse(null, respondedPhoto);

      OnDataChanged();
    }

    public void UpdateFromResponse(Photo requested, Photo responded)
    {
      if (responded == null || responded.Dbresult != Result.Success)
        return;

      switch (responded.EntityState)
      {
        case EntityState.Added:
          requested.Id       = responded.Id;
          requested.PhotoUrl = responded.PhotoUrl;
          _ioUtils.SaveFile(requested.PhotoUrl, requested.Bytestring.ToArray());
          requested.Bytestring = Google.Protobuf.ByteString.Empty;
          Add(requested);
          break;

        case EntityState.Deleted:
          Remove(requested);
          break;        
      }

      OnDataChanged();
    }

    public void UpdateFromQuery(QueryPhoto query, IList<Photo> responded)
    {
      if (query == null)
        return;

      switch (query.WithBytes)
      {
        case PhotoResultType.Full   :
        case PhotoResultType.NoBytes:
        case PhotoResultType.Undefined:
          Add(responded);
          break;
        case PhotoResultType.OnlyBytes:
          SaveLocalPhotos(responded);
          break;
      }      
    }

    private void Remove(Photo photo)
    {
      if (photo == null)
        return;

      DataSet.Remove(photo.Id);
    }

    private void SaveLocalPhotos(IList<Photo> responded)
    {
      foreach (Photo ph in responded)
      {
        long id = ph.Id;
        if (ContainesKey(id) && ph.Bytestring.Count() > 0)
          _ioUtils.SaveFile(DataSet[id].PhotoUrl, ph.Bytestring.ToArray());
      }

      OnDataChanged();
    }

    public void Add(Photo photo)
    {
      if (photo == null)
        return;

      long id = photo.Id;
      if (!ContainesKey(id))
        DataSet.Add(id, photo);
      else           
        DataSet[id] = photo;

      CheckPhotosIfFileExisted(photo);
    }

    public void Add(IEnumerable<Photo> photos)
    {
      foreach (Photo photo in photos)
        Add(photo);

      OnDataChanged();
    }  

    private bool ContainesKey(long key)
    {
      Photo result;
      return DataSet.TryGetValue(key, out result);
    }
    
    public void CheckPhotosIfFileExisted(Photo photo)
    {
      if (photo == null)
        return;

      if (!_ioUtils.FileExists(photo.PhotoUrl))
        RequestPhotoById(photo.Id, PhotoResultType.OnlyBytes);
    }

    private HashSet<long> _photosIndexesWithoutExistingFile;
    public HashSet<long> PhotosIndexesWithoutExistingFile
    {
      get
      {
        if (_photosIndexesWithoutExistingFile == null)
          _photosIndexesWithoutExistingFile = new HashSet<long>();
        return _photosIndexesWithoutExistingFile;
      }
      private set
      {
        if (_photosIndexesWithoutExistingFile != value)
          _photosIndexesWithoutExistingFile = value;
      }
    }



    public Photo GetValue(long Id)
    {
      Photo photo = null;
      if (!DataSet.TryGetValue(Id, out photo))
      {
        if (Id > 0)
        {
          Task requestNotExisting = Task.Factory.StartNew(() => RequestPhotoById(Id));     
        }
      }
      return photo;
    }



    private Dictionary<long, Photo> _dataSet;
    public Dictionary<long, Photo> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    private void RequestPhotoById(long id, PhotoResultType queryType = PhotoResultType.NoBytes)
    {
      QueryPhoto query = new QueryPhoto();
      query.Photos.Add(id);
      query.WithBytes = queryType;

      OnRequestPhoto(query);
    }

    private void OnRequestPhoto( QueryPhoto query )
    {
      if (RequestPhoto != null)
        RequestPhoto(query);
    }

    public void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }



    public event DataChangedHandler DataChanged;
    public event RequestPhotoEventHandler RequestPhoto;
    public readonly IOUtils _ioUtils;
 
  }
}

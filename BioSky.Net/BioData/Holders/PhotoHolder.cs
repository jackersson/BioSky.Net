using BioData.Holders.Utils;
using BioService;
using System.Collections.Generic;

namespace BioData.Holders
{
  public class PhotoHolder 
  {
    public PhotoHolder(IOUtils ioUtils) : base() 
    {
      _ioUtils = ioUtils;
    }
    public void CheckPhotosIfFileExisted(Person person)
    {
      foreach (Photo photo in person.Photos)      
        CheckPhotosIfFileExisted(photo);      
    }

    public void CheckPhotosIfFileExisted(Visitor visitor)
    {
      CheckPhotosIfFileExisted(visitor.Fullphoto  );
      CheckPhotosIfFileExisted(visitor.Cropedphoto);      
    }

    public void CheckPhotosIfFileExisted(Photo photo)
    {
      if (photo == null)
        return;

      if (!_ioUtils.FileExists(photo.PhotoUrl))
        PhotosIndexesWithoutExistingFile.Add(photo.Id);
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

    public readonly IOUtils _ioUtils;
  }
}

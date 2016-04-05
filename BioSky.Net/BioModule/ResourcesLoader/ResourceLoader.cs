using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace BioModule.ResourcesLoader
{

  public static class BitmapConversion
  {
    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);
    public static BitmapSource BitmapToBitmapSource(Bitmap source)
    {
      IntPtr hBitMap = source.GetHbitmap();
      BitmapSource bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap( hBitMap
                                                                                     , IntPtr.Zero
                                                                                     , Int32Rect.Empty
                                                                                     , BitmapSizeOptions.FromEmptyOptions());
      DeleteObject(hBitMap);

      return bmp;
    }

    public static Bitmap BitmapSourceToBitmap(BitmapSource source)
    {      
      if (source == null)
        return null;

      var newSource = new FormatConvertedBitmap();
      newSource.BeginInit();
      newSource.Source = source;
      newSource.DestinationFormat = PixelFormats.Bgr24;
      newSource.EndInit();      

      Bitmap bitmap = new Bitmap( newSource.PixelWidth
                                , newSource.PixelHeight
                                , System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      var data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                , System.Drawing.Imaging.ImageLockMode.WriteOnly
                                , System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      try {
        newSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
      }
      catch ( Exception e) {
        Console.Write(e);
      }

      bitmap.UnlockBits(data);     

      return bitmap;
    }
  }
    
  public class ResourceLoader
  {
    private static object syncObject = new object();

    private static volatile ResourceLoader _instance;
    public static ResourceLoader Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new ResourceLoader();
          }
        }
        return _instance;
      }
    }

    private ResourceLoader() { }

    private static BitmapSource _addIconSource   ;
    private static BitmapSource _removeIconSource;
    private static BitmapSource _deleteIconSource;
    private static BitmapSource _okIconSource    ;
    private static BitmapSource _cancelIconSource;
    private static BitmapSource _errorIconSource ;

    private static BitmapSource _userInformationIconSource      ;
    private static BitmapSource _userFacesIconSource            ;
    private static BitmapSource _userFingerprintsIconSource     ;
    private static BitmapSource _userIricesIconSource           ;
    private static BitmapSource _userContactlessCardsIconSource ;
    private static BitmapSource _userDefaultImageIconSource     ;

    private static BitmapSource _cardIconSource        ;
    private static BitmapSource _verificationIconSource;
    private static BitmapSource _verificationFailedIconSource;

    private static BitmapSource _refreshIconSource;

    private static BitmapSource _uploadIconSource;
    private static BitmapSource _enrollFromPhotoIconSource;
    private static BitmapSource _enrollFromCaptureDeviceSource;
    private static BitmapSource _settingsIconSource;

    private static BitmapSource _addUserIconSource;
    private static BitmapSource _addLocationIconSource;
    private static BitmapSource _journalListIconSource;
    private static BitmapSource _usersListIconSource;
    private static BitmapSource _logoListIconSource;
    private static BitmapSource _trackingListIconSource;

    private static BitmapSource _loaderIconSource;
    private static BitmapSource _informationCircleIconSource;
    private static BitmapSource _leftArrowIconSource;
    private static BitmapSource _rightArrowIconSource;
    private static BitmapSource _downArrowIconSource;
    private static BitmapSource _thumbnailIconSource;
    private static BitmapSource _infoDialogIconSource;
    private static BitmapSource _helpDialogIconSource;
    private static BitmapSource _stopIconSource;
    private static BitmapSource _startIconSource;
    private static BitmapSource _plusIconSource;
    private static BitmapSource _homeIconSource;
    private static BitmapSource _warningIconSource;
    private static BitmapSource _fingerScanIconSource;
    private static BitmapSource _IrisScanImageIconSource;
    private static BitmapSource _faceScanColorIconSource;
    private static BitmapSource _fingerScanColorIconSource;
    private static BitmapSource _irisScanColorIconSource;

    public static BitmapSource IrisScanColorIconSource
    {
      get
      {
        if (_irisScanColorIconSource == null)
          _irisScanColorIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.iris_scan_color);
        return _irisScanColorIconSource;
      }
    }
    public static BitmapSource FingerScanColorIconSource
    {
      get
      {
        if (_fingerScanColorIconSource == null)
          _fingerScanColorIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.fingerprint_scan_color);
        return _fingerScanColorIconSource;
      }
    }
    public static BitmapSource FaceScanColorIconSource
    {
      get
      {
        if (_faceScanColorIconSource == null)
          _faceScanColorIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.face_scan_color);
        return _faceScanColorIconSource;
      }
    }
    public static BitmapSource IrisScanImageIconSource
    {
      get
      {
        if (_IrisScanImageIconSource == null)
          _IrisScanImageIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.iris_scan_image);
        return _IrisScanImageIconSource;
      }
    }
    public static BitmapSource FingerScanIconSource
    {
      get
      {
        if (_fingerScanIconSource == null)
          _fingerScanIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.finger_scan);
        return _fingerScanIconSource;
      }
    }

    public static BitmapSource WarningIconSource
    {
      get
      {
        if (_warningIconSource == null)
          _warningIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.warning);
        return _warningIconSource;
      }
    }
    public static BitmapSource HomeIconSource
    {
      get
      {
        if (_homeIconSource == null)
          _homeIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.home);
        return _homeIconSource;
      }
    }
    public static BitmapSource PlusIconSource
    {
      get
      {
        if (_plusIconSource == null)
          _plusIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.plus);
        return _plusIconSource;
      }
    }
    public static BitmapSource StartIconSource
    {
      get
      {
        if (_startIconSource == null)
          _startIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.Start);
        return _startIconSource;
      }
    }
    public static BitmapSource StopIconSource
    {
      get
      {
        if (_stopIconSource == null)
          _stopIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.Stop);
        return _stopIconSource;
      }
    }

    public static BitmapSource HelpDialogIconSource
    {
      get
      {
        if (_helpDialogIconSource == null)
          _helpDialogIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.help_dialog);
        return _helpDialogIconSource;
      }
    }

    public static BitmapSource InfoDialogIconSource
    {
      get
      {
        if (_infoDialogIconSource == null)
          _infoDialogIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.info_dialog);
        return _infoDialogIconSource;
      }
    }

    public static BitmapSource ThumbnailIconSource
    {
      get
      {
        if (_thumbnailIconSource == null)
          _thumbnailIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.thumnail);
        return _thumbnailIconSource;
      }
    }
    public static BitmapSource DownArrowIconSource
    {
      get
      {
        if (_downArrowIconSource == null)
          _downArrowIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.down_arrow);
        return _downArrowIconSource;
      }
    }
    public static BitmapSource LeftArrowIconSource
    {
      get
      {
        if (_leftArrowIconSource == null)
          _leftArrowIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.left_arrow);
        return _leftArrowIconSource;
      }
    }
    public static BitmapSource RightArrowIconSource
    {
      get
      {
        if (_rightArrowIconSource == null)
          _rightArrowIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.right_arrow);
        return _rightArrowIconSource;
      }
    }

    public static BitmapSource InformationCircleIconSource
    {
      get
      {
        if (_informationCircleIconSource == null)
          _informationCircleIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.info_circle);
        return _informationCircleIconSource;
      }
    }

    public static BitmapSource LoaderIconSource
    {
      get
      {
        if (_loaderIconSource == null)
          _loaderIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.loader);
        return _loaderIconSource;
      }
    }
    public static BitmapSource LogoIconSource
    {
      get
      {
        if (_logoListIconSource == null)
          
          _logoListIconSource = BitmapConversion.BitmapToBitmapSource((BioModule.Properties.Resources.logo).ToBitmap());
        return _logoListIconSource;
      }
    }
    public static BitmapSource TrackingIconSource
    {
      get
      {
        if (_trackingListIconSource == null)
          _trackingListIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.tracking);
        return _trackingListIconSource;
      }
    }
    public static BitmapSource CancelIconSource
    {
      get
      {
        if (_cancelIconSource == null)
          _cancelIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.cancel);
        return _cancelIconSource;
      }
    }

    public static BitmapSource AddIconSource
    {
      get
      {
        if (_addIconSource == null)
          _addIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.add);
        return _addIconSource;
      }
    }

    public static BitmapSource RemoveIconSource
    {
      get
      {
        if (_removeIconSource == null)
          _removeIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.remove);
        return _removeIconSource;
      }
    }


    public static BitmapSource DeleteIconSource
    {
      get
      {
        if (_deleteIconSource == null)
          _deleteIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.delete);
        return _deleteIconSource;
      }
    }

    public static BitmapSource OkIconSource
    {
        get
        {
            if (_okIconSource == null)
                _okIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.ok);
            return _okIconSource;
        }
    }

    public static BitmapSource ErrorIconSource
    {
      get
      {
        if (_errorIconSource == null)
          _errorIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.error);
        return _errorIconSource;
      }
    }

    //************************************************Image View**********************************************
    public static BitmapSource UploadIconSource
    {
      get
      {
        if (_uploadIconSource == null)
          _uploadIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.upload);
        return _uploadIconSource;
      }
    }
    public static BitmapSource EnrollFromPhotoIconSource
    {
      get
      {
        if (_enrollFromPhotoIconSource == null)
          _enrollFromPhotoIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.loadphotos);
        return _enrollFromPhotoIconSource;
      }
    }

    public static BitmapSource SettingsIconSource
    {
      get
      {
        if (_settingsIconSource == null)
          _settingsIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.settings);
        return _settingsIconSource;
      }
    }

    public static BitmapSource EnrollFromCaptureDeviceIconSource
    {
      get
      {
        if (_enrollFromCaptureDeviceSource == null)
          _enrollFromCaptureDeviceSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.screenshot);
        return _enrollFromCaptureDeviceSource;
      }
    }




    //************************************************Flayout Settings*****************************************

    public static BitmapSource RefreshIconSource
    {
      get
      {
        if (_refreshIconSource == null)
          _refreshIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.refresh);
        return _refreshIconSource;
      }
    }

    //************************************************Toolbar Settings*****************************************

    public static BitmapSource AddUserIconSource
    {
      get
      {
        if (_addUserIconSource == null)
          _addUserIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.add_user);
        return _addUserIconSource;
      }
    }

    public static BitmapSource AddLocationIconSource
    {
      get
      {
        if (_addLocationIconSource == null)
          _addLocationIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.add_location);
        return _addLocationIconSource;
      }
    }

    public static BitmapSource JournalListIconSource
    {
      get
      {
        if (_journalListIconSource == null)
          _journalListIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.journal_list);
        return _journalListIconSource;
      }
    }

    public static BitmapSource UsersListIconSource
    {
      get
      {
        if (_usersListIconSource == null)
          _usersListIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.users_list);
        return _usersListIconSource;
      }
    }

    //*************************************************** User Page *******************************************
    public static BitmapSource UserInformationIconSource
    {
      get
      {
        if (_userInformationIconSource == null)
          _userInformationIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.info);
        return _userInformationIconSource;
      }
    }



    public static BitmapSource UserFacesIconSource
    {
      get
      {
        if (_userFacesIconSource == null)
          _userFacesIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.facial_scan);
        return _userFacesIconSource;
      }
    }

    public static BitmapSource UserFingerprintIconSource
    {
      get
      {
        if (_userFingerprintsIconSource == null)
          _userFingerprintsIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.fingerprint_scan);
        return _userFingerprintsIconSource;
      }
    }

    public static BitmapSource UserIricesIconSource
    {
      get
      {
        if (_userIricesIconSource == null)
          _userIricesIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.iris_scan);
        return _userIricesIconSource;
      }
    }

    public static BitmapSource UserContactlessCardsIconSource
    {
      get
      {
        if (_userContactlessCardsIconSource == null)
          _userContactlessCardsIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.contact_card);
        return _userContactlessCardsIconSource;
      }
    }

    public static BitmapSource UserDefaultImageIconSource
    {
      get
      {
        if (_userDefaultImageIconSource == null)
          _userDefaultImageIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.user_image);
        return _userDefaultImageIconSource;
      }
    }

    //*************************************************** Track Control *******************************************

    public static BitmapSource VerificationIconSource
    {
      get
      {
        if (_verificationIconSource == null)
          _verificationIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.verified);
        return _verificationIconSource;
      }
    }

    public static BitmapSource VerificationFailedIconSource
    {
      get
      {
        if (_verificationFailedIconSource == null)
          _verificationFailedIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.not_verified);
        return _verificationFailedIconSource;
      }
    }

    public static BitmapSource CardIconSource
    {
      get
      {
        if (_cardIconSource == null)
          _cardIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.card_in_use);
        return _cardIconSource;
      }
    }

  }
}

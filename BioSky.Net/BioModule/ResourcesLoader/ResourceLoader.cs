using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace BioModule.ResourcesLoader
{

  public static class BitmapConversion
  {
    public static BitmapSource BitmapToBitmapSource(Bitmap source)
    {
      return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    source.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
    }
  }
  public static class ResourceLoader
  {
    private static BitmapSource _addIconSource   ;
    private static BitmapSource _removeIconSource;
    private static BitmapSource _deleteIconSource;
    private static BitmapSource _okIconSource    ;
    private static BitmapSource _cancelIconSource;


    private static BitmapSource _userInformationIconSource      ;
    private static BitmapSource _userFacesIconSource            ;
    private static BitmapSource _userFingerprintsIconSource     ;
    private static BitmapSource _userIricesIconSource           ;
    private static BitmapSource _userContactlessCardsIconSource ;
    private static BitmapSource _userDefaultImageIconSource     ;

    private static BitmapSource _cardIconSource        ;
    private static BitmapSource _verificationIconSource;

    private static BitmapSource _refreshIconSource;

    private static BitmapSource _uploadIconSource;

    private static BitmapSource _addUserIconSource;
    private static BitmapSource _addLocationIconSource;
    private static BitmapSource _journalListIconSource;
    private static BitmapSource _usersListIconSource;


    
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace BioShell.Views
{
  /// <summary>
  /// Interaction logic for BioShellView.xaml
  /// </summary>
  public partial class BioShellView : MetroWindow
  {
    public BioShellView()
    {
      InitializeComponent();
     
    }

    static public BitmapImage imageee;

    private BitmapImage image_source;
    private Image image_;
    public Uri CurrentImageSource
    {
      get
      {
        return new Uri("F:/C#/BioSkyNetSuccess/BioModule/Resources/refresh_blue.png", UriKind.RelativeOrAbsolute);
      }
    }

   
  }
}

using System.Windows;
using MahApps.Metro.Controls;
using System;

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
      Application.Current.MainWindow = this;  
    }

    
  }
}

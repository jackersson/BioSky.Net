using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using MahApps.Metro.Controls;
using Caliburn.Micro;

namespace BioModule.ViewModels.Dialogs
{
  class FlashLoadScreen : PropertyChangedBase
  {
    public async void ShowProgressDialog()
    {
      var metroWindow = (Application.Current.MainWindow as MetroWindow);

      var controller = await metroWindow.ShowProgressAsync("Please wait...", "Loading!");
      controller.SetIndeterminate();

      await Task.Delay(5000);

      controller.SetCancelable(true);

      double i = 0.0;
      while (i < 6.0)
      {
        double val = (i / 100.0) * 20.0;
        controller.SetProgress(val);
        controller.SetMessage("Loading resources: " + i + "...");

        if (controller.IsCanceled)
          break; //canceled progressdialog auto closes.

        i += 1.0;
        await Task.Delay(2000);
        //await TaskEx.Delay(2000);
      }

      await controller.CloseAsync();

      if (controller.IsCanceled)
      {
        await metroWindow.ShowMessageAsync("Cancel!", "You stopped initialization!");
      }
      else
      {
        await metroWindow.ShowMessageAsync("Success!", "Loading done!");
      }      
    } 
  }
}

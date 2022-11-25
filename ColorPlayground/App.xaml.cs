using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;

using WpfUtils;
using System.Diagnostics;

namespace ColorPlayground
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App: Application
  {
    public App()
    {
      // Hook up CommandManager to the WpfUtil library. This needs to be explicit;
      // a static initializer in CommandManagerWrapper would not run without referencing
      // CommandManagerWrapper, and if we need to reference it anyway, better indicate
      // explicitly *why*, instead of deferring to static initializer voodoo ...
      CommandManagerWrapper.Initialize();
      // ShutdownMode = ShutdownMode.OnLastWindowClose; // that's the default anyway
      Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      e.Handled = true;
      MessageBox.Show(
        e.Exception.Message,
        e.Exception.GetType().Name,
        MessageBoxButton.OK,
        MessageBoxImage.Error);
    }

    /// <summary>
    /// Instead of using a Startup Uri, create the window manually.
    /// This method is referenced in the header of app.xaml instead of
    /// a startup URI.
    /// </summary>
    private void App_Startup(object sender, StartupEventArgs e)
    {
      Trace.TraceInformation($"App.App_Startup enter");
      var mainWindow = new MainWindow();
      MainModel = new MainViewModel();
      mainWindow.DataContext = MainModel;
      Trace.TraceInformation($"App.App_Startup showing main window");
      mainWindow.Show();
      Trace.TraceInformation($"App.App_Startup exit");
    }

    public MainViewModel? MainModel { get; private set; }


  }


}

/*
 * (c) 2022   / luc.cluitmans
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using ColorLib;

using ColorPlayground.Models;

using WpfUtils;

namespace ColorPlayground
{
  public class MainViewModel: ViewModelBase
  {
    public MainViewModel()
    {
      ColorEntries = new ObservableCollection<ColorEntry> {
      };
      for(var i=0; i < 8; i++)
      {
        ColorEntries.Add(ColorEntry.RandomColor());
      }
      _selectedIndex = ColorEntries.Count > 0 ? 0 : -1;
    }

    public ICommand ExitCommand { get; } = new DelegateCommand(p => {
      var w = Application.Current.MainWindow;
      w?.Close();
    });

    public ColorEntry? CurrentEntry {
      get => _currentEntry;
      set {
        var old = _currentEntry;
        if(SetInstanceProperty(ref _currentEntry, value))
        {
          if(old != null)
          {
            old.IsCurrentEntry = false;
          }
          if(_currentEntry != null)
          {
            _currentEntry.IsCurrentEntry = true;
          }
        }
      }
    }
    private ColorEntry? _currentEntry;

    public int SelectedIndex {
      get => _selectedIndex;
      set {
        if(SetValueProperty(ref _selectedIndex, value))
        {
        }
      }
    }
    private int _selectedIndex;

    public ObservableCollection<ColorEntry> ColorEntries { get; }
  }
}

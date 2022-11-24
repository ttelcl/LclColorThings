/*
 * (c) 2022   / luc.cluitmans
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using ColorLib;

using WpfUtils;

namespace ColorPlayground.Models
{
  /// <summary>
  /// Combines a color with some metadata
  /// </summary>
  public class ColorEntry: ViewModelBase
  {
    /// <summary>
    /// Create a new ColorEntry
    /// </summary>
    public ColorEntry(
      string label,
      ColorDetail? current = null)
    {
      _label = label;
      _current = current ?? new ColorDetail(0.5, 0.5, 0.5);
    }

    public string Label {
      get => _label;
      set {
        if(SetValueProperty(ref _label, value))
        {
        }
      }
    }
    private string _label;

    public ColorDetail Current {
      get => _current;
      set {
        var oldR = _current.R;
        var oldG = _current.G;
        var oldB = _current.B;
        if(SetInstanceProperty(ref _current, value))
        {
          RaisePropertyChanged(nameof(HexColor));
          RaisePropertyChanged(nameof(CurrentColor));
          if(oldR != _current.R)
          {
            RaisePropertyChanged(nameof(HexColor));
          }
          if(oldG != _current.G)
          {
            RaisePropertyChanged(nameof(HexColor));
          }
          if(oldB != _current.B)
          {
            RaisePropertyChanged(nameof(HexColor));
          }
        }
      }
    }
    private ColorDetail _current;

    public Color CurrentColor {
      get => Current.WpfColor;
      set {
        if(value != Current.WpfColor)
        {
          Current = ColorDetail.FromBytes(value.R, value.G, value.B);
        }
      }
    }

    public string HexColor {
      get => Current.HexColor; 
      set {
        var c = ColorDetail.FromHex(value);
        Current = c;
      }
    }

    public double R {
      get => Current.R; 
      set {
        if(value != Current.R)
        {
          Current = new ColorDetail(value, Current.G, Current.B);
        }
      }
    }

    public double G {
      get => Current.G;
      set {
        if(value != Current.G)
        {
          Current = new ColorDetail(Current.R, value, Current.B);
        }
      }
    }

    public double B {
      get => Current.B;
      set {
        if(value != Current.B)
        {
          Current = new ColorDetail(Current.R, Current.G, value);
        }
      }
    }

  }
}
/*
 * (c) 2022   / luc.cluitmans
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using ColorLib;

using WpfUtils;

namespace ColorPlayground.Models
{
  /// <summary>
  /// Combines a mutable color with some metadata in MVVM style
  /// </summary>
  public class ColorEntry: ViewModelBase
  {
    /// <summary>
    /// Create a new ColorEntry
    /// </summary>
    public ColorEntry(
      string label,
      ColorValue? current = null)
    {
      _label = label;
      _current = current ?? new ColorValue(0.5, 0.5, 0.5);
      _currentBrush = new SolidColorBrush(_current.WpfColor);
      _contrastBrush = _current.Lightness > 0.5 ? Brushes.Black : Brushes.White;
      _stateMargin = new Thickness(50, 0, 50, 0);
      _isCurrentEntry = false;
    }

    private static int __randomIndex = 1;

    public static ColorEntry RandomColor()
    {
      var rng = new Random();
      var hue = rng.NextDouble()*360.0;
      var saturation = 1.0 - rng.NextDouble()*rng.NextDouble(); // weigh towards higher saturation
      var lightness = (rng.NextDouble() + rng.NextDouble()) / 2.0; // weigh towards 0.5
      var name = $"Random {__randomIndex++}";
      return new ColorEntry(name, ColorValue.FromHsl(hue, saturation, lightness));
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

    public bool IsCurrentEntry {
      get => _isCurrentEntry;
      set {
        if(SetValueProperty(ref _isCurrentEntry, value))
        {
          StateMargin = _isCurrentEntry ? new Thickness(0, 0, 0, 0) : new Thickness(50, 0, 50, 0);
        }
      }
    }
    private bool _isCurrentEntry;

    public Thickness StateMargin {
      get => _stateMargin;
      set {
        if(SetValueProperty(ref _stateMargin, value))
        {
        }
      }
    }
    private Thickness _stateMargin;

    public ColorValue Current {
      get => _current;
      set {
        var oldR = _current.R;
        var oldG = _current.G;
        var oldB = _current.B;
        var oldHue = _current.Hue ?? 0;
        var hadHue = _current.Hue.HasValue;
        var oldS = _current.Saturation;
        var oldL = _current.Lightness;
        if(SetInstanceProperty(ref _current, value))
        {
          CurrentBrush = new SolidColorBrush(_current.WpfColor);
          ContrastBrush = _current.Lightness > 0.5 ? Brushes.Black : Brushes.White;
          RaisePropertyChanged(nameof(HexColor));
          RaisePropertyChanged(nameof(CurrentColor));
          if(oldR != _current.R)
          {
            RaisePropertyChanged(nameof(R));
          }
          if(oldG != _current.G)
          {
            RaisePropertyChanged(nameof(G));
          }
          if(oldB != _current.B)
          {
            RaisePropertyChanged(nameof(B));
          }
          if(oldHue != (_current.Hue ?? 0))
          {
            RaisePropertyChanged(nameof(Hue));
          }
          if(hadHue != _current.Hue.HasValue)
          {
            RaisePropertyChanged(nameof(HasHue));
          }
          if(oldS != Current.Saturation)
          {
            RaisePropertyChanged(nameof(Saturation));
          }
          if(oldL != Current.Lightness)
          {
            RaisePropertyChanged(nameof(Lightness));
          }
        }
      }
    }
    private ColorValue _current;

    public Color CurrentColor {
      get => Current.WpfColor;
      set {
        if(value != Current.WpfColor)
        {
          Current = ColorValue.FromBytes(value.R, value.G, value.B);
        }
      }
    }

    public Brush CurrentBrush {
      get => _currentBrush;
      set {
        if(SetInstanceProperty(ref _currentBrush, value))
        {
        }
      }
    }
    private Brush _currentBrush;

    public Brush ContrastBrush {
      get => _contrastBrush;
      set {
        if(SetInstanceProperty(ref _contrastBrush, value))
        {
        }
      }
    }
    private Brush _contrastBrush;

    public string HexColor {
      get => Current.HexColor;
      set {
        var c = ColorValue.FromHex(value);
        Current = c;
      }
    }

    public double R {
      get => Current.R;
      set {
        if(value != Current.R)
        {
          Current = new ColorValue(value, Current.G, Current.B);
        }
      }
    }

    public double G {
      get => Current.G;
      set {
        if(value != Current.G)
        {
          Current = new ColorValue(Current.R, value, Current.B);
        }
      }
    }

    public double B {
      get => Current.B;
      set {
        if(value != Current.B)
        {
          Current = new ColorValue(Current.R, Current.G, value);
        }
      }
    }

    public double Hue {
      get => Current.Hue ?? 0;
      set {
        if(value != (Current.Hue ?? 0.0))
        {
          var hue = value;
          if(hue < 0.0)
          {
            hue += 360.0;
          }
          if(hue >= 360.0)
          {
            hue -= 360.0;
          }
          Current = ColorValue.FromHsl(hue, Current.Saturation, Current.Lightness);
        }
      }
    }

    public bool HasHue => Current.Hue.HasValue;

    public double Saturation {
      get => Current.Saturation;
      set {
        if(value != Current.Saturation)
        {
          Current = ColorValue.FromHsl(Current.Hue ?? 0.0, value, Current.Lightness);
        }
      }
    }

    public double Lightness {
      get => Current.Lightness;
      set {
        if(value != Current.Lightness)
        {
          Current = ColorValue.FromHsl(Current.Hue ?? 0.0, Current.Saturation, value);
        }
      }
    }

  }
}
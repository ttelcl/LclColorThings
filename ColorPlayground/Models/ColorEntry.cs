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
      _hueColor = (current == null || current.Saturation <= 0 || !current.Hue.HasValue) 
        ? new ColorValue(0.5, 0.5, 0.5) 
        : ColorValue.FromHsl(current.Hue, 1.0, 0.5);
      _currentBrush = new SolidColorBrush(_current.WpfColor);
      _hueBrush = new SolidColorBrush(_hueColor.WpfColor);
      _contrastBrush = _current.Lightness > 0.5 ? Brushes.Black : Brushes.White;
      _stateMargin = InactiveMargin;
      _stateHeight = InactiveHeight;
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
          StateMargin = _isCurrentEntry ? ActiveMargin : InactiveMargin;
          StateHeight = _isCurrentEntry ? ActiveHeight : InactiveHeight;
        }
      }
    }
    private bool _isCurrentEntry;

    public int StateHeight {
      get => _stateHeight;
      set {
        if(SetValueProperty(ref _stateHeight, value))
        {
        }
      }
    }
    private int _stateHeight;
    
    public int ActiveHeight => 60;

    public int InactiveHeight => 30;

    public Thickness ActiveMargin => new Thickness(5, 0, 5, 0);

    public Thickness InactiveMargin => new Thickness(30, 0, 30, 0);

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
          HueColor = _current.Saturation > 0 && _current.Hue.HasValue
            ? ColorValue.FromHsl(_current.Hue, 1.0, 0.5)
            : new ColorValue(0.5, 0.5, 0.5);
          if(oldR != _current.R)
          {
            RaisePropertyChanged(nameof(R));
            RaisePropertyChanged(nameof(R255));
          }
          if(oldG != _current.G)
          {
            RaisePropertyChanged(nameof(G));
            RaisePropertyChanged(nameof(G255));
          }
          if(oldB != _current.B)
          {
            RaisePropertyChanged(nameof(B));
            RaisePropertyChanged(nameof(B255));
          }
          if(oldHue != (_current.Hue ?? 0))
          {
            RaisePropertyChanged(nameof(Hue));
            RaisePropertyChanged(nameof(HueText));
          }
          if(hadHue != _current.Hue.HasValue)
          {
            RaisePropertyChanged(nameof(HasHue));
          }
          if(oldS != Current.Saturation)
          {
            RaisePropertyChanged(nameof(Saturation));
            RaisePropertyChanged(nameof(SaturationText));
          }
          if(oldL != Current.Lightness)
          {
            RaisePropertyChanged(nameof(Lightness));
            RaisePropertyChanged(nameof(LightnessText));
          }
        }
      }
    }
    private ColorValue _current;

    public ColorValue HueColor {
      get => _hueColor;
      set {
        if(SetInstanceProperty(ref _hueColor, value))
        {
          HueBrush = new SolidColorBrush(_hueColor.WpfColor);
        }
      }
    }
    private ColorValue _hueColor;

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

    public Brush HueBrush {
      get => _hueBrush;
      set {
        if(SetInstanceProperty(ref _hueBrush, value))
        {
        }
      }
    }
    private Brush _hueBrush;

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

    public string R255 {
      get => ColorValue.FractionToByte(R).ToString();
    }

    public string G255 {
      get => ColorValue.FractionToByte(G).ToString();
    }

    public string B255 {
      get => ColorValue.FractionToByte(B).ToString();
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

    public string HueText {
      get => HasHue ? Math.Round(Hue).ToString() + "\u00b0" : "-";
    }

    public string SaturationText {
      get => Math.Round(Saturation * 100.0).ToString() + "%";
    }

    public string LightnessText {
      get => Math.Round(Lightness * 100.0).ToString() + "%";
    }
  }
}
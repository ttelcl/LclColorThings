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

namespace ColorPlayground.Models
{
  /// <summary>
  /// Models a color value as r-g-b floats
  /// </summary>
  public class ColorDetail
  {
    /// <summary>
    /// Create a new ColorDetail
    /// </summary>
    public ColorDetail(double r, double g, double b)
    {
      if(r < 0.0 || r > 1.0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(r), "'r' must be between 0.0 and 1.0");
      }
      if(g < 0.0 || g > 1.0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(g), "'g' must be between 0.0 and 1.0");
      }
      if(b < 0.0 || b > 1.0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(b), "'b' must be between 0.0 and 1.0");
      }
      R = r;
      G = g;
      B = b;
      WpfColor = Color.FromRgb(
        Double255ToByte(R),
        Double255ToByte(G),
        Double255ToByte(B));
    }

    /// <summary>
    /// The red component
    /// </summary>
    public double R { get; }

    /// <summary>
    /// The green component
    /// </summary>
    public double G { get; }

    /// <summary>
    /// The blue component
    /// </summary>
    public double B { get; }

    /// <summary>
    /// The color as a System.Windows.Media.Color (WPF) color
    /// </summary>
    public Color WpfColor { get; } 

    /// <summary>
    /// Convert a double in the range 0.0 to 1.0 to a byte in the range
    /// 0 - 255. Values are distributed smoothly over that entire range by mapping
    /// 0.0 to 0 and mapping 1.0 to 256 (and truncating 256 back to 255
    /// for the extreme)
    /// </summary>
    public static byte Double255ToByte(double d)
    {
      var i = (int)Math.Truncate(d*256.0);
      return i<0 ? (byte)0 : i>255 ? (byte)255 : (byte)i;
    }

  }
}

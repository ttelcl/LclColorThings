/*
 * (c) 2022   / luc.cluitmans
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Newtonsoft.Json;

namespace ColorLib
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
        FractionToByte(R),
        FractionToByte(G),
        FractionToByte(B));
    }

    /// <summary>
    /// Create from byte values (instead of the default 0.0-1.0 ranges)
    /// </summary>
    public static ColorDetail FromBytes(byte rbyte, byte gbyte, byte bbyte)
    {
      return new ColorDetail(ByteToFraction(rbyte), ByteToFraction(gbyte), ByteToFraction(bbyte));
    }

    /// <summary>
    /// Create from a hex string (optional prefixed with a "#")
    /// </summary>
    public static ColorDetail FromHex(string hexcolor)
    {
      var hexcolor2 = hexcolor;
      if(hexcolor2.StartsWith("#"))
      {
        hexcolor2 = hexcolor2.Substring(1);
      }
      if(hexcolor2.Length != 6)
      {
        throw new ArgumentException(
          $"Unsupported input format: {hexcolor}");
      }
      var rbyte = Byte.Parse(hexcolor2[..2], NumberStyles.HexNumber);
      var gbyte = Byte.Parse(hexcolor2[2..4], NumberStyles.HexNumber);
      var bbyte = Byte.Parse(hexcolor2[4..6], NumberStyles.HexNumber);
      return new ColorDetail(ByteToFraction(rbyte), ByteToFraction(gbyte), ByteToFraction(bbyte));
    }

    /// <summary>
    /// The red component
    /// </summary>
    [JsonProperty("r")]
    public double R { get; }

    /// <summary>
    /// The green component
    /// </summary>
    [JsonProperty("g")]
    public double G { get; }

    /// <summary>
    /// The blue component
    /// </summary>
    [JsonProperty("b")]
    public double B { get; }

    /// <summary>
    /// The color as a System.Windows.Media.Color (WPF)
    /// </summary>
    [JsonIgnore]
    public Color WpfColor { get; }

    /// <summary>
    /// The hex color code (ignored when deserializing)
    /// </summary>
    [JsonProperty("hex")]
    public string HexColor { get => ToHexString(R, G, B); }

    /// <summary>
    /// Return the hex color code for an (r,g,b) triplet
    /// (represented by values in the range 0.0 - 1.0)
    /// </summary>
    public static string ToHexString(double r, double g, double b)
    {
      var rbyte = FractionToByte(r);
      var gbyte = FractionToByte(g);
      var bbyte = FractionToByte(b);
      return $"#{rbyte:x2}{gbyte:x2}{bbyte:x2}";
    }

    /// <summary>
    /// Convert a double in the range 0.0 to 1.0 to a byte in the range
    /// 0 - 255. Values are distributed smoothly over that entire range by mapping
    /// 0.0 to 0 and mapping 1.0 to 256 (and truncating 256 back to 255
    /// for the extreme)
    /// </summary>
    public static byte FractionToByte(double d)
    {
      var i = (int)Math.Truncate(d*256.0);
      return i<0 ? (byte)0 : i>255 ? (byte)255 : (byte)i;
    }

    /// <summary>
    /// Convert a byte to a double in the range 0.0 - 1.0
    /// </summary>
    public static double ByteToFraction(byte b)
    {
      return ((double)b)/255.0;
    }

  }
}

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
using DrawingColor = System.Drawing.Color;

using Newtonsoft.Json;

namespace ColorLib
{
  /// <summary>
  /// Models a color value as r-g-b floats
  /// </summary>
  public class ColorValue
  {
    /// <summary>
    /// Create a new ColorDetail
    /// </summary>
    public ColorValue(double r, double g, double b)
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
      WinFormsColor = DrawingColor.FromArgb(
        FractionToByte(R),
        FractionToByte(G),
        FractionToByte(B));
      RgbToHsl(R, G, B, out var hue, out var saturation, out var lightness);
      Hue = hue;
      Saturation = saturation;
      Lightness = lightness;
    }

    /// <summary>
    /// Create from byte values (instead of the default 0.0-1.0 ranges)
    /// </summary>
    public static ColorValue FromBytes(byte rbyte, byte gbyte, byte bbyte)
    {
      return new ColorValue(ByteToFraction(rbyte), ByteToFraction(gbyte), ByteToFraction(bbyte));
    }

    /// <summary>
    /// Create from a hex string (optional prefixed with a "#")
    /// </summary>
    public static ColorValue FromHex(string hexcolor)
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
      return new ColorValue(ByteToFraction(rbyte), ByteToFraction(gbyte), ByteToFraction(bbyte));
    }

    /// <summary>
    /// Create from HSL values
    /// </summary>
    /// <param name="hue">
    /// The hue. Range 0.0 - 360. If saturation is 0 this can be null.
    /// </param>
    /// <param name="saturation">
    /// The saturation (range 0.0 - 1.0)
    /// </param>
    /// <param name="lightness">
    /// The lightness (range 0.0 - 1.0)
    /// </param>
    public static ColorValue FromHsl(double? hue, double saturation, double lightness)
    {
      HslToRgb(hue, saturation, lightness, out var r, out var g, out var b);
      return new ColorValue(r, g, b);
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
    /// The color as a (WPF-compatible) System.Windows.Media.Color
    /// </summary>
    [JsonIgnore]
    public Color WpfColor { get; }

    /// <summary>
    /// The color as a (Windows Forms compatible) System.Drawing.Color
    /// </summary>
    [JsonIgnore]
    public DrawingColor WinFormsColor { get; }

    /// <summary>
    /// The hex color code. Ignored when deserializing.
    /// </summary>
    [JsonProperty("hex")]
    public string HexColor { get => ToHexString(R, G, B); }

    /// <summary>
    /// The HSL hue (range 0.0 - 360.0, or null if undefined). Ignored when deserializing.
    /// </summary>
    [JsonProperty("hue")]
    public double? Hue { get; }

    /// <summary>
    /// The HSL saturation (range 0.0 - 1.0). Ignored when deserializing.
    /// </summary>
    [JsonProperty("saturation")]
    public double Saturation { get; }

    /// <summary>
    /// The HSL lightness (range 0.0 - 1.0). Ignored when deserializing.
    /// </summary>
    [JsonProperty("lightness")]
    public double Lightness { get; }

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

    /// <summary>
    /// Convert an RGB fractions triplet to a HSL triplet. 
    /// </summary>
    /// <param name="r">
    /// The input red channel (range 0.0 - 1.0)
    /// </param>
    /// <param name="g">
    /// The input green channel (range 0.0 - 1.0)
    /// </param>
    /// <param name="b">
    /// The input blue channel (range 0.0 - 1.0)
    /// </param>
    /// <param name="h">
    /// The output Hue channel (range 0.0 - 360.0; null if undefined)
    /// </param>
    /// <param name="s">
    /// The output saturation channel (range 0.0 - 1.0)
    /// </param>
    /// <param name="l">
    /// The output lightness channel (range 0.0 - 1.0)
    /// </param>
    public static void RgbToHsl(
      double r, double g, double b,
      out double? h, out double s, out double l)
    {
      // Inspired by https://www.optifunc.com/blog/hsl-color-for-wpf-and-markup-extension-to-lighten-darken-colors-in-xaml

      // Get the maximum and minimum RGB components.
      double max = r;
      if(max < g)
      {
        max = g;
      }
      if(max < b)
      {
        max = b;
      }

      double min = r;
      if(min > g)
      {
        min = g;
      }
      if(min > b)
      {
        min = b;
      }

      double diff = max - min;
      l = (max + min) / 2;
      if(Math.Abs(diff) < 0.00001)
      {
        s = 0;
        h = null;  // H is really undefined.
      }
      else
      {
        if(l <= 0.5)
        {
          s = diff / (max + min);
        }
        else
        {
          s = diff / (2 - max - min);
        }

        double r_dist = (max - r) / diff;
        double g_dist = (max - g) / diff;
        double b_dist = (max - b) / diff;

        double hv;
        if(r == max)
        {
          hv = b_dist - g_dist;
        }
        else if(g == max)
        {
          hv = 2.0 + r_dist - b_dist;
        }
        else
        {
          hv = 4.0 + g_dist - r_dist;
        }

        hv *= 60.0;
        if(hv < 0.0)
        {
          hv += 360.0;
        }
        if(hv >= 360.0)
        {
          hv -= 360.0;
        }
        h = hv;
      }
    }

    /// <summary>
    /// Convert a HSL triplet to a RGB fractions triplet
    /// </summary>
    /// <param name="h">
    /// The input Hue (range 0.0 - 360.0; can be null if s==0.0)
    /// </param>
    /// <param name="s">
    /// The input saturation
    /// </param>
    /// <param name="l">
    /// The input lightness
    /// </param>
    /// <param name="r">
    /// The output Red channel
    /// </param>
    /// <param name="g">
    /// The output Green channel
    /// </param>
    /// <param name="b">
    /// The output Blue channel
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="h"/> is null but <paramref name="s"/> is not 0.
    /// </exception>
    public static void HslToRgb(
      double? h, double s, double l,
      out double r, out double g, out double b)
    {
      // Inspired by https://www.optifunc.com/blog/hsl-color-for-wpf-and-markup-extension-to-lighten-darken-colors-in-xaml

      if(s == 0)
      {
        r = l;
        g = l;
        b = l;
      }
      else
      {
        if(!h.HasValue)
        {
          throw new ArgumentException(
            "Expecting hue to be non-null when saturation is not zero");
        }
        var hv = h.Value;
        if(hv < 0.0)
        {
          hv += 360;
        }
        if(hv >= 360.0)
        {
          hv -= 360;
        }
        var p2 = (l <= 0.5) ? l * (1 + s) : l + s - l * s;
        var p1 = 2 * l - p2;
        r = QqhToRgb(p1, p2, hv + 120);
        g = QqhToRgb(p1, p2, hv);
        b = QqhToRgb(p1, p2, hv - 120);
      }
    }

    private static double QqhToRgb(double q1, double q2, double hue)
    {
      if(hue >= 360.0)
      {
        hue -= 360.0;
      }
      else if(hue < 0.0)
      {
        hue += 360.0;
      }

      if(hue < 60)
      {
        return q1 + (q2 - q1) * hue / 60;
      }
      else if(hue < 180)
      {
        return q2;
      }
      else if(hue < 240)
      {
        return q1 + (q2 - q1) * (240 - hue) / 60;
      }
      else
      {
        return q1;
      }
    }

  }
}

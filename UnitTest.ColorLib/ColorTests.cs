using Xunit;
using Xunit.Abstractions;

using Newtonsoft.Json;

using ColorLib;

namespace UnitTest.ColorLib
{
  public class ColorTests
  {
    private readonly ITestOutputHelper _output;

    public ColorTests(ITestOutputHelper output)
    {
      _output = output;
    }

    [Fact]
    public void CanConvertToHex()
    {
      var yellow = new ColorValue(1, 1, 0);
      Assert.Equal("#ffff00", yellow.HexColor);
    }

    [Fact]
    public void ByteConversionsAreIdempotent()
    {
      // Make sure that ColorDetail.ByteToFraction and ColorDetail.FractionToByte
      // act as pseudo-inverse (from the viewpoint of bytes), despite implementation
      // differences
      for(byte b=0; b<255; b++)
      {
        var d = ColorValue.ByteToFraction(b);
        var b2 = ColorValue.FractionToByte(d);
        Assert.Equal(b, b2);
      }
    }

    [Fact]
    public void CanParseHexColors()
    {
      var white = ColorValue.FromHex("#ffffff");
      Assert.Equal(1.0, white.R);
      Assert.Equal(1.0, white.G);
      Assert.Equal(1.0, white.B);
      var black = ColorValue.FromHex("000000");
      Assert.Equal(0.0, black.R);
      Assert.Equal(0.0, black.G);
      Assert.Equal(0.0, black.B);
      var violet = ColorValue.FromHex("ff00ff");
      Assert.Equal(1.0, violet.R);
      Assert.Equal(0.0, violet.G);
      Assert.Equal(1.0, violet.B);
    }

    [Fact]
    public void CanSerializeAndDeserialize()
    {
      var color = ColorValue.FromHex("#4ef2b0");
      var json = JsonConvert.SerializeObject(color, Formatting.Indented);
      _output.WriteLine(json);
      var color2 = JsonConvert.DeserializeObject<ColorValue>(json);
      Assert.Equal("#4ef2b0", color2!.HexColor);
    }

    [Fact]
    public void RgbToHlsTests()
    {
      // Grays
      ColorValue.RgbToHsl(0.0, 0.0, 0.0, out var hue, out var saturation, out var lightness);
      Assert.Null(hue);
      Assert.Equal(0.0, saturation);
      Assert.Equal(0.0, lightness);
      ColorValue.RgbToHsl(1.0, 1.0, 1.0, out hue, out saturation, out lightness);
      Assert.Null(hue);
      Assert.Equal(0.0, saturation);
      Assert.Equal(1.0, lightness);
      ColorValue.RgbToHsl(0.5, 0.5, 0.5, out hue, out saturation, out lightness);
      Assert.Null(hue);
      Assert.Equal(0.0, saturation);
      Assert.Equal(0.5, lightness);

      // Primary & Secondary colors
      ColorValue.RgbToHsl(1.0, 0.0, 0.0, out hue, out saturation, out lightness);
      Assert.Equal(0.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.5, lightness);
      ColorValue.RgbToHsl(1.0, 1.0, 0.0, out hue, out saturation, out lightness);
      Assert.Equal(60.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.5, lightness);
      ColorValue.RgbToHsl(0.0, 1.0, 0.0, out hue, out saturation, out lightness);
      Assert.Equal(120.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.5, lightness);
      ColorValue.RgbToHsl(0.0, 1.0, 1.0, out hue, out saturation, out lightness);
      Assert.Equal(180.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.5, lightness);
      ColorValue.RgbToHsl(0.0, 0.0, 1.0, out hue, out saturation, out lightness);
      Assert.Equal(240.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.5, lightness);
      ColorValue.RgbToHsl(1.0, 0.0, 1.0, out hue, out saturation, out lightness);
      Assert.Equal(300.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.5, lightness);

      // non-primary saturated
      ColorValue.RgbToHsl(1.0, 0.5, 0.5, out hue, out saturation, out lightness);
      Assert.Equal(0.0, hue);
      Assert.Equal(1.0, saturation);
      Assert.Equal(0.75, lightness);

      // non-saturated
      ColorValue.RgbToHsl(0.75, 0.25, 0.25, out hue, out saturation, out lightness);
      Assert.Equal(0.0, hue);
      Assert.Equal(0.5, saturation);
      Assert.Equal(0.5, lightness);
      ColorValue.RgbToHsl(0.75, 0.75, 0.25, out hue, out saturation, out lightness);
      Assert.Equal(60.0, hue);
      Assert.Equal(0.5, saturation);
      Assert.Equal(0.5, lightness);
    }

    [Fact]
    public void HlsToRgbTests()
    {
      // Gray
      ColorValue.HslToRgb(null, 0.0, 0.42, out var r, out var g, out var b);
      Assert.Equal(0.42, r);
      Assert.Equal(0.42, g);
      Assert.Equal(0.42, b);

      // Primaries & Secondaries
      ColorValue.HslToRgb(0.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(1.0, r);
      Assert.Equal(0.0, g);
      Assert.Equal(0.0, b);

      ColorValue.HslToRgb(60.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(1.0, r);
      Assert.Equal(1.0, g);
      Assert.Equal(0.0, b);

      ColorValue.HslToRgb(-300.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(1.0, r);
      Assert.Equal(1.0, g);
      Assert.Equal(0.0, b);

      ColorValue.HslToRgb(120.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(0.0, r);
      Assert.Equal(1.0, g);
      Assert.Equal(0.0, b);

      ColorValue.HslToRgb(180.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(0.0, r);
      Assert.Equal(1.0, g);
      Assert.Equal(1.0, b);

      ColorValue.HslToRgb(240.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(0.0, r);
      Assert.Equal(0.0, g);
      Assert.Equal(1.0, b);

      ColorValue.HslToRgb(-120.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(0.0, r);
      Assert.Equal(0.0, g);
      Assert.Equal(1.0, b);

      ColorValue.HslToRgb(300.0, 1.0, 0.5, out r, out g, out b);
      Assert.Equal(1.0, r);
      Assert.Equal(0.0, g);
      Assert.Equal(1.0, b);
    }

    [Fact]
    public void HslRoundtripTest()
    {
      var c = ColorValue.FromHsl(0.0, 1.0, 0.5);
      Assert.Equal(1.0, c.R);
      Assert.Equal(0.0, c.G);
      Assert.Equal(0.0, c.B);
      Assert.Equal(0.0, c.Hue);
      Assert.Equal(1.0, c.Saturation);
      Assert.Equal(0.5, c.Lightness);

      c = ColorValue.FromHsl(42.0, 0.7, 0.6);
      var json = JsonConvert.SerializeObject(c, Formatting.Indented);
      _output.WriteLine(json);
      // Some rounding errors are to be expected, so compare to limited precision
      var precision = 12;
      Assert.Equal(42.0, c.Hue!.Value, precision);
      Assert.Equal(0.7, c.Saturation, precision);
      Assert.Equal(0.6, c.Lightness, precision);
    }
  }
}

using Xunit;
using Xunit.Abstractions;

using ColorLib;
using Newtonsoft.Json;

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
      var yellow = new ColorDetail(1, 1, 0);
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
        var d = ColorDetail.ByteToFraction(b);
        var b2 = ColorDetail.FractionToByte(d);
        Assert.Equal(b, b2);
      }
    }

    [Fact]
    public void CanParseHexColors()
    {
      var white = ColorDetail.FromHex("#ffffff");
      Assert.Equal(1.0, white.R);
      Assert.Equal(1.0, white.G);
      Assert.Equal(1.0, white.B);
      var black = ColorDetail.FromHex("000000");
      Assert.Equal(0.0, black.R);
      Assert.Equal(0.0, black.G);
      Assert.Equal(0.0, black.B);
      var violet = ColorDetail.FromHex("ff00ff");
      Assert.Equal(1.0, violet.R);
      Assert.Equal(0.0, violet.G);
      Assert.Equal(1.0, violet.B);
    }

    [Fact]
    public void CanSerializeAndDeserialize()
    {
      var color = ColorDetail.FromHex("#4ef2b0");
      var json = JsonConvert.SerializeObject(color, Formatting.Indented);
      _output.WriteLine(json);
      var color2 = JsonConvert.DeserializeObject<ColorDetail>(json);
      Assert.Equal("#4ef2b0", color2!.HexColor);
    }
  }
}

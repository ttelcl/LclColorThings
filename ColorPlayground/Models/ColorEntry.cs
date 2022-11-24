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
  /// Combines a color with some metadata
  /// </summary>
  public class ColorEntry
  {
    /// <summary>
    /// Create a new ColorEntry
    /// </summary>
    public ColorEntry()
    {
      WpfColor = Color.FromRgb(255, 255, 255);
    }

    public double R { get; set; }

    public Color WpfColor { get; set; }

  }
}
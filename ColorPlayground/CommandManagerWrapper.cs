/*
 * (c) 2019   / TTELCL
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WpfUtils;
using System.Windows.Input;

namespace ColorPlayground
{
  /// <summary>
  /// Wraps CommandManager to be usable in .NET standard libraries
  /// </summary>
  public class CommandManagerWrapper : ICommandManagerWrapper
  {
    public event EventHandler RequerySuggested
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Run the default initialization, registering
    /// </summary>
    public static void Initialize()
    {
      CommandManagerReference.Initialize(new CommandManagerWrapper());
    }

  }

}

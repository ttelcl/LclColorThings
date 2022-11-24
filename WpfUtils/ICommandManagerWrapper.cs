/*
 * (c) 2019   / TTELCL
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfUtils
{
  /// <summary>
  /// Implementations expose the CommandManager.RequerySuggested event
  /// to this library. Since this is a .net Standard library, it cannot
  /// access CommandManager itself
  /// </summary>
  public interface ICommandManagerWrapper
  {
    /// <summary>
    /// The event that should redirect to CommandManager.RequerySuggested.
    /// Beware that the same restrictions apply: listeners should
    /// store their event delegate to prevent garbage collection.
    /// </summary>
    event EventHandler RequerySuggested;
  }

  /// <summary>
  /// Singleton helper to import the system CommandManager
  /// </summary>
  public static class CommandManagerReference
  {
    private static ICommandManagerWrapper __host;

    /// <summary>
    /// Initialize this singleton helper
    /// </summary>
    public static void Initialize(ICommandManagerWrapper host)
    {
      __host = host;
    }

    /// <summary>
    /// The wrapper around the system ICommandManagerWrapper
    /// </summary>
    public static ICommandManagerWrapper Host
    {
      get
      {
        if(__host==null)
        {
          throw new InvalidOperationException(
            "CommandManagerReference is not initialized");
        }
        return __host;
      }
    }
  }

}


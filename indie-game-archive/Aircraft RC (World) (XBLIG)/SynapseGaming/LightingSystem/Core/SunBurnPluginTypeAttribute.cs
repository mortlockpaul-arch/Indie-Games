using System;
using System.Runtime.CompilerServices;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Assembly attribute used to register plugin classes with SunBurn.
///
/// This is required by plugins that are automatically loaded, and added to a
/// project using the Plugin Manager tool.
///
/// Usage:
///
///     [assembly: SunBurnPluginTypeAttribute(typeof(YourNamespace.YourClass))]
///
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class SunBurnPluginTypeAttribute : Attribute
{
	[CompilerGenerated]
	private string HCB;

	[CompilerGenerated]
	private string HC_0002;

	/// <summary />
	public string AssemblyQualifiedName
	{
		[CompilerGenerated]
		get
		{
			return HCB;
		}
		[CompilerGenerated]
		private set
		{
			HCB = hCB;
		}
	}

	/// <summary />
	public string FullName
	{
		[CompilerGenerated]
		get
		{
			return HC_0002;
		}
		[CompilerGenerated]
		private set
		{
			HC_0002 = text;
		}
	}

	/// <summary>
	/// Assembly attribute used to register plugin classes with SunBurn.
	/// </summary>
	/// <param name="type">The plugin class must implement the IPlugin interface.</param>
	public SunBurnPluginTypeAttribute(Type type)
	{
		AssemblyQualifiedName = type.AssemblyQualifiedName;
		FullName = type.FullName;
	}
}

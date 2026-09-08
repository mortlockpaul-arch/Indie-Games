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
	private string _3A_0018;

	[CompilerGenerated]
	private string _3AL;

	/// <summary />
	public string AssemblyQualifiedName
	{
		[CompilerGenerated]
		get
		{
			return _3A_0018;
		}
		[CompilerGenerated]
		private set
		{
			_3A_0018 = text;
		}
	}

	/// <summary />
	public string FullName
	{
		[CompilerGenerated]
		get
		{
			return _3AL;
		}
		[CompilerGenerated]
		private set
		{
			_3AL = text;
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

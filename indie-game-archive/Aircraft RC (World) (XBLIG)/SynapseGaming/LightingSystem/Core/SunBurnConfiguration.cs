using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using _0001;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides access to information and settings contained in the
/// game's SunBurn.config configuration file.
/// </summary>
public class SunBurnConfiguration
{
	/// <summary />
	public class SunBurnPluginConfigurationElement
	{
		[CompilerGenerated]
		private bool HCB;

		[CompilerGenerated]
		private string HC_0002;

		[CompilerGenerated]
		private List<string> HC_0012;

		/// <summary />
		public bool AddedByPluginManager
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
		public string Name
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

		/// <summary />
		public List<string> Assemblies
		{
			[CompilerGenerated]
			get
			{
				return HC_0012;
			}
			[CompilerGenerated]
			private set
			{
				HC_0012 = list;
			}
		}

		/// <summary />
		public SunBurnPluginConfigurationElement()
		{
			Name = string.Empty;
			Assemblies = new List<string>();
		}

		internal static SunBurnPluginConfigurationElement _0002A(_0001._7 P_0)
		{
			SunBurnPluginConfigurationElement sunBurnPluginConfigurationElement = new SunBurnPluginConfigurationElement();
			sunBurnPluginConfigurationElement.Name = _0002_0015(P_0, "name");
			sunBurnPluginConfigurationElement.AddedByPluginManager = _0002_0015(P_0, "addedby").Equals("PluginManager", StringComparison.InvariantCultureIgnoreCase);
			_0001._0002 obj = P_0.SelectNodes("Assemblies/Assembly");
			foreach (_0001.B item in obj)
			{
				sunBurnPluginConfigurationElement.Assemblies.Add(item.InnerText);
			}
			return sunBurnPluginConfigurationElement;
		}
	}

	private static SunBurnConfiguration HCB;

	[CompilerGenerated]
	private string HC_0002;

	[CompilerGenerated]
	private List<SunBurnPluginConfigurationElement> HC_0012;

	/// <summary>
	/// Global access to the game's information and settings contained
	/// in the SunBurn.config configuration file.
	/// </summary>
	public static SunBurnConfiguration Current => HCB;

	/// <summary>
	/// User specified path to the SunBurn editor services, which can be used
	/// to override the default installation path.
	/// </summary>
	public string ServicesPath
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
	/// List of plugins auto-loaded when SunBurn starts up.
	/// </summary>
	public List<SunBurnPluginConfigurationElement> Plugins
	{
		[CompilerGenerated]
		get
		{
			return HC_0012;
		}
		[CompilerGenerated]
		private set
		{
			HC_0012 = list;
		}
	}

	static SunBurnConfiguration()
	{
		string empty = string.Empty;
		string filename = Path.Combine(empty, "SunBurn.config");
		HCB = Load(filename);
	}

	/// <summary>
	/// Creates a new SunBurnConfiguration instance.
	/// </summary>
	public SunBurnConfiguration()
	{
		ServicesPath = string.Empty;
		Plugins = new List<SunBurnPluginConfigurationElement>();
	}

	/// <summary>
	/// Loads a SunBurn configuration file.
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public static SunBurnConfiguration Load(string filename)
	{
		SunBurnConfiguration sunBurnConfiguration = new SunBurnConfiguration();
		Path.GetDirectoryName(filename);
		global::_0001._0001 obj = new global::_0001._0001();
		try
		{
			using Stream stream = TitleContainer.OpenStream(filename);
			obj.Load(stream);
		}
		catch
		{
			return sunBurnConfiguration;
		}
		_0001._0002 obj3 = obj.DocumentElement.SelectNodes("SunBurn/Plugins/Plugin");
		foreach (_0001.B item in obj3)
		{
			if (item is _0001._7 obj4)
			{
				sunBurnConfiguration.Plugins.Add(SunBurnPluginConfigurationElement._0002A(obj4));
			}
		}
		return sunBurnConfiguration;
	}

	private static string _0002_0015(_0001._7 P_0, string P_1)
	{
		_0001._0012 attributeNode = P_0.GetAttributeNode(P_1);
		if (attributeNode == null)
		{
			return string.Empty;
		}
		return attributeNode.InnerText;
	}
}

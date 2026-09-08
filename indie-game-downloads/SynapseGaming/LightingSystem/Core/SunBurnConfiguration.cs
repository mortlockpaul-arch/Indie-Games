using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using D;
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
		private bool _3A_0018;

		[CompilerGenerated]
		private string _3AL;

		[CompilerGenerated]
		private List<string> _3A_0019;

		/// <summary />
		public bool AddedByPluginManager
		{
			[CompilerGenerated]
			get
			{
				return _3A_0018;
			}
			[CompilerGenerated]
			private set
			{
				_3A_0018 = flag;
			}
		}

		/// <summary />
		public string Name
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

		/// <summary />
		public List<string> Assemblies
		{
			[CompilerGenerated]
			get
			{
				return _3A_0019;
			}
			[CompilerGenerated]
			private set
			{
				_3A_0019 = list;
			}
		}

		/// <summary />
		public SunBurnPluginConfigurationElement()
		{
			Name = string.Empty;
			Assemblies = new List<string>();
		}

		internal static SunBurnPluginConfigurationElement Ly(D._6 P_0)
		{
			SunBurnPluginConfigurationElement sunBurnPluginConfigurationElement = new SunBurnPluginConfigurationElement();
			sunBurnPluginConfigurationElement.Name = Ln(P_0, "name");
			sunBurnPluginConfigurationElement.AddedByPluginManager = Ln(P_0, "addedby").Equals("PluginManager", StringComparison.InvariantCultureIgnoreCase);
			D.L l = P_0.SelectNodes("Assemblies/Assembly");
			foreach (D._0018 item in l)
			{
				sunBurnPluginConfigurationElement.Assemblies.Add(item.InnerText);
			}
			return sunBurnPluginConfigurationElement;
		}
	}

	private static SunBurnConfiguration _3A_0018;

	[CompilerGenerated]
	private string _3AL;

	[CompilerGenerated]
	private List<SunBurnPluginConfigurationElement> _3A_0019;

	/// <summary>
	/// Global access to the game's information and settings contained
	/// in the SunBurn.config configuration file.
	/// </summary>
	public static SunBurnConfiguration Current => _3A_0018;

	/// <summary>
	/// User specified path to the SunBurn editor services, which can be used
	/// to override the default installation path.
	/// </summary>
	public string ServicesPath
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
	/// List of plugins auto-loaded when SunBurn starts up.
	/// </summary>
	public List<SunBurnPluginConfigurationElement> Plugins
	{
		[CompilerGenerated]
		get
		{
			return _3A_0019;
		}
		[CompilerGenerated]
		private set
		{
			_3A_0019 = list;
		}
	}

	static SunBurnConfiguration()
	{
		string empty = string.Empty;
		string filename = Path.Combine(empty, "SunBurn.config");
		_3A_0018 = Load(filename);
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
		global::D.D d = new global::D.D();
		try
		{
			using Stream stream = TitleContainer.OpenStream(filename);
			d.Load(stream);
		}
		catch
		{
			return sunBurnConfiguration;
		}
		D.L l = d.DocumentElement.SelectNodes("SunBurn/Plugins/Plugin");
		foreach (D._0018 item in l)
		{
			if (item is D._6 obj2)
			{
				sunBurnConfiguration.Plugins.Add(SunBurnPluginConfigurationElement.Ly(obj2));
			}
		}
		return sunBurnConfiguration;
	}

	private static string Ln(D._6 P_0, string P_1)
	{
		D._0019 attributeNode = P_0.GetAttributeNode(P_1);
		if (attributeNode == null)
		{
			return string.Empty;
		}
		return attributeNode.InnerText;
	}
}

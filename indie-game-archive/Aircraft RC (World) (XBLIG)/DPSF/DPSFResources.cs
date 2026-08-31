using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DPSF;

/// <summary>
///   A strongly-typed resource class, for looking up localized strings, etc.
/// </summary>
[CompilerGenerated]
[DebuggerNonUserCode]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
internal class DPSFResources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	/// <summary>
	///   Returns the cached ResourceManager instance used by this class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(resourceMan, null))
			{
				ResourceManager resourceManager = new ResourceManager("DPSF.DPSFResources", typeof(DPSFResources).Assembly);
				resourceMan = resourceManager;
			}
			return resourceMan;
		}
	}

	/// <summary>
	///   Overrides the current thread's CurrentUICulture property for all
	///   resource lookups using this strongly typed resource class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static byte[] DPSFDefaultEffectWindowsHiDef
	{
		get
		{
			object obj = ResourceManager.GetObject("DPSFDefaultEffectWindowsHiDef", resourceCulture);
			return (byte[])obj;
		}
	}

	internal static byte[] DPSFDefaultEffectWindowsReach
	{
		get
		{
			object obj = ResourceManager.GetObject("DPSFDefaultEffectWindowsReach", resourceCulture);
			return (byte[])obj;
		}
	}

	internal static byte[] DPSFDefaultEffectXbox360HiDef
	{
		get
		{
			object obj = ResourceManager.GetObject("DPSFDefaultEffectXbox360HiDef", resourceCulture);
			return (byte[])obj;
		}
	}

	internal DPSFResources()
	{
	}
}

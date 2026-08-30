using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace EasyStorage;

[DebuggerNonUserCode]
[CompilerGenerated]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
internal class Strings
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(resourceMan, null))
			{
				ResourceManager resourceManager = new ResourceManager("EasyStorage.Strings", typeof(Strings).Assembly);
				resourceMan = resourceManager;
			}
			return resourceMan;
		}
	}

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

	internal static string forceCanceledReselectionMessage => ResourceManager.GetString("forceCanceledReselectionMessage", resourceCulture);

	internal static string forceDisconnectedReselectionMessage => ResourceManager.GetString("forceDisconnectedReselectionMessage", resourceCulture);

	internal static string NeedGamerService => ResourceManager.GetString("NeedGamerService", resourceCulture);

	internal static string No_Continue_without_device => ResourceManager.GetString("No_Continue_without_device", resourceCulture);

	internal static string Ok => ResourceManager.GetString("Ok", resourceCulture);

	internal static string promptForCancelledMessage => ResourceManager.GetString("promptForCancelledMessage", resourceCulture);

	internal static string promptForDisconnectedMessage => ResourceManager.GetString("promptForDisconnectedMessage", resourceCulture);

	internal static string Reselect_Storage_Device => ResourceManager.GetString("Reselect_Storage_Device", resourceCulture);

	internal static string Storage_Device_Required => ResourceManager.GetString("Storage_Device_Required", resourceCulture);

	internal static string StorageDevice_is_not_valid => ResourceManager.GetString("StorageDevice_is_not_valid", resourceCulture);

	internal static string Yes_Select_new_device => ResourceManager.GetString("Yes_Select_new_device", resourceCulture);

	internal Strings()
	{
	}
}

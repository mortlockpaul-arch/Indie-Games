using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Infinity.Messages;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class UIMessage
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(resourceMan, null))
			{
				ResourceManager resourceManager = new ResourceManager("Infinity.Messages.UIMessage", typeof(UIMessage).Assembly);
				resourceMan = resourceManager;
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
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

	public static string Cancel => ResourceManager.GetString("Cancel", resourceCulture);

	public static string Confirm => ResourceManager.GetString("Confirm", resourceCulture);

	public static string No => ResourceManager.GetString("No", resourceCulture);

	public static string StorageCancel => ResourceManager.GetString("StorageCancel", resourceCulture);

	public static string Yes => ResourceManager.GetString("Yes", resourceCulture);

	internal UIMessage()
	{
	}
}

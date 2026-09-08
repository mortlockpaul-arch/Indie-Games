using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Xna.Framework;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[CompilerGenerated]
[DebuggerNonUserCode]
internal class Resources
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
				ResourceManager resourceManager = new ResourceManager("Microsoft.Xna.Framework.Resources", typeof(Resources).Assembly);
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

	internal static string BackBufferDimMustBePositive => ResourceManager.GetString("BackBufferDimMustBePositive", resourceCulture);

	internal static string CannotAddSameComponentMultipleTimes => ResourceManager.GetString("CannotAddSameComponentMultipleTimes", resourceCulture);

	internal static string CannotSetItemsIntoGameComponentCollection => ResourceManager.GetString("CannotSetItemsIntoGameComponentCollection", resourceCulture);

	internal static string DefaultTitleName => ResourceManager.GetString("DefaultTitleName", resourceCulture);

	internal static string Direct3DCreateError => ResourceManager.GetString("Direct3DCreateError", resourceCulture);

	internal static string Direct3DInvalidCreateParameters => ResourceManager.GetString("Direct3DInvalidCreateParameters", resourceCulture);

	internal static string GameCannotBeNull => ResourceManager.GetString("GameCannotBeNull", resourceCulture);

	internal static string GameNotDerivedFromValidGameType => ResourceManager.GetString("GameNotDerivedFromValidGameType", resourceCulture);

	internal static string CannotCreateGameType => ResourceManager.GetString("CannotCreateGameType", resourceCulture);

	internal static string RunNotSupported => ResourceManager.GetString("RunNotSupported", resourceCulture);

	internal static string GraphicsComponentNotAttachedToGame => ResourceManager.GetString("GraphicsComponentNotAttachedToGame", resourceCulture);

	internal static string GraphicsDeviceManagerAlreadyPresent => ResourceManager.GetString("GraphicsDeviceManagerAlreadyPresent", resourceCulture);

	internal static string InactiveSleepTimeCannotBeZero => ResourceManager.GetString("InactiveSleepTimeCannotBeZero", resourceCulture);

	internal static string InvalidScreenAdapter => ResourceManager.GetString("InvalidScreenAdapter", resourceCulture);

	internal static string InvalidScreenDeviceName => ResourceManager.GetString("InvalidScreenDeviceName", resourceCulture);

	internal static string MissingGraphicsDeviceService => ResourceManager.GetString("MissingGraphicsDeviceService", resourceCulture);

	internal static string MustCallBeginDeviceChange => ResourceManager.GetString("MustCallBeginDeviceChange", resourceCulture);

	internal static string NoAudioHardware => ResourceManager.GetString("NoAudioHardware", resourceCulture);

	internal static string NoCompatibleDevices => ResourceManager.GetString("NoCompatibleDevices", resourceCulture);

	internal static string NoCompatibleDevicesAfterRanking => ResourceManager.GetString("NoCompatibleDevicesAfterRanking", resourceCulture);

	internal static string NoGraphicsDeviceService => ResourceManager.GetString("NoGraphicsDeviceService", resourceCulture);

	internal static string NoHighResolutionTimer => ResourceManager.GetString("NoHighResolutionTimer", resourceCulture);

	internal static string NoMultipleRuns => ResourceManager.GetString("NoMultipleRuns", resourceCulture);

	internal static string NoNullUseDefaultAdapter => ResourceManager.GetString("NoNullUseDefaultAdapter", resourceCulture);

	internal static string NoSuitableGraphicsDevice => ResourceManager.GetString("NoSuitableGraphicsDevice", resourceCulture);

	internal static string NullOrEmptyScreenDeviceName => ResourceManager.GetString("NullOrEmptyScreenDeviceName", resourceCulture);

	internal static string PreviousDrawThrew => ResourceManager.GetString("PreviousDrawThrew", resourceCulture);

	internal static string PropertyCannotBeCalledBeforeInitialize => ResourceManager.GetString("PropertyCannotBeCalledBeforeInitialize", resourceCulture);

	internal static string ServiceAlreadyPresent => ResourceManager.GetString("ServiceAlreadyPresent", resourceCulture);

	internal static string ServiceMustBeAssignable => ResourceManager.GetString("ServiceMustBeAssignable", resourceCulture);

	internal static string ServiceProviderCannotBeNull => ResourceManager.GetString("ServiceProviderCannotBeNull", resourceCulture);

	internal static string ServiceTypeCannotBeNull => ResourceManager.GetString("ServiceTypeCannotBeNull", resourceCulture);

	internal static string TargetElaspedCannotBeZero => ResourceManager.GetString("TargetElaspedCannotBeZero", resourceCulture);

	internal static string TitleCannotBeNull => ResourceManager.GetString("TitleCannotBeNull", resourceCulture);

	internal static string ValidateBackBufferDimsFullScreen => ResourceManager.GetString("ValidateBackBufferDimsFullScreen", resourceCulture);

	internal static string ValidateBackBufferDimsModeFullScreen => ResourceManager.GetString("ValidateBackBufferDimsModeFullScreen", resourceCulture);

	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Resources()
	{
	}
}

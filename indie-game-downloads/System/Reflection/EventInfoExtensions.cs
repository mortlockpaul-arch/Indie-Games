using System.ComponentModel;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class EventInfoExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetAddMethod(this EventInfo eventInfo)
	{
		ArgumentNullException.ThrowIfNull(eventInfo, "eventInfo");
		return eventInfo.GetAddMethod();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetAddMethod(this EventInfo eventInfo, bool nonPublic)
	{
		ArgumentNullException.ThrowIfNull(eventInfo, "eventInfo");
		return eventInfo.GetAddMethod(nonPublic);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetRaiseMethod(this EventInfo eventInfo)
	{
		ArgumentNullException.ThrowIfNull(eventInfo, "eventInfo");
		return eventInfo.GetRaiseMethod();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetRaiseMethod(this EventInfo eventInfo, bool nonPublic)
	{
		ArgumentNullException.ThrowIfNull(eventInfo, "eventInfo");
		return eventInfo.GetRaiseMethod(nonPublic);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetRemoveMethod(this EventInfo eventInfo)
	{
		ArgumentNullException.ThrowIfNull(eventInfo, "eventInfo");
		return eventInfo.GetRemoveMethod();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetRemoveMethod(this EventInfo eventInfo, bool nonPublic)
	{
		ArgumentNullException.ThrowIfNull(eventInfo, "eventInfo");
		return eventInfo.GetRemoveMethod(nonPublic);
	}
}

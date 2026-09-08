namespace System.Threading;

internal readonly struct NamedWaitHandleOptionsInternal(NamedWaitHandleOptions options)
{
	private readonly NamedWaitHandleOptions _options = options;

	private readonly bool _wasSpecified = true;

	public bool CurrentUserOnly => _options.CurrentUserOnly;

	public bool CurrentSessionOnly => _options.CurrentSessionOnly;

	public bool WasSpecified => _wasSpecified;

	public string GetNameWithSessionPrefix(string name)
	{
		if (!name.Contains('\\'))
		{
			name = ((!CurrentSessionOnly) ? ("Global\\" + name) : ("Local\\" + name));
			return name;
		}
		if ((!CurrentSessionOnly) ? name.StartsWith("Local\\", StringComparison.Ordinal) : (!name.StartsWith("Local\\", StringComparison.Ordinal)))
		{
			throw new ArgumentException(SR.Format(SR.NamedWaitHandles_IncompatibleNamePrefix, name), "name");
		}
		return name;
	}
}

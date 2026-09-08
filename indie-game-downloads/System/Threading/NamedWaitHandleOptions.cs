namespace System.Threading;

public struct NamedWaitHandleOptions
{
	private bool _notCurrentUserOnly;

	private bool _notCurrentSessionOnly;

	public bool CurrentUserOnly
	{
		get
		{
			return !_notCurrentUserOnly;
		}
		set
		{
			_notCurrentUserOnly = !value;
		}
	}

	public bool CurrentSessionOnly
	{
		get
		{
			return !_notCurrentSessionOnly;
		}
		set
		{
			_notCurrentSessionOnly = !value;
		}
	}
}

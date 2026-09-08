using System.Collections.Generic;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt;

internal struct FunctionFocus : IFocus
{
	private bool _isSet;

	private QilParameter _current;

	private QilParameter _position;

	private QilParameter _last;

	public bool IsFocusSet => _isSet;

	public void StartFocus(IList<QilNode> args, XslFlags flags)
	{
		int num = 0;
		if ((flags & XslFlags.Current) != XslFlags.None)
		{
			_current = (QilParameter)args[num++];
		}
		if ((flags & XslFlags.Position) != XslFlags.None)
		{
			_position = (QilParameter)args[num++];
		}
		if ((flags & XslFlags.Last) != XslFlags.None)
		{
			_last = (QilParameter)args[num++];
		}
		_isSet = true;
	}

	public void StopFocus()
	{
		_isSet = false;
		_current = (_position = (_last = null));
	}

	public QilNode GetCurrent()
	{
		return _current;
	}

	public QilNode GetPosition()
	{
		return _position;
	}

	public QilNode GetLast()
	{
		return _last;
	}
}

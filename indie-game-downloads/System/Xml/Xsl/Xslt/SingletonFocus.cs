using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt;

internal struct SingletonFocus(XPathQilFactory f) : IFocus
{
	private readonly XPathQilFactory _f = f;

	private SingletonFocusType _focusType = SingletonFocusType.None;

	private QilIterator _current = null;

	public void SetFocus(SingletonFocusType focusType)
	{
		_focusType = focusType;
	}

	public void SetFocus(QilIterator current)
	{
		if (current != null)
		{
			_focusType = SingletonFocusType.Iterator;
			_current = current;
		}
		else
		{
			_focusType = SingletonFocusType.None;
			_current = null;
		}
	}

	public QilNode GetCurrent()
	{
		return _focusType switch
		{
			SingletonFocusType.InitialDocumentNode => _f.Root(_f.XmlContext()), 
			SingletonFocusType.InitialContextNode => _f.XmlContext(), 
			_ => _current, 
		};
	}

	public QilNode GetPosition()
	{
		return _f.Double(1.0);
	}

	public QilNode GetLast()
	{
		return _f.Double(1.0);
	}
}

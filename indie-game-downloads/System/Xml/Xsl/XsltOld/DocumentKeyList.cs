using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld;

internal readonly struct DocumentKeyList(XPathNavigator rootNav, Hashtable keyTable)
{
	private readonly XPathNavigator _rootNav = rootNav;

	private readonly Hashtable _keyTable = keyTable;

	public XPathNavigator RootNav => _rootNav;

	public Hashtable KeyTable => _keyTable;
}

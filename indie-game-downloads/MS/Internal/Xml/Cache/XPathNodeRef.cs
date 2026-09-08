namespace MS.Internal.Xml.Cache;

internal readonly struct XPathNodeRef(XPathNode[] page, int idx)
{
	private readonly XPathNode[] _page = page;

	private readonly int _idx = idx;

	public XPathNode[] Page => _page;

	public int Index => _idx;

	public override int GetHashCode()
	{
		return XPathNodeHelper.GetLocation(_page, _idx);
	}
}

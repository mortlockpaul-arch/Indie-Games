namespace System.Xml;

internal struct LineInfo(int lineNo, int linePos)
{
	internal int lineNo = lineNo;

	internal int linePos = linePos;

	public void Set(int lineNo, int linePos)
	{
		this.lineNo = lineNo;
		this.linePos = linePos;
	}
}

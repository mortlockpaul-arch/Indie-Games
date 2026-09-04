namespace System.Xml;

public enum WriteState
{
	Attribute = 3,
	Closed = 5,
	Content = 4,
	Element = 2,
	Error = 6,
	Prolog = 1,
	Start = 0
}

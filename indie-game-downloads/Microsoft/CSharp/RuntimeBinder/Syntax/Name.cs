namespace Microsoft.CSharp.RuntimeBinder.Syntax;

internal sealed class Name
{
	public string Text { get; }

	public Name(string text)
	{
		Text = text;
	}

	public override string ToString()
	{
		return Text;
	}
}

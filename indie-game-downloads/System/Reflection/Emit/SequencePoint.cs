namespace System.Reflection.Emit;

internal sealed class SequencePoint
{
	public int Offset { get; }

	public int StartLine { get; }

	public int EndLine { get; }

	public int StartColumn { get; }

	public int EndColumn { get; }

	public bool IsHidden => StartLine == 16707566;

	public SequencePoint(int offset, int startLine, int startColumn, int endLine, int endColumn)
	{
		Offset = offset;
		StartLine = startLine;
		EndLine = endLine;
		StartColumn = startColumn;
		EndColumn = endColumn;
	}
}

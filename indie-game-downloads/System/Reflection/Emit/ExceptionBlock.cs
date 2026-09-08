namespace System.Reflection.Emit;

internal sealed class ExceptionBlock
{
	public Label TryStart;

	public Label TryEnd;

	public Label HandleStart;

	public Label HandleEnd;

	public Label FilterStart;

	public Label EndLabel;

	public ExceptionState State;
}

namespace System.Text.RegularExpressions.Symbolic;

internal readonly struct DerivativeEffect(DerivativeEffectKind kind, int captureNumber)
{
	public DerivativeEffectKind Kind { get; } = kind;

	public int CaptureNumber { get; } = captureNumber;
}

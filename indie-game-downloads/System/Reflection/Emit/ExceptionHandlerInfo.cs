using System.Reflection.Metadata;

namespace System.Reflection.Emit;

internal struct ExceptionHandlerInfo(ExceptionRegionKind kind, Label tryStart, Label tryEnd, Label handlerStart, Label handlerEnd, Label filterStart = default(Label), Type catchType = null)
{
	public readonly ExceptionRegionKind Kind = kind;

	public readonly Label TryStart = tryStart;

	public readonly Label TryEnd = tryEnd;

	public readonly Label HandlerStart = handlerStart;

	public readonly Label HandlerEnd = handlerEnd;

	public readonly Label FilterStart = filterStart;

	public Type ExceptionType = catchType;
}

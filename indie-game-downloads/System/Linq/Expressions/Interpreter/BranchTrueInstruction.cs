using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter;

internal sealed class BranchTrueInstruction : OffsetInstruction
{
	[CompilerGenerated]
	private Instruction[] _003CCache_003Ek__BackingField;

	public override Instruction[] Cache => _003CCache_003Ek__BackingField ?? (_003CCache_003Ek__BackingField = new Instruction[32]);

	public override string InstructionName => "BranchTrue";

	public override int ConsumedStack => 1;

	public override int Run(InterpretedFrame frame)
	{
		if ((bool)frame.Pop())
		{
			return _offset;
		}
		return 1;
	}
}

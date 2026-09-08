using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter;

internal sealed class CoalescingBranchInstruction : OffsetInstruction
{
	[CompilerGenerated]
	private Instruction[] _003CCache_003Ek__BackingField;

	public override Instruction[] Cache => _003CCache_003Ek__BackingField ?? (_003CCache_003Ek__BackingField = new Instruction[32]);

	public override string InstructionName => "CoalescingBranch";

	public override int ConsumedStack => 1;

	public override int ProducedStack => 1;

	public override int Run(InterpretedFrame frame)
	{
		if (frame.Peek() != null)
		{
			return _offset;
		}
		return 1;
	}
}

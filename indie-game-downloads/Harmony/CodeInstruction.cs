using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace Harmony;

internal class CodeInstruction
{
	public OpCode opcode;

	public object operand;

	public List<Label> labels = new List<Label>();

	public List<ExceptionBlock> blocks = new List<ExceptionBlock>();

	public CodeInstruction(OpCode opcode, object operand = null)
	{
		this.opcode = opcode;
		this.operand = operand;
	}

	public CodeInstruction(CodeInstruction instruction)
	{
		opcode = instruction.opcode;
		operand = instruction.operand;
		labels = instruction.labels.ToArray().ToList();
	}

	public override string ToString()
	{
		List<string> list = new List<string>();
		foreach (Label label in labels)
		{
			list.Add("Label" + label.GetHashCode());
		}
		foreach (ExceptionBlock block in blocks)
		{
			list.Add("EX_" + block.blockType.ToString().Replace("Block", ""));
		}
		string text = ((list.Count > 0) ? (" [" + string.Join(", ", list.ToArray()) + "]") : "");
		string text2 = Emitter.FormatArgument(operand);
		if (text2 != "")
		{
			text2 = " " + text2;
		}
		return string.Format(string.Concat(opcode, text2, text));
	}
}

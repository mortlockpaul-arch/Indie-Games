using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Harmony.ILCopying;

internal class MethodBodyReader
{
	private class ThisParameter : ParameterInfo
	{
		public ThisParameter(MethodBase method)
		{
			MemberImpl = method;
			ClassImpl = method.DeclaringType;
			NameImpl = "this";
			PositionImpl = -1;
		}
	}

	private readonly ILGenerator generator;

	private readonly MethodBase method;

	private readonly Module module;

	private readonly Type[] typeArguments;

	private readonly Type[] methodArguments;

	private readonly ByteBuffer ilBytes;

	private readonly ParameterInfo this_parameter;

	private readonly ParameterInfo[] parameters;

	private readonly IList<LocalVariableInfo> locals;

	private readonly IList<ExceptionHandlingClause> exceptions;

	private List<ILInstruction> ilInstructions;

	private LocalBuilder[] variables;

	private static readonly OpCode[] one_byte_opcodes;

	private static readonly OpCode[] two_bytes_opcodes;

	private static readonly Dictionary<Type, MethodInfo> emitMethods;

	public static List<ILInstruction> GetInstructions(ILGenerator generator, MethodBase method)
	{
		if (method == null)
		{
			throw new ArgumentNullException("Method cannot be null");
		}
		MethodBodyReader methodBodyReader = new MethodBodyReader(method, generator);
		methodBodyReader.DeclareVariables(null);
		methodBodyReader.ReadInstructions();
		return methodBodyReader.ilInstructions;
	}

	public MethodBodyReader(MethodBase method, ILGenerator generator)
	{
		this.generator = generator;
		this.method = method;
		module = method.Module;
		MethodBody methodBody = method.GetMethodBody();
		if (methodBody == null)
		{
			throw new ArgumentException("Method " + method.Name + " has no body");
		}
		byte[] iLAsByteArray = methodBody.GetILAsByteArray();
		if (iLAsByteArray == null)
		{
			throw new ArgumentException("Can not get IL bytes of method " + method.Name);
		}
		ilBytes = new ByteBuffer(iLAsByteArray);
		ilInstructions = new List<ILInstruction>((iLAsByteArray.Length + 1) / 2);
		Type declaringType = method.DeclaringType;
		if (declaringType.IsGenericType)
		{
			try
			{
				typeArguments = declaringType.GetGenericArguments();
			}
			catch
			{
				typeArguments = null;
			}
		}
		if (method.IsGenericMethod)
		{
			try
			{
				methodArguments = method.GetGenericArguments();
			}
			catch
			{
				methodArguments = null;
			}
		}
		if (!method.IsStatic)
		{
			this_parameter = new ThisParameter(method);
		}
		parameters = method.GetParameters();
		locals = methodBody.LocalVariables;
		exceptions = methodBody.ExceptionHandlingClauses;
	}

	public void ReadInstructions()
	{
		while (ilBytes.position < ilBytes.buffer.Length)
		{
			int position = ilBytes.position;
			ILInstruction iLInstruction = new ILInstruction(ReadOpCode())
			{
				offset = position
			};
			ReadOperand(iLInstruction);
			ilInstructions.Add(iLInstruction);
		}
		ResolveBranches();
		ParseExceptions();
	}

	public void DeclareVariables(LocalBuilder[] existingVariables)
	{
		if (generator == null)
		{
			return;
		}
		if (existingVariables != null)
		{
			variables = existingVariables;
			return;
		}
		variables = locals.Select((LocalVariableInfo lvi) => generator.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray();
	}

	private void ResolveBranches()
	{
		foreach (ILInstruction ilInstruction in ilInstructions)
		{
			switch (ilInstruction.opcode.OperandType)
			{
			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				ilInstruction.operand = GetInstruction((int)ilInstruction.operand, isEndOfInstruction: false);
				break;
			case OperandType.InlineSwitch:
			{
				int[] array = (int[])ilInstruction.operand;
				ILInstruction[] array2 = new ILInstruction[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = GetInstruction(array[i], isEndOfInstruction: false);
				}
				ilInstruction.operand = array2;
				break;
			}
			}
		}
	}

	private void ParseExceptions()
	{
		foreach (ExceptionHandlingClause exception in exceptions)
		{
			int tryOffset = exception.TryOffset;
			_ = exception.TryOffset;
			_ = exception.TryLength;
			int handlerOffset = exception.HandlerOffset;
			int offset = exception.HandlerOffset + exception.HandlerLength - 1;
			if (exception.Flags != ExceptionHandlingClauseOptions.Clause)
			{
				exception.Flags.ToString().ToLower();
			}
			GetInstruction(tryOffset, isEndOfInstruction: false).blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, null));
			GetInstruction(offset, isEndOfInstruction: true).blocks.Add(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock, null));
			switch (exception.Flags)
			{
			case ExceptionHandlingClauseOptions.Filter:
				GetInstruction(exception.FilterOffset, isEndOfInstruction: false).blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptFilterBlock, null));
				break;
			case ExceptionHandlingClauseOptions.Finally:
				GetInstruction(handlerOffset, isEndOfInstruction: false).blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock, null));
				break;
			case ExceptionHandlingClauseOptions.Clause:
				GetInstruction(handlerOffset, isEndOfInstruction: false).blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, exception.CatchType));
				break;
			case ExceptionHandlingClauseOptions.Fault:
				GetInstruction(handlerOffset, isEndOfInstruction: false).blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFaultBlock, null));
				break;
			}
		}
	}

	public void FinalizeILCodes(List<Label> endLabels, List<ExceptionBlock> endBlocks)
	{
		if (generator == null)
		{
			return;
		}
		foreach (ILInstruction ilInstruction in ilInstructions)
		{
			switch (ilInstruction.opcode.OperandType)
			{
			case OperandType.InlineSwitch:
				if (ilInstruction.operand is ILInstruction[] array)
				{
					List<Label> list = new List<Label>();
					ILInstruction[] array2 = array;
					foreach (ILInstruction obj in array2)
					{
						Label item = generator.DefineLabel();
						obj.labels.Add(item);
						list.Add(item);
					}
					ilInstruction.argument = list.ToArray();
				}
				break;
			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				if (ilInstruction.operand is ILInstruction iLInstruction)
				{
					Label label = generator.DefineLabel();
					iLInstruction.labels.Add(label);
					ilInstruction.argument = label;
				}
				break;
			}
		}
		CodeInstruction[] array3 = new CodeTranspiler(ilInstructions).GetResult(generator, method).ToArray();
		int num = 0;
		CodeInstruction[] array4 = array3;
		foreach (CodeInstruction codeInstruction in array4)
		{
			foreach (Label label3 in codeInstruction.labels)
			{
				generator.MarkLabel(label3);
			}
			foreach (ExceptionBlock block in codeInstruction.blocks)
			{
				Emitter.MarkBlockBefore(generator, block, out var _);
			}
			OpCode opcode = codeInstruction.opcode;
			object operand = codeInstruction.operand;
			if (true)
			{
				if (opcode.OperandType == OperandType.InlineNone)
				{
					generator.Emit(opcode);
				}
				else
				{
					if (operand == null)
					{
						throw new Exception("Wrong null argument: " + codeInstruction);
					}
					MethodInfo methodInfo = EmitMethodForType(operand.GetType());
					if (methodInfo == null)
					{
						throw new Exception(string.Concat("Unknown Emit argument type ", operand.GetType(), " in ", codeInstruction));
					}
					methodInfo.Invoke(generator, new object[2] { opcode, operand });
				}
			}
			foreach (ExceptionBlock block2 in codeInstruction.blocks)
			{
				Emitter.MarkBlockAfter(generator, block2);
			}
			num++;
		}
	}

	private static void GetMemberInfoValue(MemberInfo info, out object result)
	{
		result = null;
		switch (info.MemberType)
		{
		case MemberTypes.Constructor:
			result = (ConstructorInfo)info;
			break;
		case MemberTypes.Event:
			result = (EventInfo)info;
			break;
		case MemberTypes.Field:
			result = (FieldInfo)info;
			break;
		case MemberTypes.Method:
			result = (MethodInfo)info;
			break;
		case MemberTypes.TypeInfo:
		case MemberTypes.NestedType:
			result = (Type)info;
			break;
		case MemberTypes.Property:
			result = (PropertyInfo)info;
			break;
		}
	}

	private void ReadOperand(ILInstruction instruction)
	{
		switch (instruction.opcode.OperandType)
		{
		case OperandType.InlineNone:
			instruction.argument = null;
			break;
		case OperandType.InlineSwitch:
		{
			int num6 = ilBytes.ReadInt32();
			int num7 = ilBytes.position + 4 * num6;
			int[] array = new int[num6];
			for (int i = 0; i < num6; i++)
			{
				array[i] = ilBytes.ReadInt32() + num7;
			}
			instruction.operand = array;
			break;
		}
		case OperandType.ShortInlineBrTarget:
		{
			sbyte b4 = (sbyte)ilBytes.ReadByte();
			instruction.operand = b4 + ilBytes.position;
			break;
		}
		case OperandType.InlineBrTarget:
		{
			int num8 = ilBytes.ReadInt32();
			instruction.operand = num8 + ilBytes.position;
			break;
		}
		case OperandType.ShortInlineI:
			if (instruction.opcode == OpCodes.Ldc_I4_S)
			{
				sbyte b2 = (sbyte)ilBytes.ReadByte();
				instruction.operand = b2;
				instruction.argument = (sbyte)instruction.operand;
			}
			else
			{
				byte b3 = ilBytes.ReadByte();
				instruction.operand = b3;
				instruction.argument = (byte)instruction.operand;
			}
			break;
		case OperandType.InlineI:
		{
			int num5 = ilBytes.ReadInt32();
			instruction.operand = num5;
			instruction.argument = (int)instruction.operand;
			break;
		}
		case OperandType.ShortInlineR:
		{
			float num3 = ilBytes.ReadSingle();
			instruction.operand = num3;
			instruction.argument = (float)instruction.operand;
			break;
		}
		case OperandType.InlineR:
		{
			double num2 = ilBytes.ReadDouble();
			instruction.operand = num2;
			instruction.argument = (double)instruction.operand;
			break;
		}
		case OperandType.InlineI8:
		{
			long num4 = ilBytes.ReadInt64();
			instruction.operand = num4;
			instruction.argument = (long)instruction.operand;
			break;
		}
		case OperandType.InlineSig:
		{
			int metadataToken = ilBytes.ReadInt32();
			instruction.operand = module.ResolveSignature(metadataToken);
			instruction.argument = (SignatureHelper)instruction.operand;
			break;
		}
		case OperandType.InlineString:
		{
			int metadataToken6 = ilBytes.ReadInt32();
			instruction.operand = module.ResolveString(metadataToken6);
			instruction.argument = (string)instruction.operand;
			break;
		}
		case OperandType.InlineTok:
		{
			int metadataToken5 = ilBytes.ReadInt32();
			instruction.operand = module.ResolveMember(metadataToken5, typeArguments, methodArguments);
			GetMemberInfoValue((MemberInfo)instruction.operand, out instruction.argument);
			break;
		}
		case OperandType.InlineType:
		{
			int metadataToken4 = ilBytes.ReadInt32();
			instruction.operand = module.ResolveType(metadataToken4, typeArguments, methodArguments);
			instruction.argument = (Type)instruction.operand;
			break;
		}
		case OperandType.InlineMethod:
		{
			int metadataToken3 = ilBytes.ReadInt32();
			instruction.operand = module.ResolveMethod(metadataToken3, typeArguments, methodArguments);
			if (instruction.operand is ConstructorInfo)
			{
				instruction.argument = (ConstructorInfo)instruction.operand;
			}
			else
			{
				instruction.argument = (MethodInfo)instruction.operand;
			}
			break;
		}
		case OperandType.InlineField:
		{
			int metadataToken2 = ilBytes.ReadInt32();
			instruction.operand = module.ResolveField(metadataToken2, typeArguments, methodArguments);
			instruction.argument = (FieldInfo)instruction.operand;
			break;
		}
		case OperandType.ShortInlineVar:
		{
			byte b = ilBytes.ReadByte();
			if (TargetsLocalVariable(instruction.opcode))
			{
				LocalVariableInfo localVariable2 = GetLocalVariable(b);
				if (localVariable2 == null)
				{
					instruction.argument = b;
					break;
				}
				instruction.operand = localVariable2;
				instruction.argument = variables[localVariable2.LocalIndex];
			}
			else
			{
				instruction.operand = GetParameter(b);
				instruction.argument = b;
			}
			break;
		}
		case OperandType.InlineVar:
		{
			short num = ilBytes.ReadInt16();
			if (TargetsLocalVariable(instruction.opcode))
			{
				LocalVariableInfo localVariable = GetLocalVariable(num);
				if (localVariable == null)
				{
					instruction.argument = num;
					break;
				}
				instruction.operand = localVariable;
				instruction.argument = variables[localVariable.LocalIndex];
			}
			else
			{
				instruction.operand = GetParameter(num);
				instruction.argument = num;
			}
			break;
		}
		default:
			throw new NotSupportedException();
		}
	}

	private ILInstruction GetInstruction(int offset, bool isEndOfInstruction)
	{
		int num = ilInstructions.Count - 1;
		if (offset < 0 || offset > ilInstructions[num].offset)
		{
			throw new Exception("Instruction offset " + offset + " is outside valid range 0 - " + ilInstructions[num].offset);
		}
		int num2 = 0;
		int num3 = num;
		while (num2 <= num3)
		{
			int num4 = num2 + (num3 - num2) / 2;
			ILInstruction iLInstruction = ilInstructions[num4];
			if (isEndOfInstruction)
			{
				if (offset == iLInstruction.offset + iLInstruction.GetSize() - 1)
				{
					return iLInstruction;
				}
			}
			else if (offset == iLInstruction.offset)
			{
				return iLInstruction;
			}
			if (offset < iLInstruction.offset)
			{
				num3 = num4 - 1;
			}
			else
			{
				num2 = num4 + 1;
			}
		}
		throw new Exception("Cannot find instruction for " + offset.ToString("X4"));
	}

	private static bool TargetsLocalVariable(OpCode opcode)
	{
		return opcode.Name.Contains("loc");
	}

	private LocalVariableInfo GetLocalVariable(int index)
	{
		return locals?[index];
	}

	private ParameterInfo GetParameter(int index)
	{
		if (index == 0)
		{
			return this_parameter;
		}
		return parameters[index - 1];
	}

	private OpCode ReadOpCode()
	{
		byte b = ilBytes.ReadByte();
		if (b == 254)
		{
			return two_bytes_opcodes[ilBytes.ReadByte()];
		}
		return one_byte_opcodes[b];
	}

	private MethodInfo EmitMethodForType(Type type)
	{
		foreach (KeyValuePair<Type, MethodInfo> emitMethod in emitMethods)
		{
			if (emitMethod.Key == type)
			{
				return emitMethod.Value;
			}
		}
		foreach (KeyValuePair<Type, MethodInfo> emitMethod2 in emitMethods)
		{
			if (emitMethod2.Key.IsAssignableFrom(type))
			{
				return emitMethod2.Value;
			}
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	static MethodBodyReader()
	{
		one_byte_opcodes = new OpCode[225];
		two_bytes_opcodes = new OpCode[31];
		FieldInfo[] fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			OpCode opCode = (OpCode)fields[i].GetValue(null);
			if (opCode.OpCodeType != OpCodeType.Nternal)
			{
				if (opCode.Size == 1)
				{
					one_byte_opcodes[opCode.Value] = opCode;
				}
				else
				{
					two_bytes_opcodes[opCode.Value & 0xFF] = opCode;
				}
			}
		}
		emitMethods = new Dictionary<Type, MethodInfo>();
		foreach (MethodInfo item in typeof(ILGenerator).GetMethods().ToList())
		{
			if (item.Name != "Emit")
			{
				continue;
			}
			ParameterInfo[] array = item.GetParameters();
			if (array.Length == 2)
			{
				Type[] array2 = array.Select((ParameterInfo p) => p.ParameterType).ToArray();
				if (array2[0] == typeof(OpCode))
				{
					emitMethods[array2[1]] = item;
				}
			}
		}
	}
}

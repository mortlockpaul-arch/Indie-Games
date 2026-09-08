using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

internal sealed class ILGeneratorImpl : ILGenerator
{
	private readonly MethodBuilderImpl _methodBuilder;

	private readonly ModuleBuilderImpl _moduleBuilder;

	private readonly BlobBuilder _builder;

	private readonly InstructionEncoder _il;

	private readonly ControlFlowBuilder _cfBuilder;

	private readonly Scope _scope;

	private Scope _currentScope;

	private bool _hasDynamicStackAllocation;

	private int _maxStackDepth;

	private int _currentStackDepth;

	private int _targetDepth;

	private int _depthAdjustment;

	private int _localCount;

	private Dictionary<Label, LabelInfo> _labelTable = new Dictionary<Label, LabelInfo>(2);

	private List<KeyValuePair<object, BlobWriter>> _memberReferences = new List<KeyValuePair<object, BlobWriter>>();

	private List<ExceptionBlock> _exceptionStack = new List<ExceptionBlock>();

	private List<ExceptionHandlerInfo> _exceptionBlocks = new List<ExceptionHandlerInfo>();

	private Dictionary<SymbolDocumentWriter, List<SequencePoint>> _documentToSequencePoints = new Dictionary<SymbolDocumentWriter, List<SequencePoint>>();

	internal InstructionEncoder Instructions => _il;

	internal bool HasDynamicStackAllocation => _hasDynamicStackAllocation;

	internal List<LocalBuilder> Locals => _scope.GetAllLocals();

	internal int LocalCount => _localCount;

	internal Scope Scope => _scope;

	internal Dictionary<SymbolDocumentWriter, List<SequencePoint>> DocumentToSequencePoints => _documentToSequencePoints;

	public override int ILOffset => _il.Offset;

	internal ILGeneratorImpl(MethodBuilderImpl methodBuilder, int size)
	{
		_methodBuilder = methodBuilder;
		_moduleBuilder = (ModuleBuilderImpl)methodBuilder.Module;
		_builder = new BlobBuilder(Math.Max(size, 16));
		_cfBuilder = new ControlFlowBuilder();
		_il = new InstructionEncoder(_builder, _cfBuilder);
		_scope = new Scope(_il.Offset, null);
		_currentScope = _scope;
	}

	internal int GetMaxStack()
	{
		return Math.Min(65535, _maxStackDepth + _depthAdjustment);
	}

	internal List<KeyValuePair<object, BlobWriter>> GetMemberReferences()
	{
		return _memberReferences;
	}

	internal void AddExceptionBlocks()
	{
		foreach (ExceptionHandlerInfo exceptionBlock in _exceptionBlocks)
		{
			switch (exceptionBlock.Kind)
			{
			case ExceptionRegionKind.Catch:
				_cfBuilder.AddCatchRegion(GetMetaLabel(exceptionBlock.TryStart), GetMetaLabel(exceptionBlock.TryEnd), GetMetaLabel(exceptionBlock.HandlerStart), GetMetaLabel(exceptionBlock.HandlerEnd), _moduleBuilder.GetTypeHandle(exceptionBlock.ExceptionType));
				break;
			case ExceptionRegionKind.Filter:
				_cfBuilder.AddFilterRegion(GetMetaLabel(exceptionBlock.TryStart), GetMetaLabel(exceptionBlock.TryEnd), GetMetaLabel(exceptionBlock.HandlerStart), GetMetaLabel(exceptionBlock.HandlerEnd), GetMetaLabel(exceptionBlock.FilterStart));
				break;
			case ExceptionRegionKind.Fault:
				_cfBuilder.AddFaultRegion(GetMetaLabel(exceptionBlock.TryStart), GetMetaLabel(exceptionBlock.TryEnd), GetMetaLabel(exceptionBlock.HandlerStart), GetMetaLabel(exceptionBlock.HandlerEnd));
				break;
			case ExceptionRegionKind.Finally:
				_cfBuilder.AddFinallyRegion(GetMetaLabel(exceptionBlock.TryStart), GetMetaLabel(exceptionBlock.TryEnd), GetMetaLabel(exceptionBlock.HandlerStart), GetMetaLabel(exceptionBlock.HandlerEnd));
				break;
			}
		}
	}

	public override void BeginCatchBlock(Type exceptionType)
	{
		if (_exceptionStack.Count < 1)
		{
			throw new NotSupportedException(System.SR.Argument_NotInExceptionBlock);
		}
		ExceptionBlock exceptionBlock = _exceptionStack[_exceptionStack.Count - 1];
		if (exceptionBlock.State == ExceptionState.Filter)
		{
			if (exceptionType != null)
			{
				throw new ArgumentException(System.SR.Argument_ShouldNotSpecifyExceptionType);
			}
			Emit(OpCodes.Endfilter);
			MarkLabel(exceptionBlock.HandleStart);
		}
		else
		{
			ArgumentNullException.ThrowIfNull(exceptionType, "exceptionType");
			Emit(OpCodes.Leave, exceptionBlock.EndLabel);
			if (exceptionBlock.State == ExceptionState.Try)
			{
				MarkLabel(exceptionBlock.TryEnd);
			}
			else if (exceptionBlock.State == ExceptionState.Catch)
			{
				MarkLabel(exceptionBlock.HandleEnd);
			}
			exceptionBlock.HandleStart = DefineLabel();
			exceptionBlock.HandleEnd = DefineLabel();
			_exceptionBlocks.Add(new ExceptionHandlerInfo(ExceptionRegionKind.Catch, exceptionBlock.TryStart, exceptionBlock.TryEnd, exceptionBlock.HandleStart, exceptionBlock.HandleEnd, default(Label), exceptionType));
			MarkLabel(exceptionBlock.HandleStart);
		}
		_currentStackDepth = 1;
		exceptionBlock.State = ExceptionState.Catch;
	}

	private LabelHandle GetMetaLabel(Label label)
	{
		return _labelTable[label]._metaLabel;
	}

	public override void BeginExceptFilterBlock()
	{
		if (_exceptionStack.Count < 1)
		{
			throw new NotSupportedException(System.SR.Argument_NotInExceptionBlock);
		}
		ExceptionBlock exceptionBlock = _exceptionStack[_exceptionStack.Count - 1];
		Emit(OpCodes.Leave, exceptionBlock.EndLabel);
		if (exceptionBlock.State == ExceptionState.Try)
		{
			MarkLabel(exceptionBlock.TryEnd);
		}
		else if (exceptionBlock.State == ExceptionState.Catch)
		{
			MarkLabel(exceptionBlock.HandleEnd);
		}
		exceptionBlock.FilterStart = DefineLabel();
		exceptionBlock.HandleStart = DefineLabel();
		exceptionBlock.HandleEnd = DefineLabel();
		exceptionBlock.State = ExceptionState.Filter;
		_exceptionBlocks.Add(new ExceptionHandlerInfo(ExceptionRegionKind.Filter, exceptionBlock.TryStart, exceptionBlock.TryEnd, exceptionBlock.HandleStart, exceptionBlock.HandleEnd, exceptionBlock.FilterStart));
		MarkLabel(exceptionBlock.FilterStart);
		_currentStackDepth = 1;
	}

	public override Label BeginExceptionBlock()
	{
		ExceptionBlock exceptionBlock = new ExceptionBlock();
		exceptionBlock.TryStart = DefineLabel();
		exceptionBlock.TryEnd = DefineLabel();
		exceptionBlock.EndLabel = DefineLabel();
		MarkLabel(exceptionBlock.TryStart);
		exceptionBlock.State = ExceptionState.Try;
		_exceptionStack.Add(exceptionBlock);
		_currentStackDepth = 0;
		return exceptionBlock.EndLabel;
	}

	public override void BeginFaultBlock()
	{
		if (_exceptionStack.Count < 1)
		{
			throw new NotSupportedException(System.SR.Argument_NotInExceptionBlock);
		}
		ExceptionBlock exceptionBlock = _exceptionStack[_exceptionStack.Count - 1];
		Emit(OpCodes.Leave, exceptionBlock.EndLabel);
		if (exceptionBlock.State == ExceptionState.Try)
		{
			MarkLabel(exceptionBlock.TryEnd);
		}
		else if (exceptionBlock.State == ExceptionState.Catch)
		{
			MarkLabel(exceptionBlock.HandleEnd);
		}
		exceptionBlock.HandleStart = DefineLabel();
		exceptionBlock.HandleEnd = DefineLabel();
		_exceptionBlocks.Add(new ExceptionHandlerInfo(ExceptionRegionKind.Fault, exceptionBlock.TryStart, exceptionBlock.TryEnd, exceptionBlock.HandleStart, exceptionBlock.HandleEnd));
		exceptionBlock.State = ExceptionState.Fault;
		MarkLabel(exceptionBlock.HandleStart);
		_currentStackDepth = 0;
	}

	public override void BeginFinallyBlock()
	{
		if (_exceptionStack.Count < 1)
		{
			throw new NotSupportedException(System.SR.Argument_NotInExceptionBlock);
		}
		ExceptionBlock exceptionBlock = _exceptionStack[_exceptionStack.Count - 1];
		Label label = DefineLabel();
		if (exceptionBlock.State == ExceptionState.Try)
		{
			Emit(OpCodes.Leave, label);
		}
		else if (exceptionBlock.State == ExceptionState.Catch)
		{
			Emit(OpCodes.Leave, exceptionBlock.EndLabel);
			MarkLabel(exceptionBlock.HandleEnd);
			exceptionBlock.TryEnd = DefineLabel();
		}
		MarkLabel(exceptionBlock.TryEnd);
		exceptionBlock.HandleStart = DefineLabel();
		exceptionBlock.HandleEnd = label;
		_exceptionBlocks.Add(new ExceptionHandlerInfo(ExceptionRegionKind.Finally, exceptionBlock.TryStart, exceptionBlock.TryEnd, exceptionBlock.HandleStart, exceptionBlock.HandleEnd));
		exceptionBlock.State = ExceptionState.Finally;
		MarkLabel(exceptionBlock.HandleStart);
		_currentStackDepth = 0;
	}

	public override void BeginScope()
	{
		Scope currentScope = _currentScope;
		if (currentScope._children == null)
		{
			currentScope._children = new List<Scope>();
		}
		Scope scope = new Scope(_il.Offset, _currentScope);
		_currentScope._children.Add(scope);
		_currentScope = scope;
	}

	public override LocalBuilder DeclareLocal(Type localType, bool pinned)
	{
		ArgumentNullException.ThrowIfNull(localType, "localType");
		Scope currentScope = _currentScope;
		if (currentScope._locals == null)
		{
			currentScope._locals = new List<LocalBuilder>();
		}
		LocalBuilder localBuilder = new LocalBuilderImpl(_localCount++, localType, _methodBuilder, pinned);
		_currentScope._locals.Add(localBuilder);
		return localBuilder;
	}

	public override Label DefineLabel()
	{
		LabelHandle metaLabel = _il.DefineLabel();
		Label label = ILGenerator.CreateLabel(metaLabel.Id);
		_labelTable.Add(label, new LabelInfo(metaLabel));
		return label;
	}

	private void UpdateStackSize(OpCode opCode)
	{
		UpdateStackSize(opCode.EvaluationStackDelta);
		if (UnconditionalJump(opCode))
		{
			_currentStackDepth = 0;
		}
	}

	private static bool UnconditionalJump(OpCode opCode)
	{
		if (opCode.FlowControl != FlowControl.Throw && opCode.FlowControl != FlowControl.Return)
		{
			return opCode == OpCodes.Jmp;
		}
		return true;
	}

	private void UpdateStackSize(int stackChange)
	{
		_currentStackDepth += stackChange;
		_maxStackDepth = Math.Max(_maxStackDepth, _currentStackDepth);
		_targetDepth = _currentStackDepth;
	}

	public void EmitOpcode(OpCode opcode)
	{
		if (opcode == OpCodes.Localloc)
		{
			_hasDynamicStackAllocation = true;
		}
		_il.OpCode((ILOpCode)opcode.Value);
		UpdateStackSize(opcode);
	}

	public override void Emit(OpCode opcode)
	{
		EmitOpcode(opcode);
	}

	public override void Emit(OpCode opcode, byte arg)
	{
		EmitOpcode(opcode);
		_builder.WriteByte(arg);
	}

	public override void Emit(OpCode opcode, double arg)
	{
		EmitOpcode(opcode);
		_builder.WriteDouble(arg);
	}

	public override void Emit(OpCode opcode, float arg)
	{
		EmitOpcode(opcode);
		_builder.WriteSingle(arg);
	}

	public override void Emit(OpCode opcode, short arg)
	{
		EmitOpcode(opcode);
		_builder.WriteInt16(arg);
	}

	public override void Emit(OpCode opcode, int arg)
	{
		if (opcode.Equals(OpCodes.Ldc_I4))
		{
			OpCode opcode2;
			switch (arg)
			{
			case -1:
				opcode2 = OpCodes.Ldc_I4_M1;
				goto IL_009b;
			case 0:
				opcode2 = OpCodes.Ldc_I4_0;
				goto IL_009b;
			case 1:
				opcode2 = OpCodes.Ldc_I4_1;
				goto IL_009b;
			case 2:
				opcode2 = OpCodes.Ldc_I4_2;
				goto IL_009b;
			case 3:
				opcode2 = OpCodes.Ldc_I4_3;
				goto IL_009b;
			case 4:
				opcode2 = OpCodes.Ldc_I4_4;
				goto IL_009b;
			case 5:
				opcode2 = OpCodes.Ldc_I4_5;
				goto IL_009b;
			case 6:
				opcode2 = OpCodes.Ldc_I4_6;
				goto IL_009b;
			case 7:
				opcode2 = OpCodes.Ldc_I4_7;
				goto IL_009b;
			case 8:
				{
					opcode2 = OpCodes.Ldc_I4_8;
					goto IL_009b;
				}
				IL_009b:
				EmitOpcode(opcode2);
				return;
			}
			if (arg >= -128 && arg <= 127)
			{
				Emit(OpCodes.Ldc_I4_S, (sbyte)arg);
				return;
			}
		}
		else if (opcode.Equals(OpCodes.Ldarg))
		{
			if ((uint)arg <= 3u)
			{
				EmitOpcode(arg switch
				{
					0 => OpCodes.Ldarg_0, 
					1 => OpCodes.Ldarg_1, 
					2 => OpCodes.Ldarg_2, 
					_ => OpCodes.Ldarg_3, 
				});
				return;
			}
			if ((uint)arg <= 255u)
			{
				Emit(OpCodes.Ldarg_S, (byte)arg);
				return;
			}
			if ((uint)arg <= 65535u)
			{
				Emit(OpCodes.Ldarg, (short)arg);
				return;
			}
		}
		else if (opcode.Equals(OpCodes.Ldarga))
		{
			if ((uint)arg <= 255u)
			{
				Emit(OpCodes.Ldarga_S, (byte)arg);
				return;
			}
			if ((uint)arg <= 65535u)
			{
				Emit(OpCodes.Ldarga, (short)arg);
				return;
			}
		}
		else if (opcode.Equals(OpCodes.Starg))
		{
			if ((uint)arg <= 255u)
			{
				Emit(OpCodes.Starg_S, (byte)arg);
				return;
			}
			if ((uint)arg <= 65535u)
			{
				Emit(OpCodes.Starg, (short)arg);
				return;
			}
		}
		EmitOpcode(opcode);
		_builder.WriteInt32(arg);
	}

	public override void Emit(OpCode opcode, long arg)
	{
		EmitOpcode(opcode);
		_il.CodeBuilder.WriteInt64(arg);
	}

	public override void Emit(OpCode opcode, string str)
	{
		EmitOpcode(opcode);
		_il.Token(_moduleBuilder.GetStringMetadataToken(str));
	}

	public override void Emit(OpCode opcode, ConstructorInfo con)
	{
		ArgumentNullException.ThrowIfNull(con, "con");
		if (!opcode.Equals(OpCodes.Call) && !opcode.Equals(OpCodes.Callvirt) && !opcode.Equals(OpCodes.Newobj))
		{
			throw new ArgumentException(System.SR.Argument_NotMethodCallOpcode, "opcode");
		}
		int num = 0;
		if (opcode.StackBehaviourPush == StackBehaviour.Varpush)
		{
			num++;
		}
		if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
		{
			num = ((!(con is ConstructorBuilderImpl constructorBuilderImpl)) ? (num - con.GetParameters().Length) : (num - constructorBuilderImpl._methodBuilder.ParameterCount));
		}
		EmitOpcode(opcode);
		UpdateStackSize(num);
		WriteOrReserveToken(_moduleBuilder.TryGetConstructorHandle(con), con);
	}

	private void WriteOrReserveToken(EntityHandle handle, object member)
	{
		if (handle.IsNil)
		{
			_memberReferences.Add(new KeyValuePair<object, BlobWriter>(member, new BlobWriter(_il.CodeBuilder.ReserveBytes(4))));
		}
		else
		{
			_il.Token(MetadataTokens.GetToken(handle));
		}
	}

	private void AdjustDepth(OpCode opcode, LabelInfo label)
	{
		int startDepth = label._startDepth;
		int targetDepth = _targetDepth;
		if (startDepth < targetDepth)
		{
			if (startDepth >= 0)
			{
				_depthAdjustment += targetDepth - startDepth;
			}
			label._startDepth = targetDepth;
		}
		if (UnconditionalBranching(opcode))
		{
			_currentStackDepth = 0;
		}
	}

	private static bool UnconditionalBranching(OpCode opcode)
	{
		return opcode.FlowControl == FlowControl.Branch;
	}

	public override void Emit(OpCode opcode, Label label)
	{
		if (_labelTable.TryGetValue(label, out var value))
		{
			_il.Branch((ILOpCode)opcode.Value, value._metaLabel);
			UpdateStackSize(opcode);
			AdjustDepth(opcode, value);
			return;
		}
		throw new ArgumentException(System.SR.Argument_InvalidLabel);
	}

	public override void Emit(OpCode opcode, Label[] labels)
	{
		ArgumentNullException.ThrowIfNull(labels, "labels");
		if (!opcode.Equals(OpCodes.Switch))
		{
			throw new ArgumentException(System.SR.Argument_MustBeSwitchOpCode, "opcode");
		}
		SwitchInstructionEncoder switchInstructionEncoder = _il.Switch(labels.Length);
		UpdateStackSize(opcode);
		foreach (Label key in labels)
		{
			LabelInfo labelInfo = _labelTable[key];
			switchInstructionEncoder.Branch(labelInfo._metaLabel);
			AdjustDepth(opcode, labelInfo);
		}
	}

	public override void Emit(OpCode opcode, LocalBuilder local)
	{
		ArgumentNullException.ThrowIfNull(local, "local");
		if (!(local is LocalBuilderImpl localBuilderImpl) || localBuilderImpl.GetMethodBuilder() != _methodBuilder)
		{
			throw new ArgumentException(System.SR.Argument_UnmatchedMethodForLocal, "local");
		}
		int localIndex = local.LocalIndex;
		string name = opcode.Name;
		if (name.StartsWith("ldloca"))
		{
			_il.LoadLocalAddress(localIndex);
		}
		else if (name.StartsWith("ldloc"))
		{
			_il.LoadLocal(localIndex);
		}
		else if (name.StartsWith("stloc"))
		{
			_il.StoreLocal(localIndex);
		}
		UpdateStackSize(opcode);
	}

	public override void Emit(OpCode opcode, SignatureHelper signature)
	{
		ArgumentNullException.ThrowIfNull(signature, "signature");
		EmitOpcode(opcode);
		if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
		{
			int num = -(int)typeof(SignatureHelper).GetProperty("ArgumentCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(signature);
			num--;
			UpdateStackSize(num);
		}
		_il.Token(_moduleBuilder.GetSignatureMetadataToken(signature));
	}

	public override void Emit(OpCode opcode, FieldInfo field)
	{
		ArgumentNullException.ThrowIfNull(field, "field");
		EmitOpcode(opcode);
		WriteOrReserveToken(_moduleBuilder.TryGetFieldHandle(field), field);
	}

	public override void Emit(OpCode opcode, MethodInfo meth)
	{
		ArgumentNullException.ThrowIfNull(meth, "meth");
		if (opcode.Equals(OpCodes.Call) || opcode.Equals(OpCodes.Callvirt) || opcode.Equals(OpCodes.Newobj))
		{
			EmitCall(opcode, meth, null);
			return;
		}
		EmitOpcode(opcode);
		WriteOrReserveToken(_moduleBuilder.TryGetMethodHandle(meth), meth);
	}

	public override void Emit(OpCode opcode, Type cls)
	{
		ArgumentNullException.ThrowIfNull(cls, "cls");
		EmitOpcode(opcode);
		WriteOrReserveToken(_moduleBuilder.TryGetTypeHandle(cls), cls);
	}

	public override void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
	{
		ArgumentNullException.ThrowIfNull(methodInfo, "methodInfo");
		if (!opcode.Equals(OpCodes.Call) && !opcode.Equals(OpCodes.Callvirt) && !opcode.Equals(OpCodes.Newobj))
		{
			throw new ArgumentException(System.SR.Argument_NotMethodCallOpcode, "opcode");
		}
		EmitOpcode(opcode);
		UpdateStackSize(GetStackChange(opcode, methodInfo, _moduleBuilder.GetTypeFromCoreAssembly(CoreTypeId.Void), optionalParameterTypes));
		if (optionalParameterTypes == null || optionalParameterTypes.Length == 0)
		{
			WriteOrReserveToken(_moduleBuilder.TryGetMethodHandle(methodInfo), methodInfo);
		}
		else
		{
			WriteOrReserveToken(_moduleBuilder.TryGetMethodHandle(methodInfo, optionalParameterTypes), new KeyValuePair<MethodInfo, Type[]>(methodInfo, optionalParameterTypes));
		}
	}

	private static int GetStackChange(OpCode opcode, MethodInfo methodInfo, Type voidType, Type[] optionalParameterTypes)
	{
		int num = 0;
		if (methodInfo.ReturnType != voidType)
		{
			num++;
		}
		num = ((!(methodInfo is MethodBuilderImpl methodBuilderImpl)) ? ((!(methodInfo is ArrayMethod arrayMethod)) ? (num - methodInfo.GetParameters().Length) : (num - arrayMethod.ParameterTypes.Length)) : (num - methodBuilderImpl.ParameterCount));
		if (!methodInfo.IsStatic && !opcode.Equals(OpCodes.Newobj))
		{
			num--;
		}
		if (optionalParameterTypes != null)
		{
			num -= optionalParameterTypes.Length;
		}
		return num;
	}

	public override void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
	{
		if (optionalParameterTypes != null && optionalParameterTypes.Length != 0 && (callingConvention & CallingConventions.VarArgs) == 0)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_NotAVarArgCallingConvention);
		}
		int num = GetStackChange(returnType, _moduleBuilder.GetTypeFromCoreAssembly(CoreTypeId.Void), parameterTypes);
		if (optionalParameterTypes != null)
		{
			num -= optionalParameterTypes.Length;
		}
		if ((callingConvention & CallingConventions.HasThis) == CallingConventions.HasThis && (callingConvention & CallingConventions.ExplicitThis) == 0)
		{
			num--;
		}
		UpdateStackSize(num);
		EmitOpcode(OpCodes.Calli);
		_il.Token(_moduleBuilder.GetSignatureToken(callingConvention, returnType, parameterTypes, optionalParameterTypes));
	}

	public override void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
	{
		int stackChange = GetStackChange(returnType, _moduleBuilder.GetTypeFromCoreAssembly(CoreTypeId.Void), parameterTypes);
		UpdateStackSize(stackChange);
		Emit(OpCodes.Calli);
		_il.Token(_moduleBuilder.GetSignatureToken(unmanagedCallConv, returnType, parameterTypes));
	}

	private static int GetStackChange(Type returnType, Type voidType, Type[] parameterTypes)
	{
		int num = 0;
		if (returnType != voidType)
		{
			num++;
		}
		if (parameterTypes != null)
		{
			num -= parameterTypes.Length;
		}
		return num - 1;
	}

	public override void EndExceptionBlock()
	{
		if (_exceptionStack.Count < 1)
		{
			throw new NotSupportedException(System.SR.Argument_NotInExceptionBlock);
		}
		ExceptionBlock exceptionBlock = _exceptionStack[_exceptionStack.Count - 1];
		ExceptionState state = exceptionBlock.State;
		Label endLabel = exceptionBlock.EndLabel;
		switch (state)
		{
		case ExceptionState.Try:
		case ExceptionState.Filter:
			throw new InvalidOperationException(System.SR.Argument_BadExceptionCodeGen);
		case ExceptionState.Catch:
			Emit(OpCodes.Leave, endLabel);
			MarkLabel(exceptionBlock.HandleEnd);
			break;
		case ExceptionState.Finally:
		case ExceptionState.Fault:
			Emit(OpCodes.Endfinally);
			MarkLabel(exceptionBlock.HandleEnd);
			break;
		}
		MarkLabel(endLabel);
		exceptionBlock.State = ExceptionState.Done;
		_exceptionStack.Remove(exceptionBlock);
	}

	public override void EndScope()
	{
		if (_currentScope._parent == null)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_UnmatchingSymScope);
		}
		_currentScope._endOffset = _il.Offset;
		_currentScope = _currentScope._parent;
	}

	public override void MarkLabel(Label loc)
	{
		if (_labelTable.TryGetValue(loc, out var value))
		{
			if (value._position != -1)
			{
				throw new ArgumentException(System.SR.Argument_RedefinedLabel);
			}
			_il.MarkLabel(value._metaLabel);
			value._position = _il.Offset;
			int startDepth = value._startDepth;
			if (startDepth < 0)
			{
				value._startDepth = _currentStackDepth;
			}
			else if (startDepth < _currentStackDepth)
			{
				_depthAdjustment += _currentStackDepth - startDepth;
				value._startDepth = _currentStackDepth;
			}
			else if (startDepth > _currentStackDepth)
			{
				_currentStackDepth = startDepth;
			}
			return;
		}
		throw new ArgumentException(System.SR.Argument_InvalidLabel);
	}

	protected override void MarkSequencePointCore(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
	{
		if (document is SymbolDocumentWriter key)
		{
			if (_documentToSequencePoints.TryGetValue(key, out var value))
			{
				value.Add(new SequencePoint(_il.Offset, startLine, startColumn, endLine, endColumn));
				return;
			}
			value = new List<SequencePoint>
			{
				new SequencePoint(_il.Offset, startLine, startColumn, endLine, endColumn)
			};
			_documentToSequencePoints.Add(key, value);
			return;
		}
		throw new ArgumentException(System.SR.InvalidOperation_InvalidDocument, "document");
	}

	public override void UsingNamespace(string usingNamespace)
	{
		ArgumentException.ThrowIfNullOrEmpty(usingNamespace, "usingNamespace");
		Scope currentScope = _currentScope;
		if (currentScope._importNamespaces == null)
		{
			currentScope._importNamespaces = new List<string>();
		}
		_currentScope._importNamespaces.Add(usingNamespace);
	}
}

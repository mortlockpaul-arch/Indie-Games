using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

public abstract class ILGenerator
{
	private static readonly Type[] s_parameterTypes = new Type[1] { typeof(string) };

	public abstract int ILOffset { get; }

	protected static Label CreateLabel(int id)
	{
		return new Label(id);
	}

	public abstract void Emit(OpCode opcode);

	public abstract void Emit(OpCode opcode, byte arg);

	public abstract void Emit(OpCode opcode, short arg);

	public abstract void Emit(OpCode opcode, long arg);

	public abstract void Emit(OpCode opcode, float arg);

	public abstract void Emit(OpCode opcode, double arg);

	public abstract void Emit(OpCode opcode, int arg);

	public abstract void Emit(OpCode opcode, MethodInfo meth);

	public abstract void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes);

	public abstract void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes);

	public abstract void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[]? optionalParameterTypes);

	public abstract void Emit(OpCode opcode, SignatureHelper signature);

	public abstract void Emit(OpCode opcode, ConstructorInfo con);

	public abstract void Emit(OpCode opcode, Type cls);

	public abstract void Emit(OpCode opcode, Label label);

	public abstract void Emit(OpCode opcode, Label[] labels);

	public abstract void Emit(OpCode opcode, FieldInfo field);

	public abstract void Emit(OpCode opcode, string str);

	public abstract void Emit(OpCode opcode, LocalBuilder local);

	public abstract Label BeginExceptionBlock();

	public abstract void EndExceptionBlock();

	public abstract void BeginExceptFilterBlock();

	public abstract void BeginCatchBlock(Type? exceptionType);

	public abstract void BeginFaultBlock();

	public abstract void BeginFinallyBlock();

	public abstract Label DefineLabel();

	public abstract void MarkLabel(Label loc);

	public virtual void ThrowException([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type excType)
	{
		ArgumentNullException.ThrowIfNull(excType, "excType");
		if (!excType.IsSubclassOf(typeof(Exception)) && excType != typeof(Exception))
		{
			throw new ArgumentException(SR.Argument_NotExceptionType, "excType");
		}
		ConstructorInfo con = excType.GetConstructor(Type.EmptyTypes) ?? throw new ArgumentException(SR.Arg_NoDefCTorWithoutTypeName, "excType");
		Emit(OpCodes.Newobj, con);
		Emit(OpCodes.Throw);
	}

	public virtual void EmitWriteLine(string value)
	{
		Emit(OpCodes.Ldstr, value);
		MethodInfo method = Type.GetType("System.Console, System.Console", throwOnError: true).GetMethod("WriteLine", s_parameterTypes);
		Emit(OpCodes.Call, method);
	}

	public virtual void EmitWriteLine(LocalBuilder localBuilder)
	{
		Emit(OpCodes.Call, Type.GetType("System.Console, System.Console", throwOnError: true).GetMethod("get_Out"));
		Emit(OpCodes.Ldloc, localBuilder);
		Type localType = localBuilder.LocalType;
		if ((localType is TypeBuilder || localType is EnumBuilder) ? true : false)
		{
			throw new ArgumentException(SR.NotSupported_OutputStreamUsingTypeBuilder);
		}
		MethodInfo meth = typeof(TextWriter).GetMethod("WriteLine", new Type[1] { localType }) ?? throw new ArgumentException(SR.Argument_EmitWriteLineType, "localBuilder");
		Emit(OpCodes.Callvirt, meth);
	}

	public virtual void EmitWriteLine(FieldInfo fld)
	{
		ArgumentNullException.ThrowIfNull(fld, "fld");
		Emit(OpCodes.Call, Type.GetType("System.Console, System.Console", throwOnError: true).GetMethod("get_Out"));
		if ((fld.Attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
		{
			Emit(OpCodes.Ldsfld, fld);
		}
		else
		{
			Emit(OpCodes.Ldarg_0);
			Emit(OpCodes.Ldfld, fld);
		}
		Type fieldType = fld.FieldType;
		if ((fieldType is TypeBuilder || fieldType is EnumBuilder) ? true : false)
		{
			throw new NotSupportedException(SR.NotSupported_OutputStreamUsingTypeBuilder);
		}
		MethodInfo meth = typeof(TextWriter).GetMethod("WriteLine", new Type[1] { fieldType }) ?? throw new ArgumentException(SR.Argument_EmitWriteLineType, "fld");
		Emit(OpCodes.Callvirt, meth);
	}

	public virtual LocalBuilder DeclareLocal(Type localType)
	{
		return DeclareLocal(localType, pinned: false);
	}

	public abstract LocalBuilder DeclareLocal(Type localType, bool pinned);

	public abstract void UsingNamespace(string usingNamespace);

	public abstract void BeginScope();

	public abstract void EndScope();

	[CLSCompliant(false)]
	public void Emit(OpCode opcode, sbyte arg)
	{
		Emit(opcode, (byte)arg);
	}

	public void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
	{
		ArgumentNullException.ThrowIfNull(document, "document");
		if (startLine < 0 || startLine >= 536870912)
		{
			throw new ArgumentOutOfRangeException("startLine");
		}
		if (endLine < 0 || endLine >= 536870912 || startLine > endLine)
		{
			throw new ArgumentOutOfRangeException("endLine");
		}
		if (startColumn < 0 || startColumn >= 65536)
		{
			throw new ArgumentOutOfRangeException("startColumn");
		}
		if (endColumn < 0 || endColumn >= 65536 || (startLine == endLine && startLine != 16707566 && startColumn >= endColumn))
		{
			throw new ArgumentOutOfRangeException("endColumn");
		}
		MarkSequencePointCore(document, startLine, startColumn, endLine, endColumn);
	}

	protected virtual void MarkSequencePointCore(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
	{
		throw new NotSupportedException(SR.NotSupported_EmitDebugInfo);
	}
}

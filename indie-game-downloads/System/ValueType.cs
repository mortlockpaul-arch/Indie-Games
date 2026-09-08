using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

[Serializable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public abstract class ValueType
{
	private enum ValueTypeHashCodeStrategy
	{
		None,
		ReferenceField,
		DoubleField,
		SingleField,
		FastGetHashCode,
		ValueTypeOverride
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "Trimmed fields don't make a difference for equality")]
	public unsafe override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (GetType() != obj.GetType())
		{
			return false;
		}
		if (CanCompareBitsOrUseFastGetHashCode(RuntimeHelpers.GetMethodTable(obj)))
		{
			return SpanHelpers.SequenceEqual(ref this.GetRawData(), ref obj.GetRawData(), RuntimeHelpers.GetMethodTable(this)->GetNumInstanceFieldBytes());
		}
		FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(this);
			object value2 = fields[i].GetValue(obj);
			if (value == null)
			{
				if (value2 != null)
				{
					return false;
				}
			}
			else if (!value.Equals(value2))
			{
				return false;
			}
		}
		return true;
	}

	private unsafe static bool CanCompareBitsOrUseFastGetHashCode(MethodTable* pMT)
	{
		MethodTableAuxiliaryData* auxiliaryData = pMT->AuxiliaryData;
		if (auxiliaryData->HasCheckedCanCompareBitsOrUseFastGetHashCode)
		{
			return auxiliaryData->CanCompareBitsOrUseFastGetHashCode;
		}
		return CanCompareBitsOrUseFastGetHashCodeHelper(pMT);
	}

	[LibraryImport("QCall", EntryPoint = "MethodTable_CanCompareBitsOrUseFastGetHashCode")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private unsafe static bool CanCompareBitsOrUseFastGetHashCodeHelper(MethodTable* pMT)
	{
		return __PInvoke(pMT) != 0;
		[DllImport("QCall", EntryPoint = "MethodTable_CanCompareBitsOrUseFastGetHashCode", ExactSpelling = true)]
		unsafe static extern int __PInvoke(MethodTable* __pMT_native);
	}

	public unsafe override int GetHashCode()
	{
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(this);
		ref byte rawData = ref this.GetRawData();
		HashCode hashCode = default(HashCode);
		hashCode.Add((nint)methodTable);
		if (CanCompareBitsOrUseFastGetHashCode(methodTable))
		{
			uint numInstanceFieldBytes = methodTable->GetNumInstanceFieldBytes();
			hashCode.AddBytes(MemoryMarshal.CreateReadOnlySpan(in rawData, (int)numInstanceFieldBytes));
		}
		else
		{
			object o = this;
			uint fieldOffset;
			uint fieldSize;
			MethodTable* fieldMT;
			switch (GetHashCodeStrategy(methodTable, ObjectHandleOnStack.Create(ref o), out fieldOffset, out fieldSize, out fieldMT))
			{
			case ValueTypeHashCodeStrategy.ReferenceField:
				hashCode.Add(Unsafe.As<byte, object>(ref Unsafe.AddByteOffset(ref rawData, fieldOffset)).GetHashCode());
				break;
			case ValueTypeHashCodeStrategy.DoubleField:
				hashCode.Add(Unsafe.As<byte, double>(ref Unsafe.AddByteOffset(ref rawData, fieldOffset)).GetHashCode());
				break;
			case ValueTypeHashCodeStrategy.SingleField:
				hashCode.Add(Unsafe.As<byte, float>(ref Unsafe.AddByteOffset(ref rawData, fieldOffset)).GetHashCode());
				break;
			case ValueTypeHashCodeStrategy.FastGetHashCode:
				hashCode.AddBytes(MemoryMarshal.CreateReadOnlySpan(in Unsafe.AddByteOffset(ref rawData, fieldOffset), (int)fieldSize));
				break;
			case ValueTypeHashCodeStrategy.ValueTypeOverride:
				hashCode.Add(RuntimeHelpers.Box(fieldMT, ref Unsafe.AddByteOffset(ref rawData, fieldOffset))?.GetHashCode() ?? 0);
				break;
			}
		}
		return hashCode.ToHashCode();
	}

	[LibraryImport("QCall", EntryPoint = "ValueType_GetHashCodeStrategy")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static ValueTypeHashCodeStrategy GetHashCodeStrategy(MethodTable* pMT, ObjectHandleOnStack objHandle, out uint fieldOffset, out uint fieldSize, out MethodTable* fieldMT)
	{
		fieldOffset = 0u;
		fieldSize = 0u;
		fieldMT = default(MethodTable*);
		ValueTypeHashCodeStrategy result;
		fixed (MethodTable** _fieldMT_native = &fieldMT)
		{
			fixed (uint* _fieldSize_native = &fieldSize)
			{
				fixed (uint* _fieldOffset_native = &fieldOffset)
				{
					result = __PInvoke(pMT, objHandle, _fieldOffset_native, _fieldSize_native, _fieldMT_native);
				}
			}
		}
		return result;
		[DllImport("QCall", EntryPoint = "ValueType_GetHashCodeStrategy", ExactSpelling = true)]
		unsafe static extern ValueTypeHashCodeStrategy __PInvoke(MethodTable* __pMT_native, ObjectHandleOnStack __objHandle_native, uint* __fieldOffset_native, uint* __fieldSize_native, MethodTable** __fieldMT_native);
	}

	public override string? ToString()
	{
		return GetType().ToString();
	}
}

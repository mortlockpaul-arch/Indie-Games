using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public class Object
{
	[Intrinsic]
	public unsafe Type GetType()
	{
		RuntimeType runtimeType = RuntimeTypeHandle.GetRuntimeType(RuntimeHelpers.GetMethodTable(this));
		GC.KeepAlive(this);
		return runtimeType;
	}

	[Intrinsic]
	protected internal unsafe object MemberwiseClone()
	{
		object o = this;
		RuntimeHelpers.AllocateUninitializedClone(ObjectHandleOnStack.Create(ref o));
		nuint rawObjectDataSize = RuntimeHelpers.GetRawObjectDataSize(o);
		ref byte rawData = ref this.GetRawData();
		ref byte rawData2 = ref o.GetRawData();
		if (RuntimeHelpers.GetMethodTable(o)->ContainsGCPointers)
		{
			Buffer.BulkMoveWithWriteBarrier(ref rawData2, ref rawData, rawObjectDataSize);
		}
		else
		{
			SpanHelpers.Memmove(ref rawData2, ref rawData, rawObjectDataSize);
		}
		return o;
	}

	[NonVersionable]
	public Object()
	{
	}

	[NonVersionable]
	~Object()
	{
	}

	public virtual string? ToString()
	{
		return GetType().ToString();
	}

	public virtual bool Equals(object? obj)
	{
		return this == obj;
	}

	public static bool Equals(object? objA, object? objB)
	{
		if (objA == objB)
		{
			return true;
		}
		if (objA == null || objB == null)
		{
			return false;
		}
		return objA.Equals(objB);
	}

	[NonVersionable]
	public static bool ReferenceEquals(object? objA, object? objB)
	{
		return objA == objB;
	}

	public virtual int GetHashCode()
	{
		return RuntimeHelpers.GetHashCode(this);
	}
}

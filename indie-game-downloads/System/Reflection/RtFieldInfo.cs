using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace System.Reflection;

internal sealed class RtFieldInfo : RuntimeFieldInfo, IRuntimeFieldInfo
{
	private readonly nint m_fieldHandle;

	private readonly FieldAttributes m_fieldAttributes;

	private string m_name;

	private RuntimeType m_fieldType;

	private FieldAccessor m_fieldAccessor;

	internal FieldAccessor FieldAccessor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (m_fieldAccessor == null)
			{
				m_fieldAccessor = new FieldAccessor(this);
			}
			return m_fieldAccessor;
		}
	}

	RuntimeFieldHandleInternal IRuntimeFieldInfo.Value => new RuntimeFieldHandleInternal(m_fieldHandle);

	public override string Name => m_name ?? (m_name = RuntimeFieldHandle.GetName(this));

	public override int MetadataToken => RuntimeFieldHandle.GetToken(this);

	public override RuntimeFieldHandle FieldHandle => new RuntimeFieldHandle(this);

	public override FieldAttributes Attributes => m_fieldAttributes;

	public override Type FieldType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_fieldType ?? InitializeFieldType();
		}
	}

	internal RtFieldInfo(RuntimeFieldHandleInternal handle, RuntimeType declaringType, RuntimeType.RuntimeTypeCache reflectedTypeCache, BindingFlags bindingFlags)
		: base(reflectedTypeCache, declaringType, bindingFlags)
	{
		m_fieldHandle = handle.Value;
		m_fieldAttributes = RuntimeFieldHandle.GetAttributes(handle);
	}

	internal override bool CacheEquals(object o)
	{
		if (o is RtFieldInfo rtFieldInfo)
		{
			return rtFieldInfo.m_fieldHandle == m_fieldHandle;
		}
		return false;
	}

	internal override RuntimeModule GetRuntimeModule()
	{
		return RuntimeTypeHandle.GetModule(RuntimeFieldHandle.GetApproxDeclaringType(this));
	}

	public override bool Equals(object obj)
	{
		if (this != obj)
		{
			if (MetadataUpdater.IsSupported && obj is RtFieldInfo rtFieldInfo && rtFieldInfo.m_fieldHandle == m_fieldHandle)
			{
				return (object)rtFieldInfo.m_reflectedTypeCache.GetRuntimeType() == m_reflectedTypeCache.GetRuntimeType();
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(((IntPtr)m_fieldHandle).GetHashCode(), ((IntPtr)m_declaringType.GetUnderlyingNativeHandle()).GetHashCode());
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public override object GetValue(object obj)
	{
		return FieldAccessor.GetValue(obj);
	}

	public override object GetRawConstantValue()
	{
		throw new InvalidOperationException();
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public override object GetValueDirect(TypedReference obj)
	{
		if (obj.IsNull)
		{
			throw new ArgumentException(SR.Arg_TypedReference_Null);
		}
		return RuntimeFieldHandle.GetValueDirect(this, (RuntimeType)FieldType, obj, (RuntimeType)DeclaringType);
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		FieldAccessor.SetValue(obj, value, invokeAttr, binder, culture);
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public override void SetValueDirect(TypedReference obj, object value)
	{
		if (obj.IsNull)
		{
			throw new ArgumentException(SR.Arg_TypedReference_Null);
		}
		RuntimeFieldHandle.SetValueDirect(this, (RuntimeType)FieldType, obj, value, (RuntimeType)DeclaringType);
	}

	internal nint GetFieldDesc()
	{
		return m_fieldHandle;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private RuntimeType InitializeFieldType()
	{
		return m_fieldType = GetSignature().FieldType;
	}

	public override Type[] GetRequiredCustomModifiers()
	{
		return GetSignature().GetCustomModifiers(0, required: true);
	}

	public override Type[] GetOptionalCustomModifiers()
	{
		return GetSignature().GetCustomModifiers(0, required: false);
	}

	internal Signature GetSignature()
	{
		return new Signature(this, m_declaringType);
	}

	public override Type GetModifiedFieldType()
	{
		return ModifiedType.Create(FieldType, GetSignature());
	}
}

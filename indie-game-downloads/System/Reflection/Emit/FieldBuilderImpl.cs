using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit;

internal sealed class FieldBuilderImpl : FieldBuilder
{
	private readonly TypeBuilderImpl _typeBuilder;

	private readonly string _fieldName;

	private readonly Type _fieldType;

	private readonly Type[] _requiredCustomModifiers;

	private readonly Type[] _optionalCustomModifiers;

	private FieldAttributes _attributes;

	internal MarshallingData _marshallingData;

	internal int _offset;

	internal List<CustomAttributeWrapper> _customAttributes;

	internal object _defaultValue = DBNull.Value;

	internal FieldDefinitionHandle _handle;

	internal byte[] _rvaData;

	public override int MetadataToken
	{
		get
		{
			if (!(_handle == default(FieldDefinitionHandle)))
			{
				return MetadataTokens.GetToken(_handle);
			}
			return 0;
		}
	}

	public override Module Module => _typeBuilder.Module;

	public override string Name => _fieldName;

	public override Type DeclaringType
	{
		get
		{
			if (!_typeBuilder._isHiddenGlobalType)
			{
				return _typeBuilder;
			}
			return null;
		}
	}

	public override Type ReflectedType => DeclaringType;

	public override Type FieldType => _fieldType;

	public override RuntimeFieldHandle FieldHandle
	{
		get
		{
			throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
		}
	}

	public override FieldAttributes Attributes => _attributes;

	internal FieldBuilderImpl(TypeBuilderImpl typeBuilder, string fieldName, Type type, FieldAttributes attributes, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers)
	{
		_fieldName = fieldName;
		_typeBuilder = typeBuilder;
		_fieldType = type;
		_attributes = attributes & ~FieldAttributes.ReservedMask;
		_offset = -1;
		_requiredCustomModifiers = requiredCustomModifiers;
		_optionalCustomModifiers = optionalCustomModifiers;
	}

	protected override void SetConstantCore(object defaultValue)
	{
		_typeBuilder.ThrowIfCreated();
		_defaultValue = defaultValue;
		_attributes |= FieldAttributes.HasDefault;
	}

	internal void SetData(byte[] data)
	{
		_rvaData = data;
		_attributes |= FieldAttributes.HasFieldRVA;
	}

	protected override void SetCustomAttributeCore(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		switch (con.ReflectedType.FullName)
		{
		case "System.Runtime.InteropServices.FieldOffsetAttribute":
			_offset = BinaryPrimitives.ReadInt32LittleEndian(binaryAttribute.Slice(2));
			return;
		case "System.NonSerializedAttribute":
			_attributes |= FieldAttributes.NotSerialized;
			return;
		case "System.Runtime.CompilerServices.SpecialNameAttribute":
			_attributes |= FieldAttributes.SpecialName;
			return;
		case "System.Runtime.InteropServices.MarshalAsAttribute":
			_attributes |= FieldAttributes.HasFieldMarshal;
			_marshallingData = MarshallingData.CreateMarshallingData(con, binaryAttribute, isField: true);
			return;
		}
		if (_customAttributes == null)
		{
			_customAttributes = new List<CustomAttributeWrapper>();
		}
		_customAttributes.Add(new CustomAttributeWrapper(con, binaryAttribute));
	}

	protected override void SetOffsetCore(int iOffset)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(iOffset, "iOffset");
		_offset = iOffset;
	}

	public override object GetValue(object obj)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override void SetValue(object obj, object val, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override Type[] GetRequiredCustomModifiers()
	{
		return _requiredCustomModifiers ?? Type.EmptyTypes;
	}

	public override Type[] GetOptionalCustomModifiers()
	{
		return _optionalCustomModifiers ?? Type.EmptyTypes;
	}

	public override Type GetModifiedFieldType()
	{
		return FieldType;
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}
}

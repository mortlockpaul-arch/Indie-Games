using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection;

internal readonly struct MetadataImport
{
	private readonly nint m_metadataImport2;

	public override int GetHashCode()
	{
		return HashCode.Combine(m_metadataImport2);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MetadataImport))
		{
			return false;
		}
		return Equals((MetadataImport)obj);
	}

	private bool Equals(MetadataImport import)
	{
		return import.m_metadataImport2 == m_metadataImport2;
	}

	[LibraryImport("QCall", EntryPoint = "MetadataImport_GetMarshalAs")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private unsafe static bool GetMarshalAs(nint pNativeType, int cNativeType, out int unmanagedType, out int safeArraySubType, out byte* safeArrayUserDefinedSubType, out int safeArrayUserDefinedSubTypeLength, out int arraySubType, out int sizeParamIndex, out int sizeConst, out byte* marshalType, out int marshalTypeLength, out byte* marshalCookie, out int marshalCookieLength, out int iidParamIndex)
	{
		unmanagedType = 0;
		safeArraySubType = 0;
		safeArrayUserDefinedSubType = default(byte*);
		safeArrayUserDefinedSubTypeLength = 0;
		arraySubType = 0;
		sizeParamIndex = 0;
		sizeConst = 0;
		marshalType = default(byte*);
		marshalTypeLength = 0;
		marshalCookie = default(byte*);
		marshalCookieLength = 0;
		iidParamIndex = 0;
		int num;
		fixed (int* _iidParamIndex_native = &iidParamIndex)
		{
			fixed (int* _marshalCookieLength_native = &marshalCookieLength)
			{
				fixed (byte** _marshalCookie_native = &marshalCookie)
				{
					fixed (int* _marshalTypeLength_native = &marshalTypeLength)
					{
						fixed (byte** _marshalType_native = &marshalType)
						{
							fixed (int* _sizeConst_native = &sizeConst)
							{
								fixed (int* _sizeParamIndex_native = &sizeParamIndex)
								{
									fixed (int* _arraySubType_native = &arraySubType)
									{
										fixed (int* _safeArrayUserDefinedSubTypeLength_native = &safeArrayUserDefinedSubTypeLength)
										{
											fixed (byte** _safeArrayUserDefinedSubType_native = &safeArrayUserDefinedSubType)
											{
												fixed (int* _safeArraySubType_native = &safeArraySubType)
												{
													fixed (int* _unmanagedType_native = &unmanagedType)
													{
														num = __PInvoke(pNativeType, cNativeType, _unmanagedType_native, _safeArraySubType_native, _safeArrayUserDefinedSubType_native, _safeArrayUserDefinedSubTypeLength_native, _arraySubType_native, _sizeParamIndex_native, _sizeConst_native, _marshalType_native, _marshalTypeLength_native, _marshalCookie_native, _marshalCookieLength_native, _iidParamIndex_native);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return num != 0;
		[DllImport("QCall", EntryPoint = "MetadataImport_GetMarshalAs", ExactSpelling = true)]
		unsafe static extern int __PInvoke(nint __pNativeType_native, int __cNativeType_native, int* __unmanagedType_native, int* __safeArraySubType_native, byte** __safeArrayUserDefinedSubType_native, int* __safeArrayUserDefinedSubTypeLength_native, int* __arraySubType_native, int* __sizeParamIndex_native, int* __sizeConst_native, byte** __marshalType_native, int* __marshalTypeLength_native, byte** __marshalCookie_native, int* __marshalCookieLength_native, int* __iidParamIndex_native);
	}

	internal unsafe static MarshalAsAttribute GetMarshalAs(ConstArray nativeType, RuntimeModule scope)
	{
		if (!GetMarshalAs(nativeType.Signature, nativeType.Length, out var unmanagedType, out var safeArraySubType, out var safeArrayUserDefinedSubType, out var safeArrayUserDefinedSubTypeLength, out var arraySubType, out var sizeParamIndex, out var sizeConst, out var marshalType, out var marshalTypeLength, out var marshalCookie, out var marshalCookieLength, out var iidParamIndex))
		{
			throw new BadImageFormatException();
		}
		string text = ((safeArrayUserDefinedSubType == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(safeArrayUserDefinedSubType, safeArrayUserDefinedSubTypeLength)));
		string text2 = ((marshalType == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(marshalType, marshalTypeLength)));
		string marshalCookie2 = ((marshalCookie == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(marshalCookie, marshalCookieLength)));
		RuntimeType safeArrayUserDefinedSubType2 = null;
		try
		{
			safeArrayUserDefinedSubType2 = (string.IsNullOrEmpty(text) ? null : TypeNameResolver.GetTypeReferencedByCustomAttribute(text, scope));
		}
		catch (TypeLoadException)
		{
		}
		RuntimeType marshalTypeRef = null;
		try
		{
			marshalTypeRef = (string.IsNullOrEmpty(text2) ? null : TypeNameResolver.GetTypeReferencedByCustomAttribute(text2, scope));
		}
		catch (TypeLoadException)
		{
		}
		return new MarshalAsAttribute((UnmanagedType)unmanagedType)
		{
			SafeArraySubType = (VarEnum)safeArraySubType,
			SafeArrayUserDefinedSubType = safeArrayUserDefinedSubType2,
			IidParameterIndex = iidParamIndex,
			ArraySubType = (UnmanagedType)arraySubType,
			SizeParamIndex = (short)sizeParamIndex,
			SizeConst = sizeConst,
			MarshalType = text2,
			MarshalTypeRef = marshalTypeRef,
			MarshalCookie = marshalCookie2
		};
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern nint GetMetadataImport(RuntimeModule module);

	internal MetadataImport(RuntimeModule module)
	{
		ArgumentNullException.ThrowIfNull(module, "module");
		m_metadataImport2 = GetMetadataImport(module);
	}

	[LibraryImport("QCall", EntryPoint = "MetadataImport_Enum")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void Enum(nint scope, int type, int parent, ref int length, int* shortResult, ObjectHandleOnStack longResult)
	{
		fixed (int* _length_native = &length)
		{
			__PInvoke(scope, type, parent, _length_native, shortResult, longResult);
		}
		[DllImport("QCall", EntryPoint = "MetadataImport_Enum", ExactSpelling = true)]
		unsafe static extern void __PInvoke(nint __scope_native, int __type_native, int __parent_native, int* __length_native, int* __shortResult_native, ObjectHandleOnStack __longResult_native);
	}

	public unsafe void Enum(MetadataTokenType type, int parent, out MetadataEnumResult result)
	{
		result = default(MetadataEnumResult);
		int length = 16;
		fixed (int* e = &result._smallResult.e)
		{
			Enum(m_metadataImport2, (int)type, parent, ref length, e, ObjectHandleOnStack.Create(ref result._largeResult));
		}
		result._length = length;
	}

	public void EnumNestedTypes(int mdTypeDef, out MetadataEnumResult result)
	{
		Enum(MetadataTokenType.TypeDef, mdTypeDef, out result);
	}

	public void EnumCustomAttributes(int mdToken, out MetadataEnumResult result)
	{
		Enum(MetadataTokenType.CustomAttribute, mdToken, out result);
	}

	public void EnumParams(int mdMethodDef, out MetadataEnumResult result)
	{
		Enum(MetadataTokenType.ParamDef, mdMethodDef, out result);
	}

	public void EnumFields(int mdTypeDef, out MetadataEnumResult result)
	{
		Enum(MetadataTokenType.FieldDef, mdTypeDef, out result);
	}

	public void EnumProperties(int mdTypeDef, out MetadataEnumResult result)
	{
		Enum(MetadataTokenType.Property, mdTypeDef, out result);
	}

	public void EnumEvents(int mdTypeDef, out MetadataEnumResult result)
	{
		Enum(MetadataTokenType.Event, mdTypeDef, out result);
	}

	private unsafe static string ConvertMetadataStringPermitInvalidContent(char* stringMetadataEncoding, int length)
	{
		return new string(stringMetadataEncoding, 0, length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetDefaultValue(nint scope, int mdToken, out long value, out char* stringMetadataEncoding, out int length, out int corElementType);

	public unsafe string GetDefaultValue(int mdToken, out long value, out int length, out CorElementType corElementType)
	{
		ThrowBadImageExceptionForHR(GetDefaultValue(m_metadataImport2, mdToken, out value, out var stringMetadataEncoding, out length, out var corElementType2));
		corElementType = (CorElementType)corElementType2;
		if (corElementType == CorElementType.ELEMENT_TYPE_STRING && stringMetadataEncoding != null)
		{
			return ConvertMetadataStringPermitInvalidContent(stringMetadataEncoding, length);
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetUserString(nint scope, int mdToken, out char* stringMetadataEncoding, out int length);

	public unsafe string GetUserString(int mdToken)
	{
		ThrowBadImageExceptionForHR(GetUserString(m_metadataImport2, mdToken, out var stringMetadataEncoding, out var length));
		if (stringMetadataEncoding == null)
		{
			return null;
		}
		return ConvertMetadataStringPermitInvalidContent(stringMetadataEncoding, length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetName(nint scope, int mdToken, out byte* name);

	public unsafe MdUtf8String GetName(int mdToken)
	{
		ThrowBadImageExceptionForHR(GetName(m_metadataImport2, mdToken, out var name));
		return new MdUtf8String(name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetNamespace(nint scope, int mdToken, out byte* namesp);

	public unsafe MdUtf8String GetNamespace(int mdToken)
	{
		ThrowBadImageExceptionForHR(GetNamespace(m_metadataImport2, mdToken, out var namesp));
		return new MdUtf8String(namesp);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetEventProps(nint scope, int mdToken, out void* name, out int eventAttributes);

	public unsafe void GetEventProps(int mdToken, out void* name, out EventAttributes eventAttributes)
	{
		ThrowBadImageExceptionForHR(GetEventProps(m_metadataImport2, mdToken, out name, out var eventAttributes2));
		eventAttributes = (EventAttributes)eventAttributes2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFieldDefProps(nint scope, int mdToken, out int fieldAttributes);

	public void GetFieldDefProps(int mdToken, out FieldAttributes fieldAttributes)
	{
		ThrowBadImageExceptionForHR(GetFieldDefProps(m_metadataImport2, mdToken, out var fieldAttributes2));
		fieldAttributes = (FieldAttributes)fieldAttributes2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetPropertyProps(nint scope, int mdToken, out void* name, out int propertyAttributes, out ConstArray signature);

	public unsafe void GetPropertyProps(int mdToken, out void* name, out PropertyAttributes propertyAttributes, out ConstArray signature)
	{
		ThrowBadImageExceptionForHR(GetPropertyProps(m_metadataImport2, mdToken, out name, out var propertyAttributes2, out signature));
		propertyAttributes = (PropertyAttributes)propertyAttributes2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetParentToken(nint scope, int mdToken, out int tkParent);

	public int GetParentToken(int tkToken)
	{
		ThrowBadImageExceptionForHR(GetParentToken(m_metadataImport2, tkToken, out var tkParent));
		return tkParent;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetParamDefProps(nint scope, int parameterToken, out int sequence, out int attributes);

	public void GetParamDefProps(int parameterToken, out int sequence, out ParameterAttributes attributes)
	{
		ThrowBadImageExceptionForHR(GetParamDefProps(m_metadataImport2, parameterToken, out sequence, out var attributes2));
		attributes = (ParameterAttributes)attributes2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetGenericParamProps(nint scope, int genericParameter, out int flags);

	public void GetGenericParamProps(int genericParameter, out GenericParameterAttributes attributes)
	{
		ThrowBadImageExceptionForHR(GetGenericParamProps(m_metadataImport2, genericParameter, out var flags));
		attributes = (GenericParameterAttributes)flags;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetScopeProps(nint scope, out Guid mvid);

	public void GetScopeProps(out Guid mvid)
	{
		ThrowBadImageExceptionForHR(GetScopeProps(m_metadataImport2, out mvid));
	}

	public ConstArray GetMethodSignature(MetadataToken token)
	{
		if (token.IsMemberRef)
		{
			return GetMemberRefProps(token);
		}
		return GetSigOfMethodDef(token);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSigOfMethodDef(nint scope, int methodToken, ref ConstArray signature);

	public ConstArray GetSigOfMethodDef(int methodToken)
	{
		ConstArray signature = default(ConstArray);
		ThrowBadImageExceptionForHR(GetSigOfMethodDef(m_metadataImport2, methodToken, ref signature));
		return signature;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSignatureFromToken(nint scope, int methodToken, ref ConstArray signature);

	public ConstArray GetSignatureFromToken(int token)
	{
		ConstArray signature = default(ConstArray);
		ThrowBadImageExceptionForHR(GetSignatureFromToken(m_metadataImport2, token, ref signature));
		return signature;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMemberRefProps(nint scope, int memberTokenRef, out ConstArray signature);

	public ConstArray GetMemberRefProps(int memberTokenRef)
	{
		ThrowBadImageExceptionForHR(GetMemberRefProps(m_metadataImport2, memberTokenRef, out var signature));
		return signature;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCustomAttributeProps(nint scope, int customAttributeToken, out int constructorToken, out ConstArray signature);

	public void GetCustomAttributeProps(int customAttributeToken, out int constructorToken, out ConstArray signature)
	{
		ThrowBadImageExceptionForHR(GetCustomAttributeProps(m_metadataImport2, customAttributeToken, out constructorToken, out signature));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetClassLayout(nint scope, int typeTokenDef, out int packSize, out int classSize);

	public void GetClassLayout(int typeTokenDef, out int packSize, out int classSize)
	{
		ThrowBadImageExceptionForHR(GetClassLayout(m_metadataImport2, typeTokenDef, out packSize, out classSize));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFieldOffset(nint scope, int typeTokenDef, int fieldTokenDef, out int offset, out bool found);

	public bool GetFieldOffset(int typeTokenDef, int fieldTokenDef, out int offset)
	{
		int fieldOffset = GetFieldOffset(m_metadataImport2, typeTokenDef, fieldTokenDef, out offset, out var found);
		if (!found && fieldOffset < 0)
		{
			throw new BadImageFormatException();
		}
		return found;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSigOfFieldDef(nint scope, int fieldToken, ref ConstArray fieldMarshal);

	public ConstArray GetSigOfFieldDef(int fieldToken)
	{
		ConstArray fieldMarshal = default(ConstArray);
		ThrowBadImageExceptionForHR(GetSigOfFieldDef(m_metadataImport2, fieldToken, ref fieldMarshal));
		return fieldMarshal;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFieldMarshal(nint scope, int fieldToken, ref ConstArray fieldMarshal);

	public ConstArray GetFieldMarshal(int fieldToken)
	{
		ConstArray fieldMarshal = default(ConstArray);
		ThrowBadImageExceptionForHR(GetFieldMarshal(m_metadataImport2, fieldToken, ref fieldMarshal));
		return fieldMarshal;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetPInvokeMap(nint scope, int token, out int attributes, out byte* importName, out byte* importDll);

	public unsafe void GetPInvokeMap(int token, out PInvokeAttributes attributes, out string importName, out string importDll)
	{
		ThrowBadImageExceptionForHR(GetPInvokeMap(m_metadataImport2, token, out var attributes2, out var importName2, out var importDll2));
		importName = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(importName2));
		importDll = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(importDll2));
		attributes = (PInvokeAttributes)attributes2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValidToken(nint scope, int token);

	public bool IsValidToken(int token)
	{
		return IsValidToken(m_metadataImport2, token);
	}

	private static void ThrowBadImageExceptionForHR(int hr)
	{
		if (hr < 0)
		{
			throw new BadImageFormatException();
		}
	}
}

namespace System.Runtime.InteropServices.Marshalling;

[CustomMarshaller(typeof(object), MarshalMode.Default, typeof(ComVariantMarshaller))]
[CustomMarshaller(typeof(object), MarshalMode.UnmanagedToManagedRef, typeof(RefPropagate))]
public static class ComVariantMarshaller
{
	public struct RefPropagate
	{
		private ComVariant _unmanaged;

		private object _managed;

		public void FromUnmanaged(ComVariant unmanaged)
		{
			_unmanaged = unmanaged;
		}

		public void FromManaged(object? managed)
		{
			_managed = managed;
		}

		public unsafe ComVariant ToUnmanaged()
		{
			if (!_unmanaged.VarType.HasFlag(VarEnum.VT_BYREF))
			{
				return ConvertToUnmanaged(_managed);
			}
			bool flag = _managed == null;
			VarEnum varEnum;
			if (flag)
			{
				varEnum = _unmanaged.VarType & (VarEnum)(-16385);
				bool flag2 = (((uint)(varEnum - 8) <= 1u || varEnum == VarEnum.VT_UNKNOWN) ? true : false);
				flag = flag2;
			}
			if (flag)
			{
				*(IntPtr*)_unmanaged.GetRawDataRef<nint>() = (IntPtr)0;
				return _unmanaged;
			}
			varEnum = _unmanaged.VarType & (VarEnum)(-16385);
			object managed = _managed;
			int num;
			uint num8;
			switch (varEnum)
			{
			case VarEnum.VT_I1:
			case VarEnum.VT_UI1:
				if (!(managed is sbyte b))
				{
					if (managed is byte b2)
					{
						*(byte*)_unmanaged.GetRawDataRef<nint>() = b2;
						break;
					}
					goto default;
				}
				*(sbyte*)_unmanaged.GetRawDataRef<nint>() = b;
				break;
			case VarEnum.VT_I2:
			case VarEnum.VT_UI2:
				if (!(managed is short num3))
				{
					if (managed is ushort num4)
					{
						*(ushort*)_unmanaged.GetRawDataRef<nint>() = num4;
						break;
					}
					goto default;
				}
				*(short*)_unmanaged.GetRawDataRef<nint>() = num3;
				break;
			case VarEnum.VT_I4:
			case VarEnum.VT_UI4:
			case VarEnum.VT_INT:
			case VarEnum.VT_UINT:
				if (managed is int)
				{
					goto IL_0178;
				}
				if (managed is uint)
				{
					goto IL_018f;
				}
				goto default;
			case VarEnum.VT_ERROR:
				if (managed is int)
				{
					goto IL_0178;
				}
				if (managed is uint)
				{
					goto IL_018f;
				}
				if (!(managed is ErrorWrapper errorWrapper))
				{
					goto default;
				}
				*(int*)_unmanaged.GetRawDataRef<nint>() = errorWrapper.ErrorCode;
				break;
			case VarEnum.VT_I8:
			case VarEnum.VT_UI8:
				if (!(managed is long num6))
				{
					if (managed is ulong num7)
					{
						*(ulong*)_unmanaged.GetRawDataRef<nint>() = num7;
						break;
					}
					goto default;
				}
				*(long*)_unmanaged.GetRawDataRef<nint>() = num6;
				break;
			case VarEnum.VT_R4:
				if (managed is float num5)
				{
					*(float*)_unmanaged.GetRawDataRef<nint>() = num5;
					break;
				}
				goto default;
			case VarEnum.VT_R8:
				if (managed is double num2)
				{
					*(double*)_unmanaged.GetRawDataRef<nint>() = num2;
					break;
				}
				goto default;
			case VarEnum.VT_DECIMAL:
				if (managed is decimal num9)
				{
					*(decimal*)_unmanaged.GetRawDataRef<nint>() = num9;
					break;
				}
				goto default;
			case VarEnum.VT_BOOL:
				if (managed is bool flag3)
				{
					*(short*)_unmanaged.GetRawDataRef<nint>() = (short)(flag3 ? (-1) : 0);
					break;
				}
				goto default;
			case VarEnum.VT_BSTR:
			{
				if (!(managed is string s))
				{
					if (!(managed is BStrWrapper bStrWrapper))
					{
						goto default;
					}
					ref nint reference = ref *(nint*)_unmanaged.GetRawDataRef<nint>();
					Marshal.FreeBSTR(reference);
					reference = Marshal.StringToBSTR(bStrWrapper.WrappedObject);
					break;
				}
				ref nint reference2 = ref *(nint*)_unmanaged.GetRawDataRef<nint>();
				Marshal.FreeBSTR(reference2);
				reference2 = Marshal.StringToBSTR(s);
				break;
			}
			case VarEnum.VT_DATE:
				if (managed is DateTime dateTime)
				{
					*(double*)_unmanaged.GetRawDataRef<nint>() = dateTime.ToOADate();
					break;
				}
				goto default;
			case VarEnum.VT_CY:
				if (!(managed is CurrencyWrapper currencyWrapper))
				{
					goto default;
				}
				*(long*)_unmanaged.GetRawDataRef<nint>() = decimal.ToOACurrency(currencyWrapper.WrappedObject);
				break;
			case VarEnum.VT_UNKNOWN:
				if (managed == null)
				{
					goto default;
				}
				*(nint*)_unmanaged.GetRawDataRef<nint>() = StrategyBasedComWrappers.DefaultMarshallingInstance.GetOrCreateComInterfaceForObject(managed, CreateComInterfaceFlags.None);
				break;
			case VarEnum.VT_VARIANT:
				*(ComVariant*)_unmanaged.GetRawDataRef<nint>() = ConvertToUnmanaged(_managed);
				break;
			default:
				{
					throw new ArgumentException("Invalid combination of unmanaged variant type and managed object type.", "_managed");
				}
				IL_0178:
				num = (int)managed;
				*(int*)_unmanaged.GetRawDataRef<nint>() = num;
				break;
				IL_018f:
				num8 = (uint)managed;
				*(uint*)_unmanaged.GetRawDataRef<nint>() = num8;
				break;
			}
			return _unmanaged;
		}

		public object? ToManaged()
		{
			return ConvertToManaged(_unmanaged);
		}

		public void Free()
		{
			_unmanaged.Dispose();
		}
	}

	public static ComVariant ConvertToUnmanaged(object? managed)
	{
		if (managed == null)
		{
			return default(ComVariant);
		}
		if (!(managed is sbyte value))
		{
			if (!(managed is byte value2))
			{
				if (!(managed is short value3))
				{
					if (!(managed is ushort value4))
					{
						if (!(managed is int value5))
						{
							if (!(managed is uint value6))
							{
								if (!(managed is long value7))
								{
									if (!(managed is ulong value8))
									{
										if (!(managed is float value9))
										{
											if (!(managed is double value10))
											{
												if (!(managed is decimal value11))
												{
													if (!(managed is bool value12))
													{
														if (!(managed is string value13))
														{
															if (!(managed is DateTime value14))
															{
																if (!(managed is ErrorWrapper value15))
																{
																	if (!(managed is CurrencyWrapper value16))
																	{
																		if (!(managed is BStrWrapper value17))
																		{
																			if (managed is DBNull)
																			{
																				return ComVariant.Null;
																			}
																			if (TryCreateOleVariantForInterfaceWrapper(managed, out var variant))
																			{
																				return variant;
																			}
																			throw new ArgumentException(System.SR.ComVariantMarshaller_ManagedTypeNotSupported, "managed");
																		}
																		return ComVariant.Create(value17);
																	}
																	return ComVariant.Create(value16);
																}
																return ComVariant.Create(value15);
															}
															return ComVariant.Create(value14);
														}
														return ComVariant.Create(value13);
													}
													return ComVariant.Create(value12);
												}
												return ComVariant.Create(value11);
											}
											return ComVariant.Create(value10);
										}
										return ComVariant.Create(value9);
									}
									return ComVariant.Create(value8);
								}
								return ComVariant.Create(value7);
							}
							return ComVariant.Create(value6);
						}
						return ComVariant.Create(value5);
					}
					return ComVariant.Create(value4);
				}
				return ComVariant.Create(value3);
			}
			return ComVariant.Create(value2);
		}
		return ComVariant.Create(value);
	}

	private static bool TryCreateOleVariantForInterfaceWrapper(object managed, out ComVariant variant)
	{
		if (managed is UnknownWrapper unknownWrapper)
		{
			object wrappedObject = unknownWrapper.WrappedObject;
			if (wrappedObject == null)
			{
				variant = default(ComVariant);
				return true;
			}
			variant = ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, StrategyBasedComWrappers.DefaultMarshallingInstance.GetOrCreateComInterfaceForObject(wrappedObject, CreateComInterfaceFlags.None));
			return true;
		}
		if (managed != null && StrategyBasedComWrappers.DefaultIUnknownInterfaceDetailsStrategy.GetComExposedTypeDetails(managed.GetType().TypeHandle) != null)
		{
			variant = ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, StrategyBasedComWrappers.DefaultMarshallingInstance.GetOrCreateComInterfaceForObject(managed, CreateComInterfaceFlags.None));
			return true;
		}
		variant = default(ComVariant);
		return false;
	}

	public unsafe static object? ConvertToManaged(ComVariant unmanaged)
	{
		switch (unmanaged.VarType)
		{
		case VarEnum.VT_EMPTY:
		case VarEnum.VT_BYREF:
			return null;
		case VarEnum.VT_NULL:
		case (VarEnum)16385:
			return DBNull.Value;
		case VarEnum.VT_I1:
			return unmanaged.As<sbyte>();
		case VarEnum.VT_UI1:
			return unmanaged.As<byte>();
		case VarEnum.VT_I2:
			return unmanaged.As<short>();
		case VarEnum.VT_UI2:
			return unmanaged.As<ushort>();
		case VarEnum.VT_I4:
		case VarEnum.VT_INT:
			return unmanaged.As<int>();
		case VarEnum.VT_UI4:
		case VarEnum.VT_UINT:
			return unmanaged.As<uint>();
		case VarEnum.VT_I8:
			return unmanaged.As<long>();
		case VarEnum.VT_UI8:
			return unmanaged.As<ulong>();
		case VarEnum.VT_R4:
			return unmanaged.As<float>();
		case VarEnum.VT_R8:
			return unmanaged.As<double>();
		case VarEnum.VT_DECIMAL:
			return unmanaged.As<decimal>();
		case VarEnum.VT_BOOL:
			return unmanaged.As<bool>();
		case VarEnum.VT_BSTR:
			return unmanaged.As<string>();
		case VarEnum.VT_DATE:
			return unmanaged.As<DateTime>();
		case VarEnum.VT_ERROR:
			return unmanaged.As<int>();
		case VarEnum.VT_CY:
			return unmanaged.As<CurrencyWrapper>().WrappedObject;
		case VarEnum.VT_DISPATCH:
		case VarEnum.VT_UNKNOWN:
			return StrategyBasedComWrappers.DefaultMarshallingInstance.GetOrCreateObjectForComInstance(unmanaged.GetRawDataRef<nint>(), CreateObjectFlags.Unwrap);
		case (VarEnum)16396:
			return ConvertToManaged(*(ComVariant*)unmanaged.GetRawDataRef<nint>());
		case (VarEnum)16400:
			return *(sbyte*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16401:
			return *(byte*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16386:
			return *(short*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16402:
			return *(ushort*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16387:
			return *(int*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16403:
			return *(uint*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16404:
			return *(long*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16405:
			return *(ulong*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16388:
			return *(float*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16389:
			return *(double*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16398:
			return *(decimal*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16395:
			return *(short*)unmanaged.GetRawDataRef<nint>() != 0;
		case (VarEnum)16392:
			return Marshal.PtrToStringBSTR(*(nint*)unmanaged.GetRawDataRef<nint>());
		case (VarEnum)16391:
			return DateTime.FromOADate(*(double*)unmanaged.GetRawDataRef<nint>());
		case (VarEnum)16394:
			return *(int*)unmanaged.GetRawDataRef<nint>();
		case (VarEnum)16390:
			return decimal.FromOACurrency(*(long*)unmanaged.GetRawDataRef<nint>());
		case (VarEnum)16397:
			return StrategyBasedComWrappers.DefaultMarshallingInstance.GetOrCreateObjectForComInstance(*(nint*)unmanaged.GetRawDataRef<nint>(), CreateObjectFlags.Unwrap);
		default:
			throw new ArgumentException(System.SR.ComVariantMarshaller_UnmanagedTypeNotSupported, "unmanaged");
		}
	}

	public static void Free(ComVariant unmanaged)
	{
		unmanaged.Dispose();
	}
}

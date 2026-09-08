using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace System.Data.Common;

internal sealed class UInt64Storage : DataStorage
{
	private ulong[] _values;

	public UInt64Storage(DataColumn column)
		: base(column, typeof(ulong), 0uL, StorageType.UInt64)
	{
	}

	public override object Aggregate(int[] records, AggregateType kind)
	{
		bool flag = false;
		try
		{
			switch (kind)
			{
			case AggregateType.Sum:
			{
				ulong num13 = 0uL;
				int[] array = records;
				foreach (int num14 in array)
				{
					if (HasValue(num14))
					{
						num13 = checked(num13 + _values[num14]);
						flag = true;
					}
				}
				if (flag)
				{
					return num13;
				}
				return _nullValue;
			}
			case AggregateType.Mean:
			{
				decimal num6 = 0m;
				int num7 = 0;
				int[] array = records;
				foreach (int num8 in array)
				{
					if (HasValue(num8))
					{
						num6 += (decimal)_values[num8];
						num7++;
						flag = true;
					}
				}
				if (flag)
				{
					return (ulong)(num6 / (decimal)num7);
				}
				return _nullValue;
			}
			case AggregateType.Var:
			case AggregateType.StDev:
			{
				int num = 0;
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				int[] array = records;
				foreach (int num5 in array)
				{
					if (HasValue(num5))
					{
						num3 += (double)_values[num5];
						num4 += (double)_values[num5] * (double)_values[num5];
						num++;
					}
				}
				if (num > 1)
				{
					num2 = (double)num * num4 - num3 * num3;
					num2 = ((!(num2 / (num3 * num3) < 1E-15) && !(num2 < 0.0)) ? (num2 / (double)(num * (num - 1))) : 0.0);
					if (kind == AggregateType.StDev)
					{
						return Math.Sqrt(num2);
					}
					return num2;
				}
				return _nullValue;
			}
			case AggregateType.Min:
			{
				ulong num11 = ulong.MaxValue;
				foreach (int num12 in records)
				{
					if (HasValue(num12))
					{
						num11 = Math.Min(_values[num12], num11);
						flag = true;
					}
				}
				if (flag)
				{
					return num11;
				}
				return _nullValue;
			}
			case AggregateType.Max:
			{
				ulong num9 = 0uL;
				foreach (int num10 in records)
				{
					if (HasValue(num10))
					{
						num9 = Math.Max(_values[num10], num9);
						flag = true;
					}
				}
				if (flag)
				{
					return num9;
				}
				return _nullValue;
			}
			case AggregateType.First:
				if (records.Length != 0)
				{
					return _values[records[0]];
				}
				return null;
			case AggregateType.Count:
				return base.Aggregate(records, kind);
			}
		}
		catch (OverflowException)
		{
			throw ExprException.Overflow(typeof(ulong));
		}
		throw ExceptionBuilder.AggregateException(kind, _dataType);
	}

	public override int Compare(int recordNo1, int recordNo2)
	{
		ulong num = _values[recordNo1];
		ulong num2 = _values[recordNo2];
		if (num.Equals(0uL) || num2.Equals(0uL))
		{
			int num3 = CompareBits(recordNo1, recordNo2);
			if (num3 != 0)
			{
				return num3;
			}
		}
		if (num >= num2)
		{
			return (num > num2) ? 1 : 0;
		}
		return -1;
	}

	public override int CompareValueTo(int recordNo, object value)
	{
		if (_nullValue == value)
		{
			return HasValue(recordNo) ? 1 : 0;
		}
		ulong num = _values[recordNo];
		if (num == 0L && !HasValue(recordNo))
		{
			return -1;
		}
		return num.CompareTo((ulong)value);
	}

	public override object ConvertValue(object value)
	{
		if (_nullValue != value)
		{
			value = ((value == null) ? _nullValue : ((object)((IConvertible)value).ToUInt64(base.FormatProvider)));
		}
		return value;
	}

	public override void Copy(int recordNo1, int recordNo2)
	{
		CopyBits(recordNo1, recordNo2);
		_values[recordNo2] = _values[recordNo1];
	}

	public override object Get(int record)
	{
		ulong num = _values[record];
		if (!num.Equals(0uL))
		{
			return num;
		}
		return GetBits(record);
	}

	public override void Set(int record, object value)
	{
		if (_nullValue == value)
		{
			_values[record] = 0uL;
			SetNullBit(record, flag: true);
		}
		else
		{
			_values[record] = ((IConvertible)value).ToUInt64(base.FormatProvider);
			SetNullBit(record, flag: false);
		}
	}

	public override void SetCapacity(int capacity)
	{
		Array.Resize(ref _values, capacity);
		base.SetCapacity(capacity);
	}

	[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly.")]
	[RequiresDynamicCode("Members from serialized types may use dynamic code generation.")]
	public override object ConvertXmlToObject(string s)
	{
		return XmlConvert.ToUInt64(s);
	}

	[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly.")]
	[RequiresDynamicCode("Members from serialized types may use dynamic code generation.")]
	public override string ConvertObjectToXml(object value)
	{
		return XmlConvert.ToString((ulong)value);
	}

	protected override object GetEmptyStorage(int recordCount)
	{
		return new ulong[recordCount];
	}

	protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
	{
		((ulong[])store)[storeIndex] = _values[record];
		nullbits.Set(storeIndex, !HasValue(record));
	}

	protected override void SetStorage(object store, BitArray nullbits)
	{
		_values = (ulong[])store;
		SetNullStorage(nullbits);
	}
}

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace System.Data.Common;

internal sealed class DecimalStorage : DataStorage
{
	private decimal[] _values;

	internal DecimalStorage(DataColumn column)
		: base(column, typeof(decimal), 0m, StorageType.Decimal)
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
				decimal num13 = 0m;
				int[] array = records;
				foreach (int num14 in array)
				{
					if (HasValue(num14))
					{
						num13 += _values[num14];
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
						num6 += _values[num8];
						num7++;
						flag = true;
					}
				}
				if (flag)
				{
					return num6 / (decimal)num7;
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
				decimal num11 = decimal.MaxValue;
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
				decimal num9 = decimal.MinValue;
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
			throw ExprException.Overflow(typeof(decimal));
		}
		throw ExceptionBuilder.AggregateException(kind, _dataType);
	}

	public override int Compare(int recordNo1, int recordNo2)
	{
		decimal num = _values[recordNo1];
		decimal num2 = _values[recordNo2];
		if (num == 0m || num2 == 0m)
		{
			int num3 = CompareBits(recordNo1, recordNo2);
			if (num3 != 0)
			{
				return num3;
			}
		}
		return decimal.Compare(num, num2);
	}

	public override int CompareValueTo(int recordNo, object value)
	{
		if (_nullValue == value)
		{
			return HasValue(recordNo) ? 1 : 0;
		}
		decimal num = _values[recordNo];
		if (0m == num && !HasValue(recordNo))
		{
			return -1;
		}
		return decimal.Compare(num, (decimal)value);
	}

	public override object ConvertValue(object value)
	{
		if (_nullValue != value)
		{
			value = ((value == null) ? _nullValue : ((object)((IConvertible)value).ToDecimal(base.FormatProvider)));
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
		if (!HasValue(record))
		{
			return _nullValue;
		}
		return _values[record];
	}

	public override void Set(int record, object value)
	{
		if (_nullValue == value)
		{
			_values[record] = 0m;
			SetNullBit(record, flag: true);
		}
		else
		{
			_values[record] = ((IConvertible)value).ToDecimal(base.FormatProvider);
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
		return XmlConvert.ToDecimal(s);
	}

	[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly.")]
	[RequiresDynamicCode("Members from serialized types may use dynamic code generation.")]
	public override string ConvertObjectToXml(object value)
	{
		return XmlConvert.ToString((decimal)value);
	}

	protected override object GetEmptyStorage(int recordCount)
	{
		return new decimal[recordCount];
	}

	protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
	{
		((decimal[])store)[storeIndex] = _values[record];
		nullbits.Set(storeIndex, !HasValue(record));
	}

	protected override void SetStorage(object store, BitArray nullbits)
	{
		_values = (decimal[])store;
		SetNullStorage(nullbits);
	}
}

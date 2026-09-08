using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class ConversionResolution
{
	internal enum ConversionClass : sbyte
	{
		Bad,
		Identity,
		Widening,
		Narrowing,
		None,
		Ambiguous
	}

	internal sealed class OperatorCaches
	{
		internal sealed class FixedList
		{
			private struct Entry
			{
				internal Type TargetType;

				internal Type SourceType;

				internal ConversionClass Classification;

				internal Symbols.Method OperatorMethod;

				internal int Next;

				internal int Previous;
			}

			private readonly Entry[] _list;

			private readonly int _size;

			private int _first;

			private int _last;

			private int _count;

			internal FixedList()
				: this(50)
			{
			}

			internal FixedList(int size)
			{
				_size = size;
				checked
				{
					_list = new Entry[_size - 1 + 1];
					int num = _size - 2;
					for (int i = 0; i <= num; i++)
					{
						_list[i].Next = i + 1;
					}
					for (int j = _size - 1; j >= 1; j += -1)
					{
						_list[j].Previous = j - 1;
					}
					_list[0].Previous = _size - 1;
					_last = _size - 1;
				}
			}

			private void MoveToFront(int item)
			{
				if (item != _first)
				{
					int next = _list[item].Next;
					int previous = _list[item].Previous;
					_list[previous].Next = next;
					_list[next].Previous = previous;
					_list[_first].Previous = item;
					_list[_last].Next = item;
					_list[item].Next = _first;
					_list[item].Previous = _last;
					_first = item;
				}
			}

			internal void Insert(Type targetType, Type sourceType, ConversionClass classification, Symbols.Method operatorMethod)
			{
				checked
				{
					if (_count < _size)
					{
						_count++;
					}
					int num = (_first = _last);
					_last = _list[_last].Previous;
					_list[num].TargetType = targetType;
					_list[num].SourceType = sourceType;
					_list[num].Classification = classification;
					_list[num].OperatorMethod = operatorMethod;
				}
			}

			internal bool Lookup(Type targetType, Type sourceType, ref ConversionClass classification, ref Symbols.Method operatorMethod)
			{
				int num = _first;
				for (int i = 0; i < _count; i = checked(i + 1))
				{
					if ((object)targetType == _list[num].TargetType && (object)sourceType == _list[num].SourceType)
					{
						classification = _list[num].Classification;
						operatorMethod = _list[num].OperatorMethod;
						MoveToFront(num);
						return true;
					}
					num = _list[num].Next;
				}
				classification = ConversionClass.Bad;
				operatorMethod = null;
				return false;
			}
		}

		internal sealed class FixedExistenceList
		{
			private struct Entry
			{
				internal Type Type;

				internal int Next;

				internal int Previous;
			}

			private readonly Entry[] _list;

			private readonly int _size;

			private int _first;

			private int _last;

			private int _count;

			internal FixedExistenceList()
				: this(50)
			{
			}

			internal FixedExistenceList(int size)
			{
				_size = size;
				checked
				{
					_list = new Entry[_size - 1 + 1];
					int num = _size - 2;
					for (int i = 0; i <= num; i++)
					{
						_list[i].Next = i + 1;
					}
					for (int j = _size - 1; j >= 1; j += -1)
					{
						_list[j].Previous = j - 1;
					}
					_list[0].Previous = _size - 1;
					_last = _size - 1;
				}
			}

			private void MoveToFront(int item)
			{
				if (item != _first)
				{
					int next = _list[item].Next;
					int previous = _list[item].Previous;
					_list[previous].Next = next;
					_list[next].Previous = previous;
					_list[_first].Previous = item;
					_list[_last].Next = item;
					_list[item].Next = _first;
					_list[item].Previous = _last;
					_first = item;
				}
			}

			internal void Insert(Type type)
			{
				checked
				{
					if (_count < _size)
					{
						_count++;
					}
					int num = (_first = _last);
					_last = _list[_last].Previous;
					_list[num].Type = type;
				}
			}

			internal bool Lookup(Type type)
			{
				int num = _first;
				for (int i = 0; i < _count; i = checked(i + 1))
				{
					if ((object)type == _list[num].Type)
					{
						MoveToFront(num);
						return true;
					}
					num = _list[num].Next;
				}
				return false;
			}
		}

		internal static readonly FixedList ConversionCache = new FixedList();

		internal static readonly FixedExistenceList UnconvertibleTypeCache = new FixedExistenceList();
	}

	private static readonly ConversionClass[][] s_conversionTable;

	internal static readonly int[] NumericSpecificityRank;

	internal static readonly TypeCode[][] ForLoopWidestTypeCode;

	static ConversionResolution()
	{
		s_conversionTable = new ConversionClass[19][]
		{
			new ConversionClass[19],
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Identity,
				ConversionClass.Bad,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Bad,
				ConversionClass.Widening
			},
			new ConversionClass[19],
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Identity,
				ConversionClass.None,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.None,
				ConversionClass.Identity,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Narrowing,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Identity,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Identity,
				ConversionClass.Widening,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.None,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Identity,
				ConversionClass.None,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.None,
				ConversionClass.Identity,
				ConversionClass.Bad,
				ConversionClass.Narrowing
			},
			new ConversionClass[19],
			new ConversionClass[19]
			{
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Narrowing,
				ConversionClass.Widening,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Narrowing,
				ConversionClass.Bad,
				ConversionClass.Identity
			}
		};
		NumericSpecificityRank = new int[19];
		NumericSpecificityRank[6] = 1;
		NumericSpecificityRank[5] = 2;
		NumericSpecificityRank[7] = 3;
		NumericSpecificityRank[8] = 4;
		NumericSpecificityRank[9] = 5;
		NumericSpecificityRank[10] = 6;
		NumericSpecificityRank[11] = 7;
		NumericSpecificityRank[12] = 8;
		NumericSpecificityRank[15] = 9;
		NumericSpecificityRank[13] = 10;
		NumericSpecificityRank[14] = 11;
		ForLoopWidestTypeCode = new TypeCode[19][]
		{
			new TypeCode[19],
			new TypeCode[19],
			new TypeCode[19],
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int16,
				TypeCode.Empty,
				TypeCode.SByte,
				TypeCode.Int16,
				TypeCode.Int16,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Decimal,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19],
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.SByte,
				TypeCode.Empty,
				TypeCode.SByte,
				TypeCode.Int16,
				TypeCode.Int16,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Decimal,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int16,
				TypeCode.Empty,
				TypeCode.Int16,
				TypeCode.Byte,
				TypeCode.Int16,
				TypeCode.UInt16,
				TypeCode.Int32,
				TypeCode.UInt32,
				TypeCode.Int64,
				TypeCode.UInt64,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int16,
				TypeCode.Empty,
				TypeCode.Int16,
				TypeCode.Int16,
				TypeCode.Int16,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Decimal,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int32,
				TypeCode.Empty,
				TypeCode.Int32,
				TypeCode.UInt16,
				TypeCode.Int32,
				TypeCode.UInt16,
				TypeCode.Int32,
				TypeCode.UInt32,
				TypeCode.Int64,
				TypeCode.UInt64,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int32,
				TypeCode.Empty,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int32,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Decimal,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int64,
				TypeCode.Empty,
				TypeCode.Int64,
				TypeCode.UInt32,
				TypeCode.Int64,
				TypeCode.UInt32,
				TypeCode.Int64,
				TypeCode.UInt32,
				TypeCode.Int64,
				TypeCode.UInt64,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Int64,
				TypeCode.Empty,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Int64,
				TypeCode.Decimal,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Decimal,
				TypeCode.UInt64,
				TypeCode.Decimal,
				TypeCode.UInt64,
				TypeCode.Decimal,
				TypeCode.UInt64,
				TypeCode.Decimal,
				TypeCode.UInt64,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Single,
				TypeCode.Empty,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Single,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Double,
				TypeCode.Empty,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Double,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19]
			{
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Decimal,
				TypeCode.Single,
				TypeCode.Double,
				TypeCode.Decimal,
				TypeCode.Empty,
				TypeCode.Empty,
				TypeCode.Empty
			},
			new TypeCode[19],
			new TypeCode[19],
			new TypeCode[19]
		};
	}

	[RequiresUnreferencedCode("Calls ClassifyUserDefinedConversion and ClassifyPredefinedConversion")]
	internal static ConversionClass ClassifyConversion(Type targetType, Type sourceType, ref Symbols.Method operatorMethod)
	{
		ConversionClass conversionClass = ClassifyPredefinedConversion(targetType, sourceType);
		if (conversionClass == ConversionClass.None && !Symbols.IsInterface(sourceType) && !Symbols.IsInterface(targetType) && (Symbols.IsClassOrValueType(sourceType) || Symbols.IsClassOrValueType(targetType)) && (!Symbols.IsIntrinsicType(sourceType) || !Symbols.IsIntrinsicType(targetType)))
		{
			conversionClass = ClassifyUserDefinedConversion(targetType, sourceType, ref operatorMethod);
		}
		return conversionClass;
	}

	internal static ConversionClass ClassifyIntrinsicConversion(TypeCode targetTypeCode, TypeCode sourceTypeCode)
	{
		return s_conversionTable[(int)targetTypeCode][(int)sourceTypeCode];
	}

	[RequiresUnreferencedCode("Calls GetInterfaceConstraints but does so recursively on various types")]
	internal static ConversionClass ClassifyPredefinedCLRConversion(Type targetType, Type sourceType)
	{
		if ((object)targetType == sourceType)
		{
			return ConversionClass.Identity;
		}
		if (Symbols.IsRootObjectType(targetType) || Symbols.IsOrInheritsFrom(sourceType, targetType))
		{
			return ConversionClass.Widening;
		}
		if (Symbols.IsRootObjectType(sourceType) || Symbols.IsOrInheritsFrom(targetType, sourceType))
		{
			return ConversionClass.Narrowing;
		}
		if (Symbols.IsInterface(sourceType))
		{
			if (Symbols.IsClass(targetType) || Symbols.IsArrayType(targetType) || Symbols.IsGenericParameter(targetType))
			{
				return ConversionClass.Narrowing;
			}
			if (Symbols.IsInterface(targetType))
			{
				return ConversionClass.Narrowing;
			}
			if (Symbols.IsValueType(targetType))
			{
				if (Symbols.Implements(targetType, sourceType))
				{
					return ConversionClass.Narrowing;
				}
				return ConversionClass.None;
			}
			return ConversionClass.Narrowing;
		}
		if (Symbols.IsInterface(targetType))
		{
			if (Symbols.IsArrayType(sourceType))
			{
				return ClassifyCLRArrayToInterfaceConversion(targetType, sourceType);
			}
			if (Symbols.IsValueType(sourceType))
			{
				if (Symbols.Implements(sourceType, targetType))
				{
					return ConversionClass.Widening;
				}
				return ConversionClass.None;
			}
			if (Symbols.IsClass(sourceType))
			{
				if (Symbols.Implements(sourceType, targetType))
				{
					return ConversionClass.Widening;
				}
				return ConversionClass.Narrowing;
			}
		}
		if (Symbols.IsEnum(sourceType) || Symbols.IsEnum(targetType))
		{
			if (ReflectionExtensions.GetTypeCode(sourceType) == ReflectionExtensions.GetTypeCode(targetType))
			{
				if (Symbols.IsEnum(targetType))
				{
					return ConversionClass.Narrowing;
				}
				return ConversionClass.Widening;
			}
			return ConversionClass.None;
		}
		if (Symbols.IsGenericParameter(sourceType))
		{
			if (!Symbols.IsClassOrInterface(targetType))
			{
				return ConversionClass.None;
			}
			Type[] interfaceConstraints = Symbols.GetInterfaceConstraints(sourceType);
			foreach (Type sourceType2 in interfaceConstraints)
			{
				ConversionClass conversionClass = ClassifyPredefinedConversion(targetType, sourceType2);
				if (conversionClass == ConversionClass.Widening || conversionClass == ConversionClass.Identity)
				{
					return ConversionClass.Widening;
				}
			}
			Type classConstraint = Symbols.GetClassConstraint(sourceType);
			if ((object)classConstraint != null)
			{
				ConversionClass conversionClass2 = ClassifyPredefinedConversion(targetType, classConstraint);
				if (conversionClass2 == ConversionClass.Widening || conversionClass2 == ConversionClass.Identity)
				{
					return ConversionClass.Widening;
				}
			}
			return Interaction.IIf(Symbols.IsInterface(targetType), ConversionClass.Narrowing, ConversionClass.None);
		}
		if (Symbols.IsGenericParameter(targetType))
		{
			Type classConstraint2 = Symbols.GetClassConstraint(targetType);
			if ((object)classConstraint2 != null && Symbols.IsOrInheritsFrom(classConstraint2, sourceType))
			{
				return ConversionClass.Narrowing;
			}
			return ConversionClass.None;
		}
		if (Symbols.IsArrayType(sourceType) && Symbols.IsArrayType(targetType))
		{
			if (sourceType.GetArrayRank() == targetType.GetArrayRank())
			{
				return ClassifyCLRConversionForArrayElementTypes(targetType.GetElementType(), sourceType.GetElementType());
			}
			return ConversionClass.None;
		}
		return ConversionClass.None;
	}

	[RequiresUnreferencedCode("Calls ClassifyPredefinedCLRConversion")]
	private static ConversionClass ClassifyCLRArrayToInterfaceConversion(Type targetInterface, Type sourceArrayType)
	{
		if (Symbols.Implements(sourceArrayType, targetInterface))
		{
			return ConversionClass.Widening;
		}
		if (sourceArrayType.GetArrayRank() > 1)
		{
			return ConversionClass.Narrowing;
		}
		Type elementType = sourceArrayType.GetElementType();
		ConversionClass conversionClass = ConversionClass.None;
		if (targetInterface.IsGenericType && !targetInterface.IsGenericTypeDefinition)
		{
			Type genericTypeDefinition = targetInterface.GetGenericTypeDefinition();
			if ((object)genericTypeDefinition == typeof(IList<>) || (object)genericTypeDefinition == typeof(ICollection<>) || (object)genericTypeDefinition == typeof(IEnumerable<>))
			{
				conversionClass = ClassifyCLRConversionForArrayElementTypes(targetInterface.GetGenericArguments()[0], elementType);
			}
		}
		else
		{
			conversionClass = ClassifyPredefinedCLRConversion(targetInterface, typeof(IList<>).MakeGenericType(elementType));
		}
		if (conversionClass == ConversionClass.Identity || conversionClass == ConversionClass.Widening)
		{
			return ConversionClass.Widening;
		}
		return ConversionClass.Narrowing;
	}

	[RequiresUnreferencedCode("Calls ClassifyPredefinedCLRConversion")]
	private static ConversionClass ClassifyCLRConversionForArrayElementTypes(Type targetElementType, Type sourceElementType)
	{
		if (Symbols.IsReferenceType(sourceElementType) && Symbols.IsReferenceType(targetElementType))
		{
			return ClassifyPredefinedCLRConversion(targetElementType, sourceElementType);
		}
		if (Symbols.IsValueType(sourceElementType) && Symbols.IsValueType(targetElementType))
		{
			return ClassifyPredefinedCLRConversion(targetElementType, sourceElementType);
		}
		if (Symbols.IsGenericParameter(sourceElementType) && Symbols.IsGenericParameter(targetElementType))
		{
			if ((object)sourceElementType == targetElementType)
			{
				return ConversionClass.Identity;
			}
			if (Symbols.IsReferenceType(sourceElementType) && Symbols.IsOrInheritsFrom(sourceElementType, targetElementType))
			{
				return ConversionClass.Widening;
			}
			if (Symbols.IsReferenceType(targetElementType) && Symbols.IsOrInheritsFrom(targetElementType, sourceElementType))
			{
				return ConversionClass.Narrowing;
			}
		}
		return ConversionClass.None;
	}

	[RequiresUnreferencedCode("Calls ClassifyPredefinedCLRConversion")]
	internal static ConversionClass ClassifyPredefinedConversion(Type targetType, Type sourceType)
	{
		if ((object)targetType == sourceType)
		{
			return ConversionClass.Identity;
		}
		TypeCode typeCode = ReflectionExtensions.GetTypeCode(sourceType);
		TypeCode typeCode2 = ReflectionExtensions.GetTypeCode(targetType);
		if (Symbols.IsIntrinsicType(typeCode) && Symbols.IsIntrinsicType(typeCode2))
		{
			if (Symbols.IsEnum(targetType) && Symbols.IsIntegralType(typeCode) && Symbols.IsIntegralType(typeCode2))
			{
				return ConversionClass.Narrowing;
			}
			if (typeCode == typeCode2 && Symbols.IsEnum(sourceType))
			{
				return ConversionClass.Widening;
			}
			return ClassifyIntrinsicConversion(typeCode2, typeCode);
		}
		if (Symbols.IsCharArrayRankOne(sourceType) && Symbols.IsStringType(targetType))
		{
			return ConversionClass.Widening;
		}
		if (Symbols.IsCharArrayRankOne(targetType) && Symbols.IsStringType(sourceType))
		{
			return ConversionClass.Narrowing;
		}
		return ClassifyPredefinedCLRConversion(targetType, sourceType);
	}

	[RequiresUnreferencedCode("Calls Operators.CollectOperators")]
	private static List<Symbols.Method> CollectConversionOperators(Type targetType, Type sourceType, ref bool foundTargetTypeOperators, ref bool foundSourceTypeOperators)
	{
		if (Symbols.IsIntrinsicType(targetType))
		{
			targetType = typeof(object);
		}
		if (Symbols.IsIntrinsicType(sourceType))
		{
			sourceType = typeof(object);
		}
		List<Symbols.Method> list = Operators.CollectOperators(Symbols.UserDefinedOperator.Widen, targetType, sourceType, ref foundTargetTypeOperators, ref foundSourceTypeOperators);
		List<Symbols.Method> collection = Operators.CollectOperators(Symbols.UserDefinedOperator.Narrow, targetType, sourceType, ref foundTargetTypeOperators, ref foundSourceTypeOperators);
		list.AddRange(collection);
		return list;
	}

	[RequiresUnreferencedCode("Calls ClassifyPredefinedConversion")]
	private static bool Encompasses(Type larger, Type smaller)
	{
		ConversionClass conversionClass = ClassifyPredefinedConversion(larger, smaller);
		if (conversionClass != ConversionClass.Widening)
		{
			return conversionClass == ConversionClass.Identity;
		}
		return true;
	}

	[RequiresUnreferencedCode("Calls ClassifyPredefinedConversion")]
	private static bool NotEncompasses(Type larger, Type smaller)
	{
		ConversionClass conversionClass = ClassifyPredefinedConversion(larger, smaller);
		if (conversionClass != ConversionClass.Narrowing)
		{
			return conversionClass == ConversionClass.Identity;
		}
		return true;
	}

	[RequiresUnreferencedCode("Calls Encompasses")]
	private static Type MostEncompassing(List<Type> types)
	{
		Type type = types[0];
		checked
		{
			int num = types.Count - 1;
			for (int i = 1; i <= num; i++)
			{
				Type type2 = types[i];
				if (Encompasses(type2, type))
				{
					type = type2;
				}
				else if (!Encompasses(type, type2))
				{
					return null;
				}
			}
			return type;
		}
	}

	[RequiresUnreferencedCode("Calls Encompasses")]
	private static Type MostEncompassed(List<Type> types)
	{
		Type type = types[0];
		checked
		{
			int num = types.Count - 1;
			for (int i = 1; i <= num; i++)
			{
				Type type2 = types[i];
				if (Encompasses(type, type2))
				{
					type = type2;
				}
				else if (!Encompasses(type2, type))
				{
					return null;
				}
			}
			return type;
		}
	}

	private static void FindBestMatch(Type targetType, Type sourceType, List<Symbols.Method> searchList, List<Symbols.Method> resultList, ref bool genericMembersExistInList)
	{
		foreach (Symbols.Method search in searchList)
		{
			MethodBase methodBase = search.AsMethod();
			Type parameterType = methodBase.GetParameters()[0].ParameterType;
			Type returnType = ((MethodInfo)methodBase).ReturnType;
			if ((object)parameterType == sourceType && (object)returnType == targetType)
			{
				InsertInOperatorListIfLessGenericThanExisting(search, resultList, ref genericMembersExistInList);
			}
		}
	}

	private static void InsertInOperatorListIfLessGenericThanExisting(Symbols.Method operatorToInsert, List<Symbols.Method> operatorList, ref bool genericMembersExistInList)
	{
		if (Symbols.IsGeneric(operatorToInsert.DeclaringType))
		{
			genericMembersExistInList = true;
		}
		checked
		{
			if (genericMembersExistInList)
			{
				for (int i = operatorList.Count - 1; i >= 0; i += -1)
				{
					Symbols.Method method = operatorList[i];
					Symbols.Method method2 = OverloadResolution.LeastGenericProcedure(method, operatorToInsert);
					if ((object)method2 != method)
					{
						if ((object)method2 != null)
						{
							operatorList.Remove(method);
						}
						continue;
					}
					return;
				}
			}
			operatorList.Add(operatorToInsert);
		}
	}

	[RequiresUnreferencedCode("Calls ClassifyPredefinedConversion")]
	private static List<Symbols.Method> ResolveConversion(Type targetType, Type sourceType, List<Symbols.Method> operatorSet, bool wideningOnly, ref bool resolutionIsAmbiguous)
	{
		resolutionIsAmbiguous = false;
		Type type = null;
		Type type2 = null;
		bool genericMembersExistInList = false;
		List<Symbols.Method> list = new List<Symbols.Method>(operatorSet.Count);
		List<Symbols.Method> list2 = new List<Symbols.Method>(operatorSet.Count);
		List<Type> list3 = new List<Type>(operatorSet.Count);
		List<Type> list4 = new List<Type>(operatorSet.Count);
		List<Type> list5 = null;
		List<Type> list6 = null;
		if (!wideningOnly)
		{
			list5 = new List<Type>(operatorSet.Count);
			list6 = new List<Type>(operatorSet.Count);
		}
		foreach (Symbols.Method item in operatorSet)
		{
			MethodBase methodBase = item.AsMethod();
			if (wideningOnly && Symbols.IsNarrowingConversionOperator(methodBase))
			{
				break;
			}
			Type parameterType = methodBase.GetParameters()[0].ParameterType;
			Type returnType = ((MethodInfo)methodBase).ReturnType;
			if ((Symbols.IsGeneric(methodBase) || Symbols.IsGeneric(methodBase.DeclaringType)) && ClassifyPredefinedConversion(returnType, parameterType) != ConversionClass.None)
			{
				continue;
			}
			if ((object)parameterType == sourceType && (object)returnType == targetType)
			{
				InsertInOperatorListIfLessGenericThanExisting(item, list, ref genericMembersExistInList);
			}
			else
			{
				if (list.Count != 0)
				{
					continue;
				}
				if (Encompasses(parameterType, sourceType) && Encompasses(targetType, returnType))
				{
					list2.Add(item);
					if ((object)parameterType == sourceType)
					{
						type = parameterType;
					}
					else
					{
						list3.Add(parameterType);
					}
					if ((object)returnType == targetType)
					{
						type2 = returnType;
					}
					else
					{
						list4.Add(returnType);
					}
				}
				else if (!wideningOnly && Encompasses(parameterType, sourceType) && NotEncompasses(targetType, returnType))
				{
					list2.Add(item);
					if ((object)parameterType == sourceType)
					{
						type = parameterType;
					}
					else
					{
						list3.Add(parameterType);
					}
					if ((object)returnType == targetType)
					{
						type2 = returnType;
					}
					else
					{
						list6.Add(returnType);
					}
				}
				else if (!wideningOnly && NotEncompasses(parameterType, sourceType) && NotEncompasses(targetType, returnType))
				{
					list2.Add(item);
					if ((object)parameterType == sourceType)
					{
						type = parameterType;
					}
					else
					{
						list5.Add(parameterType);
					}
					if ((object)returnType == targetType)
					{
						type2 = returnType;
					}
					else
					{
						list6.Add(returnType);
					}
				}
			}
		}
		if (list.Count == 0 && list2.Count > 0)
		{
			if ((object)type == null)
			{
				type = ((list3.Count <= 0) ? MostEncompassing(list5) : MostEncompassed(list3));
			}
			if ((object)type2 == null)
			{
				type2 = ((list4.Count <= 0) ? MostEncompassed(list6) : MostEncompassing(list4));
			}
			if ((object)type == null || (object)type2 == null)
			{
				resolutionIsAmbiguous = true;
				return new List<Symbols.Method>();
			}
			FindBestMatch(type2, type, list2, list, ref genericMembersExistInList);
		}
		if (list.Count > 1)
		{
			resolutionIsAmbiguous = true;
		}
		return list;
	}

	[RequiresUnreferencedCode("Calls DoClassifyUserDefinedConversion")]
	internal static ConversionClass ClassifyUserDefinedConversion(Type targetType, Type sourceType, ref Symbols.Method operatorMethod)
	{
		ConversionClass classification = default(ConversionClass);
		lock (OperatorCaches.ConversionCache)
		{
			if (OperatorCaches.UnconvertibleTypeCache.Lookup(targetType) && OperatorCaches.UnconvertibleTypeCache.Lookup(sourceType))
			{
				return ConversionClass.None;
			}
			if (OperatorCaches.ConversionCache.Lookup(targetType, sourceType, ref classification, ref operatorMethod))
			{
				return classification;
			}
		}
		bool foundTargetTypeOperators = false;
		bool foundSourceTypeOperators = false;
		classification = DoClassifyUserDefinedConversion(targetType, sourceType, ref operatorMethod, ref foundTargetTypeOperators, ref foundSourceTypeOperators);
		lock (OperatorCaches.ConversionCache)
		{
			if (!foundTargetTypeOperators)
			{
				OperatorCaches.UnconvertibleTypeCache.Insert(targetType);
			}
			if (!foundSourceTypeOperators)
			{
				OperatorCaches.UnconvertibleTypeCache.Insert(sourceType);
			}
			if (foundTargetTypeOperators || foundSourceTypeOperators)
			{
				OperatorCaches.ConversionCache.Insert(targetType, sourceType, classification, operatorMethod);
			}
		}
		return classification;
	}

	[RequiresUnreferencedCode("Calls CollectConversionOperators")]
	private static ConversionClass DoClassifyUserDefinedConversion(Type targetType, Type sourceType, ref Symbols.Method operatorMethod, ref bool foundTargetTypeOperators, ref bool foundSourceTypeOperators)
	{
		operatorMethod = null;
		List<Symbols.Method> list = CollectConversionOperators(targetType, sourceType, ref foundTargetTypeOperators, ref foundSourceTypeOperators);
		if (list.Count == 0)
		{
			return ConversionClass.None;
		}
		bool resolutionIsAmbiguous = false;
		List<Symbols.Method> list2 = ResolveConversion(targetType, sourceType, list, wideningOnly: true, ref resolutionIsAmbiguous);
		if (list2.Count == 1)
		{
			operatorMethod = list2[0];
			operatorMethod.ArgumentsValidated = true;
			return ConversionClass.Widening;
		}
		if (list2.Count == 0 && !resolutionIsAmbiguous)
		{
			list2 = ResolveConversion(targetType, sourceType, list, wideningOnly: false, ref resolutionIsAmbiguous);
			if (list2.Count == 1)
			{
				operatorMethod = list2[0];
				operatorMethod.ArgumentsValidated = true;
				return ConversionClass.Narrowing;
			}
			if (list2.Count == 0)
			{
				return ConversionClass.None;
			}
		}
		return ConversionClass.Ambiguous;
	}
}

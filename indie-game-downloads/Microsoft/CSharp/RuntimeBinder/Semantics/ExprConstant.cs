using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprConstant : ExprWithType
{
	public Expr OptionalConstructorCall { get; set; }

	public bool IsZero => Val.IsZero(base.Type.ConstValKind);

	public ConstVal Val { get; }

	public ulong UInt64Value => Val.UInt64Val;

	public long Int64Value
	{
		get
		{
			switch (base.Type.FundamentalType)
			{
			case FUNDTYPE.FT_I8:
			case FUNDTYPE.FT_U8:
				return Val.Int64Val;
			case FUNDTYPE.FT_U4:
				return Val.UInt32Val;
			default:
				return Val.Int32Val;
			}
		}
	}

	public override object Object
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			if (base.Type is NullType)
			{
				return null;
			}
			object obj = System.Type.GetTypeCode(base.Type.AssociatedSystemType) switch
			{
				TypeCode.Boolean => Val.BooleanVal, 
				TypeCode.SByte => Val.SByteVal, 
				TypeCode.Byte => Val.ByteVal, 
				TypeCode.Int16 => Val.Int16Val, 
				TypeCode.UInt16 => Val.UInt16Val, 
				TypeCode.Int32 => Val.Int32Val, 
				TypeCode.UInt32 => Val.UInt32Val, 
				TypeCode.Int64 => Val.Int64Val, 
				TypeCode.UInt64 => Val.UInt64Val, 
				TypeCode.Single => Val.SingleVal, 
				TypeCode.Double => Val.DoubleVal, 
				TypeCode.Decimal => Val.DecimalVal, 
				TypeCode.Char => Val.CharVal, 
				TypeCode.String => Val.StringVal, 
				_ => Val.ObjectVal, 
			};
			if (!base.Type.IsEnumType)
			{
				return obj;
			}
			return Enum.ToObject(base.Type.AssociatedSystemType, obj);
		}
	}

	public ExprConstant(CType type, ConstVal value)
		: base(ExpressionKind.Constant, type)
	{
		Val = value;
	}
}

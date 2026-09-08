using System;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal static class ErrorFacts
{
	public static string GetMessage(ErrorCode code)
	{
		return code switch
		{
			ErrorCode.ERR_BadBinaryOps => System.SR.BadBinaryOps, 
			ErrorCode.ERR_BadIndexLHS => System.SR.BadIndexLHS, 
			ErrorCode.ERR_BadIndexCount => System.SR.BadIndexCount, 
			ErrorCode.ERR_BadUnaryOp => System.SR.BadUnaryOp, 
			ErrorCode.ERR_NoImplicitConv => System.SR.NoImplicitConv, 
			ErrorCode.ERR_NoExplicitConv => System.SR.NoExplicitConv, 
			ErrorCode.ERR_ConstOutOfRange => System.SR.ConstOutOfRange, 
			ErrorCode.ERR_AmbigBinaryOps => System.SR.AmbigBinaryOps, 
			ErrorCode.ERR_AmbigUnaryOp => System.SR.AmbigUnaryOp, 
			ErrorCode.ERR_ValueCantBeNull => System.SR.ValueCantBeNull, 
			ErrorCode.ERR_NoSuchMember => System.SR.NoSuchMember, 
			ErrorCode.ERR_ObjectRequired => System.SR.ObjectRequired, 
			ErrorCode.ERR_AmbigCall => System.SR.AmbigCall, 
			ErrorCode.ERR_BadAccess => System.SR.BadAccess, 
			ErrorCode.ERR_AssgLvalueExpected => System.SR.AssgLvalueExpected, 
			ErrorCode.ERR_NoConstructors => System.SR.NoConstructors, 
			ErrorCode.ERR_PropertyLacksGet => System.SR.PropertyLacksGet, 
			ErrorCode.ERR_ObjectProhibited => System.SR.ObjectProhibited, 
			ErrorCode.ERR_AssgReadonly => System.SR.AssgReadonly, 
			ErrorCode.ERR_AssgReadonlyStatic => System.SR.AssgReadonlyStatic, 
			ErrorCode.ERR_AssgReadonlyProp => System.SR.AssgReadonlyProp, 
			ErrorCode.ERR_UnsafeNeeded => System.SR.UnsafeNeeded, 
			ErrorCode.ERR_BadBoolOp => System.SR.BadBoolOp, 
			ErrorCode.ERR_MustHaveOpTF => System.SR.MustHaveOpTF, 
			ErrorCode.ERR_ConstOutOfRangeChecked => System.SR.ConstOutOfRangeChecked, 
			ErrorCode.ERR_AmbigMember => System.SR.AmbigMember, 
			ErrorCode.ERR_NoImplicitConvCast => System.SR.NoImplicitConvCast, 
			ErrorCode.ERR_InaccessibleGetter => System.SR.InaccessibleGetter, 
			ErrorCode.ERR_InaccessibleSetter => System.SR.InaccessibleSetter, 
			ErrorCode.ERR_BadArity => System.SR.BadArity, 
			ErrorCode.ERR_TypeArgsNotAllowed => System.SR.TypeArgsNotAllowed, 
			ErrorCode.ERR_HasNoTypeVars => System.SR.HasNoTypeVars, 
			ErrorCode.ERR_NewConstraintNotSatisfied => System.SR.NewConstraintNotSatisfied, 
			ErrorCode.ERR_GenericConstraintNotSatisfiedRefType => System.SR.GenericConstraintNotSatisfiedRefType, 
			ErrorCode.ERR_GenericConstraintNotSatisfiedNullableEnum => System.SR.GenericConstraintNotSatisfiedNullableEnum, 
			ErrorCode.ERR_GenericConstraintNotSatisfiedNullableInterface => System.SR.GenericConstraintNotSatisfiedNullableInterface, 
			ErrorCode.ERR_GenericConstraintNotSatisfiedValType => System.SR.GenericConstraintNotSatisfiedValType, 
			ErrorCode.ERR_CantInferMethTypeArgs => System.SR.CantInferMethTypeArgs, 
			ErrorCode.ERR_RefConstraintNotSatisfied => System.SR.RefConstraintNotSatisfied, 
			ErrorCode.ERR_ValConstraintNotSatisfied => System.SR.ValConstraintNotSatisfied, 
			ErrorCode.ERR_AmbigUDConv => System.SR.AmbigUDConv, 
			ErrorCode.ERR_BindToBogus => System.SR.BindToBogus, 
			ErrorCode.ERR_CantCallSpecialMethod => System.SR.CantCallSpecialMethod, 
			ErrorCode.ERR_ConvertToStaticClass => System.SR.ConvertToStaticClass, 
			ErrorCode.ERR_IncrementLvalueExpected => System.SR.IncrementLvalueExpected, 
			ErrorCode.ERR_BadArgCount => System.SR.BadArgCount, 
			ErrorCode.ERR_BadArgTypes => System.SR.BadArgTypes, 
			ErrorCode.ERR_BadProtectedAccess => System.SR.BadProtectedAccess, 
			ErrorCode.ERR_BindToBogusProp2 => System.SR.BindToBogusProp2, 
			ErrorCode.ERR_BindToBogusProp1 => System.SR.BindToBogusProp1, 
			ErrorCode.ERR_BadDelArgCount => System.SR.BadDelArgCount, 
			ErrorCode.ERR_BadDelArgTypes => System.SR.BadDelArgTypes, 
			ErrorCode.ERR_BadCtorArgCount => System.SR.BadCtorArgCount, 
			ErrorCode.ERR_NonInvocableMemberCalled => System.SR.NonInvocableMemberCalled, 
			ErrorCode.ERR_BadNamedArgument => System.SR.BadNamedArgument, 
			ErrorCode.ERR_BadNamedArgumentForDelegateInvoke => System.SR.BadNamedArgumentForDelegateInvoke, 
			ErrorCode.ERR_DuplicateNamedArgument => System.SR.DuplicateNamedArgument, 
			ErrorCode.ERR_NamedArgumentUsedInPositional => System.SR.NamedArgumentUsedInPositional, 
			ErrorCode.ERR_BadNonTrailingNamedArgument => System.SR.BadNonTrailingNamedArgument, 
			ErrorCode.ERR_DynamicBindingComUnsupported => System.SR.DynamicBindingComUnsupported, 
			_ => null, 
		};
	}

	public static string GetMessage(MessageID id)
	{
		string text = id.ToString();
		return System.SR.GetResourceString(text, text);
	}
}

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ObjectFlowControl
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ForLoopControl
	{
		private object _counter;

		private object _limit;

		private object _stepValue;

		private bool _positiveStep;

		private Type _enumType;

		private Type _widestType;

		private TypeCode _widestTypeCode;

		private bool _useUserDefinedOperators;

		private Symbols.Method _operatorPlus;

		private Symbols.Method _operatorGreaterEqual;

		private Symbols.Method _operatorLessEqual;

		private ForLoopControl()
		{
		}

		[RequiresUnreferencedCode("Calls ClassifyConversion")]
		private static Type GetWidestType(Type type1, Type type2)
		{
			if ((object)type1 == null || (object)type2 == null)
			{
				return null;
			}
			if (!type1.IsEnum && !type2.IsEnum)
			{
				TypeCode typeCode = ReflectionExtensions.GetTypeCode(type1);
				TypeCode typeCode2 = ReflectionExtensions.GetTypeCode(type2);
				if (Symbols.IsNumericType(typeCode) && Symbols.IsNumericType(typeCode2))
				{
					return Symbols.MapTypeCodeToType(ConversionResolution.ForLoopWidestTypeCode[(int)typeCode][(int)typeCode2]);
				}
			}
			Symbols.Method operatorMethod = null;
			ConversionResolution.ConversionClass conversionClass = ConversionResolution.ClassifyConversion(type2, type1, ref operatorMethod);
			if (conversionClass == ConversionResolution.ConversionClass.Identity || conversionClass == ConversionResolution.ConversionClass.Widening)
			{
				return type2;
			}
			operatorMethod = null;
			if (ConversionResolution.ClassifyConversion(type1, type2, ref operatorMethod) == ConversionResolution.ConversionClass.Widening)
			{
				return type1;
			}
			return null;
		}

		[RequiresUnreferencedCode("Calls GetWidestType")]
		private static Type GetWidestType(Type type1, Type type2, Type type3)
		{
			return GetWidestType(type1, GetWidestType(type2, type3));
		}

		[RequiresUnreferencedCode("Calls Conversions.ChangeType")]
		private static object ConvertLoopElement(string elementName, object value, Type sourceType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type targetType)
		{
			try
			{
				return Conversions.ChangeType(value, targetType);
			}
			catch (OutOfMemoryException ex)
			{
				throw ex;
			}
			catch (Exception)
			{
				throw new ArgumentException(System.SR.Format(System.SR.ForLoop_ConvertToType3, elementName, Utils.VBFriendlyName(sourceType), Utils.VBFriendlyName(targetType)));
			}
		}

		[RequiresUnreferencedCode("Calls Operators.GetCallableUserDefinedOperator")]
		private static Symbols.Method VerifyForLoopOperator(Symbols.UserDefinedOperator op, object forLoopArgument, Type forLoopArgumentType)
		{
			Symbols.Method callableUserDefinedOperator = Operators.GetCallableUserDefinedOperator(op, forLoopArgument, forLoopArgument);
			if ((object)callableUserDefinedOperator == null)
			{
				throw new ArgumentException(System.SR.Format(System.SR.ForLoop_OperatorRequired2, Utils.VBFriendlyNameOfType(forLoopArgumentType, fullName: true), Symbols.OperatorNames[(int)op]));
			}
			MethodInfo methodInfo = callableUserDefinedOperator.AsMethod() as MethodInfo;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			switch (op)
			{
			case Symbols.UserDefinedOperator.Plus:
			case Symbols.UserDefinedOperator.Minus:
				if (parameters.Length != 2 || (object)parameters[0].ParameterType != forLoopArgumentType || (object)parameters[1].ParameterType != forLoopArgumentType || (object)methodInfo.ReturnType != forLoopArgumentType)
				{
					throw new ArgumentException(System.SR.Format(System.SR.ForLoop_UnacceptableOperator2, callableUserDefinedOperator.ToString(), Utils.VBFriendlyNameOfType(forLoopArgumentType, fullName: true)));
				}
				break;
			case Symbols.UserDefinedOperator.LessEqual:
			case Symbols.UserDefinedOperator.GreaterEqual:
				if (parameters.Length != 2 || (object)parameters[0].ParameterType != forLoopArgumentType || (object)parameters[1].ParameterType != forLoopArgumentType)
				{
					throw new ArgumentException(System.SR.Format(System.SR.ForLoop_UnacceptableRelOperator2, callableUserDefinedOperator.ToString(), Utils.VBFriendlyNameOfType(forLoopArgumentType, fullName: true)));
				}
				break;
			}
			return callableUserDefinedOperator;
		}

		[RequiresUnreferencedCode("The types of the parameters cannot be statically analyzed and may be trimmed")]
		public static bool ForLoopInitObj(object Counter, object Start, object Limit, object StepValue, ref object LoopForResult, ref object CounterResult)
		{
			if (Start == null)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Start"));
			}
			if (Limit == null)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Limit"));
			}
			if (StepValue == null)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Step"));
			}
			Type type = Start.GetType();
			Type type2 = Limit.GetType();
			Type type3 = StepValue.GetType();
			Type widestType = GetWidestType(type3, type, type2);
			if ((object)widestType == null)
			{
				throw new ArgumentException(System.SR.Format(System.SR.ForLoop_CommonType3, Utils.VBFriendlyName(type), Utils.VBFriendlyName(type2), Utils.VBFriendlyName(StepValue)));
			}
			ForLoopControl forLoopControl = new ForLoopControl();
			TypeCode typeCode = ReflectionExtensions.GetTypeCode(widestType);
			if (typeCode == TypeCode.Object)
			{
				forLoopControl._useUserDefinedOperators = true;
			}
			if (typeCode == TypeCode.String)
			{
				typeCode = TypeCode.Double;
			}
			TypeCode typeCode2 = ReflectionExtensions.GetTypeCode(type);
			TypeCode typeCode3 = ReflectionExtensions.GetTypeCode(type2);
			TypeCode typeCode4 = ReflectionExtensions.GetTypeCode(type3);
			Type type4 = null;
			if (typeCode2 == typeCode && type.IsEnum)
			{
				type4 = type;
			}
			if (typeCode3 == typeCode && type2.IsEnum)
			{
				if ((object)type4 != null && (object)type4 != type2)
				{
					type4 = null;
					goto IL_0118;
				}
				type4 = type2;
			}
			if (typeCode4 == typeCode && type3.IsEnum)
			{
				type4 = (((object)type4 == null || (object)type4 == type3) ? type3 : null);
			}
			goto IL_0118;
			IL_0118:
			forLoopControl._enumType = type4;
			if (!forLoopControl._useUserDefinedOperators)
			{
				forLoopControl._widestType = Symbols.MapTypeCodeToType(typeCode);
			}
			else
			{
				forLoopControl._widestType = widestType;
			}
			forLoopControl._widestTypeCode = typeCode;
			forLoopControl._counter = ConvertLoopElement("Start", Start, type, forLoopControl._widestType);
			forLoopControl._limit = ConvertLoopElement("Limit", Limit, type2, forLoopControl._widestType);
			forLoopControl._stepValue = ConvertLoopElement("Step", StepValue, type3, forLoopControl._widestType);
			if (forLoopControl._useUserDefinedOperators)
			{
				forLoopControl._operatorPlus = VerifyForLoopOperator(Symbols.UserDefinedOperator.Plus, forLoopControl._counter, forLoopControl._widestType);
				VerifyForLoopOperator(Symbols.UserDefinedOperator.Minus, forLoopControl._counter, forLoopControl._widestType);
				forLoopControl._operatorLessEqual = VerifyForLoopOperator(Symbols.UserDefinedOperator.LessEqual, forLoopControl._counter, forLoopControl._widestType);
				forLoopControl._operatorGreaterEqual = VerifyForLoopOperator(Symbols.UserDefinedOperator.GreaterEqual, forLoopControl._counter, forLoopControl._widestType);
			}
			forLoopControl._positiveStep = Operators.ConditionalCompareObjectGreaterEqual(forLoopControl._stepValue, Operators.SubtractObject(forLoopControl._stepValue, forLoopControl._stepValue), TextCompare: false);
			LoopForResult = forLoopControl;
			if ((object)forLoopControl._enumType != null)
			{
				CounterResult = Enum.ToObject(forLoopControl._enumType, forLoopControl._counter);
			}
			else
			{
				CounterResult = forLoopControl._counter;
			}
			return CheckContinueLoop(forLoopControl);
		}

		[RequiresUnreferencedCode("The types of the parameters cannot be statically analyzed and may be trimmed")]
		public static bool ForNextCheckObj(object Counter, object LoopObj, ref object CounterResult)
		{
			if (LoopObj == null)
			{
				throw ExceptionUtils.VbMakeIllegalForException();
			}
			if (Counter == null)
			{
				throw new NullReferenceException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Counter"));
			}
			ForLoopControl forLoopControl = (ForLoopControl)LoopObj;
			bool flag = false;
			if (!forLoopControl._useUserDefinedOperators)
			{
				TypeCode typeCode = ReflectionExtensions.GetTypeCode(Counter.GetType());
				if (typeCode != forLoopControl._widestTypeCode || typeCode == TypeCode.String)
				{
					if (typeCode == TypeCode.Object)
					{
						throw new ArgumentException(System.SR.Format(System.SR.ForLoop_CommonType2, Utils.VBFriendlyName(Symbols.MapTypeCodeToType(typeCode)), Utils.VBFriendlyName(forLoopControl._widestType)));
					}
					TypeCode typeCode2 = ReflectionExtensions.GetTypeCode(GetWidestType(Symbols.MapTypeCodeToType(typeCode), forLoopControl._widestType));
					if (typeCode2 == TypeCode.String)
					{
						typeCode2 = TypeCode.Double;
					}
					forLoopControl._widestTypeCode = typeCode2;
					forLoopControl._widestType = Symbols.MapTypeCodeToType(typeCode2);
					flag = true;
				}
			}
			if (flag || forLoopControl._useUserDefinedOperators)
			{
				Counter = ConvertLoopElement("Start", Counter, Counter.GetType(), forLoopControl._widestType);
				if (!forLoopControl._useUserDefinedOperators)
				{
					forLoopControl._limit = ConvertLoopElement("Limit", forLoopControl._limit, forLoopControl._limit.GetType(), forLoopControl._widestType);
					forLoopControl._stepValue = ConvertLoopElement("Step", forLoopControl._stepValue, forLoopControl._stepValue.GetType(), forLoopControl._widestType);
				}
			}
			if (!forLoopControl._useUserDefinedOperators)
			{
				forLoopControl._counter = Operators.AddObject(Counter, forLoopControl._stepValue);
				TypeCode typeCode3 = ReflectionExtensions.GetTypeCode(forLoopControl._counter.GetType());
				if ((object)forLoopControl._enumType != null)
				{
					CounterResult = Enum.ToObject(forLoopControl._enumType, forLoopControl._counter);
				}
				else
				{
					CounterResult = forLoopControl._counter;
				}
				if (typeCode3 != forLoopControl._widestTypeCode)
				{
					forLoopControl._limit = Conversions.ChangeType(forLoopControl._limit, Symbols.MapTypeCodeToType(typeCode3));
					forLoopControl._stepValue = Conversions.ChangeType(forLoopControl._stepValue, Symbols.MapTypeCodeToType(typeCode3));
					return false;
				}
			}
			else
			{
				forLoopControl._counter = Operators.InvokeUserDefinedOperator(forLoopControl._operatorPlus, true, Counter, forLoopControl._stepValue);
				if ((object)forLoopControl._counter.GetType() != forLoopControl._widestType)
				{
					forLoopControl._counter = ConvertLoopElement("Start", forLoopControl._counter, forLoopControl._counter.GetType(), forLoopControl._widestType);
				}
				CounterResult = forLoopControl._counter;
			}
			return CheckContinueLoop(forLoopControl);
		}

		public static bool ForNextCheckR4(float count, float limit, float StepValue)
		{
			if (StepValue >= 0f)
			{
				return count <= limit;
			}
			return count >= limit;
		}

		public static bool ForNextCheckR8(double count, double limit, double StepValue)
		{
			if (StepValue >= 0.0)
			{
				return count <= limit;
			}
			return count >= limit;
		}

		public static bool ForNextCheckDec(decimal count, decimal limit, decimal StepValue)
		{
			if (decimal.Compare(StepValue, 0m) >= 0)
			{
				return decimal.Compare(count, limit) <= 0;
			}
			return decimal.Compare(count, limit) >= 0;
		}

		[RequiresUnreferencedCode("Calls Operators.InvokeUserDefinedOperator")]
		private static bool CheckContinueLoop(ForLoopControl loopFor)
		{
			if (!loopFor._useUserDefinedOperators)
			{
				try
				{
					int num = ((IComparable)loopFor._counter).CompareTo(loopFor._limit);
					if (loopFor._positiveStep)
					{
						return num <= 0;
					}
					return num >= 0;
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(System.SR.Format(System.SR.Argument_IComparable2, "loop control variable", Utils.VBFriendlyName(loopFor._counter)));
				}
			}
			if (loopFor._positiveStep)
			{
				return Conversions.ToBoolean(Operators.InvokeUserDefinedOperator(loopFor._operatorLessEqual, true, loopFor._counter, loopFor._limit));
			}
			return Conversions.ToBoolean(Operators.InvokeUserDefinedOperator(loopFor._operatorGreaterEqual, true, loopFor._counter, loopFor._limit));
		}
	}

	public static void CheckForSyncLockOnValueType(object Expression)
	{
		if (Expression != null && Expression.GetType().IsValueType)
		{
			throw new ArgumentException(System.SR.Format(System.SR.SyncLockRequiresReferenceType1, Utils.VBFriendlyName(Expression.GetType())));
		}
	}
}

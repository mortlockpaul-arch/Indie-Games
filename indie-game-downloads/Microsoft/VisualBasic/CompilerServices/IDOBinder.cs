using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class IDOBinder
{
	private struct SaveCopyBack : IDisposable
	{
		[ThreadStatic]
		private static bool[] s_savedCopyBack;

		private bool[] _oldCopyBack;

		public SaveCopyBack(bool[] copyBack)
		{
			this = default(SaveCopyBack);
			_oldCopyBack = s_savedCopyBack;
			s_savedCopyBack = copyBack;
		}

		public void Dispose()
		{
			s_savedCopyBack = _oldCopyBack;
		}

		void IDisposable.Dispose()
		{
			//ILSpy generated this explicit interface implementation from .override directive in Dispose
			this.Dispose();
		}

		internal static bool[] GetCopyBack()
		{
			return s_savedCopyBack;
		}
	}

	internal static readonly object missingMemberSentinel = new object();

	internal static bool[] GetCopyBack()
	{
		return SaveCopyBack.GetCopyBack();
	}

	[RequiresUnreferencedCode("Calls VBCallBinder.ctor")]
	internal static object IDOCall(IDynamicMetaObjectProvider instance, string memberName, object[] arguments, string[] argumentNames, bool[] copyBack, bool ignoreReturn)
	{
		using (new SaveCopyBack(copyBack))
		{
			CallInfo callInfo = null;
			object[] packedArgs = null;
			IDOUtils.PackArguments(0, argumentNames, arguments, ref packedArgs, ref callInfo);
			try
			{
				return IDOUtils.CreateRefCallSiteAndInvoke(new VBCallBinder(memberName, callInfo, ignoreReturn), instance, packedArgs);
			}
			finally
			{
				IDOUtils.CopyBackArguments(callInfo, packedArgs, arguments);
			}
		}
	}

	[RequiresUnreferencedCode("Calls VBGetBinder.ctor")]
	internal static object IDOGet(IDynamicMetaObjectProvider instance, string memberName, object[] arguments, string[] argumentNames, bool[] copyBack)
	{
		using (new SaveCopyBack(copyBack))
		{
			object[] packedArgs = null;
			CallInfo callInfo = null;
			IDOUtils.PackArguments(0, argumentNames, arguments, ref packedArgs, ref callInfo);
			try
			{
				return IDOUtils.CreateRefCallSiteAndInvoke(new VBGetBinder(memberName, callInfo), instance, packedArgs);
			}
			finally
			{
				IDOUtils.CopyBackArguments(callInfo, packedArgs, arguments);
			}
		}
	}

	[RequiresUnreferencedCode("Calls IDOUtils.CreateRefCallSiteAndInvoke")]
	internal static object IDOInvokeDefault(IDynamicMetaObjectProvider instance, object[] arguments, string[] argumentNames, bool reportErrors, bool[] copyBack)
	{
		using (new SaveCopyBack(copyBack))
		{
			object[] packedArgs = null;
			CallInfo callInfo = null;
			IDOUtils.PackArguments(0, argumentNames, arguments, ref packedArgs, ref callInfo);
			try
			{
				return IDOUtils.CreateRefCallSiteAndInvoke(new VBInvokeDefaultBinder(callInfo, reportErrors), instance, packedArgs);
			}
			finally
			{
				IDOUtils.CopyBackArguments(callInfo, packedArgs, arguments);
			}
		}
	}

	[RequiresUnreferencedCode("Calls VBInvokeDefaultFallbackBinder.ctor")]
	internal static object IDOFallbackInvokeDefault(IDynamicMetaObjectProvider instance, object[] arguments, string[] argumentNames, bool reportErrors, bool[] copyBack)
	{
		using (new SaveCopyBack(copyBack))
		{
			object[] packedArgs = null;
			CallInfo callInfo = null;
			IDOUtils.PackArguments(0, argumentNames, arguments, ref packedArgs, ref callInfo);
			try
			{
				return IDOUtils.CreateRefCallSiteAndInvoke(new VBInvokeDefaultFallbackBinder(callInfo, reportErrors), instance, packedArgs);
			}
			finally
			{
				IDOUtils.CopyBackArguments(callInfo, packedArgs, arguments);
			}
		}
	}

	[RequiresUnreferencedCode("Calls LateIndexSet")]
	internal static void IDOSet(IDynamicMetaObjectProvider instance, string memberName, string[] argumentNames, object[] arguments)
	{
		using (new SaveCopyBack(null))
		{
			if (arguments.Length == 1)
			{
				IDOUtils.CreateFuncCallSiteAndInvoke(new VBSetBinder(memberName), instance, arguments);
				return;
			}
			object obj = IDOUtils.CreateFuncCallSiteAndInvoke(new VBGetMemberBinder(memberName), instance, Symbols.NoArguments);
			if (obj == missingMemberSentinel)
			{
				NewLateBinding.ObjectLateSet(instance, null, memberName, arguments, argumentNames, Symbols.NoTypeArguments);
			}
			else
			{
				NewLateBinding.LateIndexSet(obj, arguments, argumentNames);
			}
		}
	}

	[RequiresUnreferencedCode("Calls LateIndexSetComplex")]
	internal static void IDOSetComplex(IDynamicMetaObjectProvider instance, string memberName, object[] arguments, string[] argumentNames, bool optimisticSet, bool rValueBase)
	{
		using (new SaveCopyBack(null))
		{
			if (arguments.Length == 1)
			{
				IDOUtils.CreateFuncCallSiteAndInvoke(new VBSetComplexBinder(memberName, optimisticSet, rValueBase), instance, arguments);
				return;
			}
			object obj = IDOUtils.CreateFuncCallSiteAndInvoke(new VBGetMemberBinder(memberName), instance, Symbols.NoArguments);
			if (obj == missingMemberSentinel)
			{
				NewLateBinding.ObjectLateSetComplex(instance, null, memberName, arguments, argumentNames, Symbols.NoTypeArguments, optimisticSet, rValueBase);
			}
			else
			{
				NewLateBinding.LateIndexSetComplex(obj, arguments, argumentNames, optimisticSet, rValueBase);
			}
		}
	}

	[RequiresUnreferencedCode("Calls IDOUtils.CreateFuncCallSiteAndInvoke")]
	internal static void IDOIndexSet(IDynamicMetaObjectProvider instance, object[] arguments, string[] argumentNames)
	{
		using (new SaveCopyBack(null))
		{
			object[] packedArgs = null;
			CallInfo callInfo = null;
			IDOUtils.PackArguments(1, argumentNames, arguments, ref packedArgs, ref callInfo);
			IDOUtils.CreateFuncCallSiteAndInvoke(new VBIndexSetBinder(callInfo), instance, packedArgs);
		}
	}

	[RequiresUnreferencedCode("Calls IDOUtils.CreateFuncCallSiteAndInvoke")]
	internal static void IDOIndexSetComplex(IDynamicMetaObjectProvider instance, object[] arguments, string[] argumentNames, bool optimisticSet, bool rValueBase)
	{
		using (new SaveCopyBack(null))
		{
			object[] packedArgs = null;
			CallInfo callInfo = null;
			IDOUtils.PackArguments(1, argumentNames, arguments, ref packedArgs, ref callInfo);
			IDOUtils.CreateFuncCallSiteAndInvoke(new VBIndexSetComplexBinder(callInfo, optimisticSet, rValueBase), instance, packedArgs);
		}
	}

	[RequiresUnreferencedCode("Calls IDOUtils.CreateConvertCallSiteAndInvoke")]
	internal static object UserDefinedConversion(IDynamicMetaObjectProvider expression, Type targetType)
	{
		return IDOUtils.CreateConvertCallSiteAndInvoke(new VBConversionBinder(targetType), expression);
	}

	[RequiresUnreferencedCode("Calls IDOUtils.CreateFuncCallSiteAndInvoke")]
	internal static object InvokeUserDefinedOperator(Symbols.UserDefinedOperator op, object[] arguments)
	{
		ExpressionType? expressionType = IDOUtils.LinqOperator(op);
		if (!expressionType.HasValue)
		{
			return Operators.InvokeObjectUserDefinedOperator(op, arguments);
		}
		ExpressionType value = expressionType.Value;
		CallSiteBinder action = ((arguments.Length != 1) ? ((DynamicMetaObjectBinder)new VBBinaryOperatorBinder(op, value)) : ((DynamicMetaObjectBinder)new VBUnaryOperatorBinder(op, value)));
		object instance = arguments[0];
		object[] arguments2 = ((arguments.Length == 1) ? Symbols.NoArguments : new object[1] { arguments[1] });
		return IDOUtils.CreateFuncCallSiteAndInvoke(action, instance, arguments2);
	}
}

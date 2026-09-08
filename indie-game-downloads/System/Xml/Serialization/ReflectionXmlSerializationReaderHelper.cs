using System.Linq.Expressions;
using System.Reflection;

namespace System.Xml.Serialization;

internal static class ReflectionXmlSerializationReaderHelper
{
	public delegate void SetMemberValueDelegate(object o, object val);

	public static SetMemberValueDelegate GetSetMemberValueDelegateWithType<TObj, TParam>(MemberInfo memberInfo)
	{
		Action<TObj, TParam> setTypedDelegate = null;
		if (memberInfo is PropertyInfo propertyInfo)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: true);
			if (setMethod == null)
			{
				return propertyInfo.SetValue;
			}
			setTypedDelegate = setMethod.CreateDelegate<Action<TObj, TParam>>();
		}
		else if (memberInfo is FieldInfo field)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(TObj));
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(TParam));
			BinaryExpression body = Expression.Assign(Expression.Field(parameterExpression, field), parameterExpression2);
			setTypedDelegate = Expression.Lambda<Action<TObj, TParam>>(body, new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
		}
		return delegate(object o, object p)
		{
			setTypedDelegate((TObj)o, (TParam)p);
		};
	}
}

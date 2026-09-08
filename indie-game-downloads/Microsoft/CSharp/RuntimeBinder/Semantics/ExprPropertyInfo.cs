using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprPropertyInfo : ExprWithType
{
	public PropWithType Property { get; }

	public PropertyInfo PropertyInfo
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		get
		{
			AggregateType ats = Property.Ats;
			PropertySymbol propertySymbol = Property.Prop();
			TypeArray typeArray = TypeManager.SubstTypeArray(propertySymbol.Params, ats, null);
			Type type = ats.AssociatedSystemType;
			PropertyInfo associatedPropertyInfo = propertySymbol.AssociatedPropertyInfo;
			if (!type.IsGenericType && !type.IsNested)
			{
				type = associatedPropertyInfo.DeclaringType;
			}
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.HasSameMetadataDefinitionAs(associatedPropertyInfo))
				{
					continue;
				}
				bool flag = true;
				ParameterInfo[] array = ((propertyInfo.GetSetMethod(nonPublic: true) != null) ? propertyInfo.GetSetMethod(nonPublic: true).GetParameters() : propertyInfo.GetGetMethod(nonPublic: true).GetParameters());
				for (int j = 0; j < typeArray.Count; j++)
				{
					if (!ExprWithType.TypesAreEqual(array[j].ParameterType, typeArray[j].AssociatedSystemType))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return propertyInfo;
				}
			}
			throw Error.InternalCompilerError();
		}
	}

	public ExprPropertyInfo(CType type, PropertySymbol propertySymbol, AggregateType propertyType)
		: base(ExpressionKind.PropertyInfo, type)
	{
		Property = new PropWithType(propertySymbol, propertyType);
	}
}

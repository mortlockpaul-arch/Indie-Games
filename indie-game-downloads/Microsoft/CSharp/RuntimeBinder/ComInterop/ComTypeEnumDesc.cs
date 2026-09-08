using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ComTypeEnumDesc : ComTypeDesc, IDynamicMetaObjectProvider
{
	private readonly string[] _memberNames;

	private readonly object[] _memberValues;

	public override string ToString()
	{
		return "<enum '" + base.TypeName + "'>";
	}

	internal ComTypeEnumDesc(ITypeInfo typeInfo, ComTypeLibDesc typeLibDesc)
		: base(typeInfo, typeLibDesc)
	{
		TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(typeInfo);
		string[] array = new string[typeAttrForTypeInfo.cVars];
		object[] array2 = new object[typeAttrForTypeInfo.cVars];
		for (int i = 0; i < typeAttrForTypeInfo.cVars; i++)
		{
			typeInfo.GetVarDesc(i, out var ppVarDesc);
			VARDESC vARDESC;
			try
			{
				vARDESC = Marshal.PtrToStructure<VARDESC>(ppVarDesc);
				if (vARDESC.varkind == VARKIND.VAR_CONST)
				{
					array2[i] = Marshal.GetObjectForNativeVariant(vARDESC.desc.lpvarValue);
				}
			}
			finally
			{
				typeInfo.ReleaseVarDesc(ppVarDesc);
			}
			array[i] = ComRuntimeHelpers.GetNameOfMethod(typeInfo, vARDESC.memid);
		}
		_memberNames = array;
		_memberValues = array2;
	}

	DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
	{
		return new TypeEnumMetaObject(this, parameter);
	}

	public object GetValue(string enumValueName)
	{
		for (int i = 0; i < _memberNames.Length; i++)
		{
			if (_memberNames[i] == enumValueName)
			{
				return _memberValues[i];
			}
		}
		throw new MissingMemberException(enumValueName);
	}

	internal bool HasMember(string name)
	{
		for (int i = 0; i < _memberNames.Length; i++)
		{
			if (_memberNames[i] == name)
			{
				return true;
			}
		}
		return false;
	}

	public string[] GetMemberNames()
	{
		return (string[])_memberNames.Clone();
	}
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Xml.Serialization;

internal sealed class EnumModel : TypeModel
{
	private ConstantModel[] _constants;

	internal ConstantModel[] Constants
	{
		get
		{
			if (_constants == null)
			{
				List<ConstantModel> list = new List<ConstantModel>();
				FieldInfo[] fields = base.Type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					ConstantModel constantModel = GetConstantModel(fields[i]);
					if (constantModel != null)
					{
						list.Add(constantModel);
					}
				}
				_constants = list.ToArray();
			}
			return _constants;
		}
	}

	internal EnumModel([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, TypeDesc typeDesc, ModelScope scope)
		: base(type, typeDesc, scope)
	{
	}

	private static ConstantModel GetConstantModel(FieldInfo fieldInfo)
	{
		if (fieldInfo.IsSpecialName)
		{
			return null;
		}
		return new ConstantModel(fieldInfo, ((IConvertible)fieldInfo.GetValue(null)).ToInt64(null));
	}
}

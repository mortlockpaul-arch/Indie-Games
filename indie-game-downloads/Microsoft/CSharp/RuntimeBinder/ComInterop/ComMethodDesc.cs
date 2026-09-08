using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ComMethodDesc
{
	private readonly INVOKEKIND _invokeKind;

	public string Name { get; }

	public int DispId { get; }

	public bool IsPropertyGet => (_invokeKind & INVOKEKIND.INVOKE_PROPERTYGET) != 0;

	public bool IsDataMember
	{
		get
		{
			if (!IsPropertyGet || DispId == -4)
			{
				return false;
			}
			return ParamCount == 0;
		}
	}

	public bool IsPropertyPut => (_invokeKind & (INVOKEKIND.INVOKE_PROPERTYPUT | INVOKEKIND.INVOKE_PROPERTYPUTREF)) != 0;

	public bool IsPropertyPutRef => (_invokeKind & INVOKEKIND.INVOKE_PROPERTYPUTREF) != 0;

	internal int ParamCount { get; }

	private ComMethodDesc(int dispId)
	{
		DispId = dispId;
	}

	internal ComMethodDesc(string name, int dispId)
		: this(dispId)
	{
		Name = name;
	}

	internal ComMethodDesc(string name, int dispId, INVOKEKIND invkind)
		: this(name, dispId)
	{
		_invokeKind = invkind;
	}

	internal ComMethodDesc(ITypeInfo typeInfo, FUNCDESC funcDesc)
		: this(funcDesc.memid)
	{
		_invokeKind = funcDesc.invkind;
		string[] array = new string[1 + funcDesc.cParams];
		typeInfo.GetNames(DispId, array, array.Length, out var pcNames);
		if (IsPropertyPut && array[^1] == null)
		{
			array[^1] = "value";
			pcNames++;
		}
		Name = array[0];
		ParamCount = funcDesc.cParams;
	}
}

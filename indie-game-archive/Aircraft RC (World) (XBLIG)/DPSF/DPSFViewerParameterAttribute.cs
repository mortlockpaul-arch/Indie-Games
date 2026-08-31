using System;

namespace DPSF;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class DPSFViewerParameterAttribute : Attribute
{
	private string _description = string.Empty;

	public string Description
	{
		get
		{
			return _description;
		}
		set
		{
			_description = value;
		}
	}
}

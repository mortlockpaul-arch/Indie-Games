using System.Collections.Generic;

namespace ImportData;

public class BoneImportData
{
	private List<BoneStateImportData> states;

	public BoneImportData()
	{
		states = new List<BoneStateImportData>();
	}

	public List<BoneStateImportData> GetJointStates()
	{
		return states;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SgMotion;

public class SkinnedModelBoneCollection : ReadOnlyCollection<SkinnedModelBone>
{
	public SkinnedModelBone this[string boneName]
	{
		get
		{
			for (int i = 0; i < base.Count; i++)
			{
				if (base[i].Name == boneName)
				{
					return base[i];
				}
			}
			return null;
		}
	}

	public SkinnedModelBoneCollection(IList<SkinnedModelBone> list)
		: base(list)
	{
	}

	public int GetBoneId(string boneName, bool ignoreCase = false)
	{
		for (int i = 0; i < base.Count; i++)
		{
			if (string.Compare(base[i].Name, boneName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
			{
				return i;
			}
		}
		return -1;
	}
}

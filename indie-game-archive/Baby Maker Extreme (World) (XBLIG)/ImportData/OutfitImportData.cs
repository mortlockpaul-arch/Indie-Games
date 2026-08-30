using System.Collections.Generic;

namespace ImportData;

public class OutfitImportData
{
	private string m_sSkeleton;

	private string m_sCostume;

	private List<string> m_lAnimations;

	public string Skeleton => m_sSkeleton;

	public string Costume => m_sCostume;

	public List<string> Animations => m_lAnimations;

	public OutfitImportData(string skel, string costume, List<string> animations)
	{
		m_sSkeleton = skel;
		m_sCostume = costume;
		m_lAnimations = animations;
	}
}

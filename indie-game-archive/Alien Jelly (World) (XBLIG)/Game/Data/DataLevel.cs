using System.Collections.Generic;

namespace Game.Data;

public class DataLevel
{
	public int expendable;

	public string sky;

	public int music;

	public int particles;

	public List<DataKeyFrame> intro;

	public List<DataAtom> atoms;

	public List<DataConversation> conversations;

	public DataLevel()
	{
	}

	public DataLevel(int pExpendable, string pSky, int pMusic, int pParticles, List<DataKeyFrame> pIntro, List<DataAtom> pAtoms, List<DataConversation> pConversations)
	{
		expendable = pExpendable;
		sky = pSky;
		music = pMusic;
		particles = pParticles;
		intro = pIntro;
		atoms = pAtoms;
		conversations = pConversations;
	}
}

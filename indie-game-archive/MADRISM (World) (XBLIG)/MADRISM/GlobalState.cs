using System.Collections.Generic;
using TechArts;

namespace MADRISM
{
	internal class GlobalState : TaskObj
	{
		public static bool inState;

		public static bool inDestroy;

		public static bool toAttract;

		public static bool inAttract;

		public override IEnumerator<int> Update()
		{
			int n = 0;
			for (int i = 0; i < 60; i++)
			{
				yield return 0;
			}
			while (true)
			{
				inState = true;
				inDestroy = false;
				manager.Entry(new TitleState());
				while (inState)
				{
					yield return 0;
				}
				if (toAttract)
				{
					if (n++ % 2 == 0)
					{
						inState = true;
						inDestroy = false;
						inAttract = true;
						manager.Entry(new PlayState("", 0));
						manager.Entry(new AttractState());
					}
					else
					{
						inState = true;
						inDestroy = false;
						inAttract = false;
						manager.Entry(new PlayState("Replay.bin", 6000));
					}
					while (inState)
					{
						yield return 0;
					}
					for (int j = 0; j < 30; j++)
					{
						yield return 0;
					}
					inAttract = false;
				}
				else
				{
					for (int k = 0; k < 60; k++)
					{
						yield return 0;
					}
					inState = true;
					inDestroy = false;
					manager.Entry(new PlayState("", 0));
					while (inState)
					{
						yield return 0;
					}
				}
			}
		}
	}
}

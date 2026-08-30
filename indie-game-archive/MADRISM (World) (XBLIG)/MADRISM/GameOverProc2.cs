using System.Collections.Generic;
using TechArts;

namespace MADRISM
{
	internal class GameOverProc2 : TaskObj
	{
		private int count;

		private void destroy(int n)
		{
			Queue<Parts> queue = new Queue<Parts>();
			int num = PlayState.core.exist.Count / n;
			for (int i = 0; i < num; i++)
			{
				queue.Enqueue(PlayState.core.exist[i]);
			}
			while (queue.Count > 0)
			{
				queue.Dequeue().DestroyDirect();
			}
		}

		public override IEnumerator<int> Update()
		{
			count = 0;
			while (true)
			{
				yield return 0;
			}
		}

		public override void PostUpdate()
		{
			count++;
			if (count == 133)
			{
				destroy(3);
			}
			if (count == 152)
			{
				destroy(2);
			}
			if (count == 165)
			{
				destroy(1);
				manager.Remove(this);
			}
		}
	}
}

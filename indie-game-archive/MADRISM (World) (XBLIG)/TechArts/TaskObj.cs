using System.Collections.Generic;

namespace TechArts
{
	public abstract class TaskObj
	{
		protected internal TaskManager manager;

		protected IEnumerator<int> func;

		protected TaskObj()
		{
			func = Update();
		}

		protected internal void doUpdate()
		{
			if (!func.MoveNext())
			{
				manager.Remove(this);
			}
		}

		public abstract IEnumerator<int> Update();

		public virtual void PreUpdate()
		{
		}

		public virtual void PostUpdate()
		{
		}

		public virtual void Draw()
		{
		}

		public virtual void Draw2()
		{
		}
	}
}

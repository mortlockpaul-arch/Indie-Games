using System.Collections.Generic;

namespace TechArts
{
	public class TaskManager
	{
		private List<TaskObj> objects = new List<TaskObj>();

		private Queue<TaskObj> added = new Queue<TaskObj>();

		private Queue<TaskObj> removed = new Queue<TaskObj>();

		public void Update()
		{
			while (added.Count > 0)
			{
				TaskObj item = added.Dequeue();
				objects.Add(item);
			}
			foreach (TaskObj @object in objects)
			{
				@object.PreUpdate();
				@object.doUpdate();
				@object.PostUpdate();
			}
			while (removed.Count > 0)
			{
				TaskObj item2 = removed.Dequeue();
				objects.Remove(item2);
			}
		}

		public void Draw()
		{
			foreach (TaskObj @object in objects)
			{
				@object.Draw();
			}
			foreach (TaskObj object2 in objects)
			{
				object2.Draw2();
			}
		}

		public void Entry(TaskObj obj)
		{
			obj.manager = this;
			added.Enqueue(obj);
		}

		public void Remove(TaskObj obj)
		{
			removed.Enqueue(obj);
		}
	}
}

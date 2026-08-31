using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;

namespace Z
{
	internal class y<T> : PooledObjectFactory<T> where T : new()
	{
		private List<T> HCB = new List<T>();

		public override T New()
		{
			T val = base.New();
			HCB.Add(val);
			return val;
		}

		public override void Free(T obj)
		{
			HCB.Remove(obj);
			base.Free(obj);
		}

		public void FreeAllTracked()
		{
			foreach (T item in HCB)
			{
				base.Free(item);
			}
			HCB.Clear();
		}

		public override void Clear()
		{
			FreeAllTracked();
			base.Clear();
		}
	}
}
namespace z
{
	internal abstract class y
	{
		public y()
		{
		}

		public static y Create()
		{
			return new _6();
		}

		public abstract void GetBytes(byte[] data);

		public abstract void GetNonZeroBytes(byte[] data);
	}
}

using System.Collections.Generic;

namespace ProjectMercury.Emitters;

public class EmitterCollection : List<Emitter>
{
	public Emitter this[string name]
	{
		get
		{
			for (int i = 0; i < base.Count; i++)
			{
				if (base[i].Name.Equals(name))
				{
					return base[i];
				}
			}
			throw new KeyNotFoundException();
		}
	}
}

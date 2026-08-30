using System.Collections.Generic;

namespace ProjectMercury.Modifiers;

public class ModifierCollection : List<Modifier>
{
	public ModifierCollection DeepCopy()
	{
		ModifierCollection modifierCollection = new ModifierCollection();
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Modifier current = enumerator.Current;
				modifierCollection.Add(current.DeepCopy());
			}
		}
		return modifierCollection;
	}

	internal unsafe void RunProcessors(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < base.Count; i++)
		{
			base[i].Process(dt, particleArray, count);
		}
	}
}

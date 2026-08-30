using System.Collections.Generic;

namespace FarseerGames.FarseerPhysics.Collisions;

public class ContactList(int capacity) : List<Contact>(capacity)
{
	private int _index = -1;

	public int IndexOfSafe(Contact contact)
	{
		_index = -1;
		for (int i = 0; i < base.Count; i++)
		{
			if (base[i] == contact)
			{
				_index = i;
				break;
			}
		}
		return _index;
	}
}

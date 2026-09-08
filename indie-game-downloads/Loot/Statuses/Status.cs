using System;
using System.Collections.Generic;
using System.IO;
using Loot.Dungeon;
using Microsoft.Xna.Framework;

namespace Loot.Statuses;

public class Status
{
	private List<StatusEffect> effects = new List<StatusEffect>();

	public Status()
	{
	}

	public Status(BinaryReader reader)
	{
		Read(reader);
	}

	public void Clear()
	{
		effects.Clear();
	}

	public void Update(GameTime gameTime, Character character)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			effects[i].Update(gameTime, character);
		}
	}

	public bool Is<T>() where T : StatusEffect
	{
		if (effects.Count == 0)
		{
			return false;
		}
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < effects.Count; i++)
		{
			if ((object)effects[i].GetType() == typeFromHandle)
			{
				return true;
			}
		}
		return false;
	}

	public void Add(StatusEffect effect)
	{
		effects.Add(effect);
	}

	public void Remove(StatusEffect effect)
	{
		effects.Remove(effect);
	}

	public void RemoveAll<T>() where T : StatusEffect
	{
		Type typeFromHandle = typeof(T);
		for (int num = effects.Count - 1; num >= 0; num--)
		{
			if ((object)effects[num].GetType() == typeFromHandle)
			{
				effects.RemoveAt(num);
			}
		}
	}

	private void Read(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			effects.Add(StatusRegistry.Load(reader));
		}
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(effects.Count);
		for (int i = 0; i < effects.Count; i++)
		{
			StatusRegistry.Save(writer, effects[i]);
		}
	}
}

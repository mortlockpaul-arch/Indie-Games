using Microsoft.Xna.Framework.Audio;
using Quasar.Global;

namespace Quasar.Audios;

public class SoundEffectManager : Manager<SoundEffect>
{
	public const string SOUND_DIR = "Sounds/";

	private static SoundEffectManager instance;

	public static SoundEffectManager SoundEffects
	{
		get
		{
			if (instance == null)
			{
				instance = new SoundEffectManager();
			}
			return instance;
		}
	}

	protected SoundEffectManager()
	{
	}

	protected override SoundEffect createDefaultItem()
	{
		return Engine.ContentManager.Load<SoundEffect>("Sounds/default");
	}

	protected override SoundEffect LoadItem(string name)
	{
		return Engine.ContentManager.Load<SoundEffect>("Sounds/" + name);
	}

	public static SoundEffect LoadSound(string path, bool addToManager)
	{
		if (addToManager && SoundEffects.TryGetValue(path, out var item))
		{
			return item;
		}
		try
		{
			item = Engine.ContentManager.Load<SoundEffect>(path);
		}
		catch
		{
			item = SoundEffects.DefaultItem;
		}
		if (addToManager)
		{
			SoundEffects.Add(path, item);
		}
		return item;
	}

	public override void Dispose()
	{
		instance = null;
		base.Dispose();
	}
}

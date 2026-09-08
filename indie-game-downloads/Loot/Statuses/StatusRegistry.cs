using System.IO;
using Eyehook.Framework;

namespace Loot.Statuses;

public static class StatusRegistry
{
	private static TypeRegistry<StatusEffect> registry = new TypeRegistry<StatusEffect>();

	public static void Initialize()
	{
		Register<StatusPoison>("SE0");
		Register<StatusFreeze>("SE1");
		Register<StatusInvisible>("SE2");
		Register<StatusLevitate>("SE3");
		Register<StatusProtect>("SE4");
	}

	private static void Register<T>(string id) where T : StatusEffect
	{
		registry.Register<T>(id);
	}

	public static void Save(BinaryWriter writer, StatusEffect statusEffect)
	{
		registry.Save(writer, statusEffect);
	}

	public static StatusEffect Load(BinaryReader reader)
	{
		return registry.Load(reader);
	}
}

using System;

namespace ProjectMercury.Emitters;

public static class EmitterCompatibilityExtensions
{
	[Obsolete("Use 'Initialise' method instead.", false)]
	public static void Initialize(this Emitter emitter)
	{
		emitter.Initialise();
	}

	[Obsolete("Use Update(deltaSeconds) method instead.", false)]
	public static void Update(this Emitter emitter, float totalSeconds, float deltaSeconds)
	{
		emitter.Update(deltaSeconds);
	}
}

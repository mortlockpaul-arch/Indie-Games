using Microsoft.Xna.Framework;

namespace Quasar.Particles;

public interface IParticleGroup
{
	int AvailableParticles { get; }

	Mesh Mesh { get; }

	string Name { get; }

	bool SpawnParticle(float time, ref Vector3 position, ref Vector3 speed);

	void Update();

	void ClearParticles();
}

using N;
using s;

namespace E;

internal class b<T> : h where T : s.b
{
	public new T CollisionInformation => (T)collisionInformation;

	protected internal b()
	{
	}

	public b(T collisionInformation)
	{
		Initialize(collisionInformation);
	}

	public b(T collisionInformation, float mass)
	{
		Initialize(collisionInformation, mass);
	}

	public b(T collisionInformation, float mass, N._7 inertiaTensor)
	{
		Initialize(collisionInformation, mass, inertiaTensor);
	}

	public b(T collisionInformation, float mass, N._7 inertiaTensor, float volume)
	{
		Initialize(collisionInformation, mass, inertiaTensor, volume);
	}
}

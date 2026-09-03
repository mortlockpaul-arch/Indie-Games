using Microsoft.Xna.Framework;

namespace OluXNA;

internal interface IPath
{
	Vector3 curLocation();

	float maxSpeed();

	float advance();

	void reset();

	IPath copy();

	Vector3 dir();
}

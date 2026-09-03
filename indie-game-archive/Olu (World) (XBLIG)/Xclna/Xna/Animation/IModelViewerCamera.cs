using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public interface IModelViewerCamera
{
	Matrix ModelWorld { get; }

	Matrix View { get; }

	Matrix Projection { get; }

	void Update(GameTime gameTime);
}

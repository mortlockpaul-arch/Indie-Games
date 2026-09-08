using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public interface IPositionVertex : IVertexType
{
	Vector3 Position { get; }
}

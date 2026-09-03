using Microsoft.Xna.Framework;

namespace OluXNA;

internal interface ITransform
{
	void Update(double gametime);

	Matrix GetMatrix();

	Matrix GetMatrix(float amountDone);

	void Initialize(int start, int end);
}

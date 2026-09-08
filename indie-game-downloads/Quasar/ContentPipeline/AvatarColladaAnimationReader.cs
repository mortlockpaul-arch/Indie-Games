using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Quasar.ContentPipeline;

public class AvatarColladaAnimationReader : ContentTypeReader<AvatarColladaAnimation>
{
	public const int BoneCount = 71;

	protected override AvatarColladaAnimation Read(ContentReader input, AvatarColladaAnimation existingInstance)
	{
		int num = input.ReadInt32();
		AvatarColladaAnimation.SFrame[] array = new AvatarColladaAnimation.SFrame[71];
		int num2 = input.ReadInt32();
		if (num2 > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = input.ReadInt32();
				array[i] = new AvatarColladaAnimation.SFrame();
				array[i].translation = new Vector3[num];
				array[i].rotation = new Quaternion[num];
				for (int j = 0; j < num; j++)
				{
					if (j < num3)
					{
						Matrix matrix = input.ReadMatrix();
						ref Quaternion reference = ref array[i].rotation[j];
						reference = Quaternion.CreateFromRotationMatrix(matrix);
						ref Vector3 reference2 = ref array[i].translation[j];
						reference2 = matrix.Translation;
					}
					else
					{
						ref Quaternion reference3 = ref array[i].rotation[j];
						reference3 = ((j > 0) ? array[i].rotation[j - 1] : Quaternion.Identity);
						ref Vector3 reference4 = ref array[i].translation[j];
						reference4 = ((j > 0) ? array[i].translation[j - 1] : Vector3.Zero);
					}
				}
			}
		}
		return new AvatarColladaAnimation(array, num);
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Shaders;

namespace RacingGame.Graphics;

internal class PlaneRenderer
{
	private const float Tiling = 20f;

	private Vector3 pos;

	private Plane plane;

	private Material material;

	private float size;

	public PlaneRenderer(Vector3 setPos, Plane setPlane, Material setMaterial, float setSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		pos = setPos;
		plane = setPlane;
		material = setMaterial;
		size = setSize;
	}

	private void DrawPlaneVertices()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normal = plane.Normal;
		if (((Vector3)(ref normal)).Length() == 0f)
		{
			((Vector3)(ref normal))._002Ector(0f, 0f, 1f);
		}
		Vector3 val = Vector3.Cross(normal, new Vector3(1f, 0f, 0f));
		if (((Vector3)(ref val)).Length() == 0f)
		{
			((Vector3)(ref val))._002Ector(0f, 1f, 0f);
		}
		Vector3 val2 = Vector3.Cross(val, normal);
		Vector3 val3 = Vector3.Cross(normal, val2);
		float d = plane.D;
		TangentVertex[] array = new TangentVertex[4]
		{
			new TangentVertex((-val2 - val3) * size + normal * d, (0f - size) / 20f, (0f - size) / 20f, normal, val2),
			new TangentVertex((-val2 + val3) * size + normal * d, (0f - size) / 20f, size / 20f, normal, val2),
			new TangentVertex((val2 - val3) * size + normal * d, size / 20f, (0f - size) / 20f, normal, val2),
			new TangentVertex((val2 + val3) * size + normal * d, size / 20f, size / 20f, normal, val2)
		};
		BaseGame.Device.DrawUserPrimitives<TangentVertex>((PrimitiveType)5, array, 0, 2);
	}

	public void Render()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.WorldMatrix = Matrix.CreateTranslation(pos);
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		ShaderEffect.normalMapping.Render(material, "DiffuseSpecular20", DrawPlaneVertices);
		BaseGame.WorldMatrix = Matrix.Identity;
	}
}

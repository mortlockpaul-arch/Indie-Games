using MaxScriptDefines;

namespace DataContent;

public class MeshAttributesParams
{
	public EnumObjectTypes ObjectType;

	public EnumMaterialTypes MaterialType;

	public EnumOpacityTypes Opacity;

	public EnumCullingTypes Culling;

	public EnumCollisionTypes Collision;

	public EnumEmitterTypes EmitterType;

	public MeshAttributesParams()
	{
		ObjectType = EnumObjectTypes.Render;
		MaterialType = EnumMaterialTypes.Concrete;
		Opacity = EnumOpacityTypes.Opaque;
		Culling = EnumCullingTypes.CullCW;
		Collision = EnumCollisionTypes.NoTest;
		EmitterType = EnumEmitterTypes.NumberOf;
	}
}

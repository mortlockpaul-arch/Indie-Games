using System;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game;

internal class FenceItem : RenderItem
{
	private const float MARGIN = 0.25f;

	private RenderItem[] borders = new RenderItem[4];

	private InstancedMesh fences;

	public FenceItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		for (int i = 0; i < 4; i++)
		{
			borders[i] = new RenderItem(new XMesh("Backgrounds/FenceTile"));
			borders[i].Mesh.Shader = ShaderManager.Shaders["BaseUVScale"];
			borders[i].Mesh.FirstMaterial.AddFloatParameter(1f);
			borders[i].Mesh.FirstMaterial.AddFloatParameter(1f);
			borders[i].Mesh.Texture = TextureManager.Textures["Backgrounds/FenceTex"];
			borders[i].Mesh.Ambient = new Vector3(0.5f);
			borders[i].Mesh.Diffuse = Vector3.One;
			borders[i].Mesh.FirstMaterial.Specular = Vector3.Zero;
			addChild(borders[i]);
		}
		fences = new InstancedMesh("Backgrounds/FencePost", AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.X * 2 + AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.Y * 2);
		fences.Shader = ShaderManager.Shaders["BaseInstancing"];
		fences.Ambient = new Vector3(0.5f);
		fences.Diffuse = Vector3.One;
		fences.Texture = TextureManager.Textures["Backgrounds/FenceTex"];
		fences.FirstMaterial.Shininess = 0f;
		addMesh(fences);
		stage.FarmData.OnIncreaseSize += FarmData_OnIncreaseSize;
		FarmData_OnIncreaseSize(stage.FarmData.FarmSize, stage.FarmData.FarmSize);
	}

	private void FarmData_OnIncreaseSize(Int2 oldSize, Int2 newSize)
	{
		borders[0].Transform.Translation = new Vector3((float)newSize.X * 2f * 0.5f, 0f, -0.5f);
		borders[1].Transform.Translation = new Vector3(((float)newSize.X + 0.25f) * 2f, 0f, (float)newSize.Y * 2f * 0.5f);
		borders[2].Transform.Translation = new Vector3((float)newSize.X * 2f * 0.5f, 0f, ((float)newSize.Y + 0.25f) * 2f);
		borders[3].Transform.Translation = new Vector3(-0.5f, 0f, (float)newSize.Y * 2f * 0.5f);
		borders[0].Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0f);
		borders[1].Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 2f);
		borders[2].Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI);
		borders[3].Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 4.712389f);
		Transform obj = borders[0].Transform;
		Vector3 scale = (borders[2].Transform.Scale = new Vector3(((float)newSize.X + 0.5f) * 2f, 1f, 1f));
		obj.Scale = scale;
		Transform obj2 = borders[1].Transform;
		Vector3 scale2 = (borders[3].Transform.Scale = new Vector3(((float)newSize.Y + 0.5f) * 2f, 1f, 1f));
		obj2.Scale = scale2;
		for (int i = 0; i < borders.Length; i++)
		{
			borders[i].Mesh.FirstMaterial.SetFloatParameter(1, borders[i].Transform.Scale.X);
		}
		fences.ClearInstances();
		fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(-0.5f, -0.5f))), checkCull: false);
		fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(((float)newSize.X + 0.25f) * 2f, -0.5f))), checkCull: false);
		fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(((float)newSize.X + 0.25f) * 2f, ((float)newSize.Y + 0.25f) * 2f))), checkCull: false);
		fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(-0.5f, ((float)newSize.Y + 0.25f) * 2f))), checkCull: false);
		float num = (float)newSize.X * 2f / 20f;
		float num2 = (float)newSize.Y * 2f / 20f;
		for (int j = 1; j < 20; j++)
		{
			fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(num * (float)j, -0.5f))), checkCull: false);
		}
		for (int k = 1; k < 20; k++)
		{
			fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(2f * ((float)newSize.X + 0.25f), num2 * (float)k))), checkCull: false);
		}
		for (int num3 = 19; num3 > 0; num3--)
		{
			fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(num * (float)num3, ((float)newSize.Y + 0.25f) * 2f))), checkCull: false);
		}
		for (int num4 = 19; num4 > 0; num4--)
		{
			fences.AddInstance(Matrix.CreateTranslation(AvatarFarmOnline.Logic.GameGlobals.WorldPosition(new Vector2(-0.5f, num2 * (float)num4))), checkCull: false);
		}
		fences.UpdateData();
	}
}

using System;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public class Stage : ModelObject
{
	private XSIModel bg;

	private ContentManager content;

	public ChapterSettings Chapter { get; private set; }

	public event Action Finished;

	public Stage(Game game, ContentManager content, ChapterSettings chapter, bool loop)
		: base(game)
	{
		this.content = content;
		Chapter = chapter;
		Initialize();
		InitializeModels(chapter, loop);
	}

	public override void Initialize()
	{
		base.Use = true;
		base.Enable = true;
		base.Visible = true;
	}

	private void InitializeModels(ChapterSettings chapter, bool loop)
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		model = (string.IsNullOrEmpty(Chapter.StageModelAsset) ? null : new XSIModel(Chapter.StageModelAsset, content));
		collision = (string.IsNullOrEmpty(Chapter.CollisionModelAsset) ? null : new XSIModel(Chapter.CollisionModelAsset, content));
		bg = (string.IsNullOrEmpty(Chapter.BgModelAsset) ? null : new XSIModel(Chapter.BgModelAsset, content));
		if (model != null)
		{
			model.Finished += delegate
			{
				if (Finished != null)
				{
					Finished();
				}
			};
			model.Play(loop);
		}
		if (collision != null)
		{
			collision.Play(loop);
			collision.UpdateBoundingSphere(GetWorld());
		}
		if (bg != null)
		{
			bg.Play(loop);
		}
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (model != null)
		{
			model.Update(elapsedGameTime);
		}
		if (bg != null)
		{
			bg.Update(elapsedGameTime);
		}
		if (collision != null)
		{
			collision.Update(elapsedGameTime);
			collision.UpdateBoundingSphere(GetWorld());
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		if (model != null)
		{
			model.Draw(Global.SASData, world);
		}
		if (bg != null)
		{
			bg.Draw(Global.SASData, world);
		}
	}

	public override bool Damage(int damage)
	{
		throw new NotImplementedException();
	}

	public override Matrix GetWorld()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.Identity;
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		return ((Matrix)(ref world)).Translation;
	}
}

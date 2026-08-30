using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skeleton;

public class CharacterManager
{
	private SkeletonManager skeleton;

	private SkeletalCostume costume;

	private SkeletalAnimation animation;

	private List<SkeletalAnimation> animations;

	private Vector2 worldPos;

	private float worldScale;

	private float worldRotation;

	private string filename;

	private Dictionary<int, List<CharacterManager>> childSkeletons;

	private float m_animSpeed;

	private bool m_bMirror;

	private bool m_bHasChildSkeletons;

	private Color m_Color;

	public Color Color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Color;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			m_Color = value;
			costume.SetColor(value);
		}
	}

	public bool Mirror
	{
		get
		{
			return m_bMirror;
		}
		set
		{
			m_bMirror = value;
		}
	}

	public float Depth
	{
		get
		{
			return costume.Depth;
		}
		set
		{
			costume.Depth = value;
		}
	}

	public float AnimSpeed
	{
		get
		{
			return m_animSpeed;
		}
		set
		{
			m_animSpeed = value;
			animation.AnimSpeed = m_animSpeed;
		}
	}

	public Vector2 WorldPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return worldPos;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			worldPos = value;
			skeleton.SetWorldPosition(value);
		}
	}

	public float WorldRotation
	{
		get
		{
			return worldRotation;
		}
		set
		{
			worldRotation = value;
			skeleton.SetWorldRotation(value);
		}
	}

	public CharacterManager(SkeletonManager skelClone, SkeletalCostume costumeClone, List<SkeletalAnimation> anims, string definitionFile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_Color = Color.White;
		m_bHasChildSkeletons = false;
		m_bMirror = false;
		m_animSpeed = 1f;
		skeleton = new SkeletonManager();
		costume = new SkeletalCostume();
		filename = definitionFile;
		skeleton.Clone(skelClone);
		costume.Clone(costumeClone, skeleton);
		worldPos = default(Vector2);
		worldScale = 1f;
		worldRotation = 0f;
		animations = anims;
		animation = new SkeletalAnimation(skeleton);
		childSkeletons = new Dictionary<int, List<CharacterManager>>();
	}

	public CharacterManager(string definitionFile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_Color = Color.White;
		m_animSpeed = 1f;
		CharacterManager characterManager = CharacterResourceManager.LoadCharacter(definitionFile);
		filename = definitionFile;
		skeleton = new SkeletonManager();
		costume = new SkeletalCostume();
		skeleton.Clone(characterManager.skeleton);
		costume.Clone(characterManager.costume, skeleton);
		worldPos = default(Vector2);
		worldScale = 1f;
		worldRotation = 0f;
		animations = characterManager.animations;
		animation = new SkeletalAnimation(skeleton);
		childSkeletons = new Dictionary<int, List<CharacterManager>>();
	}

	public string GetResourceFilename()
	{
		return filename;
	}

	public void SetResourceFilename(string val)
	{
		filename = val;
	}

	public CharacterSprite GetFirstCharacterSprite(int id)
	{
		return costume.GetFirstCharacterSprite(id);
	}

	public void AddChildSkeleton(CharacterManager c, int id)
	{
		m_bHasChildSkeletons = true;
		if (!childSkeletons.ContainsKey(id))
		{
			childSkeletons[id] = new List<CharacterManager>();
		}
		childSkeletons[id].Add(c);
	}

	public List<CharacterManager> GetChildSkeleton(int i)
	{
		if (childSkeletons.ContainsKey(i))
		{
			return childSkeletons[i];
		}
		return null;
	}

	public void SetAnimation(SkeletalAnimation anim, int timer)
	{
		animation.Clone(anim, -timer);
		animation.AnimSpeed = m_animSpeed;
		skeleton.GetRoot().RecalculatePosition();
	}

	public void SetAnimation(int i, int timer)
	{
		animation.Clone(animations[i], -timer);
		animation.AnimSpeed = m_animSpeed;
		skeleton.GetRoot().RecalculatePosition();
	}

	public int NumAnimations()
	{
		return animations.Count;
	}

	public SkeletalAnimation GetAnimation(int i)
	{
		return animations[i];
	}

	public void Update(TimeTracker gameTime)
	{
	}

	public void Draw(TimeTracker gameTime)
	{
		costume.Draw(gameTime);
		if (!m_bHasChildSkeletons)
		{
			return;
		}
		foreach (int key in childSkeletons.Keys)
		{
			foreach (CharacterManager item in childSkeletons[key])
			{
				item.Draw(gameTime);
			}
		}
	}

	public void SetMouth(int i)
	{
		costume.SetMouth(i);
	}

	public void IterateMouth(int i)
	{
		costume.SetMouth(costume.GetMouth() + i);
	}

	public int GetMouth()
	{
		return costume.GetMouth();
	}

	public void SetEyeAimAt(Vector2 v)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		costume.SetEyeAimAt(v);
	}

	public Vector2 GetEyeAimAt()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return costume.GetEyeAimAt();
	}

	public void DrawUpdater(TimeTracker gameTime)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		animation.Update(gameTime);
		skeleton.GetRoot().RecalculatePosition();
		if (Mirror)
		{
			costume.Update(gameTime, worldPos.X);
		}
		else
		{
			costume.Update(gameTime, -999f);
		}
		if (!m_bHasChildSkeletons)
		{
			return;
		}
		foreach (int key in childSkeletons.Keys)
		{
			Joint joint = GetJoint(key);
			foreach (CharacterManager item in childSkeletons[key])
			{
				item.WorldPosition = joint.GetPosition();
				item.WorldRotation = joint.GetCumulativeRotation();
				item.DrawUpdater(gameTime);
			}
		}
	}

	public void RecalcPos()
	{
		skeleton.GetRoot().RecalculatePosition();
	}

	public void SetWorldScale(float f)
	{
		worldScale = f;
		skeleton.SetWorldScale(f);
		if (!m_bHasChildSkeletons)
		{
			return;
		}
		foreach (int key in childSkeletons.Keys)
		{
			foreach (CharacterManager item in childSkeletons[key])
			{
				item.SetWorldScale(worldScale);
			}
		}
	}

	public float GetWorldScale()
	{
		return worldScale;
	}

	public Joint GetJoint(int index)
	{
		return skeleton.FindJointById(index);
	}
}

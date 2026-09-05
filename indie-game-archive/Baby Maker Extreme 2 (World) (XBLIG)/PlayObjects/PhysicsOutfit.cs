using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;

namespace PlayObjects;

public class PhysicsOutfit
{
	private const int COLLISION_ITER = 20;

	private const int MAX_ENABLED_RIGHT = 1500;

	private const int MAX_STATIC_RIGHT = 2000;

	private const int MAX_ENABLED_LEFT = 1500;

	private const int MAX_STATIC_LEFT = 2000;

	private int m_iCollisionGroup;

	private List<Vector2> m_vStartPositions;

	private List<Vector2> m_jointPoints;

	private bool m_bIsGlowing;

	private List<int> m_enabledGroups;

	private List<int> m_staticGroups;

	private bool m_bIsStatic;

	private List<int> m_collisionGroups;

	private float m_fGlowTimer;

	private List<SpriteInstance> m_sprites;

	private List<SpriteInstance> m_glowSprites;

	private List<PhysicalRepresentation> m_phyiscalRep;

	private List<RevoluteJoint> m_revJoints;

	private List<AngleJoint> m_angleJoints;

	private List<int> m_JointConnections;

	private float m_fMinPos;

	private float m_fMaxPos;

	private List<float> m_savedMasses;

	private List<bool> m_forcedStatic;

	private bool m_bSelfGlow;

	public bool IsGlowing
	{
		get
		{
			return m_bIsGlowing;
		}
		set
		{
			m_bIsGlowing = value;
		}
	}

	public Category CollisionCategory
	{
		get
		{
			return m_phyiscalRep[0].CollisionCategory;
		}
		set
		{
			for (int i = 0; i < m_phyiscalRep.Count; i++)
			{
				m_phyiscalRep[i].CollisionCategory = value;
			}
		}
	}

	public PhysicsOutfit(int collisionGroup)
	{
		m_iCollisionGroup = collisionGroup * 20;
		m_phyiscalRep = new List<PhysicalRepresentation>();
		m_vStartPositions = new List<Vector2>();
		m_angleJoints = new List<AngleJoint>();
		m_revJoints = new List<RevoluteJoint>();
		m_jointPoints = new List<Vector2>();
		m_bIsGlowing = true;
		m_enabledGroups = new List<int>();
		m_staticGroups = new List<int>();
		m_forcedStatic = new List<bool>();
		m_fMinPos = 0f;
		m_fMaxPos = 0f;
		m_bIsStatic = false;
		m_collisionGroups = new List<int>();
		m_fGlowTimer = 0f;
		m_sprites = new List<SpriteInstance>();
		m_glowSprites = new List<SpriteInstance>();
		m_bSelfGlow = false;
	}

	public void SetSelfGlow()
	{
		m_bSelfGlow = true;
	}

	public void Initialize(PhysicsOutfit clone)
	{
		m_bIsStatic = false;
		for (int i = 0; i < clone.m_sprites.Count; i++)
		{
			if (clone.m_sprites[i] != null)
			{
				m_sprites.Add((SpriteInstance)clone.m_sprites[i].Clone());
			}
			else
			{
				m_sprites.Add(null);
			}
			if (clone.m_glowSprites[i] != null)
			{
				m_glowSprites.Add((SpriteInstance)clone.m_glowSprites[i].Clone());
			}
			else
			{
				m_glowSprites.Add(null);
			}
		}
		m_fMinPos = clone.m_fMinPos;
		m_fMaxPos = clone.m_fMaxPos;
		m_vStartPositions = clone.m_vStartPositions;
		m_jointPoints = clone.m_jointPoints;
		m_bIsStatic = clone.m_bIsStatic;
		m_collisionGroups = clone.m_collisionGroups;
		for (int j = 0; j < clone.m_phyiscalRep.Count; j++)
		{
			m_phyiscalRep.Add(new PhysicalRepresentation(PhysicsObjectManager.GetSimulation(), clone.m_phyiscalRep[j]));
			List<Fixture> fixtures = m_phyiscalRep.Last().GetFixtures();
			for (int k = 0; k < fixtures.Count; k++)
			{
				m_phyiscalRep.Last().GetFixtures()[k].CollisionFilter.CollisionGroup = (short)(-Math.Abs(m_iCollisionGroup + m_collisionGroups[j]));
			}
			m_phyiscalRep[j].Mass = clone.m_phyiscalRep[j].Mass;
			m_phyiscalRep[j].AirDrag = clone.m_phyiscalRep[j].AirDrag;
		}
		m_JointConnections = clone.m_JointConnections;
		for (int l = 0; l < clone.m_revJoints.Count; l++)
		{
			if (clone.m_revJoints[l] != null)
			{
				m_revJoints.Add(m_phyiscalRep[m_JointConnections[l * 2]].RevoluteAttach(m_phyiscalRep[m_JointConnections[l * 2 + 1]], m_jointPoints[l]));
			}
			if (clone.m_angleJoints[l] != null)
			{
				m_angleJoints.Add(m_phyiscalRep[m_JointConnections[l * 2]].AngleAttach(m_phyiscalRep[m_JointConnections[l * 2 + 1]]));
				m_angleJoints.Last().Softness = clone.m_angleJoints[l].Softness;
			}
			else
			{
				m_angleJoints.Add(null);
			}
		}
		m_forcedStatic = clone.m_forcedStatic;
		m_fGlowTimer = 0f;
	}

	public void Initialize(List<List<List<Vector2>>> parts, List<SpriteInstance> sprites, List<SpriteInstance> glowSprites, List<int> connections, List<Vector2> jointPos, List<PhysicsJointType> jointTypes, List<MassTypes> masses, List<int> collisionGroups)
	{
		Initialize(parts, sprites, glowSprites, connections, jointPos, jointTypes, masses, collisionGroups, 1f);
	}

	public void Initialize(List<List<List<Vector2>>> parts, List<SpriteInstance> sprites, List<SpriteInstance> glowSprites, List<int> connections, List<Vector2> jointPos, List<PhysicsJointType> jointTypes, List<MassTypes> masses, List<int> collisionGroups, float scale)
	{
		if (scale != 1f)
		{
			for (int i = 0; i < sprites.Count; i++)
			{
				sprites[i].WidthScale *= scale;
				sprites[i].Origin *= scale;
			}
			for (int j = 0; j < glowSprites.Count; j++)
			{
				if (glowSprites[j] != null)
				{
					glowSprites[j].WidthScale *= scale;
					glowSprites[j].Origin *= scale;
				}
			}
			for (int k = 0; k < jointPos.Count; k++)
			{
				jointPos[k] *= scale;
			}
			for (int l = 0; l < parts.Count; l++)
			{
				for (int m = 0; m < parts[l].Count; m++)
				{
					for (int n = 0; n < parts[l][m].Count; n++)
					{
						parts[l][m][n] *= scale;
					}
				}
			}
		}
		for (int num = 0; num < parts.Count; num++)
		{
			for (int num2 = 0; num2 < parts[num].Count; num2++)
			{
				for (int num3 = 0; num3 < parts[num][num2].Count; num3++)
				{
					int num4 = num3;
					int num5 = ((num3 + 1 < parts[num][num2].Count) ? (num3 + 1) : 0);
					Vector2 vector = parts[num][num2][num5] - parts[num][num2][num4];
					for (int num6 = 0; num6 < parts[num][num2].Count; num6++)
					{
						if (num6 != num4 && num6 != num5)
						{
							Vector2 vector2 = parts[num][num2][num6] - parts[num][num2][num4];
							float num7 = vector.X * vector2.Y - vector.Y * vector2.X;
							if (num7 <= 0f)
							{
								parts[num][num2].Reverse();
							}
						}
					}
				}
			}
		}
		m_JointConnections = connections;
		m_bIsStatic = false;
		m_collisionGroups = collisionGroups;
		m_sprites = sprites;
		m_glowSprites = glowSprites;
		m_savedMasses = new List<float>();
		for (int num8 = 0; num8 < parts.Count; num8++)
		{
			for (int num9 = 0; num9 < parts[num8][0].Count; num9++)
			{
				if (parts[num8][0][num9].X < m_fMinPos)
				{
					m_fMinPos = parts[num8][0][num9].X;
				}
				if (parts[num8][0][num9].X > m_fMaxPos)
				{
					m_fMaxPos = parts[num8][0][num9].X;
				}
			}
			List<Vector2> list = new List<Vector2>(parts[num8][0]);
			float area = PhysicalRepresentation.GetArea(list);
			Vector2 centroid = PhysicalRepresentation.GetCentroid(list, area);
			m_vStartPositions.Add(default(Vector2));
			m_phyiscalRep.Add(PhysicsObjectManager.CreatePhysicalRepresentation(list, default(Vector2), PhysicsObjectManager.WallCollisionGroup(), scale: true));
			List<Fixture> fixtures = m_phyiscalRep.Last().GetFixtures();
			for (int num10 = 0; num10 < fixtures.Count; num10++)
			{
				fixtures[num10].CollisionFilter.CollisionGroup = (short)(m_iCollisionGroup + collisionGroups[num8]);
			}
			m_phyiscalRep.Last().AirDrag = 0f;
			if (masses[num8] == MassTypes.FLESH_MASS)
			{
				m_phyiscalRep.Last().Mass = 1f;
			}
			else
			{
				m_phyiscalRep.Last().Mass = 1f;
			}
			m_savedMasses.Add(m_phyiscalRep.Last().Mass);
			if (m_sprites[num8] != null)
			{
				m_sprites[num8].Origin += centroid;
			}
			if (m_glowSprites[num8] != null)
			{
				m_glowSprites[num8].Origin += centroid;
				m_glowSprites[num8].FlatColor = true;
			}
		}
		for (int num11 = 0; num11 < jointPos.Count; num11++)
		{
			m_revJoints.Add(m_phyiscalRep[connections[num11 * 2]].RevoluteAttach(m_phyiscalRep[connections[num11 * 2 + 1]], jointPos[num11]));
			m_angleJoints.Add(m_phyiscalRep[connections[num11 * 2]].AngleAttach(m_phyiscalRep[connections[num11 * 2 + 1]]));
			if (jointTypes[num11] == PhysicsJointType.FIRM_JOINT)
			{
				m_angleJoints.Last().Softness = 0.9f;
			}
			else if (jointTypes[num11] == PhysicsJointType.NORM_JOINT)
			{
				m_angleJoints.Last().Softness = 0.99f;
			}
			else if (jointTypes[num11] == PhysicsJointType.SOLID_JOINT)
			{
				m_angleJoints.Last().Softness = 0.01f;
			}
		}
		m_jointPoints = jointPos;
		for (int num12 = 0; num12 < m_phyiscalRep.Count; num12++)
		{
			m_forcedStatic.Add(item: false);
		}
	}

	public void Update(TimeTracker gameTime)
	{
		m_fGlowTimer += gameTime.FractionOfSecond;
		for (int i = 0; i < m_sprites.Count; i++)
		{
			if (!m_phyiscalRep[i].Static && m_phyiscalRep[i].Mass != m_savedMasses[i])
			{
				m_phyiscalRep[i].Mass = m_savedMasses[i];
			}
			if (m_sprites[i] != null)
			{
				if (m_phyiscalRep[i].Static)
				{
					m_sprites[i].Position = m_phyiscalRep[i].GetWorldCenter() - new Vector2(0f, (m_sprites[i].SurfaceScale.Y + m_sprites[i].Origin.Y) / 2f);
				}
				else
				{
					m_sprites[i].Position = m_phyiscalRep[i].GetWorldCenter();
				}
				m_sprites[i].Rotation = m_phyiscalRep[i].Rotation;
			}
			if (m_glowSprites[i] == null)
			{
				continue;
			}
			if (m_phyiscalRep[i].Static)
			{
				m_glowSprites[i].Position = m_phyiscalRep[i].GetWorldCenter() - new Vector2(0f, (m_glowSprites[i].SurfaceScale.Y + m_sprites[i].Origin.Y) / 2f);
			}
			else
			{
				m_glowSprites[i].Position = m_phyiscalRep[i].GetWorldCenter();
			}
			m_glowSprites[i].Rotation = m_phyiscalRep[i].Rotation;
			if (!m_bIsGlowing)
			{
				m_glowSprites[i].Alpha -= 3f * gameTime.FractionOfSecond;
				if (m_glowSprites[i].Alpha < 0f)
				{
					m_glowSprites[i].Alpha = 0f;
				}
			}
			else
			{
				m_glowSprites[i].Alpha = 1f;
			}
			if (m_bSelfGlow)
			{
				m_glowSprites[i].SurfaceScale = m_sprites[i].SurfaceScale + new Vector2(20f, 0f) + new Vector2(1f, 1f) * (60f + (float)Math.Sin(3f * m_fGlowTimer) * 20f);
			}
			else
			{
				m_glowSprites[i].SurfaceScale = m_sprites[i].SurfaceScale + new Vector2(1f, 1f) * (5f + (float)Math.Sin(3f * m_fGlowTimer)) * 5f;
			}
			m_glowSprites[i].Origin = m_sprites[i].Origin * m_glowSprites[i].SurfaceScale / m_sprites[i].SurfaceScale;
		}
	}

	public bool CanGlow()
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			if (m_glowSprites[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	public void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i < m_sprites.Count; i++)
		{
			if (m_sprites[i] != null)
			{
				m_sprites[i].Draw(gameTime);
			}
			if (m_glowSprites[i] != null)
			{
				m_glowSprites[i].Draw(gameTime);
			}
		}
	}

	public void ResetToPosition(Vector2 v)
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			m_phyiscalRep[i].ResetSimulation();
			m_phyiscalRep[i].ResetDynamics();
			m_phyiscalRep[i].Position = v + m_vStartPositions[i];
		}
		for (int j = 0; j < m_sprites.Count; j++)
		{
			if (m_sprites[j] != null)
			{
				m_sprites[j].Position = m_phyiscalRep[j].GetWorldCenter();
				m_sprites[j].Rotation = m_phyiscalRep[j].Rotation;
			}
			if (m_glowSprites[j] != null)
			{
				m_glowSprites[j].Position = m_phyiscalRep[j].GetWorldCenter();
				m_glowSprites[j].Rotation = m_phyiscalRep[j].Rotation;
			}
		}
		for (int k = 0; k < m_phyiscalRep.Count; k++)
		{
			if (v.X < -1000f || v.X > 1600f)
			{
				m_phyiscalRep[k].Static = true;
				m_phyiscalRep[k].Enabled = false;
				m_phyiscalRep[k].Enabled = false;
			}
			else
			{
				m_phyiscalRep[k].Static = true;
				m_phyiscalRep[k].Enabled = true;
			}
		}
	}

	public void SetCollisionHandler(OnCollisionEventHandler target)
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			if (m_glowSprites[i] != null)
			{
				m_phyiscalRep[i].SetCollisionHandler(target);
			}
		}
	}

	public List<PhysicalRepresentation> GetPhysicsObjects()
	{
		return m_phyiscalRep;
	}

	public void GetJoints(List<int> indexes, List<Joint> joints)
	{
		for (int i = 0; i < m_revJoints.Count; i++)
		{
			if (indexes.Contains(i))
			{
				joints.Add(m_revJoints[i]);
			}
		}
		for (int j = 0; j < m_angleJoints.Count; j++)
		{
			if (indexes.Contains(j))
			{
				joints.Add(m_angleJoints[j]);
			}
		}
	}

	public List<SpriteInstance> GetSprites()
	{
		return m_sprites;
	}

	public void Disable()
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			m_phyiscalRep[i].Enabled = false;
		}
	}

	public void DisableStatic()
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			m_phyiscalRep[i].Static = false;
		}
	}

	public void UpdateEnabled()
	{
		float x = SceneRenderer.GetCameraPosition().X;
		m_enabledGroups.Clear();
		m_staticGroups.Clear();
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			List<Fixture> fixtures = m_phyiscalRep[i].GetFixtures();
			for (int j = 0; j < fixtures.Count; j++)
			{
				Fixture fixture = m_phyiscalRep[i].GetFixtures()[j];
				int item = Math.Abs(fixture.CollisionFilter.CollisionGroup);
				if (m_enabledGroups.Contains(item))
				{
					continue;
				}
				if (m_phyiscalRep[i].Enabled)
				{
					if (m_phyiscalRep[i].GetWorldCenter().X > x - 1500f && m_phyiscalRep[i].GetWorldCenter().X < x + 1500f)
					{
						m_enabledGroups.Add(item);
					}
					else if (!m_staticGroups.Contains(item) && m_phyiscalRep[i].GetWorldCenter().X > x - 2000f && m_phyiscalRep[i].GetWorldCenter().X < x + 2000f)
					{
						m_staticGroups.Add(item);
					}
				}
				else if (m_phyiscalRep[i].GetWorldCenter().X > SceneRenderer.GetCameraPosition().X - 2000f)
				{
					m_staticGroups.Add(item);
				}
			}
		}
		for (int k = 0; k < m_phyiscalRep.Count; k++)
		{
			List<Fixture> fixtures2 = m_phyiscalRep[k].GetFixtures();
			int item2 = Math.Abs(fixtures2[0].CollisionFilter.CollisionGroup);
			if (m_enabledGroups.Contains(item2))
			{
				if (!m_bIsStatic && !m_forcedStatic[k])
				{
					m_phyiscalRep[k].Static = false;
				}
				m_phyiscalRep[k].Enabled = true;
			}
			else if (m_staticGroups.Contains(item2))
			{
				m_phyiscalRep[k].Static = true;
				m_phyiscalRep[k].Enabled = true;
			}
			else
			{
				m_phyiscalRep[k].Enabled = false;
			}
		}
	}

	public void SetDepth(float f)
	{
		for (int i = 0; i < m_sprites.Count; i++)
		{
			if (m_sprites[i] != null)
			{
				m_sprites[i].Depth += f + SceneRenderer.GetRand(0f, 0.0001f);
			}
			if (m_glowSprites[i] != null)
			{
				m_glowSprites[i].Depth += f;
			}
		}
	}

	public float MaxPos()
	{
		return m_fMaxPos;
	}

	public float MinPos()
	{
		return m_fMinPos;
	}

	public void GenerateParticles(Color c)
	{
		for (int i = 0; i < m_sprites.Count; i++)
		{
			ParticleManager.GetParticle().Initialize(m_sprites[i].GetSpriteImage(), m_sprites[i].Position, m_sprites[i].Depth - 1f, 500, default(Vector2), fadesOut: true, c, c, m_sprites[i].WidthScale, m_sprites[i].WidthScale, additive: false, default(Vector2), m_sprites[i].Rotation, -0.01f, m_sprites[i].Origin, isFlat: false);
		}
	}

	public void SetForcedStatic(int i)
	{
		m_forcedStatic[i] = true;
		m_phyiscalRep[i].Static = true;
	}

	public void RemoveForcedStatic(int i)
	{
		m_forcedStatic[i] = false;
		m_phyiscalRep[i].Static = false;
	}
}

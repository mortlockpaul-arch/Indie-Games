using System;
using System.Collections.Generic;
using System.Linq;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics;
using Renderer;

namespace Scene;

public class PhysicsOutfit
{
	private const int MAX_ENABLED_RIGHT = 750;

	private const int MAX_STATIC_RIGHT = 1250;

	private const int MAX_ENABLED_LEFT = 700;

	private const int MAX_STATIC_LEFT = 1200;

	private List<List<RenderSprite>> m_sprites;

	private List<PhysicalRepresentation> m_phyiscalRep;

	private List<Vector2> m_vStartPositions;

	private int m_iCollisionGroup;

	private List<RevoluteJoint> m_revJoints;

	private List<AngleJoint> m_angleJoints;

	private List<Vector2> m_jointPoints;

	private int m_iDisablePass;

	private List<int> m_enabledGroups;

	private List<int> m_staticGroups;

	private bool m_bIsStatic;

	private float m_fMinPos;

	private float m_fMaxPos;

	private List<int> m_JointConnections;

	private List<Vertices> m_Vertices;

	private List<Vertices> m_VerticesBody;

	private List<int> m_collisionGroups;

	private float m_fGlowTimer;

	public PhysicsOutfit(int collisionGroup)
	{
		m_iCollisionGroup = collisionGroup * 10;
		m_phyiscalRep = new List<PhysicalRepresentation>();
		m_vStartPositions = new List<Vector2>();
		m_angleJoints = new List<AngleJoint>();
		m_revJoints = new List<RevoluteJoint>();
		m_jointPoints = new List<Vector2>();
		m_iDisablePass = -1;
		m_enabledGroups = new List<int>();
		m_staticGroups = new List<int>();
		m_fMinPos = 0f;
		m_fMaxPos = 0f;
		m_bIsStatic = false;
		m_Vertices = new List<Vertices>();
		m_VerticesBody = new List<Vertices>();
		m_collisionGroups = new List<int>();
		m_fGlowTimer = 0f;
	}

	public void Initialize(PhysicsOutfit clone)
	{
		m_bIsStatic = false;
		m_sprites = new List<List<RenderSprite>>();
		for (int i = 0; i < clone.m_sprites.Count; i++)
		{
			m_sprites.Add(new List<RenderSprite>());
			for (int j = 0; j < clone.m_sprites[i].Count; j++)
			{
				if (clone.m_sprites[i][j] != null)
				{
					m_sprites[i].Add(new RenderSprite(clone.m_sprites[i][j].GetSpriteImage(), clone.m_sprites[i][j].Position, clone.m_sprites[i][j].Depth));
					m_sprites[i][j].Origin = clone.m_sprites[i][j].Origin;
				}
				else
				{
					m_sprites[i].Add(null);
				}
			}
		}
		m_fMinPos = clone.m_fMinPos;
		m_fMaxPos = clone.m_fMaxPos;
		m_vStartPositions = clone.m_vStartPositions;
		m_jointPoints = clone.m_jointPoints;
		m_bIsStatic = clone.m_bIsStatic;
		m_collisionGroups = clone.m_collisionGroups;
		m_Vertices = clone.m_Vertices;
		m_VerticesBody = clone.m_VerticesBody;
		for (int k = 0; k < clone.m_phyiscalRep.Count; k++)
		{
			m_phyiscalRep.Add(PhysicsObjectManager.CreatePhysicalRepresentationNoSubDiv(m_VerticesBody[k], m_Vertices[k], default(Vector2), PhysicsObjectManager.WallCollisionGroup()));
			m_phyiscalRep.Last().GetGeom().CollisionGroup = m_iCollisionGroup + m_collisionGroups[k];
			m_phyiscalRep[k].Mass = clone.m_phyiscalRep[k].Mass;
			m_phyiscalRep[k].AirDrag = clone.m_phyiscalRep[k].AirDrag;
		}
		m_JointConnections = clone.m_JointConnections;
		for (int l = 0; l < clone.m_revJoints.Count; l++)
		{
			if (clone.m_revJoints[l] != null)
			{
				m_revJoints.Add(m_phyiscalRep[m_JointConnections[l * 2]].RevoluteAttach(m_phyiscalRep[m_JointConnections[l * 2 + 1]], m_jointPoints[l]));
				m_revJoints.Last().Softness = 1f;
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
		m_fGlowTimer = 0f;
	}

	public void Initialize(List<List<Vector2>> parts, List<List<RenderSprite>> sprites, List<int> connections, List<Vector2> jointPos, List<PhysicsJointType> jointTypes, List<MassTypes> masses, List<int> collisionGroups)
	{
		m_JointConnections = connections;
		m_bIsStatic = false;
		m_collisionGroups = collisionGroups;
		m_sprites = sprites;
		for (int i = 0; i < parts.Count; i++)
		{
			for (int j = 0; j < parts[i].Count; j++)
			{
				if (parts[i][j].X < m_fMinPos)
				{
					m_fMinPos = parts[i][j].X;
				}
				if (parts[i][j].X > m_fMaxPos)
				{
					m_fMaxPos = parts[i][j].X;
				}
			}
			Vertices vertices = new Vertices(parts[i]);
			Vector2 centroid = vertices.GetCentroid();
			m_vStartPositions.Add(centroid);
			m_phyiscalRep.Add(PhysicsObjectManager.CreatePhysicalRepresentation(vertices, default(Vector2), PhysicsObjectManager.WallCollisionGroup()));
			m_Vertices.Add(vertices);
			m_VerticesBody.Add(new Vertices());
			for (int k = 0; k < m_Vertices[i].Count; k++)
			{
				m_VerticesBody.Last().Add(m_Vertices[i][k]);
			}
			m_phyiscalRep.Last().GetGeom().CollisionGroup = m_iCollisionGroup + collisionGroups[i];
			m_phyiscalRep.Last().AirDrag = 0f;
			if (masses[i] == MassTypes.FLESH_MASS)
			{
				m_phyiscalRep.Last().Mass = 0.5f;
			}
			else if (masses[i] == MassTypes.HEAVY_MASS)
			{
				m_phyiscalRep.Last().Mass = 2f;
			}
			else if (masses[i] == MassTypes.LIGHT_MASS)
			{
				m_phyiscalRep.Last().Mass = 0.5f;
			}
			else if (masses[i] == MassTypes.MED_MASS)
			{
				m_phyiscalRep.Last().Mass = 1f;
			}
			else if (masses[i] == MassTypes.ULTRAHEAVY_MASS)
			{
				m_phyiscalRep.Last().Mass = 5f;
			}
			else if (masses[i] == MassTypes.ULTRALIGHT_MASS)
			{
				m_phyiscalRep.Last().Mass = 0.2f;
			}
			for (int l = 0; l < m_sprites.Count; l++)
			{
				if (m_sprites[l][i] != null)
				{
					m_sprites[l][i].Origin += centroid;
				}
			}
		}
		for (int m = 0; m < jointPos.Count; m++)
		{
			m_revJoints.Add(m_phyiscalRep[connections[m * 2]].RevoluteAttach(m_phyiscalRep[connections[m * 2 + 1]], jointPos[m]));
			m_revJoints.Last().Softness = 1f;
			if (jointTypes[m] == PhysicsJointType.SOLID_JOINT)
			{
				m_angleJoints.Add(m_phyiscalRep[connections[m * 2]].AngleAttach(m_phyiscalRep[connections[m * 2 + 1]]));
				m_angleJoints.Last().Softness = 0f;
			}
			else if (jointTypes[m] == PhysicsJointType.NORM_JOINT)
			{
				m_angleJoints.Add(m_phyiscalRep[connections[m * 2]].AngleAttach(m_phyiscalRep[connections[m * 2 + 1]]));
				m_angleJoints.Last().Softness = 0.98f;
			}
			else if (jointTypes[m] == PhysicsJointType.LOOSE_JOINT)
			{
				m_angleJoints.Add(m_phyiscalRep[connections[m * 2]].AngleAttach(m_phyiscalRep[connections[m * 2 + 1]]));
				m_angleJoints.Last().Softness = 0.99f;
			}
			else if (jointTypes[m] == PhysicsJointType.FIRM_JOINT)
			{
				m_angleJoints.Add(m_phyiscalRep[connections[m * 2]].AngleAttach(m_phyiscalRep[connections[m * 2 + 1]]));
				m_angleJoints.Last().Softness = 0.97f;
			}
			else if (jointTypes[m] == PhysicsJointType.ULTRAFIRM_JOINT)
			{
				m_angleJoints.Add(m_phyiscalRep[connections[m * 2]].AngleAttach(m_phyiscalRep[connections[m * 2 + 1]]));
				m_angleJoints.Last().Softness = 0.96f;
			}
			else
			{
				m_angleJoints.Add(null);
			}
		}
		m_jointPoints = jointPos;
	}

	public void Update(TimeTracker gameTime)
	{
		m_fGlowTimer += gameTime.FractionOfSecond;
		for (int i = 0; i < m_sprites.Count; i++)
		{
			for (int j = 0; j < m_sprites[i].Count; j++)
			{
				if (m_sprites[i][j] == null)
				{
					continue;
				}
				m_sprites[i][j].Position = m_phyiscalRep[j].Position;
				m_sprites[i][j].Rotation = m_phyiscalRep[j].Rotation;
				if (m_iDisablePass == i)
				{
					m_sprites[i][j].Alpha -= 3f * gameTime.FractionOfSecond;
					if (m_sprites[i][j].Alpha < 0f)
					{
						m_sprites[i][j].Alpha = 0f;
					}
				}
				if (i > 0)
				{
					m_sprites[i][j].WidthScale = m_sprites[0][j].WidthScale + (1f + (float)Math.Sin(3f * m_fGlowTimer)) * 5f;
					m_sprites[i][j].Origin = m_sprites[0][j].Origin * m_sprites[i][j].WidthScale / m_sprites[0][j].WidthScale;
				}
			}
		}
	}

	public void DisablePass(int i)
	{
		if (m_iDisablePass < m_sprites.Count && m_iDisablePass >= 0)
		{
			for (int j = 0; j < m_sprites[m_iDisablePass].Count; j++)
			{
				if (m_sprites[m_iDisablePass][j] != null)
				{
					m_sprites[m_iDisablePass][j].Alpha = 1f;
				}
			}
		}
		m_iDisablePass = i;
	}

	public void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i < m_sprites.Count; i++)
		{
			for (int j = 0; j < m_sprites[i].Count; j++)
			{
				if (m_sprites[i][j] != null)
				{
					m_sprites[i][j].Draw(gameTime);
				}
			}
		}
	}

	public void ResetToPosition(Vector2 v)
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			m_phyiscalRep[i].ResetSimulation();
			m_phyiscalRep[i].GetGeom().Body.ResetDynamics();
			m_phyiscalRep[i].Position = v + m_vStartPositions[i];
			m_phyiscalRep[i].GetGeom().CollidesWith = CollisionCategory.All;
		}
		for (int j = 0; j < m_revJoints.Count; j++)
		{
			m_revJoints[j].Anchor = v + m_jointPoints[j];
		}
		for (int k = 0; k < m_sprites.Count; k++)
		{
			for (int l = 0; l < m_sprites[k].Count; l++)
			{
				if (m_sprites[k][l] != null)
				{
					m_sprites[k][l].Position = m_phyiscalRep[l].Position;
					m_sprites[k][l].Rotation = m_phyiscalRep[l].Rotation;
				}
			}
		}
		for (int m = 0; m < m_phyiscalRep.Count; m++)
		{
			if (v.X < -1000f || v.X > 1600f)
			{
				m_phyiscalRep[m].Static = true;
				m_phyiscalRep[m].Enabled = false;
				m_phyiscalRep[m].Enabled = false;
			}
			else
			{
				m_phyiscalRep[m].Static = true;
				m_phyiscalRep[m].Enabled = true;
			}
		}
	}

	public void GenerateParticles(Color c)
	{
		for (int i = 0; i < m_sprites[0].Count; i++)
		{
			ParticleManager.GetParticle().Initialize(m_sprites[0][i].GetSpriteImage(), m_sprites[0][i].Position, m_sprites[0][i].Depth - 1f, 500, default(Vector2), fadesOut: true, c, c, m_sprites[0][i].WidthScale, m_sprites[0][i].WidthScale, additive: false, default(Vector2), m_sprites[0][i].Rotation, -0.01f, m_sprites[0][i].Origin);
		}
	}

	public void SetCollisionHandler(CollisionEventHandler target)
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			if (m_sprites[1][i] != null)
			{
				m_phyiscalRep[i].SetCollisionHandler(target);
			}
		}
	}

	public List<PhysicalRepresentation> GetPhysicsObjects()
	{
		return m_phyiscalRep;
	}

	public List<RenderSprite> GetSprites(int i)
	{
		return m_sprites[i];
	}

	public void UpdateEnabled()
	{
		float x = SceneRenderer.GetCameraPosition().X;
		m_enabledGroups.Clear();
		m_staticGroups.Clear();
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			int collisionGroup = m_phyiscalRep[i].GetGeom().CollisionGroup;
			if (!m_enabledGroups.Contains(collisionGroup))
			{
				if (m_phyiscalRep[i].GetGeom().AABB.Max.X > x - 700f && m_phyiscalRep[i].GetGeom().AABB.Min.X < x + 750f)
				{
					m_enabledGroups.Add(collisionGroup);
				}
				else if (!m_staticGroups.Contains(collisionGroup) && m_phyiscalRep[i].GetGeom().AABB.Max.X > x - 1200f && m_phyiscalRep[i].GetGeom().AABB.Min.X < x + 1250f)
				{
					m_staticGroups.Add(collisionGroup);
				}
			}
		}
		for (int j = 0; j < m_phyiscalRep.Count; j++)
		{
			int collisionGroup2 = m_phyiscalRep[j].GetGeom().CollisionGroup;
			if (m_enabledGroups.Contains(collisionGroup2))
			{
				if (!m_bIsStatic)
				{
					m_phyiscalRep[j].Static = false;
				}
				m_phyiscalRep[j].Enabled = true;
			}
			else if (m_staticGroups.Contains(collisionGroup2))
			{
				m_phyiscalRep[j].Static = true;
				m_phyiscalRep[j].Enabled = true;
			}
			else
			{
				m_phyiscalRep[j].Enabled = false;
			}
		}
	}

	public void SetStatic()
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			m_phyiscalRep[i].Static = true;
		}
		m_bIsStatic = true;
	}

	public void RemoveStatic()
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			m_phyiscalRep[i].Static = false;
		}
		m_bIsStatic = false;
	}

	public void SetDepth(float f)
	{
		for (int i = 0; i < m_sprites.Count; i++)
		{
			for (int j = 0; j < m_sprites[i].Count; j++)
			{
				if (m_sprites[i][j] != null)
				{
					m_sprites[i][j].Depth += f;
				}
			}
		}
	}

	public void LightenGroup(int index)
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			if (m_phyiscalRep[i].GetGeom().CollisionGroup == index)
			{
				m_phyiscalRep[i].Mass = 0.01f;
				m_phyiscalRep[i].GetGeom().CollisionEnabled = false;
			}
		}
	}

	public void ReEnableCollisions()
	{
		for (int i = 0; i < m_phyiscalRep.Count; i++)
		{
			if (!m_phyiscalRep[i].GetGeom().CollisionEnabled)
			{
				m_phyiscalRep[i].Mass = 0.5f;
				m_phyiscalRep[i].GetGeom().CollisionEnabled = true;
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
}

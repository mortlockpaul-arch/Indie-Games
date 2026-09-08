namespace EGEngine;

internal class dtNodePool
{
	private dtNode[] m_nodes;

	private int m_maxNodes;

	private int m_nodeCount;

	public dtNodePool()
	{
	}

	public dtNodePool(int maxNodes)
	{
		m_maxNodes = maxNodes;
		m_nodeCount = 0;
		m_nodes = new dtNode[maxNodes];
		for (int i = 0; i < m_maxNodes; i++)
		{
			m_nodes[i] = new dtNode(-1, 30u, 2u);
		}
	}

	public void clear()
	{
		for (int i = 0; i < m_maxNodes; i++)
		{
			m_nodes[i].nodeIdx = -1;
			m_nodes[i].id = 0u;
		}
		m_nodeCount = 0;
	}

	public dtNode getNode(uint id)
	{
		for (int i = 0; i < m_maxNodes; i++)
		{
			if (m_nodes[i].id == id)
			{
				return m_nodes[i];
			}
			if (m_nodes[i].nodeIdx < 0)
			{
				m_nodes[i].nodeIdx = i;
				m_nodes[i].pidx = 0u;
				m_nodes[i].cost = 0f;
				m_nodes[i].total = 0f;
				m_nodes[i].id = id;
				m_nodes[i].flags = 0u;
				return m_nodes[i];
			}
		}
		return null;
	}

	public uint getNodeIdx(dtNode node)
	{
		if (node == null)
		{
			return 0u;
		}
		return (uint)(node.nodeIdx + 1);
	}

	public dtNode getNodeAtIdx(uint idx)
	{
		if (idx == 0)
		{
			return null;
		}
		return m_nodes[idx - 1];
	}

	public dtNode findNode(uint id)
	{
		for (int i = 0; i < m_maxNodes; i++)
		{
			if (m_nodes[i].id == id)
			{
				return m_nodes[i];
			}
		}
		return null;
	}
}

namespace EGEngine;

internal class dtNodeQueue
{
	private dtNode[] m_heap;

	private int m_capacity;

	private int m_size;

	public dtNodeQueue(int n)
	{
		m_capacity = n;
		m_size = 0;
		m_heap = new dtNode[m_capacity + 1];
	}

	public void clear()
	{
		m_size = 0;
	}

	public dtNode top()
	{
		return m_heap[0];
	}

	public dtNode pop()
	{
		int num = 0;
		float num2 = float.MaxValue;
		for (int i = 0; i < m_size; i++)
		{
			if (m_heap[i].total < num2)
			{
				num = i;
				num2 = m_heap[i].total;
			}
		}
		dtNode result = m_heap[num];
		for (int j = num; j < m_size - 1; j++)
		{
			m_heap[j] = m_heap[j + 1];
		}
		m_size--;
		return result;
	}

	public void push(dtNode node)
	{
		m_heap[m_size] = node;
		m_size++;
	}

	public void modify(dtNode node)
	{
		for (int i = 0; i < m_size; i++)
		{
			if (m_heap[i].id == node.id)
			{
				m_heap[i] = node;
			}
		}
	}

	public bool empty()
	{
		return m_size == 0;
	}
}

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class dtStatNavMesh
{
	public struct dtStatPoly(int size)
	{
		public ushort[] v = new ushort[size];

		public ushort[] n = new ushort[size];

		public byte nv = 0;

		public byte flags = 0;

		public bool IsPolyValid
		{
			get
			{
				return (flags & 1) == 0;
			}
			set
			{
			}
		}
	}

	public struct dtStatPolyDetail
	{
		public ushort vbase;

		public ushort nverts;

		public ushort tbase;

		public ushort ntris;
	}

	public struct dtStatBVNode(int size)
	{
		public ushort[] bmin = new ushort[size];

		public ushort[] bmax = new ushort[size];

		public int i = 0;
	}

	public class dtStatNavMeshHeader
	{
		public int magic;

		public int version;

		public int headerSize;

		public int npolys;

		public int nverts;

		public int nnodes;

		public int ndmeshes;

		public int ndverts;

		public int ndtris;

		public float cs;

		public Vector3 worldOffset;

		public Vector3 bmin;

		public Vector3 bmax;

		public Vector3 worldMin;

		public Vector3 worldMax;

		public dtStatPoly[] polys;

		public Vector3[] verts;

		public dtStatBVNode[] bvtree;

		public dtStatPolyDetail[] dmeshes;

		public Vector3[] dverts;

		public byte[][] dtris;
	}

	public const int Size_dtStatPoly = 26;

	public const int Size_dtStatPolyDetail = 8;

	public const int Size_dtStatBVNode = 16;

	private static int DT_STAT_VERTS_PER_POLYGON = 6;

	private static int DT_STAT_NAVMESH_MAGIC = 1312904781;

	private static int DT_STAT_NAVMESH_VERSION = 3;

	public static int MAX_POLYS = 256;

	public static int MaxRoutesThisUpdate = 1;

	private static Random RandGen = new Random();

	private bool Initialized;

	public Vector3 PickExtents = new Vector3(20f, 80f, 20f);

	private dtStatNavMeshHeader mHeader;

	private dtNodePool m_nodePool;

	private dtNodeQueue m_openList;

	private static float[][] straightPolys = new float[MAX_POLYS][];

	private Vector3 cptbMin = Vector3.Zero;

	private Vector3 cptbMax = Vector3.Zero;

	private Vector3 spos = Vector3.Zero;

	private Vector3 epos = Vector3.Zero;

	private Vector3 gvppV1 = Vector3.Zero;

	private Vector3 gvppV2 = Vector3.Zero;

	public Vector3 fnpClosestPoint = Vector3.Zero;

	private static float H_SCALE = 1.1f;

	private Vector3 cptpPT = Vector3.Zero;

	private Vector3[] cptpV = new Vector3[3];

	private static float thr = (float)Math.Sqrt(6.103515625E-05);

	private Vector3 cpptab = Vector3.Zero;

	private Vector3 cpptac = Vector3.Zero;

	private Vector3 cpptap = Vector3.Zero;

	private Vector3 cpptbp = Vector3.Zero;

	private Vector3 cpptcp = Vector3.Zero;

	private static float EPS = 0.0001f;

	public unsafe dtStatNavMeshHeader LoadNavigationMesh(string filename, Vector3 worldOffset)
	{
		try
		{
			FileStream fileStream = File.OpenRead(filename);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			int num = binaryReader.ReadInt32();
			byte[] array = new byte[num];
			binaryReader.Read(array, 0, num);
			Initialize((byte*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject(), num, ownsData: true);
			binaryReader.Close();
			binaryReader.Dispose();
			fileStream.Close();
			fileStream.Dispose();
			mHeader.worldOffset = worldOffset;
			mHeader.worldMin = mHeader.bmin + worldOffset;
			mHeader.worldMax = mHeader.bmax + worldOffset;
			return mHeader;
		}
		catch (FileNotFoundException)
		{
			return null;
		}
	}

	private unsafe static byte* ReadByte(byte* data, byte* e)
	{
		*e = *data;
		return data + 1;
	}

	private unsafe static byte* ReadShort(byte* data, short* e)
	{
		*(byte*)e = *data;
		((sbyte*)e)[1] = (sbyte)data[1];
		return data + 2;
	}

	private unsafe static byte* ReadUShort(byte* data, ushort* e)
	{
		*(byte*)e = *data;
		((sbyte*)e)[1] = (sbyte)data[1];
		return data + 2;
	}

	private unsafe static byte* ReadInt(byte* data, int* e)
	{
		*(byte*)e = *data;
		((sbyte*)e)[1] = (sbyte)data[1];
		((sbyte*)e)[2] = (sbyte)data[2];
		((sbyte*)e)[3] = (sbyte)data[3];
		return data + 4;
	}

	private unsafe static byte* ReadUInt(byte* data, uint* e)
	{
		*(byte*)e = *data;
		((sbyte*)e)[1] = (sbyte)data[1];
		((sbyte*)e)[2] = (sbyte)data[2];
		((sbyte*)e)[3] = (sbyte)data[3];
		return data + 4;
	}

	private unsafe static byte* ReadFloat(byte* data, float* e)
	{
		*(byte*)e = *data;
		((sbyte*)e)[1] = (sbyte)data[1];
		((sbyte*)e)[2] = (sbyte)data[2];
		((sbyte*)e)[3] = (sbyte)data[3];
		return data + 4;
	}

	public unsafe bool Initialize(byte* data, int dataSize, bool ownsData)
	{
		byte* ptr = data;
		mHeader = new dtStatNavMeshHeader();
		int num = 0;
		data = ReadInt(data, &num);
		mHeader.magic = num;
		if (num != DT_STAT_NAVMESH_MAGIC)
		{
			return false;
		}
		int num2 = 0;
		data = ReadInt(data, &num2);
		mHeader.version = num2;
		if (num2 != DT_STAT_NAVMESH_VERSION)
		{
			return false;
		}
		int headerSize = 0;
		data = ReadInt(data, &headerSize);
		mHeader.headerSize = headerSize;
		int npolys = 0;
		data = ReadInt(data, &npolys);
		mHeader.npolys = npolys;
		int nverts = 0;
		data = ReadInt(data, &nverts);
		mHeader.nverts = nverts;
		int nnodes = 0;
		data = ReadInt(data, &nnodes);
		mHeader.nnodes = nnodes;
		int ndmeshes = 0;
		data = ReadInt(data, &ndmeshes);
		mHeader.ndmeshes = ndmeshes;
		int ndverts = 0;
		data = ReadInt(data, &ndverts);
		mHeader.ndverts = ndverts;
		int ndtris = 0;
		data = ReadInt(data, &ndtris);
		mHeader.ndtris = ndtris;
		float cs = 0f;
		data = ReadFloat(data, &cs);
		mHeader.cs = cs;
		mHeader.worldOffset = Vector3.Zero;
		float x = default(float);
		data = ReadFloat(data, &x);
		float y = default(float);
		data = ReadFloat(data, &y);
		float z = default(float);
		data = ReadFloat(data, &z);
		float x2 = default(float);
		data = ReadFloat(data, &x2);
		float y2 = default(float);
		data = ReadFloat(data, &y2);
		float z2 = default(float);
		data = ReadFloat(data, &z2);
		mHeader.bmin.X = x;
		mHeader.bmin.Y = y;
		mHeader.bmin.Z = z;
		mHeader.bmax.X = x2;
		mHeader.bmax.Y = y2;
		mHeader.bmax.Z = z2;
		data = ptr + mHeader.headerSize;
		mHeader.verts = new Vector3[mHeader.nverts];
		mHeader.polys = new dtStatPoly[mHeader.npolys];
		mHeader.bvtree = new dtStatBVNode[mHeader.npolys * 2];
		mHeader.dmeshes = new dtStatPolyDetail[mHeader.ndmeshes];
		mHeader.dverts = new Vector3[mHeader.ndverts];
		mHeader.dtris = new byte[mHeader.ndtris][];
		for (int i = 0; i < mHeader.ndtris; i++)
		{
			mHeader.dtris[i] = new byte[4];
		}
		float x3 = default(float);
		float y3 = default(float);
		float z3 = default(float);
		for (int j = 0; j < mHeader.nverts; j++)
		{
			data = ReadFloat(data, &x3);
			data = ReadFloat(data, &y3);
			data = ReadFloat(data, &z3);
			ref Vector3 reference = ref mHeader.verts[j];
			reference = Vector3.Zero;
			mHeader.verts[j].X = x3;
			mHeader.verts[j].Y = y3;
			mHeader.verts[j].Z = z3;
		}
		ushort num3 = default(ushort);
		ushort num4 = default(ushort);
		byte nv = default(byte);
		byte flags = default(byte);
		for (int k = 0; k < mHeader.npolys; k++)
		{
			ref dtStatPoly reference2 = ref mHeader.polys[k];
			reference2 = new dtStatPoly(DT_STAT_VERTS_PER_POLYGON);
			for (int l = 0; l < DT_STAT_VERTS_PER_POLYGON; l++)
			{
				data = ReadUShort(data, &num3);
				mHeader.polys[k].v[l] = num3;
			}
			for (int m = 0; m < DT_STAT_VERTS_PER_POLYGON; m++)
			{
				data = ReadUShort(data, &num4);
				mHeader.polys[k].n[m] = num4;
			}
			data = ReadByte(data, &nv);
			data = ReadByte(data, &flags);
			mHeader.polys[k].nv = nv;
			mHeader.polys[k].flags = flags;
		}
		ushort num6 = default(ushort);
		ushort num8 = default(ushort);
		int i2 = default(int);
		for (int n = 0; n < mHeader.npolys * 2; n++)
		{
			ref dtStatBVNode reference3 = ref mHeader.bvtree[n];
			reference3 = new dtStatBVNode(3);
			for (int num5 = 0; num5 < 3; num5++)
			{
				data = ReadUShort(data, &num6);
				mHeader.bvtree[n].bmin[num5] = num6;
			}
			for (int num7 = 0; num7 < 3; num7++)
			{
				data = ReadUShort(data, &num8);
				mHeader.bvtree[n].bmax[num7] = num8;
			}
			data = ReadInt(data, &i2);
			mHeader.bvtree[n].i = i2;
		}
		ushort vbase = default(ushort);
		ushort nverts2 = default(ushort);
		ushort tbase = default(ushort);
		ushort ntris = default(ushort);
		for (int num9 = 0; num9 < mHeader.ndmeshes; num9++)
		{
			mHeader.dmeshes[num9] = default(dtStatPolyDetail);
			data = ReadUShort(data, &vbase);
			data = ReadUShort(data, &nverts2);
			data = ReadUShort(data, &tbase);
			data = ReadUShort(data, &ntris);
			mHeader.dmeshes[num9].vbase = vbase;
			mHeader.dmeshes[num9].nverts = nverts2;
			mHeader.dmeshes[num9].tbase = tbase;
			mHeader.dmeshes[num9].ntris = ntris;
		}
		float x4 = default(float);
		float y4 = default(float);
		float z4 = default(float);
		for (int num10 = 0; num10 < mHeader.ndverts; num10++)
		{
			data = ReadFloat(data, &x4);
			data = ReadFloat(data, &y4);
			data = ReadFloat(data, &z4);
			ref Vector3 reference4 = ref mHeader.dverts[num10];
			reference4 = Vector3.Zero;
			mHeader.dverts[num10].X = x4;
			mHeader.dverts[num10].Y = y4;
			mHeader.dverts[num10].Z = z4;
		}
		byte b = default(byte);
		byte b2 = default(byte);
		byte b3 = default(byte);
		byte b4 = default(byte);
		for (int num11 = 0; num11 < mHeader.ndtris; num11++)
		{
			data = ReadByte(data, &b);
			data = ReadByte(data, &b2);
			data = ReadByte(data, &b3);
			data = ReadByte(data, &b4);
			mHeader.dtris[num11][0] = b;
			mHeader.dtris[num11][1] = b2;
			mHeader.dtris[num11][2] = b3;
			mHeader.dtris[num11][3] = b4;
		}
		m_nodePool = new dtNodePool(2048);
		m_openList = new dtNodeQueue(2048);
		for (int num12 = 0; num12 < MAX_POLYS; num12++)
		{
			straightPolys[num12] = new float[3];
		}
		Initialized = true;
		return true;
	}

	public int PathInBounds(dtStatNavMeshHeader pathingData, ref Vector3 tmpVecFrom, ref Vector3 tmpVecTo)
	{
		int result = 0;
		cptbMin = pathingData.bmin + pathingData.worldOffset;
		cptbMax = pathingData.bmax + pathingData.worldOffset;
		if (tmpVecTo.X < cptbMin.X || tmpVecTo.X > cptbMax.X || tmpVecTo.Z < cptbMin.Z || tmpVecTo.Z > cptbMax.Z)
		{
			result = 1;
			if (tmpVecFrom.X > cptbMin.X + 200f && tmpVecFrom.X < cptbMax.X - 200f && tmpVecFrom.Z > cptbMin.Z + 200f && tmpVecFrom.Z < cptbMax.Z - 200f)
			{
				tmpVecTo.X = ((tmpVecTo.X < cptbMin.X) ? cptbMin.X : tmpVecTo.X);
				tmpVecTo.X = ((tmpVecTo.X > cptbMax.X) ? cptbMax.X : tmpVecTo.X);
				tmpVecTo.Z = ((tmpVecTo.Z < cptbMin.Z) ? cptbMin.Z : tmpVecTo.Z);
				tmpVecTo.Z = ((tmpVecTo.Z > cptbMax.Z) ? cptbMax.Z : tmpVecTo.Z);
			}
			else
			{
				result = 3;
			}
		}
		else if (tmpVecFrom.X < cptbMin.X || tmpVecFrom.X > cptbMax.X || tmpVecFrom.Z < cptbMin.Z || tmpVecFrom.Z > cptbMax.Z)
		{
			tmpVecTo.X = ((tmpVecTo.X < cptbMin.X) ? cptbMin.X : tmpVecTo.X);
			tmpVecTo.X = ((tmpVecTo.X > cptbMax.X) ? cptbMax.X : tmpVecTo.X);
			tmpVecTo.Z = ((tmpVecTo.Z < cptbMin.Z) ? cptbMin.Z : tmpVecTo.Z);
			tmpVecTo.Z = ((tmpVecTo.Z > cptbMax.Z) ? cptbMax.Z : tmpVecTo.Z);
			result = 2;
		}
		return result;
	}

	public int GetPath(ref Vector3 startpos, ref Vector3 endpos, ushort[] polys, Vector3[] spolys, bool randomDestination)
	{
		return GetPath(mHeader, ref startpos, ref endpos, polys, spolys, randomDestination);
	}

	public int GetPath(dtStatNavMeshHeader pathingData, ref Vector3 startpos, ref Vector3 endpos, ushort[] polys, Vector3[] spolys, bool randomDestination)
	{
		int result = 0;
		mHeader = pathingData;
		if (MaxRoutesThisUpdate > 0)
		{
			MaxRoutesThisUpdate--;
			int num = 0;
			ushort num2 = 0;
			ushort num3 = 0;
			spos = startpos - mHeader.worldOffset;
			epos = endpos - mHeader.worldOffset;
			num2 = findNearestPoly(ref spos, ref PickExtents);
			spos.Y = fnpClosestPoint.Y;
			num3 = findNearestPoly(ref epos, ref PickExtents);
			epos.Y = fnpClosestPoint.Y;
			if (num2 != 0 && num3 != 0)
			{
				num = findPath(num2, num3, ref spos, ref epos, polys, MAX_POLYS);
			}
			if (num > 0)
			{
				result = findStraightPath(ref spos, ref epos, polys, num, straightPolys, MAX_POLYS - 1);
				for (int i = 0; i < result; i++)
				{
					spolys[i].X = straightPolys[i][0];
					spolys[i].Y = straightPolys[i][1];
					spolys[i].Z = straightPolys[i][2];
					spolys[i] += mHeader.worldOffset;
				}
				ref Vector3 reference = ref spolys[result];
				reference = endpos;
				result++;
			}
		}
		return result;
	}

	public void DebugDrawNavMesh(int qIndex)
	{
	}

	public void DebugDrawNavMesh(bool inner, int qIndex)
	{
	}

	public void DebugDrawRoute(Vector3[] e, int n, int qIndex)
	{
	}

	public ushort GetValidPathPosition(ref Vector3 center, ref Vector3 extents)
	{
		if (!Initialized)
		{
			return 0;
		}
		ushort result = 0;
		for (int i = RandGen.Next(0, mHeader.nnodes); i < mHeader.nnodes; i++)
		{
			dtStatBVNode dtStatBVNode2 = mHeader.bvtree[i];
			if (dtStatBVNode2.i < 0)
			{
				continue;
			}
			int polyIndexByRef = getPolyIndexByRef((ushort)dtStatBVNode2.i);
			if (polyIndexByRef == -1 || !mHeader.polys[polyIndexByRef].IsPolyValid)
			{
				continue;
			}
			dtStatPoly poly = getPoly(polyIndexByRef);
			dtStatPolyDetail polyDetail = getPolyDetail(polyIndexByRef);
			for (int j = 0; j < polyDetail.ntris; j++)
			{
				byte[] detailTri = getDetailTri(polyDetail.tbase + j);
				ref Vector3 reference = ref cptpV[0];
				reference = ((detailTri[0] < poly.nv) ? mHeader.verts[poly.v[detailTri[0]]] : mHeader.dverts[polyDetail.vbase + (detailTri[0] - poly.nv)]);
				ref Vector3 reference2 = ref cptpV[1];
				reference2 = ((detailTri[1] < poly.nv) ? mHeader.verts[poly.v[detailTri[1]]] : mHeader.dverts[polyDetail.vbase + (detailTri[1] - poly.nv)]);
				ref Vector3 reference3 = ref cptpV[2];
				reference3 = ((detailTri[2] < poly.nv) ? mHeader.verts[poly.v[detailTri[2]]] : mHeader.dverts[polyDetail.vbase + (detailTri[2] - poly.nv)]);
				gvppV1 = cptpV[1] - cptpV[0];
				gvppV2 = cptpV[2] - cptpV[0];
				float num = gvppV1.Length();
				float num2 = gvppV2.Length();
				float num3 = (cptpV[2] - cptpV[1]).Length();
				float num4 = (num + num2 + num3) / 2f;
				float num5 = (float)Math.Sqrt(num4 * (num4 - num) * (num4 - num2) * (num4 - num3));
				if (num5 > 20000f)
				{
					gvppV1 = cptpV[0] + gvppV1 * (float)RandGen.NextDouble();
					center = gvppV1 + (cptpV[2] - gvppV1) * (float)RandGen.NextDouble();
					return 1;
				}
			}
		}
		return result;
	}

	public ushort findNearestPoly(ref Vector3 center, ref Vector3 extents)
	{
		if (!Initialized)
		{
			return 0;
		}
		ushort result = 0;
		int num = queryPolygons(ref center, ref extents, out var polys, 128);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			ushort num3 = polys[i];
			Vector3 closest = Vector3.Zero;
			if (closestPointToPoly(num3, ref center, ref closest))
			{
				float num4 = (center - closest).LengthSquared();
				if (num4 < num2)
				{
					num2 = num4;
					result = num3;
					fnpClosestPoint = closest;
				}
			}
		}
		return result;
	}

	public int queryPolygons(ref Vector3 center, ref Vector3 extents, out ushort[] polys, int maxPolys)
	{
		polys = new ushort[128];
		if (!Initialized)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		dtStatBVNode dtStatBVNode2 = mHeader.bvtree[num2];
		_ = mHeader.bvtree[mHeader.nnodes];
		float num3 = 1f / mHeader.cs;
		ushort[] array = new ushort[3];
		ushort[] array2 = new ushort[3];
		float num4 = MathHelper.Clamp(center.X - extents.X, mHeader.bmin.X, mHeader.bmax.X) - mHeader.bmin.X;
		float num5 = MathHelper.Clamp(center.Y - extents.Y, mHeader.bmin.Y, mHeader.bmax.Y) - mHeader.bmin.Y;
		float num6 = MathHelper.Clamp(center.Z - extents.Z, mHeader.bmin.Z, mHeader.bmax.Z) - mHeader.bmin.Z;
		float num7 = MathHelper.Clamp(center.X + extents.X, mHeader.bmin.X, mHeader.bmax.X) - mHeader.bmin.X;
		float num8 = MathHelper.Clamp(center.Y + extents.Y, mHeader.bmin.Y, mHeader.bmax.Y) - mHeader.bmin.Y;
		float num9 = MathHelper.Clamp(center.Z + extents.Z, mHeader.bmin.Z, mHeader.bmax.Z) - mHeader.bmin.Z;
		array[0] = (ushort)((int)(num3 * num4) & 0xFFFE);
		array[1] = (ushort)((int)(num3 * num5) & 0xFFFE);
		array[2] = (ushort)((int)(num3 * num6) & 0xFFFE);
		array2[0] = (ushort)((int)(num3 * num7 + 1f) | 1);
		array2[1] = (ushort)((int)(num3 * num8 + 1f) | 1);
		array2[2] = (ushort)((int)(num3 * num9 + 1f) | 1);
		while (num2 < mHeader.nnodes)
		{
			bool flag = checkOverlapBox(array, array2, dtStatBVNode2.bmin, dtStatBVNode2.bmax);
			bool flag2 = dtStatBVNode2.i >= 0;
			if (flag2 && flag)
			{
				bool flag3 = true;
				int polyIndexByRef = getPolyIndexByRef((ushort)dtStatBVNode2.i);
				if (polyIndexByRef != -1)
				{
					flag3 = mHeader.polys[polyIndexByRef].IsPolyValid;
				}
				if (flag3 && num < maxPolys)
				{
					polys[num] = (ushort)dtStatBVNode2.i;
					num++;
				}
			}
			if (flag || flag2)
			{
				num2++;
				dtStatBVNode2 = mHeader.bvtree[num2];
			}
			else
			{
				int num10 = -dtStatBVNode2.i;
				num2 += num10;
				dtStatBVNode2 = mHeader.bvtree[num2];
			}
		}
		return num;
	}

	public int findPath(ushort startRef, ushort endRef, ref Vector3 startPos, ref Vector3 endPos, ushort[] path, int maxPathSize)
	{
		if (!Initialized)
		{
			return 0;
		}
		if (startRef == 0 || endRef == 0)
		{
			return 0;
		}
		if (maxPathSize == 0)
		{
			return 0;
		}
		if (startRef == endRef)
		{
			path[0] = startRef;
			return 1;
		}
		m_nodePool.clear();
		m_openList.clear();
		dtNode node = m_nodePool.getNode(startRef);
		node.pidx = 0u;
		node.cost = 0f;
		node.total = (startPos - endPos).Length() * H_SCALE;
		node.id = startRef;
		node.flags = 1u;
		m_openList.push(node);
		dtNode dtNode2 = node;
		float num = node.total;
		while (!m_openList.empty())
		{
			dtNode dtNode3 = m_openList.pop();
			if (dtNode3.id == endRef)
			{
				dtNode2 = dtNode3;
				break;
			}
			dtStatPoly poly = getPoly((int)(dtNode3.id - 1));
			for (int i = 0; i < poly.nv; i++)
			{
				ushort num2 = poly.n[i];
				if (num2 == 0 || (dtNode3.pidx != 0 && m_nodePool.getNodeAtIdx(dtNode3.pidx).id == num2))
				{
					continue;
				}
				dtNode dtNode4 = dtNode3;
				dtNode dtNode5 = new dtNode();
				dtNode5.pidx = m_nodePool.getNodeIdx(dtNode4);
				dtNode5.id = num2;
				float num3 = 0f;
				Vector3 mid = Vector3.Zero;
				Vector3 mid2 = Vector3.Zero;
				if (dtNode4.pidx == 0)
				{
					mid = startPos;
				}
				else
				{
					getEdgeMidPoint((ushort)m_nodePool.getNodeAtIdx(dtNode4.pidx).id, (ushort)dtNode4.id, ref mid);
				}
				getEdgeMidPoint((ushort)dtNode4.id, (ushort)dtNode5.id, ref mid2);
				dtNode5.cost = dtNode4.cost + (mid - mid2).Length();
				if (dtNode5.id == endRef)
				{
					dtNode5.cost += (mid2 - endPos).Length();
				}
				num3 = (mid2 - endPos).Length() * H_SCALE;
				dtNode5.total = dtNode5.cost + num3;
				dtNode node2 = m_nodePool.getNode(dtNode5.id);
				if (node2 != null && ((node2.flags & 1) == 0 || !(dtNode5.total > node2.total)) && ((node2.flags & 2) == 0 || !(dtNode5.total > node2.total)))
				{
					node2.flags &= 4294967293u;
					node2.pidx = dtNode5.pidx;
					node2.cost = dtNode5.cost;
					node2.total = dtNode5.total;
					if (num3 < num)
					{
						num = num3;
						dtNode2 = node2;
					}
					if ((node2.flags & 1) != 0)
					{
						m_openList.modify(node2);
						continue;
					}
					node2.flags |= 1u;
					m_openList.push(node2);
				}
			}
			dtNode3.flags |= 2u;
		}
		if (dtNode2.id != endRef)
		{
			return 0;
		}
		dtNode dtNode6 = null;
		dtNode dtNode7 = dtNode2;
		do
		{
			dtNode nodeAtIdx = m_nodePool.getNodeAtIdx(dtNode7.pidx);
			dtNode7.pidx = m_nodePool.getNodeIdx(dtNode6);
			dtNode6 = dtNode7;
			dtNode7 = nodeAtIdx;
		}
		while (dtNode7 != null);
		dtNode7 = dtNode6;
		int num4 = 0;
		do
		{
			path[num4++] = (ushort)dtNode7.id;
			dtNode7 = m_nodePool.getNodeAtIdx(dtNode7.pidx);
		}
		while (dtNode7 != null && num4 < maxPathSize);
		return num4;
	}

	public int findStraightPath(ref Vector3 startPos, ref Vector3 endPos, ushort[] path, int pathSize, float[][] straightPath, int maxStraightPathSize)
	{
		if (!Initialized)
		{
			return 0;
		}
		if (maxStraightPathSize == 0)
		{
			return 0;
		}
		if (path[0] == 0)
		{
			return 0;
		}
		int num = 0;
		Vector3 closest = Vector3.Zero;
		if (!closestPointToPoly(path[0], ref startPos, ref closest))
		{
			return 0;
		}
		straightPath[num][0] = closest.X;
		straightPath[num][1] = closest.Y;
		straightPath[num][2] = closest.Z;
		num++;
		if (num >= maxStraightPathSize)
		{
			return num;
		}
		Vector3 closest2 = Vector3.Zero;
		if (!closestPointToPoly(path[pathSize - 1], ref endPos, ref closest2))
		{
			return 0;
		}
		float[] array = new float[3];
		float[] array2 = new float[3];
		float[] array3 = new float[3];
		if (pathSize > 1)
		{
			array[0] = closest.X;
			array[1] = closest.Y;
			array[2] = closest.Z;
			vcopy(array2, array);
			vcopy(array3, array);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < pathSize; i++)
			{
				Vector3 left = Vector3.Zero;
				Vector3 right = Vector3.Zero;
				if (i < pathSize - 1)
				{
					getPortalPoints(path[i], path[i + 1], ref left, ref right);
				}
				else
				{
					left = closest2;
					right = closest2;
				}
				if (vequal(array, array3))
				{
					array3[0] = right.X;
					array3[1] = right.Y;
					array3[2] = right.Z;
					num4 = i;
				}
				else if (triArea2D(array, array3, ref right) <= 0f)
				{
					if (!(triArea2D(array, array2, ref right) > 0f))
					{
						vcopy(array, array2);
						num2 = num3;
						if (!vequal(straightPath[num - 1], array))
						{
							vcopy(straightPath[num], array);
							num++;
							if (num >= maxStraightPathSize)
							{
								return num;
							}
						}
						vcopy(array2, array);
						vcopy(array3, array);
						num3 = num2;
						num4 = num2;
						i = num2;
						continue;
					}
					array3[0] = right.X;
					array3[1] = right.Y;
					array3[2] = right.Z;
					num4 = i;
				}
				if (vequal(array, array2))
				{
					array2[0] = left.X;
					array2[1] = left.Y;
					array2[2] = left.Z;
					num3 = i;
				}
				else
				{
					if (!(triArea2D(array, array2, ref left) >= 0f))
					{
						continue;
					}
					if (triArea2D(array, array3, ref left) < 0f)
					{
						array2[0] = left.X;
						array2[1] = left.Y;
						array2[2] = left.Z;
						num3 = i;
						continue;
					}
					vcopy(array, array3);
					num2 = num4;
					if (!vequal(straightPath[num - 1], array))
					{
						vcopy(straightPath[num], array);
						num++;
						if (num >= maxStraightPathSize)
						{
							return num;
						}
					}
					vcopy(array2, array);
					vcopy(array3, array);
					num3 = num2;
					num4 = num2;
					i = num2;
				}
			}
		}
		straightPath[num][0] = closest2.X;
		straightPath[num][1] = closest2.Y;
		straightPath[num][2] = closest2.Z;
		return num + 1;
	}

	public bool closestPointToPoly(ushort Ref, ref Vector3 pos, ref Vector3 closest)
	{
		int polyIndexByRef = getPolyIndexByRef(Ref);
		if (polyIndexByRef == -1)
		{
			return false;
		}
		float num = float.MaxValue;
		dtStatPoly poly = getPoly(polyIndexByRef);
		dtStatPolyDetail polyDetail = getPolyDetail(polyIndexByRef);
		for (int i = 0; i < polyDetail.ntris; i++)
		{
			byte[] detailTri = getDetailTri(polyDetail.tbase + i);
			ref Vector3 reference = ref cptpV[0];
			reference = ((detailTri[0] < poly.nv) ? mHeader.verts[poly.v[detailTri[0]]] : mHeader.dverts[polyDetail.vbase + (detailTri[0] - poly.nv)]);
			ref Vector3 reference2 = ref cptpV[1];
			reference2 = ((detailTri[1] < poly.nv) ? mHeader.verts[poly.v[detailTri[1]]] : mHeader.dverts[polyDetail.vbase + (detailTri[1] - poly.nv)]);
			ref Vector3 reference3 = ref cptpV[2];
			reference3 = ((detailTri[2] < poly.nv) ? mHeader.verts[poly.v[detailTri[2]]] : mHeader.dverts[polyDetail.vbase + (detailTri[2] - poly.nv)]);
			fastClosestPtPointTriangle(ref cptpPT, ref pos, cptpV);
			float num2 = (pos - cptpPT).LengthSquared();
			if (num2 < num)
			{
				closest = cptpPT;
				num = num2;
			}
		}
		return true;
	}

	public dtStatPoly getPolyByRef(ushort Ref)
	{
		if (!Initialized || Ref == 0 || Ref > mHeader.npolys)
		{
			_ = mHeader.polys[Ref - 1];
		}
		return mHeader.polys[Ref - 1];
	}

	public int getPolyIndexByRef(ushort Ref)
	{
		if (!Initialized || Ref == 0 || Ref > mHeader.npolys)
		{
			return -1;
		}
		return Ref - 1;
	}

	public int getPolyCount()
	{
		if (!Initialized)
		{
			return 0;
		}
		return mHeader.npolys;
	}

	public dtStatPoly getPoly(int i)
	{
		return mHeader.polys[i];
	}

	public int getVertexCount()
	{
		if (Initialized)
		{
			return 0;
		}
		return mHeader.nverts;
	}

	public Vector3 getVertex(int i)
	{
		return mHeader.verts[i];
	}

	public int getPolyDetailCount()
	{
		if (!Initialized)
		{
			return 0;
		}
		return mHeader.ndmeshes;
	}

	public dtStatPolyDetail getPolyDetail(int i)
	{
		return mHeader.dmeshes[i];
	}

	public int getDetailVertexCount()
	{
		if (Initialized)
		{
			return 0;
		}
		return mHeader.ndverts;
	}

	public Vector3 getDetailVertex(int i)
	{
		return mHeader.dverts[i];
	}

	public int getDetailTriCount()
	{
		if (Initialized)
		{
			return 0;
		}
		return mHeader.ndtris;
	}

	public byte[] getDetailTri(int i)
	{
		return mHeader.dtris[i];
	}

	public bool isInClosedList(ushort Ref)
	{
		if (m_nodePool == null)
		{
			return false;
		}
		dtNode dtNode2 = m_nodePool.findNode(Ref);
		if (dtNode2 != null)
		{
			return (dtNode2.flags & 2) != 0;
		}
		return false;
	}

	public int getBvTreeNodeCount()
	{
		if (Initialized)
		{
			return 0;
		}
		return mHeader.nnodes;
	}

	private bool getPortalPoints(ushort from, ushort to, ref Vector3 left, ref Vector3 right)
	{
		dtStatPoly polyByRef = getPolyByRef(from);
		int num = 0;
		int num2 = polyByRef.nv - 1;
		while (num < polyByRef.nv)
		{
			ushort num3 = polyByRef.n[num2];
			if (num3 == to)
			{
				left = getVertex(polyByRef.v[num2]);
				right = getVertex(polyByRef.v[num]);
				return true;
			}
			num2 = num++;
		}
		return false;
	}

	private bool getEdgeMidPoint(ushort from, ushort to, ref Vector3 mid)
	{
		Vector3 right = Vector3.Zero;
		Vector3 left = Vector3.Zero;
		if (!getPortalPoints(from, to, ref left, ref right))
		{
			return false;
		}
		mid.X = (left.X + right.X) * 0.5f;
		mid.Y = (left.Y + right.Y) * 0.5f;
		mid.Z = (left.Z + right.Z) * 0.5f;
		return true;
	}

	private float vdist(float[] v1, float[] v2)
	{
		float num = v2[0] - v1[0];
		float num2 = v2[1] - v1[1];
		float num3 = v2[2] - v1[2];
		return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	private void vcopy(float[] dest, float[] a)
	{
		dest[0] = a[0];
		dest[1] = a[1];
		dest[2] = a[2];
	}

	private void vsub(float[] dest, float[] v1, float[] v2)
	{
		dest[0] = v1[0] - v2[0];
		dest[1] = v1[1] - v2[1];
		dest[2] = v1[2] - v2[2];
	}

	private float vdot(float[] v1, float[] v2)
	{
		return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
	}

	private float vdistSqr(float[] v1, float[] v2)
	{
		float num = v2[0] - v1[0];
		float num2 = v2[1] - v1[1];
		float num3 = v2[2] - v1[2];
		return num * num + num2 * num2 + num3 * num3;
	}

	private float triArea2D(float[] a, float[] b, ref Vector3 c)
	{
		return (b[0] * a[2] - a[0] * b[2] + (c.X * b[2] - b[0] * c.Z) + (a[0] * c.Z - c.X * a[2])) * 0.5f;
	}

	private bool checkOverlapBox(ushort[] amin, ushort[] amax, ushort[] bmin, ushort[] bmax)
	{
		bool flag = true;
		flag = amin[0] <= bmax[0] && amax[0] >= bmin[0] && flag;
		flag = amin[1] <= bmax[1] && amax[1] >= bmin[1] && flag;
		return amin[2] <= bmax[2] && amax[2] >= bmin[2] && flag;
	}

	private bool vequal(float[] p0, float[] p1)
	{
		float num = vdistSqr(p0, p1);
		return num < thr;
	}

	private float distancePtLine2d(ref Vector3 pt, ref Vector3 p, ref Vector3 q)
	{
		float num = q.X - p.X;
		float num2 = q.Z - p.Z;
		float num3 = pt.X - p.X;
		float num4 = pt.Z - p.Z;
		float num5 = num * num + num2 * num2;
		float num6 = num * num3 + num2 * num4;
		if (num5 != 0f)
		{
			num6 /= num5;
		}
		num3 = p.X + num6 * num - pt.X;
		num4 = p.Z + num6 * num2 - pt.Z;
		return num3 * num3 + num4 * num4;
	}

	private void closestPtPointTriangle(ref Vector3 closest, ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{
		Vector3 vector = b - a;
		Vector3 vector2 = c - a;
		Vector3 vector3 = p - a;
		float num = Vector3.Dot(vector, vector3);
		float num2 = Vector3.Dot(vector2, vector3);
		if (num <= 0f && num2 <= 0f)
		{
			closest = a;
			return;
		}
		Vector3 vector4 = p - b;
		float num3 = Vector3.Dot(vector, vector4);
		float num4 = Vector3.Dot(vector2, vector4);
		if (num3 >= 0f && num4 <= num3)
		{
			closest = b;
			return;
		}
		float num5 = num * num4 - num3 * num2;
		if (num5 <= 0f && num >= 0f && num3 <= 0f)
		{
			float num6 = num / (num - num3);
			closest = a + num6 * vector;
			return;
		}
		Vector3 vector5 = p - c;
		float num7 = Vector3.Dot(vector, vector5);
		float num8 = Vector3.Dot(vector2, vector5);
		if (num8 >= 0f && num7 <= num8)
		{
			closest = c;
			return;
		}
		float num9 = num7 * num2 - num * num8;
		if (num9 <= 0f && num2 >= 0f && num8 <= 0f)
		{
			float num10 = num2 / (num2 - num8);
			closest = a + num10 * vector2;
			return;
		}
		float num11 = num3 * num8 - num7 * num4;
		if (num11 <= 0f && num4 - num3 >= 0f && num7 - num8 >= 0f)
		{
			float num12 = (num4 - num3) / (num4 - num3 + (num7 - num8));
			closest = b + num12 * (c - b);
			return;
		}
		float num13 = 1f / (num11 + num9 + num5);
		float num14 = num9 * num13;
		float num15 = num5 * num13;
		closest = a + vector * num14 + vector2 * num15;
	}

	private void fastClosestPtPointTriangle(ref Vector3 closest, ref Vector3 p, Vector3[] t)
	{
		cpptab = t[1] - t[0];
		cpptac = t[2] - t[0];
		cpptap = p - t[0];
		float num = cpptab.X * cpptap.X + cpptab.Y * cpptap.Y + cpptab.Z * cpptap.Z;
		float num2 = cpptac.X * cpptap.X + cpptac.Y * cpptap.Y + cpptac.Z * cpptap.Z;
		if (num <= 0f && num2 <= 0f)
		{
			closest = t[0];
			return;
		}
		cpptbp.X = p.X - t[1].X;
		cpptbp.Y = p.Y - t[1].Y;
		cpptbp.Z = p.Z - t[1].Z;
		float num3 = cpptab.X * cpptbp.X + cpptab.Y * cpptbp.Y + cpptab.Z * cpptbp.Z;
		float num4 = cpptac.X * cpptbp.X + cpptac.Y * cpptbp.Y + cpptac.Z * cpptbp.Z;
		if (num3 >= 0f && num4 <= num3)
		{
			closest = t[1];
			return;
		}
		float num5 = num * num4 - num3 * num2;
		if (num5 <= 0f && num >= 0f && num3 <= 0f)
		{
			float num6 = num / (num - num3);
			closest = t[0] + num6 * cpptab;
			return;
		}
		cpptcp.X = p.X - t[2].X;
		cpptcp.Y = p.Y - t[2].Y;
		cpptcp.Z = p.Z - t[2].Z;
		float num7 = cpptab.X * cpptcp.X + cpptab.Y * cpptcp.Y + cpptab.Z * cpptcp.Z;
		float num8 = cpptac.X * cpptcp.X + cpptac.Y * cpptcp.Y + cpptac.Z * cpptcp.Z;
		if (num8 >= 0f && num7 <= num8)
		{
			closest = t[2];
			return;
		}
		float num9 = num7 * num2 - num * num8;
		if (num9 <= 0f && num2 >= 0f && num8 <= 0f)
		{
			float num10 = num2 / (num2 - num8);
			closest = t[0] + num10 * cpptac;
			return;
		}
		float num11 = num3 * num8 - num7 * num4;
		if (num11 <= 0f && num4 - num3 >= 0f && num7 - num8 >= 0f)
		{
			float num12 = (num4 - num3) / (num4 - num3 + (num7 - num8));
			closest = t[1] + num12 * (t[2] - t[1]);
			return;
		}
		float num13 = 1f / (num11 + num9 + num5);
		float num14 = num9 * num13;
		float num15 = num5 * num13;
		closest = t[0] + cpptab * num14 + cpptac * num15;
	}
}

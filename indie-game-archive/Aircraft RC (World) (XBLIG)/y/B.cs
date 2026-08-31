using System;
using System.Collections;
using c;

namespace y
{
	internal static class B
	{
		public const string VisibleDistance = "";

		public const string FogEnabled = "";

		public const string FogStart = "";

		public const string FogEnd = "";

		public const string FogColor = "";

		public const string ShadowFadeStart = "";

		public const string ShadowFadeEnd = "";

		public const string ShadowCasterDist = "";

		public const string BloomAmount = "";

		public const string BloomThreshold = "";

		public const string ExposureAmount = "";

		public const string DynamicRangeEnabled = "";

		public const string HdrTime = "";

		public const string HdrMax = "";

		public const string HdrMin = "";

		public const string HdrSaturation = "";

		public const string HdrDarken = "";

		public const string HdrCinematic = "";

		public const string Gravity = "";

		public const string EditorIconScale = "";

		public const string EditorMoveScale = "";

		public const string EditorRotationScale = "";

		public const string EditorDefaultObjectScale = "";
	}
}
namespace Y
{
	[Serializable]
	internal class B : IList, c.B, ICollection, IEnumerable
	{
		private sealed class _0001CB : IEnumerator, c.B
		{
			private int HCB;

			private int HC_0002;

			private int HC_0012;

			private object HCH;

			private B HC7;

			private int HC_0001;

			public object Current
			{
				get
				{
					if (HCB == HC_0002 - 1)
					{
						throw new InvalidOperationException("Enumerator unusable (Reset pending, or past end of array.");
					}
					return HCH;
				}
			}

			public _0001CB(B list)
				: this(list, 0, list.Count)
			{
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public _0001CB(B list, int index, int count)
			{
				HC7 = list;
				HC_0002 = index;
				HC_0012 = count;
				HCB = HC_0002 - 1;
				HCH = null;
				HC_0001 = list.HCH;
			}

			public bool MoveNext()
			{
				if (HC7.HCH != HC_0001)
				{
					throw new InvalidOperationException("List has changed.");
				}
				HCB++;
				if (HCB - HC_0002 < HC_0012)
				{
					HCH = HC7[HCB];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				HCH = null;
				HCB = HC_0002 - 1;
			}
		}

		private sealed class _0001C_0002 : IEnumerator, c.B
		{
			private B HCB;

			private int HC_0002;

			private int HC_0012;

			private object HCH;

			private static object HC7 = new object();

			public object Current
			{
				get
				{
					if (HCH == HC7)
					{
						if (HC_0002 == -1)
						{
							throw new InvalidOperationException("Enumerator not started");
						}
						throw new InvalidOperationException("Enumerator ended");
					}
					return HCH;
				}
			}

			public _0001C_0002(B list)
			{
				HCB = list;
				HC_0002 = -1;
				HC_0012 = list.HCH;
				HCH = HC7;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (HC_0012 != HCB.HCH)
				{
					throw new InvalidOperationException("List has changed.");
				}
				if (++HC_0002 < HCB.Count)
				{
					HCH = HCB[HC_0002];
					return true;
				}
				HCH = HC7;
				return false;
			}

			public void Reset()
			{
				if (HC_0012 != HCB.HCH)
				{
					throw new InvalidOperationException("List has changed.");
				}
				HCH = HC7;
				HC_0002 = -1;
			}
		}

		[Serializable]
		private sealed class _0001C_0012 : B
		{
			private new sealed class _0001CB : IEnumerator, c.B
			{
				private int HCB;

				private int HC_0002;

				private int HC_0012;

				private IEnumerator HCH;

				public object Current => HCH.Current;

				public _0001CB(IEnumerator enumerator, int index, int count)
				{
					HC_0002 = 0;
					HCB = index;
					HC_0012 = count;
					HCH = enumerator;
					Reset();
				}

				public object Clone()
				{
					return MemberwiseClone();
				}

				public bool MoveNext()
				{
					if (HC_0002 >= HC_0012)
					{
						return false;
					}
					HC_0002++;
					return HCH.MoveNext();
				}

				public void Reset()
				{
					HC_0002 = 0;
					HCH.Reset();
					for (int i = 0; i < HCB; i++)
					{
						HCH.MoveNext();
					}
				}
			}

			private new IList HCB;

			public override object this[int index]
			{
				get
				{
					return HCB[index];
				}
				set
				{
					HCB[index] = value;
				}
			}

			public override int Count => HCB.Count;

			public override int Capacity
			{
				get
				{
					return HCB.Count;
				}
				set
				{
					if (value < HCB.Count)
					{
						throw new ArgumentException("capacity");
					}
				}
			}

			public override bool IsFixedSize => HCB.IsFixedSize;

			public override bool IsReadOnly => HCB.IsReadOnly;

			public override object SyncRoot => HCB.SyncRoot;

			public override bool IsSynchronized => HCB.IsSynchronized;

			public _0001C_0012(IList adaptee)
				: base(0, true)
			{
				HCB = adaptee;
			}

			public override int Add(object value)
			{
				return HCB.Add(value);
			}

			public override void Clear()
			{
				HCB.Clear();
			}

			public override bool Contains(object value)
			{
				return HCB.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return HCB.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return IndexOf(value, startIndex, HCB.Count - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > HCB.Count)
				{
					Hg("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					Hg("count", count, "Can't be less than 0.");
				}
				if (startIndex > HCB.Count - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				if (value == null)
				{
					for (int i = startIndex; i < startIndex + count; i++)
					{
						if (HCB[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (int j = startIndex; j < startIndex + count; j++)
					{
						if (value.Equals(HCB[j]))
						{
							return j;
						}
					}
				}
				return -1;
			}

			public override int LastIndexOf(object value)
			{
				return LastIndexOf(value, HCB.Count - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					Hg("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					Hg("count", count, "count is negative.");
				}
				if (startIndex - count + 1 < 0)
				{
					Hg("count", count, "count is too large.");
				}
				if (value == null)
				{
					for (int num = startIndex; num > startIndex - count; num--)
					{
						if (HCB[num] == null)
						{
							return num;
						}
					}
				}
				else
				{
					for (int num2 = startIndex; num2 > startIndex - count; num2--)
					{
						if (value.Equals(HCB[num2]))
						{
							return num2;
						}
					}
				}
				return -1;
			}

			public override void Insert(int index, object value)
			{
				HCB.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index > HCB.Count)
				{
					Hg("index", index, "Index must be >= 0 and <= Count.");
				}
				foreach (object item in c)
				{
					HCB.Insert(index++, item);
				}
			}

			public override void Remove(object value)
			{
				HCB.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				HCB.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				Hl(index, count, HCB.Count);
				for (int i = 0; i < count; i++)
				{
					HCB.RemoveAt(index);
				}
			}

			public override void Reverse()
			{
				Reverse(0, HCB.Count);
			}

			public override void Reverse(int index, int count)
			{
				Hl(index, count, HCB.Count);
				for (int i = 0; i < count / 2; i++)
				{
					object value = HCB[i + index];
					HCB[i + index] = HCB[index + count - i + index - 1];
					HCB[index + count - i + index - 1] = value;
				}
			}

			public override void SetRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index < 0 || index + c.Count > HCB.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				int num = index;
				foreach (object item in c)
				{
					HCB[num++] = item;
				}
			}

			public override void CopyTo(Array array)
			{
				HCB.CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				HCB.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				if (index < 0)
				{
					Hg("index", index, "Can't be less than zero.");
				}
				if (arrayIndex < 0)
				{
					Hg("arrayIndex", arrayIndex, "Can't be less than zero.");
				}
				if (count < 0)
				{
					Hg("index", index, "Can't be less than zero.");
				}
				if (index >= HCB.Count)
				{
					throw new ArgumentException("Can't be more or equal to list count.", "index");
				}
				if (array.Rank > 1)
				{
					throw new ArgumentException("Can't copy into multi-dimensional array.");
				}
				if (arrayIndex >= array.Length)
				{
					throw new ArgumentException("arrayIndex can't be greater than array.Length - 1.");
				}
				if (array.Length - arrayIndex + 1 < count)
				{
					throw new ArgumentException("Destination array is too small.");
				}
				if (index > HCB.Count - count)
				{
					throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
				}
				for (int i = 0; i < count; i++)
				{
					array.SetValue(HCB[index + i], arrayIndex + i);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return HCB.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				Hl(index, count, HCB.Count);
				return new _0001CB(HCB.GetEnumerator(), index, count);
			}

			public override void AddRange(ICollection c)
			{
				foreach (object item in c)
				{
					HCB.Add(item);
				}
			}

			public override int BinarySearch(object value)
			{
				return BinarySearch(value, null);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return BinarySearch(0, HCB.Count, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				Hl(index, count, HCB.Count);
				if (comparer == null)
				{
					comparer = _0002.Default;
				}
				int num = index;
				int num2 = index + count - 1;
				while (num <= num2)
				{
					int num3 = num + (num2 - num) / 2;
					int num4 = comparer.Compare(value, HCB[num3]);
					if (num4 < 0)
					{
						num2 = num3 - 1;
						continue;
					}
					if (num4 > 0)
					{
						num = num3 + 1;
						continue;
					}
					return num3;
				}
				return ~num;
			}

			public override object Clone()
			{
				return new _0001C_0012(HCB);
			}

			public override B GetRange(int index, int count)
			{
				Hl(index, count, HCB.Count);
				return new _0001CZ(this, index, count);
			}

			public override void TrimToSize()
			{
			}

			public override void Sort()
			{
				Sort(_0002.Default);
			}

			public override void Sort(IComparer comparer)
			{
				Sort(0, HCB.Count, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				Hl(index, count, HCB.Count);
				if (comparer == null)
				{
					comparer = _0002.Default;
				}
				H_0018(HCB, index, index + count - 1, comparer);
			}

			private static void HW(IList P_0, int P_1, int P_2)
			{
				object value = P_0[P_1];
				P_0[P_1] = P_0[P_2];
				P_0[P_2] = value;
			}

			internal static void H_0018(IList P_0, int P_1, int P_2, IComparer P_3)
			{
				if (P_1 >= P_2)
				{
					return;
				}
				int num = P_1 + (P_2 - P_1) / 2;
				if (P_3.Compare(P_0[num], P_0[P_1]) < 0)
				{
					HW(P_0, num, P_1);
				}
				if (P_3.Compare(P_0[P_2], P_0[P_1]) < 0)
				{
					HW(P_0, P_2, P_1);
				}
				if (P_3.Compare(P_0[P_2], P_0[num]) < 0)
				{
					HW(P_0, P_2, num);
				}
				if (P_2 - P_1 + 1 <= 3)
				{
					return;
				}
				HW(P_0, P_2 - 1, num);
				object y = P_0[P_2 - 1];
				int num2 = P_1;
				int num3 = P_2 - 1;
				while (true)
				{
					if (P_3.Compare(P_0[++num2], y) >= 0)
					{
						while (P_3.Compare(P_0[--num3], y) > 0)
						{
						}
						if (num2 >= num3)
						{
							break;
						}
						HW(P_0, num2, num3);
					}
				}
				HW(P_0, P_2 - 1, num2);
				H_0018(P_0, P_1, num2 - 1, P_3);
				H_0018(P_0, num2 + 1, P_2, P_3);
			}

			public override object[] ToArray()
			{
				object[] array = new object[HCB.Count];
				HCB.CopyTo(array, 0);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, HCB.Count);
				HCB.CopyTo(array, 0);
				return array;
			}
		}

		[Serializable]
		private class _0001CH : B
		{
			protected B m_InnerArrayList;

			public override object this[int index]
			{
				get
				{
					return m_InnerArrayList[index];
				}
				set
				{
					m_InnerArrayList[index] = value;
				}
			}

			public override int Count => m_InnerArrayList.Count;

			public override int Capacity
			{
				get
				{
					return m_InnerArrayList.Capacity;
				}
				set
				{
					m_InnerArrayList.Capacity = value;
				}
			}

			public override bool IsFixedSize => m_InnerArrayList.IsFixedSize;

			public override bool IsReadOnly => m_InnerArrayList.IsReadOnly;

			public override bool IsSynchronized => m_InnerArrayList.IsSynchronized;

			public override object SyncRoot => m_InnerArrayList.SyncRoot;

			public _0001CH(B innerArrayList)
			{
				m_InnerArrayList = innerArrayList;
			}

			public override int Add(object value)
			{
				return m_InnerArrayList.Add(value);
			}

			public override void Clear()
			{
				m_InnerArrayList.Clear();
			}

			public override bool Contains(object value)
			{
				return m_InnerArrayList.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return m_InnerArrayList.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return m_InnerArrayList.IndexOf(value, startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				return m_InnerArrayList.IndexOf(value, startIndex, count);
			}

			public override int LastIndexOf(object value)
			{
				return m_InnerArrayList.LastIndexOf(value);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return m_InnerArrayList.LastIndexOf(value, startIndex);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				return m_InnerArrayList.LastIndexOf(value, startIndex, count);
			}

			public override void Insert(int index, object value)
			{
				m_InnerArrayList.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				m_InnerArrayList.InsertRange(index, c);
			}

			public override void Remove(object value)
			{
				m_InnerArrayList.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				m_InnerArrayList.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				m_InnerArrayList.RemoveRange(index, count);
			}

			public override void Reverse()
			{
				m_InnerArrayList.Reverse();
			}

			public override void Reverse(int index, int count)
			{
				m_InnerArrayList.Reverse(index, count);
			}

			public override void SetRange(int index, ICollection c)
			{
				m_InnerArrayList.SetRange(index, c);
			}

			public override void CopyTo(Array array)
			{
				m_InnerArrayList.CopyTo(array);
			}

			public override void CopyTo(Array array, int index)
			{
				m_InnerArrayList.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return m_InnerArrayList.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				return m_InnerArrayList.GetEnumerator(index, count);
			}

			public override void AddRange(ICollection c)
			{
				m_InnerArrayList.AddRange(c);
			}

			public override int BinarySearch(object value)
			{
				return m_InnerArrayList.BinarySearch(value);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return m_InnerArrayList.BinarySearch(value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				return m_InnerArrayList.BinarySearch(index, count, value, comparer);
			}

			public override object Clone()
			{
				return m_InnerArrayList.Clone();
			}

			public override B GetRange(int index, int count)
			{
				return m_InnerArrayList.GetRange(index, count);
			}

			public override void TrimToSize()
			{
				m_InnerArrayList.TrimToSize();
			}

			public override void Sort()
			{
				m_InnerArrayList.Sort();
			}

			public override void Sort(IComparer comparer)
			{
				m_InnerArrayList.Sort(comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				m_InnerArrayList.Sort(index, count, comparer);
			}

			public override object[] ToArray()
			{
				return m_InnerArrayList.ToArray();
			}

			public override Array ToArray(Type elementType)
			{
				return m_InnerArrayList.ToArray(elementType);
			}
		}

		[Serializable]
		private sealed class _0001C7 : _0001CH
		{
			private new object HCB;

			public override object this[int index]
			{
				get
				{
					lock (HCB)
					{
						return m_InnerArrayList[index];
					}
				}
				set
				{
					lock (HCB)
					{
						m_InnerArrayList[index] = value;
					}
				}
			}

			public override int Count
			{
				get
				{
					lock (HCB)
					{
						return m_InnerArrayList.Count;
					}
				}
			}

			public override int Capacity
			{
				get
				{
					lock (HCB)
					{
						return m_InnerArrayList.Capacity;
					}
				}
				set
				{
					lock (HCB)
					{
						m_InnerArrayList.Capacity = value;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					lock (HCB)
					{
						return m_InnerArrayList.IsFixedSize;
					}
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					lock (HCB)
					{
						return m_InnerArrayList.IsReadOnly;
					}
				}
			}

			public override bool IsSynchronized => true;

			public override object SyncRoot => HCB;

			internal _0001C7(B P_0)
				: base(P_0)
			{
				HCB = P_0.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (HCB)
				{
					return m_InnerArrayList.Add(value);
				}
			}

			public override void Clear()
			{
				lock (HCB)
				{
					m_InnerArrayList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				lock (HCB)
				{
					return m_InnerArrayList.Contains(value);
				}
			}

			public override int IndexOf(object value)
			{
				lock (HCB)
				{
					return m_InnerArrayList.IndexOf(value);
				}
			}

			public override int IndexOf(object value, int startIndex)
			{
				lock (HCB)
				{
					return m_InnerArrayList.IndexOf(value, startIndex);
				}
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				lock (HCB)
				{
					return m_InnerArrayList.IndexOf(value, startIndex, count);
				}
			}

			public override int LastIndexOf(object value)
			{
				lock (HCB)
				{
					return m_InnerArrayList.LastIndexOf(value);
				}
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				lock (HCB)
				{
					return m_InnerArrayList.LastIndexOf(value, startIndex);
				}
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				lock (HCB)
				{
					return m_InnerArrayList.LastIndexOf(value, startIndex, count);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (HCB)
				{
					m_InnerArrayList.Insert(index, value);
				}
			}

			public override void InsertRange(int index, ICollection c)
			{
				lock (HCB)
				{
					m_InnerArrayList.InsertRange(index, c);
				}
			}

			public override void Remove(object value)
			{
				lock (HCB)
				{
					m_InnerArrayList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (HCB)
				{
					m_InnerArrayList.RemoveAt(index);
				}
			}

			public override void RemoveRange(int index, int count)
			{
				lock (HCB)
				{
					m_InnerArrayList.RemoveRange(index, count);
				}
			}

			public override void Reverse()
			{
				lock (HCB)
				{
					m_InnerArrayList.Reverse();
				}
			}

			public override void Reverse(int index, int count)
			{
				lock (HCB)
				{
					m_InnerArrayList.Reverse(index, count);
				}
			}

			public override void CopyTo(Array array)
			{
				lock (HCB)
				{
					m_InnerArrayList.CopyTo(array);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (HCB)
				{
					m_InnerArrayList.CopyTo(array, index);
				}
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				lock (HCB)
				{
					m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (HCB)
				{
					return m_InnerArrayList.GetEnumerator();
				}
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				lock (HCB)
				{
					return m_InnerArrayList.GetEnumerator(index, count);
				}
			}

			public override void AddRange(ICollection c)
			{
				lock (HCB)
				{
					m_InnerArrayList.AddRange(c);
				}
			}

			public override int BinarySearch(object value)
			{
				lock (HCB)
				{
					return m_InnerArrayList.BinarySearch(value);
				}
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				lock (HCB)
				{
					return m_InnerArrayList.BinarySearch(value, comparer);
				}
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				lock (HCB)
				{
					return m_InnerArrayList.BinarySearch(index, count, value, comparer);
				}
			}

			public override object Clone()
			{
				lock (HCB)
				{
					return m_InnerArrayList.Clone();
				}
			}

			public override B GetRange(int index, int count)
			{
				lock (HCB)
				{
					return m_InnerArrayList.GetRange(index, count);
				}
			}

			public override void TrimToSize()
			{
				lock (HCB)
				{
					m_InnerArrayList.TrimToSize();
				}
			}

			public override void Sort()
			{
				lock (HCB)
				{
					m_InnerArrayList.Sort();
				}
			}

			public override void Sort(IComparer comparer)
			{
				lock (HCB)
				{
					m_InnerArrayList.Sort(comparer);
				}
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				lock (HCB)
				{
					m_InnerArrayList.Sort(index, count, comparer);
				}
			}

			public override object[] ToArray()
			{
				lock (HCB)
				{
					return m_InnerArrayList.ToArray();
				}
			}

			public override Array ToArray(Type elementType)
			{
				lock (HCB)
				{
					return m_InnerArrayList.ToArray(elementType);
				}
			}
		}

		[Serializable]
		private class _0001C_0001 : _0001CH
		{
			protected virtual string ErrorMessage => "Can't add or remove from a fixed-size list.";

			public override int Capacity
			{
				get
				{
					return base.Capacity;
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public override bool IsFixedSize => true;

			public _0001C_0001(B innerList)
				: base(innerList)
			{
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class _0001Cw : _0001C_0001
		{
			protected override string ErrorMessage => "Can't modify a readonly list.";

			public override bool IsReadOnly => true;

			public override object this[int index]
			{
				get
				{
					return m_InnerArrayList[index];
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public _0001Cw(B innerArrayList)
				: base(innerArrayList)
			{
			}

			public override void Reverse()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void SetRange(int index, ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort(IComparer comparer)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class _0001CZ : _0001CH
		{
			private new int HCB;

			private new int HC_0002;

			private new int HC_0012;

			public override bool IsSynchronized => false;

			public override object this[int index]
			{
				get
				{
					if (index < 0 || index > HC_0002)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return m_InnerArrayList[HCB + index];
				}
				set
				{
					if (index < 0 || index > HC_0002)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					m_InnerArrayList[HCB + index] = value;
				}
			}

			public override int Count
			{
				get
				{
					H_000E();
					return HC_0002;
				}
			}

			public override int Capacity
			{
				get
				{
					return m_InnerArrayList.Capacity;
				}
				set
				{
					if (value < HC_0002)
					{
						throw new ArgumentOutOfRangeException();
					}
				}
			}

			public _0001CZ(B innerList, int index, int count)
				: base(innerList)
			{
				HCB = index;
				HC_0002 = count;
				HC_0012 = innerList.HCH;
			}

			private void H_000E()
			{
				if (HC_0012 != m_InnerArrayList.HCH)
				{
					throw new InvalidOperationException("ArrayList view is invalid because the underlying ArrayList was modified.");
				}
			}

			public override int Add(object value)
			{
				H_000E();
				m_InnerArrayList.Insert(HCB + HC_0002, value);
				HC_0012 = m_InnerArrayList.HCH;
				return ++HC_0002;
			}

			public override void Clear()
			{
				H_000E();
				m_InnerArrayList.RemoveRange(HCB, HC_0002);
				HC_0002 = 0;
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override bool Contains(object value)
			{
				return m_InnerArrayList.Hb(value, HCB, HC_0002);
			}

			public override int IndexOf(object value)
			{
				return IndexOf(value, 0);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return IndexOf(value, startIndex, HC_0002 - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > HC_0002)
				{
					Hg("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					Hg("count", count, "Can't be less than 0.");
				}
				if (startIndex > HC_0002 - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				int num = m_InnerArrayList.IndexOf(value, HCB + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - HCB;
			}

			public override int LastIndexOf(object value)
			{
				return LastIndexOf(value, HC_0002 - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					Hg("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					Hg("count", count, "count is negative.");
				}
				int num = m_InnerArrayList.LastIndexOf(value, HCB + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - HCB;
			}

			public override void Insert(int index, object value)
			{
				H_000E();
				if (index < 0 || index > HC_0002)
				{
					Hg("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.Insert(HCB + index, value);
				HC_0002++;
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void InsertRange(int index, ICollection c)
			{
				H_000E();
				if (index < 0 || index > HC_0002)
				{
					Hg("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.InsertRange(HCB + index, c);
				HC_0002 += c.Count;
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void Remove(object value)
			{
				H_000E();
				int num = IndexOf(value);
				if (num > -1)
				{
					RemoveAt(num);
				}
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void RemoveAt(int index)
			{
				H_000E();
				if (index < 0 || index > HC_0002)
				{
					Hg("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.RemoveAt(HCB + index);
				HC_0002--;
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void RemoveRange(int index, int count)
			{
				H_000E();
				Hl(index, count, HC_0002);
				m_InnerArrayList.RemoveRange(HCB + index, count);
				HC_0002 -= count;
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void Reverse()
			{
				Reverse(0, HC_0002);
			}

			public override void Reverse(int index, int count)
			{
				H_000E();
				Hl(index, count, HC_0002);
				m_InnerArrayList.Reverse(HCB + index, count);
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void SetRange(int index, ICollection c)
			{
				H_000E();
				if (index < 0 || index > HC_0002)
				{
					Hg("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.SetRange(HCB + index, c);
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override void CopyTo(Array array)
			{
				CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				CopyTo(0, array, index, HC_0002);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				Hl(index, count, HC_0002);
				m_InnerArrayList.CopyTo(HCB + index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return GetEnumerator(0, HC_0002);
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				Hl(index, count, HC_0002);
				return m_InnerArrayList.GetEnumerator(HCB + index, count);
			}

			public override void AddRange(ICollection c)
			{
				H_000E();
				m_InnerArrayList.InsertRange(HC_0002, c);
				HC_0002 += c.Count;
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override int BinarySearch(object value)
			{
				return BinarySearch(0, HC_0002, value, _0002.Default);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return BinarySearch(0, HC_0002, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				Hl(index, count, HC_0002);
				return m_InnerArrayList.BinarySearch(HCB + index, count, value, comparer);
			}

			public override object Clone()
			{
				return new _0001CZ((B)m_InnerArrayList.Clone(), HCB, HC_0002);
			}

			public override B GetRange(int index, int count)
			{
				Hl(index, count, HC_0002);
				return new _0001CZ(this, index, count);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException();
			}

			public override void Sort()
			{
				Sort(_0002.Default);
			}

			public override void Sort(IComparer comparer)
			{
				Sort(0, HC_0002, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				H_000E();
				Hl(index, count, HC_0002);
				m_InnerArrayList.Sort(HCB + index, count, comparer);
				HC_0012 = m_InnerArrayList.HCH;
			}

			public override object[] ToArray()
			{
				object[] array = new object[HC_0002];
				m_InnerArrayList.CopyTo(HCB, array, 0, HC_0002);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, HC_0002);
				m_InnerArrayList.CopyTo(HCB, array, 0, HC_0002);
				return array;
			}
		}

		[Serializable]
		private class _0001C_000F : IList, ICollection, IEnumerable
		{
			protected IList m_InnerList;

			public virtual object this[int index]
			{
				get
				{
					return m_InnerList[index];
				}
				set
				{
					m_InnerList[index] = value;
				}
			}

			public virtual int Count => m_InnerList.Count;

			public virtual bool IsSynchronized => m_InnerList.IsSynchronized;

			public virtual object SyncRoot => m_InnerList.SyncRoot;

			public virtual bool IsFixedSize => m_InnerList.IsFixedSize;

			public virtual bool IsReadOnly => m_InnerList.IsReadOnly;

			public _0001C_000F(IList innerList)
			{
				m_InnerList = innerList;
			}

			public virtual int Add(object value)
			{
				return m_InnerList.Add(value);
			}

			public virtual void Clear()
			{
				m_InnerList.Clear();
			}

			public virtual bool Contains(object value)
			{
				return m_InnerList.Contains(value);
			}

			public virtual int IndexOf(object value)
			{
				return m_InnerList.IndexOf(value);
			}

			public virtual void Insert(int index, object value)
			{
				m_InnerList.Insert(index, value);
			}

			public virtual void Remove(object value)
			{
				m_InnerList.Remove(value);
			}

			public virtual void RemoveAt(int index)
			{
				m_InnerList.RemoveAt(index);
			}

			public virtual void CopyTo(Array array, int index)
			{
				m_InnerList.CopyTo(array, index);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return m_InnerList.GetEnumerator();
			}
		}

		[Serializable]
		private sealed class _0001Cy : _0001C_000F
		{
			private object HCB;

			public override int Count
			{
				get
				{
					lock (HCB)
					{
						return m_InnerList.Count;
					}
				}
			}

			public override bool IsSynchronized => true;

			public override object SyncRoot
			{
				get
				{
					lock (HCB)
					{
						return m_InnerList.SyncRoot;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					lock (HCB)
					{
						return m_InnerList.IsFixedSize;
					}
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					lock (HCB)
					{
						return m_InnerList.IsReadOnly;
					}
				}
			}

			public override object this[int index]
			{
				get
				{
					lock (HCB)
					{
						return m_InnerList[index];
					}
				}
				set
				{
					lock (HCB)
					{
						m_InnerList[index] = value;
					}
				}
			}

			public _0001Cy(IList innerList)
				: base(innerList)
			{
				HCB = innerList.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (HCB)
				{
					return m_InnerList.Add(value);
				}
			}

			public override void Clear()
			{
				lock (HCB)
				{
					m_InnerList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				lock (HCB)
				{
					return m_InnerList.Contains(value);
				}
			}

			public override int IndexOf(object value)
			{
				lock (HCB)
				{
					return m_InnerList.IndexOf(value);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (HCB)
				{
					m_InnerList.Insert(index, value);
				}
			}

			public override void Remove(object value)
			{
				lock (HCB)
				{
					m_InnerList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (HCB)
				{
					m_InnerList.RemoveAt(index);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (HCB)
				{
					m_InnerList.CopyTo(array, index);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (HCB)
				{
					return m_InnerList.GetEnumerator();
				}
			}
		}

		[Serializable]
		private class _0001C6 : _0001C_000F
		{
			protected virtual string ErrorMessage => "List is fixed-size.";

			public override bool IsFixedSize => true;

			public _0001C6(IList innerList)
				: base(innerList)
			{
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class _0001CD : _0001C6
		{
			protected override string ErrorMessage => "List is read-only.";

			public override bool IsReadOnly => true;

			public override object this[int index]
			{
				get
				{
					return m_InnerList[index];
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public _0001CD(IList innerList)
				: base(innerList)
			{
			}
		}

		private const int HCB = 16;

		private int HC_0002;

		private object[] HC_0012;

		private int HCH;

		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= HC_0002)
				{
					Hg("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				return HC_0012[index];
			}
			set
			{
				if (index < 0 || index >= HC_0002)
				{
					Hg("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				HC_0012[index] = value;
				HCH++;
			}
		}

		public virtual int Count => HC_0002;

		public virtual int Capacity
		{
			get
			{
				return HC_0012.Length;
			}
			set
			{
				if (value < HC_0002)
				{
					Hg("Capacity", value, "Must be more than count.");
				}
				object[] array = new object[value];
				Array.Copy(HC_0012, 0, array, 0, HC_0002);
				HC_0012 = array;
			}
		}

		public virtual bool IsFixedSize => false;

		public virtual bool IsReadOnly => false;

		public virtual bool IsSynchronized => false;

		public virtual object SyncRoot => this;

		public B()
		{
			HC_0012 = new object[16];
		}

		public B(ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (c is Array { Rank: not 1 })
			{
				throw new RankException();
			}
			HC_0012 = new object[c.Count];
			AddRange(c);
		}

		public B(int capacity)
		{
			if (capacity < 0)
			{
				Hg("capacity", capacity, "The initial capacity can't be smaller than zero.");
			}
			if (capacity == 0)
			{
				capacity = 16;
			}
			HC_0012 = new object[capacity];
		}

		private B(int P_0, bool P_1)
		{
			if (P_1)
			{
				HC_0012 = null;
				return;
			}
			throw new InvalidOperationException("Use ArrayList(int)");
		}

		private B(object[] P_0, int P_1, int P_2)
		{
			if (P_2 == 0)
			{
				HC_0012 = new object[16];
			}
			else
			{
				HC_0012 = new object[P_2];
			}
			Array.Copy(P_0, P_1, HC_0012, 0, P_2);
			HC_0002 = P_2;
		}

		private void Hp(int P_0)
		{
			if (P_0 > HC_0012.Length)
			{
				int num = HC_0012.Length << 1;
				if (num == 0)
				{
					num = 16;
				}
				while (num < P_0)
				{
					num <<= 1;
				}
				object[] array = new object[num];
				Array.Copy(HC_0012, 0, array, 0, HC_0012.Length);
				HC_0012 = array;
			}
		}

		private void H1(int P_0, int P_1)
		{
			if (P_1 > 0)
			{
				if (HC_0002 + P_1 > HC_0012.Length)
				{
					int num;
					for (num = ((HC_0012.Length <= 0) ? 1 : (HC_0012.Length << 1)); num < HC_0002 + P_1; num <<= 1)
					{
					}
					object[] array = new object[num];
					Array.Copy(HC_0012, 0, array, 0, P_0);
					Array.Copy(HC_0012, P_0, array, P_0 + P_1, HC_0002 - P_0);
					HC_0012 = array;
				}
				else
				{
					Array.Copy(HC_0012, P_0, HC_0012, P_0 + P_1, HC_0002 - P_0);
				}
			}
			else if (P_1 < 0)
			{
				int num2 = P_0 - P_1;
				Array.Copy(HC_0012, num2, HC_0012, P_0, HC_0002 - num2);
				Array.Clear(HC_0012, HC_0002 + P_1, -P_1);
			}
		}

		public virtual int Add(object value)
		{
			if (HC_0012.Length <= HC_0002)
			{
				Hp(HC_0002 + 1);
			}
			HC_0012[HC_0002] = value;
			HCH++;
			return HC_0002++;
		}

		public virtual void Clear()
		{
			Array.Clear(HC_0012, 0, HC_0002);
			HC_0002 = 0;
			HCH++;
		}

		public virtual bool Contains(object item)
		{
			return IndexOf(item, 0, HC_0002) > -1;
		}

		internal virtual bool Hb(object P_0, int P_1, int P_2)
		{
			return IndexOf(P_0, P_1, P_2) > -1;
		}

		public virtual int IndexOf(object value)
		{
			return IndexOf(value, 0);
		}

		public virtual int IndexOf(object value, int startIndex)
		{
			return IndexOf(value, startIndex, HC_0002 - startIndex);
		}

		public virtual int IndexOf(object value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > HC_0002)
			{
				Hg("startIndex", startIndex, "Does not specify valid index.");
			}
			if (count < 0)
			{
				Hg("count", count, "Can't be less than 0.");
			}
			if (startIndex > HC_0002 - count)
			{
				throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
			}
			return Array.IndexOf(HC_0012, value, startIndex, count);
		}

		public virtual int LastIndexOf(object value)
		{
			return LastIndexOf(value, HC_0002 - 1);
		}

		public virtual int LastIndexOf(object value, int startIndex)
		{
			return LastIndexOf(value, startIndex, startIndex + 1);
		}

		public virtual int LastIndexOf(object value, int startIndex, int count)
		{
			return Array.LastIndexOf(HC_0012, value, startIndex, count);
		}

		public virtual void Insert(int index, object value)
		{
			if (index < 0 || index > HC_0002)
			{
				Hg("index", index, "Index must be >= 0 and <= Count.");
			}
			H1(index, 1);
			HC_0012[index] = value;
			HC_0002++;
			HCH++;
		}

		public virtual void InsertRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index > HC_0002)
			{
				Hg("index", index, "Index must be >= 0 and <= Count.");
			}
			int count = c.Count;
			if (HC_0012.Length < HC_0002 + count)
			{
				Hp(HC_0002 + count);
			}
			if (index < HC_0002)
			{
				Array.Copy(HC_0012, index, HC_0012, index + count, HC_0002 - index);
			}
			if (this == c.SyncRoot)
			{
				Array.Copy(HC_0012, 0, HC_0012, index, index);
				Array.Copy(HC_0012, index + count, HC_0012, index << 1, HC_0002 - index);
			}
			else
			{
				c.CopyTo(HC_0012, index);
			}
			HC_0002 += c.Count;
			HCH++;
		}

		public virtual void Remove(object obj)
		{
			int num = IndexOf(obj);
			if (num > -1)
			{
				RemoveAt(num);
			}
			HCH++;
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= HC_0002)
			{
				Hg("index", index, "Less than 0 or more than list count.");
			}
			H1(index, -1);
			HC_0002--;
			HCH++;
		}

		public virtual void RemoveRange(int index, int count)
		{
			Hl(index, count, HC_0002);
			H1(index, -count);
			HC_0002 -= count;
			HCH++;
		}

		public virtual void Reverse()
		{
			Array.Reverse(HC_0012, 0, HC_0002);
			HCH++;
		}

		public virtual void Reverse(int index, int count)
		{
			Hl(index, count, HC_0002);
			Array.Reverse(HC_0012, index, count);
			HCH++;
		}

		public virtual void CopyTo(Array array)
		{
			Array.Copy(HC_0012, array, HC_0002);
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			CopyTo(0, array, arrayIndex, HC_0002);
		}

		public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Must have only 1 dimensions.", "array");
			}
			Array.Copy(HC_0012, index, array, arrayIndex, count);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new _0001C_0002(this);
		}

		public virtual IEnumerator GetEnumerator(int index, int count)
		{
			Hl(index, count, HC_0002);
			return new _0001CB(this, index, count);
		}

		public virtual void AddRange(ICollection c)
		{
			InsertRange(HC_0002, c);
		}

		public virtual int BinarySearch(object value)
		{
			try
			{
				return Array.BinarySearch(HC_0012, 0, HC_0002, value);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual int BinarySearch(object value, IComparer comparer)
		{
			try
			{
				return Array.BinarySearch(HC_0012, 0, HC_0002, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual int BinarySearch(int index, int count, object value, IComparer comparer)
		{
			try
			{
				return Array.BinarySearch(HC_0012, index, count, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual B GetRange(int index, int count)
		{
			Hl(index, count, HC_0002);
			if (IsSynchronized)
			{
				return Synchronized(new _0001CZ(this, index, count));
			}
			return new _0001CZ(this, index, count);
		}

		public virtual void SetRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index + c.Count > HC_0002)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			c.CopyTo(HC_0012, index);
			HCH++;
		}

		public virtual void TrimToSize()
		{
			if (HC_0012.Length > HC_0002)
			{
				object[] array = ((HC_0002 != 0) ? new object[HC_0002] : new object[16]);
				Array.Copy(HC_0012, 0, array, 0, HC_0002);
				HC_0012 = array;
			}
		}

		public virtual void Sort()
		{
			Array.Sort(HC_0012, 0, HC_0002);
			HCH++;
		}

		public virtual void Sort(IComparer comparer)
		{
			Array.Sort(HC_0012, 0, HC_0002, comparer);
		}

		public virtual void Sort(int index, int count, IComparer comparer)
		{
			Hl(index, count, HC_0002);
			Array.Sort(HC_0012, index, count, comparer);
		}

		public virtual object[] ToArray()
		{
			object[] array = new object[HC_0002];
			CopyTo(array);
			return array;
		}

		public virtual Array ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, HC_0002);
			CopyTo(array);
			return array;
		}

		public virtual object Clone()
		{
			return new B(HC_0012, 0, HC_0002);
		}

		internal static void Hl(int P_0, int P_1, int P_2)
		{
			if (P_0 < 0)
			{
				Hg("index", P_0, "Can't be less than 0.");
			}
			if (P_1 < 0)
			{
				Hg("count", P_1, "Can't be less than 0.");
			}
			if (P_0 > P_2 - P_1)
			{
				throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
			}
		}

		internal static void Hg(string P_0, object P_1, string P_2)
		{
			throw new ArgumentOutOfRangeException(P_0, P_2);
		}

		public static B Adapter(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list is B result)
			{
				return result;
			}
			B b = new _0001C_0012(list);
			if (list.IsSynchronized)
			{
				return Synchronized(b);
			}
			return b;
		}

		public static B Synchronized(B list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new _0001C7(list);
		}

		public static IList Synchronized(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new _0001Cy(list);
		}

		public static B ReadOnly(B list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new _0001Cw(list);
		}

		public static IList ReadOnly(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new _0001CD(list);
		}

		public static B FixedSize(B list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new _0001C_0001(list);
		}

		public static IList FixedSize(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new _0001C6(list);
		}

		public static B Repeat(object value, int count)
		{
			B b = new B(count);
			for (int i = 0; i < count; i++)
			{
				b.Add(value);
			}
			return b;
		}
	}
}

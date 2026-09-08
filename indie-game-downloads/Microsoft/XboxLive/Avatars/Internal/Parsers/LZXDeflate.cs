using System;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class LZXDeflate
{
	public enum BlockType
	{
		Invalid,
		Verbatim,
		Aligned,
		Uncompressed
	}

	public enum DecoderState
	{
		Unknown,
		StartNewBlock,
		DecodingData
	}

	public const int MIN_MATCH = 2;

	public const int MAX_MATCH = 257;

	public const int NUM_CHARS = 256;

	public const int NUM_PRIMARY_LENGTHS = 7;

	public const int NUM_LENGTHS = 8;

	public const int NUM_SECONDARY_LENGTHS = 249;

	public const int NL_SHIFT = 3;

	public const int NUM_REPEATED_OFFSETS = 3;

	public const int ALIGNED_NUM_ELEMENTS = 8;

	public const int TREE_ENC_REP_MIN = 4;

	public const int TREE_ENC_REP_ZERO_FIRST = 16;

	public const int TREE_ENC_REP_ZERO_SECOND = 32;

	public const int TREE_ENC_REP_SAME_FIRST = 2;

	public const int TREE_ENC_REPZ_FIRST_EXTRA_BITS = 4;

	public const int TREE_ENC_REPZ_SECOND_EXTRA_BITS = 5;

	public const int TREE_ENC_REP_SAME_EXTRA_BITS = 1;

	public const int E8_CFDATA_FRAME_THRESHOLD = 32768;

	public const int MAX_WINDOW_SIZE = 2097152;

	public const int MAX_PARTITION_SIZE = 16777216;

	public const int LZXMARK_END_OF_STREAM = 255;

	public const int DECODER_PREFETCH_PAD_SIZE = 5;

	public const int CHUNK_SIZE = 32768;

	public const int MAX_GROWTH = 6144;

	public const int DS_TABLE_BITS = 8;

	public const int MAX_MAIN_TREE_ELEMENTS = 672;

	public const int MAIN_TREE_TABLE_BITS = 10;

	public const int SECONDARY_LEN_TREE_TABLE_BITS = 8;

	public const int ALIGNED_TABLE_BITS = 7;

	public const int TDECODE_STAGING_BUFFER_SIZE = 32768;

	public const int MAX_COMPRESSED_BLOCK_SIZE = 38922;

	public const int NUM_DECODE_SMALL = 20;

	public int dec_window_size;

	public int dec_window_mask;

	public byte[] dec_mem_window;

	public byte[] dec_extra_bits_table;

	public int[] MP_POS_minus2_table;

	public byte[] dec_main_tree_len;

	public byte[] dec_main_tree_prev_len;

	public byte[] dec_secondary_length_tree_len;

	public byte[] dec_secondary_length_tree_prev_len;

	public uint[] dec_last_matchpos_offset;

	public int dec_bufpos;

	public uint dec_current_file_size;

	public uint dec_instr_pos;

	public uint dec_num_cfdata_frames;

	public DecoderState dec_decoder_state;

	public int dec_block_size;

	public int dec_original_block_size;

	public BlockType dec_block_type;

	public uint dec_bitbuf;

	public sbyte dec_bitcount;

	public byte dec_num_position_slots;

	public bool dec_first_time_this_group;

	public bool dec_error_condition;

	public byte[] dec_input_buffer;

	public int dec_input_curpos;

	public int dec_end_input_pos;

	public byte[] dec_output_buffer;

	public byte[] dec_aligned_table;

	public byte[] dec_aligned_len;

	public short[] dec_main_tree_table;

	public short[] dec_main_tree_left_right;

	public short[] dec_secondary_length_tree_table;

	public short[] dec_secondary_length_tree_left_right;

	public int MAIN_TREE_ELEMENTS => 256 + (dec_num_position_slots << 3);

	public LZXDeflate(int compression_window_size)
	{
		Build_global_tables();
		dec_window_size = compression_window_size;
		dec_window_mask = dec_window_size - 1;
		if ((dec_window_size & dec_window_mask) == 0 && AllocateMemoryForDecompression())
		{
			dec_main_tree_len = new byte[672];
			dec_main_tree_prev_len = new byte[672];
			dec_secondary_length_tree_len = new byte[249];
			dec_secondary_length_tree_prev_len = new byte[249];
			dec_aligned_table = new byte[128];
			dec_aligned_len = new byte[8];
			dec_last_matchpos_offset = new uint[3];
			dec_main_tree_table = new short[1024];
			dec_main_tree_left_right = new short[2688];
			dec_secondary_length_tree_table = new short[256];
			dec_secondary_length_tree_left_right = new short[996];
			DecodeNewGroup();
		}
	}

	public bool AllocateMemoryForDecompression()
	{
		dec_num_position_slots = 4;
		long num = 4L;
		do
		{
			bool flag = true;
			num += 1L << (int)dec_extra_bits_table[dec_num_position_slots];
			dec_num_position_slots++;
		}
		while (num < dec_window_size);
		dec_mem_window = new byte[dec_window_size + 261];
		return true;
	}

	public static void ZeroArray(ref byte[] array)
	{
		Array.Clear(array, 0, array.Length);
	}

	public void DecodeNewGroup()
	{
		ZeroArray(ref dec_main_tree_len);
		ZeroArray(ref dec_main_tree_prev_len);
		ZeroArray(ref dec_secondary_length_tree_len);
		ZeroArray(ref dec_secondary_length_tree_prev_len);
		dec_last_matchpos_offset[0] = 1u;
		dec_last_matchpos_offset[1] = 1u;
		dec_last_matchpos_offset[2] = 1u;
		dec_bufpos = 0;
		dec_decoder_state = DecoderState.StartNewBlock;
		dec_block_size = 0;
		dec_original_block_size = 0;
		dec_block_type = BlockType.Invalid;
		dec_first_time_this_group = true;
		dec_current_file_size = 0u;
		dec_error_condition = false;
		dec_instr_pos = 0u;
		dec_num_cfdata_frames = 0u;
	}

	public void Build_global_tables()
	{
		MP_POS_minus2_table = new int[51];
		int[] mP_POS_minus2_table = MP_POS_minus2_table;
		if (BitConverter.IsLittleEndian)
		{
			dec_extra_bits_table = new byte[52]
			{
				0, 0, 0, 0, 1, 1, 2, 2, 3, 3,
				4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
				9, 9, 10, 10, 11, 11, 12, 12, 13, 13,
				14, 14, 15, 15, 16, 16, 17, 17, 17, 17,
				17, 17, 17, 17, 17, 17, 17, 17, 17, 17,
				17, 17
			};
		}
		else
		{
			dec_extra_bits_table = new byte[52]
			{
				0, 0, 0, 0, 2, 2, 1, 1, 4, 4,
				3, 3, 6, 6, 5, 5, 8, 8, 7, 7,
				10, 10, 9, 9, 12, 12, 11, 11, 14, 14,
				13, 13, 16, 16, 15, 15, 17, 17, 17, 17,
				17, 17, 17, 17, 17, 17, 17, 17, 17, 17,
				17, 17
			};
		}
		mP_POS_minus2_table[0] = -2;
		mP_POS_minus2_table[1] = -1;
		mP_POS_minus2_table[2] = 0;
		mP_POS_minus2_table[3] = 1;
		mP_POS_minus2_table[4] = 2;
		mP_POS_minus2_table[5] = 4;
		mP_POS_minus2_table[6] = 6;
		mP_POS_minus2_table[7] = 10;
		mP_POS_minus2_table[8] = 14;
		mP_POS_minus2_table[9] = 22;
		mP_POS_minus2_table[10] = 30;
		mP_POS_minus2_table[11] = 46;
		mP_POS_minus2_table[12] = 62;
		mP_POS_minus2_table[13] = 94;
		mP_POS_minus2_table[14] = 126;
		mP_POS_minus2_table[15] = 190;
		mP_POS_minus2_table[16] = 254;
		mP_POS_minus2_table[17] = 382;
		mP_POS_minus2_table[18] = 510;
		mP_POS_minus2_table[19] = 766;
		mP_POS_minus2_table[20] = 1022;
		mP_POS_minus2_table[21] = 1534;
		mP_POS_minus2_table[22] = 2046;
		mP_POS_minus2_table[23] = 3070;
		mP_POS_minus2_table[24] = 4094;
		mP_POS_minus2_table[25] = 6142;
		mP_POS_minus2_table[26] = 8190;
		mP_POS_minus2_table[27] = 12286;
		mP_POS_minus2_table[28] = 16382;
		mP_POS_minus2_table[29] = 24574;
		mP_POS_minus2_table[30] = 32766;
		mP_POS_minus2_table[31] = 49150;
		mP_POS_minus2_table[32] = 65534;
		mP_POS_minus2_table[33] = 98302;
		mP_POS_minus2_table[34] = 131070;
		mP_POS_minus2_table[35] = 196606;
		mP_POS_minus2_table[36] = 262142;
		mP_POS_minus2_table[37] = 393214;
		mP_POS_minus2_table[38] = 524286;
		mP_POS_minus2_table[39] = 655358;
		mP_POS_minus2_table[40] = 786430;
		mP_POS_minus2_table[41] = 917502;
		mP_POS_minus2_table[42] = 1048574;
		mP_POS_minus2_table[43] = 1179646;
		mP_POS_minus2_table[44] = 1310718;
		mP_POS_minus2_table[45] = 1441790;
		mP_POS_minus2_table[46] = 1572862;
		mP_POS_minus2_table[47] = 1703934;
		mP_POS_minus2_table[48] = 1835006;
		mP_POS_minus2_table[49] = 1966078;
		mP_POS_minus2_table[50] = 2097150;
	}

	public bool Reset()
	{
		DecodeNewGroup();
		return true;
	}

	public int Decompress(ref byte[] src, ref byte[] tg)
	{
		dec_input_buffer = src;
		dec_input_curpos = 0;
		dec_end_input_pos = src.Length;
		dec_output_buffer = tg;
		initialise_decoder_bitbuf();
		int result = decode_data(tg.Length);
		dec_num_cfdata_frames++;
		return result;
	}

	public int decode_data(int bytes_to_decode)
	{
		int num = 0;
		while (bytes_to_decode > 0)
		{
			if (dec_decoder_state == DecoderState.StartNewBlock)
			{
				if (dec_first_time_this_group)
				{
					dec_first_time_this_group = false;
					if (getbits(1) != 0)
					{
						uint num2 = getbits(16);
						uint num3 = getbits(16);
						dec_current_file_size = (num2 << 16) | num3;
					}
					else
					{
						dec_current_file_size = 0u;
					}
				}
				if (dec_block_type == BlockType.Uncompressed)
				{
					if ((dec_original_block_size & 1) > 0 && dec_input_curpos < dec_end_input_pos)
					{
						dec_input_curpos++;
					}
					dec_block_type = BlockType.Invalid;
					initialise_decoder_bitbuf();
				}
				dec_block_type = (BlockType)getbits(3);
				uint num4 = getbits(8);
				uint num5 = getbits(8);
				uint num6 = getbits(8);
				dec_block_size = (int)((num4 << 16) + (num5 << 8) + num6);
				dec_original_block_size = dec_block_size;
				if (dec_block_type == BlockType.Aligned)
				{
					read_aligned_offset_tree();
				}
				if (dec_block_type == BlockType.Verbatim || dec_block_type == BlockType.Aligned)
				{
					Buffer.BlockCopy(dec_main_tree_len, 0, dec_main_tree_prev_len, 0, MAIN_TREE_ELEMENTS);
					Buffer.BlockCopy(dec_secondary_length_tree_len, 0, dec_secondary_length_tree_prev_len, 0, 249);
					read_main_and_secondary_trees();
				}
				else
				{
					if (dec_block_type != BlockType.Uncompressed)
					{
						return -1;
					}
					if (!handle_beginning_of_uncompressed_block())
					{
						return -1;
					}
				}
				dec_decoder_state = DecoderState.DecodingData;
			}
			while (dec_block_size > 0 && bytes_to_decode > 0)
			{
				uint num7 = (uint)Math.Min(dec_block_size, bytes_to_decode);
				if (num7 == 0)
				{
					return -1;
				}
				if (decode_block(dec_block_type, dec_bufpos, num7) != 0)
				{
					return -1;
				}
				dec_block_size -= (int)num7;
				bytes_to_decode -= (int)num7;
				num += (int)num7;
			}
			if (dec_block_size == 0)
			{
				dec_decoder_state = DecoderState.StartNewBlock;
			}
			if (bytes_to_decode == 0)
			{
				initialise_decoder_bitbuf();
			}
		}
		bool flag = dec_current_file_size != 0 && dec_num_cfdata_frames < 32768;
		if (dec_bufpos > 0)
		{
			Buffer.BlockCopy(dec_mem_window, dec_bufpos - num, dec_output_buffer, 0, num);
		}
		else
		{
			Buffer.BlockCopy(dec_mem_window, dec_window_size - num, dec_output_buffer, 0, num);
		}
		if (flag)
		{
			decoder_translate_e8(ref dec_output_buffer, num);
		}
		return num;
	}

	public void fillbuf(int n)
	{
		dec_bitbuf <<= n;
		dec_bitcount = (sbyte)(dec_bitcount - n);
		if (dec_bitcount > 0)
		{
			return;
		}
		if (dec_input_curpos >= dec_end_input_pos)
		{
			dec_error_condition = true;
		}
		else
		{
			if (dec_input_curpos + 1 >= dec_end_input_pos)
			{
				return;
			}
			byte b = dec_input_buffer[dec_input_curpos++];
			byte b2 = dec_input_buffer[dec_input_curpos++];
			dec_bitbuf |= (uint)((b | (b2 << 8)) << -dec_bitcount);
			dec_bitcount += 16;
			if (dec_bitcount <= 0)
			{
				if (dec_input_curpos >= dec_end_input_pos)
				{
					dec_error_condition = true;
					return;
				}
				b = dec_input_buffer[dec_input_curpos++];
				b2 = dec_input_buffer[dec_input_curpos++];
				dec_bitbuf |= (uint)((b | (b2 << 8)) << -dec_bitcount);
				dec_bitcount += 16;
			}
		}
	}

	public uint getbits(int n)
	{
		uint result = dec_bitbuf >> 32 - n;
		fillbuf(n);
		return result;
	}

	public static bool make_table_8bit(ref byte[] bitlen, ref byte[] table)
	{
		ushort[] array = new ushort[17];
		ushort[] array2 = new ushort[17];
		ushort[] array3 = new ushort[18];
		ushort num;
		for (num = 1; num <= 16; num++)
		{
			array[num] = 0;
		}
		for (num = 0; num < 8; num++)
		{
			array[bitlen[num]]++;
		}
		array3[1] = 0;
		for (num = 1; num <= 16; num++)
		{
			array3[num + 1] = (ushort)(array3[num] + (array[num] << 16 - num));
		}
		if (array3[17] != 0)
		{
			return false;
		}
		for (num = 1; num <= 7; num++)
		{
			array3[num] >>= 9;
			array2[num] = (ushort)(1 << 7 - num);
		}
		while (num <= 16)
		{
			array2[num] = (ushort)(1 << 16 - num);
			num++;
		}
		ZeroArray(ref table);
		for (byte b = 0; b < 8; b++)
		{
			byte b2;
			if ((b2 = bitlen[b]) != 0)
			{
				ushort num2 = (ushort)(array3[b2] + array2[b2]);
				if (num2 > 128)
				{
					return false;
				}
				for (num = array3[b2]; num < num2; num++)
				{
					table[num] = b;
				}
				array3[b2] = num2;
			}
		}
		return true;
	}

	public bool read_aligned_offset_tree()
	{
		for (int i = 0; i < 8; i++)
		{
			dec_aligned_len[i] = (byte)getbits(3);
		}
		if (dec_error_condition)
		{
			return false;
		}
		if (!make_table_8bit(ref dec_aligned_len, ref dec_aligned_table))
		{
			return false;
		}
		return true;
	}

	public static bool make_table(int nchar, ref byte[] bitlen, byte tablebits, ref short[] table, ref short[] leftright)
	{
		uint[] array = new uint[17];
		uint[] array2 = new uint[17];
		uint[] array3 = new uint[18];
		uint num;
		for (num = 1u; num <= 16; num++)
		{
			array[num] = 0u;
		}
		for (num = 0u; num < (uint)nchar; num++)
		{
			array[bitlen[num]]++;
		}
		array3[1] = 0u;
		for (num = 1u; num <= 16; num++)
		{
			array3[num + 1] = array3[num] + (array[num] << (int)(byte)(16 - num));
		}
		if (array3[17] != 65536)
		{
			if (array3[17] == 0)
			{
				for (int i = 0; i < (uint)(1 << (int)tablebits); i++)
				{
					table[i] = 0;
				}
				return true;
			}
			return false;
		}
		byte b = (byte)(16 - tablebits);
		for (num = 1u; num <= tablebits; num++)
		{
			array3[num] >>= (int)b;
			array2[num] = (uint)(1 << (int)(byte)(tablebits - num));
		}
		for (; num <= 16; num++)
		{
			array2[num] = (uint)(1 << (int)(byte)(16 - num));
		}
		num = array3[tablebits + 1] >> (int)b;
		if (num != 65536)
		{
			for (uint num2 = 0u; num2 < (1 << (int)tablebits) - num; num2++)
			{
				table[num + num2] = 0;
			}
		}
		int num3 = nchar;
		for (int j = 0; j < nchar; j++)
		{
			byte b2;
			if ((b2 = bitlen[j]) == 0)
			{
				continue;
			}
			uint num4 = array3[b2] + array2[b2];
			if (b2 <= tablebits)
			{
				if (num4 > (uint)(1 << (int)tablebits))
				{
					return false;
				}
				for (num = array3[b2]; num < num4; num++)
				{
					table[num] = (short)j;
				}
				array3[b2] = num4;
				continue;
			}
			uint num5 = array3[b2];
			array3[b2] = num4;
			short[] array4 = table;
			int num6 = (int)(num5 >> (int)b);
			byte b3 = (byte)(b2 - tablebits);
			num5 <<= (int)tablebits;
			do
			{
				if (array4[num6] == 0)
				{
					leftright[num3 * 2] = (leftright[num3 * 2 + 1] = 0);
					array4[num6] = (short)(-num3);
					num3++;
				}
				if ((short)num5 < 0)
				{
					num6 = -array4[num6] * 2 + 1;
					array4 = leftright;
				}
				else
				{
					num6 = -array4[num6] * 2;
					array4 = leftright;
				}
				num5 <<= 1;
				b3--;
			}
			while (b3 > 0);
			array4[num6] = (short)j;
		}
		return true;
	}

	public void DECODE_FILLBUF(int n)
	{
		dec_bitbuf <<= n;
		dec_bitcount -= (sbyte)n;
		if (dec_bitcount > 0)
		{
			return;
		}
		if (dec_input_curpos >= dec_end_input_pos)
		{
			dec_error_condition = true;
			return;
		}
		dec_bitbuf |= (uint)((dec_input_buffer[dec_input_curpos] | (dec_input_buffer[dec_input_curpos + 1] << 8)) << -dec_bitcount);
		dec_input_curpos += 2;
		dec_bitcount += 16;
		if (dec_bitcount <= 0)
		{
			if (dec_input_curpos >= dec_end_input_pos)
			{
				dec_error_condition = true;
				return;
			}
			dec_bitbuf |= (uint)((dec_input_buffer[dec_input_curpos] | (dec_input_buffer[dec_input_curpos + 1] << 8)) << -dec_bitcount);
			dec_input_curpos += 2;
			dec_bitcount += 16;
		}
	}

	public bool DECODE_SMALL(ref int small_table_idx, ref short[] small_table, ref uint mask, ref int leftright_idx, ref short[] leftright_s, ref byte[] small_bitlen, ref short item)
	{
		small_table_idx = (int)(dec_bitbuf >> 24);
		if (small_table_idx >= small_table.Length)
		{
			dec_error_condition = true;
			return false;
		}
		item = small_table[small_table_idx];
		if (item < 0)
		{
			int num = 23;
			mask = (uint)(1 << num);
			do
			{
				item = (short)(-item);
				if ((dec_bitbuf & mask) != 0)
				{
					leftright_idx = 2 * item + 1;
				}
				else
				{
					leftright_idx = 2 * item;
				}
				if (leftright_idx >= leftright_s.Length)
				{
					dec_error_condition = true;
					return false;
				}
				item = leftright_s[leftright_idx];
				mask >>= 1;
			}
			while (item < 0);
		}
		if (item >= small_bitlen.Length)
		{
			dec_error_condition = true;
			return false;
		}
		DECODE_FILLBUF(small_bitlen[item]);
		return true;
	}

	public void DECODE_GETBITS(ref int dest, int n)
	{
		dest = (byte)(dec_bitbuf >> 32 - n);
		DECODE_FILLBUF(n);
	}

	public bool ReadRepTree(int num_elements, ref byte[] dec_tree_prev_len, int lastlen, ref byte[] dec_tree_len, int len)
	{
		uint mask = 0u;
		int dest = 0;
		byte[] bitlen = new byte[24];
		short[] table = new short[256];
		short[] leftright = new short[94];
		short item = 0;
		int small_table_idx = 0;
		int leftright_idx = 0;
		for (int i = 0; i < 20; i++)
		{
			bitlen[i] = (byte)getbits(4);
		}
		if (dec_error_condition)
		{
			return false;
		}
		make_table(20, ref bitlen, 8, ref table, ref leftright);
		for (int j = 0; j < num_elements; j++)
		{
			DECODE_SMALL(ref small_table_idx, ref table, ref mask, ref leftright_idx, ref leftright, ref bitlen, ref item);
			if (dec_error_condition)
			{
				break;
			}
			switch (item)
			{
			case 17:
				DECODE_GETBITS(ref dest, 4);
				dest += 4;
				if (j + dest >= num_elements)
				{
					dest = num_elements - j;
				}
				while (dest-- > 0)
				{
					dec_tree_len[len + j] = 0;
					j++;
				}
				j--;
				break;
			case 18:
				DECODE_GETBITS(ref dest, 5);
				dest += 20;
				if (j + dest >= num_elements)
				{
					dest = num_elements - j;
				}
				while (dest-- > 0)
				{
					dec_tree_len[len + j] = 0;
					j++;
				}
				j--;
				break;
			case 19:
			{
				DECODE_GETBITS(ref dest, 1);
				dest += 4;
				if (j + dest >= num_elements)
				{
					dest = num_elements - j;
				}
				DECODE_SMALL(ref small_table_idx, ref table, ref mask, ref leftright_idx, ref leftright, ref bitlen, ref item);
				int num2 = dec_tree_prev_len[lastlen + j] - item + 17;
				if (num2 >= 17)
				{
					num2 -= 17;
				}
				byte b2 = (byte)num2;
				while (dest-- > 0)
				{
					dec_tree_len[len + j] = b2;
					j++;
				}
				j--;
				break;
			}
			default:
			{
				int num = dec_tree_prev_len[lastlen + j] - item + 17;
				if (num >= 17)
				{
					num -= 17;
				}
				byte b = (byte)num;
				dec_tree_len[len + j] = b;
				break;
			}
			}
		}
		return !dec_error_condition;
	}

	public bool read_main_and_secondary_trees()
	{
		if (!ReadRepTree(256, ref dec_main_tree_prev_len, 0, ref dec_main_tree_len, 0))
		{
			return false;
		}
		if (!ReadRepTree(dec_num_position_slots * 8, ref dec_main_tree_prev_len, 256, ref dec_main_tree_len, 256))
		{
			return false;
		}
		if (!make_table(MAIN_TREE_ELEMENTS, ref dec_main_tree_len, 10, ref dec_main_tree_table, ref dec_main_tree_left_right))
		{
			return false;
		}
		if (!ReadRepTree(249, ref dec_secondary_length_tree_prev_len, 0, ref dec_secondary_length_tree_len, 0))
		{
			return false;
		}
		if (!make_table(249, ref dec_secondary_length_tree_len, 8, ref dec_secondary_length_tree_table, ref dec_secondary_length_tree_left_right))
		{
			return false;
		}
		return true;
	}

	public void decoder_translate_e8(ref byte[] mem, long bytes)
	{
		byte[] array = new byte[6];
		int num = 0;
		if (bytes <= 6)
		{
			dec_instr_pos += (uint)(int)bytes;
			return;
		}
		byte[] dst = mem;
		Buffer.BlockCopy(mem, (int)(bytes - 6), array, 0, 6);
		mem[bytes - 6] = 232;
		mem[bytes - 5] = 232;
		mem[bytes - 4] = 232;
		mem[bytes - 3] = 232;
		mem[bytes - 2] = 232;
		mem[bytes - 1] = 232;
		uint num2 = (uint)(dec_instr_pos + bytes - 10);
		while (true)
		{
			bool flag = true;
			uint num3 = 0u;
			int num4 = 0;
			while (mem[num4++] != 232)
			{
				num3++;
			}
			num = num4;
			dec_instr_pos += num3;
			if (dec_instr_pos >= num2)
			{
				break;
			}
			uint num5 = BitConverter.ToUInt32(mem, num);
			if (num5 < dec_current_file_size)
			{
				byte[] bytes2 = BitConverter.GetBytes(num5 - dec_instr_pos);
				mem[num] = bytes2[0];
				mem[num + 1] = bytes2[1];
				mem[num + 2] = bytes2[2];
				mem[num + 3] = bytes2[3];
			}
			else if ((ulong)(0L - (long)num5) <= (ulong)dec_instr_pos)
			{
				byte[] bytes3 = BitConverter.GetBytes(num5 + dec_current_file_size);
				mem[num] = bytes3[0];
				mem[num + 1] = bytes3[1];
				mem[num + 2] = bytes3[2];
				mem[num + 3] = bytes3[3];
			}
			num += 4;
			dec_instr_pos += 5u;
		}
		dec_instr_pos = num2 + 10;
		Buffer.BlockCopy(array, 0, dst, (int)(bytes - 6), 6);
	}

	public int decode_block(BlockType block_type, int bufpos, uint amount_to_decode)
	{
		return block_type switch
		{
			BlockType.Aligned => decode_aligned_offset_block(bufpos, (int)amount_to_decode), 
			BlockType.Verbatim => decode_verbatim_block(bufpos, (int)amount_to_decode), 
			BlockType.Uncompressed => decode_uncompressed_block(bufpos, (int)amount_to_decode), 
			_ => -1, 
		};
	}

	public int decode_aligned_offset_block(int bufpos, int amount_to_decode)
	{
		if (bufpos < 257)
		{
			int amount_to_decode2 = Math.Min(257 - bufpos, amount_to_decode);
			int num = special_decode_aligned_block(bufpos, amount_to_decode2);
			amount_to_decode -= num - bufpos;
			dec_bufpos = (bufpos = num);
			if (amount_to_decode <= 0)
			{
				return amount_to_decode;
			}
		}
		return fast_decode_aligned_offset_block(bufpos, amount_to_decode);
	}

	public int special_decode_aligned_block(int bufpos, int amount_to_decode)
	{
		int num = bufpos + amount_to_decode;
		while (bufpos < num)
		{
			int num2 = DecodeMainTree();
			if ((num2 -= 256) < 0)
			{
				dec_mem_window[bufpos] = (byte)num2;
				dec_mem_window[dec_window_size + bufpos] = (byte)num2;
				bufpos++;
				continue;
			}
			int matchlen;
			if ((matchlen = num2 & 7) == 7)
			{
				DecodeLenTreeNoEofCheck(ref matchlen);
			}
			sbyte b = (sbyte)(num2 >> 3);
			uint num4;
			if (b > 2)
			{
				if (dec_extra_bits_table[b] >= 3)
				{
					uint num3 = ((dec_extra_bits_table[b] - 3 > 0) ? GetBitsNoEofCheck(dec_extra_bits_table[b] - 3) : 0u);
					num4 = (uint)MP_POS_minus2_table[b] + (num3 << 3);
					num3 = DecodeAlignedNoEofCheck();
					num4 += num3;
				}
				else if (dec_extra_bits_table[b] > 0)
				{
					num4 = GetBitsNoEofCheck(dec_extra_bits_table[b]);
					num4 += (uint)MP_POS_minus2_table[b];
				}
				else
				{
					num4 = 1u;
				}
				dec_last_matchpos_offset[2] = dec_last_matchpos_offset[1];
				dec_last_matchpos_offset[1] = dec_last_matchpos_offset[0];
				dec_last_matchpos_offset[0] = num4;
			}
			else
			{
				num4 = dec_last_matchpos_offset[b];
				dec_last_matchpos_offset[b] = dec_last_matchpos_offset[0];
				dec_last_matchpos_offset[0] = num4;
			}
			matchlen += 2;
			do
			{
				uint num5 = dec_mem_window[(bufpos - num4) & dec_window_mask];
				dec_mem_window[bufpos] = (byte)num5;
				if (bufpos < 257)
				{
					dec_mem_window[dec_window_size + bufpos] = (byte)num5;
				}
				bufpos++;
			}
			while (--matchlen > 0);
		}
		return bufpos;
	}

	public int fast_decode_aligned_offset_block(int bufpos, int amount_to_decode)
	{
		int num = bufpos + amount_to_decode;
		while (bufpos < num)
		{
			int num2 = DecodeMainTree();
			if ((num2 -= 256) < 0)
			{
				dec_mem_window[bufpos++] = (byte)num2;
				continue;
			}
			int matchlen;
			if ((matchlen = num2 & 7) == 7)
			{
				DecodeLenTreeNoEofCheck(ref matchlen);
			}
			sbyte b = (sbyte)(num2 >> 3);
			uint num4;
			if (b > 2)
			{
				if (dec_extra_bits_table[b] >= 3)
				{
					uint num3 = ((dec_extra_bits_table[b] - 3 > 0) ? GetBitsNoEofCheck(dec_extra_bits_table[b] - 3) : 0u);
					num4 = (uint)MP_POS_minus2_table[b] + (num3 << 3);
					num3 = DecodeAlignedNoEofCheck();
					num4 += num3;
				}
				else if (dec_extra_bits_table[b] > 0)
				{
					num4 = GetBitsNoEofCheck(dec_extra_bits_table[b]);
					num4 += (uint)MP_POS_minus2_table[b];
				}
				else
				{
					num4 = (uint)MP_POS_minus2_table[b];
				}
				dec_last_matchpos_offset[2] = dec_last_matchpos_offset[1];
				dec_last_matchpos_offset[1] = dec_last_matchpos_offset[0];
				dec_last_matchpos_offset[0] = num4;
			}
			else
			{
				num4 = dec_last_matchpos_offset[b];
				dec_last_matchpos_offset[b] = dec_last_matchpos_offset[0];
				dec_last_matchpos_offset[0] = num4;
			}
			matchlen += 2;
			uint num5 = (uint)((bufpos - (int)num4) & dec_window_mask);
			do
			{
				dec_mem_window[bufpos++] = dec_mem_window[num5++];
			}
			while (--matchlen > 0);
		}
		int result = bufpos - num;
		bufpos &= dec_window_mask;
		dec_bufpos = bufpos;
		return result;
	}

	public int decode_verbatim_block(int bufpos, int amount_to_decode)
	{
		if (bufpos < 257)
		{
			int amount_to_decode2 = Math.Min(257 - bufpos, amount_to_decode);
			int num = special_decode_verbatim_block(bufpos, amount_to_decode2);
			amount_to_decode -= num - bufpos;
			dec_bufpos = (bufpos = num);
			if (amount_to_decode <= 0)
			{
				return amount_to_decode;
			}
		}
		return fast_decode_verbatim_block(bufpos, amount_to_decode);
	}

	public int special_decode_verbatim_block(int bufpos, int amount_to_decode)
	{
		int num = bufpos + amount_to_decode;
		while (bufpos < num)
		{
			int num2 = DecodeMainTree();
			if ((num2 -= 256) < 0)
			{
				dec_mem_window[bufpos] = (byte)num2;
				dec_mem_window[dec_window_size + bufpos] = (byte)num2;
				bufpos++;
				continue;
			}
			int matchlen;
			if ((matchlen = num2 & 7) == 7)
			{
				DecodeLenTreeNoEofCheck(ref matchlen);
			}
			sbyte b = (sbyte)(num2 >> 3);
			uint bits17NoEofCheck;
			if (b > 2)
			{
				if (b > 3)
				{
					bits17NoEofCheck = GetBits17NoEofCheck(dec_extra_bits_table[b]);
					bits17NoEofCheck += (uint)MP_POS_minus2_table[b];
				}
				else
				{
					bits17NoEofCheck = 1u;
				}
				dec_last_matchpos_offset[2] = dec_last_matchpos_offset[1];
				dec_last_matchpos_offset[1] = dec_last_matchpos_offset[0];
				dec_last_matchpos_offset[0] = bits17NoEofCheck;
			}
			else
			{
				bits17NoEofCheck = dec_last_matchpos_offset[b];
				if (b > 0)
				{
					dec_last_matchpos_offset[b] = dec_last_matchpos_offset[0];
					dec_last_matchpos_offset[0] = bits17NoEofCheck;
				}
			}
			matchlen += 2;
			do
			{
				dec_mem_window[bufpos] = dec_mem_window[(bufpos - bits17NoEofCheck) & dec_window_mask];
				if (bufpos < 257)
				{
					dec_mem_window[dec_window_size + bufpos] = dec_mem_window[bufpos];
				}
				bufpos++;
			}
			while (--matchlen > 0);
		}
		return bufpos;
	}

	public int fast_decode_verbatim_block(int bufpos, int amount_to_decode)
	{
		int num = bufpos + amount_to_decode;
		while (bufpos < num)
		{
			int num2 = DecodeMainTree();
			if ((num2 -= 256) < 0)
			{
				dec_mem_window[bufpos++] = (byte)num2;
				continue;
			}
			int matchlen;
			if ((matchlen = num2 & 7) == 7)
			{
				DecodeLenTreeNoEofCheck(ref matchlen);
			}
			sbyte b = (sbyte)(num2 >> 3);
			uint bits17NoEofCheck;
			if (b > 2)
			{
				if (b > 3)
				{
					bits17NoEofCheck = GetBits17NoEofCheck(dec_extra_bits_table[b]);
					bits17NoEofCheck += (uint)MP_POS_minus2_table[b];
				}
				else
				{
					bits17NoEofCheck = (uint)MP_POS_minus2_table[3];
				}
				dec_last_matchpos_offset[2] = dec_last_matchpos_offset[1];
				dec_last_matchpos_offset[1] = dec_last_matchpos_offset[0];
				dec_last_matchpos_offset[0] = bits17NoEofCheck;
			}
			else
			{
				bits17NoEofCheck = dec_last_matchpos_offset[b];
				if (b > 0)
				{
					dec_last_matchpos_offset[b] = dec_last_matchpos_offset[0];
					dec_last_matchpos_offset[0] = bits17NoEofCheck;
				}
			}
			matchlen += 2;
			uint num3 = (uint)((bufpos - (int)bits17NoEofCheck) & dec_window_mask);
			do
			{
				dec_mem_window[bufpos++] = dec_mem_window[num3++];
			}
			while (--matchlen > 0);
		}
		int result = bufpos - num;
		bufpos &= dec_window_mask;
		dec_bufpos = bufpos;
		return result;
	}

	public int decode_uncompressed_block(int bufpos, int amount_to_decode)
	{
		int num = bufpos + amount_to_decode;
		int num2 = Math.Min(amount_to_decode, dec_end_input_pos - dec_input_curpos);
		int num3 = bufpos;
		int num4 = bufpos + num2;
		int num5 = bufpos;
		while (bufpos < num4)
		{
			dec_mem_window[bufpos++] = dec_input_buffer[dec_input_curpos++];
		}
		if (num4 != num)
		{
			return -1;
		}
		int num6 = Math.Min(257, num);
		num2 = num6 - num3;
		num5 = num3 + dec_window_size;
		int num7 = num3;
		int num8 = num5 + num2;
		while (num5 < num8)
		{
			dec_mem_window[num5++] = dec_input_buffer[num7++];
		}
		int result = bufpos - num;
		bufpos &= dec_window_mask;
		dec_bufpos = bufpos;
		return result;
	}

	public bool handle_beginning_of_uncompressed_block()
	{
		dec_input_curpos -= 2;
		if (dec_input_curpos + 4 >= dec_end_input_pos)
		{
			return false;
		}
		for (int i = 0; i < 3; i++)
		{
			byte b = dec_input_buffer[dec_input_curpos++];
			byte b2 = dec_input_buffer[dec_input_curpos++];
			byte b3 = dec_input_buffer[dec_input_curpos++];
			byte b4 = dec_input_buffer[dec_input_curpos++];
			dec_last_matchpos_offset[i] = (uint)(b | (b2 << 8) | (b3 << 16) | (b4 << 24));
		}
		return true;
	}

	public uint DecodeAlignedNoEofCheck()
	{
		uint num = dec_aligned_table[dec_bitbuf >> 25];
		FillBufNoEofCheck(dec_aligned_len[num]);
		return num;
	}

	public void initialise_decoder_bitbuf()
	{
		if (dec_block_type != BlockType.Uncompressed && dec_input_curpos + 4 <= dec_end_input_pos)
		{
			byte b = dec_input_buffer[dec_input_curpos++];
			byte b2 = dec_input_buffer[dec_input_curpos++];
			byte b3 = dec_input_buffer[dec_input_curpos++];
			byte b4 = dec_input_buffer[dec_input_curpos++];
			dec_bitbuf = (uint)(b3 | (b4 << 8) | ((b | (b2 << 8)) << 16));
			dec_bitcount = 16;
		}
	}

	public void FillBufFillCheck(int N)
	{
		if (dec_input_curpos <= dec_end_input_pos)
		{
			dec_bitbuf <<= N;
			dec_bitcount = (sbyte)(dec_bitcount - N);
			if (dec_bitcount <= 0 && dec_input_curpos + 1 < dec_end_input_pos)
			{
				byte b = dec_input_buffer[dec_input_curpos++];
				byte b2 = dec_input_buffer[dec_input_curpos++];
				dec_bitbuf |= (uint)((b | (b2 << 8)) << -dec_bitcount);
				dec_bitcount += 16;
			}
		}
	}

	public void FillBufNoEofCheck(int N)
	{
		dec_bitbuf <<= N;
		dec_bitcount = (sbyte)(dec_bitcount - N);
		if (dec_bitcount <= 0 && dec_input_curpos + 1 < dec_end_input_pos)
		{
			byte b = dec_input_buffer[dec_input_curpos++];
			byte b2 = dec_input_buffer[dec_input_curpos++];
			dec_bitbuf |= (uint)((b | (b2 << 8)) << -dec_bitcount);
			dec_bitcount += 16;
		}
	}

	public void FillBuf17NoEofCheck(int N)
	{
		dec_bitbuf <<= N;
		dec_bitcount = (sbyte)(dec_bitcount - N);
		if (dec_bitcount <= 0 && dec_input_curpos + 1 < dec_end_input_pos)
		{
			byte b = dec_input_buffer[dec_input_curpos++];
			byte b2 = dec_input_buffer[dec_input_curpos++];
			dec_bitbuf |= (uint)((b | (b2 << 8)) << -dec_bitcount);
			dec_bitcount += 16;
			if (dec_bitcount <= 0)
			{
				b = dec_input_buffer[dec_input_curpos++];
				b2 = dec_input_buffer[dec_input_curpos++];
				dec_bitbuf |= (uint)((b | (b2 << 8)) << -dec_bitcount);
				dec_bitcount += 16;
			}
		}
	}

	public int DecodeMainTree()
	{
		int num = dec_main_tree_table[dec_bitbuf >> 22];
		if (num < 0)
		{
			int num2 = 21;
			uint num3 = (uint)(1 << num2);
			do
			{
				num = -num;
				num = (((dec_bitbuf & num3) == 0) ? dec_main_tree_left_right[num * 2] : dec_main_tree_left_right[num * 2 + 1]);
				num3 >>= 1;
			}
			while (num < 0);
		}
		FillBufFillCheck(dec_main_tree_len[num]);
		return num;
	}

	public void DecodeLenTreeNoEofCheck(ref int matchlen)
	{
		matchlen = dec_secondary_length_tree_table[dec_bitbuf >> 24];
		if (matchlen < 0)
		{
			int num = 23;
			uint num2 = (uint)(1 << num);
			do
			{
				matchlen = -matchlen;
				if ((dec_bitbuf & num2) != 0)
				{
					matchlen = dec_secondary_length_tree_left_right[matchlen * 2 + 1];
				}
				else
				{
					matchlen = dec_secondary_length_tree_left_right[matchlen * 2];
				}
				num2 >>= 1;
			}
			while (matchlen < 0);
		}
		FillBufNoEofCheck(dec_secondary_length_tree_len[matchlen]);
		matchlen += 7;
	}

	public uint GetBitsNoEofCheck(int N)
	{
		uint result = dec_bitbuf >> 32 - N;
		FillBufNoEofCheck(N);
		return result;
	}

	public uint GetBits17NoEofCheck(int N)
	{
		uint result = dec_bitbuf >> 32 - N;
		FillBuf17NoEofCheck(N);
		return result;
	}
}

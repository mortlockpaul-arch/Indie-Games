using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace XnaToFna.XEX;

public class XEXImageData
{
	public class XEXRatings
	{
		public byte rating_esrb;

		public byte rating_pegi;

		public byte rating_pegifi;

		public byte rating_pegipt;

		public byte rating_bbfc;

		public byte rating_cero;

		public byte rating_usk;

		public byte rating_oflcau;

		public byte rating_oflcnz;

		public byte rating_kmrb;

		public byte rating_brazil;

		public byte rating_fpb;

		public XEXRatings(BinaryReader reader)
		{
			rating_esrb = reader.ReadByte();
			rating_pegi = reader.ReadByte();
			rating_pegifi = reader.ReadByte();
			rating_pegipt = reader.ReadByte();
			rating_bbfc = reader.ReadByte();
			rating_cero = reader.ReadByte();
			rating_usk = reader.ReadByte();
			rating_oflcau = reader.ReadByte();
			rating_oflcnz = reader.ReadByte();
			rating_kmrb = reader.ReadByte();
			rating_brazil = reader.ReadByte();
			rating_fpb = reader.ReadByte();
		}
	}

	public class XEXVersion
	{
		public uint value;

		public XEXVersion(BinaryReader reader)
		{
			value = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXOptionalHeader
	{
		public uint offset;

		public uint length;

		public uint value;

		public XEXHeaderKeys key;

		public XEXOptionalHeader(BinaryReader reader)
		{
			key = (XEXHeaderKeys)ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			offset = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			length = 0u;
			value = 0u;
		}
	}

	public class XEXResourceInfo
	{
		public string name;

		public uint address;

		public uint size;

		public XEXResourceInfo(BinaryReader reader)
		{
			name = new string(reader.ReadChars(8)).TrimEnd(default(char));
			address = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXExecutionInfo
	{
		public uint media_id;

		public XEXVersion version;

		public XEXVersion base_version;

		public uint title_id;

		public byte platform;

		public byte executable_table;

		public byte disc_number;

		public byte disc_count;

		public uint savegame_id;

		public XEXExecutionInfo(BinaryReader reader)
		{
			media_id = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			version = new XEXVersion(reader);
			base_version = new XEXVersion(reader);
			title_id = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			platform = reader.ReadByte();
			executable_table = reader.ReadByte();
			disc_number = reader.ReadByte();
			disc_count = reader.ReadByte();
			savegame_id = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXTLSInfo
	{
		public uint slot_count;

		public uint raw_data_address;

		public uint data_size;

		public uint raw_data_size;

		public XEXTLSInfo(BinaryReader reader)
		{
			slot_count = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			raw_data_address = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			data_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			raw_data_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXImportLibraryBlockHeader
	{
		public uint string_table_size;

		public uint count;

		public XEXImportLibraryBlockHeader(BinaryReader reader)
		{
			string_table_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			count = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXImportLibraryHeader
	{
		public uint unknown;

		public byte[] digest = new byte[20];

		public uint import_id;

		public XEXVersion version;

		public XEXVersion min_version;

		public ushort name_index;

		public ushort record_count;

		public XEXImportLibraryHeader(BinaryReader reader)
		{
			unknown = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(digest, 0, digest.Length);
			version = new XEXVersion(reader);
			min_version = new XEXVersion(reader);
			name_index = ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
			record_count = ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
		}
	}

	public class XEXStaticLibrary
	{
		public string name;

		public ushort major;

		public ushort minor;

		public ushort build;

		public ushort qfe;

		public XEXAprovalType approval;

		public XEXStaticLibrary(BinaryReader reader)
		{
			name = new string(reader.ReadChars(8)).TrimEnd(default(char));
			major = ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
			minor = ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
			build = ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
			qfe = ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
			approval = (XEXAprovalType)ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXFileBasicCompressionBlock
	{
		public uint data_size;

		public uint zero_size;

		public XEXFileBasicCompressionBlock(BinaryReader reader)
		{
			data_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			zero_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXFileNormalCompressionInfo
	{
		public uint window_size;

		public uint window_bits;

		public uint block_size;

		public byte[] block_hash = new byte[20];

		public XEXFileNormalCompressionInfo(BinaryReader reader)
		{
			window_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			block_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(block_hash, 0, block_hash.Length);
			uint num = window_size;
			uint num2 = 0u;
			while (num2 < 32)
			{
				num <<= 1;
				if (num != 2147483648u)
				{
					num2++;
					window_bits++;
					continue;
				}
				break;
			}
		}
	}

	public class XEXEncryptionHeader
	{
		public XEXEncryptionType encryption_type;

		public XEXCompressionType compression_type;

		public XEXEncryptionHeader(BinaryReader reader)
		{
			encryption_type = (XEXEncryptionType)ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
			compression_type = (XEXCompressionType)ContentHelper.SwapEndian(swap: true, reader.ReadUInt16());
		}
	}

	public class XEXFileFormat
	{
		public XEXEncryptionType encryption_type;

		public XEXCompressionType compression_type;

		public List<XEXFileBasicCompressionBlock> basic_blocks = new List<XEXFileBasicCompressionBlock>();

		public XEXFileNormalCompressionInfo normal;
	}

	public class XEXLoaderInfo
	{
		public uint header_size;

		public uint image_size;

		public byte[] rsa_signature = new byte[256];

		public uint unklength;

		public uint image_flags;

		public uint load_address;

		public byte[] section_digest = new byte[20];

		public uint import_table_count;

		public byte[] import_table_digest = new byte[20];

		public byte[] media_id = new byte[16];

		public byte[] file_key = new byte[16];

		public uint export_table;

		public byte[] header_digest = new byte[20];

		public uint game_regions;

		public uint media_flags;

		public XEXLoaderInfo(BinaryReader reader)
		{
			header_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			image_size = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(rsa_signature, 0, rsa_signature.Length);
			unklength = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			image_flags = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			load_address = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(section_digest, 0, section_digest.Length);
			import_table_count = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(import_table_digest, 0, import_table_digest.Length);
			reader.BaseStream.Read(media_id, 0, media_id.Length);
			reader.BaseStream.Read(file_key, 0, file_key.Length);
			export_table = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(header_digest, 0, header_digest.Length);
			game_regions = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			media_flags = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public class XEXSection
	{
		public uint info_value;

		public byte[] digest = new byte[20];

		public XEXSection(BinaryReader reader)
		{
			info_value = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			reader.BaseStream.Read(digest, 0, digest.Length);
		}
	}

	public class XEXHeader
	{
		public uint xex2;

		public uint module_flags;

		public uint exe_offset;

		public uint unknown0;

		public uint certificate_offset;

		public uint header_count;

		public XEXHeader(BinaryReader reader)
		{
			xex2 = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			module_flags = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			exe_offset = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			unknown0 = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			certificate_offset = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
			header_count = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		}
	}

	public enum XEXHeaderKeys : uint
	{
		XEX_HEADER_RESOURCE_INFO = 767u,
		XEX_HEADER_FILE_FORMAT_INFO = 1023u,
		XEX_HEADER_DELTA_PATCH_DESCRIPTOR = 1535u,
		XEX_HEADER_BASE_REFERENCE = 1029u,
		XEX_HEADER_BOUNDING_PATH = 33023u,
		XEX_HEADER_DEVICE_ID = 33029u,
		XEX_HEADER_ORIGINAL_BASE_ADDRESS = 65537u,
		XEX_HEADER_ENTRY_POINT = 65792u,
		XEX_HEADER_IMAGE_BASE_ADDRESS = 66049u,
		XEX_HEADER_IMPORT_LIBRARIES = 66559u,
		XEX_HEADER_CHECKSUM_TIMESTAMP = 98306u,
		XEX_HEADER_ENABLED_FOR_CALLCAP = 98562u,
		XEX_HEADER_ENABLED_FOR_FASTCAP = 98816u,
		XEX_HEADER_ORIGINAL_PE_NAME = 99327u,
		XEX_HEADER_STATIC_LIBRARIES = 131327u,
		XEX_HEADER_TLS_INFO = 131332u,
		XEX_HEADER_DEFAULT_STACK_SIZE = 131584u,
		XEX_HEADER_DEFAULT_FILESYSTEM_CACHE_SIZE = 131841u,
		XEX_HEADER_DEFAULT_HEAP_SIZE = 132097u,
		XEX_HEADER_PAGE_HEAP_SIZE_AND_FLAGS = 163842u,
		XEX_HEADER_SYSTEM_FLAGS = 196608u,
		XEX_HEADER_EXECUTION_INFO = 262150u,
		XEX_HEADER_TITLE_WORKSPACE_SIZE = 262657u,
		XEX_HEADER_GAME_RATINGS = 262928u,
		XEX_HEADER_LAN_KEY = 263172u,
		XEX_HEADER_XBOX360_LOGO = 263679u,
		XEX_HEADER_MULTIDISC_MEDIA_IDS = 263935u,
		XEX_HEADER_ALTERNATE_TITLE_IDS = 264191u,
		XEX_HEADER_ADDITIONAL_TITLE_MEMORY = 264193u,
		XEX_HEADER_EXPORTS_BY_NAME = 14746626u
	}

	[Flags]
	public enum XEXModuleFlags : ushort
	{
		XEX_MODULE_TITLE = 1,
		XEX_MODULE_EXPORTS_TO_TITLE = 2,
		XEX_MODULE_SYSTEM_DEBUGGER = 4,
		XEX_MODULE_DLL_MODULE = 8,
		XEX_MODULE_MODULE_PATCH = 0x10,
		XEX_MODULE_PATCH_FULL = 0x20,
		XEX_MODULE_PATCH_DELTA = 0x40,
		XEX_MODULE_USER_MODE = 0x80
	}

	[Flags]
	public enum XEXSystemFlags : uint
	{
		XEX_SYSTEM_NO_FORCED_REBOOT = 1u,
		XEX_SYSTEM_FOREGROUND_TASKS = 2u,
		XEX_SYSTEM_NO_ODD_MAPPING = 4u,
		XEX_SYSTEM_HANDLE_MCE_INPUT = 8u,
		XEX_SYSTEM_RESTRICTED_HUD_FEATURES = 0x10u,
		XEX_SYSTEM_HANDLE_GAMEPAD_DISCONNECT = 0x20u,
		XEX_SYSTEM_INSECURE_SOCKETS = 0x40u,
		XEX_SYSTEM_XBOX1_INTEROPERABILITY = 0x80u,
		XEX_SYSTEM_DASH_CONTEXT = 0x100u,
		XEX_SYSTEM_USES_GAME_VOICE_CHANNEL = 0x200u,
		XEX_SYSTEM_PAL50_INCOMPATIBLE = 0x400u,
		XEX_SYSTEM_INSECURE_UTILITY_DRIVE = 0x800u,
		XEX_SYSTEM_XAM_HOOKS = 0x1000u,
		XEX_SYSTEM_ACCESS_PII = 0x2000u,
		XEX_SYSTEM_CROSS_PLATFORM_SYSTEM_LINK = 0x4000u,
		XEX_SYSTEM_MULTIDISC_SWAP = 0x8000u,
		XEX_SYSTEM_MULTIDISC_INSECURE_MEDIA = 0x10000u,
		XEX_SYSTEM_AP25_MEDIA = 0x20000u,
		XEX_SYSTEM_NO_CONFIRM_EXIT = 0x40000u,
		XEX_SYSTEM_ALLOW_BACKGROUND_DOWNLOAD = 0x80000u,
		XEX_SYSTEM_CREATE_PERSISTABLE_RAMDRIVE = 0x100000u,
		XEX_SYSTEM_INHERIT_PERSISTENT_RAMDRIVE = 0x200000u,
		XEX_SYSTEM_ALLOW_HUD_VIBRATION = 0x400000u,
		XEX_SYSTEM_ACCESS_UTILITY_PARTITIONS = 0x800000u,
		XEX_SYSTEM_IPTV_INPUT_SUPPORTED = 0x1000000u,
		XEX_SYSTEM_PREFER_BIG_BUTTON_INPUT = 0x2000000u,
		XEX_SYSTEM_ALLOW_EXTENDED_SYSTEM_RESERVATION = 0x4000000u,
		XEX_SYSTEM_MULTIDISC_CROSS_TITLE = 0x8000000u,
		XEX_SYSTEM_INSTALL_INCOMPATIBLE = 0x10000000u,
		XEX_SYSTEM_ALLOW_AVATAR_GET_METADATA_BY_XUID = 0x20000000u,
		XEX_SYSTEM_ALLOW_CONTROLLER_SWAPPING = 0x40000000u,
		XEX_SYSTEM_DASH_EXTENSIBILITY_MODULE = 0x80000000u
	}

	public enum XEXAprovalType : ushort
	{
		XEX_APPROVAL_UNAPPROVED,
		XEX_APPROVAL_POSSIBLE,
		XEX_APPROVAL_APPROVED,
		XEX_APPROVAL_EXPIRED
	}

	public enum XEXEncryptionType : ushort
	{
		XEX_ENCRYPTION_NONE,
		XEX_ENCRYPTION_NORMAL
	}

	public enum XEXCompressionType : ushort
	{
		XEX_COMPRESSION_NONE,
		XEX_COMPRESSION_BASIC,
		XEX_COMPRESSION_NORMAL,
		XEX_COMPRESSION_DELTA
	}

	[Flags]
	public enum XEXImageFlags : uint
	{
		XEX_IMAGE_MANUFACTURING_UTILITY = 2u,
		XEX_IMAGE_MANUFACTURING_SUPPORT_TOOLS = 4u,
		XEX_IMAGE_XGD2_MEDIA_ONLY = 8u,
		XEX_IMAGE_CARDEA_KEY = 0x100u,
		XEX_IMAGE_XEIKA_KEY = 0x200u,
		XEX_IMAGE_USERMODE_TITLE = 0x400u,
		XEX_IMAGE_USERMODE_SYSTEM = 0x800u,
		XEX_IMAGE_ORANGE0 = 0x1000u,
		XEX_IMAGE_ORANGE1 = 0x2000u,
		XEX_IMAGE_ORANGE2 = 0x4000u,
		XEX_IMAGE_IPTV_SIGNUP_APPLICATION = 0x10000u,
		XEX_IMAGE_IPTV_TITLE_APPLICATION = 0x20000u,
		XEX_IMAGE_KEYVAULT_PRIVILEGES_REQUIRED = 0x4000000u,
		XEX_IMAGE_ONLINE_ACTIVATION_REQUIRED = 0x8000000u,
		XEX_IMAGE_PAGE_SIZE_4KB = 0x10000000u,
		XEX_IMAGE_REGION_FREE = 0x20000000u,
		XEX_IMAGE_REVOCATION_CHECK_OPTIONAL = 0x40000000u,
		XEX_IMAGE_REVOCATION_CHECK_REQUIRED = 0x80000000u
	}

	[Flags]
	public enum XEXMediaFlags : uint
	{
		XEX_MEDIA_HARDDISK = 1u,
		XEX_MEDIA_DVD_X2 = 2u,
		XEX_MEDIA_DVD_CD = 4u,
		XEX_MEDIA_DVD_5 = 8u,
		XEX_MEDIA_DVD_9 = 0x10u,
		XEX_MEDIA_SYSTEM_FLASH = 0x20u,
		XEX_MEDIA_MEMORY_UNIT = 0x80u,
		XEX_MEDIA_USB_MASS_STORAGE_DEVICE = 0x100u,
		XEX_MEDIA_NETWORK = 0x200u,
		XEX_MEDIA_DIRECT_FROM_MEMORY = 0x400u,
		XEX_MEDIA_RAM_DRIVE = 0x800u,
		XEX_MEDIA_SVOD = 0x1000u,
		XEX_MEDIA_INSECURE_PACKAGE = 0x1000000u,
		XEX_MEDIA_SAVEGAME_PACKAGE = 0x2000000u,
		XEX_MEDIA_LOCALLY_SIGNED_PACKAGE = 0x4000000u,
		XEX_MEDIA_LIVE_SIGNED_PACKAGE = 0x8000000u,
		XEX_MEDIA_XBOX_PACKAGE = 0x10000000u
	}

	public enum XEXRegion : uint
	{
		XEX_REGION_NTSCU = 255u,
		XEX_REGION_NTSCJ = 65280u,
		XEX_REGION_NTSCJ_JAPAN = 256u,
		XEX_REGION_NTSCJ_CHINA = 512u,
		XEX_REGION_PAL = 16711680u,
		XEX_REGION_PAL_AU_NZ = 65536u,
		XEX_REGION_OTHER = 4278190080u,
		XEX_REGION_ALL = uint.MaxValue
	}

	public enum XEXSectionType : ushort
	{
		XEX_SECTION_CODE = 1,
		XEX_SECTION_DATA,
		XEX_SECTION_READONLY_DATA
	}

	public const uint XEX2_SECTION_LENGTH = 65536u;

	private static readonly byte[] IV = new byte[16];

	private static readonly byte[] xe_xex2_retail_key = new byte[16]
	{
		32, 177, 133, 165, 157, 40, 253, 195, 64, 88,
		63, 187, 8, 150, 191, 145
	};

	private static readonly byte[] xe_xex2_devkit_key = new byte[16];

	public XEXHeader header;

	public XEXSystemFlags system_flags;

	public XEXExecutionInfo execution_info;

	public XEXRatings game_ratings;

	public XEXTLSInfo tls_info;

	public XEXFileFormat file_format_info = new XEXFileFormat();

	public XEXLoaderInfo loader_info;

	public byte[] session_key = new byte[16];

	public uint exe_address;

	public uint exe_entry_point;

	public uint exe_stack_size;

	public uint exe_heap_size;

	public List<XEXResourceInfo> resources = new List<XEXResourceInfo>();

	public List<XEXOptionalHeader> optional_headers = new List<XEXOptionalHeader>();

	public List<XEXSection> sections = new List<XEXSection>();

	public List<uint> import_records = new List<uint>();

	public byte[] m_memoryData;

	public int m_memorySize;

	public XEXImageData(BinaryReader reader)
	{
		LoadHeaders(reader);
		LoadImageData(reader);
	}

	private void LoadHeaders(BinaryReader reader)
	{
		header = new XEXHeader(reader);
		if (header.xex2 != 1480939570)
		{
			throw new InvalidDataException(string.Format("File not a XEX2 file - magic numbers in file: 0x{0}", header.xex2.ToString("X8")));
		}
		for (uint num = 0u; num < header.header_count; num++)
		{
			XEXOptionalHeader xEXOptionalHeader = new XEXOptionalHeader(reader);
			bool flag = true;
			switch ((uint)(xEXOptionalHeader.key & (XEXHeaderKeys)0xFFu))
			{
			case 0u:
			case 1u:
				xEXOptionalHeader.value = xEXOptionalHeader.offset;
				xEXOptionalHeader.offset = 0u;
				break;
			case 255u:
			{
				long position = reader.BaseStream.Position;
				reader.BaseStream.Seek(xEXOptionalHeader.offset, SeekOrigin.Begin);
				xEXOptionalHeader.length = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
				xEXOptionalHeader.offset += 4u;
				if (xEXOptionalHeader.length + xEXOptionalHeader.offset >= reader.BaseStream.Length)
				{
					throw new InvalidDataException(string.Format("Optional header {0} (0x{1}) crosses file boundary. Will not be read.", num, xEXOptionalHeader.key.ToString("X8")));
				}
				break;
			}
			default:
				xEXOptionalHeader.length = (uint)(xEXOptionalHeader.key & (XEXHeaderKeys)0xFFu) * 4u;
				if (xEXOptionalHeader.length + xEXOptionalHeader.offset >= reader.BaseStream.Length)
				{
					throw new InvalidDataException(string.Format("Optional header {0} (0x{1}) crosses file boundary. Will not be read.", num, xEXOptionalHeader.key.ToString("X8")));
				}
				break;
			}
			if (flag)
			{
				optional_headers.Add(xEXOptionalHeader);
			}
		}
		for (int i = 0; i < optional_headers.Count; i++)
		{
			XEXOptionalHeader xEXOptionalHeader2 = optional_headers[i];
			if (xEXOptionalHeader2.length != 0 && xEXOptionalHeader2.offset != 0)
			{
				reader.BaseStream.Seek(xEXOptionalHeader2.offset, SeekOrigin.Begin);
			}
			switch (xEXOptionalHeader2.key)
			{
			case XEXHeaderKeys.XEX_HEADER_SYSTEM_FLAGS:
				system_flags = (XEXSystemFlags)xEXOptionalHeader2.value;
				break;
			case XEXHeaderKeys.XEX_HEADER_RESOURCE_INFO:
			{
				uint num6 = (xEXOptionalHeader2.length - 4) / 16;
				resources.Clear();
				for (uint num7 = 0u; num7 < num6; num7++)
				{
					resources.Add(new XEXResourceInfo(reader));
				}
				break;
			}
			case XEXHeaderKeys.XEX_HEADER_EXECUTION_INFO:
				execution_info = new XEXExecutionInfo(reader);
				break;
			case XEXHeaderKeys.XEX_HEADER_TLS_INFO:
				tls_info = new XEXTLSInfo(reader);
				break;
			case XEXHeaderKeys.XEX_HEADER_IMAGE_BASE_ADDRESS:
				exe_address = xEXOptionalHeader2.value;
				Console.WriteLine("XEX: Found base addrses: 0x{0}", exe_address.ToString("X8"));
				break;
			case XEXHeaderKeys.XEX_HEADER_ENTRY_POINT:
				exe_entry_point = xEXOptionalHeader2.value;
				Console.WriteLine("XEX: Found entry point: 0x{0}", exe_entry_point.ToString("X8"));
				break;
			case XEXHeaderKeys.XEX_HEADER_DEFAULT_STACK_SIZE:
				exe_stack_size = xEXOptionalHeader2.value;
				break;
			case XEXHeaderKeys.XEX_HEADER_DEFAULT_HEAP_SIZE:
				exe_heap_size = xEXOptionalHeader2.value;
				break;
			case XEXHeaderKeys.XEX_HEADER_FILE_FORMAT_INFO:
			{
				XEXEncryptionHeader xEXEncryptionHeader = new XEXEncryptionHeader(reader);
				file_format_info.encryption_type = xEXEncryptionHeader.encryption_type;
				file_format_info.compression_type = xEXEncryptionHeader.compression_type;
				switch (xEXEncryptionHeader.compression_type)
				{
				case XEXCompressionType.XEX_COMPRESSION_NONE:
					Console.WriteLine("XEX: image::Binary is using no compression");
					break;
				case XEXCompressionType.XEX_COMPRESSION_DELTA:
					Console.WriteLine("XEX: image::Binary is using unsupported delta compression");
					break;
				case XEXCompressionType.XEX_COMPRESSION_BASIC:
				{
					uint num4 = (xEXOptionalHeader2.length - 8) / 8;
					file_format_info.basic_blocks.Clear();
					for (uint num5 = 0u; num5 < num4; num5++)
					{
						XEXFileBasicCompressionBlock item = new XEXFileBasicCompressionBlock(reader);
						file_format_info.basic_blocks.Add(item);
					}
					Console.WriteLine("XEX: image::Binary is using basic compression with {0} blocks", num4);
					break;
				}
				case XEXCompressionType.XEX_COMPRESSION_NORMAL:
					file_format_info.normal = new XEXFileNormalCompressionInfo(reader);
					Console.WriteLine("XEX: image::Binary is using normal compression with block size = {0}", file_format_info.normal.block_size);
					break;
				}
				if (xEXEncryptionHeader.encryption_type != XEXEncryptionType.XEX_ENCRYPTION_NONE)
				{
					Console.WriteLine("XEX: image::Binary is encrypted");
				}
				break;
			}
			case XEXHeaderKeys.XEX_HEADER_IMPORT_LIBRARIES:
			{
				XEXImportLibraryBlockHeader xEXImportLibraryBlockHeader = new XEXImportLibraryBlockHeader(reader);
				_ = reader.BaseStream.Position;
				reader.BaseStream.Seek(xEXImportLibraryBlockHeader.string_table_size, SeekOrigin.Current);
				for (uint num2 = 0u; num2 < xEXImportLibraryBlockHeader.count; num2++)
				{
					XEXImportLibraryHeader xEXImportLibraryHeader = new XEXImportLibraryHeader(reader);
					for (uint num3 = 0u; num3 < xEXImportLibraryHeader.record_count; num3++)
					{
						import_records.Add(ContentHelper.SwapEndian(swap: true, reader.ReadUInt32()));
					}
				}
				break;
			}
			}
		}
		reader.BaseStream.Seek(header.certificate_offset, SeekOrigin.Begin);
		loader_info = new XEXLoaderInfo(reader);
		Console.WriteLine("XEX: Binary size: 0x{0}", loader_info.image_size.ToString("X8"));
		reader.BaseStream.Seek(header.certificate_offset + 384, SeekOrigin.Begin);
		uint num8 = ContentHelper.SwapEndian(swap: true, reader.ReadUInt32());
		for (uint num9 = 0u; num9 < num8; num9++)
		{
			sections.Add(new XEXSection(reader));
		}
		byte[] key = xe_xex2_devkit_key;
		if (execution_info.title_id != 0)
		{
			Console.WriteLine("XEX: Found TitleID 0x{0}", execution_info.title_id.ToString("X8"));
			key = xe_xex2_retail_key;
		}
		using Aes aes = new AesManaged();
		aes.Mode = CipherMode.CBC;
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.Key = key;
		aes.Padding = PaddingMode.None;
		aes.IV = IV;
		using ICryptoTransform cryptoTransform = aes.CreateDecryptor();
		session_key = new byte[16];
		cryptoTransform.TransformBlock(loader_info.file_key, 0, 16, session_key, 0);
	}

	private void LoadImageDataUncompressed(BinaryReader data)
	{
		int num = (int)(data.BaseStream.Length - header.exe_offset);
		if ((long)num >= 134217728L)
		{
			throw new InvalidDataException(string.Format("Computed image size is to big (0x{0}), the exe offset = 0x{1}", num.ToString("X8"), header.exe_offset.ToString("X8")));
		}
		data.BaseStream.Seek(header.exe_offset, SeekOrigin.Begin);
		byte[] array = data.ReadBytes(num);
		if (file_format_info.encryption_type == XEXEncryptionType.XEX_ENCRYPTION_NONE)
		{
			m_memoryData = array;
			m_memorySize = num;
			return;
		}
		byte[] array2 = new byte[num];
		DecryptBuffer(session_key, array, num, array2, num);
		m_memoryData = array2;
		m_memorySize = num;
	}

	private void LoadImageDataNormal(BinaryReader data)
	{
		int num = (int)(data.BaseStream.Length - header.exe_offset);
		data.BaseStream.Seek(header.exe_offset, SeekOrigin.Begin);
		byte[] array = data.ReadBytes(num);
		int num2 = num;
		byte[] array2 = array;
		if (file_format_info.encryption_type == XEXEncryptionType.XEX_ENCRYPTION_NORMAL)
		{
			array2 = new byte[num];
			DecryptBuffer(session_key, array, num, array2, num2);
		}
		byte[] array3 = new byte[num2];
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = (int)file_format_info.normal.block_size;
		while (num7 != 0)
		{
			int num8 = num5 + num7;
			int num9 = (int)ContentHelper.SwapEndian(swap: true, BitConverter.ToUInt32(array2, num5));
			num5 += 4;
			num5 += 20;
			while (true)
			{
				int num10 = (array2[num5] << 8) | array2[num5 + 1];
				num5 += 2;
				if (num10 == 0)
				{
					break;
				}
				Array.Copy(array2, num5, array3, num6, num10);
				num5 += num10;
				num6 += num10;
				num4 += 32768;
			}
			num5 = num8;
			num7 = num9;
		}
		num3 = num6;
		Console.WriteLine("Uncompressed image size: {0}", num4);
		Console.WriteLine("Compressed image size: {0}", num3);
		byte[] array4 = new byte[num4];
		LzxDecoder lzxDecoder = new LzxDecoder((int)file_format_info.normal.window_bits);
		using (MemoryStream inData = new MemoryStream(array3))
		{
			using MemoryStream outData = new MemoryStream(array4);
			int num11;
			if ((num11 = lzxDecoder.Decompress(inData, num3, outData, num4)) != 0)
			{
				throw new InvalidDataException($"Unable to decompress image data: {num11}");
			}
		}
		Console.WriteLine("Image data decompressed");
		m_memoryData = array4;
		m_memorySize = num4;
	}

	private void LoadImageDataBasic(BinaryReader data)
	{
		int num = 0;
		int count = file_format_info.basic_blocks.Count;
		for (int i = 0; i < count; i++)
		{
			XEXFileBasicCompressionBlock xEXFileBasicCompressionBlock = file_format_info.basic_blocks[i];
			num += (int)(xEXFileBasicCompressionBlock.data_size + xEXFileBasicCompressionBlock.zero_size);
		}
		int count2 = (int)(data.BaseStream.Length - header.exe_offset);
		data.BaseStream.Seek(header.exe_offset, SeekOrigin.Begin);
		byte[] array = data.ReadBytes(count2);
		int num2 = 0;
		if ((long)num >= 134217728L)
		{
			throw new InvalidDataException(string.Format("Computed image size is to big (0x{0}), the exe offset = 0x{1}", num.ToString("X8"), header.exe_offset.ToString("X8")));
		}
		byte[] array2 = new byte[num];
		byte[] array3 = array2;
		int num3 = 0;
		using (Aes aes = new AesManaged())
		{
			aes.Mode = CipherMode.CBC;
			aes.BlockSize = 128;
			aes.KeySize = 128;
			aes.Key = session_key;
			aes.Padding = PaddingMode.None;
			aes.IV = IV;
			using ICryptoTransform cryptoTransform = aes.CreateDecryptor();
			for (int j = 0; j < count; j++)
			{
				XEXFileBasicCompressionBlock xEXFileBasicCompressionBlock2 = file_format_info.basic_blocks[j];
				uint data_size = xEXFileBasicCompressionBlock2.data_size;
				uint zero_size = xEXFileBasicCompressionBlock2.zero_size;
				switch (file_format_info.encryption_type)
				{
				case XEXEncryptionType.XEX_ENCRYPTION_NONE:
					Array.Copy(array, 0L, array3, 0L, data_size);
					break;
				case XEXEncryptionType.XEX_ENCRYPTION_NORMAL:
				{
					int num4 = num2;
					int num5 = num3;
					int num6 = 0;
					while (num6 < data_size)
					{
						cryptoTransform.TransformBlock(array, num4, 16, array3, num5);
						num6 += 16;
						num4 += 16;
						num5 += 16;
					}
					break;
				}
				}
				num2 += (int)data_size;
				num3 += (int)(data_size + zero_size);
			}
		}
		int num7 = num2;
		if (num7 > data.BaseStream.Length)
		{
			throw new InvalidDataException($"XEX: Too much source data was consumed by block decompression ({num7} > {data.BaseStream.Length})");
		}
		if (num7 < data.BaseStream.Length)
		{
			Console.WriteLine("XEX: {0} bytes of data was not consumed in block decompression (out of {1})", data.BaseStream.Length - num7, data.BaseStream.Length);
		}
		int num8 = num3;
		if (num8 > num)
		{
			throw new InvalidDataException($"XEX: Too much data was outputed in block decompression ({num8} > {num})");
		}
		if (num8 < num)
		{
			Console.WriteLine("XEX: {0} bytes of data was not outputed in block decompression (out of {1})", num - num8, num);
		}
		m_memoryData = array2;
		m_memorySize = num;
	}

	private void LoadImageData(BinaryReader data)
	{
		XEXCompressionType compression_type = file_format_info.compression_type;
		switch (compression_type)
		{
		case XEXCompressionType.XEX_COMPRESSION_NONE:
			Console.WriteLine("XEX: image::Binary is not compressed");
			LoadImageDataUncompressed(data);
			break;
		case XEXCompressionType.XEX_COMPRESSION_BASIC:
			Console.WriteLine("XEX: image::Binary is using basic compression (zero blocks)");
			LoadImageDataBasic(data);
			break;
		case XEXCompressionType.XEX_COMPRESSION_NORMAL:
			Console.WriteLine("XEX: image::Binary is using normal compression");
			LoadImageDataNormal(data);
			break;
		default:
			throw new NotSupportedException($"Image is using unsupported compression mode {compression_type} and cannot be loaded");
		}
	}

	private void DecryptBuffer(byte[] key, byte[] inputData, int inputSize, byte[] outputData, int outputSize)
	{
		if (file_format_info.encryption_type == XEXEncryptionType.XEX_ENCRYPTION_NONE)
		{
			if (inputSize != outputSize)
			{
				throw new InvalidDataException("inputSize != outputSize");
			}
			Array.Copy(inputData, 0, outputData, 0, inputSize);
			return;
		}
		using Aes aes = new AesManaged();
		aes.Mode = CipherMode.CBC;
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.Key = key;
		aes.Padding = PaddingMode.None;
		aes.IV = IV;
		using ICryptoTransform cryptoTransform = aes.CreateDecryptor();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while (num3 < inputSize)
		{
			cryptoTransform.TransformBlock(inputData, num, 16, outputData, num2);
			num3 += 16;
			num += 16;
			num2 += 16;
		}
	}
}

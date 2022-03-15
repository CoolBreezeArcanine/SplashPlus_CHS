using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MAI2.Memory;
using MAI2.Util;
using MAI2System;
using Manager.Party.Party;
using UnityEngine;

namespace PartyLink
{
	public class Packet
	{
		private enum PacketPos
		{
			Size = 0,
			RomVersion = 4,
			DataVersion = 8,
			Command = 12,
			Max = 13
		}

		private static readonly int c_bitCount = 128;

		private static readonly int c_byteCount = c_bitCount / 8;

		private static readonly string c_aesIV = "wQQ6CY9kZz8sZkDY";

		private static readonly string c_aesKey = "BeTLnSPbfJBNLQKx";

		private static readonly byte[] c_salt = new byte[16]
		{
			125, 119, 162, 0, 236, 31, 246, 225, 133, 151,
			177, 19, 66, 220, 221, 66
		};

		private static readonly int c_sizeSize = 4;

		private static readonly int c_packetBufferSize = 2048;

		private static readonly int c_encodeBufferSize = 2048;

		private static readonly int c_serializeBufferSize = 2048;

		private static readonly int c_deserializeBufferSize = 2048;

		private static AesManaged _aes = null;

		private IpAddress _address;

		private PacketType _plane;

		private PacketType _encrypt;

		private Decoder _decoder;

		private byte[] _encodeBuffer;

		private byte[] _serializeBuffer;

		private char[] _deserializeBuffer;

		private ClientPlayInfo _clientPlayInfoBuffer;

		private PartyPlayInfo _partyPlayInfoBuffer;

		private PartyMemberInfo _partyMemberInfoBuffer;

		private Setting.SettingHostAddress _settingHostAddressBuffer;

		private AdvocateDelivery _advocateDeliveryBuffer;

		private FixedAllocator _fixedAllocator;

		private Chunk _tempBufferChunk;

		public static void createAes()
		{
			destroyAes();
			_aes = new AesManaged();
			_aes.KeySize = c_bitCount;
			_aes.IV = new Rfc2898DeriveBytes(c_aesIV, c_salt).GetBytes(c_byteCount);
			_aes.Key = new Rfc2898DeriveBytes(c_aesKey, c_salt).GetBytes(c_byteCount);
		}

		public static void destroyAes()
		{
			if (_aes != null)
			{
				_aes.Clear();
			}
			_aes = null;
		}

		public Packet(IpAddress address)
		{
			init(address);
		}

		public Packet()
		{
			init(IpAddress.Zero);
		}

		private void init(IpAddress address)
		{
			_address = address;
			_plane = new PacketType(c_packetBufferSize);
			_encrypt = new PacketType(c_packetBufferSize);
			_decoder = Encoding.UTF8.GetDecoder();
			_encodeBuffer = new byte[c_encodeBufferSize];
			_serializeBuffer = new byte[c_serializeBufferSize];
			_deserializeBuffer = new char[c_deserializeBufferSize];
			_clientPlayInfoBuffer = new ClientPlayInfo();
			_partyPlayInfoBuffer = new PartyPlayInfo();
			_partyMemberInfoBuffer = new PartyMemberInfo();
			_settingHostAddressBuffer = new Setting.SettingHostAddress();
			_advocateDeliveryBuffer = new AdvocateDelivery();
			_fixedAllocator = new FixedAllocator(c_serializeBufferSize, 1);
			_tempBufferChunk = _fixedAllocator.allocate();
		}

		public void reset(IpAddress address)
		{
			_address = address;
			reset();
		}

		public void reset()
		{
			_plane.Clear();
			_encrypt.Clear();
			_fixedAllocator.free(ref _tempBufferChunk);
			_tempBufferChunk = _fixedAllocator.allocate();
		}

		public void encode(ICommandParam param)
		{
			reset();
			int num = 0;
			uint value = 0u;
			write_uint(_encodeBuffer, num, value);
			num += 4;
			uint appRomVersion = getAppRomVersion();
			write_uint(_encodeBuffer, num, appRomVersion);
			num += 4;
			uint appDataVersion = getAppDataVersion();
			write_uint(_encodeBuffer, num, appDataVersion);
			num += 4;
			int command = (int)param.getCommand();
			write_int(_encodeBuffer, num, command);
			num += 4;
			num += serialize(_encodeBuffer, num, param);
			_plane.Set(_encodeBuffer, 0, num);
			int count = _plane.Count;
			int num2 = c_sizeSize + aes_align(_plane.Count - c_sizeSize);
			_plane.AddRange(num2 - count);
			write_uint(_plane, 0, (uint)_plane.Count);
			encrypt();
		}

		public bool decode(BufferType buffer, IpAddress address)
		{
			if (buffer.Count <= c_sizeSize)
			{
				return false;
			}
			uint num = read_uint(buffer, 0);
			if (buffer.Count < num)
			{
				return false;
			}
			reset(address);
			_encrypt.Clear();
			_encrypt.AddByBufferType(buffer, 0, (int)num);
			decrypt();
			return true;
		}

		public T getParam<T>() where T : ICommandParam
		{
			return getParam<T>(isCheckVersion: true);
		}

		public T getParam<T>(bool isCheckVersion) where T : ICommandParam
		{
			int num = 0;
			read_uint(_plane, num);
			num += 4;
			uint num2 = read_uint(_plane, num);
			num += 4;
			if (isCheckVersion)
			{
				getAppRomVersion();
			}
			uint num3 = read_uint(_plane, num);
			num += 4;
			if (isCheckVersion)
			{
				getAppDataVersion();
			}
			Command command = (Command)read_int(_plane, num);
			num += 4;
			T result = (T)deserialize(_plane, num, command);
			result?.getCommand();
			return result;
		}

		public IpAddress getAddress()
		{
			return _address;
		}

		private int getValue(PacketType buf, int size, PacketPos pos)
		{
			_ = pos + size;
			_ = buf.Count;
			getValue_sub(out int outValue, buf, (int)pos);
			return outValue;
		}

		public uint getPlaneSize()
		{
			return (uint)getValue(_plane, 4, PacketPos.Size);
		}

		public uint getEncryptSize()
		{
			return (uint)getValue(_encrypt, 4, PacketPos.Size);
		}

		public uint getRomVersion()
		{
			return (uint)getValue(_plane, 4, PacketPos.RomVersion);
		}

		public uint getDataVersion()
		{
			return (uint)getValue(_plane, 4, PacketPos.DataVersion);
		}

		public Command getCommand()
		{
			return (Command)getValue(_plane, 4, PacketPos.Command);
		}

		public Command getCommand(PacketType buf)
		{
			return (Command)getValue(buf, 4, PacketPos.Command);
		}

		public bool isSameVersion()
		{
			if (getRomVersion() != getAppRomVersion())
			{
				return false;
			}
			if (getDataVersion() != getAppDataVersion())
			{
				return false;
			}
			return true;
		}

		public PacketType getEncrypt()
		{
			return _encrypt;
		}

		public PacketType getPlane()
		{
			return _plane;
		}

		public static uint getAppRomVersion()
		{
			return Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionCode;
		}

		public static uint getAppDataVersion()
		{
			return Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo.versionCode;
		}

		public static uint getAppCardMakerVersion()
		{
			return Singleton<SystemConfig>.Instance.config.cardMakerVersionInfo.versionNo.versionCode;
		}

		private static void write_int(IList<byte> buf, int pos, int value)
		{
			write_uint(buf, pos, (uint)value);
		}

		private static void write_uint(IList<byte> buf, int pos, uint value)
		{
			write_sub(buf, buf.Count, pos, value);
		}

		private static void write_uint(PacketType buf, int pos, uint value)
		{
			write_sub(buf.GetBuffer(), buf.Count, pos, value);
		}

		private static void write_sub(IList<byte> buf, int bufSize, int pos, uint value)
		{
			if (buf != null && bufSize >= pos + 4)
			{
				buf[pos] = (byte)(value & 0xFFu);
				buf[pos + 1] = (byte)((value >> 8) & 0xFFu);
				buf[pos + 2] = (byte)((value >> 16) & 0xFFu);
				buf[pos + 3] = (byte)((value >> 24) & 0xFFu);
			}
		}

		private static int read_int(IList<byte> buf, int pos)
		{
			return (int)read_uint(buf, pos);
		}

		private static int read_int(PacketType buf, int pos)
		{
			return (int)read_uint(buf, pos);
		}

		private static uint read_uint(IList<byte> buf, int pos)
		{
			return read_sub(buf, buf.Count, pos);
		}

		private static uint read_uint(PacketType buf, int pos)
		{
			return read_sub(buf.GetBuffer(), buf.Count, pos);
		}

		private static uint read_sub(IList<byte> buf, int bufSize, int pos)
		{
			if (buf == null || bufSize < pos + 4)
			{
				return 0u;
			}
			return 0u | (buf[pos] & 0xFFu) | (uint)((buf[pos + 1] & 0xFF) << 8) | (uint)((buf[pos + 2] & 0xFF) << 16) | (uint)((buf[pos + 3] & 0xFF) << 24);
		}

		private void getValue_sub(out int outValue, PacketType buf, int pos)
		{
			outValue = read_int(buf, pos);
		}

		private void getValue_sub(out uint outValue, PacketType buf, int pos)
		{
			outValue = read_uint(buf, pos);
		}

		private int aes_align(int size)
		{
			return (int)((size + 15) & 0xFFFFFFF0u);
		}

		private void encrypt()
		{
			_encrypt.ClearAndResize(c_sizeSize);
			write_uint(_encrypt, 0, 0u);
			_ = _plane.Count - c_sizeSize;
			aes_align(_plane.Count - c_sizeSize);
			using ICryptoTransform cryptoTransform = _aes.CreateEncryptor();
			int num = cryptoTransform.InputBlockSize;
			int num2 = _plane.Count - c_sizeSize;
			num2 -= num;
			if (num2 > 0)
			{
				int num3 = cryptoTransform.TransformBlock(_plane.GetBuffer(), c_sizeSize, num2, _encrypt.GetBuffer(), _encrypt.Count);
				_encrypt.ChangeCount(_encrypt.Count + num3);
			}
			else
			{
				num = _plane.Count - c_sizeSize;
				num2 = 0;
			}
			byte[] srcBuf = cryptoTransform.TransformFinalBlock(_plane.GetBuffer(), c_sizeSize + num2, num);
			_encrypt.AddRange(srcBuf);
			write_uint(_encrypt, 0, (uint)_encrypt.Count);
		}

		private void decrypt()
		{
			_plane.ClearAndResize(c_sizeSize);
			write_uint(_plane, 0, 0u);
			using ICryptoTransform cryptoTransform = _aes.CreateDecryptor();
			int num = cryptoTransform.InputBlockSize;
			int num2 = _encrypt.Count - c_sizeSize;
			num2 -= num;
			if (num2 > 0)
			{
				int num3 = cryptoTransform.TransformBlock(_encrypt.GetBuffer(), c_sizeSize, num2, _plane.GetBuffer(), _plane.Count);
				_plane.ChangeCount(_plane.Count + num3);
			}
			else
			{
				num = _encrypt.Count - c_sizeSize;
				num2 = 0;
			}
			byte[] srcBuf = cryptoTransform.TransformFinalBlock(_encrypt.GetBuffer(), c_sizeSize + num2, num);
			_plane.AddRange(srcBuf);
			write_uint(_plane, 0, (uint)_plane.Count);
		}

		private int serializeJson(byte[] buffer, int pos, ICommandParam param)
		{
			string text = null;
			try
			{
				text = JsonUtility.ToJson(param, prettyPrint: false);
			}
			catch
			{
			}
			if (text == null || text.Length <= 0)
			{
				return 0;
			}
			int bytes = Encoding.UTF8.GetBytes(text, 0, text.Length, _serializeBuffer, 0);
			if (pos < 0 || buffer.Length < pos + bytes)
			{
				return 0;
			}
			Array.Copy(_serializeBuffer, 0, buffer, pos, bytes);
			return bytes;
		}

		private ICommandParam deserializeJson(PacketType packet, int pos, Command command)
		{
			if (packet == null)
			{
				return null;
			}
			Func<string, ICommandParam> deserializerFunc = command.getDeserializerFunc();
			if (deserializerFunc == null)
			{
				return null;
			}
			int byteCount = packet.Count - pos;
			int chars = _decoder.GetChars(packet.GetBuffer(), pos, byteCount, _deserializeBuffer, 0, flush: true);
			try
			{
				return deserializerFunc(new string(_deserializeBuffer, 0, chars));
			}
			catch
			{
			}
			return null;
		}

		private int serialize(byte[] buffer, int pos, ICommandParam param)
		{
			if (buffer == null || param == null)
			{
				return 0;
			}
			int num = 0;
			switch (param.getCommand())
			{
			default:
				num = serializeJson(buffer, pos, param);
				break;
			case Command.ClientPlayInfo:
				num = ((ClientPlayInfo)param).Serialize(0, _tempBufferChunk);
				Array.Copy(_tempBufferChunk.Buffer, _tempBufferChunk.Offset, buffer, pos, num);
				break;
			case Command.PartyPlayInfo:
				num = ((PartyPlayInfo)param).Serialize(0, _tempBufferChunk);
				Array.Copy(_tempBufferChunk.Buffer, _tempBufferChunk.Offset, buffer, pos, num);
				break;
			case Command.PartyMemberInfo:
				num = ((PartyMemberInfo)param).Serialize(0, _tempBufferChunk);
				Array.Copy(_tempBufferChunk.Buffer, _tempBufferChunk.Offset, buffer, pos, num);
				break;
			case Command.SettingHostAddress:
				num = ((Setting.SettingHostAddress)param).serialize(0, _tempBufferChunk);
				Array.Copy(_tempBufferChunk.Buffer, _tempBufferChunk.Offset, buffer, pos, num);
				break;
			case Command.AdvocateDelivery:
				num = ((AdvocateDelivery)param).serialize(0, _tempBufferChunk);
				Array.Copy(_tempBufferChunk.Buffer, _tempBufferChunk.Offset, buffer, pos, num);
				break;
			}
			return num;
		}

		private ICommandParam deserialize(PacketType packet, int pos, Command command)
		{
			int pos2 = 0;
			ICommandParam commandParam = null;
			switch (command)
			{
			default:
				return deserializeJson(packet, pos, command);
			case Command.ClientPlayInfo:
				Array.Copy(packet.GetBuffer(), pos, _tempBufferChunk.Buffer, _tempBufferChunk.Offset, packet.Count - pos);
				return _clientPlayInfoBuffer.Deserialize(ref pos2, _tempBufferChunk);
			case Command.PartyPlayInfo:
				Array.Copy(packet.GetBuffer(), pos, _tempBufferChunk.Buffer, _tempBufferChunk.Offset, packet.Count - pos);
				return _partyPlayInfoBuffer.Deserialize(ref pos2, _tempBufferChunk);
			case Command.PartyMemberInfo:
				Array.Copy(packet.GetBuffer(), pos, _tempBufferChunk.Buffer, _tempBufferChunk.Offset, packet.Count - pos);
				return _partyMemberInfoBuffer.Deserialize(ref pos2, _tempBufferChunk);
			case Command.SettingHostAddress:
				Array.Copy(packet.GetBuffer(), pos, _tempBufferChunk.Buffer, _tempBufferChunk.Offset, packet.Count - pos);
				return _settingHostAddressBuffer.deserialize(ref pos2, _tempBufferChunk);
			case Command.AdvocateDelivery:
				Array.Copy(packet.GetBuffer(), pos, _tempBufferChunk.Buffer, _tempBufferChunk.Offset, packet.Count - pos);
				return _advocateDeliveryBuffer.deserialize(ref pos2, _tempBufferChunk);
			}
		}
	}
}

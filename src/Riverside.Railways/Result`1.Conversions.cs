using System;

namespace Riverside.Railways;

partial struct Result<T>
{
	public TypeCode GetTypeCode()
	=> Type.GetTypeCode(typeof(T));

	public bool ToBoolean(IFormatProvider provider)
		=> Status;

	public byte ToByte(IFormatProvider provider)
	{
		if (typeof(T) == typeof(bool))
		{
			return Status ? (byte)1 : (byte)0;
		}
		else if (typeof(T) == typeof(byte))
		{
			return (byte)(object)this;
		}
		else
		{
			return (byte)(Status ? 1 : 0);
		}
	}

	public char ToChar(IFormatProvider provider)
	=> throw new NotSupportedException();

	public DateTime ToDateTime(IFormatProvider provider)
		=> throw new NotSupportedException();

	public decimal ToDecimal(IFormatProvider provider)
		=> (decimal)ToByte(provider);

	public double ToDouble(IFormatProvider provider) => throw new NotImplementedException();

	public short ToInt16(IFormatProvider provider) => throw new NotImplementedException();

	public int ToInt32(IFormatProvider provider) => throw new NotImplementedException();

	public long ToInt64(IFormatProvider provider) => throw new NotImplementedException();

	public sbyte ToSByte(IFormatProvider provider) => throw new NotImplementedException();

	public float ToSingle(IFormatProvider provider) => throw new NotImplementedException();

	public string ToString(IFormatProvider provider) => throw new NotImplementedException();

	public object ToType(Type conversionType, IFormatProvider provider) => throw new NotImplementedException();

	public ushort ToUInt16(IFormatProvider provider) => throw new NotImplementedException();

	public uint ToUInt32(IFormatProvider provider) => throw new NotImplementedException();

	public ulong ToUInt64(IFormatProvider provider) => throw new NotImplementedException();
}

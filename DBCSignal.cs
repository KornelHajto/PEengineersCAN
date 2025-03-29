using System;

namespace PEengineersCAN;

/// <summary>
/// Represents a signal definition from a DBC (CAN database) file.
/// </summary>
public class DBCSignal
{
    /// <summary>
    /// Gets or sets the name of the signal.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the starting bit position of the signal in the CAN message.
    /// </summary>
    public int StartBit { get; set; } = 0;

    /// <summary>
    /// Gets or sets the length of the signal in bits.
    /// </summary>
    public int Length { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether the signal uses little endian byte order.
    /// </summary>
    /// <remarks>If false, the signal uses big endian (Motorola) byte order.</remarks>
    public bool IsLittleEndian { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the signal value is signed.
    /// </summary>
    public bool IsSigned { get; set; } = false;

    /// <summary>
    /// Gets or sets the scaling factor to convert from raw to physical value.
    /// </summary>
    public double Factor { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the offset to convert from raw to physical value.
    /// </summary>
    public double Offset { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the minimum valid value for the signal.
    /// </summary>
    public double Min { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the maximum valid value for the signal.
    /// </summary>
    public double Max { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the unit of measurement for the signal.
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// Gets or sets the node that receives this signal.
    /// </summary>
    public string Receiver { get; set; }

    /// <summary>
    /// Extracts and calculates the physical value of this signal from raw CAN message data.
    /// </summary>
    /// <param name="data">The byte array containing the CAN message data</param>
    /// <returns>The physical value of the signal after applying scaling factor and offset</returns>
    /// <remarks>
    /// The method handles both little endian (Intel) and big endian (Motorola) byte orders,
    /// as well as signed and unsigned signal values.
    /// </remarks>
    public double GetValue(byte[] data)
    {
        if (data == null || data.Length == 0 || StartBit < 0 || Length <= 0)
            return 0.0;

        ulong rawValue = 0;
        int totalBits = StartBit + Length;
        int totalBytes = (totalBits + 7) / 8;

        if (totalBytes > data.Length)
            return 0.0;

        if (IsLittleEndian)
        {
            int currentByte = StartBit / 8;
            int bitsInFirstByte = Math.Min(8 - (StartBit % 8), Length);
            int remainingBits = Length - bitsInFirstByte;

            byte mask = (byte)((1 << bitsInFirstByte) - 1);
            rawValue = (ulong)((data[currentByte] >> (StartBit % 8)) & mask);

            for (int i = 0; i < remainingBits / 8; i++)
            {
                currentByte++;
                rawValue |= (ulong)data[currentByte] << (bitsInFirstByte + i * 8);
            }

            if (remainingBits % 8 != 0)
            {
                currentByte++;
                int bits = remainingBits % 8;
                mask = (byte)((1 << bits) - 1);
                rawValue |= (ulong)(data[currentByte] & mask) << (bitsInFirstByte + (remainingBits / 8) * 8);
            }
        }
        else
        {
            int currentByte = StartBit / 8;
            int bitsInFirstByte = Math.Min((StartBit % 8) + 1, Length);
            int remainingBits = Length - bitsInFirstByte;

            byte mask = (byte)((1 << bitsInFirstByte) - 1);
            rawValue = (ulong)((data[currentByte] >> (8 - (StartBit % 8) - bitsInFirstByte)) & mask);

            for (int i = 0; i < remainingBits / 8; i++)
            {
                currentByte--;
                rawValue = (rawValue << 8) | data[currentByte];
            }

            if (remainingBits % 8 != 0)
            {
                currentByte--;
                int bits = remainingBits % 8;
                mask = (byte)((1 << bits) - 1);
                rawValue = (rawValue << bits) | (ulong)((data[currentByte] >> (8 - bits)) & mask);
            }
        }

        double value;
        if (IsSigned && (rawValue & (1UL << (Length - 1))) != 0)
        {
            ulong mask = ~((1UL << Length) - 1);
            long signedValue = (long)(rawValue | mask);
            value = signedValue * Factor + Offset;
        }
        else
        {
            value = rawValue * Factor + Offset;
        }

        return value;
    }
}
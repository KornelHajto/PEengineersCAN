using System.Collections.Generic;

namespace PEengineersCAN;

/// <summary>
/// Represents a CAN message definition from a DBC (CAN database) file.
/// </summary>
public class DBCMessage
{
    /// <summary>
    /// Gets or sets the ID of the CAN message.
    /// </summary>
    public uint Id { get; set; } = 0;

    /// <summary>
    /// Gets or sets the name of the CAN message.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Data Length Code (DLC) specifying the number of bytes in the message.
    /// </summary>
    public byte Dlc { get; set; } = 0;

    /// <summary>
    /// Gets or sets the node that transmits this message.
    /// </summary>
    public string Transmitter { get; set; }

    /// <summary>
    /// Gets or sets the list of signals contained within this message.
    /// </summary>
    public List<DBCSignal> Signals { get; set; } = new List<DBCSignal>();


    /// <summary>
    /// Decodes the raw CAN data into a dictionary of signal names and their physical values.
    /// </summary>
    /// <param name="data">The byte array containing the CAN message data</param>
    /// <returns>A dictionary mapping signal names to their decoded physical values</returns>
    /// <remarks>
    /// If a signal's decoding fails, its value will be set to 0.0 in the resulting dictionary.
    /// </remarks>
    public Dictionary<string, double> Decode(byte[] data)
    {
        var decoded = new Dictionary<string, double>();
        foreach (var signal in Signals)
        {
            try
            {
                double value = signal.GetValue(data);
                decoded[signal.Name] = value;
            }
            catch
            {
                decoded[signal.Name] = 0.0;
            }
        }

        return decoded;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PEengineersCAN;

/// <summary>
/// Represents a DBC (CAN database) file containing message and signal definitions.
/// </summary>
public class DBCDatabase
{
    /// <summary>
    /// Dictionary mapping CAN message IDs to their corresponding DBCMessage objects.
    /// </summary>
    private Dictionary<uint, DBCMessage> messages = new Dictionary<uint, DBCMessage>();

    /// <summary>
    /// Reference to the currently active message during parsing.
    /// </summary>
    private DBCMessage currentMessage = null;

    /// <summary>
    /// Parses a message definition line from a DBC file.
    /// </summary>
    /// <param name="line">The line containing message definition starting with "BO_"</param>
    /// <remarks>
    /// Creates a new DBCMessage object and adds it to the messages dictionary.
    /// Sets the currentMessage reference to the newly created message.
    /// </remarks>
    private void ParseMessage(string line)
    {
        var parts = line.Trim().Split(' ');
        if (parts.Count() < 4 || parts[0] != "BO_")
            return;

        try
        {
            var msg = new DBCMessage();
            msg.Id = Utils.SafeParse<uint>(parts[1]);

            int colonPos = parts[2].IndexOf(':');
            if (colonPos == -1)
                return;

            msg.Name = parts[2].Substring(0, colonPos);
            msg.Dlc = Utils.SafeParse<byte>(parts[3]);

            if (parts.Count() > 4)
            {
                msg.Transmitter = parts[4];
            }

            messages[msg.Id] = msg;
            currentMessage = messages[msg.Id];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing message ({line}): {ex.Message}");
            currentMessage = null;
        }
    }

    /// <summary>
    /// Parses a signal definition line from a DBC file.
    /// </summary>
    /// <param name="line">The line containing signal definition with "SG_"</param>
    /// <remarks>
    /// Creates a new DBCSignal object and adds it to the current message's signals list.
    /// Requires that currentMessage is not null.
    /// </remarks>
    private void ParseSignal(string line)
    {
        if (currentMessage == null)
            return;

        var parts = line.Trim().Split(' ');

        int sgPos = parts.ToList().IndexOf("SG_");
        if (sgPos == -1 || sgPos + 4 >= parts.Length)
            return;

        try
        {
            var sig = new DBCSignal();
            sig.Name = parts[sgPos + 1];

            string bitInfo = parts[sgPos + 3];
            int pipePos = bitInfo.IndexOf('|');
            int atPos = bitInfo.IndexOf('@');

            if (pipePos == -1 || atPos == -1)
                return;

            sig.StartBit = Utils.SafeParse<int>(bitInfo.Substring(0, pipePos));
            sig.Length = Utils.SafeParse<int>(bitInfo.Substring(pipePos + 1, atPos - pipePos - 1));

            sig.IsLittleEndian = bitInfo[atPos + 1] == '1';
            sig.IsSigned = bitInfo.Contains('-');

            if (parts.Length > sgPos + 4 && parts[sgPos + 4][0] == '(' &&
                parts[sgPos + 4][parts[sgPos + 4].Length - 1] == ')')
            {
                string facOff = parts[sgPos + 4].Substring(1, parts[sgPos + 4].Length - 2);
                var foParts = facOff.Split(',');
                if (foParts.Length == 2)
                {
                    sig.Factor = Utils.SafeParse<double>(foParts[0]);
                    sig.Offset = Utils.SafeParse<double>(foParts[1]);
                }
            }

            currentMessage.Signals.Add(sig);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing signal ({line}): {ex.Message}");
        }
    }

    /// <summary>
    /// Loads a DBC file and parses its contents.
    /// </summary>
    /// <param name="filename">Path to the DBC file to load</param>
    /// <remarks>
    /// Reads the file line by line, identifying and parsing message and signal definitions.
    /// </remarks>
    public void Load(string filename)
    {
        using (StreamReader file = new StreamReader(filename))
        {
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("BO_"))
                {
                    ParseMessage(line);
                }
                else if (line.Contains("SG_"))
                {
                    ParseSignal(line);
                }
            }

            currentMessage = null;
        }
    }

    public Dictionary<string, double> DecodeMessage(uint canId, byte[] data)
    {
        if (!messages.TryGetValue(canId, out DBCMessage message))
        {
            throw new InvalidOperationException($"Message ID {canId} (0x{canId:X}) not found in DBC");
        }

        return message.Decode(data);
    }
}
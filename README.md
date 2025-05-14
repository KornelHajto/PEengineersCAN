![Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KornelHajto/b0e41f803bb07bbbfaeb69d3de741def/raw/code-coverage.json)

# PEengineersCAN Documentation / User guide

A C# library for parsing and interpreting DBC (CAN database) files, with utilities for CAN message decoding.

## Features

- Parse standard DBC file format
- Decode CAN messages using signal definitions
- Support for both Intel (little-endian) and Motorola (big-endian) byte orders
- Handle signed and unsigned signals
- Apply scaling factors and offsets to raw values

## Usage Examples

### Loading a DBC File

```csharp
// Create a new DBC database and load definitions
var dbcDatabase = new DBCDatabase();
dbcDatabase.Load("path/to/your/file.dbc");
```

### Decoding CAN Messages - Basic Example
```csharp
// Decode a CAN message with ID 0x100 and 8-byte data
byte[] messageData = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
uint canId = 0x100;

// Get a dictionary of signal names and their physical values
Dictionary<string, double> decodedSignals = dbcDatabase.DecodeMessage(canId, messageData);

// Access decoded signal values
foreach (var signal in decodedSignals)
{
    Console.WriteLine($"{signal.Key}: {signal.Value}");
}
```

### Parsing CAN Messages from Text Format
```csharp
// Parse a CAN message in candump format: "(timestamp) interface id#data"
string inputLine = "(1741954078.324423) can0 00000121#5A0B017805000000";

// Remove the timestamp part
int closingParenIndex = inputLine.IndexOf(')');
string trimmedInput = inputLine.Substring(closingParenIndex + 1).Trim();

// Split to extract interface and message token
string[] tokens = trimmedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
string[] canParts = tokens[1].Split('#');

// Parse the CAN id as hex
uint canId = Utils.SafeParse<uint>(canParts[0], hex: true);

// Parse hex string to byte array
byte[] data = Utils.HexToBytes(canParts[1]);

// Decode the message
var decodedSignals = dbcDatabase.DecodeMessage(canId, data);
```

### Converting Hex Strings to Bytes
```csharp
// Convert hex string to byte array (handles whitespace and odd-length strings)
byte[] bytes = Utils.HexToBytes("12 34 56 78 9A BC DE F0");
// Result: [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0]

// With quoted string
byte[] fromQuoted = Utils.HexToBytes("\"12 34 56 78\"");
// Result: [0x12, 0x34, 0x56, 0x78]

// With odd-length string (leading zero added automatically)
byte[] fromOdd = Utils.HexToBytes("1 23 456");
// Result: [0x01, 0x23, 0x04, 0x56]
```

### Safe Number Parsing
```csharp
// Parse string to various numeric types with error handling
int intValue = Utils.SafeParse<int>("123");
double doubleValue = Utils.SafeParse<double>("45.67");
byte hexByte = Utils.SafeParse<byte>("FF", hex: true);
uint hexUint = Utils.SafeParse<uint>("DEADBEEF", hex: true);
```

### String Extensions
```csharp
// Split string with delimiter and clean results
string data = "  value1, value2,,  value3  ";
List<string> values = data.Split(',');
// Result: ["value1", "value2", "value3"]

// Null-safe trimming
string nullableString = null;
string result = nullableString.Trim(); // Returns empty string instead of throwing exception
```

## Classes

- **DBCDatabase**: Main class for loading and managing DBC definitions
- **DBCMessage**: Represents a CAN message with multiple signals
- **DBCSignal**: Represents a signal within a CAN message
- **Utils**: Helper methods for parsing and data conversion
- **StringExtension**: String manipulation utilities

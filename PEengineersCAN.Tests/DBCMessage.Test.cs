using System;
using System.Collections.Generic;
using Xunit;

namespace PEengineersCAN.Tests
{
    public class DBCMessageTests
    {
        [Fact]
        public void Decode_WithValidSignals_ReturnsCorrectValues()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "TestMessage",
                Dlc = 8,
                Transmitter = "TestECU",
                Signals = new List<DBCSignal>
                {
                    new DBCSignal
                    {
                        Name = "Signal1",
                        StartBit = 0,
                        Length = 8,
                        IsLittleEndian = true,
                        Factor = 1.0,
                        Offset = 0.0
                    },
                    new DBCSignal
                    {
                        Name = "Signal2",
                        StartBit = 8,
                        Length = 16,
                        IsLittleEndian = true,
                        Factor = 0.1,
                        Offset = 5.0
                    }
                }
            };

            byte[] data = { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(18.0, result["Signal1"]); // 0x12 = 18
            Assert.Equal(5.0 + (0x5634 * 0.1), result["Signal2"]); // 0x5634 = 22068, * 0.1 + 5.0 = 2211.8
        }

        [Fact]
        public void Decode_WithEmptySignalList_ReturnsEmptyDictionary()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "EmptyMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>()
            };

            byte[] data = { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Decode_WithNullData_ReturnsZeroValues()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "TestMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal { Name = "Signal1", StartBit = 0, Length = 8 },
                    new DBCSignal { Name = "Signal2", StartBit = 8, Length = 16 }
                }
            };

            // Act
            var result = message.Decode(null);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(0.0, result["Signal1"]);
            Assert.Equal(0.0, result["Signal2"]);
        }

        [Fact]
        public void Decode_WithEmptyData_ReturnsZeroValues()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "TestMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal { Name = "Signal1", StartBit = 0, Length = 8 },
                    new DBCSignal { Name = "Signal2", StartBit = 8, Length = 16 }
                }
            };

            byte[] data = new byte[0];

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(0.0, result["Signal1"]);
            Assert.Equal(0.0, result["Signal2"]);
        }

        [Fact]
        public void Decode_WithDataShorterThanSignalRequires_HandlesGracefully()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "TestMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal { Name = "Signal1", StartBit = 0, Length = 8 },
                    new DBCSignal { Name = "Signal2", StartBit = 16, Length = 32 } // Requires 4 bytes
                }
            };

            byte[] data = { 0x12, 0x34 }; // Only 2 bytes

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(18.0, result["Signal1"]); // First signal should still be decoded
            Assert.Equal(0.0, result["Signal2"]); // Second signal should be 0 due to insufficient data
        }
        

        [Fact]
        public void Decode_WithSignedSignals_ReturnsCorrectValues()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "SignedMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal
                    {
                        Name = "SignedSignal",
                        StartBit = 0,
                        Length = 8,
                        IsLittleEndian = true,
                        IsSigned = true,
                        Factor = 1.0,
                        Offset = 0.0
                    }
                }
            };

            byte[] data = { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // 0xFF is -1 as signed 8-bit

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Single(result);
            Assert.Equal(-1.0, result["SignedSignal"]);
        }

        [Fact]
        public void Decode_WithFactorAndOffset_ReturnsCorrectValues()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "ScaledMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal
                    {
                        Name = "Temperature",
                        StartBit = 0,
                        Length = 8,
                        Factor = 0.5, // Each count is 0.5 degrees
                        Offset = -40.0 // Offset of -40 degrees
                    }
                }
            };

            byte[] data = { 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // 0x64 = 100 decimal

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Single(result);
            // Raw value 100 * factor 0.5 + offset -40 = 10 degrees
            Assert.Equal(10.0, result["Temperature"]);
        }

        [Fact]
        public void Decode_WithMultipleOverlappingSignals_ProcessesAllSignals()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "OverlappingMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal
                    {
                        Name = "FullByte",
                        StartBit = 0,
                        Length = 8,
                    },
                    new DBCSignal
                    {
                        Name = "LowerNibble",
                        StartBit = 0,
                        Length = 4,
                    },
                    new DBCSignal
                    {
                        Name = "UpperNibble",
                        StartBit = 4,
                        Length = 4,
                    }
                }
            };

            byte[] data = { 0xAB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // 0xAB = 171 decimal

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(171.0, result["FullByte"]); // 0xAB = 171
            Assert.Equal(11.0, result["LowerNibble"]); // 0xB = 11
            Assert.Equal(10.0, result["UpperNibble"]); // 0xA = 10
        }

        [Fact]
        public void Decode_WithInvalidSignalConfiguration_ReturnsZero()
        {
            // Arrange
            var message = new DBCMessage
            {
                Id = 0x100,
                Name = "InvalidMessage",
                Dlc = 8,
                Signals = new List<DBCSignal>
                {
                    new DBCSignal
                    {
                        Name = "InvalidSignal",
                        StartBit = -5, // Invalid start bit
                        Length = 8,
                    }
                }
            };

            byte[] data = { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

            // Act
            var result = message.Decode(data);

            // Assert
            Assert.Single(result);
            Assert.Equal(0.0, result["InvalidSignal"]);
        }
    }
}
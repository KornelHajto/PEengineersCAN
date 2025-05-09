﻿using System;
using Xunit;

namespace PEengineersCAN.Tests
{
    public class DBCSignalTests
    {
        [Fact]
        public void GetValue_LittleEndianUnsigned_ReturnsCorrectValue()
        {
            // Arrange
            var signal = new DBCSignal
            {
                Name = "TestSignal",
                StartBit = 8,
                Length = 16,
                IsLittleEndian = true,
                IsSigned = false,
                Factor = 0.1,
                Offset = 10
            };
            byte[] data = { 0x00, 0x64, 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Act
            double result = signal.GetValue(data);

            // Assert
            Assert.Equal(51300, (data[1] | (data[2] << 8))); // Raw value: 0xC864 = 51,300
            Assert.Equal(5140, result); // (51300 * 0.1) + 10 = 5140
        }
        
        [Fact]
        public void GetValue_LittleEndianSigned_ReturnsCorrectValue()
        {
            // Arrange
            var signal = new DBCSignal
            {
                Name = "TestSignal",
                StartBit = 8,
                Length = 16,
                IsLittleEndian = true,
                IsSigned = true,
                Factor = 0.1,
                Offset = 10
            };
            byte[] data = { 0x00, 0x64, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00 };
            
            // Act
            double result = signal.GetValue(data);
            
            // Assert
            // Raw value 0x8064 = -32668 (signed)
            Assert.Equal(-3256.8, result, 1); // (-32668 * 0.1) + 10 = -3256.8
        }
        

        [Fact]
        public void GetValue_NonByteAligned_ReturnsCorrectValue()
        {
            // Arrange
            var signal = new DBCSignal
            {
                Name = "TestSignal",
                StartBit = 10, // Not byte-aligned
                Length = 12,   // Not a multiple of 8
                IsLittleEndian = true,
                IsSigned = false,
                Factor = 1.0,
                Offset = 0.0
            };
            byte[] data = { 0x00, 0xF0, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Act
            double result = signal.GetValue(data);

            // Assert
            Assert.Equal(2812, result);
        }
        
        [Fact]
        public void GetValue_NullOrEmptyData_ReturnsZero()
        {
            // Arrange
            var signal = new DBCSignal
            {
                Name = "TestSignal",
                StartBit = 8,
                Length = 16
            };
            
            // Act & Assert
            Assert.Equal(0.0, signal.GetValue(null));
            Assert.Equal(0.0, signal.GetValue(new byte[0]));
        }
        
        [Fact]
        public void GetValue_InvalidSignalParameters_ReturnsZero()
        {
            // Arrange
            var signal = new DBCSignal
            {
                Name = "TestSignal",
                StartBit = -1, // Invalid start bit
                Length = 0     // Invalid length
            };
            byte[] data = { 0x00, 0x64, 0xC8, 0x00 };
            
            // Act & Assert
            Assert.Equal(0.0, signal.GetValue(data));
        }
        
        [Fact]
        public void SignalProperties_SetAndGet_WorksCorrectly()
        {
            // Arrange
            var signal = new DBCSignal
            {
                Name = "SpeedSignal",
                Unit = "km/h",
                Receiver = "BodyController",
                StartBit = 8,
                Length = 16,
                Factor = 0.1,
                Offset = 0,
                IsLittleEndian = true,
                IsSigned = false
            };

            // Act & Assert
            Assert.Equal("SpeedSignal", signal.Name);
            Assert.Equal("km/h", signal.Unit);
            Assert.Equal("BodyController", signal.Receiver);
            Assert.Equal(8, signal.StartBit);
            Assert.Equal(16, signal.Length);
            Assert.Equal(0.1, signal.Factor);
            Assert.Equal(0, signal.Offset);
            Assert.True(signal.IsLittleEndian);
            Assert.False(signal.IsSigned);
        }
    }
}
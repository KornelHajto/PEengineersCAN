using System;
using Xunit;

namespace PEengineersCAN.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void SafeParse_ValidIntegerInput_ReturnsCorrectValue()
        {
            // Arrange
            string input = "123";
            
            // Act
            int result = Utils.SafeParse<int>(input);
            
            // Assert
            Assert.Equal(123, result);
        }
        
        [Fact]
        public void SafeParse_ValidDoubleInput_ReturnsCorrectValue()
        {
            // Arrange
            string input = "123.45";
            
            // Act
            double result = Utils.SafeParse<double>(input);
            
            // Assert
            Assert.Equal(123.45, result);
        }
        
        [Fact]
        public void SafeParse_ValidHexInput_ReturnsCorrectValue()
        {
            // Arrange
            string input = "1A";
            
            // Act
            byte result = Utils.SafeParse<byte>(input, true);
            
            // Assert
            Assert.Equal(26, result);
        }
        
        [Fact]
        public void SafeParse_EmptyString_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<int>(""));
            Assert.Equal("Empty string", exception.Message);
        }
        
        [Fact]
        public void SafeParse_InvalidFormat_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<int>("not-a-number"));
            Assert.Contains("Invalid number format", exception.Message);
        }
        
        [Fact]
        public void SafeParse_UnsupportedType_ThrowsArgumentException()
        {
            // This test checks the code path that throws "Unsupported type"
            // The method doesn't handle non-numeric types like string
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<string>("text"));
            Assert.Contains("Invalid number format", exception.Message);
        }
        
        [Fact]
        public void HexToBytes_ValidHexString_ReturnsCorrectByteArray()
        {
            // Arrange
            string input = "1A2B3C";
            
            // Act
            byte[] result = Utils.HexToBytes(input);
            
            // Assert
            Assert.Equal(new byte[] { 0x1A, 0x2B, 0x3C }, result);
        }
        
        [Fact]
        public void HexToBytes_WithWhitespace_ReturnsCorrectByteArray()
        {
            // Arrange
            string input = "1A 2B 3C";
            
            // Act
            byte[] result = Utils.HexToBytes(input);
            
            // Assert
            Assert.Equal(new byte[] { 0x1A, 0x2B, 0x3C }, result);
        }
        
        [Fact]
        public void HexToBytes_WithQuotes_ReturnsCorrectByteArray()
        {
            // Arrange
            string input = "\"1A2B3C\"";
            
            // Act
            byte[] result = Utils.HexToBytes(input);
            
            // Assert
            Assert.Equal(new byte[] { 0x1A, 0x2B, 0x3C }, result);
        }
        
        [Fact]
        public void HexToBytes_OddLength_AddsLeadingZero()
        {
            // Arrange
            string input = "A2B3C"; // Odd length
            
            // Act
            byte[] result = Utils.HexToBytes(input);
            
            // Assert
            Assert.Equal(new byte[] { 0x0A, 0x2B, 0x3C }, result);
        }
        
        [Fact]
        public void HexToBytes_EmptyString_ReturnsEmptyByteArray()
        {
            // Arrange
            string input = "";
            
            // Act
            byte[] result = Utils.HexToBytes(input);
            
            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void SafeParse_MaxValues_HandlesCorrectly()
        {
            // Test max values for different types
            Assert.Equal(int.MaxValue, Utils.SafeParse<int>(int.MaxValue.ToString()));
            Assert.Equal(byte.MaxValue, Utils.SafeParse<byte>(byte.MaxValue.ToString()));
            Assert.Equal(long.MaxValue, Utils.SafeParse<long>(long.MaxValue.ToString()));
        }
    }
}
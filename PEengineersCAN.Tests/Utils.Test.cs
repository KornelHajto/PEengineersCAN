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
        [Fact]
        public void SafeParse_UintType_ReturnsCorrectValue()
        {
            // Arrange
            string input = "4294967295"; // uint.MaxValue

            // Act
            uint result = Utils.SafeParse<uint>(input);

            // Assert
            Assert.Equal(uint.MaxValue, result);
        }

        [Fact]
        public void SafeParse_UlongType_ReturnsCorrectValue()
        {
            // Arrange
            string input = "18446744073709551615"; // ulong.MaxValue

            // Act
            ulong result = Utils.SafeParse<ulong>(input);

            // Assert
            Assert.Equal(ulong.MaxValue, result);
        }

        [Fact]
        public void SafeParse_HexForUint_ReturnsCorrectValue()
        {
            // Arrange
            string input = "FFFFFFFF"; // uint.MaxValue in hex

            // Act
            uint result = Utils.SafeParse<uint>(input, true);

            // Assert
            Assert.Equal(uint.MaxValue, result);
        }

        [Fact]
        public void SafeParse_HexForInt_ReturnsCorrectValue()
        {
            // Arrange
            string input = "7FFFFFFF"; // int.MaxValue in hex

            // Act
            int result = Utils.SafeParse<int>(input, true);

            // Assert
            Assert.Equal(int.MaxValue, result);
        }

        [Fact]
        public void SafeParse_HexForLong_ReturnsCorrectValue()
        {
            // Arrange
            string input = "7FFFFFFFFFFFFFFF"; // long.MaxValue in hex

            // Act
            long result = Utils.SafeParse<long>(input, true);

            // Assert
            Assert.Equal(long.MaxValue, result);
        }

        [Fact]
        public void SafeParse_HexForUlong_ReturnsCorrectValue()
        {
            // Arrange
            string input = "FFFFFFFFFFFFFFFF"; // ulong.MaxValue in hex

            // Act
            ulong result = Utils.SafeParse<ulong>(input, true);

            // Assert
            Assert.Equal(ulong.MaxValue, result);
        }
        
        [Fact]
        public void SafeParse_HexForByte_ReturnsCorrectValue()
        {
            // Arrange
            string input = "FF"; // Hexadecimal for 255
    
            // Act
            byte result = Utils.SafeParse<byte>(input, true);
    
            // Assert
            Assert.Equal(255, result);
        }
        
                [Fact]
        public void SafeParse_ValidFloatInput_ReturnsCorrectValue()
        {
            // Arrange
            string input = "123.45";
            
            // Act
            float result = Utils.SafeParse<float>(input);
            
            // Assert
            Assert.Equal(123.45f, result);
        }

        [Fact]
        public void SafeParse_InvalidHexString_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<int>("ZZZZ", true));
            Assert.Contains("Invalid number format", exception.Message);
        }

        [Fact]
        public void SafeParse_LeadingTrailingSpaces_ReturnsCorrectValue()
        {
            // Arrange
            string input = "  42  ";
            
            // Act
            int result = Utils.SafeParse<int>(input);
            
            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void SafeParse_ByteOverflow_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<byte>("256"));
            Assert.Contains("Invalid number format", exception.Message);
        }

        [Fact]
        public void SafeParse_IntOverflow_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<int>("2147483648"));
            Assert.Contains("Invalid number format", exception.Message);
        }

        [Fact]
        public void SafeParse_UnsupportedBoolType_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.SafeParse<bool>("true"));
            Assert.Contains("Unsupported type", exception.Message);
        }

        [Fact]
        public void HexToBytes_InvalidHexString_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Utils.HexToBytes("GHIJKL"));
            Assert.Contains("Invalid number format", exception.Message);
        }

        [Fact]
        public void HexToBytes_WhitespaceOnly_ReturnsEmptyByteArray()
        {
            // Arrange
            string input = "   ";
            
            // Act
            byte[] result = Utils.HexToBytes(input);
            
            // Assert
            Assert.Empty(result);
        }

        
    }
}
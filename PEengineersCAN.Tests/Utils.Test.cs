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

        [Fact]
        public void TryParse_ValidIntegerString_ReturnsTrueAndValue()
        {
            bool success = Utils.TryParse<int>("42", out int value);
            Assert.True(success);
            Assert.Equal(42, value);
        }

        [Fact]
        public void TryParse_InvalidIntegerString_ReturnsFalseAndDefault()
        {
            bool success = Utils.TryParse<int>("notanumber", out int value);
            Assert.False(success);
            Assert.Equal(0, value);
        }

        [Fact]
        public void TryParse_EmptyString_ReturnsFalseAndDefault()
        {
            bool success = Utils.TryParse<int>("", out int value);
            Assert.False(success);
            Assert.Equal(0, value);
        }

        [Fact]
        public void TryParse_UnsupportedType_ReturnsFalseAndDefault()
        {
            bool success = Utils.TryParse<bool>("true", out bool value);
            Assert.False(success);
            Assert.False(value);
        }

        [Fact]
        public void TryParse_ValidHexString_ReturnsTrueAndValue()
        {
            bool success = Utils.TryParse<byte>("1A", out byte value, hex: true);
            Assert.True(success);
            Assert.Equal(0x1A, value);
        }

        [Fact]
        public void TryParseSpan_ValidInteger_ReturnsTrueAndValue()
        {
            ReadOnlySpan<char> span = "123".AsSpan();
            bool success = Utils.TryParse<int>(span, out int value);
            Assert.True(success);
            Assert.Equal(123, value);
        }

        [Fact]
        public void TryParseSpan_InvalidInteger_ReturnsFalseAndDefault()
        {
            ReadOnlySpan<char> span = "bad".AsSpan();
            bool success = Utils.TryParse<int>(span, out int value);
            Assert.False(success);
            Assert.Equal(0, value);
        }

        [Fact]
        public void TryParseSpan_Empty_ReturnsFalseAndDefault()
        {
            ReadOnlySpan<char> span = ReadOnlySpan<char>.Empty;
            bool success = Utils.TryParse<int>(span, out int value);
            Assert.False(success);
            Assert.Equal(0, value);
        }

        [Fact]
        public void TryParseSpan_UnsupportedType_ReturnsFalseAndDefault()
        {
            ReadOnlySpan<char> span = "true".AsSpan();
            bool success = Utils.TryParse<bool>(span, out bool value);
            Assert.False(success);
            Assert.False(value);
        }

        [Fact]
        public void TryParseSpan_ValidHex_ReturnsTrueAndValue()
        {
            ReadOnlySpan<char> span = "FF".AsSpan();
            bool success = Utils.TryParse<byte>(span, out byte value, hex: true);
            Assert.True(success);
            Assert.Equal(255, value);
        }

        [Fact]
        public void TryParse_NegativeIntegerString_ReturnsTrueAndValue()
        {
            bool success = Utils.TryParse<int>("-7", out int value);
            Assert.True(success);
            Assert.Equal(-7, value);
        }

        [Fact]
        public void TryParse_NullString_ReturnsFalseAndDefault()
        {
            string input = null;
            bool success = Utils.TryParse<int>(input, out int value);
            Assert.False(success);
            Assert.Equal(0, value);
        }

        [Fact]
        public void TryParse_ValidHexString_IntType_ReturnsTrueAndValue()
        {
            bool success = Utils.TryParse<int>("1A2B", out int value, hex: true);
            Assert.True(success);
            Assert.Equal(0x1A2B, value);
        }

        [Fact]
        public void TryParse_ValidHexString_LongType_ReturnsTrueAndValue()
        {
            bool success = Utils.TryParse<long>("7FFFFFFFFFFFFFFF", out long value, hex: true);
            Assert.True(success);
            Assert.Equal(9223372036854775807L, value); // long.MaxValue
        }

        // --- Additional coverage for all supported numeric types ---
        [Fact]
        public void TryParse_ShortTypes_ReturnsTrueAndValue()
        {
            Assert.True(Utils.TryParse<short>("123", out var s));
            Assert.Equal((short)123, s);
            Assert.True(Utils.TryParse<ushort>("456", out var us));
            Assert.Equal((ushort)456, us);
            Assert.True(Utils.TryParse<sbyte>("-12", out var sb));
            Assert.Equal((sbyte)-12, sb);
        }

        [Fact]
        public void TryParse_FloatAndDoubleTypes_ReturnsTrueAndValue()
        {
            Assert.True(Utils.TryParse<float>("3.14", out var f));
            Assert.Equal(3.14f, f, 2);
            Assert.True(Utils.TryParse<double>("2.718", out var d));
            Assert.Equal(2.718, d, 3);
        }

        [Fact]
        public void TryParse_DecimalType_ReturnsFalse()
        {
            // Not supported, should return false
            Assert.False(Utils.TryParse<decimal>("1.23", out var dec));
            Assert.Equal(0m, dec);
        }

        // --- Unsupported types ---
        private class Dummy {}
        [Fact]
        public void TryParse_UnsupportedStringAndCustomType_ReturnsFalseAndDefault()
        {
            Assert.False(Utils.TryParse<string>("test", out var str));
            Assert.Null(str);
            Assert.False(Utils.TryParse<Dummy>("test", out var dummy));
            Assert.Null(dummy);
        }

        [Fact]
        public void SafeParse_UnsupportedStringAndCustomType_Throws()
        {
            Assert.Throws<ArgumentException>(() => Utils.SafeParse<string>("test"));
            Assert.Throws<ArgumentException>(() => Utils.SafeParse<Dummy>("test"));
        }

        // --- HexToBytes edge cases ---
        [Fact]
        public void HexToBytes_OnlyQuotes_ReturnsEmpty()
        {
            Assert.Empty(Utils.HexToBytes("\"\""));
        }
        [Fact]
        public void HexToBytes_WhitespaceAndQuotes_ReturnsEmpty()
        {
            Assert.Empty(Utils.HexToBytes("   \"   \"   "));
        }
        [Fact]
        public void HexToBytes_SingleChar_ReturnsSingleByte()
        {
            var result = Utils.HexToBytes("F");
            Assert.Single(result);
            Assert.Equal(0x0F, result[0]);
        }
        [Fact]
        public void HexToBytes_OddLengthWithWhitespace_ReturnsCorrect()
        {
            var result = Utils.HexToBytes(" 1 2 3 ");
            Assert.Equal(new byte[] { 0x01, 0x23 }, result);
        }
        [Fact]
        public void HexToBytes_InvalidCharInMiddle_Throws()
        {
            Assert.Throws<ArgumentException>(() => Utils.HexToBytes("1A2X3C"));
        }

        // --- TryParse error/overflow paths ---
        [Fact]
        public void TryParse_OverflowAndNegativeUnsigned_ReturnsFalse()
        {
            Assert.False(Utils.TryParse<byte>("256", out var b));
            Assert.Equal(0, b);
            Assert.False(Utils.TryParse<uint>("-1", out var u));
            Assert.Equal(0u, u);
        }
        [Fact]
        public void TryParse_FloatAndDouble_Invalid_ReturnsFalse()
        {
            Assert.False(Utils.TryParse<float>("notafloat", out var f));
            Assert.Equal(0f, f);
            Assert.False(Utils.TryParse<double>("notadouble", out var d));
            Assert.Equal(0d, d);
        }
    }
}
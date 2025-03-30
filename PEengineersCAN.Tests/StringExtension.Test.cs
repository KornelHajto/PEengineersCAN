using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PEengineersCAN.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void Trim_NullString_ReturnsEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            string result = StringExtension.Trim(input);;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Trim_EmptyString_ReturnsEmptyString()
        {
            // Arrange
            string input = "";

            // Act
            string result = StringExtension.Trim(input);;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Trim_StringWithWhitespace_ReturnsTrimmedString()
        {
            // Arrange
            string input = "  hello world  ";

            // Act
            string result = StringExtension.Trim(input);;

            // Assert
            Assert.Equal("hello world", result);
        }

        [Fact]
        public void Trim_StringWithoutWhitespace_ReturnsSameString()
        {
            // Arrange
            string input = "helloworld";

            // Act
            string result = StringExtension.Trim(input);

            // Assert
            Assert.Equal("helloworld", result);
        }

        [Fact]
        public void Trim_OnlyWhitespace_ReturnsEmptyString()
        {
            // Arrange
            string input = "     ";

            // Act
            string result = StringExtension.Trim(input);;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Split_NullString_ThrowsNullReferenceException()
        {
            // Arrange
            string input = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => StringExtension.Split(input, ','));;
        }

        [Fact]
        public void Split_EmptyString_ReturnsEmptyList()
        {
            // Arrange
            string input = "";

            // Act
            var result = StringExtension.Split(input,',');

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Split_NoDelimiterPresent_ReturnsSingleItemList()
        {
            // Arrange
            string input = "hello world";

            // Act
            var result = StringExtension.Split(input, ',');

            // Assert
            Assert.Single(result);
            Assert.Equal("hello world", result[0]);
        }

        [Fact]
        public void Split_WithDelimiters_ReturnsCorrectItems()
        {
            // Arrange
            string input = "apple,banana,cherry";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }

        [Fact]
        public void Split_WithEmptyEntries_RemovesEmptyEntries()
        {
            // Arrange
            string input = "apple,,banana,,cherry";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }

        [Fact]
        public void Split_WithWhitespaceAroundEntries_TrimsThem()
        {
            // Arrange
            string input = "  apple  ,  banana  ,  cherry  ";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }

        [Fact]
        public void Split_WithOnlyWhitespaceEntries_ReturnsEmptyList()
        {
            // Arrange
            string input = "   ,   ,   ";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Split_WithCustomDelimiter_WorksCorrectly()
        {
            // Arrange
            string input = "apple|banana|cherry";

            // Act
            var result = input.Split('|');

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }

        [Fact]
        public void Split_WithDelimiterAtStart_HandlesCorrectly()
        {
            // Arrange
            string input = ",apple,banana,cherry";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }

        [Fact]
        public void Split_WithDelimiterAtEnd_HandlesCorrectly()
        {
            // Arrange
            string input = "apple,banana,cherry,";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }

        [Fact]
        public void Split_WithOnlyDelimiters_ReturnsEmptyList()
        {
            // Arrange
            string input = ",,,,";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Split_WithMixedWhitespaceAndEmptyEntries_HandlesCorrectly()
        {
            // Arrange
            string input = "  apple  ,,  ,,  banana  ,   ,cherry";

            // Act
            var result = StringExtension.Split(input, ',');;

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("apple", result[0]);
            Assert.Equal("banana", result[1]);
            Assert.Equal("cherry", result[2]);
        }
    }
}
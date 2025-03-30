using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PEengineersCAN.Tests
{
    public class DBCDatabaseTests
    {
        private string CreateTempDbcFile(string content)
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, content);
            return tempFile;
        }

        [Fact]
        public void Load_WithValidDBC_ParsesMessagesAndSignals()
        {
            // Arrange
            string dbcContent = @"
BO_ 100 Engine: 8 ECU
 SG_ RPM : 0|16@1+ (1,0) [0|8000] ""rpm"" Vector__XXX
 SG_ Temperature : 16|8@1+ (0.5,-40) [-40|150] ""degC"" Vector__XXX

BO_ 200 Transmission: 8 ECU
 SG_ Gear : 0|4@1+ (1,0) [0|8] """" Vector__XXX
 SG_ Speed : 8|16@1+ (0.1,0) [0|300] ""km/h"" Vector__XXX
";
            string tempFile = CreateTempDbcFile(dbcContent);

            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile);

                // Assert - Try to decode valid messages
                var engineData = new byte[] { 0x20, 0x4E, 0x64, 0, 0, 0, 0, 0 }; // RPM = 20000, Temp = 100
                var engineResult = db.DecodeMessage(100, engineData);

                Assert.Equal(2, engineResult.Count);
                Assert.Equal(20000.0, engineResult["RPM"]);
                Assert.Equal(10.0, engineResult["Temperature"]); // 100 * 0.5 - 40 = 10

                var transData = new byte[] { 0x03, 0xC8, 0x00, 0, 0, 0, 0, 0 }; // Gear = 3, Speed = 200
                var transResult = db.DecodeMessage(200, transData);

                Assert.Equal(2, transResult.Count);
                Assert.Equal(3.0, transResult["Gear"]);
                Assert.Equal(20.0, transResult["Speed"]); // 200 * 0.1 = 20
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void DecodeMessage_WithUnknownId_ThrowsException()
        {
            // Arrange
            string dbcContent = @"
BO_ 100 Engine: 8 ECU
 SG_ RPM : 0|16@1+ (1,0) [0|8000] ""rpm"" Vector__XXX
";
            string tempFile = CreateTempDbcFile(dbcContent);

            var db = new DBCDatabase();
            db.Load(tempFile);

            try
            {
                // Act & Assert
                var ex = Assert.Throws<InvalidOperationException>(() => 
                    db.DecodeMessage(999, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }));
                
                Assert.Contains("Message ID 999", ex.Message);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_WithEmptyFile_DoesNotThrowException()
        {
            // Arrange
            string tempFile = CreateTempDbcFile("");
            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile);

                // Assert - No messages should be loaded
                var ex = Assert.Throws<InvalidOperationException>(() => 
                    db.DecodeMessage(100, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }));
                Assert.Contains("Message ID 100", ex.Message);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_WithMalformedMessages_SkipsInvalidLines()
        {
            // Arrange
            string dbcContent = @"
BO_ INVALID Invalid: 8 ECU
 SG_ RPM : 0|16@1+ (1,0) [0|8000] ""rpm"" Vector__XXX

BO_ 200 Transmission: 8 ECU
 SG_ Gear : 0|4@1+ (1,0) [0|8] """" Vector__XXX
";
            string tempFile = CreateTempDbcFile(dbcContent);

            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile);

                // Assert
                // Valid message should be loaded
                var transData = new byte[] { 0x03, 0, 0, 0, 0, 0, 0, 0 }; // Gear = 3
                var transResult = db.DecodeMessage(200, transData);
                Assert.Single(transResult);
                Assert.Equal(3.0, transResult["Gear"]);

                // Invalid message should not be loaded
                Assert.Throws<InvalidOperationException>(() => 
                    db.DecodeMessage(100, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }));
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_WithMalformedSignals_SkipsInvalidSignals()
        {
            // Arrange
            string dbcContent = @"
BO_ 100 Engine: 8 ECU
 SG_ RPM : 0|16@1+ (1,0) [0|8000] ""rpm"" Vector__XXX
 SG_ BadSignal : INVALID
 SG_ Temperature : 16|8@1+ (0.5,-40) [-40|150] ""degC"" Vector__XXX
";
            string tempFile = CreateTempDbcFile(dbcContent);

            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile);

                // Assert
                var engineData = new byte[] { 0x20, 0x4E, 0x64, 0, 0, 0, 0, 0 }; // RPM = 20000, Temp = 100
                var engineResult = db.DecodeMessage(100, engineData);

                // Should have two valid signals
                Assert.Equal(2, engineResult.Count);
                Assert.Equal(20000.0, engineResult["RPM"]);
                Assert.Equal(10.0, engineResult["Temperature"]);
                
                // Bad signal should not be present
                Assert.False(engineResult.ContainsKey("BadSignal"));
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_WithSignalsBeforeMessage_IgnoresOrphanedSignals()
        {
            // Arrange
            string dbcContent = @"
 SG_ OrphanSignal : 0|8@1+ (1,0) [0|255] """" Vector__XXX

BO_ 100 Engine: 8 ECU
 SG_ RPM : 0|16@1+ (1,0) [0|8000] ""rpm"" Vector__XXX
";
            string tempFile = CreateTempDbcFile(dbcContent);

            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile);

                // Assert
                var engineData = new byte[] { 0x20, 0x4E, 0, 0, 0, 0, 0, 0 }; // RPM = 20000
                var engineResult = db.DecodeMessage(100, engineData);

                // Should only have the valid signal
                Assert.Single(engineResult);
                Assert.Equal(20000.0, engineResult["RPM"]);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_WithMessageNoSignals_ParsesMessageCorrectly()
        {
            // Arrange
            string dbcContent = @"
BO_ 100 Empty: 8 ECU
";
            string tempFile = CreateTempDbcFile(dbcContent);

            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile);

                // Assert
                var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                var result = db.DecodeMessage(100, data);

                // Message exists but has no signals
                Assert.Empty(result);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_WithMultipleFiles_AccumulatesMessages()
        {
            // Arrange
            string dbcContent1 = @"
BO_ 100 Engine: 8 ECU
 SG_ RPM : 0|16@1+ (1,0) [0|8000] ""rpm"" Vector__XXX
";
            string dbcContent2 = @"
BO_ 200 Transmission: 8 ECU
 SG_ Gear : 0|4@1+ (1,0) [0|8] """" Vector__XXX
";
            string tempFile1 = CreateTempDbcFile(dbcContent1);
            string tempFile2 = CreateTempDbcFile(dbcContent2);

            var db = new DBCDatabase();

            try
            {
                // Act
                db.Load(tempFile1);
                db.Load(tempFile2);

                // Assert - Both messages should be available
                var engineData = new byte[] { 0x20, 0x4E, 0, 0, 0, 0, 0, 0 }; // RPM = 20000
                var engineResult = db.DecodeMessage(100, engineData);
                Assert.Equal(20000.0, engineResult["RPM"]);

                var transData = new byte[] { 0x03, 0, 0, 0, 0, 0, 0, 0 }; // Gear = 3
                var transResult = db.DecodeMessage(200, transData);
                Assert.Equal(3.0, transResult["Gear"]);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile1);
                File.Delete(tempFile2);
            }
        }
    }
}
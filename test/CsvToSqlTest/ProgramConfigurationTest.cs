using System;
using CsvToSql.Core;
using CsvToSql.logging;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CsvToSqlTest
{
    public class ProgramConfigurationTest
    {

        private readonly Logging log;

        public ProgramConfigurationTest()
        {
            log = new Logging();
        }

        [Test]
        public void ProgramConfiguration_Readdefault()
        {
            // Arrange
            string[] args = { "", "" };

            // Act
            var programCfg = CsvToSql.Configuration.ProgramConfiguration.Read(log, args);

            // Assert
            Assert.AreEqual(programCfg.JsonCfgFile, "settings.json");
        }
     
        [Test]
        public void ProgramConfiguration_ReadSettings()
        {
            // Arrange
            string[] args = { "settings=abc.json", "truncate=true" };

            // Act
            var programCfg = CsvToSql.Configuration.ProgramConfiguration.Read(log, args);

            // Assert
            Assert.AreEqual(programCfg.JsonCfgFile, "abc.json");
        }
    }
}
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
            string[] args = { "--settings", "abc.json", "--truncate", "--importdate", "now" };

            // Act
            var programCfg = CsvToSql.Configuration.ProgramConfiguration.Read(log, args);

            // Assert
            Assert.AreEqual(programCfg.JsonCfgFile, "abc.json");
            Assert.AreEqual(programCfg.Truncate, true);
            Assert.AreEqual(programCfg.ImportDate, "now");
        }

        [TestCase("2021-01-5")]
        [TestCase("2021-01-25")]
        [TestCase("2021-01-25 12:03")]
        [TestCase("2021/05/1")]
        [TestCase("2021/05/01")]
        [TestCase("2021/25/01 12:03")]
        public void ProgramConfiguration_ReadDataTimeFromSettings(string importDate)
        {
            // Arrange
            string[] args = { "--importdate", importDate };

            // Act
            var programCfg = CsvToSql.Configuration.ProgramConfiguration.Read(log, args);

            // Assert
            Assert.AreEqual(programCfg.ImportDateTime.Year, 2021);
            Assert.AreEqual(programCfg.ImportDateTime.Month, 1);
        }
    }
}
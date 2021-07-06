using FakeItEasy;

using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;
using MarkZither.KimaiDotNet.ExcelAddin.Services;

using System;
using System.IO;
using System.Xml.Serialization;

using Xunit;

namespace MarkZither.KimaiDotNet.Core.Tests.Services
{
    public class EwsCalendarServiceTests
    {


        public EwsCalendarServiceTests()
        {

        }

        private EwsCalendarService CreateService()
        {
            return new EwsCalendarService(
                "",
                "",
                "");
        }

        [Fact]
        public void GetAppointments_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.GetAppointments();

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void GetCategories_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.GetCategories();

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void GetCategories_InvalidXMLFromEWS_CannotBeDeserialized()
        {
            // Arrange
            var service = this.CreateService();
            var xml = @"ï»À<?xml version=""1.0""?>
<categories default=""Red Category"" lastSavedSession=""3"" lastSavedTime=""2021-07-05T20:23:33.2235524Z"" xmlns=""CategoryList.xsd"">
	<category usageCount=""5"" lastSessionUsed=""1"" name=""Red Category"" color=""0"" keyboardShortcut=""0"" lastTimeUsedNotes=""2018-09-14T08:46:44.962Z"" lastTimeUsedJournal=""2018-09-14T08:46:44.962Z"" lastTimeUsedContacts=""2018-09-14T08:46:44.962Z"" lastTimeUsedTasks=""2018-09-14T08:46:44.962Z"" lastTimeUsedCalendar=""2018-09-14T08:46:44.962Z"" lastTimeUsedMail=""2018-09-14T08:46:44.962Z"" lastTimeUsed=""2018-10-01T12:10:05.558Z"" guid=""{a27f8c66-3621-4183-bd29-6a7d34897811}"" renameOnFirstUse=""0"" />
	<category usageCount=""0"" lastSessionUsed=""0"" renameOnFirstUse=""1"" name=""Orange Category"" color=""1"" keyboardShortcut=""0"" lastTimeUsedNotes=""2018-09-14T08:46:44.962Z"" lastTimeUsedJournal=""2018-09-14T08:46:44.962Z"" lastTimeUsedContacts=""2018-09-14T08:46:44.962Z"" lastTimeUsedTasks=""2018-09-14T08:46:44.962Z"" lastTimeUsedCalendar=""2018-09-14T08:46:44.962Z"" lastTimeUsedMail=""2018-09-14T08:46:44.962Z"" lastTimeUsed=""2018-09-14T08:46:44.962Z"" guid=""{088cd788-76b7-49bd-9ce3-e40a99b954e8}"" />
	<category usageCount=""0"" lastSessionUsed=""0"" renameOnFirstUse=""1"" name=""Yellow Category"" color=""3"" keyboardShortcut=""0"" lastTimeUsedNotes=""2018-09-14T08:46:44.962Z"" lastTimeUsedJournal=""2018-09-14T08:46:44.962Z"" lastTimeUsedContacts=""2018-09-14T08:46:44.962Z"" lastTimeUsedTasks=""2018-09-14T08:46:44.962Z"" lastTimeUsedCalendar=""2018-09-14T08:46:44.962Z"" lastTimeUsedMail=""2018-09-14T08:46:44.962Z"" lastTimeUsed=""2018-09-14T08:46:44.962Z"" guid=""{e71ddddb-855d-4c71-8475-896ead19a465}"" />
	<category usageCount=""5"" lastSessionUsed=""3"" name=""Blue Category"" color=""7"" keyboardShortcut=""0"" lastTimeUsedNotes=""2018-09-14T08:46:44.962Z"" lastTimeUsedJournal=""2018-09-14T08:46:44.962Z"" lastTimeUsedContacts=""2018-09-14T08:46:44.962Z"" lastTimeUsedTasks=""2018-09-14T08:46:44.962Z"" lastTimeUsedCalendar=""2018-09-14T08:46:44.962Z"" lastTimeUsedMail=""2018-09-14T08:46:44.962Z"" lastTimeUsed=""2018-09-14T08:46:44.962Z"" guid=""{0b19096e-c43b-4946-a5bd-a2a365e36946}"" renameOnFirstUse=""0"" />
	<category usageCount=""0"" lastSessionUsed=""0"" renameOnFirstUse=""1"" name=""Purple Category"" color=""8"" keyboardShortcut=""0"" lastTimeUsedNotes=""2018-09-14T08:46:44.962Z"" lastTimeUsedJournal=""2018-09-14T08:46:44.962Z"" lastTimeUsedContacts=""2018-09-14T08:46:44.962Z"" lastTimeUsedTasks=""2018-09-14T08:46:44.962Z"" lastTimeUsedCalendar=""2018-09-14T08:46:44.962Z"" lastTimeUsedMail=""2018-09-14T08:46:44.962Z"" lastTimeUsed=""2018-09-14T08:46:44.962Z"" guid=""{851abb56-981b-4d26-bfb5-550b4565a434}"" />
	<category name=""1-2-1s"" color=""4"" keyboardShortcut=""0"" usageCount=""7"" lastTimeUsedNotes=""2018-09-14T08:46:44.962Z"" lastTimeUsedJournal=""2018-09-14T08:46:44.962Z"" lastTimeUsedContacts=""2018-09-14T08:46:44.962Z"" lastTimeUsedTasks=""2018-09-14T08:46:44.962Z"" lastTimeUsedCalendar=""2020-10-14T12:48:38.399Z"" lastTimeUsedMail=""2018-09-14T08:46:44.962Z"" lastTimeUsed=""2020-10-14T12:48:38.399Z"" lastSessionUsed=""3"" guid=""{7d10025d-0f81-4591-8092-6fa97e506eec}"" renameOnFirstUse=""0"" />
	<category renameOnFirstUse=""0"" name=""Kimai"" color=""24"" keyboardShortcut=""0"" lastTimeUsedNotes=""2021-07-05T20:23:33.2025542Z"" lastTimeUsedJournal=""2021-07-05T20:23:33.2025542Z"" lastTimeUsedContacts=""2021-07-05T20:23:33.2025542Z"" lastTimeUsedTasks=""2021-07-05T20:23:33.2025542Z"" lastTimeUsedCalendar=""2021-07-05T20:23:33.2025542Z"" lastTimeUsedMail=""2021-07-05T20:23:33.2025542Z"" lastTimeUsed=""2021-07-05T20:23:33.2025542Z"" guid=""{d8a0ab8b-7b9e-46d7-ae00-d45bb61a4446}"" />
</categories>";

            Categories categories = null;
            // Act
            XmlSerializer serializer = new XmlSerializer(typeof(Categories));
            Action act = () =>
            {
                using (StringReader reader = new StringReader(xml))
                {
                    categories = (Categories)serializer.Deserialize(reader);
                }
            };
            
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            // Assert
            Assert.Equal("There is an error in XML document (1, 1).", exception.Message);
        }
    }
}

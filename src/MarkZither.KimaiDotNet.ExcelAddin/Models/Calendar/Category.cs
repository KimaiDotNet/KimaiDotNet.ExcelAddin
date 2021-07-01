using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar
{
    [XmlRoot(ElementName = "category")]
    public class Category
    {
        [XmlAttribute(AttributeName = "usageCount")]
        public int UsageCount { get; set; }

        [XmlAttribute(AttributeName = "lastSessionUsed")]
        public int LastSessionUsed { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "color")]
        public int Color { get; set; }

        [XmlAttribute(AttributeName = "keyboardShortcut")]
        public int KeyboardShortcut { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsedNotes")]
        public DateTime LastTimeUsedNotes { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsedJournal")]
        public DateTime LastTimeUsedJournal { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsedContacts")]
        public DateTime LastTimeUsedContacts { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsedTasks")]
        public DateTime LastTimeUsedTasks { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsedCalendar")]
        public DateTime LastTimeUsedCalendar { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsedMail")]
        public DateTime LastTimeUsedMail { get; set; }

        [XmlAttribute(AttributeName = "lastTimeUsed")]
        public DateTime LastTimeUsed { get; set; }

        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }

        [XmlAttribute(AttributeName = "renameOnFirstUse")]
        public int RenameOnFirstUse { get; set; }

        public int CustomerId { get; set; }
        public int ProjectId { get; set; }
        public int ActivityId { get; set; }
    }
}

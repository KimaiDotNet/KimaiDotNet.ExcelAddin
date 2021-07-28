using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar
{
    [XmlRoot(ElementName = "categories", Namespace = "CategoryList.xsd")]
    public class Categories
    {
        [XmlElement(ElementName = "category")]
        public List<Category> Category { get; set; }

        [XmlAttribute(AttributeName = "default")]
        public string Default { get; set; }

        [XmlAttribute(AttributeName = "lastSavedSession")]
        public int LastSavedSession { get; set; }

        [XmlAttribute(AttributeName = "lastSavedTime")]
        public DateTime LastSavedTime { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}

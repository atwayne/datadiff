using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskSource
    {
        private string connectionStringField;

        private string queryStringField;

        private string nameField;

        public string ConnectionString
        {
            get
            {
                return connectionStringField;
            }
            set
            {
                connectionStringField = value;
            }
        }

        public string QueryString
        {
            get
            {
                return queryStringField;
            }
            set
            {
                queryStringField = value;
            }
        }

        [XmlAttributeAttribute("name")]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }
    }
}
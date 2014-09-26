using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class TaskSource
    {
        private string connectionStringField;

        private string queryStringField;

        private string nameField;

        public string ConnectionString
        {
            get
            {
                return this.connectionStringField;
            }
            set
            {
                this.connectionStringField = value;
            }
        }

        public string QueryString
        {
            get
            {
                return this.queryStringField;
            }
            set
            {
                this.queryStringField = value;
            }
        }

        [XmlAttributeAttribute("name")]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
}
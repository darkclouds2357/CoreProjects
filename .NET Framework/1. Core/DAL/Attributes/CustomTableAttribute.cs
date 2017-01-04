using DAL.Services.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Attributes
{
    public class CustomTableAttribute : TableAttribute
    {
        public CustomTableAttribute(string tableName, string schema = "dbo")
            : base(tableName.ToLower())
        {
            this.Schema = schema;
        }
        public IConfigurationService ConfigurationService { get; set; }
    }

}

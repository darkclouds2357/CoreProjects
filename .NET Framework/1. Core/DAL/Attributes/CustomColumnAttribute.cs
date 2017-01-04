using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Attributes
{
    public class CustomColumnAttribute : ColumnAttribute
    {
        public CustomColumnAttribute(string name) : base(name)
        {
        }
    }
}

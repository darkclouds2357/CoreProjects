using FluentMigrator;

namespace DAL.Attributes
{
    public class CustomMigrationAttribute : MigrationAttribute
    {
        public string Connection { get; }

        public CustomMigrationAttribute(long version) : base(version)
        {
        }

        public CustomMigrationAttribute(long version, TransactionBehavior transactionBehavior) : base(version, transactionBehavior)
        {
        }

        public CustomMigrationAttribute(long version, string description) : base(version, description)
        {
        }

        public CustomMigrationAttribute(long version, string description, string Connection) : base(version, description)
        {
            this.Connection = Connection;
        }

        public CustomMigrationAttribute(long version, TransactionBehavior transactionBehavior, string description) : base(version, transactionBehavior, description)
        {
        }
    }

}

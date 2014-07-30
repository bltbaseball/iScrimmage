using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;

namespace iScrimmage.Migrations.Extensions
{
    public static class ColumnExtensions
    {
        public static ICreateTableWithColumnSyntax iScrimmageTable(this ICreateExpressionRoot root, string schema, string name)
        {
            return root.Table(name).InSchema(schema).WithBaseEntityId(name, "Id");
        }

        public static ICreateTableWithColumnSyntax iScrimmageTable(this ICreateExpressionRoot root, string name)
        {
            return root.Table(name).WithBaseEntityId(name, "Id");
        }

        public static ICreateTableWithColumnSyntax WithBaseEntityId(this ICreateTableWithColumnSyntax column, string tableName, string primaryIdName)
        {
            return column.WithColumn(primaryIdName).AsGuid().PrimaryKey("PK_" + tableName);
        }

        public static ICreateTableWithColumnSyntax WithAuditColumns(this ICreateTableWithColumnSyntax column)
        {
            return column.WithColumn("CreatedBy").AsGuid().NotNullable()
                .WithColumn("CreatedOn").AsDateTime().NotNullable()
                .WithColumn("ModifiedBy").AsGuid().Nullable()
                .WithColumn("ModifiedOn").AsDateTime().Nullable()
                .WithColumn("Archived").AsBoolean().NotNullable().WithDefaultValue(false)
                ;
        }
    }
}

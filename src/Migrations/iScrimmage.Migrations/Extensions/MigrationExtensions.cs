using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace iScrimmage.Migrations.Extensions
{
    public static class MigrationExtensions
    {
        public static void DropProcedure(this Migration migration, String procedureName)
        {
            var dropProcedure = String.Format(
                @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[blt].[{0}]') AND type in (N'P', N'PC'))
                DROP PROCEDURE [blt].[{0}]
                GO", procedureName);

            migration.Execute.Sql(dropProcedure);
        }
    }
}

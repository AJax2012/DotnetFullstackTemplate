using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceName.Data.Model.Migrations
{
    public partial class CreateLogRetrievalProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(dropStoredProc);
            migrationBuilder.Sql(storedProc);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(dropStoredProc);
        }

        private readonly string storedProc = @"
CREATE PROCEDURE [dbo].[p_GetLogs]
	@NumberOfLogs INT = 1000
AS
BEGIN
	SELECT TOP(@NumberOfLogs) * FROM [dbo].[Log] ORDER BY Id DESC
END
GO";
        private readonly string dropStoredProc = @"
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[p_GetLogs]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE [dbo].[p_GetLogs]
END";
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceName.Data.Model.Migrations
{
    public partial class UpdateUserRoleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleEntity_Roles_ApplicationRoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleEntity_Users_ApplicationUserId",
                table: "UserRoleEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleEntity_ApplicationRoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleEntity_ApplicationUserId_ApplicationRoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserRoleEntity");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "UserRoleEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UserRoleEntity",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleEntity_RoleId",
                table: "UserRoleEntity",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleEntity_UserId_RoleId",
                table: "UserRoleEntity",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleEntity_Roles_RoleId",
                table: "UserRoleEntity",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleEntity_Users_UserId",
                table: "UserRoleEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleEntity_Roles_RoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleEntity_Users_UserId",
                table: "UserRoleEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleEntity_RoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleEntity_UserId_RoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserRoleEntity");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserRoleEntity");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationRoleId",
                table: "UserRoleEntity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "UserRoleEntity",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleEntity_ApplicationRoleId",
                table: "UserRoleEntity",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleEntity_ApplicationUserId_ApplicationRoleId",
                table: "UserRoleEntity",
                columns: new[] { "ApplicationUserId", "ApplicationRoleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleEntity_Roles_ApplicationRoleId",
                table: "UserRoleEntity",
                column: "ApplicationRoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleEntity_Users_ApplicationUserId",
                table: "UserRoleEntity",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

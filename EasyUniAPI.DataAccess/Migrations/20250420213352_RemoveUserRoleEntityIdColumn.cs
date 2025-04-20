using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyUniAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRoleEntityIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "UserRoles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}

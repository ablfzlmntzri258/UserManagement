using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueConstraintForEmailAndUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "t_user",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "t_user",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_Email",
                table: "t_user",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_user_UserName",
                table: "t_user",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_t_user_Email",
                table: "t_user");

            migrationBuilder.DropIndex(
                name: "IX_t_user_UserName",
                table: "t_user");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "t_user",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "t_user",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}

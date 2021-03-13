using Microsoft.EntityFrameworkCore.Migrations;

namespace bachelor_work_backend.Migrations
{
    public partial class addIsDeletedToAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "BlockAction",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "BlockAction");
        }
    }
}

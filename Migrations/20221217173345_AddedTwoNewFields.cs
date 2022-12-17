using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo_List.Migrations
{
    public partial class AddedTwoNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeadLine",
                table: "ToDoLists",
                newName: "Deadline");

            migrationBuilder.AddColumn<int>(
                name: "Importance",
                table: "ToDoLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ToDoLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Importance",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ToDoLists");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "ToDoLists",
                newName: "DeadLine");
        }
    }
}

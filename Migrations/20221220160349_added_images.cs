using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo_List.Migrations
{
    public partial class added_images : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageModelId",
                table: "ToDoLists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_ImageModelId",
                table: "ToDoLists",
                column: "ImageModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoLists_Images_ImageModelId",
                table: "ToDoLists",
                column: "ImageModelId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoLists_Images_ImageModelId",
                table: "ToDoLists");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_ToDoLists_ImageModelId",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "ImageModelId",
                table: "ToDoLists");
        }
    }
}

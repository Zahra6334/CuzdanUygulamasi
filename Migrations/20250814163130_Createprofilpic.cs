using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuzdanUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class Createprofilpic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfiPic",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfiPic",
                table: "Kullanicilar");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuzdanUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class AddBildirimTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IslemId",
                table: "OdemeTaksitleri",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OdendiMi",
                table: "OdemeTaksitleri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SonOdemeTarihi",
                table: "OdemeTaksitleri",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OdemeTaksitleri_IslemId",
                table: "OdemeTaksitleri",
                column: "IslemId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciId",
                table: "Bildirimler",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_OdemeTaksitleri_Islemler_IslemId",
                table: "OdemeTaksitleri",
                column: "IslemId",
                principalTable: "Islemler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OdemeTaksitleri_Islemler_IslemId",
                table: "OdemeTaksitleri");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropIndex(
                name: "IX_OdemeTaksitleri_IslemId",
                table: "OdemeTaksitleri");

            migrationBuilder.DropColumn(
                name: "IslemId",
                table: "OdemeTaksitleri");

            migrationBuilder.DropColumn(
                name: "OdendiMi",
                table: "OdemeTaksitleri");

            migrationBuilder.DropColumn(
                name: "SonOdemeTarihi",
                table: "OdemeTaksitleri");
        }
    }
}

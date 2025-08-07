using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuzdanUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategoriler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaksitliOdemeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaksitSayisi = table.Column<int>(type: "int", nullable: false),
                    FaizOrani = table.Column<float>(type: "real", nullable: false),
                    KalanTaksit = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaksitliOdemeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaksitliOdemeler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemTipi = table.Column<int>(type: "int", nullable: false),
                    TekrarliMi = table.Column<bool>(type: "bit", nullable: false),
                    Periyot = table.Column<int>(type: "int", nullable: true),
                    TaksitOdemeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Islemler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Islemler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Islemler_TaksitliOdemeler_TaksitOdemeId",
                        column: x => x.TaksitOdemeId,
                        principalTable: "TaksitliOdemeler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_KategoriId",
                table: "Islemler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_KullaniciId",
                table: "Islemler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_TaksitOdemeId",
                table: "Islemler",
                column: "TaksitOdemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_KullaniciId",
                table: "Kategoriler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_TaksitliOdemeler_KullaniciId",
                table: "TaksitliOdemeler",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Islemler");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "TaksitliOdemeler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}

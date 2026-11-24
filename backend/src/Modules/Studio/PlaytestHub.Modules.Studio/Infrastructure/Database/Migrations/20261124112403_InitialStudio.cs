using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaytestHub.Modules.Studio.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialStudio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "studio");

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "studio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Genre = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Platforms = table.Column<int>(type: "integer", nullable: false),
                    MinimumSpecs = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameMembers",
                schema: "studio",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMembers", x => new { x.GameId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GameMembers_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "studio",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameMembers_UserId",
                schema: "studio",
                table: "GameMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OwnerId",
                schema: "studio",
                table: "Games",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameMembers",
                schema: "studio");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "studio");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IcMarkets.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blockchains",
                columns: table => new
                {
                    Pk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LatestUrl = table.Column<string>(type: "text", nullable: false),
                    PreviousHash = table.Column<string>(type: "text", nullable: false),
                    PreviousUrl = table.Column<string>(type: "text", nullable: false),
                    PeerCount = table.Column<int>(type: "integer", nullable: true),
                    HighFeePerKb = table.Column<int>(type: "integer", nullable: false),
                    MediumFeePerKb = table.Column<int>(type: "integer", nullable: false),
                    LowFeePerKb = table.Column<int>(type: "integer", nullable: false),
                    UnconfirmedCount = table.Column<int>(type: "integer", nullable: false),
                    LastForkHeight = table.Column<int>(type: "integer", nullable: true),
                    LastForkHash = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blockchains", x => x.Pk);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blockchains");
        }
    }
}

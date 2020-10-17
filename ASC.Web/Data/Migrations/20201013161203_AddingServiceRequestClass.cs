using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Web.Data.Migrations
{
    public partial class AddingServiceRequestClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    VehicleName = table.Column<string>(nullable: true),
                    VehicleType = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    RequestedServices = table.Column<string>(nullable: true),
                    RequestedDate = table.Column<DateTime>(nullable: true),
                    CompletedDate = table.Column<DateTime>(nullable: true),
                    ServiceEngineer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceRequests");
        }
    }
}

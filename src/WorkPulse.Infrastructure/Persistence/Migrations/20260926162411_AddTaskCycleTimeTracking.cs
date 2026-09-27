using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkPulse.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskCycleTimeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at_utc",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "started_at_utc",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_tenant_id_sprint_id_completed_at_utc",
                table: "tasks",
                columns: new[] { "tenant_id", "sprint_id", "completed_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_tasks_tenant_id_sprint_id_completed_at_utc",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "completed_at_utc",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "started_at_utc",
                table: "tasks");
        }
    }
}

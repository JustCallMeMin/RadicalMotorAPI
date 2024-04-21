using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadicalMotorAPI.Migrations
{
    /// <inheritdoc />
    public partial class initials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AddColumn<string>(
							name: "Notes",
							table: "AppointmentDetails",
							type: "nvarchar(max)",
							nullable: false,
							defaultValue: "");

			migrationBuilder.AddColumn<DateTime>(
				name: "ServiceDate",
				table: "AppointmentDetails",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.DropColumn(
				name: "ServiceAmount",
				table: "AppointmentDetails");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

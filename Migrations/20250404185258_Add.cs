using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseShopOnline.Migrations
{
    /// <inheritdoc />
    public partial class Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnrolled",
                table: "Enrollments");

            migrationBuilder.AddColumn<int>(
                name: "EnollingStatus",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnollingStatus",
                table: "Enrollments");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnrolled",
                table: "Enrollments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

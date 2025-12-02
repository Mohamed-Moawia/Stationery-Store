// StationeryStore.Infrastructure/Data/Migrations/20240101000001_AddEgyptianVAT.cs
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationeryStore.Infrastructure.Data.Migrations;

public partial class AddEgyptianVAT : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create schema for Egyptian tax
        migrationBuilder.EnsureSchema(
            name: "tax");

        migrationBuilder.EnsureSchema(
            name: "eta");

        // Create Egyptian VAT table
        migrationBuilder.CreateTable(
            name: "vat_rates",
            schema: "catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name_ar = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "arabic_ci"),
                name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                eta_tax_type_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<string>(type: "text", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_vat_rates", x => x.id);
            });

        // Insert Egyptian VAT rates
        migrationBuilder.Sql(@"
            INSERT INTO catalog.vat_rates (id, name_ar, name_en, rate, eta_tax_type_code, is_active, created_at, created_by)
            VALUES 
            (gen_random_uuid(), 'ضريبة القيمة المضافة القياسية', 'Standard VAT', 14.00, 'T1', true, NOW(), 'SYSTEM'),
            (gen_random_uuid(), 'معفى', 'Exempt', 0.00, 'T2', true, NOW(), 'SYSTEM'),
            (gen_random_uuid(), 'صفر', 'Zero Rated', 0.00, 'T3', true, NOW(), 'SYSTEM'),
            (gen_random_uuid(), 'خاص', 'Special Rate', 5.00, 'T4', true, NOW(), 'SYSTEM');
        ");

        // Add VAT rate to products
        migrationBuilder.AddColumn<Guid>(
            name: "vat_rate_id",
            schema: "catalog",
            table: "products",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000001")); // Default to standard VAT

        migrationBuilder.CreateIndex(
            name: "ix_products_vat_rate_id",
            schema: "catalog",
            table: "products",
            column: "vat_rate_id");

        migrationBuilder.AddForeignKey(
            name: "fk_products_vat_rates_vat_rate_id",
            schema: "catalog",
            table: "products",
            column: "vat_rate_id",
            principalSchema: "catalog",
            principalTable: "vat_rates",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        // Create Egyptian branches table
        migrationBuilder.CreateTable(
            name: "egyptian_branches",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                tax_office_ar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, collation: "arabic_ci"),
                tax_office_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                tax_office_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                chamber_of_commerce_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_egyptian_branches", x => x.id);
                table.ForeignKey(
                    name: "fk_egyptian_branches_branches_branch_id",
                    column: x => x.branch_id,
                    principalSchema: "public",
                    principalTable: "branches",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_egyptian_branches_branch_id",
            schema: "public",
            table: "egyptian_branches",
            column: "branch_id",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse the changes
        migrationBuilder.DropForeignKey(
            name: "fk_products_vat_rates_vat_rate_id",
            schema: "catalog",
            table: "products");

        migrationBuilder.DropTable(
            name: "egyptian_branches",
            schema: "public");

        migrationBuilder.DropTable(
            name: "vat_rates",
            schema: "catalog");

        migrationBuilder.DropIndex(
            name: "ix_products_vat_rate_id",
            schema: "catalog",
            table: "products");

        migrationBuilder.DropColumn(
            name: "vat_rate_id",
            schema: "catalog",
            table: "products");
    }
}
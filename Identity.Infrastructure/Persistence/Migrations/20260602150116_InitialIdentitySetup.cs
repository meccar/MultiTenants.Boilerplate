using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "taf_base");

            migrationBuilder.CreateTable(
                name: "idn_groups",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idn_permissions",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Resource = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idn_policies",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idn_roles",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idn_users",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idn_group_policies",
                schema: "taf_base",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_group_policies", x => new { x.GroupId, x.PolicyId });
                    table.ForeignKey(
                        name: "FK_idn_group_policies_idn_groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "taf_base",
                        principalTable: "idn_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_group_policies_idn_policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "taf_base",
                        principalTable: "idn_policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_policy_permissions",
                schema: "taf_base",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Effect = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Conditions = table.Column<JsonDocument>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_policy_permissions", x => new { x.PolicyId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_idn_policy_permissions_idn_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "taf_base",
                        principalTable: "idn_permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_policy_permissions_idn_policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "taf_base",
                        principalTable: "idn_policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_group_roles",
                schema: "taf_base",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_group_roles", x => new { x.GroupId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_idn_group_roles_idn_groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "taf_base",
                        principalTable: "idn_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_group_roles_idn_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "taf_base",
                        principalTable: "idn_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_role_claims",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_idn_role_claims_idn_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "taf_base",
                        principalTable: "idn_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_role_permissions",
                schema: "taf_base",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_role_permissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_idn_role_permissions_idn_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "taf_base",
                        principalTable: "idn_permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_role_permissions_idn_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "taf_base",
                        principalTable: "idn_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_role_policies",
                schema: "taf_base",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_role_policies", x => new { x.RoleId, x.PolicyId });
                    table.ForeignKey(
                        name: "FK_idn_role_policies_idn_policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "taf_base",
                        principalTable: "idn_policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_role_policies_idn_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "taf_base",
                        principalTable: "idn_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_user_claims",
                schema: "taf_base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_idn_user_claims_idn_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taf_base",
                        principalTable: "idn_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_user_groups",
                schema: "taf_base",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_user_groups", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_idn_user_groups_idn_groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "taf_base",
                        principalTable: "idn_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_user_groups_idn_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taf_base",
                        principalTable: "idn_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_user_logins",
                schema: "taf_base",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_idn_user_logins_idn_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taf_base",
                        principalTable: "idn_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_user_policies",
                schema: "taf_base",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_user_policies", x => new { x.UserId, x.PolicyId });
                    table.ForeignKey(
                        name: "FK_idn_user_policies_idn_policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "taf_base",
                        principalTable: "idn_policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_user_policies_idn_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taf_base",
                        principalTable: "idn_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_user_roles",
                schema: "taf_base",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_idn_user_roles_idn_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "taf_base",
                        principalTable: "idn_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idn_user_roles_idn_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taf_base",
                        principalTable: "idn_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idn_user_tokens",
                schema: "taf_base",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idn_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_idn_user_tokens_idn_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taf_base",
                        principalTable: "idn_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_idn_group_policies_PolicyId",
                schema: "taf_base",
                table: "idn_group_policies",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_group_roles_RoleId",
                schema: "taf_base",
                table: "idn_group_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_policy_permissions_PermissionId",
                schema: "taf_base",
                table: "idn_policy_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_role_claims_RoleId",
                schema: "taf_base",
                table: "idn_role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_role_permissions_PermissionId",
                schema: "taf_base",
                table: "idn_role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_role_policies_PolicyId",
                schema: "taf_base",
                table: "idn_role_policies",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "taf_base",
                table: "idn_roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_idn_user_claims_UserId",
                schema: "taf_base",
                table: "idn_user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_user_groups_GroupId",
                schema: "taf_base",
                table: "idn_user_groups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_user_logins_UserId",
                schema: "taf_base",
                table: "idn_user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_user_policies_PolicyId",
                schema: "taf_base",
                table: "idn_user_policies",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_idn_user_roles_RoleId",
                schema: "taf_base",
                table: "idn_user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "taf_base",
                table: "idn_users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "taf_base",
                table: "idn_users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idn_group_policies",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_group_roles",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_policy_permissions",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_role_claims",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_role_permissions",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_role_policies",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_user_claims",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_user_groups",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_user_logins",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_user_policies",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_user_roles",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_user_tokens",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_permissions",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_groups",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_policies",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_roles",
                schema: "taf_base");

            migrationBuilder.DropTable(
                name: "idn_users",
                schema: "taf_base");
        }
    }
}

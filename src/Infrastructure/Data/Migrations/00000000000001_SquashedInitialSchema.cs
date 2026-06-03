using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SquashedInitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SubscriptionTier = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cookbook",
                columns: table => new
                {
                    cookbook_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cookbook_id", x => x.cookbook_id);
                    table.ForeignKey(
                        name: "FK_cookbook__created_by",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cookbook_invitation",
                columns: table => new
                {
                    cookbook_invitation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_person_id = table.Column<string>(type: "text", nullable: true),
                    SenderDisplayName = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    invitation_status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cookbook_id = table.Column<Guid>(type: "uuid", nullable: false),
                    response_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cookbook_invitation_id", x => x.cookbook_invitation_id);
                    table.ForeignKey(
                        name: "FK_cookbook_invitation__cookbook_id",
                        column: x => x.cookbook_id,
                        principalTable: "cookbook",
                        principalColumn: "cookbook_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cookbook_invitation__created_by",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cookbook_invitation__recipient_person_id",
                        column: x => x.recipient_person_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cookbook_member",
                columns: table => new
                {
                    cookbook_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cookbook_id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    tier = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cookbook_member_id", x => x.cookbook_member_id);
                    table.ForeignKey(
                        name: "FK_cookbook_member__cookbook_id",
                        column: x => x.cookbook_id,
                        principalTable: "cookbook",
                        principalColumn: "cookbook_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cookbook_member__created_by",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitation_token",
                columns: table => new
                {
                    invitation_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    redeemer_person_id = table.Column<string>(type: "text", nullable: true),
                    token_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    token_salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    invitation_status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cookbook_id = table.Column<Guid>(type: "uuid", nullable: false),
                    response_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("invitation_token_id", x => x.invitation_token_id);
                    table.ForeignKey(
                        name: "FK_invitation_token__cookbook_id",
                        column: x => x.cookbook_id,
                        principalTable: "cookbook",
                        principalColumn: "cookbook_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invitation_token__created_by",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "recipe",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cookbook_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AuthorDisplayName = table.Column<string>(type: "text", nullable: true),
                    summary = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    thumbnail = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    video_path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Servings = table.Column<int>(type: "integer", nullable: true),
                    made_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    preparation_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    cooking_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    baking_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    IsVegetarian = table.Column<bool>(type: "boolean", nullable: true),
                    IsVegan = table.Column<bool>(type: "boolean", nullable: true),
                    IsGlutenFree = table.Column<bool>(type: "boolean", nullable: true),
                    IsDairyFree = table.Column<bool>(type: "boolean", nullable: true),
                    IsHealthy = table.Column<bool>(type: "boolean", nullable: true),
                    IsCheap = table.Column<bool>(type: "boolean", nullable: true),
                    IsLowFodmap = table.Column<bool>(type: "boolean", nullable: true),
                    IsHighProtein = table.Column<bool>(type: "boolean", nullable: true),
                    IsBreakfast = table.Column<bool>(type: "boolean", nullable: true),
                    IsLunch = table.Column<bool>(type: "boolean", nullable: true),
                    IsDinner = table.Column<bool>(type: "boolean", nullable: true),
                    IsDessert = table.Column<bool>(type: "boolean", nullable: true),
                    IsSnack = table.Column<bool>(type: "boolean", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    directions = table.Column<string>(type: "jsonb", nullable: true),
                    images = table.Column<string>(type: "jsonb", nullable: true),
                    ingredient_sections = table.Column<string>(type: "jsonb", nullable: true),
                    nutrition = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_id", x => x.recipe_id);
                    table.ForeignKey(
                        name: "FK_recipe__cookbook_id",
                        column: x => x.cookbook_id,
                        principalTable: "cookbook",
                        principalColumn: "cookbook_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe__created_by",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cookbook_notification",
                columns: table => new
                {
                    cookbook_notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_user_id = table.Column<string>(type: "text", nullable: false),
                    cookbook_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    actor_user_id = table.Column<string>(type: "text", nullable: true),
                    subject_user_id = table.Column<string>(type: "text", nullable: true),
                    ActorDisplayName = table.Column<string>(type: "text", nullable: true),
                    SubjectDisplayName = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cookbook_notification_id", x => x.cookbook_notification_id);
                    table.ForeignKey(
                        name: "FK_cookbook_notification__actor_user_id",
                        column: x => x.actor_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cookbook_notification__cookbook_id",
                        column: x => x.cookbook_id,
                        principalTable: "cookbook",
                        principalColumn: "cookbook_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cookbook_notification__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cookbook_notification__recipient_user_id",
                        column: x => x.recipient_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cookbook_notification__subject_user_id",
                        column: x => x.subject_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_category",
                columns: table => new
                {
                    ingredient_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_category_id", x => x.ingredient_category_id);
                    table.ForeignKey(
                        name: "FK_ingredient_category__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_creator__created_by",
                table: "cookbook",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_invitation__cookbook_id",
                table: "cookbook_invitation",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_invitation__created_by",
                table: "cookbook_invitation",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_invitation__recipient_person_id",
                table: "cookbook_invitation",
                column: "recipient_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_member__cookbook_id",
                table: "cookbook_member",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_member__created_by",
                table: "cookbook_member",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_cookbook_member__cookbook_user",
                table: "cookbook_member",
                columns: new[] { "cookbook_id", "CreatedBy" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__cookbook_id",
                table: "cookbook_notification",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipe_id",
                table: "cookbook_notification",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipient_created",
                table: "cookbook_notification",
                columns: new[] { "recipient_user_id", "created" });

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipient_user_id",
                table: "cookbook_notification",
                column: "recipient_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification_actor_user_id",
                table: "cookbook_notification",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification_subject_user_id",
                table: "cookbook_notification",
                column: "subject_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient__category_recipe_id",
                table: "ingredient_category",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token",
                column: "public_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token_cookbook_id",
                table: "invitation_token",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token_CreatedBy",
                table: "invitation_token",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_recipe__cookbook_id",
                table: "recipe",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_CreatedBy",
                table: "recipe",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "cookbook_invitation");

            migrationBuilder.DropTable(
                name: "cookbook_member");

            migrationBuilder.DropTable(
                name: "cookbook_notification");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "ingredient_category");

            migrationBuilder.DropTable(
                name: "invitation_token");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "recipe");

            migrationBuilder.DropTable(
                name: "cookbook");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}

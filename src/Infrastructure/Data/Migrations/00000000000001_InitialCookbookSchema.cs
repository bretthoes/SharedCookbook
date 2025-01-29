using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCookbookSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "TodoLists");

            migrationBuilder.CreateTable(
                name: "cookbook",
                columns: table => new
                {
                    cookbook_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    cookbook_invitation_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cookbook_id = table.Column<int>(type: "integer", nullable: false),
                    recipient_person_id = table.Column<string>(type: "text", nullable: true),
                    invitation_status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    response_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
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
                    cookbook_member_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cookbook_id = table.Column<int>(type: "integer", nullable: false),
                    is_creator = table.Column<bool>(type: "boolean", nullable: false),
                    can_add_recipe = table.Column<bool>(type: "boolean", nullable: false),
                    can_update_recipe = table.Column<bool>(type: "boolean", nullable: false),
                    can_delete_recipe = table.Column<bool>(type: "boolean", nullable: false),
                    can_send_invite = table.Column<bool>(type: "boolean", nullable: false),
                    can_remove_member = table.Column<bool>(type: "boolean", nullable: false),
                    can_edit_cookbook_details = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "recipe",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cookbook_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    summary = table.Column<string>(type: "text", nullable: true),
                    thumbnail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    video_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    preparation_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    cooking_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    baking_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    Servings = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
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
                    cookbook_notification_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cookbook_id = table.Column<int>(type: "integer", nullable: true),
                    recipe_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cookbook_notification_id", x => x.cookbook_notification_id);
                    table.ForeignKey(
                        name: "FK_cookbook_notification__cookbook_id",
                        column: x => x.cookbook_id,
                        principalTable: "cookbook",
                        principalColumn: "cookbook_id");
                    table.ForeignKey(
                        name: "FK_cookbook_notification__created_by",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cookbook_notification__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id");
                });

            migrationBuilder.CreateTable(
                name: "ingredient_category",
                columns: table => new
                {
                    ingredient_category_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "recipe_direction",
                columns: table => new
                {
                    recipe_direction_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_direction_id", x => x.recipe_direction_id);
                    table.ForeignKey(
                        name: "FK_recipe_direction__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_image",
                columns: table => new
                {
                    recipe_image_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_image_id", x => x.recipe_image_id);
                    table.ForeignKey(
                        name: "FK_recipe_image__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredient",
                columns: table => new
                {
                    recipe_ingredient_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    optional = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredient_id", x => x.recipe_ingredient_id);
                    table.ForeignKey(
                        name: "FK_recipe_ingredient__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_nutrition",
                columns: table => new
                {
                    recipe_nutrition_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    calories = table.Column<int>(type: "integer", nullable: true),
                    protein = table.Column<int>(type: "integer", nullable: true),
                    fat = table.Column<int>(type: "integer", nullable: true),
                    carbohydrates = table.Column<int>(type: "integer", nullable: true),
                    sugar = table.Column<int>(type: "integer", nullable: true),
                    fiber = table.Column<int>(type: "integer", nullable: true),
                    sodium = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_nutrition_id", x => x.recipe_nutrition_id);
                    table.ForeignKey(
                        name: "FK_recipe_nutrition__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_cookbook_notification__cookbook_id",
                table: "cookbook_notification",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__created_by",
                table: "cookbook_notification",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipe_id",
                table: "cookbook_notification",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient__category_recipe_id",
                table: "ingredient_category",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe__cookbook_id",
                table: "recipe",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_CreatedBy",
                table: "recipe",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_direction__recipe_id",
                table: "recipe_direction",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_image__recipe_id",
                table: "recipe_image",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredient__recipe_id",
                table: "recipe_ingredient",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_nutrition__recipe_id",
                table: "recipe_nutrition",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_nutrition_recipe_id",
                table: "recipe_nutrition",
                column: "recipe_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cookbook_invitation");

            migrationBuilder.DropTable(
                name: "cookbook_member");

            migrationBuilder.DropTable(
                name: "cookbook_notification");

            migrationBuilder.DropTable(
                name: "ingredient_category");

            migrationBuilder.DropTable(
                name: "recipe_direction");

            migrationBuilder.DropTable(
                name: "recipe_image");

            migrationBuilder.DropTable(
                name: "recipe_ingredient");

            migrationBuilder.DropTable(
                name: "recipe_nutrition");

            migrationBuilder.DropTable(
                name: "recipe");

            migrationBuilder.DropTable(
                name: "cookbook");

            migrationBuilder.CreateTable(
                name: "TodoLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Colour_Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ListId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Done = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Reminder = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_TodoLists_ListId",
                        column: x => x.ListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ListId",
                table: "TodoItems",
                column: "ListId");
        }
    }
}

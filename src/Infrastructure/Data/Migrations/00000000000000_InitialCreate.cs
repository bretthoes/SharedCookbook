using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Colour_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Reminder = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Done = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ListId",
                table: "TodoItems",
                column: "ListId");

            migrationBuilder.CreateTable(
                name: "cookbook",
                columns: table => new
                {
                    cookbook_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    creator_person_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cookbook_id", x => x.cookbook_id);
                    table.ForeignKey(
                        name: "FK_cookbook__creator_person_id",
                        column: x => x.creator_person_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cookbook_invitation",
                columns: table => new
                {
                    cookbook_invitation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cookbook_id = table.Column<int>(type: "int", nullable: false),
                    sender_person_id = table.Column<int>(type: "int", nullable: true),
                    recipient_person_id = table.Column<int>(type: "int", nullable: true),
                    invitation_status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    response_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_cookbook_invitation__recipient_person_id",
                        column: x => x.recipient_person_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cookbook_invitation__sender_person_id",
                        column: x => x.sender_person_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cookbook_member",
                columns: table => new
                {
                    cookbook_member_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    cookbook_id = table.Column<int>(type: "int", nullable: false),
                    is_creator = table.Column<bool>(type: "bit", nullable: false),
                    can_add_recipe = table.Column<bool>(type: "bit", nullable: false),
                    can_update_recipe = table.Column<bool>(type: "bit", nullable: false),
                    can_delete_recipe = table.Column<bool>(type: "bit", nullable: false),
                    can_send_invite = table.Column<bool>(type: "bit", nullable: false),
                    can_remove_member = table.Column<bool>(type: "bit", nullable: false),
                    can_edit_cookbook_details = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_cookbook_member__person_id",
                        column: x => x.person_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cookbook_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thumbnail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    video_path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    preparation_time_in_minutes = table.Column<int>(type: "int", nullable: true),
                    cooking_time_in_minutes = table.Column<int>(type: "int", nullable: true),
                    baking_time_in_minutes = table.Column<int>(type: "int", nullable: true),
                    Servings = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_recipe__author_id",
                        column: x => x.author_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cookbook_notification",
                columns: table => new
                {
                    cookbook_notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_person_id = table.Column<int>(type: "int", nullable: true),
                    cookbook_id = table.Column<int>(type: "int", nullable: true),
                    recipe_id = table.Column<int>(type: "int", nullable: true),
                    action_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_cookbook_notification__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id");
                    table.ForeignKey(
                        name: "FK_cookbook_notification__sender_person_id",
                        column: x => x.sender_person_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ingredient_category",
                columns: table => new
                {
                    ingredient_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    recipe_direction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ordinal = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "recipe_ingredient",
                columns: table => new
                {
                    recipe_ingredient_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ordinal = table.Column<int>(type: "int", nullable: false),
                    optional = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "recipe_image",
                columns: table => new
                {
                    recipe_image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ordinal = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "recipe_nutrition",
                columns: table => new
                {
                    recipe_nutrition_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    calories = table.Column<int>(type: "int", nullable: true),
                    protein = table.Column<int>(type: "int", nullable: true),
                    fat = table.Column<int>(type: "int", nullable: true),
                    carbohydrates = table.Column<int>(type: "int", nullable: true),
                    sugar = table.Column<int>(type: "int", nullable: true),
                    fiber = table.Column<int>(type: "int", nullable: true),
                    sodium = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_cookbook_creator__person_id",
                table: "cookbook",
                column: "creator_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_invitation__cookbook_id",
                table: "cookbook_invitation",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_invitation_recipient__person_id",
                table: "cookbook_invitation",
                column: "recipient_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_invitation_sender__person_id",
                table: "cookbook_invitation",
                column: "sender_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_member__cookbook_id",
                table: "cookbook_member",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_member__person_id",
                table: "cookbook_member",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__cookbook_id",
                table: "cookbook_notification",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipe_id",
                table: "cookbook_notification",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__sender_person_id",
                table: "cookbook_notification",
                column: "sender_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient__category_recipe_id",
                table: "ingredient_category",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe__cookbook_id",
                table: "recipe",
                column: "cookbook_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_author_id",
                table: "recipe",
                column: "author_id");

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
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TodoLists");

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
                name: "recipe_ingredient");

            migrationBuilder.DropTable(
                name: "recipe_image");

            migrationBuilder.DropTable(
                name: "recipe_nutrition");

            migrationBuilder.DropTable(
                name: "recipe");

            migrationBuilder.DropTable(
                name: "cookbook");
        }
    }
}

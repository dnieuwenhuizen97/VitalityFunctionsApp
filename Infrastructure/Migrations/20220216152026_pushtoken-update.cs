using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class pushtokenupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_ActivityCategory_CategoryId",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscribedChallenge_Activity_ActivityId",
                table: "SubscribedChallenge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityCategory",
                table: "ActivityCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activity",
                table: "Activity");

            migrationBuilder.RenameTable(
                name: "ActivityCategory",
                newName: "ActivityCategories");

            migrationBuilder.RenameTable(
                name: "Activity",
                newName: "Activities");

            migrationBuilder.RenameIndex(
                name: "IX_Activity_CategoryId",
                table: "Activities",
                newName: "IX_Activities_CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "PushTokens",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityCategories",
                table: "ActivityCategories",
                column: "CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activities",
                table: "Activities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_PushTokens_UserId",
                table: "PushTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityCategories_CategoryId",
                table: "Activities",
                column: "CategoryId",
                principalTable: "ActivityCategories",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PushTokens_Users_UserId",
                table: "PushTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribedChallenge_Activities_ActivityId",
                table: "SubscribedChallenge",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityCategories_CategoryId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_PushTokens_Users_UserId",
                table: "PushTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscribedChallenge_Activities_ActivityId",
                table: "SubscribedChallenge");

            migrationBuilder.DropIndex(
                name: "IX_PushTokens_UserId",
                table: "PushTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityCategories",
                table: "ActivityCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activities",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "PushTokens");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "ActivityCategories",
                newName: "ActivityCategory");

            migrationBuilder.RenameTable(
                name: "Activities",
                newName: "Activity");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_CategoryId",
                table: "Activity",
                newName: "IX_Activity_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityCategory",
                table: "ActivityCategory",
                column: "CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activity",
                table: "Activity",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_ActivityCategory_CategoryId",
                table: "Activity",
                column: "CategoryId",
                principalTable: "ActivityCategory",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribedChallenge_Activity_ActivityId",
                table: "SubscribedChallenge",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

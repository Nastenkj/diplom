using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Services.HashService;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateOfChangeOfAccessRights = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleApiClaims",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleApiClaims", x => new { x.ApiClaimId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RoleApiClaims_ApiClaims_ApiClaimId",
                        column: x => x.ApiClaimId,
                        principalTable: "ApiClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleApiClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DailySurveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AbdomenHurts = table.Column<bool>(type: "bit", nullable: false),
                    BloodDischarge = table.Column<bool>(type: "bit", nullable: false),
                    Nausea = table.Column<bool>(type: "bit", nullable: false),
                    UrgeToPuke = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<int>(type: "int", nullable: false),
                    BloodPressure = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HeartRate = table.Column<int>(type: "int", nullable: false),
                    GlucoseLevel = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Uro = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Bld = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Bil = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Ket = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Leu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Glu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pro = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Ph = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Nit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Sg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BloodCoagulation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OxygenLevel = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreationDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySurveys", x => x.Id);
                    table.CheckConstraint("CK_DailySurvey_BloodPressure", "[BloodPressure] LIKE '[0-9][0-9]/[0-9][0-9]' OR [BloodPressure] LIKE '[0-9][0-9][0-9]/[0-9][0-9]' OR [BloodPressure] LIKE '[0-9][0-9]/[0-9][0-9][0-9]' OR [BloodPressure] LIKE '[0-9][0-9][0-9]/[0-9][0-9][0-9]'");
                    table.ForeignKey(
                        name: "FK_DailySurveys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    BloodGroup = table.Column<int>(type: "int", nullable: false),
                    RhesusFactor = table.Column<int>(type: "int", nullable: false),
                    BloodPressure = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tonometer = table.Column<int>(type: "int", nullable: false),
                    Thermometer = table.Column<int>(type: "int", nullable: false),
                    PregnancyAmount = table.Column<int>(type: "int", nullable: false),
                    AbortionAmount = table.Column<int>(type: "int", nullable: true),
                    MiscarriageAmount = table.Column<int>(type: "int", nullable: true),
                    PrematureBirthAmount = table.Column<int>(type: "int", nullable: true),
                    PreviousBirthType = table.Column<int>(type: "int", nullable: false),
                    GynecologicalDiseases = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SomaticDiseases = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UndergoneOperations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AllergicReactions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HereditaryDiseases = table.Column<int>(type: "int", nullable: false),
                    IsSmoking = table.Column<bool>(type: "bit", nullable: false),
                    IsConsumingAlcohol = table.Column<bool>(type: "bit", nullable: false),
                    EnduredCovid = table.Column<int>(type: "int", nullable: false),
                    CreationDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientCommonInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrustedPersonFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrustedPersonPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrustedPersonEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCommonInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientCommonInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeeklySurveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasOrvi = table.Column<bool>(type: "bit", nullable: false),
                    OrviSymptoms = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HasUnordinaryTemp = table.Column<bool>(type: "bit", nullable: false),
                    UnordinaryTempOccurrences = table.Column<int>(type: "int", nullable: true),
                    UnordinaryBloodPressure = table.Column<int>(type: "int", nullable: false),
                    HasGynecologicalSymptoms = table.Column<bool>(type: "bit", nullable: false),
                    GynecologicalSymptoms = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HasUnordinaryUrine = table.Column<bool>(type: "bit", nullable: false),
                    HasSwelling = table.Column<bool>(type: "bit", nullable: false),
                    SwellingDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WaterConsumed = table.Column<int>(type: "int", nullable: false),
                    Stool = table.Column<int>(type: "int", nullable: false),
                    Urination = table.Column<int>(type: "int", nullable: false),
                    WeightAdded = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklySurveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklySurveys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailySurveys_UserId",
                table: "DailySurveys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_UserId",
                table: "MedicalHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCommonInfos_UserId",
                table: "PatientCommonInfos",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleApiClaims_RoleId",
                table: "RoleApiClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklySurveys_UserId",
                table: "WeeklySurveys",
                column: "UserId");
            
            var administratorRoleId = Guid.Parse("A987303B-BEB1-4ABE-8FCB-3E5C35D33C1C");
            var clientRoleId = Guid.Parse("59A687D0-7D05-4988-B04D-0807C2A57E24");
            var doctorRoleId = Guid.Parse("39B15202-0123-48E1-9BD4-7015D1DF09F9");
            var testClaimId = Guid.Parse("E3B3DF09-413A-4D75-9D60-A164157BDF7C");
            var dummyUserId = Guid.Parse("F378D665-798B-4A31-82A3-6645122DE60B");

            // Create new claim IDs
            var getMedicalHistoryId = Guid.Parse("D1A05D58-3249-4E4F-A6E4-41A717FDB88C");
            var postMedicalHistoryId = Guid.Parse("D621F9B4-9AB5-4F59-9E78-87E43C5E4B6B");
            var getLatestDailySurveyCreationDateUtcId = Guid.Parse("9E6B3F06-1B6B-4C53-A498-C324338F7C7C");
            var postDailySurveyId = Guid.Parse("BF14B3A4-D948-45EF-B6A1-88D844A17E7A");
            var getLatestWeeklySurveyId = Guid.Parse("C3F3F2B9-0D11-4C2E-8E1A-334579BC5283");
            var postWeeklySurveyId = Guid.Parse("B78F7789-2887-4C7C-8065-DC82482A0F6C");
            var getAllPatientsId = Guid.Parse("3D26CFA9-5E4A-46FF-9C2C-24F01C5A8A18");
            var getPatientId = Guid.Parse("7D5B7D72-9C2E-4D7A-8A4A-E537C23D56B7");
            var editPatientId = Guid.Parse("DC22FDA5-148E-485F-A20E-EA02C3983CE9");
            
            var getWeeklySurveysForUser = Guid.Parse("3B69E456-8E7B-4015-B5F4-E7CECD5BC5F4");
            var getWeeklySurveyById = Guid.Parse("7A51F702-B1FB-43EB-864D-13E6345ED995");
            var getDailySurveysForUser = Guid.Parse("A927AAD7-0EF2-40A1-8A54-894233572AD5");
            var getDailySurveyById = Guid.Parse("7FA406B2-3598-47CF-815F-A25C6603412C");

            // 1. Insert all new API claims
            migrationBuilder.InsertData(
                table: "ApiClaims",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { getMedicalHistoryId, "get_medical_history" },
                    { postMedicalHistoryId, "post_medical_history" },
                    { getLatestDailySurveyCreationDateUtcId, "get_latest_daily_survey_creation_date_utc" },
                    { postDailySurveyId, "post_daily_survey" },
                    { getLatestWeeklySurveyId, "get_latest_weekly_survey" },
                    { postWeeklySurveyId, "post_weekly_survey" },
                    { getAllPatientsId, "get_all_patients" },
                    { getPatientId, "get_patient" },
                    { editPatientId, "edit_patient_info" },
                    { getWeeklySurveysForUser, "get_weekly_surveys_for_user" },
                    { getWeeklySurveyById, "get_weekly_survey_by_id" },
                    { getDailySurveysForUser, "get_daily_surveys_for_user" },
                    { getDailySurveyById, "get_daily_survey_by_id" }
                }
            );

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { administratorRoleId, Role.AdministratorName },
                    { clientRoleId, Role.PatientName },
                    { doctorRoleId, Role.DoctorName },
                }
            );

            // 2. Associate claims with client role
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { clientRoleId, getDailySurveysForUser },
                    { clientRoleId, getMedicalHistoryId },
                    { clientRoleId, postMedicalHistoryId },
                    { clientRoleId, getLatestDailySurveyCreationDateUtcId },
                    { clientRoleId, postDailySurveyId },
                    { clientRoleId, getLatestWeeklySurveyId },
                    { clientRoleId, postWeeklySurveyId },
                    { clientRoleId, editPatientId },
                    { clientRoleId, getPatientId },
                }
            );

            // 3. Associate claims with doctor role
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { doctorRoleId, getMedicalHistoryId },
                    { doctorRoleId, getAllPatientsId },
                    { doctorRoleId, getPatientId },
                    { doctorRoleId, getWeeklySurveysForUser },
                    { doctorRoleId, getWeeklySurveyById },
                    { doctorRoleId, getDailySurveysForUser },
                    { doctorRoleId, getDailySurveyById },
                }
            );

            // 4. Associate claims with admin role (same as doctor)
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { administratorRoleId, getMedicalHistoryId },
                    { administratorRoleId, getAllPatientsId },
                    { administratorRoleId, getPatientId },
                    { administratorRoleId, editPatientId },
                    { administratorRoleId, getWeeklySurveysForUser },
                    { administratorRoleId, getWeeklySurveyById },
                    { administratorRoleId, getDailySurveysForUser },
                    { administratorRoleId, getDailySurveyById },
                }
            );

            GenerateDummyUsers(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySurveys");

            migrationBuilder.DropTable(
                name: "MedicalHistories");

            migrationBuilder.DropTable(
                name: "PatientCommonInfos");

            migrationBuilder.DropTable(
                name: "RoleApiClaims");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "WeeklySurveys");

            migrationBuilder.DropTable(
                name: "ApiClaims");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }

        private void GenerateDummyUsers(MigrationBuilder migrationBuilder)
        {
            var administratorRoleId = Guid.Parse("A987303B-BEB1-4ABE-8FCB-3E5C35D33C1C");
            var clientRoleId = Guid.Parse("59A687D0-7D05-4988-B04D-0807C2A57E24");
            var doctorRoleId = Guid.Parse("39B15202-0123-48E1-9BD4-7015D1DF09F9");

            // Define GUIDs for new users
            var adminUserId = Guid.NewGuid();
            var doctorUserId1 = Guid.NewGuid();
            var doctorUserId2 = Guid.NewGuid();
            var patientUserId1 = Guid.NewGuid();
            var patientUserId2 = Guid.NewGuid();
            var patientUserId3 = Guid.NewGuid();

            string passwordHash = HashService.SHA512("123");

            // Insert Admin User
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "IsDeleted", "DateOfChangeOfAccessRights" },
                values: new object[]
                {
                    adminUserId, "admin@example.com", passwordHash, false, null
                });

            // Insert Doctor Users
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "IsDeleted", "DateOfChangeOfAccessRights" },
                values: new object[]
                {
                    doctorUserId1, "doctor1@example.com", passwordHash, false, null
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "IsDeleted", "DateOfChangeOfAccessRights" },
                values: new object[]
                {
                    doctorUserId2, "doctor2@example.com", passwordHash, false, null
                });

            // Insert Patient Users
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "IsDeleted", "DateOfChangeOfAccessRights" },
                values: new object[]
                {
                    patientUserId1, "patient1@example.com", passwordHash, false, null
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "IsDeleted", "DateOfChangeOfAccessRights" },
                values: new object[]
                {
                    patientUserId2, "patient2@example.com", passwordHash, false, null
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "IsDeleted", "DateOfChangeOfAccessRights" },
                values: new object[]
                {
                    patientUserId3, "patient3@example.com", passwordHash, false, null
                });

            // Assign roles to users
            // Admin user gets admin role
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[]
                {
                    adminUserId, administratorRoleId
                });

            // Doctor users get doctor role
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[]
                {
                    doctorUserId1, doctorRoleId
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[]
                {
                    doctorUserId2, doctorRoleId
                });

            // Patient users get client role
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[]
                {
                    patientUserId1, clientRoleId
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[]
                {
                    patientUserId2, clientRoleId
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[]
                {
                    patientUserId3, clientRoleId
                });

            // Create patient info for each patient
            var patientInfo1Id = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "PatientCommonInfos",
                columns: new[] { "Id", "FullName", "PhoneNumber", "TrustedPersonFullName", 
                                "TrustedPersonPhoneNumber", "TrustedPersonEmail", 
                                "InsuranceNumber", "BirthDate", "UserId" },
                values: new object[]
                {
                    patientInfo1Id, "Patient One", "+79001112233", "Trusted Person One",
                    "+79004445566", "trusted1@example.com",
                    "INS-001", new DateOnly(1990, 1, 15), patientUserId1
                });

            var patientInfo2Id = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "PatientCommonInfos",
                columns: new[] { "Id", "FullName", "PhoneNumber", "TrustedPersonFullName", 
                                "TrustedPersonPhoneNumber", "TrustedPersonEmail", 
                                "InsuranceNumber", "BirthDate", "UserId" },
                values: new object[]
                {
                    patientInfo2Id, "Patient Two", "+79007778899", "Trusted Person Two",
                    "+79001234567", "trusted2@example.com",
                    "INS-002", new DateOnly(1985, 5, 20), patientUserId2
                });

            var patientInfo3Id = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "PatientCommonInfos",
                columns: new[] { "Id", "FullName", "PhoneNumber", "TrustedPersonFullName", 
                                "TrustedPersonPhoneNumber", "TrustedPersonEmail", 
                                "InsuranceNumber", "BirthDate", "UserId" },
                values: new object[]
                {
                    patientInfo3Id, "Patient Three", "+79009876543", "Trusted Person Three",
                    "+79008765432", "trusted3@example.com",
                    "INS-003", new DateOnly(1992, 11, 30), patientUserId3
                });

            // Create basic medical history for patients
            migrationBuilder.InsertData(
                table: "MedicalHistories",
                columns: new[] { "Id", "Weight", "Height", "BloodGroup", "RhesusFactor", 
                                "BloodPressure", "Tonometer", "Thermometer", "PregnancyAmount",
                                "PreviousBirthType", "AllergicReactions", "HereditaryDiseases",
                                "IsSmoking", "IsConsumingAlcohol", "EnduredCovid",
                                "CreationDateUtc", "UserId" },
                values: new object[]
                {
                    Guid.NewGuid(), 65, 170, 1, 1, 
                    "120/80", 1, 1, 1,
                    1, "None", 1,
                    false, false, 1,
                    DateTime.UtcNow, patientUserId1
                });

            migrationBuilder.InsertData(
                table: "MedicalHistories",
                columns: new[] { "Id", "Weight", "Height", "BloodGroup", "RhesusFactor", 
                                "BloodPressure", "Tonometer", "Thermometer", "PregnancyAmount",
                                "PreviousBirthType", "AllergicReactions", "HereditaryDiseases",
                                "IsSmoking", "IsConsumingAlcohol", "EnduredCovid",
                                "CreationDateUtc", "UserId" },
                values: new object[]
                {
                    Guid.NewGuid(), 70, 165, 2, 0, 
                    "110/70", 2, 1, 2,
                    2, "Pollen", 1,
                    false, false, 1,
                    DateTime.UtcNow, patientUserId2
                });

            migrationBuilder.InsertData(
                table: "MedicalHistories",
                columns: new[] { "Id", "Weight", "Height", "BloodGroup", "RhesusFactor", 
                                "BloodPressure", "Tonometer", "Thermometer", "PregnancyAmount",
                                "PreviousBirthType", "AllergicReactions", "HereditaryDiseases",
                                "IsSmoking", "IsConsumingAlcohol", "EnduredCovid",
                                "CreationDateUtc", "UserId" },
                values: new object[]
                {
                    Guid.NewGuid(), 60, 175, 3, 1, 
                    "115/75", 1, 2, 1,
                    1, "Penicillin", 1,
                    false, false, 1,
                    DateTime.UtcNow, patientUserId3
                });

            // Add daily survey for the first patient
            migrationBuilder.InsertData(
                table: "DailySurveys",
                columns: new[] { "Id", "AbdomenHurts", "BloodDischarge", "Nausea", "UrgeToPuke",
                                "Temperature", "BloodPressure", "HeartRate", "AdditionalInformation",
                                "CreationDateUtc", "UserId" },
                values: new object[]
                {
                    Guid.NewGuid(), false, false, false, 1,
                    36, "120/80", 72, "Feeling good today",
                    DateTime.UtcNow, patientUserId1
                });

            // Add weekly survey for the first patient
            migrationBuilder.InsertData(
                table: "WeeklySurveys",
                columns: new[] { "Id", "HasOrvi", "HasUnordinaryTemp", "UnordinaryBloodPressure",
                                "HasGynecologicalSymptoms", "HasUnordinaryUrine", "HasSwelling",
                                "WaterConsumed", "Stool", "Urination", "WeightAdded",
                                "CreationDateUtc", "UserId" },
                values: new object[]
                {
                    Guid.NewGuid(), false, false, 1,
                    false, false, false,
                    2000, 1, 1, 0.5m,
                    DateTime.UtcNow, patientUserId1
                });
        }
    }
}

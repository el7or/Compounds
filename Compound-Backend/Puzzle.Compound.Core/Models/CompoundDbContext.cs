using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public CompoundDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public CompoundDbContext(IConfiguration configuration, DbContextOptions<CompoundDbContext> options)
                        : base(options)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyEmployee> CompanyEmployees { get; set; }
        public virtual DbSet<CompanyRole> CompanyRoles { get; set; }
        public virtual DbSet<CompanyUser> CompanyUsers { get; set; }
        public virtual DbSet<CompanyUserRole> CompanyUserRoles { get; set; }
        public virtual DbSet<ActionsInCompanyRoles> ActionsInCompanyRoles { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<CompanyUserCompound> CompanyUserCompounds { get; set; }
        public virtual DbSet<Compound> Compounds { get; set; }
        public virtual DbSet<CompoundAd> CompoundAds { get; set; }
        public virtual DbSet<CompoundAdImage> CompoundAdImages { get; set; }
        public virtual DbSet<CompoundAdHistory> CompoundAdHistories { get; set; }
        public virtual DbSet<CompoundArea> CompoundAreas { get; set; }
        public virtual DbSet<CompoundCall> CompoundCalls { get; set; }
        public virtual DbSet<CompoundCardPrintRequest> CompoundCardPrintRequests { get; set; }
        public virtual DbSet<CompoundGate> CompoundGates { get; set; }
        public virtual DbSet<CompoundGroup> CompoundGroups { get; set; }
        public virtual DbSet<OwnerUnit> OwnerUnits { get; set; }
        public virtual DbSet<CompoundHelp> CompoundHelps { get; set; }
        public virtual DbSet<CompoundInstruction> CompoundInstructions { get; set; }
        public virtual DbSet<CompoundManagement> CompoundManagements { get; set; }
        public virtual DbSet<CompoundNearbyPlace> CompoundNearbyPlaces { get; set; }
        public virtual DbSet<CompoundNotice> CompoundNotices { get; set; }
        public virtual DbSet<CompoundNotification> CompoundNotifications { get; set; }
        public virtual DbSet<CompoundOwner> CompoundOwners { get; set; }
        public virtual DbSet<CompoundOwnerProperty> CompoundOwnerProperties { get; set; }
        public virtual DbSet<CompoundResident> CompoundResidents { get; set; }
        public virtual DbSet<CompoundSecurity> CompoundSecurities { get; set; }
        public virtual DbSet<CompoundService> CompoundServices { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ServiceSubType> ServiceSubTypes { get; set; }
        public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }
        public virtual DbSet<ServiceRequestSubType> ServiceRequestSubTypes { get; set; }
        public virtual DbSet<ServiceAttachment> ServiceAttachments { get; set; }
        public virtual DbSet<CompanyUserServiceType> CompanyUserServices { get; set; }
        public virtual DbSet<CompoundResidentsService> CompoundResidentsServices { get; set; }
        public virtual DbSet<CompoundService> CompoundIssues { get; set; }
        public virtual DbSet<IssueType> IssueTypes { get; set; }
        public virtual DbSet<IssueRequest> IssueRequests { get; set; }
        public virtual DbSet<IssueAttachment> IssueAttachments { get; set; }
        public virtual DbSet<CompanyUserIssueType> CompanyUserIssues { get; set; }
        public virtual DbSet<CompoundResidentsIssue> CompoundResidentsIssues { get; set; }
        public virtual DbSet<CompoundStore> CompoundStores { get; set; }
        public virtual DbSet<CompoundSurvey> CompoundSurveys { get; set; }
        public virtual DbSet<CompoundUnit> CompoundUnits { get; set; }
        public virtual DbSet<CompoundUnitType> CompoundUnitTypes { get; set; }
        public virtual DbSet<CompoundVisitor> CompoundVisitors { get; set; }
        public virtual DbSet<CompoundVisitsRequest> CompoundVisitsRequests { get; set; }
        public virtual DbSet<Gate> Gates { get; set; }
        public virtual DbSet<OwnerAssignUnitRequest> OwnerAssignUnitRequests { get; set; }
        public virtual DbSet<OwnerAssignedUnit> OwnerAssignedUnits { get; set; }
        public virtual DbSet<OwnerRegistration> OwnerRegistrations { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<PlanDetail> PlanDetails { get; set; }
        public virtual DbSet<PlanItem> PlanItems { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<VisitRequest> VisitRequests { get; set; }
        public virtual DbSet<VisitRequestAttachment> VisitRequestAttachments { get; set; }
        public virtual DbSet<VisitTransactionHistory> VisitTransactionHistory { get; set; }
        public virtual DbSet<PrintCardRequest> PrintCardRequests { get; set; }
        public virtual DbSet<CompoundNews> CompoundNews { get; set; }
        public virtual DbSet<CompoundNewsImage> CompoundNewsImages { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<CompoundReport> CompoundReports { get; set; }

        //Push Notification
        public virtual DbSet<NotificationUser> NotificationUsers { get; set; }
        public virtual DbSet<NotificationSchedule> NotificationSchedules { get; set; }
        //
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("CompoundDbConnection"), x => x.UseNetTopologySuite());
            }

            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<VisitRequest>(entity =>
            {
                entity.Property(x => x.VisitRequestId).HasDefaultValueSql("(newid())");

                entity.Property(x => x.VisitorName).HasMaxLength(500);

                entity.Property(x => x.Details).HasMaxLength(500);

                entity.Property(x => x.Code).HasMaxLength(500);

                entity.Property(x => x.QrCode);

                entity.Property(x => x.Days).HasMaxLength(500);

                entity.Property(x => x.CarNo).HasMaxLength(500);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(x => x.VisitType).HasMaxLength(50).HasConversion<string>();

                entity.Property(x => x.Type).HasMaxLength(50).HasConversion<string>();

                entity.HasOne(x => x.CompoundUnit)
                                .WithMany(x => x.VisitRequests)
                                .HasForeignKey(x => x.CompoundUnitId)
                                .IsRequired();

                entity.HasOne(x => x.OwnerRegistration)
                                .WithMany(x => x.VisitRequests)
                                .HasForeignKey(x => x.OwnerRegistrationId)
                                .IsRequired();

                entity.HasOne(x => x.Compound)
                                .WithMany(x => x.VisitRequests)
                                .HasForeignKey(x => x.CompoundId)
                                .IsRequired();

                entity.HasOne(x => x.Company)
                                .WithMany(x => x.VisitRequests)
                                .HasForeignKey(x => x.CompanyId)
                                .IsRequired();
            });

            modelBuilder.Entity<VisitRequestAttachment>(entity =>
            {
                entity.Property(x => x.VisitRequestAttachmentId).HasDefaultValueSql("(newid())");

                entity.HasOne(x => x.VisitRequest).WithMany(x => x.Attachments).HasForeignKey(x => x.VisitRequestId).IsRequired();

                // entity.Property(x => x.Type).HasMaxLength(50).HasConversion<string>();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(x => x.Path).HasMaxLength(1000);
            });

            modelBuilder.Entity<PrintCardRequest>(entity =>
            {
                entity.Property(x => x.Id).HasDefaultValueSql("(newid())");

                entity.Property(x => x.Name).HasMaxLength(50).HasConversion<string>();

                entity.Property(x => x.Details).HasMaxLength(500).HasConversion<string>();

                entity.Property(x => x.Status).HasMaxLength(50).HasConversion<string>();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(x => x.Picture).HasMaxLength(1000);

                entity.HasOne(x => x.OwnerRegistration).WithMany(x => x.PrintCardRequests).HasForeignKey(x => x.OwnerRegisterationId).IsRequired();

                entity.HasOne(x => x.CompoundUnit).WithMany(x => x.PrintCardRequests).HasForeignKey(x => x.CompoundUnitId).IsRequired();

                entity.HasOne(x => x.VisitRequest).WithMany(x => x.PrintCardRequests).HasForeignKey(x => x.VisitRequestId);
            });

            modelBuilder.Entity<VisitTransactionHistory>(entity =>
            {
                entity.Property(x => x.VisitTransactionHistoryId).HasDefaultValueSql("(newid())");

                entity.HasOne(x => x.VisitRequest).WithMany(x => x.VisitTransactionHistories).HasForeignKey(x => x.VisitRequestId).IsRequired();

                entity.HasOne(x => x.Gate).WithMany(x => x.VisitTransactionHistories).HasForeignKey(x => x.GateId).IsRequired();
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.CompanyType)
                                .HasComment("Multi=>2, Single=>1");

                entity.Property(e => e.CreationDate)
                                .HasColumnType("datetime");

                entity.Property(e => e.Email)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Logo)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.Name_Ar)
                                .IsRequired()
                                .HasMaxLength(500);

                entity.Property(e => e.Name_En)
                                .IsRequired()
                                .HasMaxLength(500);

                entity.Property(e => e.Phone)
                                .IsRequired()
                                .HasMaxLength(20);

                entity.Property(e => e.PlanId);

                entity.Property(e => e.WhatsAppNum)
                                .HasMaxLength(50);

                entity.Property(e => e.ChargeName)
                                .IsRequired()
                                .HasMaxLength(500);

                entity.HasOne(d => d.Plan)
                                .WithMany(p => p.Companies)
                                .HasForeignKey(d => d.PlanId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompanyEmployee>(entity =>
            {
                entity.ToTable("CompanyEmployees");

                entity.Property(e => e.CompanyEmployeeId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyId);

                entity.Property(e => e.Email)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                                .IsRequired()
                                .HasMaxLength(200);
            });

            modelBuilder.Entity<CompanyUser>(entity =>
            {
                entity.HasKey(e => e.CompanyUserId);

                entity.ToTable("CompanyUsers");

                entity.Property(e => e.CompanyUserId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Image).HasMaxLength(200);
                entity.Property(e => e.CreationDate)
                                .HasColumnType("datetime")
                                .HasColumnName("CreationDate");

                entity.Property(e => e.CompanyId);

                entity.Property(e => e.Id).HasComment("Gate ID, Employee ID, Company ID");

                entity.Property(e => e.IsActive);

                entity.Property(e => e.IsDeleted);

                entity.Property(e => e.IsVerified);

                entity.Property(e => e.Password)
                                .IsRequired()
                                .HasMaxLength(1000);

                entity.Property(e => e.UserType)
                                .HasColumnName("UserType")
                                .HasComment("1 = Company, 2 = Employee, 3 = Gate");

                entity.Property(e => e.Username)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.HasOne(d => d.Company)
                                .WithMany(p => p.CompanyUsers)
                                .HasForeignKey(d => d.CompanyId);
                entity.Property(e => e.NameAr)
                                                .IsRequired()
                                                .HasMaxLength(200);
                entity.Property(e => e.NameEn)
                                                .IsRequired()
                                                .HasMaxLength(200);
                entity.Property(e => e.Email)
                                                .IsRequired()
                                                .HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<CompanyRole>(entity =>
            {
                entity.ToTable("CompanyRoles");

                entity.Property(e => e.CompanyRoleId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyId);

                entity.Property(e => e.RoleArabicName)
                        .IsRequired()
                        .HasMaxLength(100);

                entity.Property(e => e.RoleEnglishName)
                        .IsRequired()
                        .HasMaxLength(100);

                entity.HasOne(d => d.Company)
                        .WithMany(p => p.CompanyRoles)
                        .HasForeignKey(d => d.CompanyId);
            });

            modelBuilder.Entity<CompanyUserRole>(entity =>
            {
                entity.ToTable("CompanyUserRoles");

                entity.Property(e => e.CompanyUserRoleId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyRoleId);

                entity.Property(e => e.CompanyUserId);

                entity.HasOne(d => d.CompanyRole)
                                .WithMany(p => p.CompanyUserRoles)
                                .HasForeignKey(d => d.CompanyRoleId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompanyUser)
                                .WithMany(p => p.CompanyUserRoles)
                                .HasForeignKey(d => d.CompanyUserId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.RefreshTokenId)
                                .HasColumnName("RefreshToken_Id");

                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.UserType).HasColumnName("User_Type");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CreatedByIp)
                                .IsRequired()
                                .HasMaxLength(50);

                entity.Property(e => e.Expires).HasColumnType("datetime");

                entity.Property(e => e.ReplacedByToken).HasMaxLength(1000);

                entity.Property(e => e.Revoked).HasColumnType("datetime");

                entity.Property(e => e.RevokedByIp).HasMaxLength(50);

                entity.Property(e => e.Token)
                                .IsRequired()
                                .HasMaxLength(1000);
            });

            modelBuilder.Entity<ActionsInCompanyRoles>(entity =>
            {
                entity.Property(e => e.ActionsInCompanyRolesId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyRoleId);

                entity.Property(e => e.SystemPageActionId);

                entity.HasOne(d => d.CompanyRoles)
                        .WithMany(p => p.ActionsInCompanyRoles)
                        .HasForeignKey(d => d.CompanyRoleId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SystemPageActions)
                        .WithMany(p => p.ActionsInCompanyRoles)
                        .HasForeignKey(d => d.SystemPageActionId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompanyUserCompound>(entity =>
            {
                entity.HasKey(e => e.CompanyUserCompoundId);

                entity.ToTable("CompanyUserCompounds");

                entity.Property(e => e.CompanyUserCompoundId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AssignedDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))")
                                .IsRequired();

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))")
                                .IsRequired();

                entity.HasOne(e => e.Compound)
                                .WithMany(e => e.CompanyUserCompounds)
                                .HasForeignKey(e => e.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.CompanyUser)
                                .WithMany(e => e.CompanyUserCompounds)
                                .HasForeignKey(e => e.CompanyUserId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<Compound>(entity =>
            {
                entity.Property(e => e.CompoundId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyId);

                entity.Property(e => e.CreationDate)
                                .HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Image).HasMaxLength(200);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.NameAr)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.NameEn)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.EmergencyPhone).HasMaxLength(20);

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.TimeZoneOffset).HasDefaultValue(0);

                entity.Property(e => e.TimeZoneValue).HasMaxLength(100);

                entity.Property(e => e.TimeZoneText).HasMaxLength(100);

                entity.HasOne(d => d.Company)
                                .WithMany(p => p.Compounds)
                                .HasForeignKey(d => d.CompanyId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundAd>(entity =>
            {
                entity.ToTable("CompoundAds");

                entity.Property(e => e.CompoundAdId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.AdUrl);

                entity.Property(e => e.EnglishTitle)
                .HasMaxLength(100);

                entity.Property(e => e.ArabicTitle)
                .HasMaxLength(100);

                entity.Property(e => e.EnglishDescription);

                entity.Property(e => e.ArabicDescription);

                entity.Property(e => e.CreationDate)
                .HasColumnType("datetime");

                entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundAds)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundAdImage>(entity =>
            {
                entity.ToTable("CompoundAdImages");

                entity.Property(e => e.CompoundAdImageId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundAdId);

                entity.Property(e => e.Path)
                .HasMaxLength(1000);

                entity.Property(e => e.IsMain);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(e => e.CompoundAd)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.CompoundAdId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundAdHistory>(entity =>
            {
                entity.ToTable("CompoundAdHistories");

                entity.HasKey(e => e.CompoundAdHistoryId);

                entity.Property(e => e.CompoundAdHistoryId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.OwnerRegistrationId);

                entity.Property(e => e.CompoundAdId);

                entity.Property(e => e.ActionDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

                entity.Property(e => e.ActionType)
                .IsRequired()
                .HasComment("Show = 1, Click = 2");

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.CompoundAdHistories)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompoundAd)
                                .WithMany(p => p.CompoundAdHistories)
                                .HasForeignKey(d => d.CompoundAdId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundArea>(entity =>
            {
                entity.ToTable("CompoundAreas");

                entity.Property(e => e.CompoundAreaId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundAreas)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundCall>(entity =>
            {
                entity.ToTable("CompoundCalls");

                entity.Property(e => e.CompoundCallId)
                                .HasColumnName("CompoundCallId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.EmergencyPhone)
                                .IsRequired()
                                .HasMaxLength(20);

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.CallDate)
                                .HasColumnType("datetime");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundCalls)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.CompoundCalls)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundCardPrintRequest>(entity =>
            {
                entity.HasKey(e => e.CompoundCardRequestId);

                entity.ToTable("CompoundCardPrintRequests");

                entity.Property(e => e.CompoundCardRequestId)
                                .HasColumnName("CompoundCardRequestId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CardsCount).HasColumnName("CardsCount");

                entity.Property(e => e.CompoundId).HasColumnName("CompoundId");

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.RequestDate)
                                .HasColumnType("datetime")
                                .HasColumnName("RequestDate");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundCardPrintRequests)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundGate>(entity =>
            {
                entity.ToTable("CompoundGates");

                entity.Property(e => e.CompoundGateId)
                                .HasColumnName("CompoundGateId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId).HasColumnName("CompoundId");

                entity.Property(e => e.GateId).HasColumnName("Gate_Id");

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundGates)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Gate)
                                .WithMany(p => p.CompoundGates)
                                .HasForeignKey(d => d.GateId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundGroup>(entity =>
            {
                entity.ToTable("CompoundGroups");

                entity.Property(e => e.CompoundGroupId)
                                .HasColumnName("CompoundGroupId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId).HasColumnName("CompoundId");

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.NameAr)
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnName("NameAr");

                entity.Property(e => e.NameEn)
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnName("NameEn");

                entity.Property(e => e.CreationDate)
                                .HasColumnType("datetime")
                                .HasColumnName("CreationDate");


                entity.Property(e => e.ParentGroupId)
                                .HasColumnName("ParentGroupId");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundGroups)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Group)
                                .WithMany(p => p.Groups)
                                .HasForeignKey(d => d.ParentGroupId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OwnerUnit>(entity =>
            {
                entity.ToTable("OwnerUnits");

                entity.HasKey(e => e.OwnerUnitId);

                entity.Property(e => e.OwnerUnitId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundOwnerId);

                entity.Property(e => e.CompoundUnitId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CompoundOwner)
                                .WithMany(p => p.OwnerUnits)
                                .HasForeignKey(d => d.CompoundOwnerId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompoundUnit)
                                .WithMany(p => p.OwnerUnits)
                                .HasForeignKey(d => d.CompoundUnitId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => m.IsDeleted != true);
            });

            modelBuilder.Entity<CompoundHelp>(entity =>
            {
                entity.ToTable("CompoundHelps");

                entity.Property(e => e.CompoundHelpId)
                                .HasColumnName("CompoundHelpId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId).HasColumnName("CompoundId");

                entity.Property(e => e.HelpContent)
                                .IsRequired()
                                .HasColumnName("HelpContent");

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.RequestDate)
                                .HasColumnType("datetime")
                                .HasColumnName("RequestDate");

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundHelps)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundInstruction>(entity =>
            {
                entity.ToTable("CompoundInstructions");

                entity.Property(e => e.CompoundInstructionId)
                                .HasColumnName("CompoundInstructionId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AddedDate)
                                .HasColumnType("datetime")
                                .HasColumnName("AddedDate");

                entity.Property(e => e.CompoundId).HasColumnName("CompoundId");

                entity.Property(e => e.InstructionContent)
                                .IsRequired()
                                .HasColumnName("InstructionContent");

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundInstructions)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundManagement>(entity =>
            {
                entity.ToTable("CompoundManagements");

                entity.Property(e => e.CompoundManagementId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CompoundNearbyPlace>(entity =>
            {
                entity.HasKey(e => e.CompoundPlaceId);

                entity.ToTable("CompoundNearbyPlaces");

                entity.Property(e => e.CompoundPlaceId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.PlaceName)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.Type).HasMaxLength(100);

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundNearbyPlaces)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundNews>(entity =>
            {
                entity.ToTable("CompoundNews");

                entity.Property(e => e.CompoundNewsId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.EnglishTitle)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.ArabicTitle)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.EnglishSummary)
                .HasMaxLength(250)
                .IsRequired();

                entity.Property(e => e.ArabicSummary)
                .HasMaxLength(250)
                .IsRequired();

                entity.Property(e => e.EnglishDetails)
                .IsRequired();

                entity.Property(e => e.ArabicDetails)
                .IsRequired();

                entity.Property(e => e.PublishDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.ForegroundTillDate)
                .HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))")
                                .IsRequired();

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))")
                                .IsRequired();

                entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

                entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

                entity.HasOne(e => e.Compound)
                                .WithMany(e => e.CompoundNews)
                                .HasForeignKey(e => e.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundNewsImage>(entity =>
            {
                entity.ToTable("CompoundNewsImages");

                entity.Property(e => e.CompoundNewsImageId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundNewsId);

                entity.Property(e => e.Path)
                .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(e => e.CompoundNews)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.CompoundNewsId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundNotice>(entity =>
            {
                entity.ToTable("CompoundNotices");

                entity.Property(e => e.CompoundNoticeId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyUserId);

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.NoticeContent)
                                .IsRequired();

                entity.Property(e => e.NoticeDate)
                                .HasColumnType("datetime");

                entity.HasOne(d => d.CompanyUser)
                                .WithMany(p => p.CompoundNotices)
                                .HasForeignKey(d => d.CompanyUserId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundNotices)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundNotification>(entity =>
            {
                entity.ToTable("CompoundNotifications");

                entity.Property(e => e.CompoundNotificationId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.EnglishTitle)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.ArabicTitle)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.EnglishMessage)
                .IsRequired();

                entity.Property(e => e.ArabicMessage)
                .IsRequired();

                entity.Property(e => e.IsOwnerOnly);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundNotifications)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundNotificationImage>(entity =>
            {
                entity.ToTable("CompoundNotificationImages");

                entity.Property(e => e.CompoundNotificationImageId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundNotificationId);

                entity.Property(e => e.Path)
                .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(e => e.CompoundNotification)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.CompoundNotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<NotificationUnit>(entity =>
            {
                entity.ToTable("NotificationUnits");

                entity.HasKey(e => e.NotificationUnitId);

                entity.Property(e => e.NotificationUnitId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundNotificationId);

                entity.Property(e => e.CompoundUnitId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CompoundNotification)
                                .WithMany(p => p.NotificationUnits)
                                .HasForeignKey(d => d.CompoundNotificationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompoundUnit)
                                .WithMany(p => p.NotificationUnits)
                                .HasForeignKey(d => d.CompoundUnitId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<OwnerNotification>(entity =>
            {
                entity.ToTable("OwnerNotifications");

                entity.HasKey(e => e.OwnerNotificationId);

                entity.Property(e => e.OwnerNotificationId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.OwnerRegistrationId);

                entity.Property(e => e.CompoundNotificationId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.OwnerNotifications)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompoundNotification)
                                .WithMany(p => p.OwnerNotifications)
                                .HasForeignKey(d => d.CompoundNotificationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundOwner>(entity =>
            {
                entity.ToTable("CompoundOwners");

                entity.Property(e => e.CompoundOwnerId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.Gender)
                                .HasMaxLength(15);

                entity.Property(e => e.Image).HasMaxLength(200);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.OwnerRegistrationId);

                entity.Property(e => e.Phone)
                                .IsRequired()
                                .HasMaxLength(50);

                entity.Property(e => e.WhatsAppNum)
                                .HasMaxLength(50);

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.CompoundOwners)
                                .HasForeignKey(d => d.OwnerRegistrationId);

                entity.HasQueryFilter(m => m.IsDeleted != true);
            });

            modelBuilder.Entity<CompoundOwnerProperty>(entity =>
            {
                entity.ToTable("CompoundOwnerProperties");

                entity.Property(e => e.CompoundOwnerPropertyId)
                                .HasColumnName("CompoundOwnerPropertyId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId).HasColumnName("CompoundId");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Location).IsRequired();

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundOwnerProperties)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundResident>(entity =>
            {
                entity.ToTable("CompoundResidents");

                entity.Property(e => e.CompoundResidentId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CompoundSecurity>(entity =>
            {
                entity.ToTable("CompoundSecurities");

                entity.Property(e => e.CompoundSecurityId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.NationalId)
                                .IsRequired()
                                .HasMaxLength(20);

                entity.Property(e => e.Phone)
                                .IsRequired()
                                .HasMaxLength(20);

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundSecurities)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundService>(entity =>
            {
                entity.ToTable("CompoundServices");

                entity.Property(e => e.CompoundServiceId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundServices)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ServiceType)
                                .WithMany(p => p.CompoundServices)
                                .HasForeignKey(d => d.ServiceTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => m.IsDeleted != true);
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.Property(e => e.ServiceTypeId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ArabicName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.EnglishName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.Icon)
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnName("Icon");

                entity.Property(e => e.IsFixed)
                .HasColumnType("bit");

                entity.Property(e => e.Order);
            });

            modelBuilder.Entity<ServiceSubType>(entity =>
            {

                entity.Property(e => e.ServiceSubTypeId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ArabicName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.EnglishName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.Order);

                entity.Property(e => e.Cost)
                                .IsRequired()
                                .HasPrecision(18, 2);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CompoundService)
                                .WithMany(p => p.ServiceSubTypes)
                                .HasForeignKey(d => d.CompoundServiceId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<ServiceRequest>(entity =>
            {

                entity.Property(e => e.ServiceRequestId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.RequestNumber)
                .HasColumnType("int");

                entity.Property(e => e.PostTime)
                .HasColumnType("datetime");

                entity.Property(e => e.Status)
                .HasColumnName("Status")
                .HasColumnType("smallint");

                entity.Property(e => e.CancelType)
                .HasColumnName("CancelType")
                .HasColumnType("smallint");

                entity.Property(e => e.UpdateStatusTime)
                .HasColumnType("datetime");

                entity.Property(e => e.Note)
                .HasColumnName("Note")
                .HasMaxLength(500);

                entity.Property(e => e.Rate)
                .HasColumnName("Rate")
                .HasColumnType("smallint");

                entity.Property(e => e.Comment)
                .HasColumnName("Comment")
                .HasMaxLength(500);

                entity.Property(e => e.OwnerComment)
                .HasColumnName("OwnerComment")
                .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDelete)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Date)
                .HasColumnName("date");

                entity.Property(e => e.From)
                .HasColumnName("From")
                .HasColumnType("datetime");

                entity.Property(e => e.To)
                .HasColumnName("To")
                .HasColumnType("datetime");

                entity.Property(e => e.UpdateStatusBy);

                entity.Property(e => e.ServiceSubTypesTotalCost)
                                .IsRequired()
                                .HasPrecision(18, 2);

                entity.HasOne(d => d.ServiceType)
                                .WithMany(p => p.ServiceRequests)
                                .HasForeignKey(d => d.ServiceTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.ServiceRequests)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompoundUnit)
                                .WithMany(p => p.ServiceRequests)
                                .HasForeignKey(d => d.CompoundUnitId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<ServiceRequestSubType>(entity =>
            {
                entity.HasKey(e => e.ServiceRequestSubTypeId);

                entity.ToTable("ServiceRequestSubTypes");

                entity.Property(e => e.ServiceRequestSubTypeId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ServiceSubTypeCost)
                                .IsRequired()
                                .HasPrecision(18, 2);

                entity.Property(e => e.ServiceSubTypeQuantity)
                                .IsRequired();

                entity.HasOne(e => e.ServiceRequest)
                                .WithMany(e => e.ServiceRequestSubTypes)
                                .HasForeignKey(e => e.ServiceRequestId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.ServiceSubType)
                                .WithMany(e => e.ServiceRequestSubTypes)
                                .HasForeignKey(e => e.ServiceSubTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ServiceAttachment>(entity =>
            {
                entity.Property(x => x.ServiceAttachmentId).HasDefaultValueSql("(newid())");

                entity.HasOne(x => x.ServiceRequest).WithMany(x => x.ServiceAttachments).HasForeignKey(x => x.ServiceRequestId).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(x => x.Path).HasMaxLength(1000);
            });

            modelBuilder.Entity<CompanyUserServiceType>(entity =>
            {
                entity.HasKey(e => e.CompanyUserServiceId);

                entity.ToTable("CompanyUserServices");

                entity.Property(e => e.CompanyUserServiceId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AssignedDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))")
                                .IsRequired();

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))")
                                .IsRequired();

                entity.HasOne(e => e.ServiceType)
                                .WithMany(e => e.CompanyUserServices)
                                .HasForeignKey(e => e.ServiceTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.CompanyUserCompound)
                                .WithMany(e => e.CompanyUserServices)
                                .HasForeignKey(e => e.CompanyUserCompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundResidentsService>(entity =>
            {
                entity.HasKey(e => e.CompoundResidentServiceId);

                entity.ToTable("CompoundResidentsServices");

                entity.Property(e => e.CompoundResidentServiceId)
                                .HasColumnName("CompoundResidentServiceId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundResidentId);

                entity.Property(e => e.CompoundServiceId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderDatetime)
                                .HasColumnType("datetime");
            });

            modelBuilder.Entity<CompoundIssue>(entity =>
            {
                entity.ToTable("CompoundIssues");

                entity.Property(e => e.CompoundIssueId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundIssues)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IssueType)
                                .WithMany(p => p.CompoundIssues)
                                .HasForeignKey(d => d.IssueTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IssueType>(entity =>
            {
                entity.Property(e => e.IssueTypeId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ArabicName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.EnglishName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.Icon)
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnName("Icon");

                entity.Property(e => e.IsFixed)
                .HasColumnType("bit");

                entity.Property(e => e.Order);
            });

            modelBuilder.Entity<IssueRequest>(entity =>
            {

                entity.Property(e => e.IssueRequestId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.RequestNumber)
                .HasColumnType("int");

                entity.Property(e => e.PostTime)
                .HasColumnType("datetime");

                entity.Property(e => e.Status)
                .HasColumnName("Status")
                .HasColumnType("smallint");

                entity.Property(e => e.CancelType)
                .HasColumnName("CancelType")
                .HasColumnType("smallint");

                entity.Property(e => e.UpdateStatusTime)
                .HasColumnType("datetime");

                entity.Property(e => e.Note)
                .HasColumnName("Note")
                .HasMaxLength(500);

                entity.Property(e => e.Rate)
                .HasColumnName("Rate")
                .HasColumnType("smallint");

                entity.Property(e => e.Comment)
                .HasColumnName("Comment")
                .HasMaxLength(500);

                entity.Property(e => e.OwnerComment)
                .HasColumnName("OwnerComment")
                .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDelete)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdateStatusBy);

                entity.HasOne(d => d.IssueType)
                                .WithMany(p => p.IssueRequests)
                                .HasForeignKey(d => d.IssueTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.IssueRequests)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IssueAttachment>(entity =>
            {
                entity.Property(x => x.IssueAttachmentId).HasDefaultValueSql("(newid())");

                entity.HasOne(x => x.IssueRequest).WithMany(x => x.IssueAttachments).HasForeignKey(x => x.IssueRequestId).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(x => x.Path).HasMaxLength(1000);
            });

            modelBuilder.Entity<CompanyUserIssueType>(entity =>
            {
                entity.HasKey(e => e.CompanyUserIssueId);

                entity.ToTable("CompanyUserIssues");

                entity.Property(e => e.CompanyUserIssueId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AssignedDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))")
                                .IsRequired();

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))")
                                .IsRequired();

                entity.HasOne(e => e.IssueType)
                                .WithMany(e => e.CompanyUserIssues)
                                .HasForeignKey(e => e.IssueTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.CompanyUserCompound)
                                .WithMany(e => e.CompanyUserIssues)
                                .HasForeignKey(e => e.CompanyUserCompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<CompoundResidentsIssue>(entity =>
            {
                entity.HasKey(e => e.CompoundResidentIssueId);

                entity.ToTable("CompoundResidentsIssues");

                entity.Property(e => e.CompoundResidentIssueId)
                                .HasColumnName("CompoundResidentIssueId")
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundResidentId);

                entity.Property(e => e.CompoundIssueId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderDatetime)
                                .HasColumnType("datetime");
            });

            modelBuilder.Entity<CompoundStore>(entity =>
            {
                entity.ToTable("CompoundStores");

                entity.Property(e => e.CompoundStoreId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.OwnerName)
                                .HasMaxLength(200);

                entity.Property(e => e.Phone)
                                .IsRequired()
                                .HasMaxLength(20);

                entity.Property(e => e.StoreName)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundStores)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundSurvey>(entity =>
            {
                entity.ToTable("CompoundSurveys");

                entity.Property(e => e.CompoundSurveyId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CompoundUnit>(entity =>
            {
                entity.ToTable("CompoundUnits");

                entity.Property(e => e.CompoundUnitId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompoundGroupId);

                entity.Property(e => e.CompoundUnitTypeId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.HasOne(d => d.CompoundGroup)
                                .WithMany(p => p.CompoundUnits)
                                .HasForeignKey(d => d.CompoundGroupId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CompoundUnitType)
                                .WithMany(p => p.CompoundUnits)
                                .HasForeignKey(d => d.CompoundUnitTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompoundUnitType>(entity =>
            {
                entity.ToTable("CompoundUnitTypes");

                entity.HasKey(e => e.CompoundUnitTypeId);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.NameAr)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.NameEn)
                                .IsRequired()
                                .HasMaxLength(100);
            });

            modelBuilder.Entity<CompoundVisitor>(entity =>
            {
                entity.ToTable("CompoundVisitors");

                entity.Property(e => e.CompoundVisitorId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CompoundVisitsRequest>(entity =>
            {
                entity.ToTable("CompoundVisitsRequests");

                entity.Property(e => e.CompoundVisitsRequestId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Gate>(entity =>
            {
                entity.Property(e => e.GateId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.EntryType)
                                .HasComment("1 = Entrance, 2 = Exit, 3 = All");

                entity.Property(e => e.GateName)
                                .IsRequired()
                                .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<OwnerAssignUnitRequest>(entity =>
            {
                entity.ToTable("OwnerAssignUnitRequests");

                entity.Property(e => e.OwnerAssignUnitRequestId);

                entity.Property(e => e.CompoundUnitId);

                entity.Property(e => e.IsDeleted);

                entity.Property(e => e.OwnerRegistrationId);

                entity.Property(e => e.RequestDate)
                                .HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("Waiting = 0, Cancel = 1, Approved = 2");

                entity.HasOne(d => d.CompoundUnit)
                                .WithMany(p => p.OwnerAssignUnitRequests)
                                .HasForeignKey(d => d.CompoundUnitId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.OwnerAssignUnitRequests)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OwnerAssignedUnit>(entity =>
            {
                entity.ToTable("OwnerAssignedUnits");

                entity.Property(e => e.OwnerAssignedUnitId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AssignedDate)
                                .HasColumnType("datetime");

                entity.Property(e => e.CompoundUnitId);

                entity.Property(e => e.EndTo)
                                .HasColumnType("datetime");

                entity.Property(e => e.IsActive);

                entity.Property(e => e.IsDeleted);

                entity.Property(e => e.OwnerRegistrationId);

                entity.Property(e => e.StartFrom)
                                .HasColumnType("datetime");

                entity.HasOne(d => d.CompoundUnit)
                                .WithMany(p => p.OwnerAssignedUnits)
                                .HasForeignKey(d => d.CompoundUnitId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OwnerRegistration)
                                .WithMany(p => p.OwnerAssignedUnits)
                                .HasForeignKey(d => d.OwnerRegistrationId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OwnerRegistration>(entity =>
            {
                entity.ToTable("OwnerRegistrations");

                entity.Property(e => e.OwnerRegistrationId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.CreatedByRegistrationId);

                entity.Property(e => e.Email)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.Gender)
                                .HasMaxLength(15);

                entity.Property(e => e.Image).HasMaxLength(200);

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.MainRegistrationId);

                entity.Property(e => e.Name)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.Phone)
                                .IsRequired()
                                .HasMaxLength(50);

                entity.Property(e => e.RegisterDate)
                                .HasColumnType("datetime");

                entity.Property(e => e.UserConfirmed);
                entity.Property(e => e.IsBlocked).HasColumnName("IsBlocked");

                entity.Property(e => e.UserType)
                                .HasDefaultValueSql("((1))")
                                .HasComment("Owner => 1, Sub User => 2, Tenant => 3");

                entity.Property(e => e.WhatsAppNum)
                                .HasMaxLength(50);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.Property(e => e.PlanId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.PlanNameAr)
                                .HasMaxLength(500);

                entity.Property(e => e.PlanNameEn)
                                .IsRequired()
                                .HasMaxLength(500);
            });

            modelBuilder.Entity<PlanDetail>(entity =>
            {
                entity.ToTable("PlanDetails");

                entity.Property(e => e.PlanDetailId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.ItemCount);

                entity.Property(e => e.PlanId);

                entity.Property(e => e.PlanItemId);

                entity.HasOne(d => d.Plan)
                                .WithMany(p => p.PlanDetails)
                                .HasForeignKey(d => d.PlanId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PlanItem)
                                .WithMany(p => p.PlanDetails)
                                .HasForeignKey(d => d.PlanItemId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PlanItem>(entity =>
            {
                entity.ToTable("PlanItems");

                entity.Property(e => e.PlanItemId)
                                .ValueGeneratedNever();

                entity.Property(e => e.IsActive)
                                .IsRequired()
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted);

                entity.Property(e => e.PlanItemDetailAr);

                entity.Property(e => e.PlanItemDetailEn);

                entity.Property(e => e.PlanItemNameAr)
                                .HasMaxLength(200);

                entity.Property(e => e.PlanItemNameEn)
                                .HasMaxLength(200);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActionPath)
                                .HasMaxLength(1000);

                entity.Property(e => e.ControllerPath)
                                .HasMaxLength(200);

                entity.Property(e => e.CurrentJsonData)
                                .IsRequired();

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.Property(e => e.PostDateTime)
                                .HasColumnType("datetime");

                entity.Property(e => e.PreviousJsonData);

                entity.Property(e => e.RecordId)
                                .IsRequired()
                                .HasMaxLength(1000);

                entity.Property(e => e.TableName)
                                .IsRequired()
                                .HasMaxLength(200);

                entity.Property(e => e.TransactionType)
                                .IsRequired()
                                .HasMaxLength(50);

                entity.Property(e => e.UserId);
            });

            modelBuilder.Entity<ReportType>(entity =>
            {
                entity.Property(e => e.ReportTypeId)
                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ArabicName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.EnglishName)
                                .IsRequired()
                                .HasMaxLength(100);

                entity.Property(e => e.Icon)
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnName("Icon");

                entity.Property(e => e.IsFixed)
                .HasColumnType("bit");

                entity.Property(e => e.Order);
            });

            modelBuilder.Entity<CompoundReport>(entity =>
            {
                entity.ToTable("CompoundReports");

                entity.Property(e => e.CompoundReportId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                                .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted)
                                .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Compound)
                                .WithMany(p => p.CompoundReports)
                                .HasForeignKey(d => d.CompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ReportType)
                                .WithMany(p => p.CompoundReports)
                                .HasForeignKey(d => d.ReportTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CompanyUserReport>(entity =>
            {
                entity.HasKey(e => e.CompanyUserReportId);

                entity.ToTable("CompanyUserReports");

                entity.Property(e => e.CompanyUserReportId)
                                .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AssignedDate)
                .HasColumnType("datetime")
                .IsRequired();

                entity.Property(e => e.IsActive)
                                .HasColumnName("IsActive")
                                .HasDefaultValueSql("((1))")
                                .IsRequired();

                entity.Property(e => e.IsDeleted)
                                .HasColumnName("IsDeleted")
                                .HasDefaultValueSql("((0))")
                                .IsRequired();

                entity.HasOne(e => e.ReportType)
                                .WithMany(e => e.CompanyUserReports)
                                .HasForeignKey(e => e.ReportTypeId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.CompanyUserCompound)
                                .WithMany(e => e.CompanyUserReports)
                                .HasForeignKey(e => e.CompanyUserCompoundId)
                                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            OnModelCreatingPartial(modelBuilder);

            //Push Notification Start
            modelBuilder.Entity<NotificationUser>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.HasOne(q => q.OwnerRegistration)
                    .WithMany(q => q.NotificationUsers)
                    .HasForeignKey(q => q.OwnerRegistrationId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(q => q.NotificationSchedule)
                    .WithMany(q => q.NotificationUsers)
                    .HasForeignKey(q => q.NotificationScheduleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificationSchedule>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();
            });

            modelBuilder.Entity<RegistrationForUser>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.RegisterId)
                    .IsRequired();

                entity.Property(e => e.RegisterType)
                    .HasMaxLength(50)
                    .HasComment("Device type (Android,IOS,...)")
                    .IsRequired();

                entity.HasOne(q => q.User)
                    .WithMany(q => q.RegistrationForUsers)
                    .HasForeignKey(q => q.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            //Push Notification End

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

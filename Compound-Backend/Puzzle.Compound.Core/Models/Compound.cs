using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class Compound
    {
        public Compound()
        {
            CompanyUserCompounds = new HashSet<CompanyUserCompound>();
            CompoundAds = new HashSet<CompoundAd>();
            CompoundAreas = new HashSet<CompoundArea>();
            CompoundCardPrintRequests = new HashSet<CompoundCardPrintRequest>();
            CompoundGates = new HashSet<CompoundGate>();
            CompoundGroups = new HashSet<CompoundGroup>();
            CompoundHelps = new HashSet<CompoundHelp>();
            CompoundCalls = new HashSet<CompoundCall>();
            CompoundInstructions = new HashSet<CompoundInstruction>();
            CompoundNearbyPlaces = new HashSet<CompoundNearbyPlace>();
            CompoundNews = new HashSet<CompoundNews>();
            CompoundNotices = new HashSet<CompoundNotice>();
            CompoundNotifications = new HashSet<CompoundNotification>();
            CompoundOwnerProperties = new HashSet<CompoundOwnerProperty>();
            CompoundSecurities = new HashSet<CompoundSecurity>();
            CompoundStores = new HashSet<CompoundStore>();
            VisitRequests = new HashSet<VisitRequest>();
            CompoundServices = new HashSet<CompoundService>();
            CompoundIssues = new HashSet<CompoundIssue>();
            CompoundReports = new HashSet<CompoundReport>();
        }

        public Guid CompoundId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string EmergencyPhone { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CompanyId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public string TimeZoneText { get; set; }
        public int TimeZoneOffset { get; set; }
        public string TimeZoneValue { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<CompanyUserCompound> CompanyUserCompounds { get; set; }
        public virtual ICollection<CompoundAd> CompoundAds { get; set; }
        public virtual ICollection<CompoundArea> CompoundAreas { get; set; }
        public virtual ICollection<CompoundCardPrintRequest> CompoundCardPrintRequests { get; set; }
        public virtual ICollection<CompoundGate> CompoundGates { get; set; }
        public virtual ICollection<CompoundGroup> CompoundGroups { get; set; }
        public virtual ICollection<CompoundHelp> CompoundHelps { get; set; }
        public virtual ICollection<CompoundCall> CompoundCalls { get; set; }
        public virtual ICollection<CompoundInstruction> CompoundInstructions { get; set; }
        public virtual ICollection<CompoundNearbyPlace> CompoundNearbyPlaces { get; set; }
        public virtual ICollection<CompoundNews> CompoundNews { get; set; }
        public virtual ICollection<CompoundNotice> CompoundNotices { get; set; }
        public virtual ICollection<CompoundNotification> CompoundNotifications { get; set; }
        public virtual ICollection<CompoundOwnerProperty> CompoundOwnerProperties { get; set; }
        public virtual ICollection<CompoundSecurity> CompoundSecurities { get; set; }
        public virtual ICollection<CompoundStore> CompoundStores { get; set; }
        public virtual ICollection<VisitRequest> VisitRequests { get; set; }
        public virtual ICollection<CompoundService> CompoundServices { get; set; }
        public virtual ICollection<CompoundIssue> CompoundIssues { get; set; }
        public virtual ICollection<CompoundReport> CompoundReports { get; set; }

    }
}

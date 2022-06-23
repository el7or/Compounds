using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Compounds
{
    public class DashboardInfoViewModel
    {
        public int AllUsersCount { get; set; }
        public int PendingUsersCount { get; set; }
        public int AllVisitsCount { get; set; }
        public int PendingVisitsCount { get; set; }
        public int AllServicesCount { get; set; }
        public int AllIssuesCount { get; set; }
        public int PendingServicesCount { get; set; }
        public int PendingIssuesCount { get; set; }
        public List<VisitsHistoryInScope> VisitsHistoryInScope { get; set; }
        public List<ServiceTypeStatuses> ServicesTypesCounts { get; set; }
        public List<ServiceTypeSubType> ServicesSubTypesCounts { get; set; }
        public List<IssueTypeStatuses> IssuesTypesCounts { get; set; }
    }

    public class VisitsHistoryInScope
    {
        public string Scope { get; set; }
        public int[] TypeCounts { get; set; }// [None, Once, Periodic, Labor, Group, Taxi, Delivery]
    }

    public class ServiceTypeStatuses
    {
        public string TypeEnglishName { get; set; }
        public string TypeArabicName { get; set; }
        public int[] StatusCounts { get; set; } // [Pending=0, Done=1, Canceled=2]
        public int[] RatesCounts { get; set; } // [1, 2, 3, 4, 5]
    }

    public class ServiceTypeSubType
    {
        public string TypeEnglishName { get; set; }
        public string TypeArabicName { get; set; }
        public List<ServiceSubTypeCount> ServiceSubTypesCounts { get; set; }
    }
    public class ServiceSubTypeCount
    {
        public string SubTypeEnglishName { get; set; }
        public string SubTypeArabicName { get; set; }
        public int SubTypeCount { get; set; }
    }

    public class IssueTypeStatuses
    {
        public string TypeEnglishName { get; set; }
        public string TypeArabicName { get; set; }
        public int[] StatusCounts { get; set; }// [Pending=0, Done=1, Canceled=2]
    }

    public enum ChartScope
    {
        Year = 1,
        Month = 2,
        Day = 3
    }
}

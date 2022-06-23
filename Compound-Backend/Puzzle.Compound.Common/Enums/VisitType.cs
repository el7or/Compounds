using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Puzzle.Compound.Common.Enums
{
    public enum VisitType
    {
        [Description("غير محدد")]
        None,

        [Description("مرة واحدة")]
        Once,

        [Description("دورية")]
        Periodic,

        [Description("عمالة")]
        Labor,

        [Description("مجموعة")]
        Group,

        [Description("تاكسي")]
        Taxi,

        [Description("دليفري")]
        Delivery
    }

    public enum VisitRequestType
    {
        Visit = 1,
        Owner,
        Card
    }

    public enum VisitStatus
    {
        [Description("مستهلكة")]
        Consumed = 1,

        [Description("قيد المراجعة")]
        Pending,

        [Description("تم التأكيد")]
        Confirmed,

        [Description("غير مؤكدة")]
        NotConfirmed,

        [Description("ملغية")]
        Canceled,

        [Description("منتهية الصلاحية")]
        Expired
    }
}

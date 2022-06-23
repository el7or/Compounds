using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models
{
    public partial class SystemPageAction
    {
        public SystemPageAction()
        {
            ActionsInCompanyRoles = new HashSet<ActionsInCompanyRoles>();
        }
        public Guid SystemPageActionId { get; set; }
        public string ActionArabicName { get; set; }
        public string ActionEnglishName { get; set; }
        public string ActionUniqueName { get; set; }
        public Guid SystemPageId { get; set; }

        public virtual SystemPage SystemPages { get; set; }

        public virtual ICollection<ActionsInCompanyRoles> ActionsInCompanyRoles { get; set; }
    }
}

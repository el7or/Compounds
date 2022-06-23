using System;

namespace Puzzle.Compound.Models.Issues
{
    public class IssueEvaluationModel
    {
        public Guid OwnerRegistrationId { get; set; }
        public string Comment { get; set; }
        public short IssueRate { get; set; }
        public short ProviderRate { get; set; }
    }
}

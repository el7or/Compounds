using System;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class ValidateVisitCodeModel
    {
        public string Code { get; set; }
        public Guid GateId { get; set; }
    }

    public class ValidateVisitResponse
    {
        public Guid VisitRequestId { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}

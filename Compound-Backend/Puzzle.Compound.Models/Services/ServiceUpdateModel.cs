using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Services
{
    public class ServiceUpdateModel
    {
        public Guid OwnerRegistrationId { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IFormFile Record { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public List<ServiceRequestSubTypeInPutViewModel> ServiceRequestSubTypes { get; set; }
    }
}

namespace SendNotification.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class RegisterationForUser
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        public string RegisterId { get; set; }

        public DateTime PostDate { get; set; }

        [Required]
        [StringLength(20)]
        public string RegisterType { get; set; }

        [StringLength(50)]
        public string IMEI { get; set; }

        public bool IsActive { get; set; }
    }
}

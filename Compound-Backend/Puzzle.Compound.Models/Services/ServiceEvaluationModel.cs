using System;

namespace Puzzle.Compound.Models.Services {
	public class ServiceEvaluationModel {
		public Guid OwnerRegistrationId { get; set; }
		public string Comment { get; set; }
		public short ServiceRate { get; set; }
		public short ProviderRate { get; set; }
	}
}

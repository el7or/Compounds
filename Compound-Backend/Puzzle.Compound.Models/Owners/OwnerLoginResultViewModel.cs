using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Models.Compounds;
using Puzzle.Compound.Models.Units;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Owners {
	public class OwnerLoginResultViewModel {
		public OwnerLoginResultViewModel() {

		}
		public OwnerLoginResultViewModel(OwnerStatus status) {
			Status = status;
		}
		public OwnerRegistrationType? UserType { get; set; }
		public Guid OwnerRegistrationId { get; set; }
		public OwnerStatus Status { get; set; }
		public List<UnitInfoViewModel> Units { get; set; } = new List<UnitInfoViewModel>();
		public List<CompoundInfo> Compounds { get; set; } = new List<CompoundInfo>();
		public object CompanySetting { get; set; }
		public object Token { get; set; }
		public LoginUserInfo UserInfo { get; set; }
	}

	public class LoginUserInfo {
		public string Email { get; set; }
		public string Name { get; set; }
		public int UserType { get; set; }
		public string Image { get; set; }
	}
}

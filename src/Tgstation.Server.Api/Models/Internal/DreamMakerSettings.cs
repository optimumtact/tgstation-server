﻿using System.ComponentModel.DataAnnotations;

namespace Tgstation.Server.Api.Models.Internal
{
	/// <summary>
	/// Represents the state of the DreamMaker compiler. Create action starts a new compile. Delete action cancels the current compile.
	/// </summary>
	public abstract class DreamMakerSettings
	{
		/// <summary>
		/// The name of the .dme file the server tries to compile with without the extension.
		/// </summary>
		[StringLength(Limits.MaximumStringLength)]
		[ResponseOptions]
		public string? ProjectName { get; set; }

		/// <summary>
		/// The port used during compilation to validate the DMAPI.
		/// </summary>
		[Required]
		[Range(1, 65535)]
		public ushort? ApiValidationPort { get; set; }

		/// <summary>
		/// The <see cref="DreamDaemonSecurity"/> level used to validate the DMAPI.
		/// </summary>
		[Required]
		public DreamDaemonSecurity? ApiValidationSecurityLevel { get; set; }

		/// <summary>
		/// If API validation should be required for a deployment to succeed.
		/// </summary>
		[Required]
		public bool? RequireDMApiValidation { get; set; }
	}
}

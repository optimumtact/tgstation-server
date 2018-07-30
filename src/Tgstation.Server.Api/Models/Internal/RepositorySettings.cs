﻿using System.ComponentModel.DataAnnotations;
using Tgstation.Server.Api.Rights;

namespace Tgstation.Server.Api.Models.Internal
{
	/// <summary>
	/// Represents configurable settings for a <see cref="Repository"/>
	/// </summary>
	[Model(RightsType.Repository, ReadRight = RepositoryRights.Read, RequiresInstance = true)]
	public class RepositorySettings
	{
		/// <summary>
		/// The origin URL. If <see langword="null"/>, the <see cref="Repository"/> does not exist
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.SetOrigin)]
		public string Origin { get; set; }

		/// <summary>
		/// The last commit recognized from <see cref="Origin"/>
		/// </summary>
		[Permissions(DenyWrite = true)]
		public string LastOriginCommitSha { get; set; }

		/// <summary>
		/// The name of the committer
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.ChangeCommitter)]
		[Required]
		public string CommitterName { get; set; }

		/// <summary>
		/// The e-mail of the committer
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.ChangeCommitter)]
		[Required]
		public string CommitterEmail { get; set; }

		/// <summary>
		/// The username to access the git repository with
		/// </summary>
		[Permissions(ReadRight = RepositoryRights.ChangeCredentials, WriteRight = RepositoryRights.ChangeCredentials)]
		public string AccessUser { get; set; }

		/// <summary>
		/// The token/password to access the git repository with
		/// </summary>
		[Permissions(ReadRight = RepositoryRights.ChangeCredentials, WriteRight = RepositoryRights.ChangeCredentials)]
		public string AccessToken { get; set; }

		/// <summary>
		/// If commits created from testmerges are pushed to the remote
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.ChangeTestMergeCommits)]
		[Required]
		public bool? PushTestMergeCommits { get; set; }

		/// <summary>
		/// If test merge commits are signed with the username of the person who merged it. Note this only affects future commits
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.ChangeTestMergeCommits)]
		[Required]
		public bool? ShowTestMergeCommitters { get; set; }

		/// <summary>
		/// If test merge commits should be kept when auto updating. May cause merge conflicts which will block the update
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.ChangeTestMergeCommits)]
		[Required]
		public bool? AutoUpdatesKeepTestMerges { get; set; }

		/// <summary>
		/// If synchronization should occur when auto updating
		/// </summary>
		[Permissions(WriteRight = RepositoryRights.ChangeTestMergeCommits)]
		[Required]
		public bool? AutoUpdatesSynchronize { get; set; }
	}
}

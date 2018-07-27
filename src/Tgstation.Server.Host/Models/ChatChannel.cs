﻿using Tgstation.Server.Api.Models;

namespace Tgstation.Server.Host.Models
{
	/// <inheritdoc />
	public sealed class ChatChannel : Api.Models.ChatChannel, IApiConvertable<Api.Models.ChatChannel>
	{
		/// <summary>
		/// The row Id
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// The <see cref="Api.Models.Internal.ChatSettings.Id"/>
		/// </summary>
		public long ChatSettingsId { get; set; }

		/// <summary>
		/// The <see cref="Models.ChatSettings"/>
		/// </summary>
		public ChatSettings ChatSettings { get; set; }

		/// <inheritdoc />
		public Api.Models.ChatChannel ToApi() => new Api.Models.ChatChannel
		{
			DiscordChannelId = DiscordChannelId,
			IsAdminChannel = IsAdminChannel,
			IsWatchdogChannel = IsWatchdogChannel,
			IrcChannel = IrcChannel
		};
	}
}

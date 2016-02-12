// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Mabi.Const;
using Aura.Mabi.Network;
using Aura.Shared.Util;
using Aura.Shared.Util.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Aura.Channel.Util
{
	public class ChannelConsoleCommands : ConsoleCommands
	{
		public ChannelConsoleCommands()
		{
            this.Add("shutdown", "Logs everyone out and shuts down server", HandleShutDown);
            this.Add("announce", "Sends an announcement to everyone", HandleAnnounce);
		}

        protected CommandResult HandleAnnounce(string command, IList<string> args)
        {
            if (args.Count < 2)
                return CommandResult.InvalidArgument;

            var message = string.Join(" ", args);
            var notice = Localization.Get("[Notice]") + " " + message.Substring(message.IndexOf(" "));

            var packet = new Packet(Op.Internal.BroadcastNotice, 0);
            packet.PutString(notice);
            ChannelServer.Instance.LoginServer.Send(packet);

            return CommandResult.Okay;
        }

        protected CommandResult HandleShutDown(string command, IList<string> args)
        {
            var players = ChannelServer.Instance.World.GetAllPlayers();
            
            foreach(var player in players)
            {
                var packet = new Packet(Op.RequestClientDisconnect, MabiId.Channel);
			    packet.PutByte(1);
                player.Client.Send(packet);
            }
            
            Log.Status("Players logged out. Disconnecting from login server.");
            ChannelServer.Instance.LoginServer.Kill();
            
            CliUtil.Exit(0, true);

            return CommandResult.Okay;
        }

		protected override CommandResult HandleStatus(string command, IList<string> args)
		{
			var result = base.HandleStatus(command, args);
			if (result != CommandResult.Okay)
				return result;

			var creatures = ChannelServer.Instance.World.GetAllCreatures();

			Log.Status("Creatures in world: {0}", creatures.Count);
			Log.Status("Players in world: {0}", creatures.Count(a => a.IsPlayer));

			return CommandResult.Okay;
		}
	}
}

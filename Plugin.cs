using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using SharedLibraryCore;
using SharedLibraryCore.Interfaces;
using SharedLibraryCore.Configuration;
using SharedLibraryCore.Services;
using SharedLibraryCore.Database.Models;
using System.Linq;
using SharedLibraryCore.Database;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using static SharedLibraryCore.Database.Models.EFClient;

namespace IW4MAdmin.Plugins.Welcome
{
    public class Plugin : IPlugin
    {
        public string Author => "efrenbg1";

        public float Version => 1.0f;

        public string Name => "Votemap plugin";

        public async Task OnLoadAsync(IManager manager)
        {
            Console.WriteLine("Votemap loaded");
        }

        public Task OnUnloadAsync() => Task.CompletedTask;

        public Task OnTickAsync(Server S) => Task.CompletedTask;

        int currentMap = 0;
        int nextMap = 0;
        int selectedMap = 0;
        bool changedMap, changing = false;
        Dictionary<int, bool> votes = new Dictionary<int, bool>();
        List<int> changenow = new List<int>();
        public async Task OnEventAsync(GameEvent E, Server S)
        {
            if (S.IsZombieServer()) return;
            if (E.Type == GameEvent.EventType.Join && !changedMap)
            {
                await salute(E.Origin, S);
            }
            else if (E.Type == GameEvent.EventType.MapChange && !changing)
            {
                changing = true;
                await Task.Delay(6000);
                await countvotes(S);
                currentMap = S.Maps.FindIndex(m => m.Name == S.CurrentMap.Name);
                if (!changedMap && currentMap != selectedMap)
                {
                    S.Broadcast("^3Changing map now!");
                    changedMap = true;
                    await S.LoadMap(S.Maps[selectedMap].Name);
                }
                else
                {
                    await pickmap(S);
                    votes = new Dictionary<int, bool>();
                    changenow = new List<int>();
                    S.Broadcast("^3Vote next map now!");
                    S.Broadcast("^5#0 ^7-> ^2Current map");
                    S.Broadcast("^5#1 ^7-> ^2" + S.Maps[nextMap]);
                    S.Broadcast("^5#c ^7-> ^2Vote to change map now");
                    changedMap = false;
                }
                changing = false;
            }
            else if (E.Type == GameEvent.EventType.Say)
            {
                int index = E.Data.IndexOf("#");
                if (index > -1)
                {
                    await vote(E.Origin, S, E.Data[index + 1]);
                }
            }
            else if (E.Type == GameEvent.EventType.MapEnd)
            {
                await countvotes(S);
            }
        }

        public async Task salute(EFClient player, Server S)
        {
            bool alreadyVoted = false;
            foreach (KeyValuePair<int, bool> entry in votes)
            {
                if (player.AliasLinkId == entry.Key)
                {
                    alreadyVoted = true;
                    if (entry.Value)
                    {
                        player.Tell("^2You've voted: ^5" + S.Maps[nextMap]);
                    }
                    else
                    {
                        player.Tell("^2You've voted: ^5Current map");
                    }
                    player.Tell("^3You can still change your vote:");
                    break;
                }
            }
            if (!alreadyVoted) player.Tell("^3Vote next map now!");
            player.Tell("^5#0 ^7-> ^2Current map");
            player.Tell("^5#1 ^7-> ^2" + S.Maps[nextMap]);
            player.Tell("^5#c ^7-> ^2Vote to change map now");
        }

        public async Task pickmap(Server S)
        {
            Random rnd = new Random();
            currentMap = S.Maps.FindIndex(m => m.Name == S.CurrentMap.Name);
            nextMap = rnd.Next(1, S.Maps.Count) - 1;
            while (nextMap == currentMap || S.Maps[nextMap].Name.IndexOf("zm_") > -1)
            {
                nextMap = rnd.Next(1, S.Maps.Count) - 1;
            }
        }

        public async Task vote(EFClient player, Server S, char choice)
        {
            if (choice == '0')
            {
                player.Tell("^2You've voted: ^5Current map");
                votes[player.AliasLinkId] = false;
            }
            else if (choice == '1')
            {
                player.Tell("^2You've voted: ^5" + S.Maps[nextMap]);
                votes[player.AliasLinkId] = true;
            }
            else if (choice == 'c')
            {
                player.Tell("^2You've voted: ^5change map now");
                changenow.Add(player.AliasLinkId);
                if (changenow.Count >= S.ClientNum - 1)
                {
                    await countvotes(S);
                    changedMap = true;
                    S.Broadcast("^6Changing map now!");
                    await S.LoadMap(S.Maps[selectedMap].Name);
                }
            }
        }

        public async Task countvotes(Server S)
        {
            if (votes.Count == 0)
            {
                selectedMap = nextMap;
                return;
            }
            int change = 0;
            foreach (KeyValuePair<int, bool> entry in votes)
            {
                if (entry.Value)
                {
                    change++;
                }
            }
            if (change > (votes.Count - change))
            {
                selectedMap = nextMap;
            }
            else
            {
                selectedMap = currentMap;
            }
        }
    }
}

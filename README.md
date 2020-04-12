# IW4MAdmin - Vote map plugin (chat)
 This plugin is designed to extend the functionality of [**IW4MAdmin**](https://github.com/RaidMax/IW4M-Admin) created by [RaidMax](https://github.com/RaidMax "RaidMax").
 
  ### Download
You can download the .dll file from the releases section, and place it inside the Plugins folder:
- [GitHub](https://github.com/efrenbg1/iw4m-votemap/releases)


 ### About
 This plugin tries to imitate the original behavior of lobbies. This is the first version and I plan to extend its functionality. For now, its been tested and works only with **one Black Ops 2 Multiplayer** server. No configuration needed though.
It shows three options for players to vote:
- **"#0"** vote for current map
- **"#1"** vote for a random map (choosen at round start, like the original lobby would work)
- **"#c"** vote to change map now (all players -1 are requiered to vote)

### Important notes
- As I could not find any way to detect the killcam, the plugin waits for the server to rotate to the next map, and then changed the map to the most voted.
- Votes can be made during the entire game.
- Votes are saved if a player disconnects. If the player reconnects, the plugin will remember the player his original vote.


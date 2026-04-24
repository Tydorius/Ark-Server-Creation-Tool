# Ark-Server-Creation-Tool

Ark Server Creation Tool (ASCT) - A tool for hosting your own Ark dedicated server, supporting both Ark: Survival Ascended (ASA) and Ark: Survival Evolved (ASE).

If you have any questions join the discord https://discord.gg/RspMUPqjaJ

This tool automates the downloading, installation, and management of Ark dedicated servers. Configure your server through the GUI, download the game files, and launch it -- no manual batch files or command-line arguments needed.

Requirements:
 - .NET 9
 - Windows (the tool manages Windows Firewall rules and uses DepotDownloader for Steam game files)

# Supported Games

| Feature | Ark: Survival Ascended | Ark: Survival Evolved |
|---|---|---|
| Steam App ID | 2430930 | 376030 |
| Executable | ArkAscendedServer.exe | ShooterGameServer.exe |
| Maps | TheIsland_WP, ScorchedEarth_WP, TheCenter_WP, Aberration_WP, Extinction_WP, Astraeos_WP, Ragnarok_WP, Valguero_WP, LostColony_WP | TheIsland, ScorchedEarth_P, TheCenter, Aberration_P, Extinction, Ragnarok, Valguero_P, Genesis1, Genesis2, CrystalIsles, LostIsland, Fjordur |
| Crossplay Flag | -ServerPlatform=ALL | -crossplay |
| Player Count | -WinLiveMaxPlayers=N | ?MaxPlayers=N (in map URL) |

# Instructions

You will need a Windows PC, Dedicated Server, VPS, or VDS with a minimum of 15GB RAM (20GB recommended to allow room for bases and players).
A blank Ark server will consume roughly 10-11GB of RAM, so plan accordingly if running multiple maps.

The tool will download the server files via DepotDownloader and place them in the install directory you specify.

1. Download the tool using the latest zip https://github.com/Ragonz/Ark-Server-Creation-Tool/releases/latest
2. Extract the tool to a desired location. It's recommended that you keep this in its own folder for simplicity. If you have an existing installation of ASCT, you can replace that version with the new one.
   ![Screenshot 2024-04-26 190818](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/f38390b8-1051-4c26-946f-65dea0431d85)
3. Launch `ARK Server Creation Tool.exe` to open the tool.
4. On first launch, you'll be presented with a settings window. If you need to run the server on a different drive from the tool, you can change the default paths now. If you're happy with everything on the same drive, the default paths should work fine. It's recommended you leave the port settings as their defaults.
   ![Screenshot 2024-04-26 190900](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/2382cda6-68d7-411a-aaff-cc5769036eb5)

   Press Save to continue.
5. The server list page will then open. If this is a fresh install, no servers will be present. If you upgraded an existing ASCT install, the existing server should be detected and displayed.
   ![Screenshot 2024-04-26 190917](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/0dfb2d6b-8653-40f2-8e96-c88f38de1f0b)

6. To create a server, press the + button.
7. The server configuration window will open. On this window, you can give your server a name, select the game version (ASA or ASE), choose a map, and tell the tool where to install it. The name is only used within the tool for identification purposes.
   
   Click Save to create the new server.
8. The server updater will now open. This is how the game files are downloaded. Ensure the newly created server is selected in the list and then click the "Update Game Files" button.
    ![Screenshot 2024-04-26 190942](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/75ee4c1c-efe9-4259-9ba3-7366ba9cdc2b)
9. If this is a new installation of the tool and no DepotDownloader is detected, the tool will ask if you'd like to download it. Click Yes.
    ![Screenshot 2024-04-26 190948](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/cb28f042-fd90-4848-ae7a-c91d2d99797f)
10. Once DepotDownloader has been downloaded and extracted, the tool will launch it to download the files to the correct location.
    ![Screenshot 2024-04-26 191010](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/6f7891ad-9d8b-4cd3-b432-cdfb5711c4cc)
11. The DepotDownloader console will close and the tool will confirm the update is complete. You can now close the updater window.
    ![Screenshot 2024-04-26 191717](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/f6105e27-d40d-4833-b3e9-166737209ada)
12. The server will now display in the server list window. Select it in the list and click "View Server".
    ![Screenshot 2024-04-26 191723](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/30ebd363-f5ac-4274-bf55-51e8dea4dff3)
13. You will now be presented with the server view window. This gives you the option to launch the server, access the config within ASCT, or open the main game config files (GameUserSettings.ini and Game.ini). Press "Start Server" to launch the game server.
    ![Screenshot 2024-04-26 192051](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/4c62cd1d-a3b7-40ac-a21b-ecbcd0c41e9d)
14. After clicking Start Server, the tool will run the game server and if required, create a firewall rule for you. Once the game has fully started, you should be able to join it in game.
    ![Screenshot 2024-04-26 192103](https://github.com/Ragonz/Ark-Server-Creation-Tool/assets/12957193/191f77e9-786d-41b0-ac7d-8edc64caae14)

# Features

- **Multi-server management** -- Configure and run multiple Ark servers from a single tool. Each server gets its own game port, config, and process controls.
- **Cluster support** -- Group servers into a cluster with shared save data for character transfers between maps.
- **Auto-detection of game files** -- Uses DepotDownloader to fetch the correct server files from Steam for each game version.
- **Server configuration GUI** -- Set map, ports, player count, mods, crossplay, BattlEye, active events, and custom launch arguments through a configuration window.
- **Automatic firewall rules** -- Creates Windows Firewall inbound rules for each server executable on first launch.
- **Config file management** -- Create and edit GameUserSettings.ini and Game.ini from template, with game-appropriate defaults for ASA and ASE.
- **Automatic updates** -- Check for and download server file updates via the built-in updater.
- **Automatic server start** -- Optionally start servers automatically when the tool launches.

# Local Play (ASE + Wi-Fi)

If you are hosting an ASE server on the same machine you play on and connecting over Wi-Fi, you may encounter a situation where you cannot connect to your own server. This happens because the server process binds to the wrong network adapter (for example, an unused ethernet port) and advertises an APIPA address (169.254.x.x) instead of your real LAN IP.

The Local Play option in Global Configuration addresses this by:
1. Automatically enabling the Multihome parameter with your Wi-Fi adapter's IP address
2. Temporarily setting that adapter's interface metric to priority 1 so the server binds to it

The original adapter metric is restored when the server stops. If the tool closes unexpectedly, it will offer to restore the metric on next launch.

This feature is only needed for ASE servers over Wi-Fi. ASA servers and ethernet-connected hosts do not typically require this workaround.

To enable it: open Global Configuration (gear icon or first-launch window), check the "Local Play" box, and confirm the detected IP address.

# Connecting to Your Server

**From another machine on your network or via the internet:** Open the in-game server browser and search for your server name, or use the Steam server browser to add your public IP and query port (default 27015).

**From the same machine (local play):** If hosting and playing on the same PC, use the console command `open <LAN_IP>:<port>` (for example, `open 192.168.1.100:7777`). Using `open 127.0.0.1` will not work for ASE servers.

# Notes on Port Forwarding

Port forwarding is required if you want players outside your local network to connect. If you only intend to play with others on the same LAN, port forwarding is not needed.

If you don't want to manage port forwarding yourself, consider using a VPS. Make sure it has a decent CPU speed and enough RAM (approximately 10GB per map). We don't have specific recommendations, but https://pingperfect.com/ has been used previously and worked fine.

If the tool helped you and you want to help fund its development, consider subscribing to our Patreon
https://www.patreon.com/Ragonz

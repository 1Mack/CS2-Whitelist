# CS2 Whitelist
Plugin for CS2 that creates a whitelist or blacklist system.

## Installation
1. Install **[CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)** and **[Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)**;
3. Download **[CS2-Whitelist](https://github.com/1Mack/CS2-Whitelist/releases)**;
4. Unzip the archive and upload it into **`csgo/addons/counterstrikesharp/plugins`**;

## Config
The config is created automatically. ***(Path: `csgo/addons/counterstrikesharp/configs/plugins/Whitelist`)***
```
{
  "UseDatabase": true, // true = use mysql; false = use .txt file
  "KickIfFailed": false, // If enabled, the player will be kicked when an error occurs during verification
  "UseAsBlacklist": true, // false = use as Whitelist; true = use as blacklist
  "SendMessageOnChatAfterKick": true,
  "Database": {
    "Host": "",
    "Port": 3306,
    "User": "",
    "Password": "",
    "Name": "",
    "Prefix": "whitelist"
  },
  "Commands": {
    "Add": "wladd", // Add a player to the list
    "AddPermission": "@css/root",
    "Remove": "wlremove", // Remove a player from the list
    "RemovePermission": "@css/root"
  },
  "ConfigVersion": 1
}
```

## Commands
- **`css_wladd [value] [value2] [value3]...`** - Add a player to the list; **(`@css/root` flag is required for use)**
- **`css_wlremove [value] [value2] [value3]...`** - Remove a player from the list; **(`@css/root` flag is required for use)**

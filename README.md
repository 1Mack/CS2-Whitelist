# CS2 Whitelist

Plugin for CS2 that creates a whitelist or blacklist system.
You can use Database or .txt file

## Installation

1. Install **[CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)** and **[Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)**;
2. Download **[CS2-Whitelist](https://github.com/1Mack/CS2-Whitelist/releases)**;
3. Unzip the archive and upload it into **`csgo/addons/counterstrikesharp/plugins`**;

## Config

The config is created automatically. ***(Path: `csgo/addons/counterstrikesharp/configs/plugins/Whitelist`)***

```json
{
  "UseDatabase": true, // true = use mysql; false = use .txt file
  "KickIfFailed": false, // If enabled, the player will be kicked when an error occurs during verification
  "UseAsBlacklist": false, // false = use as Whitelist; true = use as blacklist
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
  "CheckIfMemberIsInGroup": false,
  "Apikey": "", //Steam Api Key - Only required when CheckIfMemberIsInGroup = true
  "ServerID": 1, // -1 or 0 = disable. Increase this number on each server
  "ConfigVersion": 3
}
```

## Commands

- **`css_wladd [value] [value2] [value3]...`** - Add a player to the list; **(`@css/root` flag is required for use)**
- **`css_wlremove [value] [value2] [value3]...`** - Remove a player from the list; **(`@css/root` flag is required for use)**

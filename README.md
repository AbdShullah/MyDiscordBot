![.NET](https://github.com/AbdShullah/MyDiscordBot/workflows/.NET/badge.svg)

# MyDiscordBot
 
Sample config.json
```json
{
  "DiscordOptions": {
    "Token": "Your bot token",
    "Prefix": "!"
  },
  "ConnectionStrings": {
    "Default": "Data Source=db.db;"
  }
}
```
Sample logging.json
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log.log",
          "rollingInterval": "Day",
          "buffered": true
        }
      }
    ]
  }
}
```

# AniDBApi

## Features

- [x] [Rate Limiting](#rate-limiting)
- [x] [Data Dumps](#data-dumps)
- [x] [HTTP](#http)
- [ ] [UDP](#udp)

## Rate Limiting

All endpoints that require rate limiting use a custom rate limiter (see [RateLimiter.cs](AniDBApi/RateLimiter.cs)) and the library actively enforces the limit.

## Data Dumps

All Data Dumps defined [here](https://wiki.anidb.net/API#Data_Dumps) have been implemented:

- Anime Titles (XML)

You should only get the dumps once per day which the library enforces through a [Rate Limiter](#rate-limiting).

## HTTP

All of the Data Commands defined [here](https://wiki.anidb.net/HTTP_API_Definition) have been implemented:

- Anime
- Random Recommendation
- Random Similar
- Hot Anime
- Main

The interface [`AniDBApi.IHttpApi`](AniDBApi/IHttpApi.cs) is implemented by [HttpApi](AniDBApi.HTTP/HttpApi.cs). All functions return the immediate result of the Api call as a string. I decided against creating DTO classes because you will not end up using even half of the available properties. I highly recommend using the [XmlReader](https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader) for fast, non-cached, forward-only access to the XML data.

## UDP

Supported Version: `0.03.730 (2015-03-25)`

Commands implemented from the [docs](https://wiki.anidb.net/UDP_API_Definition):

- Authing Commands:
  - [x] `AUTH`: encoding is set to UTF-8
  - [x] `LOGOUT`: you actually can't logout when you get banned after the login, the API will return nothing and will time out
  - [x] `ENCRYPT`: authentication is a joke in this API, without this command you have to transmit your username and password as clear text, highly recommend you always use an encrypted session even if the docs says you should not "In order to minimize server load"...
- Notify Commands:
  - [ ] `PUSH`
  - [ ] `NOTIFY`
  - [ ] `NOTIFYLIST`
  - [ ] `NOTIFYGET`
  - [ ] `NOTIFYACK`
  - [ ] `PUSHACK`
- Notification Commands:
  - [x] `NOTIFICATIONADD`
  - [x] `NOTIFICATIONDEL`
- Buddy Commands:
  - [x] `BUDDYADD`
  - [x] `BUDDYDEL`
  - [x] `BUDDYACCEPT`
  - [x] `BUDDYDENY`
  - [x] `BUDDYLIST`
  - [x] `BUDDYSTATE`
- Data Commands:
  - [x] `ANIME`: the `amask` from the docs is incomplete/outdated, also searching by name is completely broken as well
  - [x] `ANIMEDESC`
  - [x] `CALENDAR`
  - [x] `CHARACTER`
  - [x] `CREATOR`
  - [x] `EPISODE`
  - [x] `FILE`: the server is very trigger happy when you use this command, I got banned while adding support for this...
  - [x] `GROUP`
  - [x] `GROUPSTATUS`
  - [x] `UPDATED`
- MyList Commands:
  - [ ] `MYLIST`
  - [ ] `MYLISTADD`
  - [ ] `MYLISTDEL`
  - [ ] `MYLISTSTATS`
  - [ ] `VOTE`
  - [ ] `RANDOM`
- Misc Commands:
  - [ ] `MYLISTEXPORT`
  - [x] `PING`: does not require a session
  - [x] `VERSION`: does not require a session
  - [x] `UPTIME`: does not require a session
  - [ ] `ENCODING`: will never be implemented, encoding is already set to UTF-8 in the `AUTH` Command
  - [x] `SENDMSG`
  - [x] `USER`: does not require a session

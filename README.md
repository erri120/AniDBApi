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

TODO

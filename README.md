# FunWithGBFS

**FunWithGBFS** is a modular and extensible quiz game powered by real-time GBFS (General Bikeshare Feed Specification) data. It challenges users to answer questions about bike station and vehicle availability using live data from public bike-sharing systems around the world.

---

## Features

- Connects to live GBFS endpoints for bike station and vehicle data
- Generates multiple-choice quiz questions about station or vehicle availability
- Asynchronous game timer with cancellation and event support
- Modular, testable architecture using dependency injection and interfaces
- Clean separation of concerns: data, logic, and UI layers
- Console-based UI with future support for Web and Mobile

---

## Build and Run Instructions

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- IDE or editor of your choice (Visual Studio, Rider, or VS Code)

### Build

```bash
git clone https://github.com/khalimovaelena/FunWithGBFS.git
cd FunWithGBFS
dotnet restore
dotnet build

### Run
```bash
dotnet run

Configuration is handled via appsettings.json . Ensure the GBFS URLs are valid and accessible.

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
```

### Run
```bash
dotnet run
```

Configuration is handled via [appsettings.json](https://github.com/khalimovaelena/FunWithGBFS/blob/main/FunWithGBFS/Config/appsettings.json) . Ensure the GBFS URLs are valid and accessible.

## Architecture Overview

### User Interface
- Console app as the default UI
- Separated from game logic to enable future Web, Mobile, or API-based UIs

### Game Engine
- Coordinates quiz flow: question generation, timer, score tracking, and result handling

### Question Generation
- Uses GBFS station and vehicle data to produce quiz questions
- Implemented via IQuestionGenerator interface for easy extension

### Data Providers
- Interfaces like IStationProvider and IVehicleProvider abstract GBFS fetching
- Data fetched using HttpClient and parsed as JSON

### Mappers
- Translate GBFS feed JSON directly to internal models (Station, Vehicle, etc.)

### Logging
- All output is handled via ILogger or IUserInteraction

### Timer
- GameTimer handles async countdowns with cancellation and tick/expiration events

## Tech Stack and Rationale
| Component        | Choice                       | Rationale                                                   |
|------------------|------------------------------|-------------------------------------------------------------|
| Runtime          | .NET 8                       | Modern, cross-platform, high-performance, great async model |
| Language         | C#                           | Object oriented, strong typing, async-friendly              |
| UI               | Console App                  | Lightweight, easy to prototype and test                     |
| Logging          | Microsoft.Extensions.Logging | Standardized, pluggable logging abstraction                 |
| Data Fetching    | HttpClient + System.Text.Json| Fast, low-overhead, ideal for JSON APIs                     |
| Architecture     | DI, Interfaces, Async/Await  | Promotes testability, flexibility, and clean separation     |

## Future Improvements
### 1. Token Authentication for Users
- Add JWT or OAuth2-based authentication
- Track users securely with token-based session management
- Enable user registration, login, and roles (e.g., admin/player)

### 2. API-first Architecture
- Add REST API for game engine and questions generating to be able to connect to them from various UIs: web, mobile, chatbots, external websites, etc.
- Enable support for:
	- Web apps
	- Mobile apps
	- Chatbot interactions
- Allow multiple clients to connect concurrently (multiplayer mode)
(\* Right now concurrency is achieved by running multiple instances of the console app)

### 3. Store Questions in a Database
- Save every generated question and user response
- Enable auditing, performance tracking, and analytics
- Useful for teachers, researchers, and competitive leaderboards

### 4. Parameter-Driven Question Generating Platform
- Create a table in DB or JSON configuration to define question-generation rules
- Parameters include:
	- Location (city/region)
	- Question text
	- Question type (single/multiple choice, true/false etc))
	- Data input type (station/vehicle etc)
	- Thresholds (what to calculate: min/max/avg etc)
	- Which fields in external API relate to which data of the question (where to read number of bikes, disabled/reserved bikes etc from)
- Code basics of the logic: how to calculate Thresholds and how to generate options for single/multiple choice questions
- Logic will dynamically read and apply these parameters (from DB ot json) without code changes > this will allow to create new questions without code changes

### 5. Persistent Storage 
- Replace in-memory storage with a real DB (PostgreSQL, SQLite)

### 6. More Unit Tests 
- Add tests for GameEngine, Data Providers etc

### 7. Multiplayer Support 
- Implement real-time multiplayer quiz sessions

### 8. Localization
- Add multi-language support

### 9. Admin Dashboard
- For adding rules, managing questions, or viewing game statistics

### 10. More fun features for gamers
- Leaderboards, achievements, and social sharing
- Profile customization and user settings
- Perks for top players (e.g., discounts on bike rentals, etc.)
- Nice UI and voiceover for questions and answers
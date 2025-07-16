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

## Architecture Overview and Rationale
I was trying to stick to Clean architecture principles for this project to ensure that solution is scalable, maintanable, testable and will be easy to improve in the future.
Architecture layers and classes related to them are shown in the picture below:
![Architecture](./Images/FunWithGBFS_Architecture.jpg)

### Presentation Layer
I chose Console terminal as default user interface, because it is simple and doesn't require any additional setup and external dependencies.
At the same time Presentation layer is separated from the game logic, which allows changing UI in the future to Web, Mobile, or API-based UI (that can be used by chatbots).

### Application Layer

#### 1. Game logic
Contains the main game logic (GameEngine), necessary settings (GameSettings), asynchronous timer (GameTimer) and ScoreManager for tracking user scores.
GameTimer allows to start one async timer per game (console) instance, which allows to run multiple games concurrently in different console windows.
Main game settings are configurable via appsettings.json and GameSettings class that allows to change game parameters without code changes.

#### 2. Question generation
Contains general interface IQuestionGenerator that allows to generate different kinds of questions and add them to the game without changing much in the code.
To add new question, just implement IQuestionGenerator interface and add it to Program.cs.
Right now all questions are hardcoded (each question is represented by a class that implements IQuestionGenerator). This makes the implementation simple and should be enough for the test project. It is possible to make the implementation more flexible and scalable by extracting the hardcoded questions and their processing rules to an app configuration, which is described a bit more in the Future Improvements section below.

#### 3. Startup and Dependency Injection
Application configuration and dependency injection handling is separated from Program.cs into AppConfigurator and ServiceConfigurator classes to decouple it from the game logic.

### Infrastructure Layer

#### Data Providers
Use HttpClient to fetch live GBFS data from public bike-sharing systems via REST APIs.

#### Data Mappers
Translate deserealized JSON provided by respective provider directly to internal models (Station, Vehicle, etc.) that can be used in question generators to create questions and options.

### Persistence Layer
Contains GameDBContext and Repositories (currently only UserRepository) to store data. 
As this is a test project and it doesn't need to have sophisticated data storing or processing, SQLLite DB should be sufficient for the task. It is a lightweight database that can be instantiated on the app startup. For production uses it can be replaced with full-featured DB like PostgreSQL, Oracle, MS SQL, etc.

### Domain Layer
Contains domain models (Station, Vehicle, Question, Option) that represent the core data structures used in the game. 

### Logging
Microsoft.Extensions.Logging package is used for logging throughout the application. 
It provides a standardized way to log messages and errors without using external dependencies and doesn't need extra configuration.

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
- Add full concurrent mode support to allow users to use one instance of the app in multiplayer mode.
*(\* Right now concurrency is achieved by running multiple instances of the console app)**

### 3. Improved logging and audit
For production it might require more detailed audit logs, for example, to be able to trace the data objects mutations for issues investigations or audit or analytics reports. 

### 4. Question Generating Platform
To eliminate hardcoded questions text and make the questions generation part more flexible and scalable, the way that it can be a configuration that can be updated without the app release, the following changes can be implemented:
	- Store question texts/templates in database. This will allow to update them without a code change (an admin UI could be used for this in future).
 	- Define question generation rules: every rule might be described by a set of parameters, e.g.:
		- Location (city/region)
		- Question text
		- Question type (single/multiple choice, true/false etc))
		- Data input type (station/vehicle etc)
		- Thresholds (what to calculate: min/max/avg etc)
		- Mapping of external APIs attributes to a question's data (where to read number of bikes, disabled/reserved bikes etc from)
	- Some basic logic will be still coded but can be re-used in different rules, e.g.: 
		- how to calculate Thresholds (max, min, average values etc) 
		- how to generate options for single/multiple choice questions
As a result, game admins will be able to update rules without code changes and Game logic will dynamically read and apply these rules to generate new questions.
This approach implies more sophisticated implementation and will require to cover more edge cases which is out of the scope of this assignment. 

### 5. Persistent Storage 
- Replace SQLLite with full-featured DB (PostgreSQL, Oracle, MS SQL) to be able to create users roles (admin/player), store statistics and provide more advance analytics.

### 6. More Unit Tests 
- Add tests for GameEngine, Data Providers etc.

### 7. Multiplayer Support 
- Implement real-time multiplayer quiz sessions

### 8. Localization
- Add multi-language support

### 9. Admin Dashboard
- For adding rules, managing questions, or viewing game statistics

### 10. More fun features for gamers
- Leaderboards, achievements and social sharing
- Profile customization and user settings
- Perks for top players (e.g., discounts on bike rentals, etc.)
- Nice UI and voiceover for questions and answers

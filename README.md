# 🎵 Songs Playlist Manager

A console-based **Song & Playlist Management System** built in C# that lets users create songs, organize them into playlists, persist data to text files, and store/retrieve records from a SQL Server database.

## 📌 About

Songs Playlist Manager is a menu-driven console application demonstrating core object-oriented programming concepts (classes, constructors, encapsulation), file I/O, collections, and ADO.NET database operations in C#. It's designed as a practical example of managing a music library — adding songs, tracking play counts and likes, organizing playlists by genre, and syncing data between local text files and a SQL Server backend.

## ✨ Features

- **Song Management** — Create songs with title, artist, genre, duration, like status, and play count
- **Playlist Management** — Create playlists with a name and max song capacity, add multiple songs, and view aggregate statistics (total songs, total duration, liked count)
- **Genre Search** — Find all songs in a playlist matching a specific genre
- **Play Tracking** — Play a song and automatically increment its play count
- **File Persistence** — Save songs to a text file (`songs.txt`) and reload them later
- **Database Integration** — Insert songs into a SQL Server database, fetch all songs, look up a song by ID, and get the total song count
- **Interactive Menu** — Simple numbered menu (1–13) to drive all operations from the console

## 🛠️ Tech Stack

- **Language:** C# (.NET)
- **Data Access:** ADO.NET (`System.Data.SqlClient`)
- **Database:** Microsoft SQL Server
- **Storage:** Local text file (CSV-style) for offline persistence

## 📂 Project Structure

```
SongsPlaylist/
├── Song.cs              // Song entity (properties + behavior)
├── FileManager.cs       // Load/save songs to/from a text file
├── PlaylistManager.cs   // Playlist creation, song grouping, statistics
├── SongDataBase.cs      // SQL Server CRUD operations
├── Program.cs           // Main menu loop and application entry point
└── songs.txt            // Generated data file (created at runtime)
```

> Note: In the current version all classes live in a single `.cs` file; splitting them as above is recommended for maintainability.

## 🗄️ Database Setup

The app expects a SQL Server database named `SongDatabase` with a `SONGS` table. Example schema:

```sql
CREATE DATABASE SongDatabase;
GO

USE SongDatabase;
GO

CREATE TABLE SONGS (
    SongID     INT IDENTITY(1,1) PRIMARY KEY,
    Title      NVARCHAR(200),
    Artist     NVARCHAR(200),
    Duration   FLOAT,
    Genre      NVARCHAR(100),
    isLiked    BIT,
    playCount  INT
);
```

Update the connection string in `SongDataBase.cs` if your server name or authentication method differs:

```csharp
private string connectionString = "Server=localhost;Database=SongDatabase;Trusted_Connection=True";
```

## ▶️ Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) installed
- SQL Server (LocalDB, Express, or full instance) running locally
- `System.Data.SqlClient` NuGet package

### Run the project

```bash
git clone https://github.com/<your-username>/songs-playlist-manager.git
cd songs-playlist-manager
dotnet restore
dotnet run
```

### Using the App

On launch, you'll see a menu:

```
1. Create Songs
2. Display All Songs
3. Save Songs to Text File
4. Load Songs from Text File
5. Insert Songs into Database
6. Display All Songs from Database
7. Get Song by ID (Database)
8. Get Total Song Count (Database)
9. Create Playlist
10. Display Playlist Statistics
11. Find Songs by Genre
12. Play a Song
13. Exit
```

Enter the number corresponding to the action you want to perform, and follow the on-screen prompts.

## 🚧 Known Limitations / Roadmap

- Song creation currently requires entering exactly 5 songs at a time
- No input validation for empty fields or invalid numeric entries
- No update/delete operations for the database (`Insert`/`Read` only)

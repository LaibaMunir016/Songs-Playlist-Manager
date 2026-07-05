using System.Runtime.Intrinsics.X86;
using System.IO;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Security.Principal;
using System.Data.SqlClient;
//1
namespace SongsPlaylist
{
    internal class Song
    {
        public int SongId { get; set; }
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public double Duration { get; set; }
        public string? Genre { get; set; }
        public bool isLiked { get; set; }
        public int playCount { get; set; }

        public Song()
        {
            isLiked = false;
            playCount = 0;
        }
        public Song(int id, string title, string artist, double duration, string genre, bool liked = false, int playcount = 0)
        {
            SongId = id;
            Title = title;
            Artist = artist;
            Duration = duration;
            Genre = genre;
            isLiked = liked;
            playCount = playcount;
        }
        public void playSong()
        {
            playCount++;
        }
        public void displaySongInfo()
        {
            Console.WriteLine("Song Info:\n");
            Console.WriteLine("ID: " + SongId);
            Console.WriteLine("Title: " + Title);
            Console.WriteLine("Artist: " + Artist);
            Console.WriteLine("Duration: " + Duration);
            Console.WriteLine("Genre: " + Genre);
            Console.WriteLine("Liked: " + isLiked);
            Console.WriteLine("PlayCount: " + playCount);
            Console.WriteLine();

        }
    }
    internal class FileManager
    {
        public List<Song> LoadSongsFromText(string filename)
        {
            FileStream fout = new FileStream(filename, FileMode.Open);
            StreamReader streamReader = new StreamReader(fout);
            string? line = String.Empty;
            List<Song> song = new List<Song>();
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] data = line.Split(',');
                Song songss = new Song
                {
                    SongId = int.Parse(data[0]),
                    Title = data[1],
                    Artist = data[2],
                    Duration = double.Parse(data[3]),
                    Genre = data[4],
                    isLiked = bool.Parse(data[5]),
                    playCount = int.Parse(data[6])

                };
                song.Add(songss);

            }
            streamReader.Close();
            fout.Close();
            return song;
        }
        public void SaveSongsToText(List<Song> songs, string filename)
        {
            if (songs.Count == 0){
                Console.WriteLine("No songs to save");
                return;
            }
            FileStream file = new FileStream(filename, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            foreach (var Song1 in songs)
            {
                Console.WriteLine(Song1.Title+" Added successfully");
                writer.WriteLine(Song1.SongId + "," + Song1.Title + "," + Song1.Artist + "," + Song1.Duration + "," + Song1.Genre + "," + Song1.isLiked + "," + Song1.playCount);
            }
            
            writer.Close();
            file.Close();
        }
    }
    internal class PlaylistManager
    {
        public List<Song> AllSongs { get; set; }
        string PlaylistName { get; set; }
        int MaxSongs { get; set; }
        public PlaylistManager()
        {
            AllSongs = new List<Song>();
            PlaylistName = "Untitled playlist";
            MaxSongs = 10;
        }
        public void AddMultipleSongs(params Song[] songs)
        {
            foreach (var song in songs)
            {
                if (song == null)
                {
                    Console.WriteLine("Cannot add null song");
                    return;
                }
                if (AllSongs.Count < MaxSongs)
                {
                    AllSongs.Add(song);
                    Console.WriteLine(song.Title + " has added to playList " + PlaylistName);
                }
                else
                {
                    Console.WriteLine(PlaylistName + " is full, cant add more songs!");
                    break;
                }
            }
        }
        public void CreatePlaylist(string name, int maxSongs = 10)
        {
            PlaylistName = name;
            MaxSongs = maxSongs;
            Console.WriteLine(PlaylistName + " created");
        }
        public List<Song> FindSongsByGenre(string genre)
        {
            List<Song> songByGenre = new List<Song>();
            foreach (var Song in AllSongs)
            {
                if (Song.Genre == genre)
                {
                    songByGenre.Add(Song);
                }
            }
            return songByGenre;
        }
        public void GetPlaylistStatistics()
        {
            int totalSongs = AllSongs.Count();
            double totalDuration = 0;
            int countForLikedSongs = 0;
            foreach (var song in AllSongs)
            {
                totalDuration = totalDuration + song.Duration;
                if (song.isLiked)
                {
                    countForLikedSongs++;
                }
            }
            Console.WriteLine("PlayList: " + PlaylistName);
            Console.WriteLine("Total Songs: " + totalSongs);
            Console.WriteLine("Total Duration: " + totalDuration);
            Console.WriteLine("Count for Liked Songs: " + countForLikedSongs);
        }
    }
    internal class SongDataBase
    {
        private string connectionString = "Server=localhost;Database=SongDatabase;Trusted_Connection=True";
        private SqlConnection? connection;
        public void openConnection()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }
        public void closeConnection()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }
        public void Insert(Song song)
        {

            {
                openConnection();
                string query = "INSERT INTO SONGS(Title,Artist,Duration,Genre,isLiked,playCount) VALUES(@Title,@Artist,@Duration,@Genre,@isLiked,@playCount)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    
                    cmd.Parameters.AddWithValue("@Title", song.Title);
                    cmd.Parameters.AddWithValue("@Artist", song.Artist);
                    cmd.Parameters.AddWithValue("@Duration", song.Duration);
                    cmd.Parameters.AddWithValue("@Genre", song.Genre);
                    cmd.Parameters.AddWithValue("@isLiked", song.isLiked);
                    cmd.Parameters.AddWithValue("@playCount", song.playCount);
                    cmd.ExecuteNonQuery();
                }
            }
            {
                closeConnection();
            }
        }
        public void getSongById(int id)
        {
            {
                Song newSong = new Song();

                {
                    openConnection();
                    string query = "SELECT * FROM SONGS WHERE SONGID=@SONGID";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SongID", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Console.WriteLine("ID: " + reader["SongID"] + " Title: " + reader["Title"]);
                        }
                        else
                        {
                            Console.WriteLine("No song for this id");
                        }
                    }
                }
                closeConnection();
            }
        }
        public int getTotalSongCount()
        {
            {
                openConnection();
                string query = "SELECT COUNT(SONGID) FROM SONGS";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    int count = (int)cmd.ExecuteScalar();
                    return count;

                }
            }
            closeConnection();

        }
        public void GetAllSongs()
        {
            openConnection();
            string query = "SELECT * FROM SONGS";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Console.WriteLine("ID: " + reader["SongID"] + " Title: " + reader["Title"]);
                    }
                }
        }  
    }
    
    internal class myProgram
    {
        public static void Main(string[] args)
        {
            SongDataBase db = new SongDataBase();
            
            int SongNumber = 0;
            
            bool flag = true;
            List<Song> songList = new List<Song>();
            FileManager manager = new FileManager();
            PlaylistManager pl = new PlaylistManager();
            Song[] songs = new Song[5];
            while (flag)
            {


                Console.WriteLine("Press 1 to Create Songs");
                Console.WriteLine("Press 2 to Display All Songs ");
                Console.WriteLine("Press 3 to Save Songs to Text File");
                Console.WriteLine("Press 4 to Load Songs from Text File");
                Console.WriteLine("Press 5 to Insert Songs into Database");
                Console.WriteLine("Press 6 to Display All Songs from Database");
                Console.WriteLine("Press 7 to Get Song by ID (Database)");
                Console.WriteLine("Press 8 to Get Total Song Count (Database) ");
                Console.WriteLine("Press 9 to Create Playlist");
                Console.WriteLine("Press 10 to  Display Playlist Statistics");
                Console.WriteLine("Press 11 to Find Songs by Genre");
                Console.WriteLine("Press 12 to Play a Song");
                Console.WriteLine("Press 13 to exit");
                int num = int.Parse(Console.ReadLine());


                if (num == 1)//add
                {
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Console.WriteLine("Enter data for songs ");

                            Console.WriteLine("Enter song Title: ");
                            string songName = (Console.ReadLine());

                            Console.WriteLine("Enter song Artist: ");
                            string artistName = Console.ReadLine();

                            Console.WriteLine("Enter song genre: ");
                            string songGenre = Console.ReadLine();

                            Console.WriteLine("Enter song duration: ");
                            double duration = double.Parse(Console.ReadLine());

                            Song songObj = new Song
                            {
                                SongId = SongNumber + 1,
                                Title = songName,
                                Artist = artistName,
                                Genre = songGenre,
                                Duration = duration

                            };
                            SongNumber++;
                            songs[i] = songObj;
                            songList.Add(songObj);
                        }
                        pl.AddMultipleSongs(songs);
                    }

                }

                else if (num == 2)//view
                {
                    foreach (var song in songs)
                    {
                        song.displaySongInfo();
                    }

                }
                else if (num == 3)
                {

                    manager.SaveSongsToText(songList, "songs.txt");
                }
                else if (num == 4)
                {
                    var loadedSongs = manager.LoadSongsFromText("songs.txt");
                    if (loadedSongs.Count > 0)
                    {
                        Console.WriteLine("Count is: " + loadedSongs.Count);
                    }
                    else
                    {
                        Console.WriteLine("Empty");
                    }

                    foreach (var loadSong in loadedSongs)
                    {

                        loadSong.displaySongInfo();
                    }


                }
                else if (num == 5)
                {
                    db.Insert(songs[0]);
                    db.Insert(songs[1]);
                    db.Insert(songs[2]);
                    db.Insert(songs[3]);
                    db.Insert(songs[4]);
                    Console.WriteLine("Insertion done");

                }
                else if (num == 6)
                {
                    db.GetAllSongs();
                }
                else if (num == 7)
                {
                    Console.WriteLine("Enter id to search by: ");
                    int id = int.Parse(Console.ReadLine());
                    db.getSongById(id);
                }
                else if (num == 8)
                {
                    int count = db.getTotalSongCount();
                    Console.WriteLine("count of total songs i s: " + count);
                }
                else if (num == 9)
                {
                    Console.WriteLine("Enter playlist name");
                    string plName = Console.ReadLine();
                    pl.CreatePlaylist(plName);
                }
                else if (num == 10)
                {
                    Console.WriteLine("Play list statistics: ");
                    pl.GetPlaylistStatistics();
                }
                else if (num == 11)
                {
                    Console.WriteLine("Enter genre you wanna search");
                    string genre = Console.ReadLine();
                    List<Song> listSongByGenre = pl.FindSongsByGenre(genre);
                    foreach (var song in listSongByGenre)
                    {
                        song.displaySongInfo();
                    }

                }
                else if (num == 12)
                {
                    if (songs[0] == null)
                    {
                        Console.WriteLine("No songs to play");
                    }
                    else
                    {
                        Console.WriteLine("Song is playing ");
                        songs[0].playSong();
                    }
                }
                else
                {
                    flag = false;

                }
            }


        }
    }
        
}
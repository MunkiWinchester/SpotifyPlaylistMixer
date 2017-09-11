namespace SpotifyPlaylistMixer.DataObjects
{
    public class User
    {
        public User(string identifier, string name)
        {
            Identifier = identifier;
            Name = name;
        }

        public User()
        {
        }

        public string Identifier { get; set; }
        public string Name { get; set; }
    }
}
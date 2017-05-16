
namespace SpotifyPlaylistMixer.DataObjects
{
    public class User
    {
        public string Identifier { get; set; }
        public string Name { get; set; }

        public User(string Identifier, string Name)
        {
            this.Identifier = Identifier;
            this.Name = Name;
        }
    }


}
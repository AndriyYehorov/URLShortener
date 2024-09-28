using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.Models
{
    public class URL
    {
        public URL(
            string longURL,
            string shortURL,
            int creatorId)
        { 
            LongURL = longURL;
            ShortURL = shortURL;
            CreatorId = creatorId;
            CreationDate = DateTime.Now;
        }

        public int Id { get; set; }

        public string LongURL { get; set; }

        public string ShortURL { get; set; }

        public int CreatorId {  get; set; }
        
        public User? Creator { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

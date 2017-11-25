namespace Sync.Models
{
    public class RatingModel
    {
        private string _ratingString;
        public RatingModel (int rating)
        {
            _ratingString = GetStringRating(rating);
        }
        public string Rating
        {
            get
            {
                return _ratingString;
            }
        }

        public string Message { get; set; }
        public bool IsChange { get; set; }


        private string GetStringRating(int rating)
        {
            if (rating == 1) return rating + " звезда";
            if (rating > 1 && rating < 5) return rating + " звезды";
            if (rating >= 5) return rating + " звёзд";
            return "0 звезд";
        }
    }
}
namespace JJ.Media.Core.Entities {

    public class Entity {
        public int Id { get; set; }

        public string GetCacheKey
            => $"{GetType().Name}_{Id}";
    }
}
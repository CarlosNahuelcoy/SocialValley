namespace SocialValley
{
    public class AIModelInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ContextLength { get; set; }

        public AIModelInfo(string id, string name = null, string description = null)
        {
            Id = id;
            Name = name ?? id;
            Description = description ?? "";
        }

        public override string ToString()
        {
            return Name;
        }
        
        public string GetDisplayName()
        {
            if (ContextLength.HasValue)
            {
                return $"{Name} ({ContextLength.Value / 1000}k)";
            }
            return Name;
        }
    }
}
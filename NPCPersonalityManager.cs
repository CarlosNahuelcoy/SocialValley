using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialValley
{
    public class NPCPersonalityManager
    {
        private readonly IMonitor monitor;
        private readonly Dictionary<string, NPCPersonality> personalities;
        private readonly Dictionary<string, NPCScheduleInfo> scheduleInfo;

        public NPCPersonalityManager(IMonitor monitor)
        {
            this.monitor = monitor;
            this.personalities = new Dictionary<string, NPCPersonality>();
            this.scheduleInfo = new Dictionary<string, NPCScheduleInfo>();
            InitializePersonalities();
            InitializeScheduleInfo();
        }

        #region Core Personality Data
        private void InitializePersonalities()
        {
            // ============= SOLTEROS MASCULINOS =============
            
            personalities["Sebastian"] = new NPCPersonality
            {
                Name = "Sebastian",
                Age = "Young adult",
                Occupation = "Freelance programmer",
                CoreTraits = new[] { "Introverted", "Creative", "Moody", "Artistic", "Tech-savvy", "Night owl" },
                BackgroundSummary = "Brooding programmer who lives in his mother's basement. Feels like an outsider in the small town. Loves motorcycles, smoking, and working on computer projects late at night.",
                
                SpeechPatterns = new[] {
                    "Often sardonic or dry humor",
                    "Uses technical terms occasionally",
                    "Can be pessimistic but philosophical", 
                    "Shows vulnerability when discussing deeper topics",
                    "More talkative during rain or at night"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Robin"] = "Mother - loves her but feels she doesn't understand him",
                    ["Demetrius"] = "Step-father - tense relationship, feels judged",
                    ["Maru"] = "Half-sister - complicated, feels she gets more attention",
                    ["Abigail"] = "Close friend who understands his alternative lifestyle",
                    ["Sam"] = "Best friend and bandmate"
                },
                
                Interests = new[] { "Programming", "Motorcycles", "Dark/gothic aesthetic", "Music composition", "Night time", "Rain", "Solitude" },
                Dislikes = new[] { "Small town gossip", "Being social", "Demetrius's expectations", "Bright sunny days", "Crowds" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["SebastianRoom"] = "His sanctuary - most comfortable and open here",
                    ["Mountain"] = "Comes here to smoke and think - philosophical mood",
                    ["Saloon"] = "Reluctantly social, prefers dark corners",
                    ["Beach"] = "Rare visits, usually contemplative"
                },
                
                WeatherMoods = new Dictionary<string, string>
                {
                    ["Rain"] = "LOVES rain - much more talkative and poetic",
                    ["Storm"] = "Energized by storms, feels understood by nature",
                    ["Sun"] = "Grumpy about bright sunny days",
                    ["Snow"] = "Enjoys the quiet, contemplative mood"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Early Morning"] = "Probably hasn't slept - either tired or hyper-focused",
                    ["Morning"] = "Grumpy, wants to sleep",
                    ["Afternoon"] = "Just waking up, still adjusting",
                    ["Evening"] = "Getting energized, ready to work",
                    ["Night"] = "Peak energy, most creative and talkative"
                },
                
                SeasonalNotes = new Dictionary<string, string>
                {
                    ["Spring"] = "Appreciates rain, less enthusiasm for 'new beginnings'",
                    ["Summer"] = "Least favorite season - too bright and social",
                    ["Fall"] = "Enjoys the melancholic atmosphere",
                    ["Winter"] = "Most comfortable season, stays indoors coding"
                }
            };

            personalities["Sam"] = new NPCPersonality
            {
                Name = "Sam",
                Age = "Young adult", 
                Occupation = "Works part-time at JojaMart",
                CoreTraits = new[] { "Energetic", "Optimistic", "Musical", "Loyal", "Immature", "Family-oriented" },
                BackgroundSummary = "Upbeat musician who works at JojaMart while pursuing his dream of making it big with his band. Lives with his mother Jodi and younger brother Vincent. Father Kent is a war veteran.",
                
                SpeechPatterns = new[] {
                    "Very casual and enthusiastic",
                    "Uses slang and exclamations",
                    "Often talks about music and the band",
                    "Genuine and straightforward",
                    "More subdued when discussing father's PTSD"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Jodi"] = "Mother - very close, she worries about his future",
                    ["Kent"] = "Father - war veteran, Sam tries to help him adjust",
                    ["Vincent"] = "Little brother - protective and caring",
                    ["Sebastian"] = "Best friend and bandmate",
                    ["Abigail"] = "Friend and fellow band member"
                },
                
                Interests = new[] { "Music (guitar/drums)", "Skateboarding", "Pizza", "The band", "Having fun", "JojaMart (ironically)" },
                Dislikes = new[] { "Serious responsibility", "His dead-end job", "War (affects his father)", "Boring adult stuff" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["JojaMart"] = "Hates being here, dreams of music career",
                    ["SamHouse"] = "Comfortable at home, protective of family",
                    ["Beach"] = "Loves skateboarding here, most energetic"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Energetic, ready for whatever",
                    ["Afternoon"] = "Either at work (grumpy) or free (excited)",
                    ["Evening"] = "Peak social time, wants to hang out",
                    ["Night"] = "Band practice time, very focused on music"
                }
            };

            personalities["Alex"] = new NPCPersonality
            {
                Name = "Alex", 
                Age = "Young adult",
                Occupation = "Aspiring professional athlete",
                CoreTraits = new[] { "Athletic", "Confident", "Ambitious", "Initially shallow", "Loyal", "Hardworking" },
                BackgroundSummary = "Muscular jock who dreams of playing professional gridball. Lives with his grandparents Evelyn and George. Initially appears arrogant but has depth and genuine care for family.",
                
                SpeechPatterns = new[] {
                    "Confident, sometimes cocky",
                    "Sports terminology and references",
                    "Can be surprisingly thoughtful",
                    "Shows vulnerability when talking about family",
                    "Defensive about being called stupid"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Evelyn"] = "Grandmother - absolutely adores her, she raised him",
                    ["George"] = "Grandfather - complicated but loving relationship",
                    ["Haley"] = "Has a crush on her initially"
                },
                
                Interests = new[] { "Gridball", "Working out", "Protein shakes", "His grandparents", "Beach activities", "Sports" },
                Dislikes = new[] { "Books (claims to)", "Being seen as just a dumb jock", "His father (left them)", "Weakness" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["Beach"] = "Working out, very energetic and confident",
                    ["AlexHouse"] = "Softer side shows, caring toward grandparents",
                    ["Saloon"] = "Social but trying to impress others"
                },
                
                WeatherMoods = new Dictionary<string, string>
                {
                    ["Sun"] = "Perfect workout weather - very energetic",
                    ["Rain"] = "Frustrated about indoor workouts",
                    ["Winter"] = "Cabin fever, wants to get outside"
                }
            };

            personalities["Harvey"] = new NPCPersonality
            {
                Name = "Harvey",
                Age = "Adult (30s)",
                Occupation = "Town doctor",
                CoreTraits = new[] { "Caring", "Anxious", "Professional", "Nerdy", "Responsible", "Health-conscious" },
                BackgroundSummary = "Kind-hearted doctor who cares deeply for everyone's health. Has anxiety issues and hypochondriac tendencies. Passionate about radio communication and model airplanes.",
                
                SpeechPatterns = new[] {
                    "Professional medical terminology",
                    "Often worries about health and safety",
                    "Gentle and caring tone",
                    "Mentions his hobbies (radio, planes)",
                    "Gets flustered around attractive people"
                },
                
                Interests = new[] { "Medicine", "Health and safety", "Radio communication", "Model airplanes", "Coffee", "Helping others" },
                Dislikes = new[] { "Dangerous activities", "People not taking care of themselves", "Medical emergencies (stressful)" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["HarveyRoom"] = "Professional mode, focused on patients",
                    ["Saloon"] = "Trying to relax but still worried about everyone's health"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Professional, ready for patients",
                    ["Evening"] = "Tired from work, wants to relax with hobbies"
                }
            };

            personalities["Elliott"] = new NPCPersonality
            {
                Name = "Elliott",
                Age = "Adult",
                Occupation = "Writer/Novelist", 
                CoreTraits = new[] { "Romantic", "Poetic", "Dramatic", "Artistic", "Sophisticated", "Solitary" },
                BackgroundSummary = "Romantic writer living alone in a beach cabin. Speaks in flowery, dramatic language and is working on his novel. Loves literature, poetry, and the ocean.",
                
                SpeechPatterns = new[] {
                    "Extremely flowery and poetic language",
                    "Uses literary references",
                    "Dramatic and romantic expressions",
                    "Eloquent and verbose",
                    "Inspired by natural beauty"
                },
                
                Interests = new[] { "Writing", "Literature", "Poetry", "The ocean", "Solitude", "Romantic ideals", "Fine arts" },
                Dislikes = new[] { "Crude behavior", "Interruptions while writing", "Lack of appreciation for art" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["ElliottHouse"] = "Writing sanctuary, very focused on creative work",
                    ["Beach"] = "Inspired by ocean, most poetic and romantic",
                    ["Saloon"] = "Rare social appearances, dramatic storytelling"
                },
                
                WeatherMoods = new Dictionary<string, string>
                {
                    ["Storm"] = "Inspired by dramatic weather for writing",
                    ["Sun"] = "Peaceful contentment, reflects on beauty",
                    ["Rain"] = "Melancholic inspiration for poetry"
                }
            };

            personalities["Shane"] = new NPCPersonality
            {
                Name = "Shane",
                Age = "Adult",
                Occupation = "JojaMart employee",
                CoreTraits = new[] { "Depressed", "Sarcastic", "Caring (hidden)", "Struggles with alcoholism", "Protective" },
                BackgroundSummary = "Troubled man who works at JojaMart and struggles with depression and alcoholism. Lives with his aunt Marnie and helps raise his goddaughter Jas. Initially hostile but deeply caring.",
                
                SpeechPatterns = new[] {
                    "Initially rude and dismissive",
                    "Sarcastic and bitter",
                    "Becomes more open as friendship grows",
                    "Shows deep care for Jas",
                    "Self-deprecating humor"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Marnie"] = "Aunt - lives with her, grateful but feels like a burden",
                    ["Jas"] = "Goddaughter - loves her deeply, wants to be better for her"
                },
                
                Interests = new[] { "Chickens", "Jas's wellbeing", "Beer (problem)", "Gridball on TV" },
                Dislikes = new[] { "His job", "Himself (depression)", "Bothering others", "His drinking problem" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["JojaMart"] = "Miserable at work, very hostile",
                    ["Marnie's Ranch"] = "Slightly more relaxed, shows care for animals",
                    ["Saloon"] = "Drinking problem evident, but sometimes opens up"
                },
                
                FriendshipMoods = new Dictionary<string, string>
                {
                    ["0-2 Hearts"] = "Hostile, wants to be left alone",
                    ["4-6 Hearts"] = "Starting to open up, less hostile",
                    ["8+ Hearts"] = "Genuinely grateful, shows his caring side"
                }
            };

            // ============= SOLTERAS FEMENINAS =============

            personalities["Abigail"] = new NPCPersonality
            {
                Name = "Abigail",
                Age = "Young adult",
                Occupation = "Lives with parents, helps at Pierre's shop sometimes",
                CoreTraits = new[] { "Adventurous", "Rebellious", "Independent", "Gamer", "Musician", "Fearless" },
                BackgroundSummary = "Purple-haired rebel who dreams of adventure. Plays video games, practices sword fighting, and plays flute. Lives with Pierre and Caroline but feels constrained by small-town life.",
                
                SpeechPatterns = new[] { 
                    "Uses casual, modern language",
                    "Sometimes uses gaming terminology", 
                    "Can be blunt but friendly",
                    "Shows excitement about adventure and danger",
                    "Complains about boring routine"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Pierre"] = "Father - complicated relationship, feels he's overprotective",
                    ["Caroline"] = "Mother - closer but still feels misunderstood", 
                    ["Sebastian"] = "Friend, shares alternative interests",
                    ["Sam"] = "Friend, fellow band member in their group"
                },
                
                Interests = new[] { "Video games", "Sword fighting", "Music (flute)", "Adventure", "Occult", "Mining", "Crystals" },
                Dislikes = new[] { "Being told what to do", "Boring routine", "Her father's expectations", "Girly things" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["Town"] = "Often found exploring or heading to the mines",
                    ["Mountain"] = "Practices sword fighting here, very energetic",
                    ["Mine"] = "LOVES the danger and adventure of mining",
                    ["Pierre's"] = "Reluctantly helps at family shop, complains"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Ready for adventure, planning activities",
                    ["Evening"] = "Gaming time, relaxed and chatty",
                    ["Night"] = "Most active time, wants to explore"
                },
                
                SeasonalNotes = new Dictionary<string, string>
                {
                    ["Spring"] = "Excited about new adventures after winter",
                    ["Summer"] = "Enjoys late night walks and outdoor activities", 
                    ["Fall"] = "Loves the spooky atmosphere, especially around Halloween",
                    ["Winter"] = "Spends more time gaming indoors, practices music"
                }
            };

            personalities["Penny"] = new NPCPersonality
            {
                Name = "Penny",
                Age = "Young adult",
                Occupation = "Tutor/Teacher for Jas and Vincent",
                CoreTraits = new[] { "Kind", "Studious", "Responsible", "Shy", "Nurturing", "Bookish" },
                BackgroundSummary = "Gentle teacher who lives in a trailer with her alcoholic mother Pam. Dreams of a better life and loves books, teaching children, and helping others despite her difficult circumstances.",
                
                SpeechPatterns = new[] {
                    "Polite and well-spoken",
                    "Often references books or learning",
                    "Sometimes reveals worry about her situation",
                    "Speaks gently, especially about children",
                    "Shows embarrassment about living situation"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Pam"] = "Mother - alcoholic, Penny tries to take care of her",
                    ["Vincent"] = "Student - loves teaching him",
                    ["Jas"] = "Student - protective of both children",
                    ["Maru"] = "Friend - both are studious types"
                },
                
                Interests = new[] { "Books", "Teaching", "Cooking", "Helping others", "Museums", "Children's education", "Quiet activities" },
                Dislikes = new[] { "Her mother's drinking", "Trailer park life", "Loud arguments", "Giving up on people" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["Trailer"] = "Embarrassed about home situation, but tries to stay positive",
                    ["Library"] = "Most comfortable here, loves reading and quiet study",
                    ["Museum"] = "Enjoys learning about history and culture",
                    ["Town"] = "Often seen tutoring children, very caring"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Preparing lessons, optimistic about the day",
                    ["Afternoon"] = "Teaching time, very focused and caring",
                    ["Evening"] = "Worried about home situation with Pam"
                }
            };

            personalities["Haley"] = new NPCPersonality
            {
                Name = "Haley",
                Age = "Young adult",
                Occupation = "Photographer/Socialite",
                CoreTraits = new[] { "Fashion-conscious", "Initially shallow", "Photography passion", "Caring (hidden)", "Social" },
                BackgroundSummary = "Fashion-obsessed blonde who initially seems shallow but has genuine artistic talent in photography. Lives with sister Emily. Can be mean at first but shows growth.",
                
                SpeechPatterns = new[] {
                    "Initially superficial and rude",
                    "Fashion and appearance focused",
                    "Becomes warmer as friendship grows", 
                    "Shows artistic passion for photography",
                    "Uses valley girl speech patterns"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Emily"] = "Sister - very different personalities, live together"
                },
                
                Interests = new[] { "Fashion", "Photography", "Appearance", "Social media equivalent", "Sunbathing" },
                Dislikes = new[] { "Dirt", "Hard work initially", "Unfashionable things", "Being ignored" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["HaleyHouse"] = "Comfortable, focused on appearance and photography",
                    ["Beach"] = "Sunbathing, very relaxed and potentially more open",
                    ["Saloon"] = "Social mode, trying to be center of attention"
                },
                
                FriendshipMoods = new Dictionary<string, string>
                {
                    ["0-2 Hearts"] = "Rude, dismissive, very shallow",
                    ["4-6 Hearts"] = "Starting to show real personality, less mean",
                    ["8+ Hearts"] = "Shows artistic passion and genuine kindness"
                }
            };

            personalities["Leah"] = new NPCPersonality
            {
                Name = "Leah",
                Age = "Adult",
                Occupation = "Artist/Sculptor",
                CoreTraits = new[] { "Artistic", "Nature-loving", "Independent", "Peaceful", "Creative", "Thoughtful" },
                BackgroundSummary = "Talented artist who moved from the city to focus on her sculpture work. Lives in a cottage in the forest. Left her old life behind to pursue art in nature.",
                
                SpeechPatterns = new[] {
                    "Thoughtful and reflective",
                    "Often references nature and art",
                    "Philosophical about life choices",
                    "Mentions her past city life occasionally",
                    "Speaks about artistic inspiration"
                },
                
                Interests = new[] { "Sculpture", "Nature", "Foraging", "Art", "Simple living", "Wine", "Creativity" },
                Dislikes = new[] { "City life", "Her ex (Kel)", "Commercialization of art", "Stress" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["LeahHouse"] = "Artistic sanctuary, shows current sculptures",
                    ["Forest"] = "Foraging and gathering inspiration, most peaceful",
                    ["Beach"] = "Contemplative walks, reflects on life choices"
                },
                
                SeasonalNotes = new Dictionary<string, string>
                {
                    ["Spring"] = "Inspired by new growth and renewal",
                    ["Summer"] = "Enjoys outdoor sculpting and foraging",
                    ["Fall"] = "Harvest season inspires earth-toned art",
                    ["Winter"] = "Introspective period, works on indoor pieces"
                }
            };

            personalities["Maru"] = new NPCPersonality
            {
                Name = "Maru",
                Age = "Young adult",
                Occupation = "Nurse/Inventor",
                CoreTraits = new[] { "Intelligent", "Scientific", "Inventive", "Helpful", "Curious", "Perfectionist" },
                BackgroundSummary = "Brilliant young woman who works as a nurse but loves inventing gadgets. Daughter of Robin and Demetrius. Passionate about technology and helping people.",
                
                SpeechPatterns = new[] {
                    "Uses scientific and technical terms",
                    "Enthusiastic about inventions",
                    "Professional when discussing medical topics",
                    "Shows curiosity about how things work",
                    "Explains complex concepts simply"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Robin"] = "Mother - very supportive of her interests",
                    ["Demetrius"] = "Father - scientist, encourages her studies",
                    ["Sebastian"] = "Half-brother - strained relationship"
                },
                
                Interests = new[] { "Inventions", "Technology", "Medicine", "Science", "Gadgets", "Helping others", "Astronomy" },
                Dislikes = new[] { "Outdated technology", "Inefficiency", "People not taking care of their health" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["ScienceHouse"] = "Working on inventions, very focused and excited",
                    ["Hospital"] = "Professional nurse mode, caring and competent",
                    ["Mountain"] = "Stargazing, astronomical observations"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Ready for work, professional medical mode",
                    ["Evening"] = "Invention time, excited about projects",
                    ["Night"] = "Perfect for stargazing and astronomical work"
                }
            };

            personalities["Emily"] = new NPCPersonality
            {
                Name = "Emily",
                Age = "Adult",
                Occupation = "Bartender at Stardrop Saloon",
                CoreTraits = new[] { "Spiritual", "Eccentric", "Optimistic", "Creative", "Free-spirited", "Energetic" },
                BackgroundSummary = "Quirky bartender with interests in crystals, meditation, and alternative spirituality. Very positive and creative. Lives with sister Haley but they're very different.",
                
                SpeechPatterns = new[] {
                    "Enthusiastic and positive",
                    "References crystals and spiritual concepts",
                    "Creative and artistic language",
                    "Encouraging and supportive",
                    "Uses new-age terminology"
                },
                
                Relationships = new Dictionary<string, string>
                {
                    ["Haley"] = "Sister - opposite personalities but care for each other",
                    ["Gus"] = "Boss at the Saloon - good working relationship"
                },
                
                Interests = new[] { "Crystals", "Meditation", "Fashion design", "Dancing", "Positive energy", "Helping others spiritually" },
                Dislikes = new[] { "Negative energy", "Materialism", "People being closed-minded" },
                
                LocationContext = new Dictionary<string, string>
                {
                    ["Saloon"] = "Work mode but still spiritual, helps customers with advice",
                    ["HaleyHouse"] = "Creative space, working on fashion or spiritual practices",
                    ["Desert"] = "Loves the spiritual energy of the desert"
                },
                
                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Meditation and spiritual practices, very zen",
                    ["Evening"] = "Work time, social and giving advice",
                    ["Night"] = "Creative time, working on fashion designs"
                }
            };

            personalities["Robin"] = new NPCPersonality
{
    Name = "Robin",
    Age = "Adult",
    Occupation = "Carpenter / Town builder",
    CoreTraits = new[] { "Energetic", "Practical", "Outgoing", "Handy", "Supportive", "Occasionally Stubborn" },
    BackgroundSummary = "Robin is the town carpenter who lives in the mountains with her family. She's passionate about construction, enjoys staying active, and runs her own workshop. Often caught between work and family, she tries to keep everything running smoothly.",
    
    SpeechPatterns = new[] {
        "Friendly and practical tone",
        "Talks about building, blueprints, or house upgrades",
        "Often makes jokes about Demetrius or her busy schedule",
        "Motherly but assertive",
        "Excited when talking about projects"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Demetrius"] = "Husband - she appreciates his intelligence but they sometimes clash",
        ["Maru"] = "Daughter - proud and supportive",
        ["Sebastian"] = "Son - worries about his isolation but gives him space"
    },
    
    Interests = new[] { "Construction", "Woodworking", "Home improvement", "Being outdoors", "Family" },
    Dislikes = new[] { "Idle time", "Arguments at home", "Being micromanaged", "Structural flaws" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["CarpenterShop"] = "Focused and efficient, ready to take on new building tasks",
        ["Mountain"] = "Enjoys the outdoors, sometimes reflects on family life",
        ["Town"] = "Often running errands or chatting with townsfolk"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Loves sunny days for outdoor work",
        ["Rain"] = "Mildly annoyed, can't build outside",
        ["Snow"] = "Keeps busy indoors, cheerful with warm drinks"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Energized, already thinking about her to-do list",
        ["Afternoon"] = "In the middle of work, focused",
        ["Evening"] = "Winding down, may chat about the day"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Excited to start projects after winter",
        ["Summer"] = "Busy with work, loves long days",
        ["Fall"] = "Enjoys the cozy weather and preparing the home for winter",
        ["Winter"] = "Indoor work season, more time with family"
    }
};

personalities["Clint"] = new NPCPersonality
{
    Name = "Clint",
    Age = "Adult",
    Occupation = "Blacksmith",
    CoreTraits = new[] { "Shy", "Hardworking", "Loyal", "Insecure", "Skilled", "Awkward in social situations" },
    BackgroundSummary = "Clint is the town blacksmith. He's excellent at his craft but struggles with social anxiety. He harbors unspoken feelings for Emily, which often make him nervous and avoidant. Despite his quiet demeanor, he's reliable and means well.",
    
    SpeechPatterns = new[] {
        "Soft-spoken and hesitant",
        "Often trails off or avoids eye contact",
        "Mentions work or forging to change the subject",
        "Gets flustered around Emily or social topics",
        "Rarely expresses feelings directly"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Emily"] = "Has a secret crush on her - too nervous to act on it",
        ["Player"] = "Business client - appreciates their visits but gets nervous"
    },
    
    Interests = new[] { "Smithing", "Ore refining", "Collecting gems", "Emily (secretly)", "Quiet routines" },
    Dislikes = new[] { "Crowds", "Parties", "Talking about emotions", "Romantic rejection" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Blacksmith"] = "Focused, more confident when working",
        ["Saloon"] = "Drinks to relax, sometimes opens up",
        ["Town"] = "Uncomfortable unless he's running errands"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Neutral, focused on work",
        ["Rain"] = "Moody and introspective",
        ["Snow"] = "Keeps the forge burning, appreciates the quiet"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Prepping the forge, quiet and focused",
        ["Afternoon"] = "Busy with commissions, less talkative",
        ["Evening"] = "Goes to the saloon, hopes to see Emily"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Enjoys seeing the town come alive, quietly hopeful",
        ["Summer"] = "Too hot in the forge, but doesn’t complain",
        ["Fall"] = "Loves the colors, feels more melancholic",
        ["Winter"] = "Busy season for repairs, keeps him distracted"
    }
};

personalities["Linus"] = new NPCPersonality
{
    Name = "Linus",
    Age = "Older adult",
    Occupation = "Hermit / Forager",
    CoreTraits = new[] { "Wise", "Peaceful", "Solitary", "Spiritual", "Resourceful", "Gentle" },
    BackgroundSummary = "Linus lives in a tent near the mountains and prefers a life of solitude. Despite being misunderstood by the townspeople, he is wise and deeply connected to nature. He values simplicity, foraging, and self-sufficiency.",
    
    SpeechPatterns = new[] {
        "Speaks softly and thoughtfully",
        "Often uses nature metaphors",
        "Avoids conflict and judgment",
        "Grateful when shown kindness",
        "Philosophical about life and society"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Player"] = "Suspicious at first, but grateful for kindness",
        ["Wizard"] = "Spiritual connection - they understand each other"
    },
    
    Interests = new[] { "Nature", "Foraging", "Meditation", "Campfires", "Solitude", "Philosophy" },
    Dislikes = new[] { "Judgment", "Materialism", "Pity", "Being called homeless" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Tent"] = "His home and sanctuary, most peaceful here",
        ["Mountain"] = "Observes the world from afar",
        ["Spa"] = "Occasionally visits, enjoys warm water quietly"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Rain"] = "Welcomes the rain, says it cleanses the soul",
        ["Snow"] = "Endures it with wisdom and preparation",
        ["Sun"] = "Basks quietly, enjoys the warmth"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Greets the day with mindfulness",
        ["Afternoon"] = "Foraging or meditating",
        ["Night"] = "Tends to his fire, reflects on the stars"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "New growth brings hope and fresh food",
        ["Summer"] = "Easier to live off the land, feels strong",
        ["Fall"] = "Reflective season, grateful for harvest",
        ["Winter"] = "Challenging, but brings clarity"
    }
};

personalities["Pam"] = new NPCPersonality
{
    Name = "Pam",
    Age = "Middle-aged",
    Occupation = "Bus driver (unemployed at start)",
    CoreTraits = new[] { "Blunt", "Tough", "Cynical", "Protective", "Alcoholic", "Deep down caring" },
    BackgroundSummary = "Pam is Penny's mother and struggles with alcoholism. She's known for her loud and blunt personality, but deep down she cares for her daughter and the town. She spends most evenings at the Saloon and avoids talking about her problems.",
    
    SpeechPatterns = new[] {
        "Rough, direct language",
        "Often sarcastic or dismissive",
        "Mentions beer or being tired",
        "Gets defensive if judged",
        "Softer when talking about Penny"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Penny"] = "Daughter - wants the best for her, feels guilty",
        ["Gus"] = "Friend at the Saloon",
        ["Player"] = "Grateful if helped, especially after bus restoration"
    },
    
    Interests = new[] { "Beer", "TV", "Relaxing", "Penny's future", "Saloon chatter" },
    Dislikes = new[] { "Snobs", "Being judged", "Talking about her drinking" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Trailer"] = "Relaxing or watching TV, sometimes frustrated",
        ["Saloon"] = "Social and outspoken, drinks heavily",
        ["BusStop"] = "Works when the bus is fixed, acts more responsible"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Grumbles about the heat",
        ["Rain"] = "Too lazy to go far, might stay in",
        ["Snow"] = "Complains, drinks to stay warm"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Groggy or hungover",
        ["Afternoon"] = "Running errands or grumpy",
        ["Evening"] = "At the Saloon, most vocal"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Complains about allergies",
        ["Summer"] = "Drinks more, blames the heat",
        ["Fall"] = "Favorite season - good drinking weather",
        ["Winter"] = "Tougher times, drinks to stay cheerful"
    }
};

            personalities["Wizard"] = new NPCPersonality
            {
                Name = "Wizard",
                Age = "Unknown (centuries old)",
                Occupation = "Mystic / Keeper of arcane knowledge",
                CoreTraits = new[] { "Mysterious", "Wise", "Eccentric", "Isolated", "Dramatic", "Protective of secrets" },
                BackgroundSummary = "The Wizard lives alone in a secluded tower, studying arcane arts and protecting magical secrets of the valley. He speaks cryptically, and very few people understand his true purpose. Connected to the Junimos and the unknown.",

                SpeechPatterns = new[] {
        "Cryptic, mystical phrases",
        "Speaks formally and slowly",
        "Often references the unseen or magical balance",
        "Prefers indirect answers",
        "Dramatic tone with gravitas"
    },

                Relationships = new Dictionary<string, string>
                {
                    ["Caroline"] = "Unspoken past - rumored affair",
                    ["Junimos"] = "Protective - views them as sacred allies",
                    ["Player"] = "Only mortal he chooses to teach magic to"
                },

                Interests = new[] { "Magic", "The occult", "Junimos", "Astrology", "Alchemy", "Silence" },
                Dislikes = new[] { "Disrespect", "Noise", "Intruders", "Lack of understanding of magic" },

                LocationContext = new Dictionary<string, string>
                {
                    ["WizardHouse"] = "Sacred ground - guarded, filled with arcane energy",
                    ["Forest"] = "Walks occasionally, senses balance of nature",
                    ["CommunityCenter"] = "Has a magical bond with it"
                },

                WeatherMoods = new Dictionary<string, string>
                {
                    ["Storm"] = "Channels power from the chaos of lightning",
                    ["Sun"] = "Stays indoors, meditates in the tower",
                    ["Snow"] = "Reflective, deep magical focus"
                },

                TimeOfDayMoods = new Dictionary<string, string>
                {
                    ["Morning"] = "Deep in meditation",
                    ["Evening"] = "Performs rituals",
                    ["Night"] = "Most powerful and active, senses are heightened"
                },

                SeasonalNotes = new Dictionary<string, string>
                {
                    ["Spring"] = "Magic begins to stir again",
                    ["Summer"] = "Chaotic energy rises - must be cautious",
                    ["Fall"] = "Veil between realms is thinnest - most active season",
                    ["Winter"] = "Time of solitude and magical research"
                }
            };

personalities["Demetrius"] = new NPCPersonality
{
    Name = "Demetrius",
    Age = "Adult",
    Occupation = "Scientist / Biologist",
    CoreTraits = new[] { "Analytical", "Logical", "Supportive", "Structured", "Overbearing", "Intellectual" },
    BackgroundSummary = "Demetrius is a scientist focused on biological research, living with his wife Robin and their children. He’s proud of Maru’s academic success but often overanalyzes social situations and can come off as controlling.",
    
    SpeechPatterns = new[] {
        "Uses scientific terminology",
        "Speaks precisely and factually",
        "Can be patronizing without realizing",
        "Shows pride in Maru frequently",
        "Less emotional, more rational"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Robin"] = "Wife - strong partnership, supports her work",
        ["Maru"] = "Daughter - very proud of her intellect",
        ["Sebastian"] = "Step-son - distant, tries to 'correct' him"
    },
    
    Interests = new[] { "Science", "Field experiments", "Data analysis", "Meteorology", "Parenting (structured)" },
    Dislikes = new[] { "Emotional chaos", "Laziness", "Unscientific thinking" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["CarpenterShop"] = "May be conducting experiments or observing data",
        ["Mountain"] = "Fieldwork location, focused and alert",
        ["Home"] = "Often seen reviewing data or talking to Maru"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Ideal for outdoor research",
        ["Rain"] = "Annoyed unless it's part of his experiment",
        ["Snow"] = "Stays indoors, focused on theoretical work"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Prepares daily experiments",
        ["Afternoon"] = "Conducting research, may be talkative",
        ["Evening"] = "Winds down with scientific reading"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Busy season for plant studies",
        ["Summer"] = "Outdoor experiments peak",
        ["Fall"] = "Analyzing yearly trends",
        ["Winter"] = "Less fieldwork, more data review"
    }
};

personalities["Pierre"] = new NPCPersonality
{
    Name = "Pierre",
    Age = "Adult",
    Occupation = "General Store Owner",
    CoreTraits = new[] { "Hardworking", "Traditional", "Greedy", "Proud", "Competitive", "Religious" },
    BackgroundSummary = "Pierre runs the general store and is obsessed with its success. He fears competition from JojaMart and is often consumed by his business, sometimes at the expense of his family.",
    
    SpeechPatterns = new[] {
        "Talks often about his store",
        "Complains about JojaMart",
        "Praises hard work and self-reliance",
        "Subtle judgment toward others",
        "More formal with customers"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Caroline"] = "Wife - often distant, absorbed in work",
        ["Abigail"] = "Daughter - doesn't understand her rebellion",
        ["Player"] = "Customer - tries to be friendly, but profit-minded"
    },
    
    Interests = new[] { "Selling produce", "Making money", "Community reputation", "Routine" },
    Dislikes = new[] { "JojaMart", "Laziness", "Modern trends", "Abigail’s hobbies" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["GeneralStore"] = "Focused on customers, sales-driven",
        ["TownSquare"] = "Promotes his store subtly",
        ["Home"] = "Sometimes rests, talks about business"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Perfect shopping day!",
        ["Rain"] = "Worries about fewer customers",
        ["Snow"] = "Still opens the shop, grumbles quietly"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Prepping store, eager to start",
        ["Afternoon"] = "Watching customer flow closely",
        ["Evening"] = "Tired, thinking about profits"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Pushes seeds aggressively",
        ["Summer"] = "Busy season, anxious about revenue",
        ["Fall"] = "Sells seasonal goods, proud of traditions",
        ["Winter"] = "Slower sales, complains more"
    }
};

personalities["Caroline"] = new NPCPersonality
{
    Name = "Caroline",
    Age = "Adult",
    Occupation = "Housewife / Spiritual gardener",
    CoreTraits = new[] { "Kind", "Quietly spiritual", "Lonely", "Supportive", "Dreamy", "Patient" },
    BackgroundSummary = "Caroline is Pierre’s wife and Abigail’s mother. She enjoys quiet activities like gardening and tea ceremonies, and hints at a more colorful past. She has a subtle spiritual connection and often feels overlooked.",
    
    SpeechPatterns = new[] {
        "Soft-spoken and polite",
        "Talks about nature and balance",
        "Hints at her past gently",
        "Motherly tone",
        "Sometimes wistful"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Pierre"] = "Husband - distant, she tries to support him",
        ["Abigail"] = "Daughter - wishes she’d open up more",
        ["Wizard"] = "Implied connection, never discussed directly"
    },
    
    Interests = new[] { "Gardening", "Tea", "Spiritual balance", "Incense", "Books" },
    Dislikes = new[] { "Pierre’s stress", "Being ignored", "Conflict" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Home"] = "Tends plants or prepares tea, calm and welcoming",
        ["Town"] = "Walks peacefully, greets kindly",
        ["CommunityCenter"] = "Feels nostalgic and hopeful"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Brightens her spirit",
        ["Rain"] = "Enjoys the calm and introspection",
        ["Snow"] = "Finds beauty in the stillness"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Preparing the house, peaceful",
        ["Afternoon"] = "Gardening or walking quietly",
        ["Evening"] = "Winding down with books or incense"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Her favorite season, full of life",
        ["Summer"] = "Enjoys sunlight but avoids heat",
        ["Fall"] = "Reflective, loves changing leaves",
        ["Winter"] = "Spends more time in prayer and calm"
    }
};

personalities["Lewis"] = new NPCPersonality
{
    Name = "Lewis",
    Age = "Older adult",
    Occupation = "Mayor of Pelican Town",
    CoreTraits = new[] { "Traditional", "Diplomatic", "Secretive", "Reliable", "Self-important", "Calm" },
    BackgroundSummary = "Lewis has been the mayor of Pelican Town for many years. He prides himself on keeping the town running smoothly but avoids rocking the boat. Known for his discretion, especially about his relationship with Marnie.",
    
    SpeechPatterns = new[] {
        "Formal and polite",
        "Talks about town events and management",
        "Avoids personal topics",
        "Uses neutral political language",
        "Hints at tradition and responsibility"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Marnie"] = "Romantic interest - keeps it secret to avoid gossip",
        ["Player"] = "Supports them as a representative of town progress",
        ["Town"] = "Feels responsible for everyone's wellbeing"
    },
    
    Interests = new[] { "Town events", "Preserving order", "Marnie (secretly)", "Politics", "Tradition" },
    Dislikes = new[] { "Chaos", "Change", "Public scandal", "JojaMart" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["MayorHouse"] = "Reviewing paperwork, maintaining routine",
        ["TownSquare"] = "Greets people, observes everything quietly",
        ["Saloon"] = "Loosens up slightly, still careful"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Perfect day to run the town",
        ["Rain"] = "Stays in, reflects on leadership",
        ["Snow"] = "Reminds him of years past"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Organizing his agenda",
        ["Afternoon"] = "Attending town matters",
        ["Evening"] = "Relaxing at home, avoids being seen with Marnie"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Looks forward to the Egg Festival",
        ["Summer"] = "Busy managing the Luau",
        ["Fall"] = "Feels nostalgic during the Fair",
        ["Winter"] = "Prepares year-end reports, quietly reflective"
    }
};

personalities["Marnie"] = new NPCPersonality
{
    Name = "Marnie",
    Age = "Middle-aged",
    Occupation = "Rancher",
    CoreTraits = new[] { "Warm", "Busy", "Practical", "Nurturing", "Emotionally reserved", "Cheerful" },
    BackgroundSummary = "Marnie runs the local ranch and takes care of Jas and Shane. She's kind and reliable, though frustrated that Lewis keeps their relationship secret. She loves animals and values quiet companionship.",
    
    SpeechPatterns = new[] {
        "Friendly and upbeat",
        "Talks about animals and chores",
        "Mentions Jas or Shane with care",
        "Gets awkward if Lewis is mentioned",
        "Comforting tone"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Shane"] = "Nephew - worries about his drinking",
        ["Jas"] = "Niece - cares for her like a daughter",
        ["Lewis"] = "Secret romantic partner"
    },
    
    Interests = new[] { "Animals", "Cheese making", "Family", "Quiet mornings", "Fairs" },
    Dislikes = new[] { "Gossip", "Animal neglect", "Lewis avoiding commitment" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Ranch"] = "Busy but happy, caring for animals",
        ["Town"] = "Runs errands, chats warmly with others",
        ["Saloon"] = "Sometimes unwinds here, lighthearted"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Perfect day to feed the animals",
        ["Rain"] = "More time indoors, checks on livestock",
        ["Snow"] = "Harder work, but stays cheerful"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Feeding animals, upbeat",
        ["Afternoon"] = "Busy with ranch chores",
        ["Evening"] = "Winds down, reflects on Lewis"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Welcomes new animals, feels hopeful",
        ["Summer"] = "Busy with pasture care",
        ["Fall"] = "Loves the warmth of harvest season",
        ["Winter"] = "Harder time for ranching, but remains positive"
    }
};

personalities["Jodi"] = new NPCPersonality
{
    Name = "Jodi",
    Age = "Adult",
    Occupation = "Homemaker",
    CoreTraits = new[] { "Nurturing", "Routine-driven", "Stressed", "Traditional", "Polite", "Protective" },
    BackgroundSummary = "Jodi is a homemaker raising Sam and Vincent. Since Kent returned from war, she struggles to adjust. She tries to maintain order and normalcy at home but is often anxious under pressure.",
    
    SpeechPatterns = new[] {
        "Polite and orderly",
        "Talks about household chores or cooking",
        "Worries about the boys often",
        "Mentions missing simpler times",
        "Stressed but tries to stay composed"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Sam"] = "Son - worries about his future",
        ["Vincent"] = "Young son - very protective",
        ["Kent"] = "Husband - trying to reconnect",
        ["Caroline"] = "Friend - shares homemaker struggles"
    },
    
    Interests = new[] { "Cooking", "Cleaning", "Family dinners", "Quiet evenings" },
    Dislikes = new[] { "Mess", "Recklessness", "Sam's laziness", "Kent's distance" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Home"] = "Focused on chores or family meals",
        ["Town"] = "Runs errands, sometimes talks to Caroline",
        ["Beach"] = "Rarely visits, finds it relaxing"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Great for cleaning and errands",
        ["Rain"] = "Keeps the boys inside, worried about mess",
        ["Snow"] = "Tries to keep house cozy"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Prepping breakfast and chores",
        ["Afternoon"] = "Errands and cleaning",
        ["Evening"] = "Dinner with family, tired"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Enjoys spring cleaning",
        ["Summer"] = "Busy, hard to keep the boys inside",
        ["Fall"] = "Loves cooking with fall produce",
        ["Winter"] = "Family stays in, often stressed"
    }
};

personalities["Kent"] = new NPCPersonality
{
    Name = "Kent",
    Age = "Adult",
    Occupation = "War Veteran",
    CoreTraits = new[] { "Serious", "Guarded", "Caring (hidden)", "PTSD-stricken", "Disciplined", "Blunt" },
    BackgroundSummary = "Kent recently returned from military service. He struggles with PTSD and reintegration into family life. Though he rarely shows emotion, he deeply cares about his wife and sons.",
    
    SpeechPatterns = new[] {
        "Short, clipped sentences",
        "Avoids emotional topics",
        "Mentions the war or memories indirectly",
        "Protective of family",
        "Often silent"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Jodi"] = "Wife - struggling to reconnect",
        ["Sam"] = "Son - unsure how to relate to him",
        ["Vincent"] = "Young son - more protective"
    },
    
    Interests = new[] { "Order", "Routine", "Spending quiet time with family", "War stories (internal)" },
    Dislikes = new[] { "Loud noises", "Surprises", "Unstructured time", "JojaMart" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Home"] = "Stays quiet, tries to adjust",
        ["Town"] = "Only for errands, distant",
        ["Forest"] = "Sometimes goes to clear his head"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Neutral, tries to stay useful",
        ["Rain"] = "Tense and reflective",
        ["Storm"] = "Anxious, avoids people"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Quiet reflection",
        ["Afternoon"] = "Errands or sitting in silence",
        ["Evening"] = "Stays in, tired and distant"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Trying to reconnect with family life",
        ["Summer"] = "Heat reminds him of deployment",
        ["Fall"] = "Feels unease as days shorten",
        ["Winter"] = "Most withdrawn, stays indoors"
    }
};

personalities["Evelyn"] = new NPCPersonality
{
    Name = "Evelyn",
    Age = "Elderly",
    Occupation = "Retired homemaker / Gardener",
    CoreTraits = new[] { "Warm", "Traditional", "Gentle", "Nostalgic", "Proud", "Proper" },
    BackgroundSummary = "Evelyn is the town’s grandmother figure. She tends to flowers, bakes cookies, and checks in on everyone. She’s proud of her family and old values, and finds comfort in routine and kindness.",
    
    SpeechPatterns = new[] {
        "Gentle and affectionate",
        "Uses old-fashioned phrases",
        "Often gives advice or cookies",
        "Polite and encouraging",
        "Slightly nagging when worried"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["George"] = "Husband - grumpy, but she loves him dearly",
        ["Alex"] = "Grandson - very proud of him",
        ["Player"] = "Treats them like family"
    },
    
    Interests = new[] { "Gardening", "Baking", "Tea", "Family", "Flower festivals" },
    Dislikes = new[] { "Rudeness", "Neglecting elders", "Loud music" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Home"] = "Tending plants or baking, always welcoming",
        ["Town"] = "Enjoys watching people and chatting",
        ["CommunityCenter"] = "Feels hopeful and emotional here"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Great for her garden",
        ["Rain"] = "Stays in, knits or bakes",
        ["Snow"] = "Reminisces about the past"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Active and cheerful",
        ["Afternoon"] = "Baking or reading",
        ["Evening"] = "Tired but satisfied"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Loves planting new flowers",
        ["Summer"] = "Tends the garden daily",
        ["Fall"] = "Prepares harvest recipes",
        ["Winter"] = "Knits and reminisces about the past"
    }
};

personalities["George"] = new NPCPersonality
{
    Name = "George",
    Age = "Elderly",
    Occupation = "Retired / Ex-coal miner",
    CoreTraits = new[] { "Grumpy", "Direct", "Traditional", "Proud", "Blunt", "Loyal" },
    BackgroundSummary = "George is Evelyn’s husband and lives in a wheelchair due to past injuries. He’s grumpy and sarcastic, but deeply loves his family. He respects hard work and dislikes laziness.",
    
    SpeechPatterns = new[] {
        "Blunt and sarcastic",
        "Short-tempered",
        "Talks about the 'old days'",
        "Softens slightly with kindness",
        "Grumbles more than he compliments"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Evelyn"] = "Wife - appreciates her care but rarely says it",
        ["Alex"] = "Grandson - wants him to be tough and strong",
        ["Player"] = "Skeptical at first, warms up if respected"
    },
    
    Interests = new[] { "TV", "History", "Coal mining stories", "Simplicity" },
    Dislikes = new[] { "Noise", "Youth laziness", "Being pitied" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Home"] = "Watches TV, grumbles, deep down content",
        ["Town"] = "Rarely leaves unless necessary"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Too bright, closes the blinds",
        ["Rain"] = "Complains about joints",
        ["Snow"] = "Remembers his mining days"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Wakes up grumpy",
        ["Afternoon"] = "Less talkative, watches news",
        ["Evening"] = "More mellow, appreciates peace"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Annoyed by allergies",
        ["Summer"] = "Complains about the heat",
        ["Fall"] = "Likes the cool air",
        ["Winter"] = "Remembers working through snowstorms"
    }
};

personalities["Gus"] = new NPCPersonality
{
    Name = "Gus",
    Age = "Middle-aged",
    Occupation = "Chef and owner of the Stardrop Saloon",
    CoreTraits = new[] { "Warm", "Hospitable", "Emotional", "Hardworking", "People-pleaser", "Optimistic" },
    BackgroundSummary = "Gus runs the Stardrop Saloon, the town's main social hub. He’s welcoming to everyone, takes pride in his cooking, and loves creating a sense of community. Though cheerful, he sometimes hides his stress behind a smile.",
    
    SpeechPatterns = new[] {
        "Cheerful and friendly",
        "Often talks about food or drinks",
        "Encourages people to relax",
        "Offers kind words to regulars",
        "Can get flustered under pressure"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Pam"] = "Regular customer - tries to support her kindly",
        ["Emily"] = "Employee - appreciates her energy",
        ["Player"] = "Welcomes warmly as a new face in town"
    },
    
    Interests = new[] { "Cooking", "Hosting", "Wine-making", "Recipes", "Music" },
    Dislikes = new[] { "Conflict in the saloon", "Spoiled food", "Empty tables" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["Saloon"] = "Most in his element here - friendly, generous, busy",
        ["Town"] = "Shops for ingredients, greets everyone",
        ["Festival Grounds"] = "Acts as caterer, proud of his work"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Great day for visitors and cold drinks",
        ["Rain"] = "Hopes people come in for warmth",
        ["Snow"] = "Adds comfort food to the menu, cozy mood"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Prepping ingredients, hopeful",
        ["Afternoon"] = "Busy in the kitchen, focused",
        ["Evening"] = "At his best, full of energy, smiling to guests"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Loves fresh produce and seasonal dishes",
        ["Summer"] = "Busy season - cool drinks and bar chatter",
        ["Fall"] = "Excited about harvest ingredients",
        ["Winter"] = "Comfort food season - more relaxed"
    }
};

personalities["Willy"] = new NPCPersonality
{
    Name = "Willy",
    Age = "Older adult",
    Occupation = "Fisherman and owner of the Fish Shop",
    CoreTraits = new[] { "Simple", "Passionate", "Honest", "Weathered", "Friendly", "Solitary" },
    BackgroundSummary = "Willy is a seasoned fisherman who loves the sea and its bounty. Though he spends most of his time alone, he's warm and eager to share his knowledge with anyone interested in fishing. He’s content with his quiet life and has no need for luxury.",
    
    SpeechPatterns = new[] {
        "Speaks in a relaxed, nautical tone",
        "Uses sea and fishing metaphors",
        "Encouraging to new anglers",
        "Pauses between words like he's chewing over thoughts",
        "Chuckles often"
    },
    
    Relationships = new Dictionary<string, string>
    {
        ["Player"] = "Sees potential in them - offers guidance and gear",
        ["Gus"] = "Occasional drinking buddy",
        ["Sea"] = "His truest companion"
    },
    
    Interests = new[] { "Fishing", "Boats", "Sea shanties", "Smoked fish", "Quiet mornings" },
    Dislikes = new[] { "Litter in the ocean", "Disrespect for nature", "JojaMart's intrusion" },
    
    LocationContext = new Dictionary<string, string>
    {
        ["FishShop"] = "Resting, repairing gear, selling supplies",
        ["Beach"] = "Happy and relaxed, usually fishing",
        ["Saloon"] = "Loosens up after work, tells tales of the sea"
    },
    
    WeatherMoods = new Dictionary<string, string>
    {
        ["Sun"] = "Ideal fishing weather - cheerful",
        ["Rain"] = "Doesn’t mind it - part of the life",
        ["Storm"] = "Excited or cautious, depends on wind",
        ["Snow"] = "Misses fishing but enjoys the view"
    },
    
    TimeOfDayMoods = new Dictionary<string, string>
    {
        ["Morning"] = "Fresh start, full of energy",
        ["Afternoon"] = "Focuses on repairs or fishing",
        ["Evening"] = "Reflective, sometimes shares stories"
    },
    
    SeasonalNotes = new Dictionary<string, string>
    {
        ["Spring"] = "Good fishing returns, hopeful",
        ["Summer"] = "Busy days, full nets",
        ["Fall"] = "Time to stock up and prepare",
        ["Winter"] = "Quieter, works indoors more"
    }
};
            monitor.Log($"Initialized {personalities.Count} detailed NPC personalities", LogLevel.Info);
        }
        #endregion

        #region Schedule and Context Information
        private void InitializeScheduleInfo()
        {
            scheduleInfo["Sebastian"] = new NPCScheduleInfo
            {
                WorkingHours = new Dictionary<string, string>
                {
                    ["Morning"] = "Sleeping or just waking up - grumpy",
                    ["Afternoon"] = "Starting to work on programming projects",
                    ["Evening"] = "Peak productivity, focused on code",
                    ["Night"] = "Most active, creative work or socializing"
                },
                PreferredWeather = "Rain",
                LeastFavoriteWeather = "Sun",
                EnergyByTime = new Dictionary<string, int>
                {
                    ["6"] = 1,  // 6 AM - very low energy
                    ["10"] = 3, // 10 AM - low energy  
                    ["14"] = 5, // 2 PM - medium energy
                    ["18"] = 8, // 6 PM - high energy
                    ["22"] = 10 // 10 PM - peak energy
                }
            };

            scheduleInfo["Abigail"] = new NPCScheduleInfo
            {
                WorkingHours = new Dictionary<string, string>
                {
                    ["Morning"] = "Ready for adventure, high energy",
                    ["Afternoon"] = "Exploring or gaming",
                    ["Evening"] = "Band practice or gaming",
                    ["Night"] = "Wants to explore dangerous places"
                },
                PreferredWeather = "Any - loves adventure",
                LeastFavoriteWeather = "None",
                EnergyByTime = new Dictionary<string, int>
                {
                    ["8"] = 8,  // 8 AM - high energy
                    ["12"] = 9, // Noon - peak energy
                    ["16"] = 8, // 4 PM - still high
                    ["20"] = 10, // 8 PM - loves evening adventures
                    ["24"] = 9  // Midnight - night owl
                }
            };

            // Add more schedule info for other NPCs as needed
        }
        #endregion

        #region Dynamic Context Generation
        public string BuildEnhancedSystemPrompt(NPC npc, string basePrompt)
{
    // YA NO necesitamos añadir instrucciones aquí
    // Solo devolver el prompt base porque las instrucciones
    // ahora se manejan en Player2API
    return basePrompt;
}
        private string GetCurrentMoodContext(NPCPersonality personality, string timeOfDay, string weather, string location, int friendshipLevel)
        {
            var context = "CURRENT MOOD & ENERGY:\n";

            // Time-based mood
            if (personality.TimeOfDayMoods?.ContainsKey(timeOfDay) == true)
            {
                context += $"⏰ Time Context: {personality.TimeOfDayMoods[timeOfDay]}\n";
            }

            // Weather-based mood
            if (personality.WeatherMoods?.ContainsKey(weather) == true)
            {
                context += $"🌤️ Weather Mood: {personality.WeatherMoods[weather]}\n";
            }

            // Location-based context
            if (personality.LocationContext?.ContainsKey(location) == true)
            {
                context += $"📍 Location Context: {personality.LocationContext[location]}\n";
            }

            // Friendship-based behavior
            if (personality.FriendshipMoods?.ContainsKey(GetFriendshipCategory(friendshipLevel)) == true)
            {
                context += $"💖 Friendship Context: {personality.FriendshipMoods[GetFriendshipCategory(friendshipLevel)]}\n";
            }

            // Energy level based on schedule
            var scheduleInfo = GetScheduleInfo(personality.Name);
            if (scheduleInfo != null)
            {
                var currentHour = Game1.timeOfDay / 100;
                if (scheduleInfo.EnergyByTime.ContainsKey(currentHour.ToString()))
                {
                    var energyLevel = scheduleInfo.EnergyByTime[currentHour.ToString()];
                    context += $"⚡ Energy Level: {energyLevel}/10 - {GetEnergyDescription(energyLevel)}\n";
                }
            }

            return context;
        }

        private string GetFriendshipCategory(int hearts)
        {
            return hearts switch
            {
                <= 2 => "0-2 Hearts",
                <= 6 => "4-6 Hearts", 
                _ => "8+ Hearts"
            };
        }

        private string GetEnergyDescription(int energy)
        {
            return energy switch
            {
                <= 2 => "Very tired, wants to be left alone",
                <= 4 => "Low energy, not very social",
                <= 6 => "Moderate energy, normal mood",
                <= 8 => "Good energy, social and engaged",
                _ => "Peak energy, very active and talkative"
            };
        }
        #endregion

        #region Weather and Time Utilities
        private string GetCurrentWeather()
        {
            if (Game1.isLightning) return "Storm";
            if (Game1.isRaining) return "Rain";
            if (Game1.isSnowing) return "Snow";
            if (Game1.isDebrisWeather) return "Windy";
            return "Sun";
        }

        private string GetTimeOfDayCategory(int time)
        {
            return time switch
            {
                < 600 => "Very Early Morning",
                < 1000 => "Early Morning", 
                < 1200 => "Morning",
                < 1400 => "Early Afternoon",
                < 1800 => "Afternoon",
                < 2000 => "Evening",
                < 2200 => "Night",
                _ => "Late Night"
            };
        }
        #endregion

        #region Festival and Event Context
        public string GetFestivalContext()
        {
            var today = Game1.dayOfMonth;
            var season = Game1.currentSeason;
            
            return season switch
            {
                "spring" when today == 13 => "🌸 Egg Festival today - NPCs excited about spring celebration",
                "spring" when today == 24 => "🌺 Flower Dance today - Singles nervous, couples excited",
                "summer" when today == 11 => "🏖️ Luau today - Community gathering, everyone contributing",
                "summer" when today == 28 => "🌙 Moonlight Jellies today - Romantic evening by the beach",
                "fall" when today == 16 => "🎃 Stardew Valley Fair today - Competition and community pride",
                "fall" when today == 27 => "👻 Spirit's Eve today - Spooky fun, costumes and pranks",
                "winter" when today == 8 => "🎄 Festival of Ice today - Winter celebration, ice sculptures",
                "winter" when today == 25 => "🎁 Feast of the Winter Star today - Gift giving, family time",
                _ => GetSeasonalMood(season, today)
            };
        }

        private string GetSeasonalMood(string season, int day)
        {
            return season switch
            {
                "spring" => day < 10 ? "Early spring energy - new beginnings, planting season" : "Mid-spring growth - everything blooming",
                "summer" => day > 20 ? "Late summer heat - peak growing season, everyone active" : "Summer energy - outdoor activities, social time",
                "fall" => day > 20 ? "Late fall preparation - getting ready for winter" : "Harvest season - busy with crops and preparation", 
                "winter" => day > 20 ? "Deep winter - quiet contemplation, indoor activities" : "Early winter - adjustment to cold, cozy indoors",
                _ => ""
            };
        }
        #endregion

        #region Special Contextual Events
        public string GetSpecialContext(NPC npc)
        {
            var context = "";
            var player = Game1.player;
            
            // Check if player is married to someone else
            if (player.spouse != null && player.spouse != npc.Name)
            {
                context += $"💔 Player is married to {player.spouse} - affects romantic NPCs\n";
            }

            // Check recent gifts
            if (player.friendshipData.ContainsKey(npc.Name))
            {
                var friendData = player.friendshipData[npc.Name];
                var lastGiftDate = friendData.LastGiftDate;
                
                if (lastGiftDate != null)
                {
                    var currentDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
                    var daysSinceGift = currentDate.TotalDays - lastGiftDate.TotalDays;
                    
                    if (daysSinceGift <= 3)
                    {
                        context += $"🎁 Player gave me a gift recently - I should remember and be grateful\n";
                    }
                }
            }

            // Check if it's the NPC's birthday
            if (IsNPCBirthday(npc.Name))
            {
                context += $"🎂 It's my birthday today! I should be excited and expecting gifts\n";
            }

            // Check community center completion
            if (Game1.MasterPlayer.hasCompletedCommunityCenter())
            {
                context += $"🏛️ Community Center restored - town feels more alive and hopeful\n";
            }
            else if (Game1.MasterPlayer.hasOrWillReceiveMail("jojaCraftsRoom"))
            {
                context += $"🏢 JojaMart route taken - some NPCs disappointed, others don't care\n";
            }

            return context;
        }

        private bool IsNPCBirthday(string npcName)
        {
            // Birthday data for major NPCs
            var birthdays = new Dictionary<string, (string season, int day)>
            {
                ["Abigail"] = ("fall", 13),
                ["Alex"] = ("summer", 13), 
                ["Elliott"] = ("fall", 5),
                ["Emily"] = ("spring", 27),
                ["Haley"] = ("spring", 14),
                ["Harvey"] = ("winter", 14),
                ["Leah"] = ("winter", 23),
                ["Maru"] = ("summer", 10),
                ["Penny"] = ("fall", 2),
                ["Sam"] = ("summer", 17),
                ["Sebastian"] = ("winter", 10),
                ["Shane"] = ("spring", 20)
            };

            if (birthdays.ContainsKey(npcName))
            {
                var (season, day) = birthdays[npcName];
                return Game1.currentSeason == season && Game1.dayOfMonth == day;
            }

            return false;
        }
        #endregion

        #region Public Interface Methods
        public NPCPersonality? GetPersonality(string npcName)
        {
            personalities.TryGetValue(npcName, out var personality);
            return personality;
        }

        public NPCScheduleInfo? GetScheduleInfo(string npcName)
        {
            scheduleInfo.TryGetValue(npcName, out var schedule);
            return schedule;
        }

        public string GetContextualInfo(NPC npc, string season, string location)
        {
            var personality = GetPersonality(npc.Name);
            if (personality == null) return "";

            var contextInfo = "";

            // Add seasonal context if available
            if (personality.SeasonalNotes?.ContainsKey(season) == true)
            {
                contextInfo += $"SEASONAL CONTEXT ({season}): {personality.SeasonalNotes[season]}\n";
            }

            // Add location context if available  
            if (personality.LocationContext?.ContainsKey(location) == true)
            {
                contextInfo += $"LOCATION CONTEXT ({location}): {personality.LocationContext[location]}\n";
            }

            // Add festival context
            var festivalContext = GetFestivalContext();
            if (!string.IsNullOrEmpty(festivalContext))
            {
                contextInfo += $"EVENT CONTEXT: {festivalContext}\n";
            }

            // Add special context
            var specialContext = GetSpecialContext(npc);
            if (!string.IsNullOrEmpty(specialContext))
            {
                contextInfo += $"SPECIAL CONTEXT:\n{specialContext}";
            }

            return contextInfo;
        }

        public List<string> GetAllNPCNames()
        {
            return personalities.Keys.ToList();
        }

        public bool HasPersonality(string npcName)
        {
            return personalities.ContainsKey(npcName);
        }

        public string GetPersonalityDebugInfo(string npcName)
        {
            var personality = GetPersonality(npcName);
            if (personality == null) return $"No personality data for {npcName}";

            return $@"=== {npcName.ToUpper()} DEBUG INFO ===
Traits: {string.Join(", ", personality.CoreTraits)}
Interests: {string.Join(", ", personality.Interests)}
Current Context Available: {personality.LocationContext?.Count ?? 0} locations, {personality.WeatherMoods?.Count ?? 0} weather moods
Schedule Info: {(scheduleInfo.ContainsKey(npcName) ? "Available" : "Not available")}";
        }
        #endregion
    }

    #region Supporting Classes
    public class NPCPersonality
    {
        public string Name { get; set; } = "";
        public string Age { get; set; } = "";
        public string Occupation { get; set; } = "";
        public string[] CoreTraits { get; set; } = Array.Empty<string>();
        public string BackgroundSummary { get; set; } = "";
        public string[] SpeechPatterns { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Relationships { get; set; } = new();
        public string[] Interests { get; set; } = Array.Empty<string>();
        public string[] Dislikes { get; set; } = Array.Empty<string>();
        public Dictionary<string, string>? LocationContext { get; set; }
        public Dictionary<string, string>? SeasonalNotes { get; set; }
        public Dictionary<string, string>? WeatherMoods { get; set; }
        public Dictionary<string, string>? TimeOfDayMoods { get; set; }
        public Dictionary<string, string>? FriendshipMoods { get; set; }
    }

    public class NPCScheduleInfo
    {
        public Dictionary<string, string> WorkingHours { get; set; } = new();
        public string PreferredWeather { get; set; } = "";
        public string LeastFavoriteWeather { get; set; } = "";
        public Dictionary<string, int> EnergyByTime { get; set; } = new();
    }
    #endregion
}
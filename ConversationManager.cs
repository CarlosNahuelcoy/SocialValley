using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace SocialValley
{
    public class ConversationManager
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private Dictionary<string, List<ConversationEntry>> npcConversations;
        private readonly string saveFileName = "SocialValley-conversations.json";

        public ConversationManager(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.npcConversations = new Dictionary<string, List<ConversationEntry>>();
        }

        public void LoadConversations()
        {
            try
            {
                var saveData = helper.Data.ReadSaveData<Dictionary<string, List<ConversationEntry>>>(saveFileName);
                if (saveData != null)
                {
                    npcConversations = saveData;
                    monitor.Log($"Loaded conversations for {npcConversations.Count} NPCs", LogLevel.Debug);
                }
                else
                {
                    npcConversations = new Dictionary<string, List<ConversationEntry>>();
                    monitor.Log("No previous conversations found, starting fresh", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Error loading conversations: {ex.Message}", LogLevel.Error);
                npcConversations = new Dictionary<string, List<ConversationEntry>>();
            }
        }

        public void SaveConversations()
        {
            try
            {
                helper.Data.WriteSaveData(saveFileName, npcConversations);
                monitor.Log($"Saved conversations for {npcConversations.Count} NPCs", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                monitor.Log($"Error saving conversations: {ex.Message}", LogLevel.Error);
            }
        }

        public void AddMessage(NPC npc, string message, bool isFromPlayer)
        {
            string npcKey = GetNPCKey(npc);

            if (!npcConversations.ContainsKey(npcKey))
            {
                npcConversations[npcKey] = new List<ConversationEntry>();
            }

            var entry = new ConversationEntry
            {
                Message = message,
                IsFromPlayer = isFromPlayer,
                Timestamp = DateTime.Now,
                GameDate = new GameDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth),
                Location = Game1.currentLocation?.Name ?? "Unknown"
            };

            npcConversations[npcKey].Add(entry);

            // Mantener solo las últimas 50 entradas por NPC para evitar archivos muy grandes
            if (npcConversations[npcKey].Count > 50)
            {
                npcConversations[npcKey].RemoveAt(0);
            }

            // Auto-save después de cada mensaje
            SaveConversations();

            monitor.Log($"Added message to {npc.Name}: {message.Substring(0, Math.Min(30, message.Length))}...", LogLevel.Debug);
        }

        public List<ConversationEntry> GetConversationHistory(NPC npc)
        {
            string npcKey = GetNPCKey(npc);

            if (npcConversations.ContainsKey(npcKey))
            {
                return npcConversations[npcKey].ToList();
            }

            return new List<ConversationEntry>();
        }

        public string BuildConversationContext(NPC npc, int maxEntries = 6)
{
    var history = GetConversationHistory(npc);

    if (!history.Any())
    {
        return "This is your first conversation with the player.";
    }

    var recentEntries = history.TakeLast(maxEntries).ToList();
    
    // ✅ CONSTRUIR contexto como antes (SIN cambios)
    var context = "Previous conversations:\n";

    foreach (var entry in recentEntries)
    {
        string speaker = entry.IsFromPlayer ? "Player" : npc.Name;
        string timeInfo = entry.GameDate != null ?
            $" ({entry.GameDate.Season} {entry.GameDate.DayOfMonth}, Year {entry.GameDate.Year})" : "";

        context += $"{speaker}{timeInfo}: {entry.Message}\n";
    }
    
    // ✅ SOLO AGREGAR: Análisis del flujo conversacional (SIN duplicar contexto)
    var conversationFlow = AnalyzeConversationFlow(recentEntries, npc);
    if (!string.IsNullOrEmpty(conversationFlow))
    {
        context += "\n" + conversationFlow;
    }

    return context;
}

// ✅ NUEVO: Análisis mínimo y específico del flujo
private string AnalyzeConversationFlow(List<ConversationEntry> recentEntries, NPC npc)
{
    var analysis = "";
    
    // Solo analizar si hay suficientes mensajes
    if (recentEntries.Count < 4) return "";
    
    var npcMessages = recentEntries.Where(e => !e.IsFromPlayer).ToList();
    var playerMessages = recentEntries.Where(e => e.IsFromPlayer).ToList();
    
    // ✅ ANÁLISIS 1: Longitud de conversación
    if (npcMessages.Count >= 3)
    {
        analysis += "CONVERSATION FLOW:\n";
        analysis += "- You've exchanged several messages with the player\n";
        analysis += "- Ensure you're building on their responses, not just repeating your usual topics\n";
    }
    
    // ✅ ANÁLISIS 2: Último mensaje del jugador
    var lastPlayerMessage = playerMessages.LastOrDefault();
    if (lastPlayerMessage != null)
    {
        var message = lastPlayerMessage.Message;
        
        if (message.Contains("?") || message.Contains("¿"))
        {
            analysis += "- The player just asked you a question - make sure to address it directly\n";
        }
        
        if (message.Length > 60)
        {
            analysis += "- The player shared something detailed - show genuine interest and respond thoughtfully\n";
        }
        
        if (message.Length < 15)
        {
            analysis += "- The player gave a brief response - consider asking them something or sharing more\n";
        }
    }
    
    return analysis;
}
        // ✅ REEMPLAZAR este método completo
        public string BuildGameContext(NPC npc)
        {
            var context = $"Current game context:\n";

            // ✅ INFORMACIÓN BÁSICA (siempre incluida)
            context += $"- Date: {Game1.currentSeason} {Game1.dayOfMonth}, Year {Game1.year}\n";
            context += $"- Time: {Game1.timeOfDay / 100}:{Game1.timeOfDay % 100:D2}\n";

            // ✅ CLIMA Y EVENTOS UNIFICADOS
            var weatherAndEvents = GetUnifiedWeatherAndEventContext();
            context += weatherAndEvents;

            context += $"- Location: {Game1.currentLocation?.DisplayName ?? "Unknown"}\n";

            // Información de amistad
            try
            {
                int friendshipLevel = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
                context += $"- Friendship with {npc.Name}: {friendshipLevel} hearts\n";

                var player = Game1.player;
                context += $"- Player name: {player.Name}\n";
                context += $"- Farm name: {player.farmName}\n";
                context += $"- Farm type: {GetFarmTypeDescription()}\n";

                // ✅ CONTEXTO VISUAL REDUCIDO (no obsesivo)
                var visualContext = GetRealisticPlayerContext(npc);
                if (!string.IsNullOrEmpty(visualContext))
                {
                    context += visualContext;
                }

                // Usar NPCPersonalityManager si está disponible
                if (ModEntry.PersonalityManager != null)
                {
                    string personalityContext = ModEntry.PersonalityManager.GetContextualInfo(
                        npc,
                        Game1.currentSeason,
                        Game1.currentLocation?.Name ?? "Unknown"
                    );

                    if (!string.IsNullOrEmpty(personalityContext))
                    {
                        context += "\n" + personalityContext;
                    }

                    var personality = ModEntry.PersonalityManager.GetPersonality(npc.Name);
                    if (personality != null)
                    {
                        context += $"\nCHARACTER BACKGROUND: {personality.BackgroundSummary}\n";
                        context += $"CURRENT INTERESTS: {string.Join(", ", personality.Interests)}\n";

                        var timeCategory = GetTimeOfDayCategory(Game1.timeOfDay);
                        var weather = GetWeatherDescription();

                        if (personality.TimeOfDayMoods?.ContainsKey(timeCategory) == true)
                        {
                            context += $"TIME MOOD: {personality.TimeOfDayMoods[timeCategory]}\n";
                        }

                        if (personality.WeatherMoods?.ContainsKey(weather) == true)
                        {
                            context += $"WEATHER MOOD: {personality.WeatherMoods[weather]}\n";
                        }
                    }
                }
                else
                {
                    context += GetNPCSpecificContext(npc);
                }

                // ✅ CONTEXTO ESTACIONAL REALISTA
                context += GetRealisticSeasonalContext();

            }
            catch (Exception ex)
            {
                monitor.Log($"Error building game context: {ex.Message}", LogLevel.Debug);
            }

            return context;
        }

        // ✅ NUEVO MÉTODO: Contexto unificado de clima y eventos
private string GetUnifiedWeatherAndEventContext()
{
    var context = "";
    
    // ✅ CLIMA ACTUAL con detalles realistas
    var weather = GetDetailedWeatherDescription();
    context += $"- Weather: {weather}\n";
    
    // ✅ EVENTOS ESPECIALES (festivales, cumpleaños, etc.)
    var events = GetCurrentEventsContext();
    if (events.Count > 0)
    {
        context += $"- Special Events: {string.Join(", ", events)}\n";
    }
    
    // ✅ CONTEXTO TEMPORAL (hora del día con atmosfera)
    var timeContext = GetAtmosphericTimeContext();
    if (!string.IsNullOrEmpty(timeContext))
    {
        context += $"- Atmosphere: {timeContext}\n";
    }
    
    return context;
}

// ✅ NUEVO MÉTODO: Descripción detallada del clima
private string GetDetailedWeatherDescription()
{
    var baseWeather = GetWeatherDescription(); // Tu método existente
    var details = new List<string> { baseWeather };
    
    // Agregar detalles atmosféricos
    var time = Game1.timeOfDay;
    var season = Game1.currentSeason;
    
    if (Game1.isRaining)
    {
        if (Game1.isLightning)
        {
            details.Add("thunderstorm with lightning");
        }
        else
        {
            details.Add(time < 1200 ? "morning drizzle" : time > 1800 ? "evening rain" : "steady rainfall");
        }
    }
    else if (Game1.isSnowing)
    {
        details.Add(time < 1000 ? "morning snowfall" : "gentle snow");
    }
    else
    {
        // Clima claro con detalles estacionales
        details.Add(season switch
        {
            "spring" => time < 1200 ? "crisp spring morning" : "warm spring day",
            "summer" => time > 1400 ? "hot summer afternoon" : "bright summer day", 
            "fall" => time > 1600 ? "cool autumn evening" : "pleasant fall day",
            "winter" => time < 1000 ? "cold winter morning" : "chilly winter day",
            _ => "clear day"
        });
    }
    
    return string.Join(" - ", details.Take(2)); // Máximo 2 detalles
}

// ✅ NUEVO MÉTODO: Detectar múltiples eventos actuales
private List<string> GetCurrentEventsContext()
{
    var events = new List<string>();
    var today = Game1.dayOfMonth;
    var season = Game1.currentSeason;
    
    // ✅ FESTIVALES
    var festival = GetTodaysFestival(season, today);
    if (!string.IsNullOrEmpty(festival))
    {
        events.Add(festival);
    }
    
    // ✅ CUMPLEAÑOS
    var birthday = GetTodaysBirthdays(season, today);
    if (birthday.Count > 0)
    {
        events.Add($"{string.Join(" and ", birthday)}'s birthday");
    }
    
    // ✅ EVENTOS ESPECIALES DEL JUEGO
    var gameEvents = GetGameSpecificEvents();
    events.AddRange(gameEvents);
    
    // ✅ EVENTOS ESTACIONALES
    var seasonalEvent = GetSeasonalEvent(season, today);
    if (!string.IsNullOrEmpty(seasonalEvent))
    {
        events.Add(seasonalEvent);
    }
    
    return events;
}

// ✅ NUEVO MÉTODO: Festivales específicos
private string GetTodaysFestival(string season, int day)
{
    return season switch
    {
        "spring" when day == 13 => "Egg Festival",
        "spring" when day == 24 => "Flower Dance",
        "summer" when day == 11 => "Luau",
        "summer" when day == 28 => "Moonlight Jellies",
        "fall" when day == 16 => "Stardew Valley Fair",
        "fall" when day == 27 => "Spirit's Eve",
        "winter" when day == 8 => "Festival of Ice",
        "winter" when day == 25 => "Feast of the Winter Star",
        _ => ""
    };
}

// ✅ NUEVO MÉTODO: Cumpleaños múltiples
private List<string> GetTodaysBirthdays(string season, int day)
{
    var birthdays = new List<string>();
    
    // Base de datos de cumpleaños
    var birthdayData = new Dictionary<(string, int), string>
    {
        [("spring", 14)] = "Haley",
        [("spring", 20)] = "Shane", 
        [("spring", 27)] = "Emily",
        [("summer", 10)] = "Maru",
        [("summer", 13)] = "Alex",
        [("summer", 17)] = "Sam",
        [("fall", 2)] = "Penny",
        [("fall", 5)] = "Elliott",
        [("fall", 13)] = "Abigail",
        [("winter", 10)] = "Sebastian",
        [("winter", 14)] = "Harvey",
        [("winter", 23)] = "Leah"
    };
    
    if (birthdayData.TryGetValue((season, day), out var person))
    {
        birthdays.Add(person);
    }
    
    return birthdays;
}

// ✅ NUEVO MÉTODO: Eventos específicos del juego
private List<string> GetGameSpecificEvents()
{
    var events = new List<string>();
    
    if (Game1.MasterPlayer.hasCompletedCommunityCenter())
    {
        events.Add("Community Center restored");
    }
    else if (Game1.MasterPlayer.hasOrWillReceiveMail("jojaCraftsRoom"))
    {
        events.Add("JojaMart development completed");
    }
    
    // Eventos de matrimonio
    if (Game1.player.spouse != null)
    {
        events.Add($"married to {Game1.player.spouse}");
    }
    
    return events;
}

// ✅ NUEVO MÉTODO: Eventos estacionales específicos
private string GetSeasonalEvent(string season, int day)
{
    return season switch
    {
        "spring" when day <= 7 => "early planting season",
        "spring" when day >= 25 => "late spring growth",
        "summer" when day >= 25 => "harvest season approaching",
        "fall" when day >= 20 => "preparing for winter",
        "fall" when day <= 10 => "harvest season",
        "winter" when day >= 20 => "deep winter quiet time",
        "winter" when day <= 10 => "early winter adjustment",
        _ => ""
    };
}

        // ✅ NUEVO MÉTODO: Contexto atmosférico por hora
        private string GetAtmosphericTimeContext()
        {
            var time = Game1.timeOfDay;
            var season = Game1.currentSeason;

            return time switch
            {
                < 600 => "pre-dawn darkness",
                < 900 => $"early {season} morning",
                < 1200 => $"pleasant {season} morning",
                < 1400 => $"midday {season} warmth",
                < 1700 => $"afternoon {season} light",
                < 2000 => $"{season} evening",
                < 2200 => $"late {season} evening",
                _ => $"nighttime {season} quiet"
            };
        }

// ✅ NUEVO MÉTODO: Contexto del jugador más realista y menos repetitivo
private string GetRealisticPlayerContext(NPC npc)
{
    if (!IsNPCActuallyNear(npc))
    {
        return "";
    }
    
    var player = Game1.player;
    var observations = new List<string>();
    
    // ✅ INFORMACIÓN COMPLETA PARA LA IA (pero no mencionada automáticamente)
    var silentContext = BuildSilentPlayerContext(player);
    
    // ✅ SOLO MENCIONES ACTIVAS (lo que dirán espontáneamente)
    var physicalState = GetNoteworthyPhysicalState(player);
    if (!string.IsNullOrEmpty(physicalState))
    {
        observations.Add($"Player looks {physicalState}");
    }
    
    var tool = GetRelevantToolContext(player, npc);
    if (!string.IsNullOrEmpty(tool))
    {
        observations.Add(tool);
    }
    
    var locationContext = GetLocationSpecificContext(player, npc);
    if (!string.IsNullOrEmpty(locationContext))
    {
        observations.Add(locationContext);
    }
    
    // ✅ COMBINAR: Contexto silencioso + menciones activas
    var context = silentContext; // Información que saben pero no mencionan
    
    // Máximo 2 observaciones activas
    if (observations.Count > 0)
    {
        var limitedObservations = observations.Take(2);
        context += $"- Currently observing: {string.Join(" and ", limitedObservations)}\n";
    }
    
    return context;
}

// ✅ NUEVO MÉTODO: Información que saben pero no mencionan automáticamente
private string BuildSilentPlayerContext(Farmer player)
{
    var context = "\nPlayer's current state (you can see this but don't mention unless asked):\n";
    
    // Herramienta en mano
    var currentTool = player.CurrentTool;
    if (currentTool != null)
    {
        var toolDescription = GetToolDescription(currentTool);
        context += $"- Holding: {toolDescription}\n";
    }
    
    // Apariencia básica
    if (player.hat.Value != null)
    {
        context += $"- Wearing: {player.hat.Value.Name.ToLower()}\n";
    }
    
    // Items muy obvios
    var obviousItems = GetBasicInventoryContext(player);
    if (!string.IsNullOrEmpty(obviousItems))
    {
        context += $"- Carrying: {obviousItems}\n";
    }
    
    // Estado físico general
    var staminaRatio = player.Stamina / (float)player.MaxStamina;
    var healthRatio = player.health / (float)player.maxHealth;
    
    if (staminaRatio < 0.5f)
        context += $"- Energy: Looking somewhat tired\n";
    if (healthRatio < 0.7f)
        context += $"- Health: Has some visible injuries\n";
    
    context += "\nIMPORTANT: Only mention these details if the player specifically asks or if it's directly relevant to the conversation topic.\n";
    
    return context;
}

// ✅ MÉTODO SIMPLIFICADO: Items básicos sin obsesión
private string GetBasicInventoryContext(Farmer player)
{
    var items = new List<string>();
    
    foreach (var item in player.Items.Take(12)) // Solo hotbar
    {
        if (item == null) continue;
        
        if (item.Stack > 10)
        {
            if (item.Category == StardewValley.Object.FishCategory)
                items.Add("fish");
            else if (item.Category == StardewValley.Object.VegetableCategory)
                items.Add("vegetables");
            else if (item.Category == StardewValley.Object.FruitsCategory)
                items.Add("fruits");
            else if (item.Name.Contains("Wood"))
                items.Add("wood");
            else if (item.Name.Contains("Stone"))
                items.Add("stone");
        }
        
        if (items.Count >= 2) break; // Máximo 2 tipos
    }
    
    return items.Count > 0 ? string.Join(", ", items) : "";
}

// ✅ NUEVO MÉTODO: Solo estados físicos muy notorios
private string GetNoteworthyPhysicalState(Farmer player)
{
    var staminaRatio = player.Stamina / (float)player.MaxStamina;
    var healthRatio = player.health / (float)player.maxHealth;
    
    // Solo mencionar estados extremos
    if (healthRatio < 0.2f)
        return "badly injured";
    if (staminaRatio < 0.1f)
        return "completely exhausted";
        
    return "";
}

// ✅ NUEVO MÉTODO: Herramientas relevantes por NPC
private string GetRelevantToolContext(Farmer player, NPC npc)
{
    var currentTool = player.CurrentTool;
    if (currentTool == null) return "";
    
    // Solo mencionar si es relevante para el NPC específico
    var toolName = GetToolDescription(currentTool);
    
    return npc.Name.ToLower() switch
    {
        "clint" when toolName.Contains("pickaxe") || toolName.Contains("axe") => $"Player brought a {toolName} to show Clint",
        "willy" when toolName.Contains("fishing rod") => $"Player has fishing gear ready",
        "harvey" when toolName.Contains("weapon") => $"Player looks injured from combat",
        _ => "" // No mencionar herramientas para otros NPCs
    };
}

// ✅ NUEVO MÉTODO: Contexto específico de ubicación
private string GetLocationSpecificContext(Farmer player, NPC npc)
{
    var location = Game1.currentLocation?.Name ?? "";
    
    return location switch
    {
        var l when l.Contains("Mine") => "Player emerged from the mines looking dusty",
        "Beach" when Game1.timeOfDay > 1800 => "Player is enjoying the evening beach atmosphere",
        "Desert" => "Player traveled far to reach the desert",
        _ => ""
    };
}

// ✅ MÉTODO AUXILIAR: Verificar si el NPC está realmente cerca
private bool IsNPCActuallyNear(NPC npc)
{
    var currentLocation = Game1.currentLocation;
    
    // Solo describir apariencia si están en la misma ubicación
    if (currentLocation?.characters?.Contains(npc) == true)
    {
        return true;
    }
    
    return false;
}

        // ✅ NUEVO MÉTODO: Contexto estacional realista
        private string GetRealisticSeasonalContext()
        {
            var season = Game1.currentSeason;
            var day = Game1.dayOfMonth;

            var seasonal = season switch
            {
                "spring" when day <= 7 => "- Season Context: New growth everywhere, farmers busy planting\n",
                "spring" when day >= 20 => "- Season Context: Everything blooming, energy of renewal\n",
                "summer" when day >= 20 => "- Season Context: Peak growing season, long warm days\n",
                "fall" when day <= 15 => "- Season Context: Harvest time, preparation for winter\n",
                "fall" when day >= 20 => "- Season Context: Autumn colors, getting colder\n",
                "winter" => "- Season Context: Quiet season, time for relationships and indoor activities\n",
                _ => ""
            };

            return seasonal;
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
        private string GetNPCSpecificContext(NPC npc)
        {
            var context = "";

            // Contexto específico por NPC basado en sus características
            switch (npc.Name.ToLower())
            {
                case "abigail":
                    context += "- Abigail loves adventure and exploring the mines\n";
                    break;
                case "penny":
                    context += "- Penny is the local teacher and lives in a trailer with her mom Pam\n";
                    break;
                case "sebastian":
                    context += "- Sebastian is a programmer who works from home and likes motorcycles\n";
                    break;
                case "sam":
                    context += "- Sam works at JojaMart and loves music, especially his band\n";
                    break;
                case "alex":
                    context += "- Alex is athletic and dreams of playing professional gridball\n";
                    break;
                case "harvey":
                    context += "- Harvey is the town doctor and has a passion for radio and airplanes\n";
                    break;
                case "shane":
                    context += "- Shane works at JojaMart and raises chickens, struggles with depression\n";
                    break;
                case "elliott":
                    context += "- Elliott is a romantic writer living in a cabin on the beach\n";
                    break;
                case "emily":
                    context += "- Emily works at the Saloon and loves crystals, meditation, and fashion\n";
                    break;
                case "haley":
                    context += "- Haley loves photography and fashion, initially seems superficial but has depth\n";
                    break;
                case "leah":
                    context += "- Leah is an artist who moved from the city to focus on her sculptures\n";
                    break;
                case "maru":
                    context += "- Maru is a brilliant inventor and nurse, Demetrius and Robin's daughter\n";
                    break;
            }

            return context;
        }

        // ✅ OPCIONAL: Método mejorado para bienvenidas que usa PersonalityManager
    public string GetContextualWelcomePrompt(NPC npc)
    {
        if (ModEntry.PersonalityManager == null)
        {
            return "The player just approached you. Give a natural greeting.";
        }

        var personality = ModEntry.PersonalityManager.GetPersonality(npc.Name);
        if (personality == null)
        {
            return "The player just approached you. Give a natural greeting.";
        }

        // Obtener contexto actual del juego
        var currentTime = Game1.timeOfDay;
        var currentSeason = Game1.currentSeason;
        var currentLocation = Game1.currentLocation?.Name ?? "Unknown";
        var weather = GetWeatherDescription();
        var timeOfDay = GetTimeOfDayCategory(currentTime);
        var friendshipLevel = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
        
        // Contexto específico basado en nivel de amistad
        string relationshipContext = friendshipLevel switch
        {
            0 => "This is the first time the player has approached you for a conversation",
            1 => "You've met the player once or twice before, but don't know them well yet",
            2 => "You're starting to recognize the player as a regular face around town",
            <= 4 => "You consider the player a friendly acquaintance", 
            <= 6 => "The player has become a good friend",
            <= 8 => "You and the player are close friends",
            _ => "The player is one of your best friends in town"
        };

        string welcomePrompt = $@"The player has just approached you for the first time in this conversation. Give a natural greeting that shows your personality.

    CURRENT SITUATION:
    - Time: {currentTime / 100}:{currentTime % 100:D2} ({timeOfDay})
    - Season: {currentSeason}  
    - Location: {currentLocation}
    - Weather: {weather}
    - Your relationship: {relationshipContext} ({friendshipLevel} hearts)

    YOUR CHARACTER DETAILS:
    {personality.BackgroundSummary}

    PERSONALITY TRAITS: {string.Join(", ", personality.CoreTraits)}

    CURRENT MOOD CONTEXT:";

        // Agregar contextos específicos si existen
        if (personality.TimeOfDayMoods?.ContainsKey(timeOfDay) == true)
        {
            welcomePrompt += $"\n- Time Mood: {personality.TimeOfDayMoods[timeOfDay]}";
        }

        if (personality.WeatherMoods?.ContainsKey(weather) == true)
        {
            welcomePrompt += $"\n- Weather Mood: {personality.WeatherMoods[weather]}";
        }

        if (personality.LocationContext?.ContainsKey(currentLocation) == true)
        {
            welcomePrompt += $"\n- Location Context: {personality.LocationContext[currentLocation]}";
        }

        welcomePrompt += $@"

    GREETING INSTRUCTIONS:
    - Give a natural, in-character greeting as {npc.Name}
    - Reference the current context (time, weather, location) if it makes sense for your personality
    - Show your personality immediately through your greeting style
    - Keep it brief (1-2 sentences max)
    - React authentically based on your current mood and energy level
    - Your friendship level affects how warm or distant you are

    Remember: This is just a GREETING, not a full conversation starter.";

        return welcomePrompt;
    }

        private bool IsNPCNearPlayer(NPC npc)
        {
            var currentLocation = Game1.currentLocation;

            // Verificar si el NPC está en la ubicación actual
            if (currentLocation?.characters?.Contains(npc) == true)
            {
                return true;
            }

            // O si estamos en la ubicación donde normalmente está el NPC
            if (currentLocation?.Name == npc.DefaultMap)
            {
                return true;
            }

            return false;
        }

private string GetToolDescription(Tool tool)
{
    if (tool is StardewValley.Tools.FishingRod)
        return "fishing rod";
    else if (tool is StardewValley.Tools.Pickaxe)
        return "pickaxe";
    else if (tool is StardewValley.Tools.Axe)
        return "axe";
    else if (tool is StardewValley.Tools.Hoe)
        return "hoe";
    else if (tool is StardewValley.Tools.WateringCan)
        return "watering can";
    else if (tool is StardewValley.Tools.MeleeWeapon)
        return "weapon";
    else if (tool is StardewValley.Tools.Slingshot)
        return "slingshot";
    else if (tool is StardewValley.Tools.MilkPail)
        return "milk pail";
    else if (tool is StardewValley.Tools.Shears)
        return "shears";
    else
        return tool.Name?.ToLower() ?? "unknown tool";
}
        private string GetWeatherDescription()
        {
            if (Game1.isRaining)
                return Game1.isLightning ? "Stormy" : "Rainy";
            if (Game1.isSnowing)
                return "Snowy";
            if (Game1.isDebrisWeather)
                return "Windy";

            return "Sunny";
        }

        private string GetFarmTypeDescription()
        {
            // Esto puede variar según la versión del juego
            try
            {
                return Game1.whichFarm switch
                {
                    0 => "Standard Farm",
                    1 => "Riverland Farm",
                    2 => "Forest Farm",
                    3 => "Hilltop Farm",
                    4 => "Wilderness Farm",
                    5 => "Four Corners Farm",
                    6 => "Beach Farm",
                    _ => "Unknown Farm Type"
                };
            }
            catch
            {
                return "Unknown Farm Type";
            }
        }

        private string GetNPCKey(NPC npc)
        {
            // Usar nombre del NPC como clave
            return npc.Name;
        }

        public void ClearConversation(NPC npc)
        {
            string npcKey = GetNPCKey(npc);
            if (npcConversations.ContainsKey(npcKey))
            {
                npcConversations[npcKey].Clear();
                SaveConversations();
                monitor.Log($"Cleared conversation history for {npc.Name}", LogLevel.Info);
            }
        }

        public int GetConversationCount(NPC npc)
        {
            string npcKey = GetNPCKey(npc);
            return npcConversations.ContainsKey(npcKey) ? npcConversations[npcKey].Count : 0;
        }
    }

    public class ConversationEntry
    {
        public string Message { get; set; } = "";
        public bool IsFromPlayer { get; set; }
        public DateTime Timestamp { get; set; }
        public GameDate? GameDate { get; set; }
        public string Location { get; set; } = "";
    }

    public class GameDate
    {
        public int Year { get; set; }
        public string Season { get; set; } = "";
        public int DayOfMonth { get; set; }

        public GameDate() { }

        public GameDate(int year, string season, int dayOfMonth)
        {
            Year = year;
            Season = season;
            DayOfMonth = dayOfMonth;
        }
    }
    
    
}
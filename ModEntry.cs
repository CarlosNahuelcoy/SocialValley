using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SocialValley
{
    public class ModEntry : Mod
    {
        private UnifiedAIClient? aiClient;
        private ConversationManager? conversationManager;
        private LanguageManager? languageManager;
        private IModHelper? helper;
        private static bool isChatOpen = false;
        private NPCPersonalityManager? personalityManager;
        
        // ===== HEALTH PING =====
        private int tickCounter = 0;
        private const int HEALTH_PING_INTERVAL_TICKS = 3600; // ~60 segundos a 60fps

        public static bool IsChatOpen
        {
            get => isChatOpen;
            set => isChatOpen = value;
        }

        public static ConversationManager? ConversationManager { get; private set; }
        public static LanguageManager? LanguageManager { get; private set; }
        public static NPCPersonalityManager? PersonalityManager { get; private set; }
        
        private static ConfigManager? configManager;
        public static ConfigManager? ConfigManager => configManager;
        public static IModHelper? ModHelper { get; private set; }

        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            ModHelper = helper;

            configManager = new ConfigManager(helper, Monitor);
            
            languageManager = new LanguageManager(this.Monitor);
            LanguageManager = languageManager;

            aiClient = new UnifiedAIClient(this.Monitor, configManager);

            conversationManager = new ConversationManager(helper, this.Monitor);
            ConversationManager = conversationManager;
            
            personalityManager = new NPCPersonalityManager(this.Monitor);
            PersonalityManager = personalityManager;
            
            FontManager.Initialize(Monitor);

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked; // ✅ Health pings

            this.Monitor.Log($"SocialValley loaded! Initial language: {languageManager.GetCurrentLanguage()}", LogLevel.Info);
            this.Monitor.Log($"AI Provider: {configManager.GetSelectedProvider()}", LogLevel.Info);
            this.Monitor.Log("• Right-click NPCs for normal dialogue", LogLevel.Info);
            this.Monitor.Log("• Left-click NPCs anytime for AI chat", LogLevel.Info);
            this.Monitor.Log("• Conversations are automatically saved per NPC", LogLevel.Info);
            ApplyLoadedConfiguration();
        }

        // ===== HEALTH PING Y DETECCIÓN PERIÓDICA =====
        
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (aiClient == null) return;
            
            tickCounter++;
            
            if (tickCounter >= HEALTH_PING_INTERVAL_TICKS)
            {
                tickCounter = 0;
                
                // Solo hacer pings si Player2 está seleccionado
                if (configManager?.GetSelectedProvider() == AIProvider.Player2)
                {
                    _ = Task.Run(async () =>
                    {
                        // Si ya está detectado: health ping para mantener sesión activa
                        // Si no está detectado: intentar detectar de nuevo
                        if (UnifiedAIClient.IsLocalPlayer2Detected)
                        {
                            await aiClient.PerformHealthPingIfNeededAsync();
                        }
                        else
                        {
                            Monitor.Log("Retrying Player2 local detection...", LogLevel.Debug);
                            await aiClient.ForceDetectLocalPlayer2Async();
                        }
                    });
                }
            }
        }

        private void ApplyLoadedConfiguration()
        {
            if (configManager == null) return;
            
            if (configManager.Config.Language != "Auto" && languageManager != null)
            {
                languageManager.SetLanguage(configManager.Config.Language);
                Monitor.Log($"Applied saved language: {configManager.Config.Language}", LogLevel.Info);
            }
            
            var provider = configManager.GetSelectedProvider();
            if (!configManager.IsProviderConfigured(provider))
            {
                Monitor.Log($"⚠️ AI Provider {provider} is not fully configured. Please set API key and model in settings.", LogLevel.Warn);
            }
            else
            {
                Monitor.Log($"✅ AI Provider {provider} is configured", LogLevel.Info);
            }
            
            Monitor.Log($"Chat key is set to: {configManager.Config.OpenChatKey}", LogLevel.Info);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        conversationManager?.LoadConversations();
        if (languageManager != null)
        {
            if (configManager?.Config.Language == "Auto")
            {
                // Auto-detect language from game settings
                languageManager.ReDetectLanguage();
                this.Monitor.Log($"🌍 Language auto-detected: {languageManager.GetCurrentLanguage()}", LogLevel.Info);
            }
            else
            {
                // Re-apply the user's explicitly configured language
                languageManager.SetLanguage(configManager!.Config.Language);
                this.Monitor.Log($"🌍 Using configured language: {languageManager.GetCurrentLanguage()}", LogLevel.Info);
            }
        }
    }

        private void OnSaved(object? sender, SavedEventArgs e)
        {
            conversationManager?.SaveConversations();
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            this.Monitor.Log("Game launched, checking AI configuration...", LogLevel.Debug);
            
            if (configManager == null || aiClient == null) return;
            
            var provider = configManager.GetSelectedProvider();
            
            // ✅ Si el proveedor es Player2, intentar detectar la app desktop automáticamente
            if (provider == AIProvider.Player2)
            {
                this.Monitor.Log("Player2 selected - attempting to detect desktop app...", LogLevel.Info);
                
                _ = Task.Run(async () =>
                {
                    bool detected = await aiClient.ForceDetectLocalPlayer2Async();
                    
                    if (detected)
                    {
                        Monitor.Log("✅ Player2 desktop app detected and ready!", LogLevel.Info);
                    }
                    else
                    {
                        Monitor.Log("ℹ️ Player2 desktop app not found. Will use manual API key if configured.", LogLevel.Info);
                        
                        // Verificar si hay API key manual como fallback
                        var manualKey = configManager.GetCurrentApiKey();
                        if (string.IsNullOrEmpty(manualKey))
                        {
                            Monitor.Log("⚠️ No Player2 API key configured either. Please open Player2 app or add API key in Settings.", LogLevel.Warn);
                        }
                        else
                        {
                            Monitor.Log("✅ Player2 manual API key found as fallback.", LogLevel.Info);
                        }
                    }
                });
            }
            else
            {
                bool isConfigured = configManager.IsProviderConfigured(provider);
                if (isConfigured)
                    this.Monitor.Log($"✅ {provider.GetLabel()} is configured and ready!", LogLevel.Info);
                else
                    this.Monitor.Log($"⚠️ {provider.GetLabel()} is not configured. Please set API key and model in chat settings.", LogLevel.Warn);
            }
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            this.Monitor.Log($"Menu changed - Old: {e.OldMenu?.GetType().Name ?? "null"}, New: {e.NewMenu?.GetType().Name ?? "null"}", LogLevel.Info);

            if (e.OldMenu is ChatUI && e.NewMenu == null)
            {
                this.Monitor.Log("ChatUI was closed externally, cleaning up state", LogLevel.Info);

                if (IsChatOpen)
                {
                    IsChatOpen = false;
                }

                if (Game1.keyboardDispatcher.Subscriber != null)
                {
                    Game1.keyboardDispatcher.Subscriber = null;
                }
            }

            if (e.NewMenu is ChatUI && !IsChatOpen)
            {
                IsChatOpen = true;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            var configuredButton = configManager?.GetChatButton() ?? SButton.MouseLeft;
            
            if (Game1.player.UsingTool || Game1.player.isEating) return;
            
            if (Game1.player.CurrentTool is StardewValley.Tools.MeleeWeapon weapon)
            {
                if (weapon.Name != null && weapon.Name.Contains("Net") && weapon.isOnSpecial)
                    return;
            }
            
            if (Game1.player.CurrentTool is StardewValley.Tools.FishingRod rod)
            {
                if (rod.isFishing || rod.isNibbling || rod.isReeling || rod.hit)
                    return;
            }

            if (Game1.activeClickableMenu != null)
            {
                var menuType = Game1.activeClickableMenu.GetType().Name;

                var systemMenus = new[] {
                    "GameMenu", "OptionsMenu", "SaveGameMenu", "LoadGameMenu", "TitleMenu",
                    "DialogueBox", "ShopMenu", "CraftingPage", "InventoryMenu", "ItemGrabMenu",
                    "GeodeMenu", "MuseumMenu", "JunimoNoteMenu", "QuestLog", "Billboard",
                    "LetterViewerMenu", "ProfileMenu", "ShippingMenu", "LevelUpMenu",
                    "AnimalQueryMenu", "PurchaseAnimalsMenu", "CarpenterMenu", "BuildingPaintMenu", "SpecificModConfigMenu"
                };

                if (Array.IndexOf(systemMenus, menuType) >= 0) return;

                if (menuType == "ChatUI" || menuType == "EditMessageModal" || 
                    menuType == "SettingsModal" || menuType == "PersonalizeNPCModal")
                {
                    // Continuar con lógica del chat
                }
                else
                {
                    return;
                }
            }

            if (e.Button == SButton.Escape && IsChatOpen)
            {
                if (Game1.activeClickableMenu is EditMessageModal || 
                    Game1.activeClickableMenu is SettingsModal || 
                    Game1.activeClickableMenu is PersonalizeNPCModal)
                    return;

                if (Game1.activeClickableMenu is ChatUI)
                    return;

                if (Game1.activeClickableMenu == null)
                {
                    IsChatOpen = false;
                    return;
                }
            }

            if (IsChatOpen && (Game1.activeClickableMenu is ChatUI || Game1.activeClickableMenu is EditMessageModal))
            {
                switch (e.Button)
                {
                    case SButton.E:
                    case SButton.I:
                    case SButton.M:
                    case SButton.J:
                    case SButton.Tab:
                    case SButton.F:
                    case SButton.LeftShift:
                    case SButton.RightShift:
                        helper?.Input.Suppress(e.Button);
                        return;
                }
            }

            if (!Context.IsWorldReady) return;

            if (Game1.activeClickableMenu != null) return;

            // Comando de emergencia
            if (e.Button == SButton.F12 &&
                (helper?.Input.IsDown(SButton.LeftControl) == true || helper?.Input.IsDown(SButton.RightControl) == true))
            {
                this.Monitor.Log("=== EMERGENCY RESET TRIGGERED ===", LogLevel.Warn);
                IsChatOpen = false;
                if (Game1.activeClickableMenu is ChatUI)
                    Game1.activeClickableMenu = null;
                if (Game1.keyboardDispatcher.Subscriber != null)
                    Game1.keyboardDispatcher.Subscriber = null;
                this.Monitor.Log("=== EMERGENCY RESET COMPLETED ===", LogLevel.Warn);
                helper?.Input.Suppress(e.Button);
                return;
            }

            bool isChatKey = (e.Button == configuredButton);
            bool isRightClick = (e.Button == SButton.MouseRight);
            
            if (!isChatKey && !isRightClick) return;

            var targetNPC = GetNPCAtCursor() ?? GetNearbyNPC();

            if (targetNPC != null)
            {
                if (IsNonConversableCharacter(targetNPC)) return;

                var distance = Vector2.Distance(Game1.player.Position, targetNPC.Position);
                if (distance > 128f) return;

                bool isHoldingGift = IsPlayerHoldingGift();
                bool canGiveGift = CanGiveGiftToNPC(targetNPC);
                if (isHoldingGift && canGiveGift) return;
                if (isHoldingGift && !canGiveGift) return;

                if (isChatKey)
                {
                    this.Monitor.Log($"Opening AI chat with {targetNPC.Name}", LogLevel.Info);
                    helper?.Input.Suppress(e.Button);

                    if (Game1.activeClickableMenu == null && !IsChatOpen && aiClient != null)
                    {
                        Game1.activeClickableMenu = new ChatUI(this.Monitor, aiClient, targetNPC);
                        
                        // Mostrar advertencia contextual si Player2 no está listo
                        if (configManager?.GetSelectedProvider() == AIProvider.Player2)
                        {
                            if (!UnifiedAIClient.IsLocalPlayer2Detected && 
                                string.IsNullOrEmpty(configManager.GetCurrentApiKey()))
                            {
                                Game1.addHUDMessage(new HUDMessage(
                                    "⚠️ Open the Player2 app or add API key in Settings", 
                                    HUDMessage.newQuest_type));
                            }
                        }
                        else if (configManager != null && !configManager.IsProviderConfigured(configManager.GetSelectedProvider()))
                        {
                            Game1.addHUDMessage(new HUDMessage(
                                "⚠️ Please configure AI provider in Settings", 
                                HUDMessage.newQuest_type));
                        }
                    }
                    return;
                }

                if (isRightClick) return;
            }
        }
        
        private bool IsPlayerHoldingGift()
        {
            var activeObject = Game1.player.ActiveObject;
            if (activeObject == null) return false;

            var giftCategories = new[]
            {
                StardewValley.Object.FruitsCategory, StardewValley.Object.VegetableCategory,
                StardewValley.Object.flowersCategory, StardewValley.Object.GemCategory,
                StardewValley.Object.mineralsCategory, StardewValley.Object.CookingCategory,
                StardewValley.Object.artisanGoodsCategory, StardewValley.Object.EggCategory,
                StardewValley.Object.MilkCategory
            };

            if (giftCategories.Contains(activeObject.Category)) return true;

            var commonGifts = new[] {
                "Coffee", "Beer", "Wine", "Juice", "Tea", "Chocolate Cake", "Pink Cake", "Cookie",
                "Mayonnaise", "Cheese", "Goat Cheese", "Honey", "Jelly", "Pickles", "Oil",
                "Battery Pack", "Refined Quartz", "Coal"
            };

            if (commonGifts.Contains(activeObject.Name)) return true;
            if (activeObject.Quality > 0) return true;

            return false;
        }

        private bool CanGiveGiftToNPC(NPC npc)
        {
            try
            {
                if (Game1.player.friendshipData.ContainsKey(npc.Name))
                {
                    var friendship = Game1.player.friendshipData[npc.Name];
                    if (friendship.GiftsToday >= 1 || friendship.GiftsThisWeek >= 2)
                        return false;
                }
                return true;
            }
            catch { return true; }
        }

        private NPC? GetNPCAtCursor()
        {
            var cursorTile = Game1.currentCursorTile;
            var location = Game1.currentLocation;
            if (location == null) return null;

            foreach (var npc in location.characters)
            {
                if (IsNonConversableCharacter(npc)) continue;

                if (npc.Tile == cursorTile) return npc;

                var npcTile = npc.Tile;
                if (Math.Abs(npcTile.X - cursorTile.X) + Math.Abs(npcTile.Y - cursorTile.Y) <= 1.0f)
                    return npc;
            }

            var cursorPixelPos = new Vector2(
                Game1.getOldMouseX() + Game1.viewport.X,
                Game1.getOldMouseY() + Game1.viewport.Y
            );

            foreach (var npc in location.characters)
            {
                if (IsNonConversableCharacter(npc)) continue;
                if (npc.GetBoundingBox().Contains((int)cursorPixelPos.X, (int)cursorPixelPos.Y))
                    return npc;
            }

            return null;
        }

        private NPC? GetNearbyNPC()
        {
            var location = Game1.currentLocation;
            if (location == null) return null;

            NPC? closest = null;
            float closestDist = float.MaxValue;

            foreach (var npc in location.characters)
            {
                if (IsNonConversableCharacter(npc)) continue;
                float dist = Vector2.Distance(Game1.player.Position, npc.Position);
                if (dist < 128f && dist < closestDist)
                {
                    closest = npc;
                    closestDist = dist;
                }
            }

            return closest;
        }

        private bool IsNonConversableCharacter(NPC npc)
        {
            if (IsDefinitelyConversableNPC(npc)) return false;
            if (npc is Pet || npc is Horse || npc is JunimoHarvester) return true;
            if (IsHostileMonster(npc)) return true;
            if (!HasConversableCharacteristics(npc)) return true;
            if (IsInHostileLocation(npc) && LooksLikeMonster(npc)) return true;
            return false;
        }

        private bool IsHostileMonster(NPC npc)
        {
            var npcType = npc.GetType();
            if (npcType.Name.Contains("Monster") || npcType.Name.Contains("Slime") ||
                npcType.Name.Contains("Bat") || npcType.Name.Contains("Ghost") ||
                npcType.Name.Contains("Skeleton") || npcType.Name.Contains("Crab") ||
                npcType.Name.Contains("Serpent") || npcType.Name.Contains("Bug") ||
                npcType.Name.Contains("Spirit"))
                return true;

            try
            {
                var hasAttackDamage = npcType.GetProperty("attackDamage") != null;
                var hasHealth = npcType.GetProperty("health") != null && npcType.GetProperty("maxHealth") != null;
                var hasExperience = npcType.GetProperty("experienceGained") != null;
                if (hasAttackDamage && hasHealth && hasExperience) return true;
            }
            catch { }

            return false;
        }

        private bool HasConversableCharacteristics(NPC npc)
        {
            if (string.IsNullOrEmpty(npc.Name) || npc.Name.Length < 2) return false;
            if (npc.Dialogue == null) return false;
            try { if (npc.Portrait != null) return true; } catch { }
            try { if (Game1.player.friendshipData.ContainsKey(npc.Name)) return true; } catch { }
            if (npc.Speed > 4) return false;
            var namePatterns = new[] { "Slime", "Bat", "Bug", "Ghost", "Skeleton", "Monster", "Enemy" };
            if (namePatterns.Any(p => npc.Name.Contains(p))) return false;
            return true;
        }

        private bool IsInHostileLocation(NPC npc)
        {
            var location = Game1.currentLocation;
            if (location == null) return false;
            var hostileLocations = new[] { "UndergroundMine", "Mine", "SkullCave", "VolcanoDungeon", "BugLand", "Woods", "Sewer", "Caldera" };
            return hostileLocations.Any(h => location.Name.Contains(h));
        }

        private bool LooksLikeMonster(NPC npc)
        {
            if (string.IsNullOrEmpty(npc.DefaultMap)) return true;
            try
            {
                if (npc.GetType().GetProperty("objectsToDrop") != null) return true;
            }
            catch { }
            return false;
        }

        private bool IsDefinitelyConversableNPC(NPC npc)
        {
            try { if (npc.Portrait != null) return true; } catch { }
            try { if (Game1.player.friendshipData.ContainsKey(npc.Name)) return true; } catch { }
            try { if (npc.Schedule != null && npc.Schedule.Count > 0) return true; } catch { }
            return false;
        }
    }
}
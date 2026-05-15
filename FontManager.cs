using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace SocialValley
{
    public static class FontManager
    {
        private static Dictionary<string, SpriteFont?> fontCache = new Dictionary<string, SpriteFont?>();
        private static Dictionary<string, bool> fontAvailability = new Dictionary<string, bool>();
        private static IMonitor? monitor;
        private static bool hasLoggedCJKWarning = false;
        
        public static void Initialize(IMonitor mon)
        {
            monitor = mon;
            CheckAvailableFonts();
        }
        
        private static void CheckAvailableFonts()
        {
            // Verificar qué fuentes están realmente disponibles
            var currentGameLanguage = LocalizedContentManager.CurrentLanguageCode;
            
            fontAvailability["chinese"] = currentGameLanguage == LocalizedContentManager.LanguageCode.zh;
            fontAvailability["japanese"] = currentGameLanguage == LocalizedContentManager.LanguageCode.ja;
            fontAvailability["korean"] = currentGameLanguage == LocalizedContentManager.LanguageCode.ko;
            fontAvailability["russian"] = true; // Ruso generalmente funciona
            
            monitor?.Log($"Font availability - Chinese: {fontAvailability["chinese"]}, Japanese: {fontAvailability["japanese"]}, Korean: {fontAvailability["korean"]}", LogLevel.Debug);
        }
        
        public static bool IsCJKFontAvailable(string language)
        {
            language = language.ToLower();
            return fontAvailability.ContainsKey(language) && fontAvailability[language];
        }
        
        public static bool RequiresGameRestart(string language)
        {
            language = language.ToLower();
            
            // Si el usuario quiere usar CJK pero el juego no está en ese idioma
            if ((language == "chinese" || language == "japanese" || language == "korean"))
            {
                var currentGameLanguage = LocalizedContentManager.CurrentLanguageCode;
                
                if (language == "chinese" && currentGameLanguage != LocalizedContentManager.LanguageCode.zh)
                    return true;
                if (language == "japanese" && currentGameLanguage != LocalizedContentManager.LanguageCode.ja)
                    return true;
                if (language == "korean" && currentGameLanguage != LocalizedContentManager.LanguageCode.ko)
                    return true;
            }
            
            return false;
        }
        
        public static SpriteFont GetFontForLanguage(string language)
        {
            // Si ya intentamos y falló, devolver default directamente
            if (fontCache.ContainsKey(language))
            {
                return fontCache[language] ?? Game1.smallFont;
            }
            
            SpriteFont? font = null;
            
            // Solo intentar cargar fuentes CJK si el juego está en ese idioma
            var currentGameLanguage = LocalizedContentManager.CurrentLanguageCode;
            
            switch (language.ToLower())
            {
                case "chinese":
                    if (currentGameLanguage == LocalizedContentManager.LanguageCode.zh)
                    {
                        font = TryLoadFont("Fonts\\Chinese");
                    }
                    break;
                    
                case "japanese":
                    if (currentGameLanguage == LocalizedContentManager.LanguageCode.ja)
                    {
                        font = TryLoadFont("Fonts\\Japanese");
                    }
                    break;
                    
                case "korean":
                    if (currentGameLanguage == LocalizedContentManager.LanguageCode.ko)
                    {
                        font = TryLoadFont("Fonts\\Korean");
                    }
                    break;
                    
                case "russian":
                    font = TryLoadFont("Fonts\\SpriteFont1");
                    break;
                    
                default:
                    font = Game1.smallFont;
                    break;
            }
            
            // Si no se pudo cargar, usar default
            if (font == null)
            {
                font = Game1.smallFont;
                
                // Mostrar advertencia solo una vez
                if (!hasLoggedCJKWarning && (language == "chinese" || language == "japanese" || language == "korean"))
                {
                    monitor?.Log($"Cannot load {language} font - game must be restarted in {language} mode", LogLevel.Info);
                    hasLoggedCJKWarning = true;
                }
            }
            
            // Guardar en cache
            fontCache[language] = font;
            return font;
        }
        
        private static SpriteFont? TryLoadFont(string path)
        {
            try
            {
                var font = Game1.content.Load<SpriteFont>(path);
                monitor?.Log($"Successfully loaded font: {path}", LogLevel.Debug);
                return font;
            }
            catch (Exception ex)
            {
                // Solo registrar el error una vez, no spam
                if (!fontCache.ContainsKey(path))
                {
                    monitor?.Log($"Cannot load font {path}: {ex.GetType().Name}", LogLevel.Debug);
                }
                return null;
            }
        }
        
        public static SpriteFont GetFontForText(string text)
        {
            // Si el texto contiene CJK pero las fuentes no están disponibles, usar default
            if (ContainsCJKCharacters(text))
            {
                var currentGameLanguage = LocalizedContentManager.CurrentLanguageCode;
                
                if (ContainsChineseCharacters(text) && currentGameLanguage == LocalizedContentManager.LanguageCode.zh)
                    return GetFontForLanguage("chinese");
                else if (ContainsJapaneseCharacters(text) && currentGameLanguage == LocalizedContentManager.LanguageCode.ja)
                    return GetFontForLanguage("japanese");
                else if (ContainsKoreanCharacters(text) && currentGameLanguage == LocalizedContentManager.LanguageCode.ko)
                    return GetFontForLanguage("korean");
            }
            
            return Game1.smallFont;
        }
        
        public static bool ContainsChineseCharacters(string text)
        {
            foreach (char c in text)
            {
                if ((c >= 0x4E00 && c <= 0x9FFF) || // CJK Unified Ideographs (Chinese)
                    (c >= 0x3400 && c <= 0x4DBF))    // CJK Extension A
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool ContainsJapaneseCharacters(string text)
        {
            foreach (char c in text)
            {
                if ((c >= 0x3040 && c <= 0x309F) || // Hiragana
                    (c >= 0x30A0 && c <= 0x30FF) || // Katakana
                    (c >= 0xFF65 && c <= 0xFF9F))   // Half-width Katakana
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool ContainsKoreanCharacters(string text)
        {
            foreach (char c in text)
            {
                if ((c >= 0xAC00 && c <= 0xD7AF) || // Hangul Syllables
                    (c >= 0x1100 && c <= 0x11FF) || // Hangul Jamo
                    (c >= 0x3130 && c <= 0x318F))   // Hangul Compatibility Jamo
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool ContainsCJKCharacters(string text)
        {
            return ContainsChineseCharacters(text) || 
                   ContainsJapaneseCharacters(text) || 
                   ContainsKoreanCharacters(text);
        }
        
        public static void ClearCache()
        {
            fontCache.Clear();
            monitor?.Log("Font cache cleared", LogLevel.Debug);
        }
    }
}
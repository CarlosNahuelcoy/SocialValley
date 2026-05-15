using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SocialValley
{
    public class LanguageManager
    {
        private readonly IMonitor monitor;
        private string currentLanguage = "English";
        private Dictionary<string, LanguageConfig> languageConfigs;

        public LanguageManager(IMonitor monitor)
        {
            this.monitor = monitor;
            InitializeLanguageConfigs();
            DetectGameLanguage();
        }

        private void InitializeLanguageConfigs()
        {
            languageConfigs = new Dictionary<string, LanguageConfig>
            {
                ["English"] = new LanguageConfig
                {
                    Code = "en",
                    SystemPromptAddition = "Respond in English with natural conversation flow.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "Hello! It's nice to see you. What would you like to chat about?",
                        ["abigail"] = "Hey! What's up? Want to talk about something?",
                        ["penny"] = "Hello! How are you doing today?",
                        ["sebastian"] = "Oh, hey...",
                        ["sam"] = "Yo! How's it going?",
                        ["alex"] = "Hey there!",
                        ["harvey"] = "Hello! I hope you're feeling well.",
                        ["elliott"] = "Ah, greetings!",
                        ["shane"] = "...What do you want?",
                        ["haley"] = "Oh, it's you.",
                        ["leah"] = "Hi there! Nice to see you.",
                        ["maru"] = "Hello! I was just working on something.",
                        ["emily"] = "Hi! Great energy today!"
                    }
                },
                ["Spanish"] = new LanguageConfig
                {
                    Code = "es",
                    SystemPromptAddition = "Responde en español. Mantén la personalidad del personaje pero usa un español natural y conversacional.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "¡Hola! Es bueno verte. ¿De qué te gustaría hablar?",
                        ["abigail"] = "¡Oye! ¿Qué tal? ¿Quieres hablar de algo?",
                        ["penny"] = "¡Hola! ¿Cómo estás hoy?",
                        ["sebastian"] = "Oh, hola...",
                        ["sam"] = "¡Ey! ¿Qué tal va todo?",
                        ["alex"] = "¡Hola!",
                        ["harvey"] = "¡Hola! Espero que te sientas bien.",
                        ["elliott"] = "¡Ah, saludos!",
                        ["shane"] = "...¿Qué quieres?",
                        ["haley"] = "Oh, eres tú.",
                        ["leah"] = "¡Hola! Me alegra verte.",
                        ["maru"] = "¡Hola! Estaba trabajando en algo.",
                        ["emily"] = "¡Hola! ¡Qué buena energía hoy!"
                    }
                },
                ["French"] = new LanguageConfig
                {
                    Code = "fr",
                    SystemPromptAddition = "Réponds en français. Garde la personnalité du personnage mais utilise un français naturel et conversationnel.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "Bonjour ! C'est bon de te voir. De quoi aimerais-tu parler ?",
                        ["abigail"] = "Salut ! Quoi de neuf ? Tu veux parler de quelque chose ?",
                        ["penny"] = "Bonjour ! Comment ça va aujourd'hui ?",
                        ["sebastian"] = "Oh, salut...",
                        ["sam"] = "Yo ! Comment ça se passe ?",
                        ["alex"] = "Salut !",
                        ["harvey"] = "Bonjour ! J'espère que tu te sens bien.",
                        ["elliott"] = "Ah, salutations !",
                        ["shane"] = "...Qu'est-ce que tu veux ?",
                        ["haley"] = "Oh, c'est toi.",
                        ["leah"] = "Salut ! Content de te voir.",
                        ["maru"] = "Bonjour ! J'étais en train de travailler sur quelque chose.",
                        ["emily"] = "Salut ! Quelle belle énergie aujourd'hui !"
                    }
                },
                ["German"] = new LanguageConfig
                {
                    Code = "de",
                    SystemPromptAddition = "Antworte auf Deutsch. Behalte die Persönlichkeit des Charakters bei, aber verwende natürliches, umgangssprachliches Deutsch.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "Hallo! Schön dich zu sehen. Worüber möchtest du sprechen?",
                        ["abigail"] = "Hey! Was geht? Willst du über etwas sprechen?",
                        ["penny"] = "Hallo! Wie geht es dir heute?",
                        ["sebastian"] = "Oh, hallo...",
                        ["sam"] = "Yo! Wie läuft's?",
                        ["alex"] = "Hey!",
                        ["harvey"] = "Hallo! Ich hoffe, du fühlst dich wohl.",
                        ["elliott"] = "Ah, Grüße!",
                        ["shane"] = "...Was willst du?",
                        ["haley"] = "Oh, du bist es.",
                        ["leah"] = "Hi! Schön dich zu sehen.",
                        ["maru"] = "Hallo! Ich hab gerade an etwas gearbeitet.",
                        ["emily"] = "Hi! Tolle Energie heute!"
                    }
                },
                ["Italian"] = new LanguageConfig
                {
                    Code = "it",
                    SystemPromptAddition = "Rispondi in italiano. Mantieni la personalità del personaggio ma usa un italiano naturale e colloquiale.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "Ciao! È bello vederti. Di cosa vorresti parlare?",
                        ["abigail"] = "Ehi! Come va? Vuoi parlare di qualcosa?",
                        ["penny"] = "Ciao! Come stai oggi?",
                        ["sebastian"] = "Oh, ciao...",
                        ["sam"] = "Yo! Come butta?",
                        ["alex"] = "Ciao!",
                        ["harvey"] = "Ciao! Spero che tu stia bene.",
                        ["elliott"] = "Ah, saluti!",
                        ["shane"] = "...Cosa vuoi?",
                        ["haley"] = "Oh, sei tu.",
                        ["leah"] = "Ciao! Bello vederti.",
                        ["maru"] = "Ciao! Stavo lavorando a qualcosa.",
                        ["emily"] = "Ciao! Che bella energia oggi!"
                    }
                },
                ["Portuguese"] = new LanguageConfig
                {
                    Code = "pt",
                    SystemPromptAddition = "Responda em português. Mantenha a personalidade do personagem mas use um português natural e conversacional.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "Olá! É bom te ver. Do que gostaria de conversar?",
                        ["abigail"] = "Ei! E aí? Quer falar sobre alguma coisa?",
                        ["penny"] = "Olá! Como você está hoje?",
                        ["sebastian"] = "Oh, oi...",
                        ["sam"] = "E aí! Como tá?",
                        ["alex"] = "Oi!",
                        ["harvey"] = "Olá! Espero que esteja se sentindo bem.",
                        ["elliott"] = "Ah, saudações!",
                        ["shane"] = "...O que você quer?",
                        ["haley"] = "Oh, é você.",
                        ["leah"] = "Oi! Bom te ver.",
                        ["maru"] = "Olá! Estava trabalhando em algo.",
                        ["emily"] = "Oi! Que energia boa hoje!"
                    }
                },
                ["Japanese"] = new LanguageConfig
                {
                    Code = "ja",
                    SystemPromptAddition = "日本語で自然な会話をしてください。キャラクターの個性を保ちながら、自然で親しみやすい日本語を使ってください。",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "こんにちは！会えて嬉しいです。何について話したいですか？",
                        ["abigail"] = "やあ！元気？何か話したいことある？",
                        ["penny"] = "こんにちは！今日の調子はどうですか？",
                        ["sebastian"] = "ああ、やあ...",
                        ["sam"] = "よう！どんな感じ？",
                        ["alex"] = "よう！",
                        ["harvey"] = "こんにちは！体調は大丈夫ですか？",
                        ["elliott"] = "ああ、ご挨拶を！",
                        ["shane"] = "...何の用だ？",
                        ["haley"] = "ああ、あなたね。",
                        ["leah"] = "こんにちは！会えて嬉しいです。",
                        ["maru"] = "こんにちは！今何かの作業をしていました。",
                        ["emily"] = "こんにちは！今日はいいエネルギーですね！"
                    }
                },

                // ✅ NUEVO: Chinese
                ["Chinese"] = new LanguageConfig
                {
                    Code = "zh",
                    SystemPromptAddition = "请用简体中文进行自然对话。保持角色的个性，使用自然、口语化的中文表达。",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "你好！很高兴见到你。你想聊些什么？",
                        ["abigail"] = "嘿！最近怎么样？有什么想聊的吗？",
                        ["penny"] = "你好！今天感觉怎么样？",
                        ["sebastian"] = "哦，你来了……",
                        ["sam"] = "嘿！最近过得如何？",
                        ["alex"] = "嘿，你好！",
                        ["harvey"] = "你好！希望你身体一切安好。",
                        ["elliott"] = "啊，你好！",
                        ["shane"] = "……你来干什么？",
                        ["haley"] = "哦，是你啊。",
                        ["leah"] = "你好！见到你真高兴。",
                        ["maru"] = "你好！我刚在做点什么。",
                        ["emily"] = "嗨！今天感觉能量满满呢！"
                    }
                },

                // ✅ NUEVO: Korean
                ["Korean"] = new LanguageConfig
                {
                    Code = "ko",
                    SystemPromptAddition = "한국어로 자연스럽게 대화해 주세요. 캐릭터의 개성을 유지하면서 자연스럽고 친근한 한국어를 사용해 주세요.",
                    WelcomeMessages = new Dictionary<string, string>
                    {
                        ["default"] = "안녕하세요! 만나서 반가워요. 무슨 이야기를 하고 싶으세요?",
                        ["abigail"] = "야! 잘 지냈어? 뭔가 얘기하고 싶은 거 있어?",
                        ["penny"] = "안녕하세요! 오늘 기분은 어때요?",
                        ["sebastian"] = "어, 안녕...",
                        ["sam"] = "야! 어떻게 지냈어?",
                        ["alex"] = "야, 안녕!",
                        ["harvey"] = "안녕하세요! 건강하게 지내고 계신가요?",
                        ["elliott"] = "아, 안녕하세요!",
                        ["shane"] = "...무슨 일이야?",
                        ["haley"] = "어, 왔네.",
                        ["leah"] = "안녕! 만나서 반가워요.",
                        ["maru"] = "안녕하세요! 방금 뭔가 만들고 있었어요.",
                        ["emily"] = "안녕! 오늘 에너지가 넘치네요!"
                    }
                }
            };
        }

        public void ReDetectLanguage()
        {
            monitor.Log("🔄 Re-detecting game language...", LogLevel.Debug);
            DetectGameLanguage();
        }

        private void DetectGameLanguage()
        {
            try
            {
                var gameLanguage = LocalizedContentManager.CurrentLanguageCode;
                monitor.Log($"🌍 Current game language code: {gameLanguage}", LogLevel.Info);

                string detectedLanguage = gameLanguage switch
                {
                    LocalizedContentManager.LanguageCode.es => "Spanish",
                    LocalizedContentManager.LanguageCode.fr => "French",
                    LocalizedContentManager.LanguageCode.de => "German",
                    LocalizedContentManager.LanguageCode.it => "Italian",
                    LocalizedContentManager.LanguageCode.pt => "Portuguese",
                    LocalizedContentManager.LanguageCode.ru => "Russian",
                    LocalizedContentManager.LanguageCode.zh => "Chinese",
                    LocalizedContentManager.LanguageCode.ja => "Japanese",
                    LocalizedContentManager.LanguageCode.ko => "Korean",
                    LocalizedContentManager.LanguageCode.tr => "Turkish",
                    LocalizedContentManager.LanguageCode.hu => "Hungarian",
                    _ => "English"
                };

                monitor.Log($"📍 Mapped to language: {detectedLanguage}", LogLevel.Info);

                if (!languageConfigs.ContainsKey(detectedLanguage))
                {
                    monitor.Log($"⚠️ No config found for {detectedLanguage}, falling back to English", LogLevel.Warn);
                    detectedLanguage = "English";
                }

                if (currentLanguage != detectedLanguage)
                {
                    currentLanguage = detectedLanguage;
                    monitor.Log($"✅ Language changed to: {currentLanguage}", LogLevel.Info);
                }
                else
                {
                    monitor.Log($"✅ Language confirmed: {currentLanguage}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"❌ Failed to detect game language: {ex.Message}. Using English.", LogLevel.Warn);
                currentLanguage = "English";
            }

            // Fallback: system culture si seguimos en inglés
            if (currentLanguage == "English")
            {
                try
                {
                    var systemCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                    monitor.Log($"System culture: {systemCulture}", LogLevel.Debug);

                    var systemLanguage = systemCulture switch
                    {
                        "es" => "Spanish",
                        "fr" => "French",
                        "de" => "German",
                        "it" => "Italian",
                        "pt" => "Portuguese",
                        "ja" => "Japanese",
                        "zh" => "Chinese",
                        "ko" => "Korean",
                        _ => "English"
                    };

                    if (languageConfigs.ContainsKey(systemLanguage) && systemLanguage != "English")
                    {
                        currentLanguage = systemLanguage;
                        monitor.Log($"✅ Using system culture: {systemCulture} -> {currentLanguage}", LogLevel.Info);
                    }
                }
                catch (Exception ex)
                {
                    monitor.Log($"Failed to detect system culture: {ex.Message}", LogLevel.Debug);
                }
            }
        }

        public string GetCurrentLanguage() => currentLanguage;

        public string GetSystemPromptAddition()
        {
            var config = languageConfigs.ContainsKey(currentLanguage)
                ? languageConfigs[currentLanguage]
                : languageConfigs["English"];

            var promptAddition = config.SystemPromptAddition;

            if (currentLanguage != "English")
            {
                promptAddition += $"\n- Always respond in {currentLanguage}, even if the player writes in English";
                promptAddition += $"\n- Maintain natural {currentLanguage} conversation flow and expressions";
                promptAddition += $"\n- Use appropriate cultural context for {currentLanguage} speakers";
            }

            return promptAddition;
        }

        public string GetWelcomeMessage(string npcName)
        {
            if (!languageConfigs.ContainsKey(currentLanguage))
                return languageConfigs["English"].WelcomeMessages.GetValueOrDefault("default", "Hello!");

            var config = languageConfigs[currentLanguage];
            var npcKey = npcName.ToLower();

            return config.WelcomeMessages.GetValueOrDefault(npcKey,
                   config.WelcomeMessages.GetValueOrDefault("default", "Hello!"));
        }

        public void SetLanguage(string language)
        {
            if (languageConfigs.ContainsKey(language))
            {
                currentLanguage = language;
                monitor.Log($"Language manually set to: {language}", LogLevel.Info);
            }
            else
            {
                monitor.Log($"Language '{language}' not supported. Available: {string.Join(", ", languageConfigs.Keys)}", LogLevel.Warn);
            }
        }

        public List<string> GetSupportedLanguages()
        {
            return new List<string>(languageConfigs.Keys);
        }

        public string GetLocalizedUIText(string key)
        {
            var uiTexts = GetUITexts();
            return uiTexts.GetValueOrDefault(key, key);
        }

        private Dictionary<string, string> GetUITexts()
        {
            return currentLanguage switch
            {
                "Spanish" => new Dictionary<string, string>
                {
                    ["chat_title"] = "Charlando con {0}",
                    ["send_button"] = "Enviar",
                    ["clear_button"] = "Limpiar",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "Tengo problemas para conectarme ahora... ¿intentas de nuevo?",
                    ["error_response"] = "Perdón, tengo problemas para responder ahora.",
                    ["timeout_error"] = "Perdón, está tardando mucho en responder. ¿Intentas de nuevo?",
                    ["save_button"] = "Guardar",
                    ["cancel_button"] = "Cancelar"
                },
                "French" => new Dictionary<string, string>
                {
                    ["chat_title"] = "Discussion avec {0}",
                    ["send_button"] = "Envoyer",
                    ["clear_button"] = "Effacer",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "J'ai des problèmes de connexion... peux-tu réessayer ?",
                    ["error_response"] = "Désolé, j'ai des problèmes à répondre maintenant.",
                    ["timeout_error"] = "Désolé, ça prend trop de temps à répondre. Réessayer ?",
                    ["save_button"] = "Sauvegarder",
                    ["cancel_button"] = "Annuler"
                },
                "German" => new Dictionary<string, string>
                {
                    ["chat_title"] = "Gespräch mit {0}",
                    ["send_button"] = "Senden",
                    ["clear_button"] = "Löschen",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "Ich habe Verbindungsprobleme... kannst du es nochmal versuchen?",
                    ["error_response"] = "Entschuldigung, ich habe Probleme zu antworten.",
                    ["timeout_error"] = "Entschuldigung, das dauert zu lange. Nochmal versuchen?",
                    ["save_button"] = "Speichern",
                    ["cancel_button"] = "Abbrechen"
                },
                "Italian" => new Dictionary<string, string>
                {
                    ["chat_title"] = "Chiacchierando con {0}",
                    ["send_button"] = "Invia",
                    ["clear_button"] = "Pulisci",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "Ho problemi di connessione... puoi riprovare?",
                    ["error_response"] = "Scusa, ho problemi a rispondere ora.",
                    ["timeout_error"] = "Scusa, ci sta mettendo troppo a rispondere. Riprova?",
                    ["save_button"] = "Salva",
                    ["cancel_button"] = "Annulla"
                },
                "Portuguese" => new Dictionary<string, string>
                {
                    ["chat_title"] = "Conversando com {0}",
                    ["send_button"] = "Enviar",
                    ["clear_button"] = "Limpar",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "Estou com problemas de conexão... pode tentar novamente?",
                    ["error_response"] = "Desculpe, estou com problemas para responder agora.",
                    ["timeout_error"] = "Desculpe, está demorando muito para responder. Tentar novamente?",
                    ["save_button"] = "Salvar",
                    ["cancel_button"] = "Cancelar"
                },
                "Japanese" => new Dictionary<string, string>
                {
                    ["chat_title"] = "{0}との会話",
                    ["send_button"] = "送信",
                    ["clear_button"] = "クリア",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "接続に問題があります...もう一度試してみてください？",
                    ["error_response"] = "すみません、返答に問題があります。",
                    ["timeout_error"] = "すみません、返答に時間がかかりすぎています。もう一度試しますか？",
                    ["save_button"] = "保存",
                    ["cancel_button"] = "キャンセル"
                },

                // ✅ NUEVO: Chinese
                "Chinese" => new Dictionary<string, string>
                {
                    ["chat_title"] = "与{0}的对话",
                    ["send_button"] = "发送",
                    ["clear_button"] = "清除",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "连接出现问题……再试一次？",
                    ["error_response"] = "抱歉，现在无法回复。",
                    ["timeout_error"] = "抱歉，回复时间太长了。再试一次？",
                    ["save_button"] = "保存",
                    ["cancel_button"] = "取消"
                },

                // ✅ NUEVO: Korean
                "Korean" => new Dictionary<string, string>
                {
                    ["chat_title"] = "{0}와의 대화",
                    ["send_button"] = "보내기",
                    ["clear_button"] = "지우기",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "연결에 문제가 있어요... 다시 시도해 볼까요?",
                    ["error_response"] = "죄송해요, 지금 답하기 어려워요.",
                    ["timeout_error"] = "죄송해요, 응답이 너무 오래 걸리네요. 다시 시도할까요?",
                    ["save_button"] = "저장",
                    ["cancel_button"] = "취소"
                },

                _ => new Dictionary<string, string>
                {
                    ["chat_title"] = "Chatting with {0}",
                    ["send_button"] = "Send",
                    ["clear_button"] = "Clear",
                    ["loading"] = "...",
                    ["typing_indicator"] = "...",
                    ["connection_error"] = "I'm having trouble connecting right now... maybe try again?",
                    ["error_response"] = "Sorry, I'm having trouble responding right now.",
                    ["timeout_error"] = "Sorry, that's taking too long to respond. Try again?",
                    ["save_button"] = "Save",
                    ["cancel_button"] = "Cancel"
                }
            };
        }
    }

    public class LanguageConfig
    {
        public string Code { get; set; } = "";
        public string SystemPromptAddition { get; set; } = "";
        public Dictionary<string, string> WelcomeMessages { get; set; } = new();
    }
}
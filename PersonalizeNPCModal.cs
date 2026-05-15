using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using System;
using System.IO;
using System.Collections.Generic;

namespace SocialValley
{
    public class PersonalizeNPCModal : IClickableMenu
    {
        private readonly ChatUI parentChatUI;
        private readonly NPC npc;
        private readonly IMonitor monitor;

        // Para manejo de teclas repetidas
        private Keys? heldKey = null;
        private float keyHoldTimer = 0f;
        private float keyRepeatDelay = 0.5f; // Esperar 500ms antes de empezar a repetir
        private float keyRepeatRate = 0.03f; // Repetir cada 30ms después del delay inicial
        private float timeSinceLastRepeat = 0f;

        // UI Components
        private ClickableTextureComponent? saveButton;
        private ClickableTextureComponent? cancelButton;
        private ClickableTextureComponent? toggleButton;
        private ClickableTextureComponent? textArea;

        // Text handling
        private string customPersonalityText = "";
        private string nativePersonalityText = "";
        private bool useCustomPersonality = false;
        private PersonalityTextReceiver? textReceiver;
        private int scrollOffset = 0;
        private float blinkTimer = 0f;
        private int cursorPosition = 0;

        // Configuration
        private ConfigManager? configManager;

        // Visual properties
        private readonly int modalWidth = 800;
        private readonly int modalHeight = 600;
        private readonly Color backgroundColor = new Color(40, 40, 40, 240);
        private readonly Color textBoxColor = new Color(60, 60, 60, 255);

        public PersonalizeNPCModal(ChatUI parentChatUI, NPC npc, IMonitor monitor)
            : base((Game1.uiViewport.Width - 800) / 2, (Game1.uiViewport.Height - 600) / 2, 800, 600)
        {
            this.parentChatUI = parentChatUI;
            this.npc = npc;
            this.monitor = monitor;

            // Obtener el ConfigManager
            configManager = ModEntry.ConfigManager;

            InitializeComponents();
            LoadCurrentConfiguration();

            // Crear el text receiver DESPUÉS de cargar la configuración
            this.textReceiver = new PersonalityTextReceiver(this);
            
            // Set as keyboard subscriber
            Game1.keyboardDispatcher.Subscriber = textReceiver;
        }

        private void InitializeComponents()
        {
            // Toggle button (Native/Custom)
            toggleButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + modalWidth - 150, yPositionOnScreen + 15, 120, 35),
                null, Rectangle.Empty, 1f);

            // Text area
            textArea = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 20, yPositionOnScreen + 120,
                modalWidth - 40, modalHeight - 200),
                null, Rectangle.Empty, 1f);

            // Save button
            saveButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + modalWidth - 180, yPositionOnScreen + modalHeight - 50,
                80, 35),
                null, Rectangle.Empty, 1f);

            // Cancel button
            cancelButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + modalWidth - 90, yPositionOnScreen + modalHeight - 50,
                80, 35),
                null, Rectangle.Empty, 1f);
        }

        private void LoadCurrentConfiguration()
        {
            // Cargar la personalidad nativa del NPCPersonalityManager
            nativePersonalityText = GetNativePersonality();

            // Cargar configuración guardada
            if (configManager != null)
            {
                var config = configManager.GetNPCConfig(npc.Name);
                useCustomPersonality = config.UseCustom;
                customPersonalityText = config.CustomPersonality;

                // Si no hay personalidad custom guardada, crear template inicial
                if (string.IsNullOrEmpty(customPersonalityText))
                {
                    customPersonalityText = GeneratePersonalityTemplate();
                }
            }
            else
            {
                customPersonalityText = GeneratePersonalityTemplate();
            }

            // Inicializar cursor al final del texto
            cursorPosition = customPersonalityText.Length;

            monitor.Log($"Loaded config for {npc.Name} - UseCustom: {useCustomPersonality}", LogLevel.Debug);
        }

        private string GetNativePersonality()
        {
            // Obtener la personalidad del NPCPersonalityManager con la estructura CORRECTA
            if (ModEntry.PersonalityManager != null)
            {
                var personality = ModEntry.PersonalityManager.GetPersonality(npc.Name);
                if (personality != null)
                {
                    // Usar las propiedades REALES de NPCPersonality
                    return $@"You are {personality.Name} from Stardew Valley.

AGE & OCCUPATION:
{personality.Age}, {personality.Occupation}

CORE TRAITS:
{string.Join(", ", personality.CoreTraits)}

BACKGROUND:
{personality.BackgroundSummary}

SPEECH PATTERNS:
{string.Join("\n- ", personality.SpeechPatterns)}

INTERESTS:
{string.Join(", ", personality.Interests)}

DISLIKES:
{string.Join(", ", personality.Dislikes)}";
                }
            }

            // Si no hay personalidad específica, dar una descripción básica
            return $@"You are {npc.Name}, a resident of Pelican Town.
Maintain your canonical personality from Stardew Valley.
Be true to your established character traits and relationships.";
        }

        private string GeneratePersonalityTemplate()
        {
            return $@"You are {npc.Name} from Stardew Valley.

AGE & OCCUPATION:
[Describe age range and job/role]

CORE TRAITS:
[List main personality traits separated by commas]

BACKGROUND:
[Brief background story and important life events]

SPEECH PATTERNS:
- [How they typically speak]
- [Common phrases or expressions]
- [Tone and mannerisms]

INTERESTS:
[What they enjoy, hobbies, passions]

DISLIKES:
[What they avoid or dislike]

Remember to stay true to this personality while being natural and engaging in conversation.";
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            // Detectar click en el área de texto para posicionar cursor
            if (textArea != null && textArea.containsPoint(x, y) && useCustomPersonality)
            {
                // Calcular aproximadamente dónde clickeó el usuario
                int relativeY = y - textArea.bounds.Y - 10;
                int lineClicked = scrollOffset + (relativeY / Game1.smallFont.LineSpacing);

                var lines = WrapText(customPersonalityText, textArea.bounds.Width - 50);
                if (lineClicked >= 0 && lineClicked < lines.Count)
                {
                    // Calcular posición del cursor basado en la línea clickeada
                    int newPos = 0;
                    for (int i = 0; i < lineClicked && i < lines.Count; i++)
                    {
                        newPos += lines[i].Length;
                        if (i < lineClicked - 1 && customPersonalityText.Length > newPos &&
                            customPersonalityText[newPos] == '\n')
                        {
                            newPos++; // Contar el salto de línea
                        }
                    }

                    // Ajustar por posición X dentro de la línea
                    if (lineClicked < lines.Count)
                    {
                        int relativeX = x - textArea.bounds.X - 15;
                        string lineText = lines[lineClicked];
                        int charPos = 0;

                        for (int i = 0; i < lineText.Length; i++)
                        {
                            float charWidth = Game1.smallFont.MeasureString(lineText.Substring(0, i + 1)).X;
                            if (charWidth > relativeX)
                            {
                                break;
                            }
                            charPos = i + 1;
                        }

                        newPos += charPos;
                    }

                    cursorPosition = Math.Min(newPos, customPersonalityText.Length);
                }
                return;
            }

            // Toggle button
            if (toggleButton != null && toggleButton.containsPoint(x, y))
            {
                useCustomPersonality = !useCustomPersonality;
                if (useCustomPersonality)
                {
                    cursorPosition = customPersonalityText.Length;
                }
                Game1.playSound("smallSelect");
                return;
            }

            // Save button
            if (saveButton != null && saveButton.containsPoint(x, y))
            {
                SaveConfiguration();
                return;
            }

            // Cancel button
            if (cancelButton != null && cancelButton.containsPoint(x, y))
            {
                CloseModal();
                return;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            // Manejar las teclas especiales directamente aquí si estamos editando
            if (useCustomPersonality)
            {
                bool handled = false;

                switch (key)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.Delete:
                    case Keys.PageUp:
                    case Keys.PageDown:
                        ProcessKey(key);
                        // Iniciar el tracking de tecla mantenida
                        heldKey = key;
                        keyHoldTimer = 0f;
                        timeSinceLastRepeat = 0f;
                        handled = true;
                        break;
                }

                if (handled)
                {
                    return; // No procesar más
                }
            }

            // Manejar Escape para cerrar
            if (key == Keys.Escape)
            {
                CloseModal();
                return;
            }

            base.receiveKeyPress(key);
        }

        // Método separado para procesar teclas (usado tanto para presión inicial como repetición)
        private void ProcessKey(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    MoveCursorLeft();
                    break;

                case Keys.Right:
                    MoveCursorRight();
                    break;

                case Keys.Up:
                    MoveCursorUp();
                    break;

                case Keys.Down:
                    MoveCursorDown();
                    break;

                case Keys.Home:
                    MoveCursorToLineStart();
                    break;

                case Keys.End:
                    MoveCursorToLineEnd();
                    break;

                case Keys.Delete:
                    HandleDelete();
                    break;

                case Keys.PageUp:
                    ScrollUp();
                    break;

                case Keys.PageDown:
                    ScrollDown();
                    break;
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            // Update blink timer for cursor
            blinkTimer += (float)time.ElapsedGameTime.TotalSeconds;
            if (blinkTimer > 1f) blinkTimer = 0f;

            // Manejar repetición de teclas
            if (heldKey.HasValue && useCustomPersonality)
            {
                keyHoldTimer += (float)time.ElapsedGameTime.TotalSeconds;
                timeSinceLastRepeat += (float)time.ElapsedGameTime.TotalSeconds;

                // Si hemos esperado el delay inicial
                if (keyHoldTimer > keyRepeatDelay)
                {
                    // Y ha pasado suficiente tiempo desde la última repetición
                    if (timeSinceLastRepeat > keyRepeatRate)
                    {
                        ProcessKey(heldKey.Value);
                        timeSinceLastRepeat = 0f;
                    }
                }
            }

            // Detectar si se soltó la tecla
            var keyboard = Keyboard.GetState();
            if (heldKey.HasValue)
            {
                if (!keyboard.IsKeyDown(heldKey.Value))
                {
                    // Se soltó la tecla
                    heldKey = null;
                    keyHoldTimer = 0f;
                    timeSinceLastRepeat = 0f;
                }
            }
        }

        private void SaveConfiguration()
        {
            if (configManager != null)
            {
                configManager.SaveNPCConfig(npc.Name, useCustomPersonality, customPersonalityText);

                monitor.Log($"Saved configuration for {npc.Name} - UseCustom: {useCustomPersonality}", LogLevel.Info);

                string message = useCustomPersonality ?
                    $"Using custom personality for {npc.displayName}" :
                    $"Using native personality for {npc.displayName}";

                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.achievement_type));
            }

            Game1.playSound("bigSelect");
            CloseModal();
        }

        private void CloseModal()
        {
            // Clear keyboard subscriber
            if (Game1.keyboardDispatcher.Subscriber == textReceiver)
            {
                Game1.keyboardDispatcher.Subscriber = null;
            }

            Game1.activeClickableMenu = parentChatUI;
            parentChatUI.RestoreFromModal();
            Game1.playSound("bigDeSelect");
        }

        public override void draw(SpriteBatch b)
        {
            // Dark overlay
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.7f);

            // Modal background
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                xPositionOnScreen, yPositionOnScreen, modalWidth, modalHeight, backgroundColor);

            // Header
            DrawHeader(b);

            // Instructions
            string instructions = useCustomPersonality ?
                "Edit the personality traits, interests, and speaking style:" :
                "Native personality (read-only):";

            Utility.drawTextWithShadow(b, instructions, Game1.smallFont,
                new Vector2(xPositionOnScreen + 20, yPositionOnScreen + 90),
                useCustomPersonality ? Color.White : Color.Gray);

            // Text area
            DrawTextArea(b);

            // Buttons
            DrawButton(b, saveButton, "Save", new Color(100, 200, 100));
            DrawButton(b, cancelButton, "Cancel", new Color(150, 150, 150));

            drawMouse(b);
        }

        private void DrawHeader(SpriteBatch b)
        {
            // NPC Portrait
            try
            {
                var portraitTexture = npc.Portrait;
                if (portraitTexture != null)
                {
                    var portraitDest = new Rectangle(xPositionOnScreen + 20, yPositionOnScreen + 15, 48, 48);
                    var portraitSource = new Rectangle(0, 0, 64, 64);
                    b.Draw(portraitTexture, portraitDest, portraitSource, Color.White);
                }
            }
            catch
            {
                // Si no hay portrait, mostrar solo el nombre
            }

            // Title
            string title = $"Personality: {npc.displayName}";
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont,
                new Vector2(xPositionOnScreen + 80, yPositionOnScreen + 25),
                Color.White);

            // Toggle button
            if (toggleButton != null)
            {
                Color toggleColor = useCustomPersonality ?
                    new Color(100, 150, 200) : new Color(100, 200, 100);

                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                    toggleButton.bounds.X, toggleButton.bounds.Y,
                    toggleButton.bounds.Width, toggleButton.bounds.Height,
                    toggleColor);

                string toggleText = useCustomPersonality ? "Custom" : "Native";
                Vector2 toggleTextSize = Game1.smallFont.MeasureString(toggleText);
                Utility.drawTextWithShadow(b, toggleText, Game1.smallFont,
                    new Vector2(toggleButton.bounds.X + (toggleButton.bounds.Width - toggleTextSize.X) / 2,
                            toggleButton.bounds.Y + (toggleButton.bounds.Height - toggleTextSize.Y) / 2),
                    Color.White);
            }
        }

        private void DrawTextArea(SpriteBatch b)
        {
            if (textArea == null) return;

            // Background - más oscuro si es read-only
            Color bgColor = useCustomPersonality ? textBoxColor : new Color(40, 40, 40, 255);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                textArea.bounds.X, textArea.bounds.Y, textArea.bounds.Width, textArea.bounds.Height,
                bgColor);

            // Get the text to display
            string displayText = useCustomPersonality ? customPersonalityText : nativePersonalityText;

            // Insertar cursor en la posición correcta si estamos editando
            if (useCustomPersonality && blinkTimer < 0.5f)
            {
                if (cursorPosition <= displayText.Length)
                {
                    displayText = displayText.Insert(cursorPosition, "|");
                }
                else
                {
                    displayText += "|";
                }
            }

            // Word wrap and draw
            var lines = WrapText(displayText, textArea.bounds.Width - 50);
            int lineHeight = Game1.smallFont.LineSpacing;
            int maxVisibleLines = (textArea.bounds.Height - 20) / lineHeight;

            // Draw lines
            for (int i = scrollOffset; i < Math.Min(lines.Count, scrollOffset + maxVisibleLines); i++)
            {
                int yPos = textArea.bounds.Y + 10 + ((i - scrollOffset) * lineHeight);

                Color textColor = useCustomPersonality ? Color.White : Color.LightGray;

                Utility.drawTextWithShadow(b, lines[i], Game1.smallFont,
                    new Vector2(textArea.bounds.X + 15, yPos),
                    textColor);
            }

            // Scrollbar
            if (lines.Count > 0)
            {
                DrawScrollbar(b, lines.Count, maxVisibleLines);
            }

            // Character count (only for custom)
            if (useCustomPersonality)
            {
                string charCount = $"{customPersonalityText.Length}/2000";
                Color countColor = customPersonalityText.Length > 1800 ? Color.Orange :
                                  customPersonalityText.Length > 1500 ? Color.Yellow :
                                  Color.Gray;

                Utility.drawTextWithShadow(b, charCount, Game1.tinyFont,
                    new Vector2(textArea.bounds.X + 10, textArea.bounds.Bottom - 20),
                    countColor);
            }
        }

        private void DrawScrollbar(SpriteBatch b, int totalLines, int maxVisibleLines)
        {
            if (textArea == null) return;

            int scrollbarX = textArea.bounds.Right - 20;
            int scrollbarY = textArea.bounds.Y + 10;
            int scrollbarHeight = textArea.bounds.Height - 20;
            int scrollbarWidth = 12;

            // Draw track
            b.Draw(Game1.fadeToBlackRect,
                new Rectangle(scrollbarX, scrollbarY, scrollbarWidth, scrollbarHeight),
                new Color(40, 40, 40, 200));

            // Calculate thumb position and size
            float scrollPercentage = totalLines > maxVisibleLines ?
                (float)scrollOffset / (totalLines - maxVisibleLines) : 0f;

            int thumbHeight = Math.Max(20, (int)((float)maxVisibleLines / totalLines * scrollbarHeight));
            int thumbY = scrollbarY + (int)((scrollbarHeight - thumbHeight) * scrollPercentage);

            // Draw thumb
            Color thumbColor = totalLines > maxVisibleLines ?
                new Color(100, 100, 100, 255) :
                new Color(60, 60, 60, 255);

            b.Draw(Game1.fadeToBlackRect,
                new Rectangle(scrollbarX + 2, thumbY, scrollbarWidth - 4, thumbHeight),
                thumbColor);
        }

        private void DrawButton(SpriteBatch b, ClickableTextureComponent? button, string text, Color color)
        {
            if (button == null) return;

            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height,
                color * 0.8f);

            Vector2 textSize = Game1.smallFont.MeasureString(text);
            Utility.drawTextWithShadow(b, text, Game1.smallFont,
                new Vector2(button.bounds.X + (button.bounds.Width - textSize.X) / 2,
                        button.bounds.Y + (button.bounds.Height - textSize.Y) / 2),
                Color.White);
        }

        private List<string> WrapText(string text, int maxWidth)
        {
            var lines = new List<string>();
            var paragraphs = text.Split('\n');

            foreach (var paragraph in paragraphs)
            {
                if (string.IsNullOrEmpty(paragraph))
                {
                    lines.Add("");
                    continue;
                }

                var words = paragraph.Split(' ');
                var currentLine = "";

                foreach (var word in words)
                {
                    var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    if (Game1.smallFont.MeasureString(testLine).X > maxWidth)
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                        {
                            lines.Add(currentLine);
                            currentLine = word;
                        }
                        else
                        {
                            lines.Add(word);
                        }
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
            }

            return lines;
        }

        // Métodos para manejo de cursor y texto
        public void InsertTextAtCursor(string text)
        {
            if (!useCustomPersonality) return;

            if (customPersonalityText.Length + text.Length <= 2000)
            {
                customPersonalityText = customPersonalityText.Insert(cursorPosition, text);
                cursorPosition += text.Length;
                EnsureCursorVisible();
            }
        }

        public void HandleBackspace()
        {
            if (!useCustomPersonality) return;

            if (cursorPosition > 0 && customPersonalityText.Length > 0)
            {
                customPersonalityText = customPersonalityText.Remove(cursorPosition - 1, 1);
                cursorPosition--;
                EnsureCursorVisible();
            }
        }

        public void HandleDelete()
        {
            if (!useCustomPersonality) return;

            if (cursorPosition < customPersonalityText.Length)
            {
                customPersonalityText = customPersonalityText.Remove(cursorPosition, 1);
            }
        }

        public void MoveCursorLeft()
        {
            if (!useCustomPersonality) return;

            if (cursorPosition > 0)
            {
                cursorPosition--;
                EnsureCursorVisible();
            }
        }

        public void MoveCursorRight()
        {
            if (!useCustomPersonality) return;

            if (cursorPosition < customPersonalityText.Length)
            {
                cursorPosition++;
                EnsureCursorVisible();
            }
        }

        public void MoveCursorUp()
        {
            if (!useCustomPersonality) return;

            // Encontrar el inicio de la línea actual
            int lineStart = cursorPosition;
            while (lineStart > 0 && customPersonalityText[lineStart - 1] != '\n')
                lineStart--;

            if (lineStart == 0) return; // Ya estamos en la primera línea

            // Encontrar el inicio de la línea anterior
            int prevLineEnd = lineStart - 1;
            int prevLineStart = prevLineEnd;
            while (prevLineStart > 0 && customPersonalityText[prevLineStart - 1] != '\n')
                prevLineStart--;

            // Calcular la posición en la línea anterior
            int posInLine = cursorPosition - lineStart;
            int prevLineLength = prevLineEnd - prevLineStart;

            cursorPosition = prevLineStart + Math.Min(posInLine, prevLineLength);
            EnsureCursorVisible();
        }

        public void MoveCursorDown()
        {
            if (!useCustomPersonality) return;

            // Encontrar el final de la línea actual
            int lineEnd = cursorPosition;
            while (lineEnd < customPersonalityText.Length && customPersonalityText[lineEnd] != '\n')
                lineEnd++;

            if (lineEnd >= customPersonalityText.Length) return; // Última línea

            // Encontrar la posición en la línea actual
            int lineStart = cursorPosition;
            while (lineStart > 0 && customPersonalityText[lineStart - 1] != '\n')
                lineStart--;
            int posInLine = cursorPosition - lineStart;

            // Ir al inicio de la siguiente línea
            int nextLineStart = lineEnd + 1;
            if (nextLineStart >= customPersonalityText.Length)
            {
                cursorPosition = customPersonalityText.Length;
            }
            else
            {
                int nextLineEnd = nextLineStart;
                while (nextLineEnd < customPersonalityText.Length && customPersonalityText[nextLineEnd] != '\n')
                    nextLineEnd++;

                int nextLineLength = nextLineEnd - nextLineStart;
                cursorPosition = nextLineStart + Math.Min(posInLine, nextLineLength);
            }

            EnsureCursorVisible();
        }

        public void MoveCursorToLineStart()
        {
            if (!useCustomPersonality) return;

            while (cursorPosition > 0 && customPersonalityText[cursorPosition - 1] != '\n')
                cursorPosition--;

            EnsureCursorVisible();
        }

        public void MoveCursorToLineEnd()
        {
            if (!useCustomPersonality) return;

            while (cursorPosition < customPersonalityText.Length && customPersonalityText[cursorPosition] != '\n')
                cursorPosition++;

            EnsureCursorVisible();
        }

        // Método para asegurar que el cursor sea visible
        private void EnsureCursorVisible()
        {
            if (textArea == null) return;

            // Calcular en qué línea está el cursor
            int currentLine = 0;
            for (int i = 0; i < cursorPosition && i < customPersonalityText.Length; i++)
            {
                if (customPersonalityText[i] == '\n')
                    currentLine++;
            }

            // Calcular líneas visibles
            int lineHeight = Game1.smallFont.LineSpacing;
            int maxVisibleLines = (textArea.bounds.Height - 20) / lineHeight;

            // Ajustar scroll si es necesario
            if (currentLine < scrollOffset)
            {
                scrollOffset = currentLine;
            }
            else if (currentLine >= scrollOffset + maxVisibleLines)
            {
                scrollOffset = currentLine - maxVisibleLines + 1;
            }
        }

        public void ScrollUp()
        {
            if (scrollOffset > 0)
                scrollOffset--;
        }

        public void ScrollDown()
        {
            var lines = WrapText(customPersonalityText, textArea?.bounds.Width - 50 ?? 750);
            int maxVisibleLines = (textArea?.bounds.Height - 20 ?? 460) / Game1.smallFont.LineSpacing;
            int maxScroll = Math.Max(0, lines.Count - maxVisibleLines);

            if (scrollOffset < maxScroll)
                scrollOffset++;
        }

        public int GetTextLength()
        {
            return customPersonalityText.Length;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (textArea == null) return;

            string displayText = useCustomPersonality ? customPersonalityText : nativePersonalityText;
            var lines = WrapText(displayText, textArea.bounds.Width - 50);
            int maxVisibleLines = (textArea.bounds.Height - 20) / Game1.smallFont.LineSpacing;
            int maxScroll = Math.Max(0, lines.Count - maxVisibleLines);

            scrollOffset = Math.Max(0, Math.Min(maxScroll, scrollOffset - (direction / 120)));
        }
    }

    // Text receiver simplificado
    public class PersonalityTextReceiver : IKeyboardSubscriber
    {
        private readonly PersonalizeNPCModal modal;

        public PersonalityTextReceiver(PersonalizeNPCModal modal)
        {
            this.modal = modal;
        }

        public bool Selected { get; set; } = true;

        public void RecieveTextInput(char inputChar)
        {
            if (!char.IsControl(inputChar))
            {
                modal.InsertTextAtCursor(inputChar.ToString());
            }
        }

        public void RecieveTextInput(string text)
        {
            modal.InsertTextAtCursor(text);
        }

        public void RecieveCommandInput(char command)
        {
            switch (command)
            {
                case '\r':
                case '\n':
                    modal.InsertTextAtCursor("\n");
                    break;
                case '\b':
                    modal.HandleBackspace();
                    break;
                case '\u001b':
                    modal.receiveKeyPress(Keys.Escape);
                    break;
            }
        }

        public void RecieveSpecialInput(Keys key)
        {
            // Las teclas especiales se manejan en receiveKeyPress del modal
            // Este método queda vacío pero debe existir por la interfaz
        }
    }
}
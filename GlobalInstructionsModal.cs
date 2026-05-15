using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace SocialValley
{
    public class GlobalInstructionsModal : IClickableMenu
    {
        private readonly SettingsModal parentSettings;
        private readonly IMonitor monitor;

        // ===== KEY REPEAT =====
        private Keys? heldKey = null;
        private float keyHoldTimer = 0f;
        private float keyRepeatDelay = 0.5f;
        private float keyRepeatRate = 0.03f;
        private float timeSinceLastRepeat = 0f;

        // ===== UI Components =====
        private ClickableTextureComponent? saveButton;
        private ClickableTextureComponent? cancelButton;
        private ClickableTextureComponent? clearButton;
        private ClickableTextureComponent? textArea;

        // ===== Text handling =====
        private string instructionsText = "";
        private GlobalInstructionsTextReceiver? textReceiver;
        private int scrollOffset = 0;
        private float blinkTimer = 0f;
        private int cursorPosition = 0;
        private const int MAX_CHARS = 1000;

        // ===== Visual =====
        private readonly int modalWidth = 750;
        private readonly int modalHeight = 550;
        private readonly Color backgroundColor = new Color(40, 40, 40, 240);
        private readonly Color textBoxColor = new Color(60, 60, 60, 255);

        public GlobalInstructionsModal(SettingsModal parentSettings, IMonitor monitor)
            : base((Game1.uiViewport.Width - 750) / 2, (Game1.uiViewport.Height - 550) / 2, 750, 550)
        {
            this.parentSettings = parentSettings;
            this.monitor = monitor;

            InitializeComponents();
            LoadCurrentInstructions();

            this.textReceiver = new GlobalInstructionsTextReceiver(this);
            Game1.keyboardDispatcher.Subscriber = textReceiver;
        }

        private void InitializeComponents()
        {
            textArea = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 20, yPositionOnScreen + 110,
                modalWidth - 40, modalHeight - 195),
                null, Rectangle.Empty, 1f);

            saveButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + modalWidth - 180, yPositionOnScreen + modalHeight - 50,
                80, 35),
                null, Rectangle.Empty, 1f);

            cancelButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + modalWidth - 90, yPositionOnScreen + modalHeight - 50,
                80, 35),
                null, Rectangle.Empty, 1f);

            clearButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 20, yPositionOnScreen + modalHeight - 50,
                80, 35),
                null, Rectangle.Empty, 1f);
        }

        private void LoadCurrentInstructions()
        {
            if (ModEntry.ConfigManager != null)
                instructionsText = ModEntry.ConfigManager.GetGlobalInstructions();

            cursorPosition = instructionsText.Length;
        }

        // ===== INPUT =====

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (textArea != null && textArea.containsPoint(x, y))
            {
                // Posicionar cursor aproximadamente donde clickeó el usuario
                int relativeY = y - textArea.bounds.Y - 10;
                int lineClicked = scrollOffset + (relativeY / Game1.smallFont.LineSpacing);
                var lines = WrapText(instructionsText, textArea.bounds.Width - 50);

                if (lineClicked >= 0 && lineClicked < lines.Count)
                {
                    int newPos = 0;
                    for (int i = 0; i < lineClicked && i < lines.Count; i++)
                    {
                        newPos += lines[i].Length;
                        if (i < lineClicked - 1 && instructionsText.Length > newPos &&
                            instructionsText[newPos] == '\n')
                            newPos++;
                    }
                    if (lineClicked < lines.Count)
                    {
                        int relativeX = x - textArea.bounds.X - 15;
                        string lineText = lines[lineClicked];
                        int charPos = 0;
                        for (int i = 0; i < lineText.Length; i++)
                        {
                            if (Game1.smallFont.MeasureString(lineText.Substring(0, i + 1)).X > relativeX)
                                break;
                            charPos = i + 1;
                        }
                        newPos += charPos;
                    }
                    cursorPosition = Math.Min(newPos, instructionsText.Length);
                }
                return;
            }

            if (saveButton != null && saveButton.containsPoint(x, y))
            {
                SaveAndClose();
                return;
            }

            if (cancelButton != null && cancelButton.containsPoint(x, y))
            {
                CloseModal();
                return;
            }

            if (clearButton != null && clearButton.containsPoint(x, y))
            {
                instructionsText = "";
                cursorPosition = 0;
                scrollOffset = 0;
                Game1.playSound("trashcan");
                return;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
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
                    heldKey = key;
                    keyHoldTimer = 0f;
                    timeSinceLastRepeat = 0f;
                    return;
                case Keys.Escape:
                    CloseModal();
                    return;
            }

            base.receiveKeyPress(key);
        }

        private void ProcessKey(Keys key)
        {
            switch (key)
            {
                case Keys.Left:      MoveCursorLeft();        break;
                case Keys.Right:     MoveCursorRight();       break;
                case Keys.Up:        MoveCursorUp();          break;
                case Keys.Down:      MoveCursorDown();        break;
                case Keys.Home:      MoveCursorToLineStart(); break;
                case Keys.End:       MoveCursorToLineEnd();   break;
                case Keys.Delete:    HandleDelete();          break;
                case Keys.PageUp:    ScrollUp();              break;
                case Keys.PageDown:  ScrollDown();            break;
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            blinkTimer += (float)time.ElapsedGameTime.TotalSeconds;
            if (blinkTimer > 1f) blinkTimer = 0f;

            if (heldKey.HasValue)
            {
                keyHoldTimer += (float)time.ElapsedGameTime.TotalSeconds;
                timeSinceLastRepeat += (float)time.ElapsedGameTime.TotalSeconds;

                if (keyHoldTimer > keyRepeatDelay && timeSinceLastRepeat > keyRepeatRate)
                {
                    ProcessKey(heldKey.Value);
                    timeSinceLastRepeat = 0f;
                }

                if (!Keyboard.GetState().IsKeyDown(heldKey.Value))
                {
                    heldKey = null;
                    keyHoldTimer = 0f;
                    timeSinceLastRepeat = 0f;
                }
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (textArea == null) return;
            var lines = WrapText(instructionsText, textArea.bounds.Width - 50);
            int maxVisible = (textArea.bounds.Height - 20) / Game1.smallFont.LineSpacing;
            int maxScroll = Math.Max(0, lines.Count - maxVisible);
            scrollOffset = Math.Max(0, Math.Min(maxScroll, scrollOffset - (direction / 120)));
        }

        // ===== TEXT OPERATIONS =====

        public void InsertTextAtCursor(string text)
        {
            if (instructionsText.Length + text.Length > MAX_CHARS) return;
            instructionsText = instructionsText.Insert(cursorPosition, text);
            cursorPosition += text.Length;
            EnsureCursorVisible();
        }

        public void HandleBackspace()
        {
            if (cursorPosition > 0 && instructionsText.Length > 0)
            {
                instructionsText = instructionsText.Remove(cursorPosition - 1, 1);
                cursorPosition--;
                EnsureCursorVisible();
            }
        }

        public void HandleDelete()
        {
            if (cursorPosition < instructionsText.Length)
                instructionsText = instructionsText.Remove(cursorPosition, 1);
        }

        public void MoveCursorLeft()
        {
            if (cursorPosition > 0) { cursorPosition--; EnsureCursorVisible(); }
        }

        public void MoveCursorRight()
        {
            if (cursorPosition < instructionsText.Length) { cursorPosition++; EnsureCursorVisible(); }
        }

        public void MoveCursorUp()
        {
            int lineStart = cursorPosition;
            while (lineStart > 0 && instructionsText[lineStart - 1] != '\n') lineStart--;
            if (lineStart == 0) return;

            int prevLineEnd = lineStart - 1;
            int prevLineStart = prevLineEnd;
            while (prevLineStart > 0 && instructionsText[prevLineStart - 1] != '\n') prevLineStart--;

            int posInLine = cursorPosition - lineStart;
            int prevLineLength = prevLineEnd - prevLineStart;
            cursorPosition = prevLineStart + Math.Min(posInLine, prevLineLength);
            EnsureCursorVisible();
        }

        public void MoveCursorDown()
        {
            int lineEnd = cursorPosition;
            while (lineEnd < instructionsText.Length && instructionsText[lineEnd] != '\n') lineEnd++;
            if (lineEnd >= instructionsText.Length) return;

            int lineStart = cursorPosition;
            while (lineStart > 0 && instructionsText[lineStart - 1] != '\n') lineStart--;
            int posInLine = cursorPosition - lineStart;

            int nextLineStart = lineEnd + 1;
            if (nextLineStart >= instructionsText.Length)
            {
                cursorPosition = instructionsText.Length;
            }
            else
            {
                int nextLineEnd = nextLineStart;
                while (nextLineEnd < instructionsText.Length && instructionsText[nextLineEnd] != '\n') nextLineEnd++;
                cursorPosition = nextLineStart + Math.Min(posInLine, nextLineEnd - nextLineStart);
            }
            EnsureCursorVisible();
        }

        public void MoveCursorToLineStart()
        {
            while (cursorPosition > 0 && instructionsText[cursorPosition - 1] != '\n') cursorPosition--;
            EnsureCursorVisible();
        }

        public void MoveCursorToLineEnd()
        {
            while (cursorPosition < instructionsText.Length && instructionsText[cursorPosition] != '\n') cursorPosition++;
            EnsureCursorVisible();
        }

        public void ScrollUp() { if (scrollOffset > 0) scrollOffset--; }

        public void ScrollDown()
        {
            var lines = WrapText(instructionsText, textArea?.bounds.Width - 50 ?? 700);
            int maxVisible = (textArea?.bounds.Height - 20 ?? 400) / Game1.smallFont.LineSpacing;
            if (scrollOffset < Math.Max(0, lines.Count - maxVisible)) scrollOffset++;
        }

        private void EnsureCursorVisible()
        {
            if (textArea == null) return;
            int currentLine = 0;
            for (int i = 0; i < cursorPosition && i < instructionsText.Length; i++)
                if (instructionsText[i] == '\n') currentLine++;

            int maxVisible = (textArea.bounds.Height - 20) / Game1.smallFont.LineSpacing;
            if (currentLine < scrollOffset)
                scrollOffset = currentLine;
            else if (currentLine >= scrollOffset + maxVisible)
                scrollOffset = currentLine - maxVisible + 1;
        }

        // ===== SAVE & CLOSE =====

        private void SaveAndClose()
        {
            if (ModEntry.ConfigManager != null)
            {
                ModEntry.ConfigManager.SetGlobalInstructions(instructionsText);
                monitor.Log("Global instructions saved", LogLevel.Info);
                Game1.addHUDMessage(new HUDMessage("Global instructions saved!", HUDMessage.achievement_type));
            }
            Game1.playSound("bigSelect");
            ReturnToSettings();
        }

        public void CloseModal()
        {
            // Descarta cambios y vuelve a Settings
            Game1.playSound("bigDeSelect");
            ReturnToSettings();
        }

        private void ReturnToSettings()
        {
            if (Game1.keyboardDispatcher.Subscriber == textReceiver)
                Game1.keyboardDispatcher.Subscriber = null;

            Game1.activeClickableMenu = parentSettings;
        }

        // ===== DRAW =====

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.7f);

            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                xPositionOnScreen, yPositionOnScreen, modalWidth, modalHeight, backgroundColor);

            // Título
            string title = "Global Instructions";
            Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont,
                new Vector2(xPositionOnScreen + (modalWidth - titleSize.X) / 2, yPositionOnScreen + 18),
                Color.White);

            // Descripción — dos líneas cortas para no desbordar el modal (ancho útil ~710px)
            Utility.drawTextWithShadow(b, "These instructions apply to every NPC automatically.", Game1.smallFont,
                new Vector2(xPositionOnScreen + 20, yPositionOnScreen + 62), Color.LightGray);

            Utility.drawTextWithShadow(b, "e.g. \"Add line breaks.\"  \"Keep responses short.\"", Game1.smallFont,
                new Vector2(xPositionOnScreen + 20, yPositionOnScreen + 84), new Color(120, 120, 120));

            DrawTextArea(b);
            DrawButtons(b);
            drawMouse(b);
        }

        private void DrawTextArea(SpriteBatch b)
        {
            if (textArea == null) return;

            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                textArea.bounds.X, textArea.bounds.Y, textArea.bounds.Width, textArea.bounds.Height,
                textBoxColor);

            // Insertar cursor parpadeante
            string displayText = instructionsText;
            if (blinkTimer < 0.5f)
            {
                if (cursorPosition <= displayText.Length)
                    displayText = displayText.Insert(cursorPosition, "|");
                else
                    displayText += "|";
            }

            var lines = WrapText(displayText, textArea.bounds.Width - 50);
            int lineHeight = Game1.smallFont.LineSpacing;
            int maxVisible = (textArea.bounds.Height - 20) / lineHeight;

            for (int i = scrollOffset; i < Math.Min(lines.Count, scrollOffset + maxVisible); i++)
            {
                int yPos = textArea.bounds.Y + 10 + ((i - scrollOffset) * lineHeight);
                Utility.drawTextWithShadow(b, lines[i], Game1.smallFont,
                    new Vector2(textArea.bounds.X + 15, yPos), Color.White);
            }

            // Scrollbar
            if (lines.Count > maxVisible)
            {
                int sbX = textArea.bounds.Right - 18;
                int sbY = textArea.bounds.Y + 10;
                int sbH = textArea.bounds.Height - 20;
                b.Draw(Game1.fadeToBlackRect, new Rectangle(sbX, sbY, 10, sbH), new Color(40, 40, 40, 200));
                float pct = (float)scrollOffset / (lines.Count - maxVisible);
                int thumbH = Math.Max(20, (int)((float)maxVisible / lines.Count * sbH));
                int thumbY = sbY + (int)((sbH - thumbH) * pct);
                b.Draw(Game1.fadeToBlackRect, new Rectangle(sbX + 2, thumbY, 6, thumbH), new Color(120, 120, 120));
            }

            // Contador de caracteres
            string charCount = $"{instructionsText.Length}/{MAX_CHARS}";
            Color countColor = instructionsText.Length > MAX_CHARS * 0.9f ? Color.Orange :
                               instructionsText.Length > MAX_CHARS * 0.75f ? Color.Yellow : Color.Gray;
            Utility.drawTextWithShadow(b, charCount, Game1.tinyFont,
                new Vector2(textArea.bounds.X + 10, textArea.bounds.Bottom - 20), countColor);
        }

        private void DrawButtons(SpriteBatch b)
        {
            // Clear
            if (clearButton != null)
            {
                bool hasText = !string.IsNullOrEmpty(instructionsText);
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                    clearButton.bounds.X, clearButton.bounds.Y,
                    clearButton.bounds.Width, clearButton.bounds.Height,
                    (hasText ? new Color(180, 80, 80) : Color.Gray) * 0.8f);
                string clearTxt = "Clear";
                Vector2 cs = Game1.smallFont.MeasureString(clearTxt);
                Utility.drawTextWithShadow(b, clearTxt, Game1.smallFont,
                    new Vector2(clearButton.bounds.X + (clearButton.bounds.Width - cs.X) / 2,
                            clearButton.bounds.Y + (clearButton.bounds.Height - cs.Y) / 2),
                    hasText ? Color.White : Color.Gray);
            }

            // Save
            if (saveButton != null)
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                    saveButton.bounds.X, saveButton.bounds.Y,
                    saveButton.bounds.Width, saveButton.bounds.Height,
                    Color.Green * 0.8f);
                string saveTxt = "Save";
                Vector2 ss = Game1.smallFont.MeasureString(saveTxt);
                Utility.drawTextWithShadow(b, saveTxt, Game1.smallFont,
                    new Vector2(saveButton.bounds.X + (saveButton.bounds.Width - ss.X) / 2,
                            saveButton.bounds.Y + (saveButton.bounds.Height - ss.Y) / 2),
                    Color.White);
            }

            // Cancel
            if (cancelButton != null)
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                    cancelButton.bounds.X, cancelButton.bounds.Y,
                    cancelButton.bounds.Width, cancelButton.bounds.Height,
                    Color.Red * 0.8f);
                string cancelTxt = "Cancel";
                Vector2 cs2 = Game1.smallFont.MeasureString(cancelTxt);
                Utility.drawTextWithShadow(b, cancelTxt, Game1.smallFont,
                    new Vector2(cancelButton.bounds.X + (cancelButton.bounds.Width - cs2.X) / 2,
                            cancelButton.bounds.Y + (cancelButton.bounds.Height - cs2.Y) / 2),
                    Color.White);
            }
        }

        private List<string> WrapText(string text, int maxWidth)
        {
            var lines = new List<string>();
            var paragraphs = text.Split('\n');

            foreach (var paragraph in paragraphs)
            {
                if (string.IsNullOrEmpty(paragraph)) { lines.Add(""); continue; }

                var words = paragraph.Split(' ');
                var currentLine = "";

                foreach (var word in words)
                {
                    var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    if (Game1.smallFont.MeasureString(testLine).X > maxWidth && !string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
            }

            return lines.Count > 0 ? lines : new List<string> { "" };
        }
    }

    public class GlobalInstructionsTextReceiver : IKeyboardSubscriber
    {
        private readonly GlobalInstructionsModal modal;
        public GlobalInstructionsTextReceiver(GlobalInstructionsModal modal) { this.modal = modal; }
        public bool Selected { get; set; } = true;

        public void RecieveTextInput(char inputChar)
        {
            if (!char.IsControl(inputChar))
                modal.InsertTextAtCursor(inputChar.ToString());
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
                    modal.CloseModal();
                    break;
            }
        }

        public void RecieveSpecialInput(Keys key)
        {
            if (key == Keys.Escape)
                modal.CloseModal();
        }
    }
}
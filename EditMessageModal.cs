// ===== EDITMEESSAGEMODAL.CS COMPLETO Y CORREGIDO =====
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace SocialValley
{
    public class EditMessageModal : IClickableMenu
    {
        private readonly ChatUI parentChatUI;
        private readonly int messageIndex;
        private readonly bool isFromPlayer;
        private readonly IMonitor monitor;
        
        // UI Components
        private ClickableTextureComponent? saveButton;
        private ClickableTextureComponent? cancelButton;
        private ClickableTextureComponent? textBox;
        
        // Text handling
        private string editText;
        private string originalText;
        private bool hasFocus = true;
        
        // Visual properties
        private readonly int modalWidth = 800;
        private readonly int modalHeight = 500;
        private readonly Color backgroundColor = new Color(40, 40, 40, 240);
        private readonly Color textBoxColor = new Color(60, 60, 60, 255);
        
        // Text input handling
        private EditMessageTextReceiver? textReceiver;
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;
        
        // Text scrolling for long messages
        private int textScrollOffset = 0;
        private float blinkTimer = 0f;
        
        public EditMessageModal(ChatUI parentChatUI, int messageIndex, string currentText, bool isFromPlayer, IMonitor monitor)
            : base((Game1.uiViewport.Width - 800) / 2, (Game1.uiViewport.Height - 500) / 2, 800, 500)
        {
            this.parentChatUI = parentChatUI;
            this.messageIndex = messageIndex;
            this.isFromPlayer = isFromPlayer;
            this.monitor = monitor;
            this.editText = currentText;
            this.originalText = currentText;
            
            this.previousKeyboardState = Keyboard.GetState();
            this.currentKeyboardState = Keyboard.GetState();
            
            // ✅ CREAR textReceiver con monitor
            this.textReceiver = new EditMessageTextReceiver(this, monitor);

            // ✅ CRÍTICO: Usar nuestro propio control de cierre
            // No podemos modificar readyToClose directamente

            InitializeComponents();
            
            // Set as keyboard subscriber
            Game1.keyboardDispatcher.Subscriber = textReceiver;
            
            monitor.Log($"=== MODAL CONSTRUCTOR COMPLETED ===", LogLevel.Info);
            monitor.Log($"Opened edit modal for message {messageIndex}: {currentText.Substring(0, Math.Min(30, currentText.Length))}...", LogLevel.Debug);
            monitor.Log($"Modal active menu set: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Info);
        }
        
        private void InitializeComponents()
        {
            // Text box area
            textBox = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 20, yPositionOnScreen + 60, 
                modalWidth - 40, modalHeight - 140),
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
        
        // ✅ OVERRIDE CRÍTICO: Interceptar ESC antes que Stardew lo procese
        public override void receiveKeyPress(Keys key)
        {
            monitor.Log($"=== RECEIVE KEY PRESS: {key} ===", LogLevel.Info);
            
            if (key == Keys.Escape)
            {
                monitor.Log("ESC key intercepted in receiveKeyPress", LogLevel.Info);
                HandleEscape();
                return; // NO llamar a base.receiveKeyPress para ESC
            }
            
            // Para otras teclas, llamar al método base
            base.receiveKeyPress(key);
        }
        
        // ✅ OVERRIDE para prevenir cierre automático
        protected override void cleanupBeforeExit()
        {
            monitor.Log("=== CLEANUP BEFORE EXIT CALLED ===", LogLevel.Info);
            // NO llamar al método base para evitar cleanup automático
            // base.cleanupBeforeExit();
        }
        
        // ✅ OVERRIDE del método update con verificación de estado
        public override void update(GameTime time)
        {
            base.update(time);
            
            // Update keyboard states
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            
            // ✅ INTERCEPTAR ESC directamente desde el estado del teclado como backup
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                monitor.Log("ESC detected in update() keyboard state", LogLevel.Info);
                HandleEscape();
                return;
            }
            
            // ✅ DEBUG: Verificar si seguimos siendo el menú activo
            if (Game1.activeClickableMenu != this)
            {
                monitor.Log($"=== MODAL LOST CONTROL IN UPDATE ===", LogLevel.Warn);
                monitor.Log($"Expected: EditMessageModal, Got: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Warn);
                
                // ✅ PREVENIR pérdida de control no autorizada
                if (Game1.activeClickableMenu == null)
                {
                    monitor.Log("Detected unauthorized closure, attempting restore", LogLevel.Warn);
                    Game1.activeClickableMenu = this;
                }
            }
            
            // Update blink timer for cursor
            blinkTimer += (float)time.ElapsedGameTime.TotalSeconds;
            if (blinkTimer > 1f) blinkTimer = 0f;
            
            UpdateTextScrolling();
        }
        
        // ✅ OVERRIDE del emergencyShutDown para manejar cierres forzados
        public override void emergencyShutDown()
        {
            monitor.Log("=== EMERGENCY SHUTDOWN CALLED ===", LogLevel.Warn);
            monitor.Log($"Current menu: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Warn);
            
            // Intentar restaurar ChatUI antes del shutdown
            try
            {
                parentChatUI.RestoreFromModal();
                monitor.Log("Emergency restore attempted", LogLevel.Warn);
            }
            catch (Exception ex)
            {
                monitor.Log($"Emergency restore failed: {ex.Message}", LogLevel.Error);
            }
            
            // Limpiar keyboard subscriber
            if (Game1.keyboardDispatcher.Subscriber == textReceiver)
            {
                Game1.keyboardDispatcher.Subscriber = null;
            }
            
            // Llamar al método base como último recurso
            base.emergencyShutDown();
        }
        
        private void UpdateTextScrolling()
        {
            // Ya no necesitamos scroll horizontal, el textarea maneja word wrap
        }
        
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            
            monitor.Log($"=== MODAL RECEIVED CLICK ===", LogLevel.Info);
            monitor.Log($"Click at ({x}, {y})", LogLevel.Debug);
            
            if (saveButton != null && saveButton.containsPoint(x, y))
            {
                monitor.Log("Save button clicked", LogLevel.Info);
                SaveEdit();
                return;
            }
            
            if (cancelButton != null && cancelButton.containsPoint(x, y))
            {
                monitor.Log("Cancel button clicked", LogLevel.Info);
                CancelEdit();
                return;
            }
            
            // Click on text box to focus
            if (textBox != null && textBox.containsPoint(x, y))
            {
                hasFocus = true;
                monitor.Log("Text box focused", LogLevel.Debug);
            }
        }
        
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }
        
        private void SaveEdit()
        {
            monitor.Log("=== SAVE EDIT CALLED ===", LogLevel.Info);
            
            if (string.IsNullOrWhiteSpace(editText))
            {
                monitor.Log("Cannot save empty message", LogLevel.Warn);
                return;
            }
            
            monitor.Log($"Applying edit to message {messageIndex}", LogLevel.Info);
            parentChatUI.ApplyMessageEdit(messageIndex, editText.Trim());
            monitor.Log($"Saved edit: {editText.Trim().Substring(0, Math.Min(30, editText.Trim().Length))}...", LogLevel.Debug);
            
            CloseModal();
        }
        
        private void CancelEdit()
        {
            monitor.Log("=== CANCEL EDIT CALLED ===", LogLevel.Info);
            CloseModal();
        }
        
        // ✅ MÉTODO CloseModal mejorado con más verificaciones
        private void CloseModal()
        {
            monitor.Log("=== CLOSE MODAL CALLED ===", LogLevel.Info);
            monitor.Log($"Before cleanup - ActiveMenu: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Info);
            monitor.Log($"Before cleanup - IsChatOpen: {ModEntry.IsChatOpen}", LogLevel.Info);
            
            // ✅ ASEGURAR que el modal sigue activo durante la limpieza
            if (Game1.activeClickableMenu != this)
            {
                monitor.Log("WARNING: Modal is not active menu during cleanup", LogLevel.Warn);
                Game1.activeClickableMenu = this; // Forzar reactivación temporal
            }
            
            // Clear our keyboard subscriber
            if (Game1.keyboardDispatcher.Subscriber == textReceiver)
            {
                Game1.keyboardDispatcher.Subscriber = null;
                monitor.Log("Cleared modal keyboard subscriber", LogLevel.Debug);
            }
            else
            {
                monitor.Log($"WARNING: Keyboard subscriber was not our textReceiver. Current: {Game1.keyboardDispatcher.Subscriber?.GetType().Name ?? "null"}", LogLevel.Warn);
            }
            
            // ✅ DELEGAR la restauración al ChatUI padre
            monitor.Log("Calling parentChatUI.RestoreFromModal()", LogLevel.Info);
            try
            {
                parentChatUI.RestoreFromModal();
                monitor.Log("RestoreFromModal completed successfully", LogLevel.Info);
            }
            catch (Exception ex)
            {
                monitor.Log($"ERROR in RestoreFromModal: {ex.Message}", LogLevel.Error);
                monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Error);
                
                // ✅ FALLBACK: Si RestoreFromModal falla, forzar restauración manual
                monitor.Log("Attempting manual ChatUI restoration", LogLevel.Warn);
                Game1.activeClickableMenu = parentChatUI;
                if (parentChatUI.textReceiver != null)
                {
                    Game1.keyboardDispatcher.Subscriber = parentChatUI.textReceiver;
                }
            }
            
            monitor.Log($"After RestoreFromModal - ActiveMenu: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Info);
            monitor.Log($"After RestoreFromModal - IsChatOpen: {ModEntry.IsChatOpen}", LogLevel.Info);
            
            // ✅ VERIFICAR que la restauración fue exitosa antes de cerrar
            if (Game1.activeClickableMenu == parentChatUI)
            {
                monitor.Log("ChatUI successfully restored, modal can close safely", LogLevel.Info);
                exitThisMenu();
            }
            else
            {
                monitor.Log("ChatUI restoration failed, forcing manual closure", LogLevel.Error);
                Game1.activeClickableMenu = parentChatUI;
            }
            
            monitor.Log($"After exitThisMenu - ActiveMenu: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Info);
            monitor.Log($"After exitThisMenu - IsChatOpen: {ModEntry.IsChatOpen}", LogLevel.Info);
            monitor.Log("=== CLOSE MODAL COMPLETED ===", LogLevel.Info);
        }
        
        // ✅ MÉTODO HandleEscape mejorado
        public void HandleEscape()
        {
            monitor.Log("=== HANDLE ESCAPE CALLED IN MODAL ===", LogLevel.Info);
            monitor.Log($"Current state - ActiveMenu: {Game1.activeClickableMenu?.GetType().Name ?? "null"}", LogLevel.Info);
            monitor.Log($"IsChatOpen: {ModEntry.IsChatOpen}", LogLevel.Info);
            
            CancelEdit();
        }
        
        public override void draw(SpriteBatch b)
        {
            // Draw dark overlay over everything
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.7f);

            // Draw modal background
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                xPositionOnScreen, yPositionOnScreen, modalWidth, modalHeight, backgroundColor);

            // Draw title
            string title = $"Edit {(isFromPlayer ? "Your" : "NPC")} Message";
            Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont,
                new Vector2(xPositionOnScreen + (modalWidth - titleSize.X) / 2, yPositionOnScreen + 15),
                Color.White);

            // Draw instructions
            string instructions = "Enter: New line  •  Ctrl+Enter: Save  •  Escape: Cancel";
            Vector2 instructionsSize = Game1.smallFont.MeasureString(instructions);
            Utility.drawTextWithShadow(b, instructions, Game1.smallFont,
                new Vector2(xPositionOnScreen + (modalWidth - instructionsSize.X) / 2, yPositionOnScreen + 45),
                Color.LightGray);

            // Draw text box
            DrawTextBox(b);

            // Draw buttons
            DrawButtons(b);

            drawMouse(b);
        }
        
        private void DrawTextBox(SpriteBatch b)
        {
            if (textBox == null) return;
            
            // Draw text box background
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                textBox.bounds.X, textBox.bounds.Y, textBox.bounds.Width, textBox.bounds.Height,
                textBoxColor);
            
            // Prepare display text with cursor
            string displayText = editText;
            if (hasFocus && blinkTimer < 0.5f)
            {
                displayText += "|";
            }
            
            // Word wrap para textarea completo
            var wrappedLines = WrapText(displayText, textBox.bounds.Width - 40);
            
            // Calcular cuántas líneas caben en el textarea
            int maxVisibleLines = (textBox.bounds.Height - 20) / Game1.smallFont.LineSpacing;
            
            // Scroll vertical si hay demasiadas líneas
            int startLine = 0;
            if (wrappedLines.Count > maxVisibleLines)
            {
                startLine = Math.Max(0, wrappedLines.Count - maxVisibleLines);
            }
            
            // Dibujar líneas visibles
            for (int i = startLine; i < Math.Min(wrappedLines.Count, startLine + maxVisibleLines); i++)
            {
                int lineY = textBox.bounds.Y + 15 + (i - startLine) * Game1.smallFont.LineSpacing;
                
                Utility.drawTextWithShadow(b, wrappedLines[i], Game1.smallFont,
                    new Vector2(textBox.bounds.X + 15, lineY),
                    Color.White);
            }
            
            // Mostrar scroll indicator si hay más contenido
            if (wrappedLines.Count > maxVisibleLines)
            {
                string scrollInfo = $"({startLine + maxVisibleLines}/{wrappedLines.Count} lines)";
                Vector2 scrollInfoSize = Game1.tinyFont.MeasureString(scrollInfo);
                
                Utility.drawTextWithShadow(b, scrollInfo, Game1.tinyFont,
                    new Vector2(textBox.bounds.Right - scrollInfoSize.X - 10, 
                               textBox.bounds.Bottom - scrollInfoSize.Y - 5),
                    Color.Gray);
            }
            
            // Contador de caracteres
            string charCount = $"{editText.Length}/1000";
            Utility.drawTextWithShadow(b, charCount, Game1.tinyFont,
                new Vector2(textBox.bounds.X + 10, textBox.bounds.Bottom - 20),
                editText.Length > 900 ? Color.Orange : Color.Gray);
        }
        
        private void DrawButtons(SpriteBatch b)
        {
            // Save button
            if (saveButton != null)
            {
                Color saveColor = string.IsNullOrWhiteSpace(editText) ? Color.Gray : Color.Green;
                
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                    saveButton.bounds.X, saveButton.bounds.Y, saveButton.bounds.Width, saveButton.bounds.Height,
                    saveColor * 0.8f);
                
                string saveText = ModEntry.LanguageManager?.GetLocalizedUIText("save_button") ?? "Save";
                Vector2 saveTextSize = Game1.smallFont.MeasureString(saveText);
                Utility.drawTextWithShadow(b, saveText, Game1.smallFont,
                    new Vector2(saveButton.bounds.X + (saveButton.bounds.Width - saveTextSize.X) / 2,
                            saveButton.bounds.Y + (saveButton.bounds.Height - saveTextSize.Y) / 2),
                    Color.White);
            }
            
            // Cancel button
            if (cancelButton != null)
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                    cancelButton.bounds.X, cancelButton.bounds.Y, cancelButton.bounds.Width, cancelButton.bounds.Height,
                    Color.Red * 0.8f);
                
                string cancelText = ModEntry.LanguageManager?.GetLocalizedUIText("cancel_button") ?? "Cancel";
                Vector2 cancelTextSize = Game1.smallFont.MeasureString(cancelText);
                Utility.drawTextWithShadow(b, cancelText, Game1.smallFont,
                    new Vector2(cancelButton.bounds.X + (cancelButton.bounds.Width - cancelTextSize.X) / 2,
                            cancelButton.bounds.Y + (cancelButton.bounds.Height - cancelTextSize.Y) / 2),
                    Color.White);
            }
        }
        
        private System.Collections.Generic.List<string> WrapText(string text, int maxWidth)
        {
            var lines = new System.Collections.Generic.List<string>();
            var words = text.Split(' ');
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
                        currentLine = "";
                    }
                }
                else
                {
                    currentLine = testLine;
                }
            }
            
            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);
            
            return lines.Count > 0 ? lines : new System.Collections.Generic.List<string> { text };
        }
        
        // Text input methods
        public void ReceiveTextInput(char inputChar)
        {
            if (char.IsControl(inputChar)) return;
            
            if (editText.Length < 1000)
            {
                editText += inputChar;
                UpdateTextScrolling();
            }
        }
        
        public void HandleBackspace()
        {
            if (editText.Length > 0)
            {
                editText = editText.Substring(0, editText.Length - 1);
                UpdateTextScrolling();
            }
        }
        
        public void HandleEnter()
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
            {
                monitor.Log("Ctrl+Enter pressed, saving edit", LogLevel.Info);
                SaveEdit();
            }
            else
            {
                if (editText.Length < 1000)
                {
                    editText += "\n";
                    UpdateTextScrolling();
                }
            }
        }
    }
    
    // ===== CLASE PARA MANEJAR INPUT DE TEXTO CON DEBUG =====
    public class EditMessageTextReceiver : IKeyboardSubscriber
    {
        private readonly EditMessageModal modal;
        private readonly IMonitor monitor;

        public EditMessageTextReceiver(EditMessageModal modal, IMonitor monitor)
        {
            this.modal = modal;
            this.monitor = monitor;
        }

        public bool Selected { get; set; } = true;

        public void RecieveTextInput(char inputChar)
        {
            if (!char.IsControl(inputChar))
            {
                modal.ReceiveTextInput(inputChar);
            }
        }

        public void RecieveTextInput(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsControl(c))
                {
                    modal.ReceiveTextInput(c);
                }
            }
        }

        public void RecieveCommandInput(char command)
        {
            monitor.Log($"=== COMMAND INPUT RECEIVED: {(int)command} ===", LogLevel.Info);
            
            switch (command)
            {
                case '\r':
                case '\n':
                    monitor.Log("Enter command received", LogLevel.Debug);
                    modal.HandleEnter();
                    break;
                case '\b':
                    monitor.Log("Backspace command received", LogLevel.Debug);
                    modal.HandleBackspace();
                    break;
                case '\u001b': // Escape
                    monitor.Log("=== ESCAPE COMMAND RECEIVED IN TEXT RECEIVER ===", LogLevel.Info);
                    modal.HandleEscape();
                    break;
                default:
                    monitor.Log($"Unknown command: {(int)command}", LogLevel.Debug);
                    break;
            }
        }

        public void RecieveSpecialInput(Keys key)
        {
            monitor.Log($"=== SPECIAL INPUT RECEIVED: {key} ===", LogLevel.Info);
            
            if (key == Keys.Escape)
            {
                monitor.Log("=== ESCAPE KEY RECEIVED IN TEXT RECEIVER ===", LogLevel.Info);
                modal.HandleEscape();
            }
        }
    }
}
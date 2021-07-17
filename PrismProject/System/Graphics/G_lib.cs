﻿using Cosmos.System;
using Cosmos.System.Graphics;
using System.Collections.Generic;
using System.Drawing;

namespace PrismProject
{
    internal class G_lib
    {
        //Define the graphics method
        private static readonly int screenY = Driver.screenY;
        private static readonly Canvas canvas = Driver.canvas;
        private static readonly G_lib draw = new G_lib();

        //Individual shapes
        public void Box(Color color, int from_X, int from_Y, int Width, int Height)
        {
            canvas.DrawFilledRectangle(new Pen(color), from_X, from_Y, Width, Height);
        }

        public void Rounded_Box(Color color, int x, int y, int Width, int Height, int radius)
        {
            int x2 = x + Width, y2 = y + Height, r2 = radius + radius;
            // Draw Outside circles
            draw.Circle(color, x + radius, y + radius, radius);
            draw.Circle(color, x2 - radius - 1, y + radius, radius);
            draw.Circle(color, x + radius, y2 - radius - 1, radius);
            draw.Circle(color, x2 - radius - 1, y2 - radius - 1, radius);

            // Draw Main Rectangle
            draw.Box(color, x, y + radius, Width, Height - r2);
            // Draw Outside Rectangles
            draw.Box(color, x + radius, y, Width - r2, radius);
            draw.Box(color, x + radius, y2 - radius, Width - r2, radius);
        }

        public void Top_Rounded_Box(Color color, int x, int y, int Width, int Height, int radius = 6)
        {
            int x2 = x + Width, r2 = radius + radius;
            // Draw Outside circles
            draw.Circle(color, x + radius, y + radius, radius);
            draw.Circle(color, x2 - radius - 1, y + radius, radius);

            // Draw Main Rectangle
            draw.Box(color, x, y + radius, Width, Height - radius);
            // Draw Outside Rectangles
            draw.Box(color, x + radius, y, Width - r2, radius + 3);
        }

        public void Bottom_Rounded_Box(Color color, int x, int y, int Width, int Height, int radius = 6)
        {
            int x2 = x + Width, y2 = y + Height, r2 = radius + radius;
            // Draw Outside circles
            draw.Circle(color, x + radius, y2 - radius - 1, radius);
            draw.Circle(color, x2 - radius - 1, y2 - radius - 1, radius);

            // Draw Main Rectangle
            draw.Box(color, x, y + radius, Width, Height - r2);
            // Draw Outside Rectangles
            draw.Box(color, x + radius, y2 - radius, Width - r2, radius);
        }

        public void Empty_Box(Color color, int from_X, int from_Y, int to_X, int to_Y)
        {
            canvas.DrawRectangle(new Pen(color), from_X, from_Y, to_X, to_Y);
        }

        public void Circle(Color color, int from_X, int from_Y, int radius)
        {
            canvas.DrawFilledCircle(new Pen(color), from_X, from_Y, radius);
        }

        public void Triangle(Color color, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            canvas.DrawTriangle(new Pen(color), x1, y1, x2, y2, x3, y3);
        }

        public void Arrow(Color color, int width, int x, int y)
        {
            canvas.DrawPoint(new Pen(color, width), x, y);
        }

        public void Line(Color color, int from_X, int from_y, int to_X, int to_Y)
        {
            canvas.DrawLine(new Pen(color), from_X, from_y, to_X, to_Y);
        }

        //UI elements
        public void Text(Color color, string font, string text, int x, int y)
        {
            canvas.DrawBitFontString(font, color, text, x, y);
        }

        public void Loadbar(int fromX, int fromY, int length, int height, int percentage)
        {
            Rounded_Box(Color.SlateGray, fromX, fromY, length, height, 50);
            Rounded_Box(Color.White, fromX-1, fromY-1, percentage+2, height+2, 50);
        }

        public void Window(string font, int from_X, int from_Y, int Width, int Height, string Title, bool showtitlebar, int radius)
        {
            Rounded_Box(Desktop.Accent, from_X - 1, from_Y - 1, Width + 2, Height + 2, radius);
            Bottom_Rounded_Box(Desktop.Window, from_X, from_Y, Width, Height, radius);
            if (showtitlebar) { Top_Rounded_Box(Desktop.Accent, from_X, from_Y, Width, screenY / 23, radius); }
            Text(Color.White, font, Title, from_X + 10, from_Y + 4);
        }

        public void Textbox(string font, string text, Color Background, Color Foreground, int from_X, int from_Y, int Width)
        {
            Box(Background, from_X, from_Y, Width, 15);
            Empty_Box(Foreground, from_X, from_Y, Width, 15);
            Text(Foreground, font, text, from_X, from_Y + 1);
        }

        public void Image(Bitmap img, int x, int y)
        {
            canvas.DrawImageAlpha(img, x, y);
        }
    }

    internal abstract class BaseGuiElement
    {
        public int X, Y, Width, Height;

        public BaseGuiElement Parent;

        public BaseGuiElement(int x, int y, int w, int h, BaseGuiElement parent = null)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Parent = parent;
        }

        internal abstract void Render(G_lib draw, int offset_x, int offset_y);

        internal virtual bool Click(int x, int y, int btn)
        {
            Desktop.ActiveElement = this;
            return true;
        }

        internal virtual bool MouseDown(int x, int y, int btn)
        {
            return true;
        }

        internal virtual bool MouseUp(int x, int y, int btn)
        {
            return true;
        }

        internal virtual bool Key(KeyEvent keyInfo)
        {
            return false;
        }
    }

    internal class GuiWindow : BaseGuiElement
    {
        public string Title;
        public List<BaseGuiElement> Children;

        private int lastX, lastY;
        private readonly int titleHeight;
        private readonly int Radius;

        public GuiWindow(int x, int y, int w, int h, string title, int Rad) : base(x, y, w, h)
        {
            Title = title;
            Radius = Rad;
            Children = new List<BaseGuiElement>();
            lastX = x;
            lastY = y;
            titleHeight = Driver.screenY / 25;
        }

        public void AddChild(BaseGuiElement guiElement)
        {
            guiElement.Parent = this;
            Children.Add(guiElement);
        }

        internal override bool Click(int x, int y, int btn)
        {
            Desktop.ActiveElement = this;
            foreach (var child in Children)
            {
                int x1 = child.X;
                int x2 = x1 + child.Width;
                int y1 = child.Y;
                int y2 = y1 + child.Height;
                if (x >= x1 && x <= x2 && y >= y1 && y <= y2)
                {
                    child.Click(x - x1, y - y1, btn);
                }
            }
            return true;
        }

        internal void Dragging(int dx, int dy)
        {
            lastX = X;
            lastY = Y;
            X += dx * 2;
            Y += dy * 2;
        }

        internal override void Render(G_lib draw, int offset_x = 0, int offset_y = 0)
        {
            if (lastX != X || lastY != Y) // we moved
            {
                draw.Rounded_Box(Desktop.Background, lastX, lastY, Width, Height, Radius);
                lastX = X;
                lastY = Y;
            }
            draw.Window(Driver.font, X, Y, Width, Height, Title, true, Radius);

            foreach (var child in Children)
            {
                child.Render(draw, X, Y + titleHeight);
            }
        }
    }

    internal class GuiText : BaseGuiElement
    {
        public string Value;
        public Color TextColour;

        public GuiText(int x, int y, string text) : base(x, y, 0, 0)
        {
            Value = text;
            TextColour = Desktop.Text;
        }

        internal override void Render(G_lib draw, int offset_x, int offset_y)
        {
            draw.Text(TextColour, Driver.font, Value, X + offset_x, Y + offset_y);
        }
    }

    internal class GuiTextBox : BaseGuiElement
    {
        public string Value;
        public Color Background;
        public Color TextColour;

        public GuiTextBox(int x, int y, int w = 150) : base(x, y, w, 15)
        {
            Background = Color.White;
            TextColour = Desktop.Text;
        }

        internal override bool Click(int x, int y, int btn)
        {
            // todo: Add support for moving cursor for text entry
            return base.Click(x, y, btn);
        }

        internal override bool Key(KeyEvent keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKeyEx.Backspace:
                    if (Value.Length > 0)
                    {
                        Value = Value.Substring(0, Value.Length - 1);
                    }
                    break;

                default:
                    if (!KeyboardManager.ControlPressed && keyInfo.KeyChar > (char)0)
                    {
                        Value += keyInfo.KeyChar;
                    }
                    break;
            }
            return true;
        }

        internal override void Render(G_lib draw, int offset_x, int offset_y)
        {
            string txt = Value + (Desktop.ActiveElement == this ? "|" : "");
            draw.Textbox(Driver.font, txt, Background, TextColour, X + offset_x, Y + offset_y, Width);
        }
    }

    internal class GuiButton : BaseGuiElement
    {
        public string Value;
        public Color Background;
        public Color TextColour;
        public bool Rounded;

        public delegate void ClickDelegate(GuiButton self);

        public ClickDelegate OnClick;

        public GuiButton(int x, int y, int w, int h, string text, bool round, Color Textclr, Color Backcolor, ClickDelegate onClick) : base(x, y, w, h)
        {
            Value = text;
            Background = Backcolor;
            TextColour = Textclr;
            Rounded = round;
            OnClick = onClick;
        }

        internal override void Render(G_lib draw, int offset_x, int offset_y)
        {
            //int mx = X + (Width / 2) - (Value.Length * 8);
            int mx = X + 4;
            int my = Y + (Height / 2) - 8;
            if (Rounded) { draw.Rounded_Box(Background, X + offset_x, Y + offset_y, Width, Height, 6); }
            else { draw.Box(Background, X + offset_x, Y + offset_y, Width, Height); }
            draw.Text(TextColour, Driver.font, Value, mx + offset_x, my + offset_y);
        }

        internal override bool Click(int x, int y, int btn)
        {
            OnClick(this);
            return base.Click(x, y, btn);
        }
    }

    internal class GuiImage : BaseGuiElement
    {
        public Bitmap img;
        public bool showborder;
        public int Height, Width;

        public GuiImage(int x, int y, int w, int h, Bitmap image, bool border) : base(x, y, w, h)
        {
            X = x;
            Y = y;
            img = image;
            showborder = border;
            Height = h;
            Width = w;
        }

        internal override void Render(G_lib draw, int offset_x, int offset_y)
        {
            draw.Image(img, offset_x+X, offset_y+Y);
            if (showborder) { draw.Empty_Box(Color.Black, X, Y, Width, Height); }
        }
    }

    internal class GuiSwitch : BaseGuiElement
    {
        public Color Back;
        public Color Fore;
        public bool status;

        public delegate void ClickDelegate(GuiSwitch self);

        public ClickDelegate OnClick;

        public GuiSwitch(int x, int y, Color Background, Color Foreground, bool enabled, ClickDelegate onClick) : base(x, y, 45, 20)
        {
            Back = Background;
            Fore = Foreground;
            status = enabled;
            OnClick = onClick;
        }

        internal override void Render(G_lib draw, int offset_x, int offset_y)
        {
            draw.Rounded_Box(Fore, offset_x + X - 1, offset_y + Y - 1, 39, 23, 9);
            draw.Rounded_Box(Back, offset_x + X, offset_y + Y, 37, 21, 9);
            if (status) { draw.Circle(Color.White, offset_x + X + 9, offset_y + Y + 10, 8); }
            if (!status) { draw.Circle(Color.White, offset_x + X + 16, offset_y + Y + 10, 8); }
        }

        internal override bool Click(int x, int y, int btn)
        {
            OnClick(this);
            return base.Click(x, y, btn);
        }
    }

    internal class GuiDiv : BaseGuiElement
    {
        public Color Fore;

        public GuiDiv(int x, int y, int h, int w, Color Foreground) : base(x, y, h, w)
        {
            Fore = Foreground;
            Height = h;
            Width = w;
        }

        internal override void Render(G_lib draw, int offset_x, int offset_y)
        {
            draw.Line(Fore, offset_x + X, offset_y + Y, Width, offset_y + Y);
            draw.Line(Fore, offset_x + X, offset_y + Y + Height, Width, offset_y + Y + Height);
        }
    }
}
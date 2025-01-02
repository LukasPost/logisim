using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.Java;
public class JGraphics(Graphics graphics)
{
    protected Graphics Graphics { get; private set; } = graphics;
    public JGraphics create() => this;
    internal void dispose() { }

    Color currentColor = Color.White;
    public Color getColor() => currentColor;
    public void setColor(Color c)=> currentColor = c;

    int currentStroke = 1;
    public int getStroke() => currentStroke;
    public void setStroke(int stroke)=> currentStroke = stroke;

    

    Point currentPoint = new Point();
    public void translate(int x, int y)
    {
        currentPoint = new Point(x,y);
    }

    Font currentFont;
    public Font getFont() => currentFont;
    public void setFont(Font font) => currentFont = font;
    public SizeF measureString(string text)
    {
        return Graphics.MeasureString(text, currentFont);
    }

    public void drawString(string label, int x, int y)
    {
        using Brush brush = new SolidBrush(currentColor);
        Graphics.DrawString(label, currentFont, brush, currentPoint.X + x, currentPoint.Y + y);
    }

    public void fillRect(int x, int y, int width, int height)
    {
        using Brush brush = new SolidBrush(currentColor);
        Graphics.FillRectangle(brush, currentPoint.X + x, currentPoint.Y + y, width, height);
    }
    public void drawRect(int x, int y, int width, int height)
    {
        using Pen pen = new Pen(currentColor, currentStroke);
        Graphics.DrawRectangle(pen, currentPoint.X + x, currentPoint.Y + y, width, height);
    }

    public void fillOval(int x, int y, int width, int height)
    {
        using Brush brush = new SolidBrush(currentColor);
        Graphics.FillEllipse(brush, currentPoint.X + x, currentPoint.Y + y, width, height);
    }
    public void drawOval(int x, int y, int width, int height)
    {
        using Pen pen = new Pen(currentColor, currentStroke);
        Graphics.DrawEllipse(pen, currentPoint.X + x, currentPoint.Y + y, width, height);
    }

    public void drawLine(int x1, int y1, int x2, int y2)
    {
        using Pen pen = new Pen(currentColor, currentStroke);
        Graphics.DrawLine(pen, currentPoint.X + x1, currentPoint.Y + y1, currentPoint.X + x2, currentPoint.Y + y2);
    }

}

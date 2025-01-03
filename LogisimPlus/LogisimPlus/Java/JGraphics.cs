using logisim.data;
using logisim.std.memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.Java;
public class JGraphics(Graphics g)
{
    public static float ConvertToRadians(float deg) => (float)((Math.PI / 180) * deg);
    public static float ConvertToDegrees(float rad) => (float)(rad / (Math.PI / 180));

    protected Graphics Graphics { get; private set; } = g;
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

    public void scale(double sx, double sy)
    {
        Graphics.ScaleTransform((float)sx, (float)sy);
    }
    public void rotate(double rad)
    {
        Graphics.RotateTransform(ConvertToDegrees((float)rad));
    }
    internal void rotate(double rad, double cx, double cy)
    {
        Graphics.RotateTransform(ConvertToDegrees((float)rad));
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

    public void fillPolygon(int[] xs, int[] ys, int v)
    {
        Point[] points = xs.Zip(ys).Select(p => new Point(p.First,p.Second)).ToArray();
        using Brush brush = new SolidBrush(currentColor);
        Graphics.FillPolygon(brush, points);
    }

    public void drawPolygon(int[] xs, int[] ys, int v)
    {
        Point[] points = xs.Zip(ys).Select(p => new Point(p.First, p.Second)).ToArray();
        using Pen pen = new Pen(currentColor, currentStroke);
        Graphics.DrawPolygon(pen, points);
    }

    public void drawPolyline(int[] xs, int[] ys, int v)
    {
        Point[] points = xs.Zip(ys).Select(p => new Point(p.First, p.Second)).ToArray();
        using Pen pen = new Pen(currentColor, currentStroke);
        Graphics.DrawLines(pen, points);
    }

    public void fillRoundRect(int x, int y, int width, int height, int cornerRadius, int obsolete)
    {
        using Brush brush = new SolidBrush(currentColor);
        using (GraphicsPath path = RoundedRect(new Rectangle(x, y, width, height), cornerRadius))
        {
            Graphics.FillPath(brush, path);
        }
    }

    public void drawRoundRect(int x, int y, int width, int height, int cornerRadius, int obsolete)
    {
        using Pen pen = new Pen(currentColor, currentStroke);
        using (GraphicsPath path = RoundedRect(new Rectangle(x, y, width, height), cornerRadius))
        {
            Graphics.DrawPath(pen, path);
        }
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        System.Drawing.Size size = new System.Drawing.Size(diameter, diameter);
        Rectangle arc = new Rectangle(bounds.Location, size);
        GraphicsPath path = new GraphicsPath();

        if (radius == 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        // top left arc  
        path.AddArc(arc, 180, 90);

        // top right arc  
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // bottom right arc  
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // bottom left arc 
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    internal void drawArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        using Pen pen = new Pen(currentColor, currentStroke);
        Graphics.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
    }

}

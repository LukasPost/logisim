using LogisimPlus.draw.gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.Java;
public class JComponent : Control
{
    protected void setToolTipText(string text)
    {
        AutoToolTip.AttachToolTip(this, () => getToolTipText(new MouseEvent()) ?? text);
    }

    public virtual string getToolTipText(MouseEvent e)
    {
        return "";
    }

    protected void setFocusable(bool focusable)
    {

    }

    public virtual void paintComponent(JGraphics g)
    {

    }

    #region MouseListener
    MouseListener listener;
    protected void addMouseListener(MouseListener l)
    {
        if (listener == null)
            listener = l;
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (listener != null)
            listener.mousePressed(new MouseEvent(e));
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (listener != null)
            listener.mouseReleased(new MouseEvent(e));
    }
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        if (listener != null)
            listener.mouseEntered(new MouseEvent());
    }
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (listener != null)
            listener.mouseExited(new MouseEvent());
    }
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (listener != null)
            listener.mousePressed(new MouseEvent(e));
    }
    #endregion

    public int getX() => Location.X;
    public int getY() => Location.Y;
    public int getWidth() => Size.Width;
    public int getHeight() => Size.Height;
    public Size getSize() => Size;
}

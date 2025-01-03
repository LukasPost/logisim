using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.Java;
public interface MouseListener
{
    public void mousePressed(MouseEvent e);

    public void mouseReleased(MouseEvent e);

    public void mouseClicked(MouseEvent e);

    public void mouseEntered(MouseEvent e);

    public void mouseExited(MouseEvent e);
}

public class MouseEvent
{
    public int x;
    public int y;
    public int getX() => x;
    public int getY()=>y;
    public MouseEvent() { }
    public MouseEvent(MouseEventArgs e)
    {
        x = e.Location.X;
        y = e.Location.Y;
    }
}

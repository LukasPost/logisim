using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.draw.gui;
internal class AutoToolTip
{
    private static Dictionary<Control, ToolTip> tooltips = [];
    public static void AttachToolTip(Control control, string text)
    {
        if (!tooltips.TryGetValue(control, out var toolTip))
        {
            toolTip = new ToolTip
            {
                // Set up the delays for the ToolTip.
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                // Force the ToolTip text to be displayed whether or not the form is active.
                ShowAlways = true
            };
            tooltips[control] = toolTip;
        }

        // Set up the ToolTip text for the Button and Checkbox.
        toolTip.SetToolTip(control, "My button1");
    }

    public static void RemoveToolTip(Control control)
    {
        if (tooltips.TryGetValue(control, out var toolTip))
        {
            tooltips.Remove(control);
            toolTip.RemoveAll();
            toolTip.Dispose();
        }

    }
}

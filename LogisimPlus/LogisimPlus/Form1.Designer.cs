namespace LogisimPlus
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            treeViewCircuits = new TreeView();
            lblSelection = new Label();
            pnlTopLevelControls = new Panel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 100);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1079, 416);
            splitContainer1.SplitterDistance = 359;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(treeViewCircuits);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(lblSelection);
            splitContainer2.Size = new Size(359, 416);
            splitContainer2.SplitterDistance = 170;
            splitContainer2.TabIndex = 0;
            // 
            // treeViewCircuits
            // 
            treeViewCircuits.Dock = DockStyle.Fill;
            treeViewCircuits.Location = new Point(0, 0);
            treeViewCircuits.Name = "treeViewCircuits";
            treeViewCircuits.Size = new Size(359, 170);
            treeViewCircuits.TabIndex = 0;
            // 
            // lblSelection
            // 
            lblSelection.AutoEllipsis = true;
            lblSelection.Dock = DockStyle.Top;
            lblSelection.Location = new Point(0, 0);
            lblSelection.Name = "lblSelection";
            lblSelection.Size = new Size(359, 15);
            lblSelection.TabIndex = 0;
            lblSelection.Text = "Selection: ";
            lblSelection.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlTopLevelControls
            // 
            pnlTopLevelControls.Dock = DockStyle.Top;
            pnlTopLevelControls.Location = new Point(0, 0);
            pnlTopLevelControls.Name = "pnlTopLevelControls";
            pnlTopLevelControls.Size = new Size(1079, 100);
            pnlTopLevelControls.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleSizes = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1079, 516);
            Controls.Add(splitContainer1);
            Controls.Add(pnlTopLevelControls);
            Name = "Form1";
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private TreeView treeViewCircuits;
        private Label lblSelection;
        private Panel pnlTopLevelControls;
    }
}

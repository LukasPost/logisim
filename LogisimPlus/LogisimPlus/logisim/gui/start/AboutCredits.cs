// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.start
{

	internal class AboutCredits : JComponent
	{
		/// <summary>
		/// Time to spend freezing the credits before after after scrolling </summary>
		private const int MILLIS_FREEZE = 1000;

		/// <summary>
		/// Speed of how quickly the scrolling occurs </summary>
		private const int MILLIS_PER_PIXEL = 20;

		/// <summary>
		/// Path to Hendrix College's logo - if you want your own logo included, please add it separately rather than
		/// replacing this.
		/// </summary>
		private const string HENDRIX_PATH = "logisim/hendrix.png";
		private const int HENDRIX_WIDTH = 50;

		private class CreditsLine
		{
			internal int y;
			internal int type;
			internal string text;
			internal Image img;
			internal int imgWidth;

			public CreditsLine(int type, string text) : this(type, text, null, 0)
			{
			}

			public CreditsLine(int type, string text, Image img, int imgWidth)
			{
				this.y = 0;
				this.type = type;
				this.text = text;
				this.img = img;
				this.imgWidth = imgWidth;
			}
		}

		private Color[] colorBase;
		private Paint[] paintSteady;
		private Font[] font;

		private int scroll;
		private float fadeStop;

		private List<CreditsLine> lines;
		private int initialLines; // number of lines to show in initial freeze
		private int initialHeight; // computed in code based on above
		private int linesHeight; // computed in code based on above

		public AboutCredits()
		{
			scroll = 0;
			setOpaque(false);

			int prefWidth = About.IMAGE_WIDTH + 2 * About.IMAGE_BORDER;
			int prefHeight = About.IMAGE_HEIGHT / 2 + About.IMAGE_BORDER;
			setPreferredSize(new Size(prefWidth, prefHeight));

			fadeStop = (float)(About.IMAGE_HEIGHT / 4.0);

			colorBase = new Color[]
			{
				Color.FromArgb(255, 143, 0, 0),
				Color.FromArgb(255, 48, 0, 96),
				Color.FromArgb(255, 48, 0, 96)
			};
			font = new Font[]
			{
				new Font("Sans Serif", Font.ITALIC, 20),
				new Font("Sans Serif", Font.BOLD, 24),
				new Font("Sans Serif", Font.BOLD, 18)
			};
			paintSteady = new Paint[colorBase.Length];
			for (int i = 0; i < colorBase.Length; i++)
			{
				Color hue = colorBase[i];
				paintSteady[i] = new GradientPaint(0.0f, 0.0f, derive(hue, 0), 0.0f, fadeStop, hue);
			}

			URL url = typeof(AboutCredits).getClassLoader().getResource(HENDRIX_PATH);
			Image hendrixLogo = null;
			if (url != null)
			{
				hendrixLogo = getToolkit().createImage(url);
			}

			// Logisim's policy concerning who is given credit:
			// Past contributors are not acknowledged in the About dialog for the current
			// version, but they do appear in the acknowledgements section of the User's
			// Guide. Current contributors appear in both locations.

			lines = new List<CreditsLine>();
			linesHeight = 0; // computed in paintComponent
			lines.Add(new CreditsLine(1, "www.cburch.com/logisim/"));
			lines.Add(new CreditsLine(0, Strings.get("creditsRoleLead"), hendrixLogo, HENDRIX_WIDTH));
			lines.Add(new CreditsLine(1, "Carl Burch"));
			lines.Add(new CreditsLine(2, "Hendrix College"));
			initialLines = lines.Count;
			lines.Add(new CreditsLine(0, Strings.get("creditsRoleGerman")));
			lines.Add(new CreditsLine(1, "Uwe Zimmerman"));
			lines.Add(new CreditsLine(2, "Uppsala universitet"));
			lines.Add(new CreditsLine(0, Strings.get("creditsRoleGreek")));
			lines.Add(new CreditsLine(1, "Thanos Kakarountas"));
			lines.Add(new CreditsLine(2, "\u03A4.\u0395.\u0399 \u0399\u03BF\u03BD\u03AF\u03C9\u03BD \u039D\u03AE\u03C3\u03C9\u03BD"));
			lines.Add(new CreditsLine(0, Strings.get("creditsRolePortuguese")));
			lines.Add(new CreditsLine(1, "Theldo Cruz Franqueira"));
			lines.Add(new CreditsLine(2, "PUC Minas"));
			lines.Add(new CreditsLine(0, Strings.get("creditsRoleRussian")));
			lines.Add(new CreditsLine(1, "Ilia Lilov"));
			lines.Add(new CreditsLine(2, "\u041C\u043E\u0441\u043A\u043E\u0432\u0441\u043A\u0438\u0439 \u0433\u043E\u0441\u0443\u0434\u0430\u0440\u0441\u0442\u0432\u0435\u043D\u043D\u044B\u0439"));
			lines.Add(new CreditsLine(2, "\u0443\u043D\u0438\u0432\u0435\u0440\u0441\u0438\u0442\u0435\u0442 \u043F\u0435\u0447\u0430\u0442\u0438"));
			lines.Add(new CreditsLine(0, Strings.get("creditsRoleTesting")));
			lines.Add(new CreditsLine(1, "Ilia Lilov"));
			lines.Add(new CreditsLine(2, "\u041C\u043E\u0441\u043A\u043E\u0432\u0441\u043A\u0438\u0439 \u0433\u043E\u0441\u0443\u0434\u0430\u0440\u0441\u0442\u0432\u0435\u043D\u043D\u044B\u0439"));
			lines.Add(new CreditsLine(2, "\u0443\u043D\u0438\u0432\u0435\u0440\u0441\u0438\u0442\u0435\u0442 \u043F\u0435\u0447\u0430\u0442\u0438"));

			/*
			 * If you fork Logisim, feel free to change the above lines, but please do not change these last four lines!
			 */
			lines.Add(new CreditsLine(0, Strings.get("creditsRoleOriginal"), hendrixLogo, HENDRIX_WIDTH));
			lines.Add(new CreditsLine(1, "Carl Burch"));
			lines.Add(new CreditsLine(2, "Hendrix College"));
			lines.Add(new CreditsLine(1, "www.cburch.com/logisim/"));
		}

		public virtual int Scroll
		{
			set
			{
				scroll = value;
				repaint();
			}
		}

		private Color derive(Color @base, int alpha)
		{
			return Color.FromArgb(alpha, @base.R, @base.G, @base.B);
		}

		protected internal override void paintComponent(JGraphics g)
		{
			FontMetrics[] fms = new FontMetrics[font.Length];
			for (int i = 0; i < fms.Length; i++)
			{
				fms[i] = g.getFontMetrics(font[i]);
			}
			if (linesHeight == 0)
			{
				int y = 0;
				int index = -1;
				foreach (CreditsLine line in lines)
				{
					index++;
					if (index == initialLines)
					{
						initialHeight = y;
					}
					if (line.type == 0)
					{
						y += 10;
					}
					FontMetrics fm = fms[line.type];
					line.y = y + fm.getAscent();
					y += fm.getHeight();
				}
				linesHeight = y;
			}

			Paint[] paint = paintSteady;
			int yPos = 0;
			int height = getHeight();
			int initY = Math.Min(0, initialHeight - height + About.IMAGE_BORDER);
			int maxY = linesHeight - height - initY;
			int totalMillis = 2 * MILLIS_FREEZE + (linesHeight + height) * MILLIS_PER_PIXEL;
			int offs = scroll % totalMillis;
			if (offs >= 0 && offs < MILLIS_FREEZE)
			{
				// frozen before starting the credits scroll
				int a = 255 * (MILLIS_FREEZE - offs) / MILLIS_FREEZE;
				if (a > 245)
				{
					paint = null;
				}
				else if (a < 15)
				{
					paint = paintSteady;
				}
				else
				{
					paint = new Paint[colorBase.Length];
					for (int i = 0; i < paint.Length; i++)
					{
						Color hue = colorBase[i];
						paint[i] = new GradientPaint(0.0f, 0.0f, derive(hue, a), 0.0f, fadeStop, hue);
					}
				}
				yPos = initY;
			}
			else if (offs < MILLIS_FREEZE + maxY * MILLIS_PER_PIXEL)
			{
				// scrolling through credits
				yPos = initY + (offs - MILLIS_FREEZE) / MILLIS_PER_PIXEL;
			}
			else if (offs < 2 * MILLIS_FREEZE + maxY * MILLIS_PER_PIXEL)
			{
				// freezing at bottom of scroll
				yPos = initY + maxY;
			}
			else if (offs < 2 * MILLIS_FREEZE + (linesHeight - initY) * MILLIS_PER_PIXEL)
			{
				// scrolling bottom off screen
				yPos = initY + (offs - 2 * MILLIS_FREEZE) / MILLIS_PER_PIXEL;
			}
			else
			{
				// scrolling next credits onto screen
				int millis = offs - 2 * MILLIS_FREEZE - (linesHeight - initY) * MILLIS_PER_PIXEL;
				paint = null;
				yPos = -height + millis / MILLIS_PER_PIXEL;
			}

			int width = getWidth();
			int centerX = width / 2;
			maxY = getHeight();
			foreach (CreditsLine line in lines)
			{
				int y = line.y - yPos;
				if (y < -100 || y > maxY + 50)
				{
					continue;
				}

				int type = line.type;
				if (paint == null)
				{
					g.setColor(colorBase[type]);
				}
				else
				{
					g.setPaint(paint[type]);
				}
				g.setFont(font[type]);
				int textWidth = fms[type].stringWidth(line.text);
				g.drawString(line.text, centerX - textWidth / 2, line.y - yPos);

				Image img = line.img;
				if (img != null)
				{
					int x = width - line.imgWidth - About.IMAGE_BORDER;
					int top = y - fms[type].getAscent();
					g.drawImage(img, x, top, this);
				}
			}
		}
	}
}

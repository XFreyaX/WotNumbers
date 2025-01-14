﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Code
{
	[DebuggerNonUserCode]
	public class ColorTheme
	{
		// Forms
		public static Color FormTranparency = Color.FromArgb(255, 1, 1, 1);
		public static Color FormBack = Color.FromArgb(255, 32, 32, 32);
		public static Color FormBackTitle = Color.FromArgb(255, 45, 45, 49);
		public static Color FormBackTitleFont = Color.FromArgb(255, 180, 180, 186);
		public static Color FormBackFooter = Color.FromArgb(255, 8, 8, 8);
		public static Color FormBorderBlack = Color.FromArgb(255, 0, 0, 0);
		public static Color FormBorderBlue = Color.FromArgb(255, 68, 96, 127);
		public static Color FormBorderRed = Color.FromArgb(255, 190, 0, 0);
		
		// Gadget
		public static Color FormBackSelectedGadget = Color.FromArgb(255, 38, 38, 38);
		public static Color gadgetGrid = Color.FromArgb(255, 45, 45, 49);
		public static Color gadgetGridLight = Color.FromArgb(255, 57, 57, 61);
		public static Color gadgetOriginForMoved = Color.FromArgb(255, 75, 75, 79);

		// Controls
		public static Color ControlBack = Color.FromArgb(255, 55, 55, 59);
		public static Color ControlBackDark = Color.FromArgb(255, 45, 45, 49);
		public static Color ControlBackDarkMoving = Color.FromArgb(255, 24, 24, 24);
		public static Color ControlBorder = Color.FromArgb(255, 65, 65, 69);
		public static Color ControlBorderFocused = Color.FromArgb(255, 100, 100, 106);
		public static Color ControlBackMouseOver = Color.FromArgb(255, 68, 68, 72);
		public static Color ControlBackMouseDown = Color.FromArgb(255, 82, 82, 86);
		public static Color ControlFont = Color.FromArgb(255, 200, 200, 206);
        public static Color ControlFontWarning = Color.FromArgb(255, 255, 174, 94);
        public static Color ControlFontHighLight = Color.FromArgb(255, 220, 220, 226);
		public static Color ControlDarkFont = Color.FromArgb(255, 130, 130, 136);
		public static Color ControlDisabledFont = Color.FromArgb(255, 100, 100, 106);
		public static Color ControlDimmedFont = Color.FromArgb(255, 85, 85, 89);
		public static Color ControlDarkRed = Color.FromArgb(255, 127, 0, 0);
		//public static Color ControlSeparatorGroupBoxBorder = Color.FromArgb(255, 75, 75, 79);
		public static Color ControlSeparatorGroupBoxBorder = Color.FromArgb(255, 55, 55, 59);
		
		// ToolStrip 
		public static Color ToolGrayMainBack = Color.FromArgb(255, 22, 22, 22);
		public static Color ToolGrayMain = Color.FromArgb(255, 45, 45, 49);
		public static Color ToolGrayHover = Color.FromArgb(255, 68, 68, 72);
		public static Color ToolGrayOutline = Color.FromArgb(255, 82, 82, 86);
		public static Color ToolGrayScrollbar = Color.FromArgb(255, 102, 102, 106);
		public static Color ToolGrayScrollbarHover = Color.FromArgb(255, 126, 126, 130);

		public static Color ToolGrayCheckBack = Color.FromArgb(255, 68, 68, 72);
        public static Color ToolGrayCheckBorder = Color.FromArgb(255, 180, 180, 186);
        public static Color ToolGrayCheckHover = Color.FromArgb(255, 92, 92, 96);
        public static Color ToolGrayCheckPressed = Color.FromArgb(255, 108, 108, 112);

        public static Color ToolWhiteToolStrip = Color.FromArgb(255, 220, 220, 220);
		public static Color ToolBlueHoverButton = Color.FromArgb(255, 70, 98, 129); // Color.FromArgb(255, 66, 125, 215);
		public static Color ToolBlueSelectedButton = Color.FromArgb(255, 60, 86, 114);
        public static Color ToolLabelHeading = Color.FromArgb(255, 140, 140, 146);
		
		// Grid 
		public static Color GridHeaderBackLight = Color.FromArgb(255, 22, 22, 22);
		public static Color GridBorders = Color.FromArgb(255, 17, 17, 17);
		public static Color GridSelectedHeaderColor = Color.FromArgb(255, 37, 37, 37);
		public static Color GridSelectedCellColor = Color.FromArgb(255, 52, 52, 52);
		public static Color GridCellFont = Color.FromArgb(255, 200, 200, 200);
		public static Color GridColumnSeparator = Color.FromArgb(255, 22, 22, 22);
		public static Color GridColumnHeaderSeparator = Color.FromArgb(255, 17, 17, 17);
		public static Color GridTotalsRow = Color.FromArgb(255, 22, 22, 22);
		
		public static Color GridRowCurrentPlayerAlive = Color.FromArgb(255, 38, 38, 20);
		public static Color GridRowCurrentPlayerAliveSelected = Color.FromArgb(255, 58, 58, 40);
		
		public static Color GridRowCurrentPlayerDead = Color.FromArgb(255, 53, 37, 24);
		public static Color GridRowCurrentPlayerDeadeSelected = Color.FromArgb(255, 73, 57, 44);
		
		public static Color GridRowPlayerDead = Color.FromArgb(255, 34, 24, 24);
		public static Color GridRowPlayerDeadeSelected = Color.FromArgb(255, 54, 44, 44);

		// Scrollbar
		public static Color ScrollbarBack = Color.FromArgb(255, 65, 65, 69);
		public static Color ScrollbarFront = Color.FromArgb(255, 102, 102, 106);
		public static Color ScrollbarArrow = Color.FromArgb(255, 152, 152, 156);
        public static Color ScrollbarArrowHighLigh = Color.FromArgb(255, 192, 192, 196);

        // Charts
        public static Color ChartBarBlue = ColorTranslator.FromHtml("#1F47A5");  
		public static Color ChartBarRed = ColorTranslator.FromHtml("#A31F1F");
		public static Color ChartBarGreen = ColorTranslator.FromHtml("#1B8E30");
		public static Color ChartBarPurple = ColorTranslator.FromHtml("#761E99");
		public static Color ChartBarOcre = ColorTranslator.FromHtml("#896A1B");
		// Lighter tone
		public static Color ChartBarBlueLight = ColorTranslator.FromHtml("#315377");
		public static Color ChartBarRedLight = ColorTranslator.FromHtml("#772F2F");
		public static Color ChartBarGreenLight = ColorTranslator.FromHtml("#3A7030");
		public static Color ChartBarPurpleLight = ColorTranslator.FromHtml("#683468");
		public static Color ChartBarOcreLight = ColorTranslator.FromHtml("#755D2E");

        // Player rating colors - named
        public static Color Rating_0_redDark = ColorTranslator.FromHtml("#CE0000");		// red dark
        public static Color Rating_1_red = ColorTranslator.FromHtml("#FF0000");			// red
        public static Color Rating_2_orange = ColorTranslator.FromHtml("#FF8400");	    // orange
        public static Color Rating_3_yellow = ColorTranslator.FromHtml("#FFFF00");		// yellow
        public static Color Rating_4_greenLight = ColorTranslator.FromHtml("#AAFF00");	// green - to - yellow
        public static Color Rating_4_green = ColorTranslator.FromHtml("#4CFF00");		// green
        public static Color Rating_5_greenDark = ColorTranslator.FromHtml("#2F9E00");	// green dark
        public static Color Rating_6_blue = ColorTranslator.FromHtml("#75C5FF");		// blue
        public static Color Rating_7_blueDark = ColorTranslator.FromHtml("#30A8FF");	// blue dark
        public static Color Rating_8_purple = ColorTranslator.FromHtml("#CC5EFF");		// purple
        public static Color Rating_9_purpleDark = ColorTranslator.FromHtml("#B200FF");	// purple dark

        // Player rating colors 
        public static Color Rating_very_bad =       Rating_0_redDark;		
		public static Color Rating_bad =            Rating_1_red;
        public static Color Rating_below_average =  Rating_2_orange;
        public static Color Rating_average =        Rating_3_yellow;
        public static Color Rating_good =           Rating_4_green;
        public static Color Rating_very_good =      Rating_5_greenDark;
        public static Color Rating_great =          Rating_6_blue;
        public static Color Rating_very_great =     Rating_7_blueDark;
        public static Color Rating_uniqum =         Rating_8_purple;
        public static Color Rating_super_uniqum =   Rating_9_purpleDark;

        // XVM rating colors
        public static Color Rating_XVM_very_bad = Rating_0_redDark;
        public static Color Rating_XVM_bad = Rating_1_red;
        public static Color Rating_XVM_normal = Rating_3_yellow;
        public static Color Rating_XVM_good = Rating_4_green;
        public static Color Rating_XVM_very_good = Rating_7_blueDark;
        public static Color Rating_XVM_unique = Rating_8_purple;
        


		public static int[] DefaultChartBarColors()
		{
			int[] defaultColors = new int[16];
			defaultColors[0] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarRed);
			defaultColors[1] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarOcre);
			defaultColors[2] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarGreen);
			defaultColors[3] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarBlue);
			defaultColors[4] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarPurple);
			defaultColors[5] = System.Drawing.ColorTranslator.ToOle(Color.White);
			defaultColors[6] = System.Drawing.ColorTranslator.ToOle(Color.White);
			defaultColors[7] = System.Drawing.ColorTranslator.ToOle(Color.White);
			defaultColors[8] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarRedLight);
			defaultColors[9] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarOcreLight);
			defaultColors[10] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarGreenLight);
			defaultColors[11] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarBlueLight);
			defaultColors[12] = System.Drawing.ColorTranslator.ToOle(ColorTheme.ChartBarPurpleLight);
			defaultColors[13] = System.Drawing.ColorTranslator.ToOle(Color.White);
			defaultColors[14] = System.Drawing.ColorTranslator.ToOle(Color.White);
			defaultColors[15] = System.Drawing.ColorTranslator.ToOle(Color.White);
			return defaultColors;
		}
	}
}

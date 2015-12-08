﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaboliteLevels.Utilities
{
    /// <summary>
    /// Fonts!
    /// </summary>
    static class FontHelper
    {                     
        public static Font RegularFont;
        public static Font BoldFont;
        public static Font ItalicFont;
        public static Font StrikeFont;

        public static Font LargeRegularFont;
        public static Font LargeBoldFont;

        public static Font SmallRegularFont;

        internal static void Initialise(Font font)
        {
            RegularFont = new Font("Segoe UI", 12, FontStyle.Regular);
            BoldFont = new Font("Segoe UI", 12, FontStyle.Bold);
            ItalicFont = new Font("Segoe UI", 12, FontStyle.Italic);
            StrikeFont = new Font("Segoe UI", 12, FontStyle.Strikeout);

            LargeRegularFont = new Font("Segoe UI", 14, FontStyle.Regular);
            LargeBoldFont = new Font("Segoe UI", 14, FontStyle.Bold);

            SmallRegularFont = new Font("Segoe UI", 8, FontStyle.Regular);
        }
    }
}
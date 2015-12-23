using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryMeowChat {
    public static class HexConverter {
        /// <summary>
        /// Convert Color class to HEX color code string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string Convert(Color color) {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }

    public static class Time {
        /// <summary>
        /// Returns the current time in the specified format
        /// </summary>
        /// <returns></returns>
        public static string NowTime() {
            DateTime time = DateTime.Now;
            string timeFormat = "HH:mm:ss";
            return time.ToString(timeFormat);
        }

        public static string NowTimeDate() {
            DateTime time = DateTime.Now;
            string timeFormat = "HH:mm:ss - dd/MM/yyyy";
            return time.ToString(timeFormat);
        }
    }

    public static class TabFormat {
        /// <summary>
        /// Formats the size of TabPages in a provided TabControl object
        /// </summary>
        public static void ItemEvenSize(TabControl tabControl) {
            // Get the inital length
            int tabLength = tabControl.ItemSize.Width;
            // measure the text in each tab and make adjustment to the size
            for (int i = 1; i < tabControl.TabPages.Count; i++) {
                TabPage currentPage = tabControl.TabPages[i];
                int currentTabLength = TextRenderer.MeasureText(currentPage.Text, tabControl.Font).Width;
                // Adjust the length for what text is written
                currentTabLength += 40;
                if (currentTabLength > tabLength) {
                    tabLength = currentTabLength;
                }
            }
            // Create the new size
            Size newTabSize = new Size(tabLength, 24);
            tabControl.ItemSize = newTabSize;
        }
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommonLibrary
{
    public static class ChatMethodsStatic
    {
        //Convert Color class to HEX color code string
        public static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        //Formats the size of TabPages in a provided TabControll
        public static void FormatItemSize(TabControl tabControl)
        {
            //go thro and resize
            // get the inital length
            int tabLength = tabControl.ItemSize.Width;
            // measure the text in each tab and make adjustment to the size
            for (int i = 1; i < tabControl.TabPages.Count; i++)
            {
                TabPage currentPage = tabControl.TabPages[i];
                int currentTabLength = TextRenderer.MeasureText(currentPage.Text, tabControl.Font).Width;
                // adjust the length for what text is written
                currentTabLength += 40;
                if (currentTabLength > tabLength)
                {
                    tabLength = currentTabLength;
                }
            }
            // create the new size
            Size newTabSize = new Size(tabLength, 24);
            tabControl.ItemSize = newTabSize;
        }

        //Returns the current time in HH:mm format
        public static string Time()
        {
            DateTime time = DateTime.Now;
            string timeFormat = "HH:mm";
            return time.ToString(timeFormat);
        }
    }
}
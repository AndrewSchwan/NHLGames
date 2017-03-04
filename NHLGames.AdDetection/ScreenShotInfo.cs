using System;
using System.Windows.Forms;

namespace NHLGames.AdDetection
{
    public class ScreenShotInfo
    {
        private const float m_streamRes = 16/(float) 9;


        public ScreenShotInfo(Screen screen)
        {
            var screenRes = screen.Bounds.Size.Width/(float) screen.Bounds.Size.Height;

            if (Math.Abs(screenRes - m_streamRes) < .001)
            {
                Width = screen.Bounds.Size.Width;
                Height = screen.Bounds.Size.Height;
                XOffSet = 0;
                YOffSet = 0;
            }
            else if (screenRes > m_streamRes)
            {
                //wider than 16:9 (Black bars on sides)
                Height = screen.Bounds.Size.Height;
                var heightRatio = Height/(float) 1080;
                YOffSet = 0;

                var width = 1920*heightRatio;

                Width = (int) width;
                XOffSet = (screen.Bounds.Size.Width - Width)/2;
            }
            else if (screenRes < m_streamRes)
            {
                //Taller than 16:9 (Black bars on top and bottom)
                Width = screen.Bounds.Size.Width;
                var widthRatio = Width/(float) 1920;
                XOffSet = 0;

                var height = 1080*widthRatio;

                Height = (int) height;
                YOffSet = (screen.Bounds.Size.Height - Height)/2;
            }
        }

        public int Width { get; }

        public int Height { get; }

        public int XOffSet { get; private set; }

        public int YOffSet { get; private set; }
    }
}
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Caro_ai
{
    public class EBoard
    {
        public int x, y;
        public int[,] eBoard = new int[20,20];
        public void clear()
        {
            for (int i = 0; i < 20;i++ )
            {
                for (int j = 0; j < 20; j++ )
                {
                    eBoard[i, j] = 0;
                }
            }
        }
        public Point maxPos()
        {
            int max = 0;
            Point p = new Point();
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (eBoard[i,j] > max)
                    {
                        p.X = i;
                        p.Y = j;
                        max = eBoard[i,j];
                    }
                }
            }
            return p;
        }
    }
}

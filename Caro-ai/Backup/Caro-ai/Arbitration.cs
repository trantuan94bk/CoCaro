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
    public class Arbitration
    {
        //các hàm kiểm tra chặn
        //theo chieu doc
        public static bool isBlock1(int x, int y, int[,] cellFill)
        {
            if (cellFill[x, y] == 0) return false;
            for (int i = x + 1; i < 20 && i <= x + 5; i++)
            {
                if (cellFill[i,y] == -cellFill[x,y])
                {
                    for (int j = x - 1; j >= 0 && j >= i - 6; j--)
                    {
                        if (cellFill[j,y] == -cellFill[x,y])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //theo chieu ngang
        public static bool isBlock2(int x, int y, int[,] cellFill)
        {
            if (cellFill[x, y] == 0) return false;
            for (int i = y + 1; i < 20 && i <= y + 5; i++)
            {
                if (cellFill[x,i] == -cellFill[x,y])
                {
                    for (int j = y - 1; j >= 0 && j >= i - 6; j--)
                    {
                        if (cellFill[x,j] == -cellFill[x,y])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //theo chieu cheo xuong \
        public static bool isBlock3(int x, int y, int[,] cellFill)
        {
            if (cellFill[x, y] == 0) return false;
            for (int i = x + 1, j = y + 1; i < 20 && i <= x + 5 && j < 20 && j <= y + 5; i++, j++)
            {
                if (cellFill[i,j] == -cellFill[x,y])
                {
                    for (int n = x - 1, m = y - 1; n >= 0 && n >= i - 6 && m >= 0 && m >= j - 6; n--, m--)
                    {
                        if (cellFill[n,m] == -cellFill[x,y])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //theo chieu cheo /
        public static bool isBlock4(int x, int y, int[,] cellFill)
        {
            if (cellFill[x, y] == 0) return false;
            for (int i = x - 1, j = y + 1; i >= 0 && i >= x - 5 && j < 20 && j <= y + 5; i--, j++)
            {
                if (cellFill[i,j] == -cellFill[x,y])
                {
                    for (int n = x + 1, m = y - 1; n < 20 && n <= i + 6 && m >= 0 && m >= j - 6; n++, m--)
                    {
                        if (cellFill[n,m] == -cellFill[x,y])
                        {
                            //JOptionPane.showMessageDialog(this, cellFill[x][y] == 1 ? "X is block" : "O is block");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //tat ca
        public static bool isBlock(int x, int y, int[,] cellFill)
        {
            if (cellFill[x, y] == 0) return false;
            //theo chieu doc
            if (isBlock1(x, y, cellFill) == true)
            {
                return true;
            }
            //theo chieu ngang
            if (isBlock2(x, y, cellFill) == true)
            {
                return true;
            }
            //theo chieu cheo \
            if (isBlock3(x, y, cellFill) == true)
            {
                return true;
            }
            //theo chieu cheo /
            if (isBlock4(x, y, cellFill) == true)
            {
                return true;
            }
            return false;
        }

        //ham dem
        //dem theo cheo doc
        public static int count1(int x, int y, int[,] cellFill)
        {
            int count = 1;
            for (int i = x + 1; i < 20; i++)
            {
                if (cellFill[i,y] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            for (int i = x - 1; i >= 0; i--)
            {
                if (cellFill[i,y] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        //dem theo chieu ngang
        public static int count2(int x, int y, int[,] cellFill)
        {
            int count = 1;
            for (int i = y + 1; i < 20; i++)
            {
                if (cellFill[x,i] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            for (int i = y - 1; i >= 0; i--)
            {
                if (cellFill[x,i] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        //dem theo chieu cheo xuong \
        public static int count3(int x, int y, int[,] cellFill)
        {
            int count = 1;
            for (int i = x + 1, j = y + 1; i < 20 && j < 20; i++, j++)
            {
                if (cellFill[i,j] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (cellFill[i,j] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        //dem theo chieu cheo len /
        public static int count4(int x, int y, int[,] cellFill)
        {
            int count = 1;
            for (int i = x + 1, j = y - 1; i < 20 && j >= 0; i++, j--)
            {
                if (cellFill[i,j] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            for (int i = x - 1, j = y + 1; i >= 0 && j < 20; i--, j++)
            {
                if (cellFill[i,j] == cellFill[x,y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        public static bool isWin(int x, int y, int[,] cellFill)
        {
            if (cellFill[x,y] == 0)
            {
                return false;
            }
            // theo chieu doc
            if (count1(x, y, cellFill) == 5 && !(isBlock1(x, y, cellFill)))
            {
                return true;
            }
            //theo chieu ngang
            if (count2(x, y, cellFill) == 5 && !(isBlock2(x, y, cellFill)))
            {
                return true;
            }
            //theo cheo xuong \
            if (count3(x, y, cellFill) == 5 && !(isBlock3(x, y, cellFill)))
            {
                return true;
            }
            //theo chieu cheo /
            if (count4(x, y, cellFill) == 5 && !(isBlock4(x, y, cellFill)))
            {
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using System.IO;
namespace Caro_ai
{
    public partial class Board : PhoneApplicationPage
    {

        Image iconX, iconY;
        //mot so bien can dung den

        //che do choi 1 la ngvsmay, 2 la ngvsng
        int mode;

        //data chua du lieu khi tombstoning
        String data = "";

        //so tran thang
        int WinX, WinO;

        // mang chua XO
        int[,] cellFill = new int[20,20];
        
        //luot di hien tai
        bool turn = true;

        //doi tuong chua mang luong tu
        EBoard eb = new EBoard();

        //mang tinh diem
        int[] DScore = new int[5] {0, 1, 9, 81, 729};
        int[] AScore = new int[5] {0, 2, 18, 162, 1458};

        //so nc di tinh truoc hien tai
        public int depth = 0;

        //kiem tra thang
        public bool fWin = false;

        // so nc di tinh trc toi da va chieu sau tinh truoc toi da
        public static int maxDepth = 11;
        public static int maxMove = 3;

        //mang chua cac nc di tinh truoc den thang
        Point[] PCMove = new Point[maxMove + 2];
        Point[] HumanMove = new Point[maxMove + 2];
        Point[] WinMove = new Point[maxDepth + 2];
        
        
        //contructor
        public Board()
        {
            
            //tao anh icon X
            iconX = new Image();
            iconX.Source = new BitmapImage(new Uri("images/Xicon.png", UriKind.Relative));
            
            iconY = new Image();
            iconY.Source = new BitmapImage(new Uri("images/Xicon.png", UriKind.Relative));
            InitializeComponent();

            //khoi tao cellFill (gan gia tri 0)
            this.clear();
            WinO = 0;
            WinX = 0;
            
        }

        #region CAc ham xu ly su kien
        
        //khi chuyen den trang nay
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string strmode = NavigationContext.QueryString["mode"];
            this.mode = int.Parse(strmode);
            if (mode == 1)
            {
                //chuyen ham su kien
                for (int i = 0; i < board.Children.Count; i++)
                {
                    var child = VisualTreeHelper.GetChild(board, i);
                    Image imgtype = child as Image;
                    if (imgtype == null)
                    {
                        MessageBox.Show("Loi do hoa!");
                    }
                    else
                    {
                        imgtype.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(icon_Tap_2);
                        imgtype.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(icon_Tap);
                        imgtype.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(icon_Tap);
                    }
                }

                //doi background
                try
                {
                    ImageBrush bg = new ImageBrush();
                    bg.ImageSource = new BitmapImage(new Uri("images/Playbg.png", UriKind.Relative));
                    LayoutRoot.Background = bg;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Khong doi dc bg");
                }
            }
            else if (mode == 2)
            {
                //chuyen ham su kien
                for (int i = 0; i < board.Children.Count; i++)
                {
                    var child = VisualTreeHelper.GetChild(board, i);
                    Image imgtype = child as Image;
                    if (imgtype == null)
                    {
                        MessageBox.Show("Loi do hoa!");
                    }
                    else
                    {
                        imgtype.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(icon_Tap);
                        imgtype.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(icon_Tap_2);
                        imgtype.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(icon_Tap_2);
                    }
                }
                //doi background
                try
                {
                    ImageBrush bg = new ImageBrush();
                    bg.ImageSource = new BitmapImage(new Uri("images/playbg2.png", UriKind.Relative));


                    LayoutRoot.Background = bg;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Khong doi duoc bg");
                }
            }
            
            /*if (loadState(mode))
            {
                draw();
            } */        
        }

        //khi roi khoi trang nay (chua thoat)
       /* protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            saveState(mode);
        }*/


        //khi anh icon dc tap (mode =1)
        private void icon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //am thanh
            


            //xu ly do hoa
            Image itmp = (Image)sender;
            string name = itmp.Name;
            int index = int.Parse(name.Substring(1));
            int x = index / 20;
            int y = index % 20;
            //           MessageBox.Show(index+" "+x+";"+y);
            if (turn == true && cellFill[x, y] == 0)
            {
                turn = false;
                cellFill[x, y] = 1;
                //itmp.Source = new BitmapImage(new Uri("images/Xicon.png", UriKind.Relative));
                drawX(x, y);

                if (Arbitration.isWin(x, y, cellFill) == true)
                {
                    MessageBox.Show("X win!");
                    WinX++;
                    numx.Text = WinX.ToString();
                    //clear graphics
                    clearGraphics();
                    //clear cellFill
                    clear();
                }
                else
                {
                    AI();
                    if (fWin == true)
                    {
                        x = (int)WinMove[0].X;
                        y = (int)WinMove[0].Y;
                        //MessageBox.Show(x + ";" + y);
                    }
                    else
                    {
                        EvalBoard(turn, ref eb);

                        Point tmp = new Point();

                        tmp = eb.maxPos();
                        x = (int)tmp.X;
                        y = (int)tmp.Y;
                    }
                    cellFill[x, y] = -1;
                    drawO(x, y);
                    turn = true;
                    if (Arbitration.isWin(x, y, cellFill) == true)
                    {
                        MessageBox.Show("O Win!");
                        WinO++;
                        numo.Text = WinO.ToString();
                        //clear graphics
                        clearGraphics();
                        //clear cellFill
                        clear();
                    }
                }
                
            }
            else if (turn == false)
            {
                // doi turn
                cellFill[9, 10] = -1;
                drawO(9, 10);
                turn = true;
            }
        }

        //tap mode=2
        private void icon_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image itmp = (Image)sender;
            string name = itmp.Name;
            int index = int.Parse(name.Substring(1));
            int x = index / 20;
            int y = index % 20;
            if (turn == true && cellFill[x, y] == 0)
            {
                turn = false;
                cellFill[x, y] = 1;
                drawX(x, y);
                if (Arbitration.isWin(x, y, cellFill) == true)
                {
                    MessageBox.Show("X win ^^!");
                    WinX++;
                    numx.Text = WinX.ToString();
                    //clear graphics
                    clearGraphics();
                    //clear cellFill
                    clear();
                }
            }
            else if (turn == false && cellFill[x, y] == 0)
            {
                turn = true;
                cellFill[x, y] = -1;
                drawO(x, y);
                if (Arbitration.isWin(x, y, cellFill) == true)
                {
                    MessageBox.Show("O win ^^!");
                    WinO++;
                    numo.Text = WinO.ToString();
                    //clear graphics
                    clearGraphics();
                    //clear cellFill
                    clear();
                }
            }
        }

        //cac nut bam
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            clear();
            clearGraphics();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            WinO = 0;
            numo.Text = WinO.ToString();
            WinX = 0;
            numx.Text = WinX.ToString();
            clearGraphics();
            clear();
        }
        #endregion

        #region 
        //Cac ham xu ly do hoa
        private void draw()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (cellFill[i, j] == 1)
                    {
                        drawX(i, j);
                    }
                    else if (cellFill[i, j] == -1)
                    {
                        drawO(i, j);
                    }
                }
            }
        }
        private void drawX(int x, int y)
        {
            string name = "i" + (x * 20 + y).ToString();
            int countchild = board.Children.Count;
            for (int i = 0; i < countchild; i++)
            {
                var child = VisualTreeHelper.GetChild(board, i);
                Image imagetype = child as Image;
                if (imagetype == null)
                {
                    MessageBox.Show("KHong tim thay anh!");
                }
                else if (imagetype.Name == name)
                {
                    imagetype.Source = new BitmapImage(new Uri("images/Xicon.png", UriKind.Relative));
                }

            }
        }
        private void drawO(int x, int y)
        {
            string name = "i" + (x * 20 + y).ToString();
            int countchild = board.Children.Count;
            for (int i = 0; i < countchild; i++)
            {
                var child = VisualTreeHelper.GetChild(board, i);
                Image imagetype = child as Image;
                if (imagetype == null)
                {
                    MessageBox.Show("KHong tim thay anh!");
                }
                else if (imagetype.Name == name)
                {
                    imagetype.Source = new BitmapImage(new Uri("images/Oicon.png", UriKind.Relative));
                }

            }
        }

        private void clearGraphics()
        {
            for (int i = 0; i < board.Children.Count; i++)
            {
                var child = VisualTreeHelper.GetChild(board, i);
                Image imagetype = child as Image;
                imagetype.Source = new BitmapImage(new Uri("images/noneicon.png", UriKind.Relative));
            }
        }

        private void clear()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    cellFill[i, j] = 0;
                }

            }
        }
        #endregion

        #region cac ham xu ly am thanh
       

        #endregion

        #region Cac ham tinh nc di cua may
        // ham tinh gia tri luong gia
        // turn true nguoi, false may 
        // quy dinh nguoi danh x turn la true, may danh o turn la false
        private void EvalBoard(bool turn, ref EBoard eb)
        {
            int eX, eO;
            eb.clear();


            // danh gia theo cot (chieu doc)
            for (int i = 0; i < 20; i++)
            {          // theo chieu ngang(moi lan la 1 cot)
                for (int j = 0; j < 16; j++)
                {    // theo chieu doc (moi lan 5 phan tu trong cot)
                    eX = 0;
                    eO = 0;
                    for (int k = 0; k < 5; k++)
                    {
                        if (cellFill[j + k,i] == 1)
                        {
                            eX++;
                        }
                        if (cellFill[j + k,i] == -1)
                        {
                            eO++;
                        }
                    }
                    if (eO * eX == 0 && eO != eX)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            if (cellFill[j + k, i] != 0)
                            {
                                //kiem tra block
                                if (Arbitration.isBlock1(j + k, i, cellFill) == true)
                                {
                                    break;
                                }
                            }
                            if (cellFill[j + k,i] == 0)
                            { // chua duoc danh'
                                
                                
                                
                                // chi co cac nc O, nc cua may
                                if (eX == 0)
                                {
                                    #region kiem tra block
                                    cellFill[j + k, i] = -1;
                                    if (Arbitration.isBlock1(j + k, i, cellFill) == true)
                                    {
                                        cellFill[j + k, i] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[j + k, i] = 0;
                                    }
                                    #endregion
                                    if (turn == true)
                                    {// neu luot cua ng choi la nc phong thu cua may
                                        eb.eBoard[j + k, i] += DScore[eO];
                                    }
                                    else
                                    {//neu luot cua may la nc tan cong cua may
                                        eb.eBoard[j + k, i] += AScore[eO];
                                    }
                                }
                                // chi co nc X, nc cua ng choi
                                if (eO == 0)
                                {
                                    #region kiem tra block
                                    cellFill[j + k, i] = 1;
                                    if (Arbitration.isBlock1(j + k, i, cellFill) == true)
                                    {
                                        cellFill[j + k, i] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[j + k, i] = 0;
                                    }
                                    #endregion
                                    if (turn == false)
                                    {// neu luot cua may la nc phong thu cua ng choi
                                        eb.eBoard[j + k,i] += DScore[eX];
                                    }
                                    else
                                    {// neu luot cua ng choi la nc tan cong cua ng choi
                                        eb.eBoard[j + k,i] += AScore[eX];
                                    }
                                }
                                // sap thang hoac sap thua thi danh
                                if (eX == 4 || eO == 4)
                                {
                                    eb.eBoard[j + k,i] *= 2;
                                }
                            }
                        }
                    }
                }
            }

            // theo chieu ngang (hang) 
            for (int i = 0; i < 20; i++)
            {          // theo chieu doc (moi lan la 1 hang)
                for (int j = 0; j < 16; j++)
                {    // theo chieu ngang (moi lan la 5 phan tu trong hang)
                    eX = 0;
                    eO = 0;
                    for (int k = 0; k < 5; k++)
                    {
                        if (cellFill[i,j + k] == 1)
                        {
                            eX++;
                        }
                        if (cellFill[i,j + k] == -1)
                        {
                            eO++;
                        }
                    }
                    if (eO * eX == 0 && eO != eX)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            //kiem tra block
                            if (cellFill[i, j + k] != 0)
                            {
                                if (Arbitration.isBlock2(i, j + k, cellFill) == true)
                                {
                                    break;
                                }
                            }
                            if (cellFill[i,j + k] == 0)
                            { // chua duoc danh'
                                // chi co cac nc O, nc cua may
                                if (eX == 0)
                                {
                                    #region kiem tra block
                                    cellFill[i, j + k] = -1;
                                    if (Arbitration.isBlock2(i, j + k, cellFill) == true)
                                    {
                                        cellFill[i, j + k] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[i, j + k] = 0;
                                    }
                                    #endregion
                                    if (turn == true)
                                    {// neu luot cua ng choi la nc phong thu cua may
                                        eb.eBoard[i,j + k] += DScore[eO];
                                    }
                                    else
                                    {//neu luot cua may la nc tan cong cua may
                                        eb.eBoard[i,j + k] += AScore[eO];
                                    }
                                }
                                // chi co nc X, nc cua ng choi
                                if (eO == 0)
                                {
                                    #region kiem tra block
                                    cellFill[i, j + k] = 1;
                                    if (Arbitration.isBlock2(i, j + k, cellFill) == true)
                                    {
                                        cellFill[i, j + k] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[i, j + k] = 0;
                                    }
                                    #endregion
                                    if (turn == false)
                                    {// neu luot cua may la nc phong thu cua ng choi
                                        eb.eBoard[i,j + k] += DScore[eX];
                                    }
                                    else
                                    {// neu luot cua ng choi la nc tan cong cua ng choi
                                        eb.eBoard[i,j + k] += AScore[eX];
                                    }
                                }
                                // sap thang hoac sap thua thi danh
                                if (eX == 4 || eO == 4)
                                {
                                    eb.eBoard[i,j + k] *= 2;
                                }
                            }
                        }
                    }
                }
            }

            // theo chieu cheo xuong \
            for (int i = 0; i < 16; i++)
            {          //  noi chung la chuan (col)
                for (int j = 0; j < 16; j++)
                {    //  noi chung la chuan (row)
                    eX = 0;
                    eO = 0;
                    for (int k = 0; k < 5; k++)
                    {
                        if (cellFill[j + k,i + k] == 1)
                        {
                            eX++;
                        }
                        if (cellFill[j + k,i + k] == -1)
                        {
                            eO++;
                        }
                    }
                    if (eO * eX == 0 && eO != eX)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            if (cellFill[j + k, i + k] != 0)
                            {
                                if (Arbitration.isBlock3(j + k, i + k, cellFill) == true)
                                {
                                    break;
                                }
                            }
                            if (cellFill[j + k,i + k] == 0)
                            { // chua duoc danh'
                                // chi co cac nc O, nc cua may
                                if (eX == 0)
                                {
                                    #region kiem tra block
                                    cellFill[j + k, i + k] = -1;
                                    if (Arbitration.isBlock3(j + k, i + k, cellFill) == true)
                                    {
                                        cellFill[j + k, i + k] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[j + k, i + k] = 0;
                                    }
                                    #endregion
                                    if (turn == true)
                                    {// neu luot cua ng choi la nc phong thu cua may
                                        eb.eBoard[j + k,i + k] += DScore[eO];
                                    }
                                    else
                                    {//neu luot cua may la nc tan cong cua may
                                        eb.eBoard[j + k,i + k] += AScore[eO];
                                    }
                                }
                                // chi co nc X, nc cua ng choi
                                if (eO == 0)
                                {
                                    #region kiem tra block
                                    cellFill[j + k, i + k] = 1;
                                    if (Arbitration.isBlock3(j + k, i + k, cellFill) == true)
                                    {
                                        cellFill[j + k, i + k] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[j + k, i + k] = 0;
                                    }
                                    #endregion
                                    if (turn == false)
                                    {// neu luot cua may la nc phong thu cua ng choi
                                        eb.eBoard[j + k,i + k] += DScore[eX];
                                    }
                                    else
                                    {// neu luot cua ng choi la nc tan cong cua ng choi
                                        eb.eBoard[j + k,i + k] += AScore[eX];
                                    }
                                }
                                // sap thang hoac sap thua thi danh
                                if (eX == 4 || eO == 4)
                                {
                                    eb.eBoard[j + k,i + k] *= 2;
                                }
                            }
                        }
                    }
                }
            }
            //theo chieu cheo len 

            for (int i = 4; i < 20; i++) {          // row
                for (int j = 0; j < 16; j++) {    // col
                    eX = 0;
                    eO = 0;
                    for (int k = 0; k < 5; k++) {
                        if (cellFill[i - k,j + k] == 1) {
                            eX++;
                        }
                        if (cellFill[i - k,j + k] == -1) {
                            eO++;
                        }
                    }
                    if (eO * eX == 0 && eO != eX) {
                        for (int k = 0; k < 5; k++) {
                            if (cellFill[i - k, j + k] != 0)
                            {
                                if (Arbitration.isBlock4(i - k, j + k, cellFill) == true)
                                {
                                    break;
                                }
                            }
                            if (cellFill[i - k,j + k] == 0) { // chua duoc danh'
                                // chi co cac nc O, nc cua may
                                if (eX == 0) {
                                    #region kiem tra block
                                    cellFill[i - k, j + k] = -1;
                                    if (Arbitration.isBlock4(i - k, j + k, cellFill) == true)
                                    {
                                        cellFill[i - k, j + k] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[i - k, j + k] = 0;
                                    }
                                    #endregion
                                    if (turn == true) {// neu luot cua ng choi la nc phong thu cua may
                                        eb.eBoard[i - k,j + k] += DScore[eO];
                                    } else {//neu luot cua may la nc tan cong cua may
                                        eb.eBoard[i - k,j + k] += AScore[eO];
                                    }
                                }
                                // chi co nc X, nc cua ng choi
                                if (eO == 0) {
                                    #region kiem tra block
                                    cellFill[i - k, j + k] = 1;
                                    if (Arbitration.isBlock4(i - k, j + k, cellFill) == true)
                                    {
                                        cellFill[i - k, j + k] = 0;
                                        break;
                                    }
                                    else
                                    {
                                        cellFill[i - k, j + k] = 0;
                                    }
                                    #endregion
                                    if (turn == false) {// neu luot cua may la nc phong thu cua ng choi
                                        eb.eBoard[i - k,j + k] += DScore[eX];
                                    } else {// neu luot cua ng choi la nc tan cong cua ng choi
                                        eb.eBoard[i - k,j + k] += AScore[eX];
                                    }
                                }
                                // sap thang hoac sap thua thi danh
                                if (eX == 4 || eO == 4) {
                                    eb.eBoard[i - k,j + k] *= 2;
                                }
                            }
                        }
                    }
                }
            }
        }


        //ham chon nc nc di tu bang luong gia o tren
        // danh thu cac nc di, kiem tra thang, lam de quy,..
        private void move()
        {
            if (depth > maxDepth)
            {
                return;
            }
            depth++;
            fWin = false;

            Point pcMove = new Point();
            Point hmMove = new Point();
            int countMove;


            // may goi move => turn = false
            EvalBoard(false,ref eb);

            // lay ra maxMove nc di tot nhat cua may cho vao mang PCMove
            Point tmp = new Point();
            for (int i = 0; i < maxMove; i++)
            {
                tmp = eb.maxPos();
                PCMove[i] = tmp;
                eb.eBoard[(int)tmp.X,(int)tmp.Y] = 0;
            }

            countMove = 0;
            while (countMove < maxMove)
            {

                //danh thu cac nc di cua may'
                pcMove = PCMove[countMove++];
                cellFill[(int)pcMove.X,(int)pcMove.Y] = -1; // khi turn false thi may la -1
                WinMove[depth - 1] = pcMove;
                if (Arbitration.isWin((int)pcMove.X, (int)pcMove.Y, cellFill) == true)
                {
                    fWin = true;
                    cellFill[(int)pcMove.X,(int)pcMove.Y] = 0;
                    cellFill[(int)hmMove.X,(int)hmMove.Y] = 0;
                    return;
                }

                //tim cac nc di toi uu cua nguoi
                eb.clear();
                EvalBoard(true,ref eb);

                // lay ra maxmove nc di co diem vao nhat cua nguoi
                for (int i = 0; i < maxMove; i++)
                {
                    tmp = eb.maxPos();
                    HumanMove[i] = tmp;
                    eb.eBoard[(int)tmp.X,(int)tmp.Y] = 0;
                }

                //danh thu nuoc di cua nguoi
                for (int i = 0; i < maxMove; i++)
                {
                    hmMove = HumanMove[i];
                    cellFill[(int)hmMove.X,(int)hmMove.Y] = 1;
                    if (Arbitration.isWin((int)hmMove.X, (int)hmMove.Y, cellFill) == true)
                    {
                        cellFill[(int)pcMove.X,(int)pcMove.Y] = 0;
                        cellFill[(int)hmMove.X,(int)hmMove.Y] = 0;
                        break;
                    }
                    move();
                    cellFill[(int)hmMove.X,(int)hmMove.Y] = 0;
                }
                cellFill[(int)pcMove.X, (int)pcMove.Y] = 0;
            }
        }


        //Cho 2 phan tren vao day de tim ra nc di toi uu hoac dan den thang
        private void AI()
        {
            for (int i = 0; i < maxMove; i++)
            {
                WinMove[i] = new Point();
                PCMove[i] = new Point();
                HumanMove[i] = new Point();
            }
            depth = 0;
            move();
        }


        #endregion

        private void reset(object sender, RoutedEventArgs e)
        {

        }

        private void setIcon(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        	// TODO: Add event handler implementation here.
        }

       
       
     
        


    }
}

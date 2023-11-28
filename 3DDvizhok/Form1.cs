using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace _3DDvizhok
{
    public partial class Form1 : Form
    {
        Player player = new Player { location = new Point3D { x = 540, y = 436, z = 5 }, viewDirection = 193, viewAngle = 120, viewRange = 1000, viewRotate = 2, speed = 3, size = new Size3D {length = 25, width = 25, height = 25 } };
        double divideAngle =1;

        List<Box> boxes = new List<Box>();

        bool show2D = true;
        bool show3D = true; 
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            timerButton.Start();
           

            
            boxes.Add(new Box { location = new Point3D { x = 250, y = 50, z = 0 }, size = new Size3D { length = 25, width = 25, height = 50 } });
            boxes.Add(new Box { location = new Point3D { x = 120, y = 230, z = 0 }, size = new Size3D { length = 60, width = 60, height = 50 } });
            boxes.Add(new Box { location = new Point3D { x = 250, y = 125, z = 0 }, size = new Size3D { length = 10, width = 25, height = 50 } });

            for (int i=0; i<boxes.Count; i++)
            {
                boxes[i].points = FindPoints(boxes[i].location, boxes[i].size);
            }
        }

        bool pressedA = false;
        bool pressedD = false;
        bool pressedW = false;
        bool pressedS = false;
        

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           

           
        }

        public Point3D[] FindPoints(Point3D location, Size3D size)
        {
            Point3D[] points = new Point3D[8];
            points[0] = location; // перед слева снизу
            points[1] = new Point3D { x = location.x, y = location.y, z = location.z + size.height }; //  перед слева сверху
            points[2] = new Point3D { x = location.x, y = location.y + size.length, z = location.z + size.height }; // перед справа сверху
            points[3] = new Point3D { x = location.x, y = location.y + size.length, z = location.z }; // перед справа снизу
            points[4] = new Point3D { x = location.x + size.width, y = location.y, z = location.z }; // зад слева снизу
            points[5] = new Point3D { x = location.x + size.width, y = location.y, z = location.z + size.height }; // зад слева сверху
            points[6] = new Point3D { x = location.x + size.width, y = location.y + size.length, z = location.z + size.height }; // зад спроава сверху
            points[7] = new Point3D { x = location.x + size.width, y = location.y + size.length, z = location.z }; // зад справа снизу
            return points;


            // чтобы нарисовать куб. I грань - 0-1-2-3, II грань 4-5-6-7, III грань 0-1-5-4, IV грань 2-3-7-6, V грань 1-2-6-5, VI грань 0-3-7-4
        }


            private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap bit1 = new Bitmap(panel1.Width, panel1.Height);

            Bitmap bit2 = new Bitmap(panel2.Width, panel2.Height);
            Graphics g = Graphics.FromImage(bit1);
                
                g.Clear(Color.Gray);

                Brush boxBrush = new SolidBrush(Color.Black);
                for (int i = 0; i < boxes.Count; i++)
                {
                   if (show2D) g.FillRectangle(boxBrush, new RectangleF(new Point(boxes[i].location.x, boxes[i].location.y), new Size(boxes[i].size.width, boxes[i].size.length)));
                }
                Pen vectorViewPen = new Pen(Color.Black, 1);

                player.viewVector = new List<Vector>();
                int v1 = Convert.ToInt32(player.location.x + Math.Cos(player.viewDirection * Math.PI / 180) * (player.size.width / 2));
                int v2 = Convert.ToInt32(player.location.y + Math.Sin(player.viewDirection * Math.PI / 180) * (player.size.length / 2));
                Point p1 = new Point(v1, v2);
                for (int i = 0; i <= (player.viewAngle / divideAngle); i++)
                {

                    double angleDegree = (player.viewDirection - (player.viewAngle / 2.0) + divideAngle * i);
                    double angle = angleDegree * Math.PI / 180.0;
                    bool isBlocked = false;




                    int viewRange = player.viewRange;
                    Box collisionBox = null;

                for (int j = 0; j < boxes.Count; j++)
                {

                    RectangleF r2 = new RectangleF(new Point(boxes[j].location.x, boxes[j].location.y), new Size(boxes[j].size.width, boxes[j].size.length));
                    r2.Location = new Point(boxes[j].location.x, boxes[j].location.y);

                    for (int k = 0; k < player.viewRange; k++)
                    {
                        int x = Convert.ToInt32(p1.X + Math.Cos(angle) * (k));
                        int y = Convert.ToInt32(p1.Y + Math.Sin(angle) * (k));
                        if (r2.Contains(p1) || r2.Contains(x, y))
                        {
                            collisionBox = boxes[j];
                            if (k < viewRange)
                            {
                                viewRange = k;
                            }
                            isBlocked = true;
                            break;
                        }
                    }
                }
                    Rectangle panel = panel1.DisplayRectangle;
                    panel.Location = panel.Location;
                    if (!isBlocked)
                    {
                        for (int k = 0; k < player.viewRange; k++)
                        {
                            int x = Convert.ToInt32(p1.X + Math.Cos(angle) * (k));
                            int y = Convert.ToInt32(p1.Y + Math.Sin(angle) * (k));
                            if (!panel.Contains(p1) || !panel.Contains(x, y))
                            {
                                viewRange = k;
                                break;
                            }
                        }
                    }




                    int x2 = Convert.ToInt32(p1.X + Math.Cos(angle) * (viewRange));
                    int y2 = Convert.ToInt32(p1.Y + Math.Sin(angle) * (viewRange));
                    Point p2 = new Point(x2, y2);
                    if (collisionBox != null)
                    {
                        player.viewVector.Add(new Vector { p1 = new Point3D { x = p1.X, y = p1.Y, z = 0 }, p2 = new Point3D { x = p2.X, y = p2.Y, z = 0 }, angle = angleDegree, boxCollision = collisionBox });
                    }
                    player.viewVector.Add(new Vector { p1 = new Point3D { x = p1.X, y = p1.Y, z = 0 }, p2 = new Point3D { x = p2.X, y = p2.Y, z = 0 }, angle = angleDegree });
                    if (angleDegree == player.viewDirection)
                    {
                        vectorViewPen.Color = Color.Red;
                        if (show2D) g.DrawLine(vectorViewPen, new Point(player.viewVector[i].p1.x, player.viewVector[i].p1.y), new Point(player.viewVector[i].p2.x, player.viewVector[i].p2.y));
                    }
                    else
                    {
                        vectorViewPen.Color = Color.Black;
                        if (i == 0 || i == player.viewAngle / divideAngle - 1)
                        {
                            if (show2D) g.DrawLine(vectorViewPen, new Point(player.viewVector[i].p1.x, player.viewVector[i].p1.y), new Point(player.viewVector[i].p2.x, player.viewVector[i].p2.y));
                        }
                        // po vektoram
                    }


                }
                for (int i = 0; i < player.viewVector.Count; i++)
                {
                    if (player.viewVector[i].boxCollision != null)
                    {
                        label1.Text = player.viewVector[i].boxCollision.location.x + ", " + player.viewVector[i].boxCollision.location.y;
                    }
                }
                Brush playerBrush = new SolidBrush(Color.Red);

               if (show2D) g.FillEllipse(playerBrush, new Rectangle(player.location.x - player.size.width / 2, player.location.y - player.size.length / 2, player.size.width, player.size.length));
                /*label1.Text = "";
                for (int i=0; i<player.viewVector.Count; i++)
                {
                    if (i%5!=0)
                    {
                        return;
                    }
                    Point vp1 = player.viewVector[i].p1;
                    Point vp2 = player.viewVector[i].p2;

                    int vectorLength = Convert.ToInt32(Math.Sqrt(Math.Pow(vp1.X - vp2.X, 2) + Math.Pow(vp1.Y - vp2.Y, 2)));
                    label1.Text += vectorLength + ", ";
                }*/
            
           
                int v11 = Convert.ToInt32(player.location.x + Math.Cos(player.viewDirection * Math.PI / 180) * (player.size.width / 2));
                int v22 = Convert.ToInt32(player.location.y + Math.Sin(player.viewDirection * Math.PI / 180) * (player.size.length / 2));
                Point3D p11 = new Point3D { x = v11, y = v22, z = player.location.z };
                List<Point> polygons = new List<Point>();
            Graphics g2 = Graphics.FromImage(bit2);
                g2.Clear(Color.LightGray);


                int oneDegree = panel2.Width / player.viewAngle;
                Pen forBox = new Pen(Color.Black, oneDegree);
                for (int i = 0; i < player.viewVector.Count; i++)
                {
                    Point p2 = new Point(player.viewVector[i].p2.x, player.viewVector[i].p2.y);


                    int xdiff = Math.Abs(p11.x - p2.X);
                    int ydiff = Math.Abs(p11.y - p2.Y);
                    int zdiff = Math.Abs(p11.z);

                    int d3x = i * oneDegree;
                    int d3y = Convert.ToInt32(panel2.Height - Math.Sqrt(xdiff + ydiff + zdiff));
                    Point d3point = new Point(d3x, d3y);


                    polygons.Add(d3point);
                    Box box = player.viewVector[i].boxCollision;
                    if (box != null)
                    {
                        int vectorlength = Convert.ToInt32(Math.Sqrt(Math.Pow(p11.x - player.viewVector[i].p2.x, 2) + Math.Pow(p11.y - player.viewVector[i].p2.y, 2)));
                        int oneColor = player.viewRange / 255;
                        forBox.Color = Color.FromArgb(vectorlength / oneColor, vectorlength / oneColor, vectorlength / oneColor);
                    Rectangle rect = new Rectangle(new Point(d3point.X, Convert.ToInt32(d3point.Y - box.size.height - (500 - vectorlength) / 2)), new Size(oneDegree, d3point.Y - box.size.height + (500 - vectorlength) / 2));
                        if (show3D) g2.DrawRectangle(forBox, rect);
            
                    }

                    
                    
                    
                    
                }
                polygons.Add(new Point(panel2.Size.Width, panel2.Size.Height));
                polygons.Add(new Point(0, panel2.Size.Height));
                Brush brush = new SolidBrush(Color.Gray);
                Pen pen = new Pen(Color.Gray, 3);
                if (show3D) g2.FillPolygon(brush, polygons.ToArray());

            Graphics panel_1 = panel1.CreateGraphics();
            Graphics panel_2 = panel2.CreateGraphics();
            panel_1.DrawImage(bit1, Point.Empty);
            panel_2.DrawImage(bit2, Point.Empty);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {

                pressedA = false ;

            }
            if (e.KeyCode == Keys.D)
            {

                pressedD = false;

            }
            if (e.KeyCode == Keys.W)
            {
                pressedW = false;
            }
            if (e.KeyCode == Keys.S)
            {
                pressedS = false;
            }
        }

      
        private void timerButton_Tick(object sender, EventArgs e)
        {
          if (pressedA)
            {
                player.viewDirection -= player.viewRotate;
                if (player.viewDirection < 0)
                {
                    player.viewDirection = 360-player.viewDirection;
                }
            }
          if (pressedD)
            {
                player.viewDirection += player.viewRotate;
                if (player.viewDirection > 360)
                {
                    player.viewDirection = player.viewDirection-360;
                }
            }
          if (pressedW)
            {
                double angle = (player.viewDirection * Math.PI / 180.0);
                int x1 = Convert.ToInt32(player.location.x + Math.Cos(angle) * player.speed);
                int y1 = Convert.ToInt32(player.location.y + Math.Sin(angle) * player.speed);
                Rectangle r1 = new Rectangle(x1 - player.size.width / 2, y1 - player.size.length / 2, player.size.width, player.size.length);
                r1.Location = new Point(x1 - player.size.width / 2, y1 - player.size.length / 2);
                for (int i = 0; i < boxes.Count; i++)
                {
                    Rectangle r2 = new Rectangle(new Point(boxes[i].location.x, boxes[i].location.y), new Size(boxes[i].size.width, boxes[i].size.length));
                    r2.Location = new Point(boxes[i].location.x, boxes[i].location.y);
                    if (r1.IntersectsWith(r2))
                    {
                        return;
                    }
                    
                }
                Rectangle panel = panel1.DisplayRectangle;
                panel.Location = panel.Location;

                Point px1 = r1.Location;
                Point px2 = new Point(r1.Location.X + r1.Width, r1.Location.Y);
                Point px3 = new Point(r1.Location.X, r1.Location.Y + r1.Height);
                Point px4 = new Point(r1.Location.X + r1.Width, r1.Location.Y + r1.Height);
                if (panel.Contains(px1) && panel.Contains(px2) && panel.Contains(px3) && panel.Contains(px4))
                {
                    player.location = new Point3D { x= x1, y=y1, z=player.location.z };
                }

            }
            if (pressedS)
            {
                double angle = (player.viewDirection * Math.PI / 180.0);
                int x1 = Convert.ToInt32(player.location.x - Math.Cos(angle) * player.speed);
                int y1 = Convert.ToInt32(player.location.y - Math.Sin(angle) * player.speed);
                Rectangle r1 = new Rectangle(x1 - player.size.width / 2, y1 - player.size.length / 2, player.size.width, player.size.length);
                r1.Location = new Point(x1 - player.size.width / 2, y1 - player.size.length / 2);
                for (int i = 0; i < boxes.Count; i++)
                {
                    Rectangle r2 = new Rectangle(new Point(boxes[i].location.x, boxes[i].location.y), new Size(boxes[i].size.width, boxes[i].size.length));
                    r2.Location = new Point(boxes[i].location.x, boxes[i].location.y);
                    if (r1.IntersectsWith(r2))
                    {
                        return;
                    }

                }
                Rectangle panel = panel1.DisplayRectangle;
                panel.Location = panel.Location;

                Point px1 = r1.Location;
                Point px2 = new Point(r1.Location.X + r1.Width, r1.Location.Y);
                Point px3 = new Point(r1.Location.X, r1.Location.Y +r1.Height);
                Point px4 = new Point(r1.Location.X + r1.Width, r1.Location.Y + r1.Height);
                if (panel.Contains(px1) && panel.Contains(px2) && panel.Contains(px3) && panel.Contains(px4))
                {
                    player.location = new Point3D { x = x1, y = y1, z = player.location.z };
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                
                pressedA = true;
               
            }
            if (e.KeyCode == Keys.D)
            {
              
                pressedD = true;
              
            }
            if (e.KeyCode == Keys.W)
            {
                pressedW = true;
            }
            if (e.KeyCode == Keys.S)
            {
                pressedS = true;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
         




         /*   int v1 = Convert.ToInt32(player.location.x + Math.Cos(player.viewDirection * Math.PI / 180) * (player.size.width / 2));
            int v2 = Convert.ToInt32(player.location.y + Math.Sin(player.viewDirection * Math.PI / 180) * (player.size.length / 2));
            Point3D p1 = new Point3D { x = v1, y = v2, z = player.location.z };
            List<Point> polygons = new List<Point>();
            Graphics g = panel2.CreateGraphics();
            g.Clear(Color.LightGray);

           
            int oneDegree = panel2.Width / player.viewAngle;
            Pen forBox = new Pen(Color.Black, oneDegree);
            for (int i=0; i<player.viewVector.Count; i++)
            {
                Point p2 = new Point(player.viewVector[i].p2.x, player.viewVector[i].p2.y);


                int xdiff = Math.Abs(p1.x-p2.X);
                int ydiff = Math.Abs(p1.y - p2.Y);
                int zdiff = Math.Abs(p1.z);
                
                int d3x = i * oneDegree;
                int d3y = Convert.ToInt32(panel2.Height - Math.Sqrt(xdiff + ydiff+ zdiff));
                Point d3point = new Point(d3x, d3y );

              
                polygons.Add(d3point);
                Box box = player.viewVector[i].boxCollision;
                if (box != null)
                {
                   int vectorlength = Convert.ToInt32(Math.Sqrt(Math.Pow(p1.x - player.viewVector[i].p2.x,2)+ Math.Pow(p1.y - player.viewVector[i].p2.y, 2)));
                    int oneColor = player.viewRange / 255;
                    forBox.Color = Color.FromArgb(vectorlength/oneColor, vectorlength / oneColor, vectorlength / oneColor);
                    g.DrawRectangle(forBox, new Rectangle(new Point(d3point.X, Convert.ToInt32(d3point.Y - box.size.height -  (500-vectorlength)/2)), new Size(oneDegree, d3point.Y - box.size.height + (500 - vectorlength) / 2)));
                }

              
                


            }
            polygons.Add(new Point(panel2.Size.Width, panel2.Size.Height));
            polygons.Add(new Point(0, panel2.Size.Height));
            Brush brush = new SolidBrush(Color.Gray);
            Pen pen = new Pen(Color.Gray, 3);
            g.FillPolygon(brush, polygons.ToArray());
*/
           /* for (int i=0; i<player.viewVector.Count; i++)
            {
                
                Pen pen = new Pen(Color.Red);
                Point p2 = player.viewVector[i].p2;
                int diff = Convert.ToInt32(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));

                int d3x = i * oneDegree;
                int d3y = Convert.ToInt32(panel2.Height + 30 - (diff / 5) + (-(player.viewDirection + Math.Abs(player.viewVector[i].angle)) / 6));
                Point d3point = new Point(d3x, d3y);
                g.DrawEllipse(pen, new Rectangle(d3point, new Size(2, 2)));
            }
*/

        }
    }

    public class Player
    {
        public Point3D location { get; set; }
        public int viewDirection { get; set; }
        public int speed { get; set; }
        public int viewAngle { get; set; }
        public int viewRange { get; set; }
        public int viewRotate { get; set; }
        public List<Vector> viewVector { get; set; }

        public Size3D size { get; set; }
    }

    public class Box
    {
        public Point3D location { get; set; }
        public Size3D size { get; set; }
        public Point3D[] points { get; set; }
    }

    public class Vector
    {
        public Point3D p1 { get; set; }
        public Point3D p2 { get; set; }
        public double angle { get; set; }

        public Box boxCollision { get; set; }   
    }

    public class Size3D
    {
        public int width { get; set; }
        public int height { get; set; }
        public int length { get; set; }
    }
    public class Point3D
    {
        public int x { get; set; } 
        public int y { get; set; }
        public int z { get; set; }
    }
}

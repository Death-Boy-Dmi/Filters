using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filters
{
    public partial class MainForm : Form
    {
        TStructElem structElem;
        Bitmap image;
        Bitmap startImage;
        bool[,] matr;
        public MainForm()
        {
            InitializeComponent();
            panel1.Visible = false;
            panel1.Enabled = false;
            structElem = new TStructElem();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.jpg; *.bmp; *.png; | All Files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                startImage = image;
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter invertion = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(invertion);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar2.Value = 0;
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void гауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);

        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Stamping();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void Назад_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = startImage;
            pictureBox1.Refresh();
        }

        private void поворотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SpinFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сдвигToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new RemoveFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чБToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightnessFilterPlus();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightnessFilterMinus();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //matr = new bool[3,3]  { {false,true,false },{true,false,true }, {false,true,false } };
            pictureBox1.Image = MathMorfology.Erosion(image, matr);
            pictureBox1.Refresh();
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //matr = new bool[3, 3] { { false, true, false }, { true, false, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Dilation(image, matr);
            pictureBox1.Refresh();
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //matr = new bool[3, 3] { { false, true, false }, { true, false, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Dilation(MathMorfology.Erosion(image, matr), matr);
            pictureBox1.Refresh();
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //matr = new bool[3, 3] { { false, true, false }, { true, false, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Erosion(MathMorfology.Dilation(image, matr), matr);
            pictureBox1.Refresh();
        }

        private void gradToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //matr = new bool[3, 3] { { false, true, false }, { true, false, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Gradient(image, matr);
            pictureBox1.Refresh();
        }

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter(5);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void линейноеРастяжениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new LinearStrain();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayWorldFilter(ref image);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void задатьСтруктурныйЭлементToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1.Enabled = true;

        }

        private void yes_Click_1(object sender, EventArgs e)
        {
            structElem.sizeOfMatrix = int.Parse(sizeTXT.Text);
            for (int i = 0; i < structElem.sizeOfMatrix; i++)
            {
                dataGridView1.Columns.Add("Column" + (1 + i).ToString(), (i + 1).ToString());
                dataGridView1.Rows.Add();
            }

        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1.Enabled = false;
        }

        private void ok_Click_1(object sender, EventArgs e)
        {
            structElem.matrix = new int[structElem.sizeOfMatrix, structElem.sizeOfMatrix];
            for (int j = 0; j < structElem.sizeOfMatrix; j++)
            {
                for (int i = 0; i < structElem.sizeOfMatrix; i++)
                    structElem.matrix[i, j] = Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value);
            }
            matr = new bool[structElem.sizeOfMatrix, structElem.sizeOfMatrix];
            for (int j = 0; j < structElem.sizeOfMatrix; j++)
            {
                for (int i = 0; i < structElem.sizeOfMatrix; i++)
                    matr[i, j] = Convert.ToBoolean(structElem.matrix[i, j]);
            }
            panel1.Visible = false;
            panel1.Enabled = false;
        }
    }
    public class TStructElem
    {
        public int sizeOfMatrix;
        public int[,] matrix;
    }

}

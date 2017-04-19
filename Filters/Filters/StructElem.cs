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
    public partial class StructElem : Form
    {
        TStructElem structElem;
        public StructElem()
        {
            InitializeComponent();
            structElem = new TStructElem();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            structElem.matrix = new int[structElem.sizeOfMatrix, structElem.sizeOfMatrix];
            for (int j = 0; j < structElem.sizeOfMatrix; j++)
            {
                structElem.matrix[j, j] = Convert.ToInt32(dataGridView1.Rows[j].Cells[j].Value); //cчитываем данные
            }
            Close();
        }
        private void yes_Click(object sender, EventArgs e)
        {
            structElem.sizeOfMatrix = int.Parse(sizeTXT.Text);
            for (int i = 0; i<structElem.sizeOfMatrix; i++)
            {
                dataGridView1.Columns.Add("Column"+(1+i).ToString(), (i+1).ToString());
                dataGridView1.Rows.Add();
            }
        }
    }

    class TStructElem
    {
        public int sizeOfMatrix;
        public int[,] matrix;
    }
}

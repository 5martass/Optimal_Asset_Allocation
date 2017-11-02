using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace free
{
    public partial class Form1 : Form
    {
        int factCount;
        int varCount;
        int MoneyCount;
        string[] plan;
        string[] newPlan;

        public Form1()
        {
            InitializeComponent();
            
            grid.RowHeadersVisible = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            {
                if (txt1.Value > 1)
                { 
                btnStart.Enabled = true;
                    btnGen.Enabled = true;
                        num.Enabled = true;
                }
                else
                {
                btnStart.Enabled = false;
                    btnGen.Enabled = false;
                        num.Enabled = false;
                MessageBox.Show("Для выполнения расчетов количество предприятий, должно быть больше 1");
            }
        }
            int moneyPart = 0;
            try
            {
                factCount = Convert.ToInt32(txt1.Value);
                varCount = (Convert.ToInt32(txt2.Value)+1);

                if (txtMoney.Text == "")
                {
                    moneyPart = 1;
                }
                else
                {
                    MoneyCount = Convert.ToInt32(txtMoney.Text);
                    moneyPart = MoneyCount / (varCount - 1);
                }

                grid.Rows.Clear();
                grid.Columns.Clear();

                DataGridViewColumn col1 = new DataGridViewTextBoxColumn();
                col1.HeaderText = "Вкладываемые средства";
                grid.Columns.Add(col1);

                for (int i = 1; i < factCount + 1; i++)
                {
                    DataGridViewColumn col = new DataGridViewTextBoxColumn();
                    col.HeaderText = "Fact" + i.ToString();
                    grid.Columns.Add(col);
                }

                for (int i = 0; i < varCount; i++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    grid.Rows.Add(row);
                    grid.Rows[i].Cells[0].Value = (i*moneyPart).ToString();
                }
            }
            catch (FormatException ex) { System.Console.WriteLine(ex.Data.ToString()); }
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
             Random random = new Random();
             int[] mass;
             for (int i = 1; i < grid.Columns.Count; i++)
             {
                 mass = new int[grid.Rows.Count];
                 for (int j = 0; j < grid.Rows.Count; j++)
                 {
                     mass[j] = random.Next(0, (int)num.Value+1);
                 }
                 Array.Sort(mass);
                 for (int j = 0; j < grid.Rows.Count; j++)
                {
                    if (Convert.ToInt32(grid.Rows[j].Cells[0].Value) == 0)
                    {
                        grid.Rows[j].Cells[i].Value = 0;

                    }
                    else grid.Rows[j].Cells[i].Value = mass[j].ToString();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {          
            try
            {
                int[] X1 = new int[varCount];//Варианты вложений
                int[] Fn = new int[varCount];//выгодные функции из предыдущего шага
                int[] fn = new int[varCount];//функции для предприятий из начальной таблицы
                int[,] plus = new int[varCount, varCount];
                plan = new string[varCount];
                newPlan = new string[varCount];
                for (int i = 0; i < X1.Length; i++)//запись вариантов вложений
                {
                    X1[i] = Convert.ToInt32(grid.Rows[i].Cells[0].Value);
                }

                for (int i = 0; i < varCount; i++)//запись начального Fn
                {
                    Fn[i] = Convert.ToInt32(grid.Rows[i].Cells[1].Value);
                }



                for (int fact = 0; fact < factCount - 1; fact++)//итерации вычислениый
                {

                    for (int i = 0; i < varCount; i++)//запись начального fn
                    {
                        fn[i] = Convert.ToInt32(grid.Rows[i].Cells[fact + 2].Value);
                    }
                    for (int i = 0; i < varCount; i++)//сложение элементов на пересечении 
                    {

                        for (int j = 0; j < varCount; j++)
                        {
                            if (X1[varCount - 1] >= (X1[i] + X1[j])) plus[i, j] = fn[i] + Fn[j];
                        }
                    }
                    Array.Copy(newPlan, plan, plan.Length);
                    for (int var = 0; var < varCount; var++)
                    {
                        int variant = X1[var];
                        int max = 0;
                        int x = 0, y = 0;

                        for (int i = 0; i < varCount; i++)
                        {
                            for (int j = 0; j < varCount; j++)
                                if (X1[j] + X1[i] == variant && plus[i, j] > max)
                                {
                                    max = plus[i, j];
                                    x = i;
                                    y = j;
                                }

                        }
                        Fn[var] = plus[x, y];
                        if (fact == 0)
                        {
                            newPlan[var] = X1[y].ToString() + " " + X1[x].ToString();
                        }
                        else
                        {
                            newPlan[var] = plan[y] + " " + X1[x].ToString();
                        }
                    }


                }

                int max1 = 0;
                int x1 = 0;
                string res = "";
                for (int i = 0; i < varCount; i++)
                {
                    if (Fn[i] > max1)
                    {
                        max1 = Fn[i];
                        x1 = i;
                    }
                }
                int kolRes=0;
                for (int i = 0; i < grid.RowCount; i++)
                    if (Convert.ToInt32(grid.Rows[i].Cells[0].Value) != 0) kolRes++;
                res = "Наибольший прирост, при " + Convert.ToString(kolRes) +  " вариантах вложения между " + factCount.ToString();
                if (factCount <= 4)
                {
                   res+= " -мя предприятиями, составит " + Fn[x1].ToString() + "\n";
                }
                else
                    res+= " -ю предприятиями, составит " + Fn[x1].ToString() + "\n";
                for (int i = 0; i < factCount; i++)
                {
                    res += (i + 1).ToString() + "-му предприятию выделить " + newPlan[x1].Split(' ')[i] + "\n";
                    //res += "вложить в " + (i + 1).ToString() + " предприятие " + newPlan[x1].Split(' ')[i] + "\n";
                }
                MessageBox.Show(res, "Ответ", MessageBoxButtons.OK);

            }
            catch (Exception) { }
        }

        private void grid_Sorted(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show(
             "Вы действительно хотите выйти из программы?",
             "Завершение программы",
             MessageBoxButtons.YesNo,
             MessageBoxIcon.Warning
            );
            if (dialog == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (grid.CurrentCell.ColumnIndex >= 0)
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            }
            else
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress -= tb_KeyPress;
            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!Char.IsNumber(e.KeyChar) && (e.KeyChar != '-') && (e.KeyChar != ',')))
            {
                if ((e.KeyChar != (char)Keys.Back) || (e.KeyChar != (char)Keys.Delete))
                { e.Handled = true; }
            }

        }

        private void справкаToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Help.chm");
        }

        private void выходToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оРазработчикахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Студенты 346 группы, специальности 230115 Чернышов Дмитрий и Миннеханов Ильназ");
        }
    }
}

    

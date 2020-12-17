using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Windows.Forms;

namespace v1
{
    public partial class form4LeaderBoard : Form
    {
        private string userID;
        private List<string> unsorted_grouped = new List<string>();
        private List<string> sorted_grouped = new List<string>();
        public form4LeaderBoard(string userID)
        {
            InitializeComponent();
            this.userID = userID;
        }


        private void form6LeaderBoard_Load(object sender, EventArgs e)
        {
            cmbSort.SelectedIndex = 0;
            loadLeaderBoard_DSC();
        }

        private void loadLeaderBoard_ASC()
        {
            //Loading the listview with data from the database using Merge Sort in ASC order
            unsorted_grouped.Clear();
            sorted_grouped.Clear();
            List<string> temp = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT userID, userBalance from tblUser WHERE userType = 'Student'";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                temp.Add(dr[0].ToString() + "|" + Convert.ToDecimal(dr[1]));
            }
            dBConnector.close();
            foreach (var item in temp)
            {
                string userID = "";
                string total = "";
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i] == '|')
                    {
                        userID = item.Remove(i, item.Length - i);
                        total = item.Remove(0, i + 1);
                        i = item.Length;
                    }
                }
                sqlStr = "SELECT portfolioNetPrice * portfolioHolding FROM tblPortfolio WHERE userID = '" + userID + "'";
                dBConnector.Connect();
                dr = dBConnector.DoSQL(sqlStr);
                decimal portfolioTotal = 0;
                while (dr.Read())
                {
                    portfolioTotal = portfolioTotal + Convert.ToDecimal(dr[0]);
                }
                dBConnector.close();
                unsorted_grouped.Add(userID + "|" + (Convert.ToDecimal(total) + Convert.ToDecimal(portfolioTotal)));
            }
            sorted_grouped = AscMergeSort(unsorted_grouped);
            loadlsVLeaderBoard();
        }

        private List<string> AscMergeSort(List<string> unsorted_grouped)
        {
            //Recursion to split data into different lists
            if (unsorted_grouped.Count <= 1)
            {
                return unsorted_grouped;
            }
            List<string> left = new List<string>();
            List<string> right = new List<string>();

            int middle = unsorted_grouped.Count / 2;
            for (int i = 0; i < middle; i++)
            {
                left.Add(unsorted_grouped[i]);
            }
            for (int i = middle; i < unsorted_grouped.Count; i++)
            {
                right.Add(unsorted_grouped[i]);
            }
            left = AscMergeSort(left);
            right = AscMergeSort(right);
            return AscMerge(left, right);
        }

        private void loadLeaderBoard_DSC()
        {
            //Loading the listview with data from the database using Merge Sort in DSC order
            unsorted_grouped.Clear();
            sorted_grouped.Clear();
            List<string> temp = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT userID, userBalance from tblUser WHERE userType = 'Student'";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                temp.Add(dr[0].ToString() + "|" + Convert.ToDecimal(dr[1]));
            }
            dBConnector.close();
            foreach (var item in temp)
            {
                string userID = "";
                string total = "";
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i] == '|')
                    {
                        userID = item.Remove(i, item.Length - i);
                        total = item.Remove(0, i + 1);
                        i = item.Length;
                    }
                }
                sqlStr = "SELECT portfolioNetPrice * portfolioHolding FROM tblPortfolio WHERE userID = '" + userID + "'";
                dBConnector.Connect();
                dr = dBConnector.DoSQL(sqlStr);
                decimal portfolioTotal = 0;
                while (dr.Read())
                {
                    portfolioTotal = portfolioTotal + Convert.ToDecimal(dr[0]);
                }
                dBConnector.close();
                unsorted_grouped.Add(userID + "|" + (Convert.ToDecimal(total) + Convert.ToDecimal(portfolioTotal)));
            }
            sorted_grouped = DscMergeSort(unsorted_grouped);
            lsVLeaderBoard.Clear();
            loadlsVLeaderBoard();
        }

        private void loadlsVLeaderBoard()
        {
            lsVLeaderBoard.Clear();
            Initialize_lsVLeaderBoard();
            List<string> userID = new List<string>();
            List<string> StudentName = new List<string>();
            List<string> TotalAssets = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            foreach (var item in sorted_grouped)
            {
                //Disassemble the item in the list, ignore all chars after |
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i] == '|')
                    {
                        userID.Add(item.Remove(i, item.Length - i));
                        TotalAssets.Add(item.Remove(0, i + 1));
                        i = item.Length;
                    }
                }
            }
            foreach (var item in userID)
            {
                dBConnector.Connect();
                string sqlString = "SELECT studentName FROM tblStudent WHERE userID ='" + item + "'";
                dr = dBConnector.DoSQL(sqlString);
                while (dr.Read())
                {
                    StudentName.Add(dr[0].ToString());
                }
                dBConnector.close();
            }
            for (int i = 0; i < userID.Count; i++)
            {
                int x = userID.Count;
                //Ranking index starts from 1 if sort by DSC
                if (cmbSort.SelectedIndex == 0)
                {
                    ListViewItem lvi = new ListViewItem((i + 1).ToString());
                    lvi.SubItems.Add(userID[i]);
                    lvi.SubItems.Add(StudentName[i]);
                    lvi.SubItems.Add("$ " + Convert.ToDecimal(TotalAssets[i]).ToString("#,##0.##"));
                    lsVLeaderBoard.Items.Add(lvi); 
                }
                //Ranking index ends in 1 if sort by ASC
                else
                {
                    ListViewItem lvi = new ListViewItem((x - i).ToString());
                    lvi.SubItems.Add(userID[i]);
                    lvi.SubItems.Add(StudentName[i]);
                    lvi.SubItems.Add("$ " + Convert.ToDecimal(TotalAssets[i]).ToString("#,##0.##"));
                    lsVLeaderBoard.Items.Add(lvi);
                }
            }
        }

        private void Initialize_lsVLeaderBoard()
        {
            //Adding the columns and adjust settings
            lsVLeaderBoard.View = View.Details;
            lsVLeaderBoard.LabelEdit = true;
            lsVLeaderBoard.GridLines = true;
            lsVLeaderBoard.Columns.Add("", 20, HorizontalAlignment.Left);
            lsVLeaderBoard.Columns.Add("User ID", 80, HorizontalAlignment.Left);
            lsVLeaderBoard.Columns.Add("Student Name", 335 / 2, HorizontalAlignment.Left);
            lsVLeaderBoard.Columns.Add("Total Assets", 335 / 2, HorizontalAlignment.Left);
        }

        private List<string> DscMergeSort(List<string> unsorted_grouped)
        {
            //Recursion to split data into different lists
            if (unsorted_grouped.Count <= 1)
            {
                return unsorted_grouped;
            }
            List<string> left = new List<string>();
            List<string> right = new List<string>();

            int middle = unsorted_grouped.Count / 2;
            for (int i = 0; i < middle; i++)
            {
                left.Add(unsorted_grouped[i]);
            }
            for (int i = middle; i < unsorted_grouped.Count; i++)
            {
                right.Add(unsorted_grouped[i]);
            }
            left = DscMergeSort(left);
            right = DscMergeSort(right);
            return DscMerge(left, right);
        }

        private List<string> DscMerge(List<string> left, List<string> right)
        {
            //Merge Sort
            List<string> result = new List<string>();
            while (left.Count > 0 || right.Count > 0)
            {
                decimal _leftFirst = 0;
                decimal _rightFirst = 0;
                if (left.Count > 0)
                {
                    for (int i = 0; i < left.First().Length; i++)
                    {
                        //Disassemble the string, ignore any char before the |
                        if (left.First()[i] == '|')
                        {
                            _leftFirst = Convert.ToDecimal(left.First().Remove(0, i + 1));
                            i = left.First().Length;
                        }
                    }
                }
                if (right.Count > 0)
                {
                    for (int i = 0; i < right.First().Length; i++)
                    {
                        //Disassemble the string, ignore any char before the |
                        if (right.First()[i] == '|')
                        {
                            _rightFirst = Convert.ToDecimal(right.First().Remove(0, i + 1));
                            i = right.First().Length;
                        }
                    }
                }
                if (left.Count > 0 && right.Count > 0)
                {
                    if (_leftFirst >= _rightFirst)
                    {
                        result.Add(left.First());
                        left.Remove(left.First());
                    }
                    else
                    {
                        result.Add(right.First());
                        right.Remove(right.First());
                    }
                }
                else if (left.Count > 0)
                {
                    result.Add(left.First());
                    left.Remove(left.First());
                }
                else if (right.Count > 0)
                {
                    result.Add(right.First());
                    right.Remove(right.First());
                }
            }
            return result;
        }



        private List<string> AscMerge(List<string> left, List<string> right)
        {
            //Merge Sort
            List<string> result = new List<string>();
            while (left.Count > 0 || right.Count > 0)
            {
                decimal _leftFirst = 0;
                decimal _rightFirst = 0;
                if (left.Count > 0)
                {
                    for (int i = 0; i < left.First().Length; i++)
                    {
                        //Disassemble the string, ignore any char before the |
                        if (left.First()[i] == '|')
                        {
                            _leftFirst = Convert.ToDecimal(left.First().Remove(0, i + 1));
                            i = left.First().Length;
                        }
                    } 
                }
                if (right.Count > 0)
                {
                    for (int i = 0; i < right.First().Length; i++)
                    {
                        //Disassemble the string, ignore any char before the |
                        if (right.First()[i] == '|')
                        {
                            _rightFirst = Convert.ToDecimal(right.First().Remove(0, i + 1));
                            i = right.First().Length;
                        }
                    } 
                }
                if (left.Count > 0 && right.Count > 0)
                {
                    if (_leftFirst <= _rightFirst)
                    {
                        result.Add(left.First());
                        left.Remove(left.First());
                    }
                    else
                    {
                        result.Add(right.First());
                        right.Remove(right.First());
                    }
                }
                else if (left.Count > 0)
                {
                    result.Add(left.First());
                    left.Remove(left.First());
                }
                else if (right.Count > 0)
                {
                    result.Add(right.First());
                    right.Remove(right.First());
                }
            }
            return result;
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            //When sort by DSC is chosen
            if (cmbSort.SelectedIndex == 0)
            {
                loadLeaderBoard_DSC();
            }
            //When sort by ASC is chosen
            else
            {
                loadLeaderBoard_ASC();
            }
        }
    }
}
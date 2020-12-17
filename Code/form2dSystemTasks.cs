using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace v1
{
    public partial class form2dSystemTasks : Form
    {
        private string userID;
        private List<string> CourseStaffIDCreate = new List<string>();
        private List<string> CourseDepartmentIDCreate = new List<string>();
        private List<string> CourseDepartmentIDUpdate = new List<string>();
        private List<string> CourseLevelCreate = new List<string>();
        private List<string> ProgrammeStaffID = new List<string>();
        private List<string> ProgrammeID = new List<string>();
        private List<string> CourseID = new List<string>();


        public form2dSystemTasks(string userID)
        {
            InitializeComponent();
            this.userID = userID;
        }

        private void form2dSystemTasks_Load(object sender, EventArgs e)
        {
            //Load controls in default tab
            loadDefaultPanel();
        }

        private void loadDefaultPanel()
        {
            //Load Department Update
            pnlDepartment.Visible = true;
            pnlCourse.Visible = false;
            pnlProgramme.Visible = false;
            pnlCurriculum.Visible = false;
            pnlDepartmentHeadUpdate.Visible = true;
            pnlDeparmentCreate.Visible = false;
            lblDepartmentUpdate.Visible = true;
            lblDepartmentCreate.Visible = false;
            txtDepartmentHead.Clear();
            txtDepartmentID.Clear();
            txtDepartmentName.Clear();
            LoadlsVDepartment();
        }

        private void LoadlsVDepartment()
        {
            //Queue data from tblDepartment and load into lists
            InitializelsVDepartment();
            List<string> departmentID = new List<string>();
            List<string> departmentName = new List<string>();
            List<string> departmentHead = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT departmentID, departmentName, departmentHead FROM tblDepartment";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                departmentID.Add(dr[0].ToString());
                departmentName.Add(dr[1].ToString());
                departmentHead.Add(dr[2].ToString());
            }
            dBConnector.close();
            //Load data from lists into Department listview
            for (int i = 0; i < departmentID.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(departmentID[i]);
                lvi.SubItems.Add(departmentName[i]);
                lvi.SubItems.Add(departmentHead[i]);
                lsVDepartment.Items.Add(lvi);
            }
        }

        private void InitializelsVDepartment()
        {
            //Add columns and adjust settings
            lsVDepartment.Clear();
            lsVDepartment.View = View.Details;
            lsVDepartment.LabelEdit = true;
            lsVDepartment.GridLines = true;
            lsVDepartment.Columns.Add("Department ID", 85, HorizontalAlignment.Left);
            lsVDepartment.Columns.Add("Department Name", 110, HorizontalAlignment.Left);
            lsVDepartment.Columns.Add("Department Head", 110, HorizontalAlignment.Left);
        }

        private void lsVDepartment_ItemActivate(object sender, EventArgs e)
        {
            //Change text in relevant textboxes when new item selected in combo box
            txtDepartmentID.Text = lsVDepartment.SelectedItems[0].Text;
            txtDepartmentName.Text = lsVDepartment.SelectedItems[0].SubItems[1].Text;
            txtDepartmentHead.Text = lsVDepartment.SelectedItems[0].SubItems[2].Text;
        }

        private void updateDepartmentHeadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Only show relevant controls for update department
            pnlDepartment.Visible = true;
            pnlCourse.Visible = false;
            pnlProgramme.Visible = false;
            pnlCurriculum.Visible = false;
            pnlDepartmentHeadUpdate.Visible = true;
            pnlDeparmentCreate.Visible = false;
            lblDepartmentUpdate.Visible = true;
            lblDepartmentCreate.Visible = false;
            txtDepartmentHead.Clear();
            txtDepartmentID.Clear();
            txtDepartmentName.Clear();
            LoadlsVDepartment();
        }

        private void btnDepartmentUpdate_Click(object sender, EventArgs e)
        {
            //Validate the input
            if (txtDepartmentHead.Text == "")
            {
                MessageBox.Show("Department Head must not be left blank", "System Message");
            }
            else if (txtDepartmentName.Text == "")
            {
                MessageBox.Show("Department Name must not be left blank", "System Message");
            }
            else if (txtDepartmentHead.Text == getCurrentHead() && repetitionCheck())
            {
                MessageBox.Show("Department Head & Department Name cannot be the same", "System Message");
            }
            else if (txtDepartmentHead.Text != getCurrentHead() && txtDepartmentName.Text != getCurrentDepartmentName() && NameRepetitionWithOtherSubitems())
            {
                MessageBox.Show("Department Name entered already exists.", "System Message");
            }
            else
            {
                bool _1validated = departmentHeadValidation();
                bool _2validated = departmentNameValidation();
                if (!_1validated)
                {
                    MessageBox.Show("Please check that name entered does not contain illegal characters. A-Z, hyphens, commas and dots allowed", "System Message");
                }
                else if (!_2validated)
                {
                    MessageBox.Show("Please check that name entered does not contain illegal characters. A-Z, hyphens, commas and dots allowed", "System Message");
                }
                else
                {
                    //Update the relevant record when input validated
                    clsDBConnector dBConnector = new clsDBConnector();
                    string sqlString = "UPDATE tblDepartment SET departmentHead ='" + txtDepartmentHead.Text + "', departmentName ='" + txtDepartmentName.Text + "' WHERE departmentID = '" + txtDepartmentID.Text + "'";
                    dBConnector.Connect();
                    dBConnector.DoSQL(sqlString);
                    dBConnector.close();
                    LoadlsVDepartment();
                    MessageBox.Show("Success!", "System Message");
                }
            }
        }

        private bool NameRepetitionWithOtherSubitems()
        {
            //Check if input is same with any item in pdepartment list view
            bool result = false;
            for (int i = 0; i < lsVDepartment.Items.Count; i++)
            {
                foreach (ListViewItem.ListViewSubItem subItem in lsVDepartment.Items[i].SubItems)
                {
                    if (subItem.Text == txtDepartmentName.Text)
                    {
                        result = true;
                        break;
                    }
                }
                if (result) break;
            }
            return result;
        }

        private bool departmentNameValidation()
        {
            //Accpect names with hyphens , commas and dots. E.g: Martin Luther King, Jr. & Kaali-ah etc...
            bool validated = false;
            Regex regX = new Regex(@"^[a-zA-Z ,.-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection name = regX.Matches(txtDepartmentName.Text);
            if (name.Count.ToString() == "1")
            {
                validated = true;
            }
            return validated;
        }

        private bool repetitionCheck()
        {
            //check if input already exists in tblDeparment
            bool repeat = false;
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            int count = 0;
            string sqlString = "SELECT COUNT(*) FROM tblDepartment WHERE departmentName ='" + txtDepartmentName.Text + "'";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                count = Convert.ToInt32(dr[0]);
            }
            dBConnector.close();
            if (count > 0)
            {
                repeat = true;
            }
            return repeat;
        }

        private string getCurrentDepartmentName()
        {
            //get department name with user input(department ID)
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string departmentName = "";
            string sqlString = "SELECT departmentName FROM tblDepartment WHERE departmentID ='" + txtDepartmentID.Text + "'";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                departmentName = dr[0].ToString();
            }
            dBConnector.close();
            return departmentName;
        }

        private string getCurrentHead()
        {
            //get department head with user input(department ID)
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string departmentHead = "";
            string sqlString = "SELECT departmentHead FROM tblDepartment WHERE departmentID ='" + txtDepartmentID.Text + "'";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                departmentHead = dr[0].ToString();
            }
            dBConnector.close();
            return departmentHead;
        }

        private bool departmentHeadValidation()
        {
            //Accpect names with hyphens , commas and dots. E.g: Martin Luther King, Jr. & Kaali-ah etc...
            bool validated = false;
            Regex regX = new Regex(@"^[a-zA-Z ,.-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection name = regX.Matches(txtDepartmentHead.Text);
            if (name.Count.ToString() == "1")
            {
                validated = true;
            }
            return validated;
        }

        private void createNewDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Only show relevant controls for create new deparmtent
            pnlDepartment.Visible = true;
            pnlCourse.Visible = false;
            pnlProgramme.Visible = false;
            pnlCurriculum.Visible = false;
            pnlDepartmentHeadUpdate.Visible = false;
            pnlDeparmentCreate.Visible = true;
            lblDepartmentUpdate.Visible = false;
            lblDepartmentCreate.Visible = true;
            txtDepartmentHeadCreate.Clear();
            txtDepartmentIDCreate.Clear();
            txtDepartmentNameCreate.Clear();
            LoadlsVDepartment();
        }

        private void btnDepartmentCreate_Click(object sender, EventArgs e)
        {
            //Validate the input
            if (txtDepartmentHeadCreate.Text == "" || txtDepartmentNameCreate.Text == "" || txtDepartmentIDCreate.Text == "")
            {
                MessageBox.Show("All details must not be left blank", "System Message");
            }
            else
            {
                List<string> departmentID = new List<string>();
                List<string> departmentName = new List<string>();
                clsDBConnector dBConnector = new clsDBConnector();
                OleDbDataReader dr;
                string sqlStr = "SELECT departmentID, departmentName, departmentHead FROM tblDepartment";
                dBConnector.Connect();
                dr = dBConnector.DoSQL(sqlStr);
                while (dr.Read())
                {
                    departmentID.Add(dr[0].ToString());
                    departmentName.Add(dr[1].ToString());
                }
                dBConnector.close();
                bool repeat = false;
                for (int i = 0; i < departmentID.Count; i++)
                {
                    if (txtDepartmentIDCreate.Text == departmentID[i] || txtDepartmentNameCreate.Text == departmentName[i])
                    {
                        repeat = true;
                        i = departmentID.Count;
                    }
                }
                if (repeat)
                {
                    MessageBox.Show("No duplications of Department ID/Name allowed", "System Message");
                }
                else
                {
                    bool ID = false, Name = false, Head = false;
                    CreateDetailsValidation(ref ID, ref Name, ref Head);
                    string errorMsg = "";
                    if (!ID)
                    {
                        errorMsg = "Department ID can only contain 1-10 alphabet characters.";
                    }
                    if (!Name)
                    {
                        if (errorMsg.Length == 0)
                        {
                            errorMsg = "Department Name can only contain 1-30 alphabet characters and whitespaces.";
                        }
                        else
                        {
                            errorMsg = errorMsg + "\n\nDepartment Name can only contain 1-30 alphabet characters and whitespaces.";
                        }
                    }
                    if (!Head)
                    {
                        if (errorMsg.Length == 0)
                        {
                            errorMsg = "Please check that name entered does not contain illegal characters. A-Z, hyphens, commas and dots allowed";
                        }
                        else
                        {
                            errorMsg = errorMsg + "\n\nPlease check that name entered does not contrain illegal characters. A - Z, hyphens, commas and dots allowed";
                        }
                    }
                    if (!ID || !Name || !Head)
                    {
                        MessageBox.Show(errorMsg, "System Message");
                    }
                    else
                    {
                        //insert new record in tblDepartment when validation checks out
                        sqlStr = "INSERT INTO tblDepartment VALUES('" + txtDepartmentIDCreate.Text.ToUpper() + "', '" + txtDepartmentNameCreate.Text + "', '" + txtDepartmentHeadCreate.Text + "')";
                        dBConnector.Connect();
                        dBConnector.DoSQL(sqlStr);
                        dBConnector.close();
                        LoadlsVDepartment();
                        MessageBox.Show("Success!", "System Message");
                    }
                }
            }
        }

        private void CreateDetailsValidation(ref bool iD, ref bool name, ref bool Head)
        {
            //1-10 a-zA-Z
            Regex regX = new Regex(@"[a-zA-Z]{1,10}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection _1match = regX.Matches(txtDepartmentIDCreate.Text);
            if (_1match.Count.ToString() == "1")
            {
                iD = true;
            }
            //1-30 a-zA-Z and spaces. Cannot start or end with spaces
            regX = new Regex(@"^[a-zA-Z]*[a-zA-Z+ ?]*[a-zA-Z]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection _2match = regX.Matches(txtDepartmentNameCreate.Text);
            if (_2match.Count.ToString() == "1")
            {
                if (txtDepartmentNameCreate.Text.Length < 31)
                {
                    name = true;
                }
            }
            //Accpect names with hyphens , commas and dots. E.g: Martin Luther King, Jr. & Kaali-ah etc...
            regX = new Regex(@"^[a-zA-Z ,.-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection _3match = regX.Matches(txtDepartmentHeadCreate.Text);
            if (_3match.Count.ToString() == "1")
            {
                Head = true;
            }
        }

        private void updateCourseStaffIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Only show relevant controls for update course
            pnlDepartment.Visible = false;
            pnlCourse.Visible = true;
            pnlProgramme.Visible = false;
            pnlCurriculum.Visible = false;
            pnlCourseUpdate.Visible = true;
            pnlCourseCreate.Visible = false;
            lblCourseUpdate.Visible = true;
            lblCourseCreate.Visible = false;
            cmbCourseDepartmentIDUpdate.Items.Clear();
            txtCourseIDUpdate.Clear();
            txtCourseLevelUpdate.Clear();
            txtCourseNameUpdate.Clear();
            loadlsVCourse();
            populate_cmbCourseStaffIDUpdate();
            populate_cmbCourseDepartmentIDUpdate();
        }

        private void populate_cmbCourseDepartmentIDUpdate()
        {
            //get all records' departmentID and name from tblDepartment then load them into the combo box
            CourseDepartmentIDUpdate.Clear();
            cmbCourseDepartmentIDUpdate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT departmentID, departmentName FROM tblDepartment";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbCourseDepartmentIDUpdate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                CourseDepartmentIDUpdate.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private void populate_cmbCourseStaffIDUpdate()
        {
            //get all records' staffID and name from tblStaff then load them into the combo box
            CourseStaffIDCreate.Clear();
            cmbCourseStaffIDUpdate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT staffID, staffName FROM tblStaff";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbCourseStaffIDUpdate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                CourseStaffIDCreate.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private void loadlsVCourse()
        {
            //Queue data from tblCourse and load into lists
            InitializelsVCourse();
            List<string> courseID = new List<string>();
            List<string> courseName = new List<string>();
            List<string> courseLevel = new List<string>();
            List<string> departmentID = new List<string>();
            List<string> staffID = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT courseID, courseName, courseLevel, departmentID, staffID FROM tblCourse";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                courseID.Add(dr[0].ToString());
                courseName.Add(dr[1].ToString());
                courseLevel.Add(dr[2].ToString());
                departmentID.Add(dr[3].ToString());
                staffID.Add(dr[4].ToString());
            }
            dBConnector.close();
            //load data from the lists into the course listview
            for (int i = 0; i < courseID.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(courseID[i]);
                lvi.SubItems.Add(courseName[i]);
                lvi.SubItems.Add(courseLevel[i]);
                lvi.SubItems.Add(departmentID[i]);
                lvi.SubItems.Add(staffID[i]);
                lsVCourse.Items.Add(lvi);
            }
        }

        private void InitializelsVCourse()
        {
            //add columns and adjust settings
            lsVCourse.Clear();
            lsVCourse.View = View.Details;
            lsVCourse.LabelEdit = true;
            lsVCourse.GridLines = true;
            lsVCourse.Columns.Add("Course ID", 65, HorizontalAlignment.Left);
            lsVCourse.Columns.Add("Course Name", 85, HorizontalAlignment.Left);
            lsVCourse.Columns.Add("Course Level", 80, HorizontalAlignment.Left);
            lsVCourse.Columns.Add("Department ID", 85, HorizontalAlignment.Left);
            lsVCourse.Columns.Add("Staff ID", 60, HorizontalAlignment.Left);
        }

        private void createNewCourseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Only show relevant controls for create new course
            pnlDepartment.Visible = false;
            pnlCourse.Visible = true;
            pnlProgramme.Visible = false;
            pnlCurriculum.Visible = false;
            pnlCourseUpdate.Visible = false;
            pnlCourseCreate.Visible = true;
            lblCourseUpdate.Visible = false;
            lblCourseCreate.Visible = true;
            txtCourseIDCreate.Clear();
            txtCourseNameCreate.Clear();
            txtCourseIDCreate.Text = calculateNewCourseID();
            loadlsVCourse();
            populate_cmbCourseLevelCreate();
            populate_cmbCourseStaffIDCreate();
            populate_cmbCourseDepartmentIDCreate();
        }

        private void populate_cmbCourseDepartmentIDCreate()
        {
            //get all records' departmentID and name from tblDepartment then load them into the combo box
            CourseDepartmentIDCreate.Clear();
            cmbCourseDepartmentIDCreate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT departmentID, departmentName FROM tblDepartment";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbCourseDepartmentIDCreate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                CourseDepartmentIDCreate.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private void populate_cmbCourseLevelCreate()
        {
            //get all records' courseLevel from tblCourse then load them into the combo box
            CourseLevelCreate.Clear();
            cmbCourseLevelCreate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT courseLevel FROM tblCourse";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                bool duplicate = false;
                foreach (var item in CourseLevelCreate)
                {
                    if (dr[0].ToString() == item)
                    {
                        duplicate = true;
                    }
                }
                if (!duplicate)
                {
                    cmbCourseLevelCreate.Items.Add(dr[0].ToString());
                    CourseLevelCreate.Add(dr[0].ToString());
                }
            }
            dBConnector.close();
        }

        private void populate_cmbCourseStaffIDCreate()
        {
            //get all records' staffID and name from tblStaf then load them into the combo box
            CourseStaffIDCreate.Clear();
            cmbCourseStaffIDCreate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT staffID, staffName FROM tblStaff";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbCourseStaffIDCreate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                CourseStaffIDCreate.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private string calculateNewCourseID()
        {
            //Calculatin the next ID (incrementing the last record by 1) in the correct format (C000)
            string newID = "";
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            dBConnector.Connect();
            string sqlStr = "SELECT COUNT(*) FROM tblCourse";
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                int ID = Convert.ToInt32(dr[0].ToString()) + 1;
                string template = "C000";
                newID = template.Remove(template.Length - ID.ToString().Length) + ID;
            }
            dBConnector.close();
            return newID;
        }

        private void lsVCourse_ItemActivate(object sender, EventArgs e)
        {
            //When item in list view selected, populate textboxes with selected item's data
            txtCourseIDUpdate.Text = lsVCourse.SelectedItems[0].Text;
            txtCourseNameUpdate.Text = lsVCourse.SelectedItems[0].SubItems[1].Text;
            txtCourseLevelUpdate.Text = lsVCourse.SelectedItems[0].SubItems[2].Text;
            foreach (var item in cmbCourseDepartmentIDUpdate.Items)
            {
                string temp = "";
                for (int i = 0; i < item.ToString().Length; i++)
                {
                    if (item.ToString()[i] != ' ' && item.ToString()[i+1] != '|')
                    {
                        temp = temp + item.ToString()[i];
                    }
                    else
                    {
                        i = item.ToString().Length;
                    }
                }
                if (temp == lsVCourse.SelectedItems[0].SubItems[3].Text)
                {
                    cmbCourseDepartmentIDUpdate.SelectedItem = item;
                }
            }
            foreach (var item in cmbCourseStaffIDUpdate.Items)
            {
                string temp = "";
                for (int i = 0; i < item.ToString().Length; i++)
                {
                    if (item.ToString()[i] != ' ' && item.ToString()[i + 1] != '|')
                    {
                        temp = temp + item.ToString()[i];
                    }
                    else
                    {
                        i = item.ToString().Length;
                    }
                }
                if (temp == lsVCourse.SelectedItems[0].SubItems[4].Text)
                {
                    cmbCourseStaffIDUpdate.SelectedItem = item;
                }
            }
        }

        private void btnCourseUpdate_Click(object sender, EventArgs e)
        {
            //Validate the user's input, if not correct show error messages
            bool NameValidation = generalValidation(txtCourseNameUpdate.Text), LevelValidation = generalValidation(txtCourseLevelUpdate.Text);
            if (txtCourseNameUpdate.Text == "")
            {
                MessageBox.Show("Course Name must not be empty", "System Message");
            }
            else if (txtCourseLevelUpdate.Text == "")
            {
                MessageBox.Show("Course Level must not be empty", "System Message");
            }
            else if (!NameValidation)
            {
                MessageBox.Show("Please check that Course Name entered does not contain illegal characters. A-Z, hyphens, commas and dots allowed");
            }
            else if (!LevelValidation)
            {
                MessageBox.Show("Please check that Course Level entered does not contain illegal characters. A-Z, hyphens, commas and dots allowed");
            }
            else
            {
                //See if input would create replicate
                clsDBConnector dBConnector = new clsDBConnector();
                OleDbDataReader dr;
                string sqlString = "SELECT COUNT(*) FROM tblCourse WHERE staffID ='" + CourseStaffIDCreate[cmbCourseStaffIDUpdate.SelectedIndex] + "' AND departmentID='" + CourseDepartmentIDUpdate[cmbCourseDepartmentIDUpdate.SelectedIndex] + "' AND courseLevel ='" + txtCourseLevelUpdate.Text + "' AND courseName ='" + txtCourseNameUpdate.Text + "'";
                int count = 0;
                dBConnector.Connect();
                dr = dBConnector.DoSQL(sqlString);
                while (dr.Read())
                {
                    count = Convert.ToInt32(dr[0]);
                }
                dBConnector.close();
                if (count > 0)
                {
                    MessageBox.Show("A record with the same Course Name, Course Level, Course Department and Staff ID already exists.", "System Message");
                }
                else
                {
                    //Update record in database when all validations check out
                    sqlString = "UPDATE tblCourse SET staffID ='" + CourseStaffIDCreate[cmbCourseStaffIDUpdate.SelectedIndex] + "', departmentID='" + CourseDepartmentIDUpdate[cmbCourseDepartmentIDUpdate.SelectedIndex] + "', courseLevel ='" + txtCourseLevelUpdate.Text + "', courseName ='" + txtCourseNameUpdate.Text + "' WHERE courseID= '" + txtCourseIDUpdate.Text + "'";
                    dBConnector.Connect();
                    dBConnector.DoSQL(sqlString);
                    dBConnector.close();
                    loadlsVCourse();
                    MessageBox.Show("Success!", "System Message");
                }
            }
        }

        private bool generalValidation(string text)
        {
            bool validated = false;
            //Accpect names with hyphens , commas and dots. E.g: Martin Luther King, Jr. & Kaali-ah etc...
            Regex regX = new Regex(@"^[a-zA-Z ,.-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection name = regX.Matches(text);
            if (name.Count.ToString() == "1")
            {
                validated = true;
            }
            return validated;
        }

        private void btnCourseCreate_Click(object sender, EventArgs e)
        {
            //Validatet the user's input
            if (txtCourseIDCreate.Text == "" || txtCourseNameCreate.Text == "" || cmbCourseDepartmentIDCreate.SelectedItem == null || cmbCourseLevelCreate.SelectedItem == null || cmbCourseStaffIDCreate.SelectedItem == null)
            {
                MessageBox.Show("All details must not be left blank", "System Message");
            }
            else
            {
                //Check for duplications in courseID & courseName
                int count = 0;
                clsDBConnector dBConnector = new clsDBConnector();
                OleDbDataReader dr;
                string sqlStr = "SELECT COUNT(*) FROM tblCourse WHERE courseName ='" + txtCourseNameCreate.Text + "' AND courseLevel = '" + cmbCourseLevelCreate.SelectedItem.ToString() + "'";
                dBConnector.Connect();
                dr = dBConnector.DoSQL(sqlStr);
                while (dr.Read())
                {
                    count = Convert.ToInt32(dr[0]);
                }
                dBConnector.close();
                if (count > 0)
                {
                    MessageBox.Show(cmbCourseLevelCreate.SelectedItem.ToString() + " " + txtCourseNameCreate.Text + " already exist", "System Message");
                }
                    else
                    {
                        bool Name = false;
                        CourseCreateDetailsValidation(ref Name);
                        if (!Name)
                        {
                            MessageBox.Show("Please check that Course Name entered does not cont    ain illegal characters. A - Z, hyphens, commas and dots allowed", "System Message");
                        }
                        else
                        {
                            //insert record into database if all validations check out 
                            sqlStr = "INSERT INTO tblCourse VALUES('" + txtCourseIDCreate.Text + "', '" + txtCourseNameCreate.Text + "', '" + cmbCourseLevelCreate.SelectedItem.ToString() + "', '"+ CourseDepartmentIDCreate[cmbCourseDepartmentIDCreate.SelectedIndex] + "', '" + CourseStaffIDCreate[cmbCourseStaffIDCreate.SelectedIndex] + "')";
                            dBConnector.Connect();
                            dBConnector.DoSQL(sqlStr);
                            dBConnector.close();
                            loadlsVCourse();
                            DialogResult result = MessageBox.Show("Success!", "System Message", MessageBoxButtons.OK);
                            if (result == DialogResult.OK)
                            {
                                txtCourseIDCreate.Text = calculateNewCourseID();
                                txtCourseNameCreate.Clear();
                                cmbCourseLevelCreate.SelectedIndex = -1;
                                cmbCourseDepartmentIDCreate.SelectedIndex = -1;
                                cmbCourseStaffIDCreate.SelectedIndex = -1;
                            }
                        } 
                    }
                }
            }

        private void CourseCreateDetailsValidation(ref bool name)
        {
            //Accpect names with hyphens , commas and dots.
            Regex regX = new Regex(@"^[a-zA-Z ,.-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection _match = regX.Matches(txtCourseNameCreate.Text);
            if (_match.Count.ToString() == "1")
            {
                name = true;
            }
        }

        private void updateStaffIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //only show the relevant controls for update staff
            pnlDepartment.Visible = false;
            pnlCourse.Visible = false;
            pnlProgramme.Visible = true;
            pnlCurriculum.Visible = false;
            pnlProgrammeUpdate.Visible = true;
            pnlProgrammeCreate.Visible = false;
            lblProgrammeUpdate.Visible = true;
            lblProgammeCreate.Visible = false;
            txtProgrammeIDUpdate.Clear();
            txtProgrammeNameUpdate.Clear();
            LoadlsVProgramme();
            populate_cmbProgrammeStaffIDUpdate();
        }

        private void populate_cmbProgrammeStaffIDUpdate()
        {
            //get all records' staffID and name from tblStaff then load them into the combo box
            ProgrammeStaffID.Clear();
            cmbProgrammeStaffIDUpdate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT staffID, staffName FROM tblStaff";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbProgrammeStaffIDUpdate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                ProgrammeStaffID.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private void LoadlsVProgramme()
        {
            //Queue data from tblProgramme and load into lists
            InitializelsVProgramme();
            List<string> ProgrammeID = new List<string>();
            List<string> ProgrammeName = new List<string>();
            List<string> staffID = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT programmeID, programmeName, staffID FROM tblProgramme";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                ProgrammeID.Add(dr[0].ToString());
                ProgrammeName.Add(dr[1].ToString());
                staffID.Add(dr[2].ToString());
            }
            dBConnector.close();
            //load data from the lists into the programme listview
            for (int i = 0; i < ProgrammeID.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(ProgrammeID[i]);
                lvi.SubItems.Add(ProgrammeName[i]);
                lvi.SubItems.Add(staffID[i]);
                lsVProgramme.Items.Add(lvi);
            }
        }

        private void InitializelsVProgramme()
        {
            //add columns and adjust settings
            lsVProgramme.Clear();
            lsVProgramme.View = View.Details;
            lsVProgramme.LabelEdit = true;
            lsVProgramme.GridLines = true;
            lsVProgramme.Columns.Add("Programme ID", 82, HorizontalAlignment.Left);
            lsVProgramme.Columns.Add("Programme Name", 100, HorizontalAlignment.Left);
            lsVProgramme.Columns.Add("Staff ID", 60, HorizontalAlignment.Left);
        }

        private void lsVProgramme_ItemActivate(object sender, EventArgs e)
        {
            //When item in list view selected, populate textboxes with selected item's data
            txtProgrammeIDUpdate.Text = lsVProgramme.SelectedItems[0].Text;
            txtProgrammeNameUpdate.Text = lsVProgramme.SelectedItems[0].SubItems[1].Text;
            foreach (var item in cmbProgrammeStaffIDUpdate.Items)
            {
                string temp = "";
                for (int i = 0; i < item.ToString().Length; i++)
                {
                    if (item.ToString()[i] != ' ' && item.ToString()[i + 1] != '|')
                    {
                        temp = temp + item.ToString()[i];
                    }
                    else
                    {
                        i = item.ToString().Length;
                    }
                }
                if (temp == lsVProgramme.SelectedItems[0].SubItems[2].Text)
                {
                    cmbProgrammeStaffIDUpdate.SelectedItem = item;
                }
            }
        }

        private void btnProgammeUpdate_Click(object sender, EventArgs e)
        {
            //Validate the user's input
            bool programmeNameValidation = generalValidation(txtProgrammeNameUpdate.Text);
            if (txtProgrammeNameUpdate.Text == "")
            {
                MessageBox.Show("Programme Name must not be left blank.", "System Message");
            }
            else if (!programmeNameValidation)
            {
                MessageBox.Show("Please check that Programme Name entered does not contain illegal characters. A-Z, hyphens, commas and dots allowed");
            }
            else
            {
                clsDBConnector dBConnector = new clsDBConnector();
                OleDbDataReader dr;
                int count = 0;
                string sqlString = "SELECT COUNT(*) FROM tblProgramme WHERE programmeName = '" + txtProgrammeNameUpdate.Text + "'";
                dBConnector.Connect();
                dr = dBConnector.DoSQL(sqlString);
                while (dr.Read())
                {
                    count = Convert.ToInt32(dr[0]);
                }
                dBConnector.close();
                if (count == 1)
                {
                    sqlString = "SELECT programmeID FROM tblProgramme WHERE programmeName = '" + txtProgrammeNameUpdate.Text + "'";
                    dBConnector.Connect();
                    dr = dBConnector.DoSQL(sqlString);
                    string temp = "";
                    while (dr.Read())
                    {
                        temp = dr[0].ToString();
                    }
                    dBConnector.close();
                    if (temp == txtProgrammeIDUpdate.Text)
                    {
                        sqlString = "UPDATE tblProgramme SET staffID = '" + ProgrammeStaffID[cmbProgrammeStaffIDUpdate.SelectedIndex] + "', programmeName ='" + txtProgrammeNameUpdate.Text + "' WHERE programmeID = '" + txtProgrammeIDUpdate.Text + "'";
                        dBConnector.Connect();
                        dBConnector.DoSQL(sqlString);
                        dBConnector.close();
                        LoadlsVProgramme();
                        MessageBox.Show("Success!", "System Message");
                    }
                    else
                    {
                        MessageBox.Show("A record with the same Programme Name already exists.", "System Message");
                    }
                }
                else
                {
                    //Update the record in database if all validations check out
                    sqlString = "UPDATE tblProgramme SET staffID = '" + ProgrammeStaffID[cmbProgrammeStaffIDUpdate.SelectedIndex] + "', programmeName ='" + txtProgrammeNameUpdate.Text + "' WHERE programmeID = '" + txtProgrammeIDUpdate.Text + "'";
                    dBConnector.Connect();
                    dBConnector.DoSQL(sqlString);
                    dBConnector.close();
                    LoadlsVProgramme();
                    MessageBox.Show("Success!", "System Message");
                }
            }
        }

        private void createNewProgrammeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //only show the relevant controls for create new programme
            pnlDepartment.Visible = false;
            pnlCourse.Visible = false;
            pnlProgramme.Visible = true;
            pnlCurriculum.Visible = false;
            pnlProgrammeUpdate.Visible = false;
            pnlProgrammeCreate.Visible = true;
            lblProgrammeUpdate.Visible = false;
            lblProgammeCreate.Visible = true;
            LoadlsVProgramme();
            populate_cmbProgrammeStaffIDCreate();
        }

        private void populate_cmbProgrammeStaffIDCreate()
        {
            //get all records' staffID and name from tblStaff then load them into the combo box
            ProgrammeStaffID.Clear();
            cmbProgrammeStaffIDCreate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT staffID, staffName FROM tblStaff";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbProgrammeStaffIDCreate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                ProgrammeStaffID.Add(dr[0].ToString());
            }
            dBConnector.close();
            cmbProgrammeStaffIDCreate.SelectedIndex = 0;
        }

        private void btnProgrammeCreate_Click(object sender, EventArgs e)
        {
            //Validate the user's input
            if (txtProgrammeIDCreate.Text == "" || txtProgrammeNameCreate.Text == "" || cmbProgrammeStaffIDCreate.SelectedItem == null)
            {
                MessageBox.Show("All details must not be left blank", "System Message");
            }
            else
            {
                bool programmeID = false, programmeName = false;
                programmeCreateValidation(ref programmeID, ref programmeName);
                if (!programmeID || !programmeName)
                {
                    MessageBox.Show("Programme ID can only contain 1-10 letters.\n\nProgramme Name can only contain 1-255 letters.\n\nPlease check that details entered are in correct format", "System Message");
                }
                else
                {
                    bool ID_duplication = false, Name_duplication = false;
                    programmeCreateDuplicationCheck(ref ID_duplication, ref Name_duplication);
                    if (ID_duplication || Name_duplication)
                    {
                        MessageBox.Show("No duplications allowed. Please check that Programme ID & Programme Name entered are unique");
                    }
                    else
                    {
                        //Insert a new record in the database if all validations check out
                        clsDBConnector dBConnector = new clsDBConnector();
                        string sqlStr = "INSERT INTO tblProgramme VALUES('" + txtProgrammeIDCreate.Text.ToUpper() + "', '" + txtProgrammeNameCreate.Text + "', '" + ProgrammeStaffID[cmbProgrammeStaffIDCreate.SelectedIndex] + "')";
                        dBConnector.Connect();
                        dBConnector.DoSQL(sqlStr);
                        dBConnector.close();
                        LoadlsVProgramme();
                        DialogResult result = MessageBox.Show("Success!", "System Message", MessageBoxButtons.OK);
                        if (result == DialogResult.OK)
                        {
                            populate_cmbProgrammeStaffIDCreate();
                            txtProgrammeIDCreate.Clear();
                            txtProgrammeNameCreate.Clear();
                        }
                    }
                }
            }
        }

        private void programmeCreateDuplicationCheck(ref bool iD_duplication, ref bool name_duplication)
        {
            //See if the input already exists in the database
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT COUNT(*) FROM tblProgramme WHERE programmeID ='" + txtProgrammeIDCreate.Text + "'";
            int count = 0;
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                count = Convert.ToInt32(dr[0]);
            }
            dBConnector.close();
            if (count > 0)
            {
                iD_duplication = true;
            }
            sqlStr = "SELECT COUNT(*) FROM tblProgramme WHERE programmeName = '" + txtProgrammeNameCreate.Text + "'";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                count = Convert.ToInt32(dr[0]);
            }
            dBConnector.close();
            if (count > 0)
            {
                name_duplication = true;
            }

        }
        private void programmeCreateValidation(ref bool programmeID, ref bool programmeName)
        {
            //1-10 a-zA-Z
            Regex regX = new Regex(@"[a-zA-Z]{1,10}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection _1match = regX.Matches(txtProgrammeIDCreate.Text);
            if (_1match.Count.ToString() == "1")
            {
                programmeID = true;
            }
            //lower- and upper-case letters, integers, underscore and whitespaces
            regX = new Regex(@"^[a-zA-Z0-9_ ]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection _2match = regX.Matches(txtProgrammeNameCreate.Text);
            if (_2match.Count.ToString() == "1")
            {
                if (txtProgrammeNameCreate.Text.Length <= 255)
                {
                    programmeName = true;
                }
            }
        }

        private void createCurriculumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opnly shot the relevant controls in create curriculum
            pnlDepartment.Visible = false;
            pnlCourse.Visible = false;
            pnlProgramme.Visible = false;
            pnlCurriculum.Visible = true;
            pnlCurriculumCreate.Visible = true;
            lblCurriculumCreate.Visible = true;
            LoadlsVCurriculum();
            populate_cmbCurriculumProgrammeID();
            populate_cmbCurriculumCourseID();
            populate_cmbCurriculumSchoolTerm();
        }

        private void populate_cmbCurriculumProgrammeID()
        {
            //get all records' programmeID and name from tblProgramme then load them into the combo box
            ProgrammeID.Clear();
            cmbCurriculumProgrammeIDCreate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT programmeID, programmeName FROM tblProgramme";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbCurriculumProgrammeIDCreate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString());
                ProgrammeID.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private void populate_cmbCurriculumCourseID()
        {
            //get all records' courseID, name and level from tblCourse then load them into the combo box
            CourseID.Clear();
            cmbCurriculumCourseIDCreate.Items.Clear();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlString = "SELECT courseID, courseName, courseLevel FROM tblCourse";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlString);
            while (dr.Read())
            {
                cmbCurriculumCourseIDCreate.Items.Add(dr[0].ToString() + " | " + dr[1].ToString() + " | " + dr[2].ToString());
                CourseID.Add(dr[0].ToString());
            }
            dBConnector.close();
        }

        private void populate_cmbCurriculumSchoolTerm()
        {
            //load 'Spring', 'Summer', 'Autumn' into the combo box as items
            cmbCurriculumSchoolTermCreate.Items.Clear();
            cmbCurriculumSchoolTermCreate.Items.Add("Spring");
            cmbCurriculumSchoolTermCreate.Items.Add("Summer");
            cmbCurriculumSchoolTermCreate.Items.Add("Autumn");
            cmbCurriculumSchoolTermCreate.SelectedIndex = 1;
        }

        private void LoadlsVCurriculum()
        {
            //Queue data from tblCurriculum and load into lists
            InitializelsVCurriculum();
            List<string> ProgrammeID = new List<string>();
            List<string> courseID = new List<string>();
            List<string> schoolTerm = new List<string>();
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT programmeID, courseID, schoolTerm FROM tblCurriculum";
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                ProgrammeID.Add(dr[0].ToString());
                courseID.Add(dr[1].ToString());
                schoolTerm.Add(dr[2].ToString());
            }
            dBConnector.close();
            //load data from the lists into the curriculum listview
            for (int i = 0; i < ProgrammeID.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(ProgrammeID[i]);
                lvi.SubItems.Add(courseID[i]);
                lvi.SubItems.Add(schoolTerm[i]);
                lsVCurriculum.Items.Add(lvi);
            }
        }

        private void InitializelsVCurriculum()
        {
            //add columns and adjust settings
            lsVCurriculum.Clear();
            lsVCurriculum.View = View.Details;
            lsVCurriculum.LabelEdit = true;
            lsVCurriculum.GridLines = true;
            lsVCurriculum.Columns.Add("Programme ID", 82, HorizontalAlignment.Left);
            lsVCurriculum.Columns.Add("Course ID", 75, HorizontalAlignment.Left);
            lsVCurriculum.Columns.Add("School Term", 78, HorizontalAlignment.Left);
        }

        private void btnCurriculumCreate_Click(object sender, EventArgs e)
        {
            //validate the selected items, make sure no duplications
            if (cmbCurriculumCourseIDCreate.SelectedItem == null || cmbCurriculumProgrammeIDCreate.SelectedItem == null)
            {
                MessageBox.Show("All details must not be left blank", "System Message");
            }
            else 
            {
                bool duplication = false;
                CurriculumDuplicationCheck(ref duplication);
                if (duplication)
                {
                    MessageBox.Show("Record already exist.", "System Message");
                }
                else
                {
                    //insert new reocrd into database if all validations check out
                    clsDBConnector dBConnector = new clsDBConnector();
                    string sqlStr = "INSERT INTO tblCurriculum VALUES('" + ProgrammeID[cmbCurriculumProgrammeIDCreate.SelectedIndex] + "', '" + CourseID[cmbCurriculumCourseIDCreate.SelectedIndex] + "', '" + cmbCurriculumSchoolTermCreate.SelectedItem.ToString() +"')";
                    dBConnector.Connect();
                    dBConnector.DoSQL(sqlStr);
                    dBConnector.close();
                    LoadlsVCurriculum();
                    DialogResult result = MessageBox.Show("Success!", "System Message", MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                    {
                        populate_cmbCurriculumCourseID();
                        populate_cmbCurriculumProgrammeID();
                        populate_cmbCurriculumSchoolTerm();
                    }
                }
            }
        }

        private void CurriculumDuplicationCheck(ref bool duplication)
        {
            //check if records with selected items already exists in the database
            clsDBConnector dBConnector = new clsDBConnector();
            OleDbDataReader dr;
            string sqlStr = "SELECT COUNT(*) FROM tblCurriculum WHERE programmeID ='" + ProgrammeID[cmbCurriculumProgrammeIDCreate.SelectedIndex] + "' AND courseID = '" + CourseID[cmbCurriculumCourseIDCreate.SelectedIndex] + "'";
            int count = 0;
            dBConnector.Connect();
            dr = dBConnector.DoSQL(sqlStr);
            while (dr.Read())
            {
                count = Convert.ToInt32(dr[0]);
            }
            dBConnector.close();
            if (count > 0)
            {
                duplication = true;
            }
        }
    }
}
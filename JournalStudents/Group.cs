using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;

namespace JournalStudents
{
    public partial class Group : Form
    {
        DataSet dataSet;
        int groupID;
        DataRow rowGroup;
        SqlDataAdapter adapterGroups;
        SqlDataAdapter adapterSubjects;
        SqlDataAdapter adapterStudents;
        SqlDataAdapter adapterPeople;
        SqlDataAdapter adapterImages;
        SqlDataAdapter adapterEvaluations;

        public Group(DataSet dataSet, int groupID, SqlDataAdapter adapterGroups, SqlDataAdapter adapterSubjects, SqlDataAdapter adapterStudents, SqlDataAdapter adapterPeople, SqlDataAdapter adapterImages, SqlDataAdapter adapterEvaluations)
        {
            InitializeComponent();
            this.dataSet = dataSet;
            this.groupID = groupID;
            this.adapterGroups = adapterGroups;
            this.adapterSubjects = adapterSubjects;
            this.adapterStudents = adapterStudents;
            this.adapterPeople = adapterPeople;
            this.adapterImages = adapterImages;
            this.adapterEvaluations = adapterEvaluations;

            rowGroup = dataSet.Tables["Groups"].Select("ID=" + groupID)[0];
            textBoxGroupName.Text = rowGroup["Name"].ToString();
            dateTimePickerAdmission.Value = (DateTime)rowGroup["Admission"];

            FillDateGridViewStusents();

            FillListBoxSubject();
        }

        private void FillListBoxSubject()
        {
            var querySubjects = from lesson in dataSet.Tables["Lessons"].AsEnumerable()
                                where lesson["GroupID"].Equals(groupID)
                                join subject in dataSet.Tables["Subjects"].AsEnumerable()
                                on lesson["SubjectID"] equals subject["ID"]
                                select subject;

            listBoxSubjects.ValueMember = "ID";
            listBoxSubjects.DisplayMember = "Name";
            if (querySubjects.Count() > 0)
                listBoxSubjects.DataSource = querySubjects.CopyToDataTable();
        }

        private void FillDateGridViewStusents()
        {
            var queryPeople = from student in dataSet.Tables["Students"].AsEnumerable()
                              where student["GroupID"].Equals(groupID)
                              join person in dataSet.Tables["People"].AsEnumerable()
                              on student["PeopleID"] equals person["ID"]
                              select person;
            if (queryPeople.Count() > 0)
            {
                dataGridViewStudents.DataSource = queryPeople.CopyToDataTable();
                dataGridViewStudents.Columns["ID"].Visible = false;
                dataGridViewStudents.Columns["ImageID"].Visible = false;
            }
        }

        private void dataGridViewStudents_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                (sender as DataGridView).CurrentCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            rowGroup.BeginEdit();
            rowGroup["Name"] = textBoxGroupName.Text;
            rowGroup["Admission"] = dateTimePickerAdmission.Value.Date;
            rowGroup.EndEdit();

            adapterGroups.Update(dataSet.Tables["Groups"]);
            dataSet.Tables["Groups"].Clear();
            adapterGroups.Fill(dataSet.Tables["Groups"]);

            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void dataGridViewStudents_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PeopleDialog studentDialog = new PeopleDialog(dataSet, adapterImages, ImageIdDefault.Student);
            studentDialog.Text = "Edit student";
            var row = dataGridViewStudents.Rows[e.RowIndex];

            studentDialog.ID = (int)row.Cells["ID"].Value;
            studentDialog.LastName = row.Cells["LastName"].Value as string;
            studentDialog.FirstName = row.Cells["FirstName"].Value as string;
            studentDialog.MiddleName = row.Cells["MiddleName"].Value as string;
            studentDialog.Birthday = (DateTime)row.Cells["Birthday"].Value;
            studentDialog.PhoneNumber = row.Cells["PhoneNumber"].Value as string;
            studentDialog.Email = row.Cells["Email"].Value as string;
            studentDialog.Address = row.Cells["Address"].Value as string;
            studentDialog.ImageID = (int)row.Cells["ImageID"].Value;

            if(DialogResult.OK == studentDialog.ShowDialog())
            {
                DataRow rowPeople = dataSet.Tables["People"].
                    Select("ID=" + studentDialog.ID.ToString())[0];

                rowPeople["LastName"] = studentDialog.LastName;
                rowPeople["FirstName"] = studentDialog.FirstName;
                rowPeople["MiddleName"] = studentDialog.MiddleName;
                rowPeople["Birthday"] = studentDialog.Birthday;
                rowPeople["PhoneNumber"] = studentDialog.PhoneNumber;
                rowPeople["Email"] = studentDialog.Email;
                rowPeople["Address"] = studentDialog.Address;
                rowPeople["ImageID"] = studentDialog.ImageID;

                adapterPeople.Update(dataSet.Tables["People"]);
                dataSet.Tables["People"].Clear();
                adapterPeople.Fill(dataSet.Tables["People"]);
                FillDateGridViewStusents();
            }
        }

        private void addStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PeopleDialog studentDialog = new PeopleDialog(dataSet, adapterImages, ImageIdDefault.Student);
            studentDialog.Text = "Add student";
            if (DialogResult.OK == studentDialog.ShowDialog())
            {
                DataRow rowPeople = dataSet.Tables["People"].NewRow();
                rowPeople["LastName"] = studentDialog.LastName;
                rowPeople["FirstName"] = studentDialog.FirstName;
                rowPeople["MiddleName"] = studentDialog.MiddleName;
                rowPeople["Birthday"] = studentDialog.Birthday;
                rowPeople["PhoneNumber"] = studentDialog.PhoneNumber;
                rowPeople["Email"] = studentDialog.Email;
                rowPeople["Address"] = studentDialog.Address;
                rowPeople["ImageID"] = studentDialog.ImageID;

                dataSet.Tables["People"].Rows.Add(rowPeople);
                adapterPeople.Update(dataSet.Tables["People"]);
                dataSet.Tables["People"].Clear();
                adapterPeople.Fill(dataSet.Tables["People"]);
                int idPeople = (int)dataSet.Tables["People"].AsEnumerable().Max(p => p["ID"]);
                DataRow rowStudent = dataSet.Tables["Students"].NewRow();
                rowStudent["PeopleID"] = idPeople;
                rowStudent["GroupID"] = groupID;
                dataSet.Tables["Students"].Rows.Add(rowStudent);
                adapterStudents.Update(dataSet.Tables["Students"]);
                dataSet.Tables["Students"].Clear();
                adapterStudents.Fill(dataSet.Tables["Students"]);

                FillDateGridViewStusents();
            }
        }

        private void listBoxSubjects_DoubleClick(object sender, EventArgs e)
        {
            SubjectDialog subjectDialog = new SubjectDialog(dataSet, adapterImages, ImageIdDefault.Subject);
            subjectDialog.Text = "Add subject";
            subjectDialog.ID = (int)listBoxSubjects.SelectedValue;
            DataRow row = dataSet.Tables["Subjects"].Select("ID=" + subjectDialog.ID)[0];
            subjectDialog.ImageID = (int)row["ImageID"];
            subjectDialog.NameSubject = row["Name"] as string;

            if (DialogResult.OK == subjectDialog.ShowDialog())
            {
                row["Name"] = subjectDialog.NameSubject;
                row["ImageID"] = subjectDialog.ImageID;
                adapterSubjects.Update(dataSet.Tables["Subjects"]);
                dataSet.Tables["Subjects"].Clear();
                adapterSubjects.Fill(dataSet.Tables["Subjects"]);
                FillListBoxSubject();
            }
        }

        private void buttonAddEvaluation_Click(object sender, EventArgs e)
        {
            AddEvaluation addEvaluation = new AddEvaluation(dataSet, adapterEvaluations, groupID, (int)listBoxSubjects.SelectedValue);
            if(DialogResult.OK == addEvaluation.ShowDialog())
            {
                FillDateGridViewStusents();
                FillListBoxSubject();
            }
        }
    }
}

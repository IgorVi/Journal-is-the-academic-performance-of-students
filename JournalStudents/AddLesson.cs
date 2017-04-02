using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JournalStudents
{
    public partial class AddLesson : Form
    {
        DataSet dataSet;
        SqlDataAdapter adapterLessons;
        public AddLesson(DataSet dataSet, SqlDataAdapter adapterLessons)
        {
            InitializeComponent();
            this.dataSet = dataSet;
            this.adapterLessons = adapterLessons;

            comboBoxGroups.DataSource = dataSet.Tables["Groups"];
            comboBoxGroups.DisplayMember = "Name";
            comboBoxGroups.ValueMember = "ID";

            comboBoxSubjects.DataSource = dataSet.Tables["Subjects"];
            comboBoxSubjects.DisplayMember = "Name";
            comboBoxSubjects.ValueMember = "ID";

            var q = from t in dataSet.Tables["Teachers"].AsEnumerable()
                    join p in dataSet.Tables["People"].AsEnumerable()
                    on t["PeopleID"] equals p["ID"]
                    select new
                    {
                        FullName = p["LastName"] + " " + p["FirstName"] + " " + p["MiddleName"],
                        TeacherID = t["ID"]
                    };

            comboBoxTeachers.DataSource = q.ToList();
            comboBoxTeachers.DisplayMember = "FullName";
            comboBoxTeachers.ValueMember = "TeacherID";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DataRow row = dataSet.Tables["Lessons"].NewRow();
            row["SubjectID"] = comboBoxSubjects.SelectedValue;
            row["TeacherID"] = comboBoxTeachers.SelectedValue;
            row["GroupID"] = comboBoxGroups.SelectedValue;
            row["DateTime"] = dateTimePicker1.Value;
            dataSet.Tables["Lessons"].Rows.Add(row);
            adapterLessons.Update(dataSet.Tables["Lessons"]);
            dataSet.Tables["Lessons"].Clear();
            adapterLessons.Fill(dataSet.Tables["Lessons"]);
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

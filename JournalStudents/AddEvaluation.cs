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
    public partial class AddEvaluation : Form
    {
        DataSet dataSet;
        SqlDataAdapter adapterEvaluations;
        public AddEvaluation(DataSet dataSet, SqlDataAdapter adapterEvaluations, int groupID, int subjectID)
        {
            InitializeComponent();
            this.dataSet = dataSet;
            this.adapterEvaluations = adapterEvaluations;

            var lesson = from g in dataSet.Tables["Groups"].AsEnumerable()
                         join l in dataSet.Tables["Lessons"].AsEnumerable()
                         on g["ID"] equals l["GroupID"]
                         join s in dataSet.Tables["Subjects"].AsEnumerable()
                         on l["SubjectID"] equals s["ID"]
                         join t in dataSet.Tables["Teachers"].AsEnumerable()
                         on l["TeacherID"] equals t["ID"]
                         join p in dataSet.Tables["People"].AsEnumerable()
                         on t["PeopleID"] equals p["ID"]
                         where g["ID"].Equals(groupID) && s["ID"].Equals(subjectID)
                         select new
                         {
                             Name = s["Name"] + " " + ((DateTime)l["DateTime"]).ToShortDateString() + " " + p["LastName"] + " " + p["FirstName"] + " " + p["MiddleName"],
                             LessonID = l["ID"]
                         };

            comboBoxLessons.DataSource = lesson.ToList();
            comboBoxLessons.DisplayMember = "Name";
            comboBoxLessons.ValueMember = "LessonID";

            var students = from g in dataSet.Tables["Groups"].AsEnumerable()
                         join s in dataSet.Tables["Students"].AsEnumerable()
                         on g["ID"] equals s["GroupID"]
                         join p in dataSet.Tables["People"].AsEnumerable()
                         on s["PeopleID"] equals p["ID"]
                         where g["ID"].Equals(groupID)
                         select new
                         {
                             Name = p["LastName"] + " " + p["FirstName"] + " " + p["MiddleName"],
                             StudentID = s["ID"]
                         };

            comboBoxStudents.DataSource = students.ToList();
            comboBoxStudents.DisplayMember = "Name";
            comboBoxStudents.ValueMember = "StudentID";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DataRow row = dataSet.Tables["Evaluation"].NewRow();
            row["Evaluation"] = (int)numericUpDownEvaluation.Value;
            row["StudentID"] = comboBoxStudents.SelectedValue;
            row["LessonID"] = comboBoxLessons.SelectedValue;
            dataSet.Tables["Evaluation"].Rows.Add(row);
            adapterEvaluations.Update(dataSet.Tables["Evaluation"]);
            dataSet.Tables["Evaluation"].Clear();
            adapterEvaluations.Fill(dataSet.Tables["Evaluation"]);

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using JournalStudents.Properties;
using System.Threading;

namespace JournalStudents
{
    public partial class MainForm : Form
    {
        private DataSet dataSet;
        private SqlDataAdapter adapterGroups;
        private SqlDataAdapter adapterSubjects;
        private SqlDataAdapter adapterLessons;
        private SqlDataAdapter adapterTeachers;
        private SqlDataAdapter adapterPeople;
        private SqlDataAdapter adapterStudents;
        private SqlDataAdapter adapterImages;
        private SqlDataAdapter adapterEvaluations;
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePickerEnd.Value = dateTimePickerEnd.Value.AddDays(6);

            string connectionString = Settings.Default.JournalStudents;
            dataSet = new DataSet("JournalStudents");
            adapterGroups = new SqlDataAdapter("SELECT * FROM Groups", connectionString);
            new SqlCommandBuilder(adapterGroups);
            adapterSubjects = new SqlDataAdapter("SELECT * FROM Subjects", connectionString);
            new SqlCommandBuilder(adapterSubjects);
            adapterLessons = new SqlDataAdapter("SELECT * FROM Lessons", connectionString);
            new SqlCommandBuilder(adapterLessons);
            adapterTeachers = new SqlDataAdapter("SELECT * FROM Teachers", connectionString);
            new SqlCommandBuilder(adapterTeachers);
            adapterPeople = new SqlDataAdapter("SELECT * FROM People", connectionString);
            new SqlCommandBuilder(adapterPeople);
            adapterStudents = new SqlDataAdapter("SELECT * FROM Students", connectionString);
            new SqlCommandBuilder(adapterStudents);
            adapterImages = new SqlDataAdapter("SELECT * FROM Images", connectionString);
            new SqlCommandBuilder(adapterImages);
            adapterEvaluations = new SqlDataAdapter("SELECT * FROM Evaluation", connectionString);
            new SqlCommandBuilder(adapterEvaluations);

            adapterGroups.Fill(dataSet, "Groups");
            adapterSubjects.Fill(dataSet, "Subjects");
            adapterLessons.Fill(dataSet, "Lessons");
            adapterTeachers.Fill(dataSet, "Teachers");
            adapterPeople.Fill(dataSet, "People");
            adapterStudents.Fill(dataSet, "Students");
            adapterImages.Fill(dataSet, "Images");
            adapterEvaluations.Fill(dataSet, "Evaluation");

            //dataSet.Relations.Add("GroupsLessons",
            //    dataSet.Tables["Groups"].Columns["ID"],
            //    dataSet.Tables["Lessons"].Columns["GroupID"]);
            //dataSet.Relations.Add("SubjectsLessons",
            //    dataSet.Tables["Subjects"].Columns["ID"],
            //    dataSet.Tables["Lessons"].Columns["SubjectID"]);

            listBoxGroups.DataSource = dataSet.Tables["Groups"];
            listBoxGroups.ValueMember = "ID";
            listBoxGroups.DisplayMember = "Name";

            ListBoxGroups_Click(null, null);
            //ListBoxSubjects_Click(null, null);
            //DataGridViewTeachers_CellClick(dataGridViewEvaluation, new DataGridViewCellEventArgs(0, 0));

            listBoxGroups.Click += ListBoxGroups_Click;
            listBoxSubjects.Click += ListBoxSubjects_Click;

            listBoxGroups.DoubleClick += ListBoxGroups_DoubleClick;
            listBoxSubjects.DoubleClick += ListBoxSubjects_DoubleClick;
            dataGridViewTeachers.CellClick += DataGridViewTeachers_CellClick;
            dateTimePickerStart.ValueChanged += dateTimePicker_ValueChanged;
            dateTimePickerEnd.ValueChanged += dateTimePicker_ValueChanged;
        }

        private void DataGridViewTeachers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridViewTeachers.CurrentRow;

                var students = from g in dataSet.Tables["Groups"].AsEnumerable()
                               join s in dataSet.Tables["Students"].AsEnumerable()
                               on g["ID"] equals s["GroupID"]
                               join p in dataSet.Tables["People"].AsEnumerable()
                               on s["PeopleID"] equals p["ID"]
                               where g["ID"].Equals((int)listBoxGroups.SelectedValue)
                               select new
                               {
                                   FullNane = p["LastName"] as string + " " + p["FirstName"] as string + " " + p["MiddleName"] as string,
                                   PeopleID = p["ID"]
                               };

                var lessons = from g in dataSet.Tables["Groups"].AsEnumerable()
                              join l in dataSet.Tables["Lessons"].AsEnumerable()
                              on g["ID"] equals l["GroupID"]
                              join t in dataSet.Tables["Teachers"].AsEnumerable()
                              on l["TeacherID"] equals t["ID"]
                              join s in dataSet.Tables["Subjects"].AsEnumerable()
                              on l["SubjectID"] equals s["ID"]
                              where g["ID"].Equals((int)listBoxGroups.SelectedValue)
                                  && t["PeopleID"].Equals(row.Cells["ID"].Value)
                                  && (DateTime)l["DateTime"] >= dateTimePickerStart.Value
                                  && (DateTime)l["DateTime"] <= dateTimePickerEnd.Value
                                  && s["ID"].Equals(listBoxSubjects.SelectedValue)
                              select new
                              {
                                  DateTimeLesson = (DateTime)l["DateTime"]
                              };

                var evaluations = from teacher in dataSet.Tables["Teachers"].AsEnumerable()
                                  join lesson in dataSet.Tables["Lessons"].AsEnumerable()
                                  on teacher["ID"] equals lesson["TeacherID"]
                                  join Group in dataSet.Tables["Groups"].AsEnumerable()
                                  on lesson["GroupID"] equals Group["ID"]
                                  join student in dataSet.Tables["Students"].AsEnumerable()
                                  on Group["ID"] equals student["GroupID"]
                                  join people in dataSet.Tables["People"].AsEnumerable()
                                  on student["PeopleID"] equals people["ID"]
                                  join evaluation in dataSet.Tables["Evaluation"].AsEnumerable()
                                  on lesson["ID"] equals evaluation["LessonID"]
                                  join subject in dataSet.Tables["Subjects"].AsEnumerable()
                                  on lesson["SubjectID"] equals subject["ID"]

                                  where Group["ID"].Equals((int)listBoxGroups.SelectedValue)
                                  && teacher["PeopleID"].Equals(row.Cells["ID"].Value)
                                          && evaluation["StudentID"].Equals(student["ID"])
                                  && (DateTime)lesson["DateTime"] >= dateTimePickerStart.Value
                                        && (DateTime)lesson["DateTime"] <= dateTimePickerEnd.Value
                                         && subject["ID"].Equals(listBoxSubjects.SelectedValue)
                                  select new
                                  {
                                      FullNane = people["LastName"] as string + " " + people["FirstName"] as string + " " + people["MiddleName"] as string,
                                      Evaluation = (int)evaluation["Evaluation"],
                                      DateTimeLesson = (DateTime)lesson["DateTime"],
                                      PeopleID = people["ID"]
                                  };

                DataTable table = new DataTable();
                table.Columns.Add("StudentID");
                table.Columns.Add("Student");
                DateTime dateTime = new DateTime(dateTimePickerStart.Value.Year, dateTimePickerStart.Value.Month, dateTimePickerStart.Value.Day);
                for (; dateTime < dateTimePickerEnd.Value; dateTime = dateTime.AddDays(1))
                {
                    table.Columns.Add(dateTime.Day.ToString());
                }

                //int countDay = (dateTimePickerEnd.Value - dateTimePickerStart.Value).Days + dateTimePickerStart.Value.Day;
                //for (int i = dateTimePickerStart.Value.Day; i <= countDay; i++)
                //{
                //    table.Columns.Add(i.ToString());
                //}
                foreach (var item in students)
                {
                    DataRow r = table.NewRow();
                    r["StudentID"] = item.PeopleID;
                    r["Student"] = item.FullNane;
                    table.Rows.Add(r);
                }
                foreach (var item in evaluations)
                {
                    DataRow r = table.Select("StudentID=" + item.PeopleID)[0];
                    r[item.DateTimeLesson.Day.ToString()] = item.Evaluation;
                }
                dataGridViewEvaluation.DataSource = null;
                dataGridViewEvaluation.DataSource = table.DefaultView;
                dataGridViewEvaluation.Columns["StudentID"].Visible = false;
                foreach (var item in lessons)
                {
                    dataGridViewEvaluation.Columns[item.DateTimeLesson.Day.ToString()].DefaultCellStyle.BackColor = Color.Blue;
                }
            }
        }

        private void ListBoxSubjects_DoubleClick(object sender, EventArgs e)
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
                ListBoxGroups_Click(null, null);
            }
        }

        private void ListBoxGroups_DoubleClick(object sender, EventArgs e)
        {
            Group group = new Group(dataSet, (int)listBoxGroups.SelectedValue,
                adapterGroups, adapterSubjects, adapterStudents, adapterPeople, adapterImages, adapterEvaluations);
            group.ShowDialog();
        }

        private void ListBoxGroups_Click(object sender, EventArgs e)
        {
            var q = from l in dataSet.Tables["Lessons"].AsEnumerable()
                    join s in dataSet.Tables["Subjects"].AsEnumerable()
                    on l["SubjectID"] equals s["ID"]
                    where l["GroupID"].Equals(listBoxGroups.SelectedValue)
                    select s;
            if (q.Count() > 0)
            {
                listBoxSubjects.DataSource = q.CopyToDataTable();
                listBoxSubjects.ValueMember = "ID";
                listBoxSubjects.DisplayMember = "Name";
                ListBoxSubjects_Click(null, null);
            }
            else
            {
                listBoxSubjects.DataSource = null;
                dataGridViewTeachers.DataSource = null;
                dataGridViewEvaluation.DataSource = null;
            }
        }

        private void ListBoxSubjects_Click(object sender, EventArgs e)
        {
            if (listBoxSubjects.DataSource == null) return;

            var q = from l in dataSet.Tables["Lessons"].AsEnumerable()
                    join t in dataSet.Tables["Teachers"].AsEnumerable()
                    on l["TeacherID"] equals t["ID"]
                    join p in dataSet.Tables["People"].AsEnumerable()
                    on t["PeopleID"] equals p["ID"]
                    where l["SubjectID"].Equals(listBoxSubjects.SelectedValue) && l["GroupID"].Equals(listBoxGroups.SelectedValue)
                    select p;

            if(q.Count() > 0)
            {
                dataGridViewTeachers.DataSource = q.CopyToDataTable();
                dataGridViewTeachers.Columns["ID"].Visible = false;
                dataGridViewTeachers.Columns["ImageID"].Visible = false;
                dataGridViewTeachers.Select();
                DataGridViewTeachers_CellClick(dataGridViewEvaluation, new DataGridViewCellEventArgs(0, 0));
            }
            else
            {
                dataGridViewTeachers.DataSource = null;
                dataGridViewEvaluation.DataSource = null;
            }
        }

        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddGroup addGroup = new AddGroup();
            if (DialogResult.OK == addGroup.ShowDialog())
            {
                DataRow row = dataSet.Tables["Groups"].NewRow();
                row["Name"] = addGroup.NameGroup;
                row["Admission"] = addGroup.Admission;
                dataSet.Tables["Groups"].Rows.Add(row);
                adapterGroups.Update(dataSet.Tables["Groups"]);
                dataSet.Tables["Groups"].Clear();
                adapterGroups.Fill(dataSet.Tables["Groups"]);

            }
        }

        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                (sender as DataGridView).CurrentCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        private void dataGridViewTeachers_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PeopleDialog teacherDialog = new PeopleDialog(dataSet, adapterImages, ImageIdDefault.Teacher);
            teacherDialog.Text = "Edit teacher";
            var row = dataGridViewTeachers.Rows[e.RowIndex];

            teacherDialog.ID = (int)row.Cells["ID"].Value;
            teacherDialog.LastName = row.Cells["LastName"].Value as string;
            teacherDialog.FirstName = row.Cells["FirstName"].Value as string;
            teacherDialog.MiddleName = row.Cells["MiddleName"].Value as string;
            teacherDialog.Birthday = (DateTime)row.Cells["Birthday"].Value;
            teacherDialog.PhoneNumber = row.Cells["PhoneNumber"].Value as string;
            teacherDialog.Email = row.Cells["Email"].Value as string;
            teacherDialog.Address = row.Cells["Address"].Value as string;
            teacherDialog.ImageID = (int)row.Cells["ImageID"].Value;

            if (DialogResult.OK == teacherDialog.ShowDialog())
            {
                DataRow rowPeople = dataSet.Tables["People"].
                    Select("ID=" + teacherDialog.ID.ToString())[0];

                rowPeople["LastName"] = teacherDialog.LastName;
                rowPeople["FirstName"] = teacherDialog.FirstName;
                rowPeople["MiddleName"] = teacherDialog.MiddleName;
                rowPeople["Birthday"] = teacherDialog.Birthday;
                rowPeople["PhoneNumber"] = teacherDialog.PhoneNumber;
                rowPeople["Email"] = teacherDialog.Email;
                rowPeople["Address"] = teacherDialog.Address;
                rowPeople["ImageID"] = teacherDialog.ImageID;

                adapterPeople.Update(dataSet.Tables["People"]);
                dataSet.Tables["People"].Clear();
                adapterPeople.Fill(dataSet.Tables["People"]);
                ListBoxSubjects_Click(null, null);
            }
        }

        private void addTeacherToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            PeopleDialog teacherDialog = new PeopleDialog(dataSet, adapterImages, ImageIdDefault.Teacher);
            teacherDialog.Text = "Add teacher";
            if (DialogResult.OK == teacherDialog.ShowDialog())
            {
                DataRow rowPeople = dataSet.Tables["People"].NewRow();
                rowPeople["LastName"] = teacherDialog.LastName;
                rowPeople["FirstName"] = teacherDialog.FirstName;
                rowPeople["MiddleName"] = teacherDialog.MiddleName;
                rowPeople["Birthday"] = teacherDialog.Birthday;
                rowPeople["PhoneNumber"] = teacherDialog.PhoneNumber;
                rowPeople["Email"] = teacherDialog.Email;
                rowPeople["Address"] = teacherDialog.Address;
                rowPeople["ImageID"] = teacherDialog.ImageID;

                dataSet.Tables["People"].Rows.Add(rowPeople);
                adapterPeople.Update(dataSet.Tables["People"]);
                dataSet.Tables["People"].Clear();
                adapterPeople.Fill(dataSet.Tables["People"]);

                int idPeople = (int)dataSet.Tables["People"].AsEnumerable().Max(p => p["ID"]);
                DataRow rowStudent = dataSet.Tables["Teachers"].NewRow();
                rowStudent["PeopleID"] = idPeople;
                dataSet.Tables["Teachers"].Rows.Add(rowStudent);

                adapterTeachers.Update(dataSet.Tables["Teachers"]);
                dataSet.Tables["Teachers"].Clear();
                adapterTeachers.Fill(dataSet.Tables["Teachers"]);
            }
        }

        private void addSubjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubjectDialog subjectDialog = new SubjectDialog(dataSet, adapterImages, ImageIdDefault.Subject);
            subjectDialog.Text = "Add subject";

            if (DialogResult.OK == subjectDialog.ShowDialog())
            {
                DataRow row = dataSet.Tables["Subjects"].NewRow();
                row["Name"] = subjectDialog.NameSubject;
                row["ImageID"] = subjectDialog.ImageID;
                dataSet.Tables["Subjects"].Rows.Add(row);
                adapterSubjects.Update(dataSet.Tables["Subjects"]);
                dataSet.Tables["Subjects"].Clear();
                adapterSubjects.Fill(dataSet.Tables["Subjects"]);
            }
        }

        bool rollback;
        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan timeSpan = dateTimePickerEnd.Value - dateTimePickerStart.Value;
            if (timeSpan.TotalDays > 28)
            {
                if (!rollback)
                {
                    MessageBox.Show("Количество дней должно быть меньше 28");
                    rollback = true;
                    dateTimePickerStart.Value = DateTime.Now;
                    dateTimePickerEnd.Value = DateTime.Now.AddDays(6);
                    rollback = false;
                    ListBoxGroups_Click(null, null);
                }
            }
            else
                ListBoxGroups_Click(null, null);
        }

        private void addLessonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLesson addLesson = new AddLesson(dataSet, adapterLessons);
            if(DialogResult.OK == addLesson.ShowDialog())
            {
                ListBoxGroups_Click(null, null);
            }

        }

        private void loadFromXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML (*.xml)|*.xml|All files(*.*)|*.*";
            openFileDialog1.FileName = "";
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                dataSet.Clear();
                dataSet.ReadXml(openFileDialog1.FileName);
            }
        }

        private void saveToXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "XML (*.xml)|*.xml|All files(*.*)|*.*";
            saveFileDialog1.FileName = "";
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                dataSet.WriteXml(saveFileDialog1.FileName);
            }
        }
    }
}

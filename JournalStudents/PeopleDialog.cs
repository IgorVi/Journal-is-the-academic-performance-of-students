using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace JournalStudents
{
    public partial class PeopleDialog : Form
    {
        private int imageID;
        private DataSet daraSet;
        private bool imageChenges;
        private SqlDataAdapter adapterImages;
        public PeopleDialog(DataSet daraSet, SqlDataAdapter adapterImages, ImageIdDefault imageIdDefault)
        {
            InitializeComponent();
            this.daraSet = daraSet;
            this.adapterImages = adapterImages;
            ImageID = (int)imageIdDefault;
        }

        public string LastName
        {
            get { return textBoxLastName.Text == string.Empty ? null : textBoxLastName.Text; }
            set { textBoxLastName.Text = value; }
        }
        public string FirstName
        {
            get { return textBoxFirstName.Text == string.Empty ? null : textBoxFirstName.Text; }
            set { textBoxFirstName.Text = value; }
        }
        public string MiddleName
        {
            get { return textBoxMiddleName.Text == string.Empty ? null : textBoxMiddleName.Text; }
            set { textBoxMiddleName.Text = value; }
        }
        public DateTime Birthday
        {
            get { return dateTimePickerBirthday.Value; }
            set { dateTimePickerBirthday.Value = value; }
        }
        public string PhoneNumber
        {
            get { return textBoxPhoneNumber.Text == string.Empty ? null : textBoxPhoneNumber.Text; }
            set { textBoxPhoneNumber.Text = value; }
        }
        public string Email
        {
            get { return textBoxEmail.Text == string.Empty ? null : textBoxEmail.Text; }
            set { textBoxEmail.Text = value; }
        }
        public string Address
        {
            get { return textBoxAddress.Text == string.Empty ? null : textBoxAddress.Text; }
            set { textBoxAddress.Text = value; }
        }
        public int ImageID
        {
            get { return imageID; }
            set
            {
                imageID = value;
                DataRow row = daraSet.Tables["Images"].Select("ID=" + ImageID)[0];
                MemoryStream stream = new MemoryStream((byte[])row["Image"]);
                pictureBox.Image = Image.FromStream(stream);
            }
        }
        public int ID { get; set; }

        private void buttonOpenFileImage_Click(object sender, EventArgs e)
        {
            imageChenges = true;
            openFileDialogImage.Filter = "Image Files(*.JPEG;*.JPG)|*.jpeg;*.jpg|Image Files(*.PNG)|*.png|Image Files(*.BMP)|*.bmp|All files(*.*)|*.*";
            if(DialogResult.OK == openFileDialogImage.ShowDialog())
            {
                textBoxPachImage.Text = openFileDialogImage.FileName;
                pictureBox.Image = Image.FromFile(textBoxPachImage.Text);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (imageChenges && (ImageID == (int)ImageIdDefault.Student ||
                                    ImageID == (int)ImageIdDefault.Teacher ||
                                    ImageID == (int)ImageIdDefault.Subject))
            {
                DataRow row = daraSet.Tables["Images"].NewRow();
                MemoryStream stream = new MemoryStream();
                pictureBox.Image.Save(stream, pictureBox.Image.RawFormat);
                row["Image"] = stream.GetBuffer();
                daraSet.Tables["Images"].Rows.Add(row);
                adapterImages.Update(daraSet.Tables["Images"]);
                daraSet.Tables["Images"].Clear();
                adapterImages.Fill(daraSet.Tables["Images"]);
                imageID = (int)daraSet.Tables["Images"].AsEnumerable().Max(i => i["ID"]);
            }
            else if (imageChenges && (ImageID != (int)ImageIdDefault.Student ||
                                    ImageID != (int)ImageIdDefault.Teacher ||
                                    ImageID != (int)ImageIdDefault.Subject))
            {
                DataRow row = daraSet.Tables["Images"].Select("ID=" + ImageID)[0];
                MemoryStream stream = new MemoryStream();
                pictureBox.Image.Save(stream, pictureBox.Image.RawFormat);
                row["Image"] = stream.GetBuffer();
                adapterImages.Update(daraSet.Tables["Images"]);
                daraSet.Tables["Images"].Clear();
                adapterImages.Fill(daraSet.Tables["Images"]);
                imageID = (int)daraSet.Tables["Images"].AsEnumerable().Max(i => i["ID"]);
            }
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

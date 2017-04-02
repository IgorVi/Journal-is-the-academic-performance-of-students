using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JournalStudents
{
    public partial class SubjectDialog : Form
    {
        private int imageID;
        private DataSet daraSet;
        private SqlDataAdapter adapterImages;
        private bool imageChenges;

        public SubjectDialog(DataSet daraSet, SqlDataAdapter adapterImages, ImageIdDefault imageIdDefault)
        {
            InitializeComponent();
            this.daraSet = daraSet;
            this.adapterImages = adapterImages;
            ImageID = (int)imageIdDefault;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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

        public int ID { get; set; }
        public string NameSubject
        {
            get
            {
                return textBoxName.Text == "" ? null: textBoxName.Text;
            }
            set
            {
                textBoxName.Text = value;
            }
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

        private void buttonOpenFileImage_Click(object sender, EventArgs e)
        {
            imageChenges = true;
            openFileDialogImage.Filter = "Image Files(*.JPEG;*.JPG)|*.jpeg;*.jpg|Image Files(*.PNG)|*.png|Image Files(*.BMP)|*.bmp|All files(*.*)|*.*";
            if (DialogResult.OK == openFileDialogImage.ShowDialog())
            {
                textBoxPachImage.Text = openFileDialogImage.FileName;
                pictureBox.Image = Image.FromFile(textBoxPachImage.Text);
            }
        }
    }
}

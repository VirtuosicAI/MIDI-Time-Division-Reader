using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIDI_Time_Division_Reader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void BrowseBTN_Click(object sender, EventArgs e)
        {
            if (OpenMIDIDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream midiReader = new FileStream(OpenMIDIDialog.FileName, FileMode.Open))
                {
                    midiReader.Seek(4, SeekOrigin.Begin);

                    byte[] headerLength = new byte[4];
                    midiReader.Read(headerLength, 0, headerLength.Length);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(headerLength);
                    int headerLengthInt = BitConverter.ToInt32(headerLength, 0);

                    if (headerLengthInt != 6)
                    {
                        MessageBox.Show(this, "Error: " + Path.GetFileName(OpenMIDIDialog.FileName) + " is not a MIDI file created under the MIDI 1.0 specification, and won't be opened for reading.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MIDIFileBox.Text = OpenMIDIDialog.FileName;
                    }
                }
            }
        }

        private void ReadBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(MIDIFileBox.Text) || !File.Exists(MIDIFileBox.Text))
                {
                    MessageBox.Show(this, "Error: MIDI file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    FileStream timeDivisionReader = new FileStream(MIDIFileBox.Text, FileMode.Open);

                    timeDivisionReader.Seek(12, SeekOrigin.Begin);

                    byte[] timeDivision = new byte[2];
                    timeDivisionReader.Read(timeDivision, 0, timeDivision.Length);
                    timeDivisionReader.Close();

                    string timeDivisionString = BitConverter.ToString(timeDivision);

                    int firstByteInt = timeDivision[0];
                    int secondByteInt = timeDivision[1];

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(timeDivision);

                    int timeDivisionInt = BitConverter.ToUInt16(timeDivision, 0);
                    int ticksPerBar = (1920 * timeDivisionInt) / 480;

                    MessageBox.Show(this, Path.GetFileName(MIDIFileBox.Text) + "\n\nHexdecimal String: " + timeDivisionString + "\nFirst Byte Value: " + firstByteInt.ToString() + "\nSecond Byte Value: " + secondByteInt.ToString() + "\nInteger Value: " + timeDivisionInt.ToString() + "\n\nTicks Per Bar: " + ticksPerBar.ToString(), "Result");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Developed by VirtuosicAI \n\n(https://github.com/VirtuosicAI)", "About");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Path.GetExtension(data[0]) == ".mid" && data.Length == 1)
                {
                    e.Effect = DragDropEffects.All;
                }
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var inputFileArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string midiFile = inputFileArray[0];

            using (FileStream midiReader = new FileStream(midiFile, FileMode.Open))
            {
                midiReader.Seek(4, SeekOrigin.Begin);

                byte[] headerLength = new byte[4];
                midiReader.Read(headerLength, 0, headerLength.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(headerLength);
                int headerLengthInt = BitConverter.ToInt32(headerLength, 0);

                if (headerLengthInt != 6)
                {
                    MessageBox.Show(this, "Error: " + Path.GetFileName(OpenMIDIDialog.FileName) + " is not a MIDI file created under the MIDI 1.0 specification, and won't be opened for reading.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MIDIFileBox.Text = midiFile;
                }
            }
        }
    }
}

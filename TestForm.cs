using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Speech;
using System.Speech.Synthesis;

namespace Question_Getter_Front_End
{
    public partial class TestForm : Form
    {
        private WikiInterface w;
        public TestForm()
        {
            InitializeComponent();
            w = new WikiInterface();
            Wnlib.WNCommon.path = "C:\\Program Files\\Word Net\\dict\\";
        }

        private void getSiteButton_Click(object sender, EventArgs e)
        {
            string search = getSiteText.Text;
            search = search.Replace(" ", "_");
            search = w.queryUrl(search);
            if (getSiteText.Text.Length == 0)
            {
                MessageBox.Show("Please enter something to search.");
                return;
            }
            string str = w.getSite(search,true);
            if (str == null)
                 return;
            StreamWriter sw = new StreamWriter("./wiki.txt");
            sw.Write(str);
        }

        private void Random_Click(object sender, EventArgs e)
        {
            string str = w.getSite(WikiInterface.randUrl,true);
            if (str == null)
                return;
            StreamWriter sw = new StreamWriter("./wiki.txt");
            sw.Write(str);
        }

        private void Xml_Click(object sender, EventArgs e)
        {
        }

        private void wtionary_Click(object sender, EventArgs e)
        {
            string str=w.getSite(w.queryUrl(getSiteText.Text).Replace("en.wikipedia", "en.wiktionary"), true);
            if (str == null)
                return;
            StreamWriter sw = new StreamWriter("./wiki.txt");
            sw.Write(str);
        }

        private void RandTestButton_Click(object sender, EventArgs e)
        {
            int x;
            string write;
            StreamWriter sw;
            for (x = 1; x <= 10; x++)
            {
                write = w.getSite(WikiInterface.randUrl,true);
                (sw = new StreamWriter("C:\\QGData\\wiki" + x + ".txt")).Write(write);
                sw.Close();
            }
        }

        private void WordType_Click(object sender, EventArgs e)
        {
            Word wrd = new Word(TypeBox.Text);
            DictionaryInterface.findType(wrd);
            TypeLabel.Text = wrd.type;
        }

        private void blt_Click(object sender, EventArgs e)
        {
            MessageBox.Show(w.getNumBackLinks(getSiteText.Text).ToString());
        }

    }
}

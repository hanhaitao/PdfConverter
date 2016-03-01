using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Controls
{
    public partial class ListViewST : UserControl
    {
        public int DefaultCount
        {
            get { return m_DefaultCount; }
            set { m_DefaultCount = value; Init(); }
        }private int m_DefaultCount;

        public List<Item> Items
        {
            get;
            set;
        }

        public ListViewST()
        {
            InitializeComponent();
            Items = new List<Item>();
            DefaultCount = 7;
            Init();
        }

        public void Init()
        {
            panelItem.Controls.Clear();
            for (int i = 0; i < Items.Count; i++)
            {
                panelItem.Controls.Add(Items[i]);
            }
            for (int i = panelItem.Controls.Count; i < DefaultCount; i++)
            {
                Item c = new Item();
                c.Dock = DockStyle.Top;
                Random r = new Random();
                c.Name = "item" + i + 1;
                panelItem.Controls.Add(c);
            }
        }

        public int AddItem(string filePath, string convertPath)
        {
            Item i = new Item(Application.StartupPath, Application.StartupPath);
            Random r = new Random();
            i.Name = "item" + r.Next(100000);
            Items.Add(i);
            Init();
            return 0;
        }
    }
}

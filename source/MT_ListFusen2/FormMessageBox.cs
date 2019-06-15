using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MT_ListFusen2
{
	public partial class FormMessageBox : Form
	{
		public FormMessageBox()
		{
			InitializeComponent();
		}

		private void FormMessageBox_Load(object sender, EventArgs e)
		{
			// メインカラーを反映
			panelLabel3.BackColor = Settings.mainColor;
		}
	}
}

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
	public partial class FormSettings : Form
	{
		public FormSettings()
		{
			InitializeComponent();
		}

		// 変数
		public static Font	fontTB;
		public static Font	fontTV;
		public static Color	fontColorTB;
		public static Color	fontColorTV;
		public static bool	wordWrap;
		public static Color	mainColor;
		public static bool	toolTip;
		public static int	autoSave;
		public static int	autoBackup;
		public static bool	deactiveSave;

		// 関数：初期化
		public static void Initialize()
		{
			fontTB			= new Font("Meiryo", 9.75F);
			fontTV			= new Font("Meiryo", 10F);
			fontColorTB		= Color.FromArgb(190, 185, 180);
			fontColorTV		= Color.FromArgb(190, 185, 180);
			wordWrap		= false;
			mainColor		= Color.FromArgb(191, 92, 55);
			toolTip			= true;
			autoSave		= 0;
			autoBackup		= 3;
			deactiveSave	= false;
		}

		// ボタン：全て初期値に戻す
		private void ButtonResetSettings_Click(object sender, EventArgs e)
		{
			// 初期化
			Initialize();

			// 初期化した値をSettingsフォームに反映
			this.panelLabel1.BackColor = mainColor;
			this.cbWordWrap.Checked = wordWrap;
			this.cbToolTip.Checked = toolTip;
			this.cbDeactiveSave.Checked = deactiveSave;
			this.cmbAutoSave.SelectedIndex = autoSave;
			this.cmbAutoBackup.SelectedIndex = autoBackup;
		}

		// イベント：フォームがロードされたとき
		private void FormSettings_Load(object sender, EventArgs e)
		{
			// 現在の値をSettingsフォームの変数に格納
			fontTB			= Properties.Settings.Default.FontTB;
			fontTV			= Properties.Settings.Default.FontTV;
			fontColorTB		= Settings.fontColorTB;
			fontColorTV		= Settings.fontColorTV;
			wordWrap		= Settings.wordWrap;
			mainColor		= Settings.mainColor;
			toolTip			= Settings.toolTip;
			autoSave		= Settings.autoSave;
			autoBackup		= Settings.autoBackup;
			deactiveSave	= Settings.deactiveSave;

			// 現在の値をSettingsフォームに反映
			this.panelLabel1.BackColor	= mainColor;
			this.cbWordWrap.Checked		= wordWrap;
			this.cbToolTip.Checked		= toolTip;
			this.cbDeactiveSave.Checked	= deactiveSave;
			try
			{
				this.cmbAutoSave.SelectedIndex = autoSave;
				this.cmbAutoBackup.SelectedIndex = autoBackup;
			}
			catch (Exception)
			{
				this.cmbAutoSave.SelectedIndex = 0;
				this.cmbAutoBackup.SelectedIndex = 3;
			}
		}
		
		// ボタン：TextBoxのフォントスタイルの変更
		private void ButtonFStyleTB_Click(object sender, EventArgs e)
		{
			try
			{
				// フォント設定ダイアログを表示する
				if (fontDialogTB.ShowDialog() != DialogResult.Cancel)
				{
					// フォントの設定を更新
					fontTB = fontDialogTB.Font;
				}
			}
			catch (ArgumentException)
			{
				MessageBox.Show("そのフォントは使えません");
			}
		}
		// ボタン：TextBoxのフォントカラーの変更
		private void ButtonFColorTB_Click(object sender, EventArgs e)
		{
			try
			{
				// カラー設定ダイアログを表示する
				if (colorDialogTB.ShowDialog() != DialogResult.Cancel)
				{
					// フォントカラーの設定を更新
					fontColorTB = colorDialogTB.Color;
				}
			}
			catch (ArgumentException)
			{
				MessageBox.Show("そのフォントは使えません");
			}
		}
		// ボタン：TreeViewのフォントスタイルの変更
		private void ButtonFStyleTV_Click(object sender, EventArgs e)
		{
			try
			{
				// フォント設定ダイアログを表示する
				if (fontDialogTV.ShowDialog() != DialogResult.Cancel)
				{
					// フォントの設定を更新
					fontTV = fontDialogTV.Font;
				}
			}
			catch (ArgumentException)
			{
				MessageBox.Show("そのフォントは使えません");
			}
		}
		// ボタン：TreeViewのフォントカラーの変更
		private void ButtonFColorTV_Click(object sender, EventArgs e)
		{
			try
			{
				// カラー設定ダイアログを表示する
				if (colorDialogTV.ShowDialog() != DialogResult.Cancel)
				{
					// フォントカラーの設定を更新
					fontColorTV = colorDialogTV.Color;
				}
			}
			catch (ArgumentException)
			{
				MessageBox.Show("そのフォントは使えません");
			}			
		}

		// チェックボックス：右端で折り返す
		private void CbWordWrap_CheckedChanged(object sender, EventArgs e)
		{
			// ON の場合
			if (cbWordWrap.Checked == true)
			{
				// 設定を更新
				wordWrap = true;
			}
			// OFF の場合
			else
			{
				// 設定を更新
				wordWrap = false;
			}
		}
		// ボタン：メインカラーを変更
		private void ButtonMainColor_Click(object sender, EventArgs e)
		{
			try
			{
				// カラー設定ダイアログを表示する
				if (colorDialogMain.ShowDialog() != DialogResult.Cancel)
				{
					// メインカラーの設定を更新
					mainColor = colorDialogMain.Color;

					// 設定をSettingsフォームに反映
					this.panelLabel1.BackColor = mainColor;
				}
			}
			catch (ArgumentException)
			{
				MessageBox.Show("そのフォントは使えません");
			}
			
		}
		// チェックボックス：ツールチップの表示
		private void CbToolTip_CheckedChanged(object sender, EventArgs e)
		{
			// ON の場合
			if (cbToolTip.Checked == true)
			{
				// 設定を更新
				toolTip = true;
			}
			// OFF の場合
			else
			{
				// 設定を更新
				toolTip = false;
			}
		}
		
		// コンボボックス：オートセーブの実行間隔
		private void CmbAutoSave_TextChanged(object sender, EventArgs e)
		{
			// 設定を更新
			autoSave = cmbAutoSave.SelectedIndex;
		}
		// コンボボックス：オートバックアップの実行間隔
		private void CmbAutoBackup_TextChanged(object sender, EventArgs e)
		{
			// 設定を更新
			autoBackup = cmbAutoBackup.SelectedIndex;
		}
		// チェックボックス：非アクティブ時のセーブ
		private void CbDeactiveSave_CheckedChanged(object sender, EventArgs e)
		{
			// ON の場合
			if (cbDeactiveSave.Checked == true)
			{
				// 設定を更新
				deactiveSave = true;
			}
			// OFF の場合
			else
			{
				// 設定を更新
				deactiveSave = false;
			}
		}
	}
}

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

namespace MT_ListFusen2
{
	public partial class FormExportAll : Form
	{
		// 変数
		TreeNodeCollection nodes;

		// コンストラクタ
		public FormExportAll(TreeNodeCollection tv)
		{
			InitializeComponent();

			nodes = tv;
		}

		private void FormExportAll_Load(object sender, EventArgs e)
		{
			// メインカラーを反映
			panelLabel3.BackColor = Settings.mainColor;

			// TextBoxにデフォルトでデスクトップパスを表示
			this.textBoxExport.Text =
				Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		}

		// 関数：ノードとメモを一括出力
		private void ExportAllMemo()
		{
			// ルートノードのテキストファイルを作成
			foreach (var node in nodes)
			{
				TreeNode nodeP = (TreeNode)node;

				// ルートノードの名前を取得
				string name = nodeP.Text;

				// ノードからメモオブジェクトを取得
				var memo = new Form1.Memo();
				memo = (Form1.Memo)nodeP.Tag;

				// 出力先パスとルートノードの名前を結合
				// フォルダパス末尾に"\"がある場合と無い場合に対処できる
				string mergePath = Path.Combine(this.textBoxExport.Text, name + @".txt");

				// txtファイルにメモの内容を書き込む
				File.WriteAllText(
					mergePath,
					memo.Text,
					Encoding.GetEncoding("shift_jis"));

				// パスを格納する変数
				string folderPath = Path.Combine(this.textBoxExport.Text, name);
				
				// 再帰処理
				ExportRec(nodeP, folderPath);
			}

			ShowMsgExportDone();
		}
		// 再帰処理用
		private void ExportRec(TreeNode nodeP, string pathP)
		{
			foreach (var nodeC in nodeP.Nodes)
			{
				// 親の名前でフォルダを作成
				Directory.CreateDirectory(pathP);

				TreeNode child = (TreeNode)nodeC;

				// ルートノードの名前を取得
				string nameC = child.Text;

				// ノードからメモオブジェクトを取得
				var memoC = new Form1.Memo();
				memoC = (Form1.Memo)child.Tag;

				// 出力先パスとルートノードの名前を結合
				// フォルダパス末尾に"\"がある場合と無い場合に対処できる
				string mergePath = Path.Combine(pathP, nameC + @".txt");

				// txtファイルにメモの内容を書き込む
				File.WriteAllText(
					mergePath,
					memoC.Text,
					Encoding.GetEncoding("shift_jis"));

				// パスを更新
				string pathC = Path.Combine(pathP, nameC);

				// 再帰処理
				ExportRec(child, pathC);
			}
		}
		
		// 関数：メッセージ「出力しました！」
		private void ShowMsgExportDone()
		{
			// メッセージフォーム
			var form = new FormMessageBox();
			// 画面の真ん中に表示
			form.StartPosition = FormStartPosition.CenterParent;

			// メッセージフォームをタスクバーに表示しない
			form.ShowInTaskbar = false;

			// 画像を非表示
			form.pictureBox1.Enabled = false;
			form.pictureBox1.Visible = false;
			// メッセージテキストを差し替え
			form.label1.Text = "出力しました！";
			form.label2.Enabled = false;
			form.label2.Visible = false;
			// フォームサイズを小さく
			form.Width = 250;
			form.Height = 100;
			// ボタンのアレンジ
			form.btnOK.Enabled = false;
			form.btnOK.Visible = false;
			form.btnCancel.Text = "OK";

			// メッセージフォームをモーダルで開く
			form.ShowDialog();

			// Disposeでフォームを解放
			form.Dispose();
		}

		// 関数：エラーメッセージ「出力先のパスが正しいか確認してください」
		public static void ShowMsgErrPath()
		{
			// メッセージフォーム
			var form = new FormMessageBox();
			// 画面の真ん中に表示
			form.StartPosition = FormStartPosition.CenterParent;

			// メッセージフォームをタスクバーに表示しない
			form.ShowInTaskbar = false;

			// 画像を×マークに差し替え
			form.pictureBox1.BackgroundImage = Properties.Resources.icon_alert_error;
			// メッセージテキストを差し替え
			form.label1.Text = "出力先のパスが正しいか確認してください";
			form.label1.ForeColor = Color.FromArgb(191, 92, 55);
			form.label2.Enabled = false;
			form.label2.Visible = false;
			// フォームサイズを伸ばす
			form.Width = 370;
			form.Height = 100;
			// ボタンのアレンジ
			form.btnOK.Enabled = false;
			form.btnOK.Visible = false;
			form.btnCancel.Text = "OK";

			// メッセージフォームをモーダルで開いて何のボタンで終了したかを受け取る
			form.ShowDialog();

			// Disposeでフォームを解放
			form.Dispose();
		}

		// ボタン：参照
		private void ButtonReference_Click(object sender, EventArgs e)
		{
			// FolderBrowserDialogクラスのインスタンスを作成
			FolderBrowserDialog fbd = new FolderBrowserDialog();

			// 上部に表示する説明テキストを指定する
			fbd.Description = "フォルダを指定してください。";
			// ルートフォルダを指定する
			// デフォルトでDesktop
			fbd.RootFolder = Environment.SpecialFolder.Desktop;
			// 最初に選択するフォルダを指定する
			// RootFolder以下にあるフォルダである必要がある
			fbd.SelectedPath = @"C:\Windows";
			// ユーザーが新しいフォルダを作成できるようにする
			// デフォルトでTrue
			fbd.ShowNewFolderButton = true;

			// ダイアログを表示する
			if (fbd.ShowDialog(this) == DialogResult.OK)
			{
				// 選択されたフォルダを表示する
				this.textBoxExport.Text = fbd.SelectedPath;
			}
		}

		// ボタン：出力
		private void BtnOK_Click(object sender, EventArgs e)
		{
			// フォルダパスが有効か判定する
			if (!Directory.Exists(this.textBoxExport.Text))
			{
				ShowMsgErrPath();

				return;
			}

			// 一括出力に失敗したらフォームを閉じずに戻る
			try
			{
				ExportAllMemo();
			}
			catch (Exception)
			{
				return;
			}

			// FormExportAllを閉じる
			this.Close();
		}

		// ボタン：キャンセル 何もせずダイアログを閉じる
		private void BtnCancel_Click(object sender, EventArgs e)
		{
			// FormExportAllを閉じる
			this.Close();
		}
	}
}

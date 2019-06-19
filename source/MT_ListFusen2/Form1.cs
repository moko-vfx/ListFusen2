using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MT_ListFusen2
{
	public partial class Form1 : Form
	{
		// 変数：マウス座標
		private int mouseX;
		private int mouseY;
		
		// 変数：オートセーブ/オートバックアップ用のタイマー
		private Timer timerAutoSave;
		private Timer timerAutoBackup;

		//********************************************************//
		//　スクロールバーの位置を取得＆復元するための定数と関数　//
		//********************************************************//

		const int SB_HORZ = 0x00;
		const int SB_VERT = 0x01;
		const int WM_HSCROLL = 0x0114;
		const int WM_VSCROLL = 0x0115;
		const int SB_THUMBPOSITION = 4;

		[DllImport("USER32.DLL", CharSet = CharSet.Auto)]
		static extern int GetScrollPos(IntPtr hWnd, Int32 nBar);
		[DllImport("user32.dll")]
		static extern int SetScrollPos(IntPtr hWnd, Int32 nBar, int nPos, bool bRedraw);
		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

		public static Point GetTextBoxScrollPos(TextBox textBox)
		{
			return new Point(GetScrollPos(textBox.Handle, SB_HORZ),
								GetScrollPos(textBox.Handle, SB_VERT));
		}
		public static void SetTextBoxScrollPos(TextBox textBox, Point newPos)
		{
			SendMessage(textBox.Handle, WM_HSCROLL, (newPos.X << 16) + SB_THUMBPOSITION, 0);
			SendMessage(textBox.Handle, WM_VSCROLL, (newPos.Y << 16) + SB_THUMBPOSITION, 0);
		}

		//********************************************************//
		//　　前回終了時の選択ノードを選択するための変数と関数　　//
		//********************************************************//
		
		List<int> listNodeId = new List<int>();	// Indexを格納
		TreeNode selNode;   // 最終的に選択したいノードを宣言

		// 選択ノードのIndexを取得
		private void GetSelNodeIndex()
		{
			// リストを空にする
			listNodeId.Clear();
			Settings.nodeSelPath = "";

			// 選択ノードを取得
			TreeNode tn = treeView1.SelectedNode;

			// 選択ノードのIndexを取得してListに格納
			listNodeId.Add(tn.Index);

			// 親ノードを取得
			TreeNode tnP = tn.Parent;

			// 再帰処理
			GetSelNodeRecursive(tnP);

			// リストを設定用の変数に格納
			for (int i = 0; i < listNodeId.Count; i++)
			{
				Settings.nodeSelPath += listNodeId[i].ToString();

				// 最後じゃなければセパレート記号を追加する
				if (i != listNodeId.Count -1)
				{
					Settings.nodeSelPath += ",";
				}
			}
		}
		// 再帰処理
		private void GetSelNodeRecursive(TreeNode tnP)
		{
			// 親がある場合
			if (tnP != null)
			{
				// 親ノードのIndexを取得してListの先頭に挿入
				listNodeId.Insert(0, tnP.Index);

				// さらに親に対して実行
				GetSelNodeRecursive(tnP.Parent);
			}
		}

		// 選択ノードのIndexを取得
		private void SetSelNodeIndex()
		{
			// リストを空にする
			listNodeId.Clear();

			// リストに設定用の変数からカンマ区切りで格納する
			listNodeId = Settings.nodeSelPath
				.Split(',')
				.ToList()
				.ConvertAll(a => int.Parse(a));

			// リストからトップ階層のノードを取得
			TreeNode tnP = treeView1.Nodes[listNodeId[0]];
			// 最終的に選択したいノードに一旦指定
			selNode = tnP;

			// リストのIndex指定用
			int i = 1;

			// リストに子のIndex情報がある場合
			if (listNodeId.Count > i)
			{
				SetSelNodeRecursive(tnP, i);
			}

			// TreeViewにフォーカスする
			treeView1.Focus();
			// 選択する
			treeView1.SelectedNode = selNode;
		}
		// 再帰処理
		private void SetSelNodeRecursive(TreeNode tnP, int i)
		{
			// リストから1つ下の子ノードを取得
			TreeNode tnC = tnP.Nodes[listNodeId[i]];
			// 最終的に選択したいノードに指定
			selNode = tnC;

			// カウントアップ
			i++;

			// リストに子のIndex情報がある場合
			if (listNodeId.Count > i)
			{
				SetSelNodeRecursive(tnC, i);
			}
		}


		//******************************//
		//								//
		//		　 フォーム関係　		//
		//								//
		//******************************//

		// Form1のコンストラクタ
		public Form1()
		{
			InitializeComponent();
		}

		// 起動時にまず実行する内容
		private void Form1_Load(object sender, EventArgs e)
		{
			// Form端をドラッグ＆ドロップでサイズ変更可能にする
			DAndDSizeChanger sizeChanger = new DAndDSizeChanger(this, this, DAndDArea.All, 8);
			
			// 設定ファイルの読み込み
			if (Settings.LoadSettings() != true)
			{
				// 設定の初期化
				Settings.Initialize();
			}
			else
			{
				// メインウインドウの位置とサイズの復元
				this.Left	= Settings.winPosX;
				this.Top	= Settings.winPosY;
				this.Width	= Settings.winSizeX;
				this.Height	= Settings.winSizeY;

				// SplitContainerの位置の復元
				this.splitContainer1.SplitterDistance = Settings.splitDistance;

				// メインカラーの復元
				panelLabel1.BackColor = Settings.mainColor;
				panelLabel2.BackColor = Settings.mainColor;
				panelLabel3.BackColor = Settings.mainColor;

				// フォントの種類・カラーの復元
				textBox1.Font		= Properties.Settings.Default.FontTB;
				treeView1.Font		= Properties.Settings.Default.FontTV;
				textBox1.ForeColor	= Settings.fontColorTB;
				treeView1.ForeColor	= Settings.fontColorTV;

				// 最前面表示設定の復元
				if (Settings.frontView == true)
				{
					buttonFront.BackgroundImage = Properties.Resources.icon_pin_on;
					this.TopMost = true;
				}

				// ツールチップ表示設定の復元
				toolTip1.Active = Settings.toolTip;

				// テキスト折り返し設定の復元
				if (Settings.wordWrap == true)
				{
					textBox1.WordWrap = true;
				}
				else
				{
					textBox1.WordWrap = false;
				}
			}

			// TextBoxの内容が変化した時のイベントをここでOFFにする(Dirtyマーク用)
			textBox1.TextChanged -= TextBox1_TextChanged;

			// XMLファイルの読み込み
			try
			{
				XmlLoad();

				// 前回終了時に選択していたノードを選択する
				try
				{
					SetSelNodeIndex();
				}
				catch (Exception)
				{
					// TreeViewにフォーカスする
					treeView1.Focus();
				}
			}
			catch (Exception)
			{
				// メッセージフォーム
				var form = new FormMessageBox();
				// 画面の真ん中に表示
				form.StartPosition = FormStartPosition.CenterScreen;

				// メッセージフォームをタスクバーに表示しない
				form.ShowInTaskbar = false;

				// 画像を非表示
				form.pictureBox1.Enabled = false;
				form.pictureBox1.Visible = false;
				// メッセージテキストを差し替え
				form.label1.Text = "ListFusen2 にようこそ！";
				form.label2.Enabled = false;
				form.label2.Visible = false;
				// フォームサイズを小さく
				form.Width = 300;
				form.Height = 100;
				// ボタンのアレンジ
				form.btnOK.Enabled = false;
				form.btnOK.Visible = false;
				form.btnCancel.Text = "OK";

				// メッセージフォームをモーダルで開く
				form.ShowDialog();

				// Disposeでフォームを解放
				form.Dispose();

				// 親ノードを1つ追加
				AddMemParent();
			}

			// オートセーブ用のタイマーを開始
			timerAutoSave = new Timer();
			timerAutoSave.Tick += new EventHandler(doSave);		// イベントを設定
			timerAutoSave.Interval = 60 * 60000;				// 実行間隔 60分
			timerAutoSave.Enabled = false;						// timer.Stop()と同じ

			// オートバックアップ用のタイマーを開始
			timerAutoBackup = new Timer();
			timerAutoBackup.Tick += new EventHandler(doBackup);	// イベントを設定
			timerAutoBackup.Interval = 60 * 60000;              // 実行間隔 60分
			timerAutoBackup.Enabled = true;						// timer.Start()と同じ
		}


		//******************************//
		//								//
		//		　 基本的な機能　		//
		//								//
		//******************************//

		/// <summary>
		/// メモの情報を保持するクラス
		/// </summary>
		public class Memo
		{
			// フィールド
			public string	Text		{ get; set; } = "";	// テキストデータ
			public Point	ScrollPos = new Point(0, 0);	// テキストのキャレットの位置

		}

		// 関数：ルートメモの追加
		private void AddMemParent()
		{
			// ルートノードの数をカウント
			int i = 1 + treeView1.Nodes.Count;

			// 今から作成するノード名
			string s = "Parent Memo " + i.ToString();

			TreeNode tn = new TreeNode(s);	// ノード作成
			var memo = new Memo();		// メモオブジェクトを追加
			tn.Tag = memo;					// ノードにメモオブジェクトを登録
			treeView1.Nodes.Add(tn);		// ノード登録

			// 追加されたノードを選択
			treeView1.SelectedNode = tn;
		}
		// 関数：選択ノードに子メモを追加
		private void AddMemChild()
		{
			// 選択されているノードがあるか判定
			if (treeView1.SelectedNode != null)
			{
				// 同じ親に属するノードの数をカウント
				TreeNode tnP = treeView1.SelectedNode;
				int i = 1 + tnP.Nodes.Count;

				// 今から作成するノード名
				string s = "Child Memo " + i.ToString();

				TreeNode tnC = new TreeNode(s); // ノード作成
				var memo = new Memo();     // メモオブジェクトを追加
				tnC.Tag = memo;				// ノードにメモオブジェクトを登録
				tnP.Nodes.Add(tnC);             // ノード登録
				tnP.Expand(); // 展開
			}
			else
			{
				MessageBox.Show("メモを追加したい場所を選択してください");
			}
		}

		// 関数：選択ノードを上に移動
		private void MoveUpMem()
		{
			// 選択されているノードがあるか判定
			if (treeView1.SelectedNode != null)
			{
				// 選択中のノードを取得
				TreeNode node = treeView1.SelectedNode;

				// 親ノードを取得
				TreeNode parent = node.Parent;
				// 選択中のノード群を取得
				TreeView view = node.TreeView;

				// 親ノードがある場合
				if (parent != null)
				{
					int index = parent.Nodes.IndexOf(node);
					if (index > 0)
					{
						parent.Nodes.RemoveAt(index);
						parent.Nodes.Insert(index - 1, node);

						// 選択する
						treeView1.SelectedNode = node;
					}
				}
				// 親ノードが無い場合
				else if (view != null && view.Nodes.Contains(node)) //root node
				{
					int index = view.Nodes.IndexOf(node);
					if (index > 0)
					{
						view.Nodes.RemoveAt(index);
						view.Nodes.Insert(index - 1, node);

						// 選択する
						treeView1.SelectedNode = node;
					}
				}
			}
			else
			{
				return;
			}
		}
		// 関数：選択ノードを下に移動
		private void MoveDownMem()
		{
			// 選択されているノードがあるか判定
			if (treeView1.SelectedNode != null)
			{
				// 選択中のノードを取得
				TreeNode node = treeView1.SelectedNode;

				// 親ノードを取得
				TreeNode parent = node.Parent;
				// 選択中のノード群を取得
				TreeView view = node.TreeView;

				// 親ノードがある場合
				if (parent != null)
				{
					int index = parent.Nodes.IndexOf(node);
					if (index < parent.Nodes.Count - 1)
					{
						parent.Nodes.RemoveAt(index);
						parent.Nodes.Insert(index + 1, node);

						// 選択する
						treeView1.SelectedNode = node;
					}
				}
				// 親ノードが無い場合
				else if (view != null && view.Nodes.Contains(node)) //root node
				{
					int index = view.Nodes.IndexOf(node);
					if (index < view.Nodes.Count - 1)
					{
						view.Nodes.RemoveAt(index);
						view.Nodes.Insert(index + 1, node);

						// 選択する
						treeView1.SelectedNode = node;
					}
				}
			}
			else
			{
				return;
			}
		}

		// 関数：選択ノードを削除
		private void RemoveMem()
		{
			// 選択されているノードがあるか判定
			if (treeView1.SelectedNode != null)
			{
				// メッセージフォーム
				var form = new FormMessageBox();
				// オーナーウィンドウの真ん中に表示
				form.StartPosition = FormStartPosition.CenterParent;
				// メッセージフォームをタスクバーに表示しない
				form.ShowInTaskbar = false;

				// メッセージフォームをモーダルで開いて何のボタンで終了したかを受け取る
				DialogResult result = form.ShowDialog();

				// OK ボタンで閉じたとき
				if (result == DialogResult.OK)
				{
					// 選択中のノードを削除
					treeView1.SelectedNode.Remove();

					// ノードが全て無くなった場合
					if (treeView1.Nodes.Count == 0)
					{
						// 表示を消す
						textBox1.Text = "";
					}
				}

				// Disposeでフォームを解放
				form.Dispose();
			}
			else
			{
				return;
			}
		}

		// 関数：選択ノードのTextBoxの内容をメモオブジェクトに記録
		private void MemoryText()
		{
			// 選択ノードの取得
			var tn = treeView1.SelectedNode;
			// ノード削除時に例外が発生するのでその対処
			if (tn != null)
			{
				// 選択ノードのメモオブジェクトを取得
				Memo memo = (Memo)tn.Tag;
				// TextBoxの内容をメモオブジェクトに記録
				memo.Text = textBox1.Text;
				// スクロール位置を取得
				memo.ScrollPos = GetTextBoxScrollPos(this.textBox1);
			}
		}
		// 関数：新しく選択したメモの内容をオブジェクトからTextBoxに表示
		private void ShowTextData()
		{
			// 選択ノードの取得
			var tn = treeView1.SelectedNode;
			// 選択ノードのメモオブジェクトを取得
			Memo memo = (Memo)tn.Tag;
			// メモオブジェクトの内容をTextBoxに表示
			textBox1.Text = memo.Text;

			// スクロール位置を復元
			try
			{
				SetTextBoxScrollPos(this.textBox1, memo.ScrollPos);
			}
			catch (Exception)
			{

			}

			// TextBoxの内容が変化した時のイベントをここでONにする(Dirtyマーク用)
			textBox1.TextChanged += TextBox1_TextChanged;
		}

		//**********************************************//
		//				保存関係の関数					//
		//**********************************************//

		// 関数：XMLの読み込み
		private void XmlLoad()
		{
			// XMLファイルを指定
			XElement xml = XElement.Load(Settings.FILEPATH_XML);

			// ルートノードの情報を取得
			IEnumerable<XElement> infos = from item in xml.Elements("Root")
										  select item;

			// ルートノード分ループ
			foreach (XElement info in infos)
			{
				// ラベル名をXMLから取得してルートノード作成
				TreeNode tn = new TreeNode(info.Element("Name").Value);
				// メモオブジェクトを作成
				var memo = new Memo();
				// メモオブジェクトに情報を登録
				string s = info.Element("Text").Value;
				memo.Text = s.Replace(Settings.MarkNewLine, "\r\n");
				string s2 = info.Element("ScrollPos").Value;
				string[] s3 = s2.Split('_');
				memo.ScrollPos = new Point(int.Parse(s3[0]), int.Parse(s3[1]));
				
				// ルートノードにメモオブジェクトを登録
				tn.Tag = memo;
				// ノード登録
				treeView1.Nodes.Add(tn);

				// 再帰処理
				XmlLoadRec(info, tn);
			}
		}
		// 再帰処理
		private void XmlLoadRec(XElement parent, TreeNode tnP)
		{
			// ループ
			foreach (XElement child in parent.Elements("Child"))
			{
				// ラベル名をXMLから取得して親ノード作成
				TreeNode tnC = new TreeNode(child.Element("Name").Value);
				// メモオブジェクトを作成
				var memo = new Memo();
				// メモオブジェクトに情報を登録
				string s = child.Element("Text").Value;
				memo.Text = s.Replace(Settings.MarkNewLine, "\r\n");
				string s2 = child.Element("ScrollPos").Value;
				string[] s3 = s2.Split('_');
				memo.ScrollPos = new Point(int.Parse(s3[0]), int.Parse(s3[1]));

				// 子ノードにメモオブジェクトを登録
				tnC.Tag = memo;
				// ノード登録
				tnP.Nodes.Add(tnC);

				// 自分自身を呼び出す
				XmlLoadRec(child, tnC);
			}
		}
		
		// 関数：XMLの書き出し
		private void XmlSave(string path)
		{
			// XMLファイルの保存
			try
			{
				// XMLの内容の定義
				var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

				// 構造ごとの要素を定義
				XElement Sample = new XElement("TreeViewData");
				xml.Add(Sample);

				// TreeViewのノード情報をnodesコレクションに格納
				TreeNodeCollection nodes = treeView1.Nodes;
				// ループ
				foreach (TreeNode node in nodes)
				{
					// 自分自身のノードを定義  
					XElement n = new XElement("Root");
					xml.Root.Add(n);

					// 再帰処理
					XmlSaveRec(node, n);
				}

				// 変更を保存する
				xml.Save(path);

				// Dirtyマークを消す
				panelDirty.Visible = false;

				// SAVED!ラベルを1秒間表示
				showSaved();
			}
			catch (Exception)
			{
				MessageBox.Show("ツールデータの保存に失敗しました");
			}
		}
		// 再帰処理
		private void XmlSaveRec(TreeNode node, XElement n)
		{
			XElement name = new XElement("Name", node.Text);
			n.Add(name);

			// オブジェクトを取得
			Memo memo = (Memo)node.Tag;
			// 情報を取得
			string s = memo.Text.Replace("\r\n", Settings.MarkNewLine);
			Point p = memo.ScrollPos;
			int pX = p.X;
			int pY = p.Y;

			// 情報のXElementを作成
			XElement nText = new XElement("Text", s);
			XElement nCP = new XElement("ScrollPos", pX.ToString() + "_" + pY.ToString());
			// 情報をXMLに追加
			n.Add(nText);
			n.Add(nCP);

			// ループ
			foreach (TreeNode tn in node.Nodes)
			{
				// 自分自身のノードを定義  
				XElement n2 = new XElement("Child");
				n.Add(n2);

				// 自分自身を呼び出す
				XmlSaveRec(tn, n2);
			}
		}
		
		// 関数：オートセーブ
		private void doSave(object sender, EventArgs e)
		{
			// XMLの出力
			XmlSave(Settings.FILEPATH_XML);
		}
		// 関数「SAVED!」ラベルを1秒間だけ表示
		private async void showSaved()
		{
			this.labelSaved.Visible = true;
			await Task.Run(() => {
				System.Threading.Thread.Sleep(1000);
			});
			this.labelSaved.Visible = false;
		}
		// 関数：オートバックアップ
		private void doBackup(object sender, EventArgs e)
		{
			// 日付を取得
			DateTime dt = DateTime.Now;
			long dateNow = long.Parse(dt.ToString("yyyyMMddHHmm"));
			long datePast = Settings.prevBackup;

			// 出力するかどうか
			bool doBackup = false;

			// バックアップ間隔の設定を確認
			switch (Settings.autoBackup)
			{
				case 0: // なし
					doBackup = false;
					break;

				case 1: // 日
					if (dateNow >= datePast + 10000)
					{
						doBackup = true;
					}
					break;

				case 2: // 週
					if (dateNow >= datePast + 70000)
					{
						doBackup = true;
					}
					break;
				case 3: // 月
					if (dateNow >= datePast + 1000000)
					{
						doBackup = true;
					}
					break;

				default:
					break;
			}

			// バックアップする設定なら実行
			if (doBackup == true)
			{
				// backupフォルダが無ければ作成する
				if (Directory.Exists(Settings.FILEPATH_BACKUP) != true)
				{
					Directory.CreateDirectory(Settings.FILEPATH_BACKUP);
				}

				// XMLファイルをbackupフォルダに出力
				string s = dateNow.ToString();
				XmlSave(Settings.FILEPATH_BACKUP + @"\data" + s + @".xml");

				Settings.prevBackup = dateNow;
			}
		}

		//**********************************************//
		//			　オプション画面の関数　			//
		//**********************************************//

		// 関数：オプション画面を開く
		private void OpenOptionForm()
		{
			// 設定フォーム
			var form = new FormSettings();
			
			//**********************************************//
			//					Formの表示					//
			//**********************************************//

			// オーナーウィンドウの真ん中に表示
			form.StartPosition = FormStartPosition.CenterParent;
			// 設定フォームをタスクバーに表示しない
			form.ShowInTaskbar = false;

			// 設定フォームをモーダルで開いて何のボタンで終了したかを受け取る
			DialogResult result = form.ShowDialog();

			// OK ボタンで閉じたとき
			if (result == DialogResult.OK)
			{
				// 現在の値をSettingsフォームの変数に格納
				Properties.Settings.Default.FontTB = FormSettings.fontTB;
				Properties.Settings.Default.FontTV = FormSettings.fontTV;
				Settings.fontColorTB = FormSettings.fontColorTB;
				Settings.fontColorTV = FormSettings.fontColorTV;
				Settings.wordWrap = FormSettings.wordWrap;
				Settings.mainColor = FormSettings.mainColor;
				Settings.toolTip = FormSettings.toolTip;
				Settings.autoSave = FormSettings.autoSave;
				Settings.autoBackup = FormSettings.autoBackup;
				Settings.deactiveSave = FormSettings.deactiveSave;

				// フォントの設定を更新
				textBox1.Font = Properties.Settings.Default.FontTB;
				treeView1.Font = Properties.Settings.Default.FontTV;
				textBox1.ForeColor = Settings.fontColorTB;
				treeView1.ForeColor = Settings.fontColorTV;

				// パネルのカラーを更新
				panelLabel1.BackColor = Settings.mainColor;
				panelLabel2.BackColor = Settings.mainColor;
				panelLabel3.BackColor = Settings.mainColor;

				// 右端で折り返す設定を更新
				textBox1.WordWrap = Settings.wordWrap;

				// ツールチップの表示を更新
				toolTip1.Active = Settings.toolTip;

				// オートセーブ設定の反映
				switch (Settings.autoSave)
				{
					case 0: // なし
						timerAutoSave.Enabled = false;
						break;

					case 1: // 5
						timerAutoSave.Interval = 5 * 60000;
						timerAutoSave.Enabled = true;
						break;

					case 2: // 10
						timerAutoSave.Interval = 10 * 60000;
						timerAutoSave.Enabled = true;
						break;

					case 3: // 30
						timerAutoSave.Interval = 30 * 60000;
						timerAutoSave.Enabled = true;
						break;

					case 4: // 60
						timerAutoSave.Interval = 60 * 60000;
						timerAutoSave.Enabled = true;
						break;

					default:
						break;			
				}
			}

			// Disposeでフォームを解放
			form.Dispose();
		}


		//******************************//
		//								//
		//　　　　　 イベント 　　　　　//
		//								//
		//******************************//

		// イベント：ノードのラベル名を編集終了したとき
		private void TreeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			// ラベルが変更されたか調べる（e.Labelがnullなら変更なし）
			if (e.Label != null)
			{
				// 同名のノードが同じ階層にあるか調べる
				if (e.Node.Parent == null) // 自身がルートノードの場合
				{
					foreach (TreeNode n in treeView1.Nodes)
					{
						// 同名のノードがあるときは編集をキャンセルする
						if (n != e.Node && n.Text == e.Label)
						{
							ShowMsgErrSameName();

							// 編集をキャンセルして元に戻す
							e.CancelEdit = true;
							return;
						}
					}
				}
				else // 親ノードがある場合
				{
					foreach (TreeNode n in e.Node.Parent.Nodes)
					{
						// 同名のノードがあるときは編集をキャンセルする
						if (n != e.Node && n.Text == e.Label)
						{
							ShowMsgErrSameName();

							// 編集をキャンセルして元に戻す
							e.CancelEdit = true;
							return;
						}
					}
				}

				// ファイル名に使用できない文字をchar配列に格納
				char[] invalidChars = Path.GetInvalidFileNameChars();
				// ノード名に使用できない文字があるかチェック
				if (e.Label.IndexOfAny(invalidChars) >= 0)
				{
					ShowMsgErrInvalidName();

					// 編集をキャンセルして元に戻す
					e.CancelEdit = true;
					return;
				}
			}
		}
		// 関数：エラーメッセージ「同じ場所に同じ名前のメモがあります」
		public static void ShowMsgErrSameName()
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
			form.label1.Text = "同じ場所に同名のアイテムがあります";
			form.label2.Text = "別の名前を指定してください";
			form.label1.ForeColor = Color.FromArgb(191, 92, 55);
			// フォームサイズを伸ばす
			form.Width = 400;
			// ボタンのアレンジ
			form.btnOK.Enabled = false;
			form.btnOK.Visible = false;
			form.btnCancel.Text = "OK";

			// メッセージフォームをモーダルで開いて何のボタンで終了したかを受け取る
			form.ShowDialog();

			// Disposeでフォームを解放
			form.Dispose();
		}
		// 関数：エラーメッセージ「ファイル名に使用できない文字が含まれています」
		public static void ShowMsgErrInvalidName()
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
			form.label1.Text = "ファイル名に使用できない文字を含んでいます";
			form.label2.Text = "別の名前を指定してください";
			form.label1.ForeColor = Color.FromArgb(191, 92, 55);
			// フォームサイズを伸ばす
			form.Width = 400;
			// ボタンのアレンジ
			form.btnOK.Enabled = false;
			form.btnOK.Visible = false;
			form.btnCancel.Text = "OK";

			// メッセージフォームをモーダルで開いて何のボタンで終了したかを受け取る
			form.ShowDialog();

			// Disposeでフォームを解放
			form.Dispose();
		}

		// イベント：TextBoxがアクティブじゃなくなったとき
		//			 TextBox編集後すぐボタンを押しに行くようなときに必要
		private void TextBox1_Leave(object sender, EventArgs e)
		{
			MemoryText();
		}
		// イベント：メモの内容が変わったとき
		private void TextBox1_TextChanged(object sender, EventArgs e)
		{
			// Dirtyマークを表示
			panelDirty.Visible = true;
		}

		// イベント：TreeViewの選択が移ろうとするとき（スクロール位置だけ保存）
		private void TreeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			// TextBoxの内容が変化した時のイベントをここでOFFにする(Dirtyマーク用)
			textBox1.TextChanged -= TextBox1_TextChanged;

			// 選択ノードの取得
			var tn = treeView1.SelectedNode;
			// ノード削除時に例外が発生するのでその対処
			if (tn != null)
			{
				// 選択ノードのメモオブジェクトを取得
				Memo memo = (Memo)tn.Tag;
				// スクロール位置を取得
				memo.ScrollPos = GetTextBoxScrollPos(this.textBox1);
			}
		}
		// イベント：TreeViewの選択が移ったとき（メモを表示）
		private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ShowTextData();
		}

		// ボタン：Up 選択中のノードを上に移動
		private void ButtonUp_MouseClick(object sender, MouseEventArgs e)
		{
			MoveUpMem();
		}
		private void ButtonUp_MouseEnter(object sender, EventArgs e)
		{
			buttonUp.BackgroundImage = Properties.Resources.icon_up_on;
		}
		private void ButtonUp_MouseLeave(object sender, EventArgs e)
		{
			buttonUp.BackgroundImage = Properties.Resources.icon_up;
		}
		// ボタン：Down 選択中のノードを下に移動
		private void ButtonDown_MouseClick(object sender, MouseEventArgs e)
		{
			MoveDownMem();
		}
		private void ButtonDown_MouseEnter(object sender, EventArgs e)
		{
			buttonDown.BackgroundImage = Properties.Resources.icon_down_on;
		}
		private void ButtonDown_MouseLeave(object sender, EventArgs e)
		{
			buttonDown.BackgroundImage = Properties.Resources.icon_down;
		}
		// ボタン：Add Parent ツリーにルートメモの追加
		private void ButtonAddP_MouseClick(object sender, MouseEventArgs e)
		{
			AddMemParent();
		}
		private void ButtonAddP_MouseEnter(object sender, EventArgs e)
		{
			buttonAddP.BackgroundImage = Properties.Resources.icon_add1_on;
		}
		private void ButtonAddP_MouseLeave(object sender, EventArgs e)
		{
			buttonAddP.BackgroundImage = Properties.Resources.icon_add1;
		}
		// ボタン：Add Child ツリーに子メモを追加
		private void ButtonAddC_MouseClick(object sender, MouseEventArgs e)
		{
			AddMemChild();
		}
		private void ButtonAddC_MouseEnter(object sender, EventArgs e)
		{
			buttonAddC.BackgroundImage = Properties.Resources.icon_add2_on;
		}
		private void ButtonAddC_MouseLeave(object sender, EventArgs e)
		{
			buttonAddC.BackgroundImage = Properties.Resources.icon_add2;
		}
		// ボタン：Del 選択ノードの削除
		private void ButtonDel_MouseClick(object sender, MouseEventArgs e)
		{
			RemoveMem();
		}
		private void ButtonDel_MouseEnter(object sender, EventArgs e)
		{
			buttonDel.BackgroundImage = Properties.Resources.icon_delete_on;
		}
		private void ButtonDel_MouseLeave(object sender, EventArgs e)
		{
			buttonDel.BackgroundImage = Properties.Resources.icon_delete;
		}
		// ボタン：Undo アンドゥ
		private void ButtonUndo_MouseClick(object sender, MouseEventArgs e)
		{
			// Undoできるか?
			if (textBox1.CanUndo == true)
			{
				// Undoを実行する
				textBox1.Undo();
			}
		}
		private void ButtonUndo_MouseEnter(object sender, EventArgs e)
		{
			// Undoできるか?
			if (textBox1.CanUndo == true)
			{
				buttonUndo.BackgroundImage = Properties.Resources.icon_undo_on;
			}
		}
		private void ButtonUndo_MouseLeave(object sender, EventArgs e)
		{
			buttonUndo.BackgroundImage = Properties.Resources.icon_undo;
		}
		// ボタン：Settings 設定ウインドウ
		private void ButtonSettings_MouseClick(object sender, MouseEventArgs e)
		{
			OpenOptionForm();
		}
		private void ButtonSettings_MouseEnter(object sender, EventArgs e)
		{
			buttonSettings.BackgroundImage = Properties.Resources.icon_settings_on;
		}
		private void ButtonSettings_MouseLeave(object sender, EventArgs e)
		{
			buttonSettings.BackgroundImage = Properties.Resources.icon_settings;
		}
		// ボタン：Export テキストファイル一括出力
		private void ButtonExport_MouseClick(object sender, MouseEventArgs e)
		{
			// TreeViewのノード情報をnodesコレクションに格納
			TreeNodeCollection nodes = treeView1.Nodes;

			// 一括出力ダイアログを開く
			FormExportAll form = new FormExportAll(nodes);
			// オーナーウィンドウの真ん中に表示
			form.StartPosition = FormStartPosition.CenterParent;
			// オーナーウィンドウにthisを指定する
			form.ShowDialog(this);
			//フォームが必要なくなったところで、Disposeを呼び出す
			form.Dispose();
		}
		private void ButtonExport_MouseEnter(object sender, EventArgs e)
		{
			buttonExport.BackgroundImage = Properties.Resources.icon_export_on;
		}
		private void ButtonExport_MouseLeave(object sender, EventArgs e)
		{
			buttonExport.BackgroundImage = Properties.Resources.icon_export;
		}
		// ボタン：Save
		private void ButtonSave_MouseClick(object sender, MouseEventArgs e)
		{
			// XMLファイルの保存
			XmlSave(Settings.FILEPATH_XML);
		}
		private void ButtonSave_MouseEnter(object sender, EventArgs e)
		{
			buttonSave.BackgroundImage = Properties.Resources.icon_save_on;
		}
		private void ButtonSave_MouseLeave(object sender, EventArgs e)
		{
			buttonSave.BackgroundImage = Properties.Resources.icon_save;
		}
		// イベント：非アクティブ時にセーブ
		private void Form1_Deactivate(object sender, EventArgs e)
		{
			// 非アクティブ時にセーブする設定がON
			if (Settings.deactiveSave == true)
			{
				// XMLファイルの保存
				XmlSave(Settings.FILEPATH_XML);
			}
		}

		// ボタン：Front 最前面表示 ON / OFF のトグル
		private void ButtonFront_MouseClick(object sender, MouseEventArgs e)
		{
			if (Settings.frontView == false)
			{
				buttonFront.BackgroundImage = Properties.Resources.icon_pin_on;
				this.TopMost = true;
				Settings.frontView = true;
			}
			else
			{
				buttonFront.BackgroundImage = Properties.Resources.icon_pin;
				this.TopMost = false;
				Settings.frontView = false;
			}
		}
		
		// ボタン：フォーム最小化
		private void PBoxMinimize_MouseClick(object sender, MouseEventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}
		// ボタン：フォーム終了
		private void PBoxClose_MouseClick(object sender, MouseEventArgs e)
		{
			this.Close();
		}
		// イベント：フォームが閉じるとき
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// ツール設定の出力
			try
			{
				// メインウインドウの位置とサイズの代入
				Settings.winPosX = this.Left;
				Settings.winPosY = this.Top;
				Settings.winSizeX = this.Width;
				Settings.winSizeY = this.Height;

				// SplitContainerの位置の代入
				Settings.splitDistance = this.splitContainer1.SplitterDistance;

				// 最後に選択していたノードのIndexを取得
				GetSelNodeIndex();

				// 設定ファイルの出力
				Settings.OutputSettings();
			}
			catch (Exception)
			{
				MessageBox.Show("ツール設定の出力に失敗しました");
			}

			// XMLファイルの保存
			XmlSave(Settings.FILEPATH_XML);
		}

		// ショートカットキー：TextBox
		private void TextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			// 保存
			if (e.Control && e.KeyCode == Keys.S)
			{
				// 選択ノードの取得
				var tn = treeView1.SelectedNode;
				// ノード削除時に例外が発生するのでその対処
				if (tn != null)
				{
					// 選択ノードのメモオブジェクトを取得
					Memo memo = (Memo)tn.Tag;
					// メモの内容を保存
					memo.Text = this.textBox1.Text;
				}

				// XMLファイルに保存
				XmlSave(Settings.FILEPATH_XML);
				e.SuppressKeyPress = true;
			}

			// テキスト全選択
			if (e.Control && e.KeyCode == Keys.A)
			{
				textBox1.SelectAll();
				e.SuppressKeyPress = true;
			}
		}
		// ショートカットキー：TreeView
		private void TreeView1_KeyDown(object sender, KeyEventArgs e)
		{
			TreeView tv = (TreeView)sender;

			// F2キーでラベル名の編集を開始
			if (e.KeyCode == Keys.F2 && tv.SelectedNode != null && tv.LabelEdit)
			{
				tv.SelectedNode.BeginEdit();
			}

			// 選択ノードの削除
			if (e.KeyData == Keys.Delete)
			{
				RemoveMem();
				e.SuppressKeyPress = true;
			}
		}


		/// <summary>
		/// コントロールの端をD＆Dすることによってサイズを変更出来る機能を提供するクラス
		/// こちらの記事のコードを利用させていただいています
		/// http://anis774.net/codevault/danddsizechanger.html
		/// </summary>
		class DAndDSizeChanger
		{
			Control mouseListner;
			Control sizeChangeCtrl;
			DAndDArea sizeChangeArea;
			Size lastMouseDownSize;
			Point lastMouseDownPoint;
			DAndDArea status;
			int sizeChangeAreaWidth;
			Cursor defaultCursor;

			/// <param name="mouseListner">マウス入力を受け取るコントロール</param>
			/// <param name="sizeChangeCtrl">マウス入力によってサイズが変更されるコントロール</param>
			/// <param name="sizeChangeArea">上下左右のサイズ変更が有効になる範囲を指定</param>
			/// <param name="sizeChangeAreaWidth">サイズ変更が有効になる範囲の幅を指定</param>
			public DAndDSizeChanger(Control mouseListner, Control sizeChangeCtrl, DAndDArea sizeChangeArea, int sizeChangeAreaWidth)
			{
				this.mouseListner = mouseListner;
				this.sizeChangeCtrl = sizeChangeCtrl;
				this.sizeChangeAreaWidth = sizeChangeAreaWidth;
				this.sizeChangeArea = sizeChangeArea;
				defaultCursor = mouseListner.Cursor;

				mouseListner.MouseDown += new MouseEventHandler(mouseListner_MouseDown);
				mouseListner.MouseMove += new MouseEventHandler(mouseListner_MouseMove);
				mouseListner.MouseUp += new MouseEventHandler(mouseListner_MouseUp);
				// マウスカーソルが通常に戻らない現象回避のために追加
				mouseListner.MouseLeave += new EventHandler(MouseListner_MouseLeave);
			}

			// 関数：マウスカーソルが通常に戻らない現象回避のために追加
			private void MouseListner_MouseLeave(object sender, EventArgs e)
			{
				mouseListner.Cursor = defaultCursor;
			}

			void mouseListner_MouseDown(object sender, MouseEventArgs e)
			{
				lastMouseDownPoint = e.Location;
				lastMouseDownSize = sizeChangeCtrl.Size;

				//動作を決定
				status = DAndDArea.None;
				if (getTop().Contains(e.Location))
				{
					status |= DAndDArea.Top;
				}
				if (getLeft().Contains(e.Location))
				{
					status |= DAndDArea.Left;
				}
				if (getBottom().Contains(e.Location))
				{
					status |= DAndDArea.Bottom;
				}
				if (getRight().Contains(e.Location))
				{
					status |= DAndDArea.Right;
				}

				if (status != DAndDArea.None)
				{
					mouseListner.Capture = true;
				}
			}

			void mouseListner_MouseMove(object sender, MouseEventArgs e)
			{
				//カーソルを変更
				if ((getTop().Contains(e.Location) &&
					getLeft().Contains(e.Location)) ||
					(getBottom().Contains(e.Location) &&
					getRight().Contains(e.Location)))
				{

					mouseListner.Cursor = Cursors.SizeNWSE;
				}
				else if ((getTop().Contains(e.Location) &&
				  getRight().Contains(e.Location)) ||
				  (getBottom().Contains(e.Location) &&
				  getLeft().Contains(e.Location)))
				{

					mouseListner.Cursor = Cursors.SizeNESW;
				}
				else if (getTop().Contains(e.Location) ||
				  getBottom().Contains(e.Location))
				{

					mouseListner.Cursor = Cursors.SizeNS;
				}
				else if (getLeft().Contains(e.Location) ||
				  getRight().Contains(e.Location))
				{

					mouseListner.Cursor = Cursors.SizeWE;
				}
				else
				{
					mouseListner.Cursor = defaultCursor;
				}

				if (e.Button == MouseButtons.Left)
				{
					int diffX = e.X - lastMouseDownPoint.X;
					int diffY = e.Y - lastMouseDownPoint.Y;

					if ((status & DAndDArea.Top) == DAndDArea.Top)
					{
						int h = sizeChangeCtrl.Height;
						sizeChangeCtrl.Height -= diffY;
						sizeChangeCtrl.Top += h - sizeChangeCtrl.Height;
					}
					if ((status & DAndDArea.Bottom) == DAndDArea.Bottom)
					{
						sizeChangeCtrl.Height = lastMouseDownSize.Height + diffY;
					}
					if ((status & DAndDArea.Left) == DAndDArea.Left)
					{
						int w = sizeChangeCtrl.Width;
						sizeChangeCtrl.Width -= diffX;
						sizeChangeCtrl.Left += w - sizeChangeCtrl.Width;
					}
					if ((status & DAndDArea.Right) == DAndDArea.Right)
					{
						sizeChangeCtrl.Width = lastMouseDownSize.Width + diffX;
					}
				}
			}

			void mouseListner_MouseUp(object sender, MouseEventArgs e)
			{
				mouseListner.Capture = false;
			}

			/// <summary>
			/// ポイントがD＆Dするとサイズが変更されるエリア内にあるかどうかを判定します。
			/// </summary>
			public bool ContainsSizeChangeArea(Point p)
			{
				return getTop().Contains(p) ||
					getBottom().Contains(p) ||
					getLeft().Contains(p) ||
					getRight().Contains(p);
			}

			private Rectangle getTop()
			{
				if ((sizeChangeArea & DAndDArea.Top) == DAndDArea.Top)
				{
					return new Rectangle(0, 0, mouseListner.Width, sizeChangeAreaWidth);
				}
				else
				{
					return new Rectangle();
				}
			}

			private Rectangle getBottom()
			{
				if ((sizeChangeArea & DAndDArea.Bottom) == DAndDArea.Bottom)
				{
					return new Rectangle(0, mouseListner.Height - sizeChangeAreaWidth,
						mouseListner.Width, sizeChangeAreaWidth);
				}
				else
				{
					return new Rectangle();
				}
			}

			private Rectangle getLeft()
			{
				if ((sizeChangeArea & DAndDArea.Left) == DAndDArea.Left)
				{
					return new Rectangle(0, 0,
						sizeChangeAreaWidth, mouseListner.Height);
				}
				else
				{
					return new Rectangle();
				}
			}

			private Rectangle getRight()
			{
				if ((sizeChangeArea & DAndDArea.Right) == DAndDArea.Right)
				{
					return new Rectangle(mouseListner.Width - sizeChangeAreaWidth, 0,
						sizeChangeAreaWidth, mouseListner.Height);
				}
				else
				{
					return new Rectangle();
				}
			}
		}
		// DAndDSizeChangerクラス用の列挙体
		public enum DAndDArea
		{
			None = 0,
			Top = 1,
			Bottom = 2,
			Left = 4,
			Right = 8,
			All = 15
		}

		// フォームのドラッグ＆ドロップ移動
		private void PanelButton_MouseDown(object sender, MouseEventArgs e) { MDown(e); }
		private void PanelButton_MouseMove(object sender, MouseEventArgs e) { MMove(e); }
		private void PanelUP_MouseDown(object sender, MouseEventArgs e) { MDown(e); }
		private void PanelUP_MouseMove(object sender, MouseEventArgs e) { MMove(e); }
		private void LabelTitle_MouseDown(object sender, MouseEventArgs e) { MDown(e); }
		private void LabelTitle_MouseMove(object sender, MouseEventArgs e) { MMove(e); }
		private void MDown(MouseEventArgs e)
		{
			// 判定 押されたボタンはマウスの左ボタン？
			if (e.Button == MouseButtons.Left)
			{
				// Yesの場合
				this.mouseX = e.X;  // マウスX座標を記憶
				this.mouseY = e.Y;  // マウスY座標を記憶
			}
		}
		private void MMove(MouseEventArgs e)
		{
			// 判定:押されたボタンはマウスの左ボタン？
			if (e.Button == MouseButtons.Left)
			{
				this.Left += e.X - mouseX;  // フォームのX座標を更新
				this.Top += e.Y - mouseY;   // フォームのY座標を更新
			}
		}
	}
}

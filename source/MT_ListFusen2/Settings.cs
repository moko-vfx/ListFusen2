using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MT_ListFusen2
{
	class Settings
	{
		//******************************//
		//								//
		//		　　ツール設定　		//
		//								//
		//******************************//

		// 定数
		public static string FILEPATH_INI = @"settings.txt";
		public static string FILEPATH_XML = @"data.xml";
		public static string FILEPATH_BACKUP = @"backup";

		// 変数
		public static string nodeSelPath;	// 前回の終了時にどのノードを選択していたか

		// 変数：フォームの位置と大きさ
		public static int winPosX;
		public static int winPosY;
		public static int winSizeX;
		public static int winSizeY;
		public static int splitDistance;    // SplitContainerの位置

		// 変数：カラー
		public static Color mainColor;

		// 変数：フォントカラー
		public static Color	fontColorTB;
		public static Color	fontColorTV;

		// 変数：フラグ
		public static bool	frontView;		// 最前面表示するか
		public static bool	toolTip;		// ToolTipを表示するか
		public static bool	wordWrap;		// 折り返しするか
		public static int	autoSave;		// オートセーブの設定
		public static bool	deactiveSave;	// 非アクティブ時のセーブをするか
		public static int	autoBackup;		// オートバックアップするか
		public static long	prevBackup;		// 前回のバックアップ日時

		// 変数：特殊記号
		public static string MarkNewLine = "[:NL:]";    // 改行記号

		// 関数：初期化
		public static void Initialize()
		{
			nodeSelPath = null;

			winPosX = 0;
			winPosY = 0;
			winSizeX = 668;
			winSizeY = 437;
			splitDistance = 181;

			mainColor = Color.FromArgb(191,92,55);

			fontColorTB = Color.FromArgb(190, 185, 180);
			fontColorTV = Color.FromArgb(190, 185, 180);

			frontView = false;
			toolTip = true;			
			wordWrap = false;		
			autoSave = 0;				
			deactiveSave = false;	
			autoBackup = 3;			
			prevBackup = 201801010101;

			Properties.Settings.Default.FontTB = new Font("Meiryo", 9.5F);
			Properties.Settings.Default.FontTV = new Font("Meiryo", 10F);
		}


		//******************************//
		//								//
		//		　 ツールの保存　		//
		//								//
		//******************************//

		// 関数：ツール設定を出力する
		public static void OutputSettings()
		{
			// 出力設定
			StreamWriter sw = new StreamWriter(
				FILEPATH_INI,
				false,
				Encoding.Default);

			// 改行記号
			string rn = "\r\n";

			// 1行分のデータ
			string data = "";

			// 1行に連結
			data =
				"[Slct] " + nodeSelPath + rn +

				"[PosX] " + winPosX.ToString() + rn +
				"[PosY] " + winPosY.ToString() + rn +
				"[SizX] " + winSizeX.ToString() + rn +
				"[SizY] " + winSizeY.ToString() + rn +
				"[Splt] " + splitDistance.ToString() + rn +

				"[ColR] " + mainColor.R.ToString() + rn +
				"[ColG] " + mainColor.G.ToString() + rn +
				"[ColB] " + mainColor.B.ToString() + rn +

				"[TBCR] " + fontColorTB.R.ToString() + rn +
				"[TBCG] " + fontColorTB.G.ToString() + rn +
				"[TBCB] " + fontColorTB.B.ToString() + rn +
				"[TVCR] " + fontColorTV.R.ToString() + rn +
				"[TVCG] " + fontColorTV.G.ToString() + rn +
				"[TVCB] " + fontColorTV.B.ToString() + rn +

				"[Frnt] " + frontView.ToString() + rn +
				"[Tips] " + toolTip.ToString() + rn +
				"[Wrap] " + wordWrap.ToString() + rn +
				"[AtSv] " + autoSave.ToString() + rn +
				"[DeSv] " + deactiveSave.ToString() + rn +
				"[AtBk] " + autoBackup.ToString() + rn +
				"[PrBk] " + prevBackup.ToString();

			// 書き込む
			sw.WriteLine(data);

			sw.Close();
		}

		// 関数：ツール設定を読み込む
		public static List<string> InputSettings()
		{
			// ツール設定ファイルがあるなら List に加えて返す
			if (File.Exists(FILEPATH_INI))
			{
				// List
				List<string> list = new List<string>();

				// 入力設定
				using (StreamReader sr = new StreamReader(
					FILEPATH_INI,
					Encoding.Default))
				{
					string s = "";

					// 1行ずつ読み込んで、末端(何もない行)まで繰り返す
					while ((s = sr.ReadLine()) != null)
					{
						// 先頭の7文字を削除する
						s = s.Remove(0, 7);

						list.Add(s);
					}
				}

				return (list);
			}
			// ツール設定ファイルが無ければ Null を返す
			else
			{
				return (null);
			}
		}

		// 関数：ツール設定を反映
		public static bool LoadSettings()
		{
			try
			{
				// ツール設定を読み込む
				List<string> list = new List<string>();
				list = InputSettings();

				// 設定ファイル読み込み失敗時
				if (list != null)
				{
					// 各パラメータの復元（例外処理あり）
					TrySetSettings(list);

					// 読み込み成功
					return (true);
				}
				else
				{
					// 読み込み失敗　※初期化
					Initialize();

					return (false);
				}
			}
			catch (Exception)
			{
				// 読み込み失敗　※初期化
				Initialize();
				MessageBox.Show("設定ファイルの読み込みに失敗しました");

				return (false);
			}
		}

		// 関数：ツール設定からパラメータを復元（例外処理付き）
		public static void TrySetSettings(List<string> list)
		{
			// 0 前回終了時の選択メモ
			try
			{
				nodeSelPath = list[0];
			}
			catch (Exception)
			{
				ShowMsgErrSettings("前回終了時の選択メモ");
				nodeSelPath = null;
			}
			// 1～4 ウインドウの表示位置と大きさ
			try
			{
				// 0以下なら0にする X
				if (int.Parse(list[1]) < 0)
				{
					winPosX = 0;
				}
				else
				{
					winPosX = int.Parse(list[1]);
				}
				// 0以下なら0にする Y
				if (int.Parse(list[2]) < 0)
				{
					winPosY = 0;
				}
				else
				{
					winPosY = int.Parse(list[2]);
				}
				// 526以下なら526にする X
				if (int.Parse(list[3]) < 526)
				{
					winSizeX = 526;
				}
				else
				{
					winSizeX = int.Parse(list[3]);
				}
				// 220以下なら220にする Y
				if (int.Parse(list[4]) < 220)
				{
					winSizeY = 220;
				}
				else
				{
					winSizeY = int.Parse(list[4]);
				}
			}
			catch (Exception)
			{
				ShowMsgErrSettings("ウインドウの表示位置と大きさ");
				winPosX = 0;
				winPosY = 0;
				winSizeX = 668;
				winSizeY = 437;
			}
			// 5 画面分割ラインの位置
			try
			{
				if (int.Parse(list[5]) < 50)
				{
					splitDistance = 50;
				}
				else
				{
					splitDistance = int.Parse(list[5]);
				}
			}
			catch (Exception)
			{
				ShowMsgErrSettings("画面分割ラインの位置");
				splitDistance = 181;
			}
			// 6～8 ツールのメインカラー
			try
			{
				mainColor = Color.FromArgb(byte.Parse(list[6]), byte.Parse(list[7]), byte.Parse(list[8]));
			}
			catch (Exception)
			{
				ShowMsgErrSettings("ウインドウ端のカラー");
				mainColor = Color.FromArgb(191, 92, 55);
			}
			// 9～11 メモのフォントカラー
			try
			{
				fontColorTB = Color.FromArgb(byte.Parse(list[9]), byte.Parse(list[10]), byte.Parse(list[11]));
			}
			catch (Exception)
			{
				ShowMsgErrSettings("メモのフォントカラー");
				fontColorTB = Color.FromArgb(190, 185, 180);
			}
			// 12～14 ツリーのフォントカラー
			try
			{
				fontColorTV = Color.FromArgb(byte.Parse(list[12]), byte.Parse(list[13]), byte.Parse(list[14]));
			}
			catch (Exception)
			{
				ShowMsgErrSettings("ツリーのフォントカラー");
				fontColorTV = Color.FromArgb(190, 185, 180);
			}
			// 15 最前面表示
			try
			{
				frontView = bool.Parse(list[15]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("最前面表示");
				frontView = false;
			}
			// 16 バルーンヘルプの表示
			try
			{
				toolTip = bool.Parse(list[16]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("バルーンヘルプの表示");
				toolTip = true;
			}
			// 17 テキストの折り返し
			try
			{
				wordWrap = bool.Parse(list[17]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("テキストの折り返し");
				wordWrap = false;
			}
			// 18 オートセーブ
			try
			{
				autoSave = int.Parse(list[18]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("オートセーブ");
				autoSave = 0;
			}
			// 19 非アクティブ時のセーブ
			try
			{
				deactiveSave = bool.Parse(list[19]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("非アクティブ時のセーブ");
				deactiveSave = false;
			}
			// 20 オートバックアップ
			try
			{
				autoBackup = int.Parse(list[20]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("オートバックアップ");
				autoBackup = 3;
			}
			// 21 前回のバックアップ日時
			try
			{
				prevBackup = long.Parse(list[21]);
			}
			catch (Exception)
			{
				ShowMsgErrSettings("前回のバックアップ日時");
				prevBackup = 201801010101;
			}
		}

		// 関数：エラーメッセージ「設定ファイルに使用できない数値が含まれています」
		public static void ShowMsgErrSettings(string s)
		{
			// メッセージフォーム
			var form = new FormMessageBox();
			// 画面の真ん中に表示
			form.StartPosition = FormStartPosition.CenterScreen;

			// メッセージフォームをタスクバーに表示しない
			form.ShowInTaskbar = false;

			// 画像を×マークに差し替え
			form.pictureBox1.BackgroundImage = Properties.Resources.icon_alert_error;
			// メッセージテキストを差し替え
			form.label1.Text = "設定ファイルの \" " + s + " \" に";
			form.label2.Text = "使用できない値が含まれていたので、デフォルト値を設定します";
			form.label1.ForeColor = Color.FromArgb(191, 92, 55);
			// フォームサイズを伸ばす
			form.Width = 500;
			// ボタンのアレンジ
			form.btnOK.Enabled = false;
			form.btnOK.Visible = false;
			form.btnCancel.Text = "OK";
			
			// メッセージフォームをモーダルで開いて何のボタンで終了したかを受け取る
			form.ShowDialog();

			// Disposeでフォームを解放
			form.Dispose();
		}
	}
}

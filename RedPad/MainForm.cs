using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ScintillaNET;
using RedPad.Utils;

namespace RedPad
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

        #region private variables
        ScintillaNET.Scintilla TextArea;
		bool _fileChanged = false;
		string _currentFilePath = "";
		string _asmKeywordSet0 = "db dw dd dq ah al ax eax rax bh bl bx ebx rbx ch cl cx ecx rcx dh dl dx edx rdx si esi rsi di edi rdi bp ebp rbp sp esp rsp";
		string _asmKeywordSet1 = "times call cmp int include org push pop pusha popa ret mov add div mul jmp je jne jg jge ja jae jl jle jz jnz jc jnc lods lodsb lodsw stos stosb stosw";
		string _cKeywordSet0 = "void int long short float double char short signed unsigned register";
		string _cKeywordSet1 = "auto break case const continue default do else enum extern for goto if return sizeof static struct switch typedef union volatile while";
        #endregion
        private void MainForm_Load(object sender, EventArgs e)
		{

			// CREATE CONTROL
			TextArea = new ScintillaNET.Scintilla();
			TextArea.BorderStyle = BorderStyle.None;
			TextPanel.Controls.Add(TextArea);

			// BASIC CONFIG
			TextArea.Dock = System.Windows.Forms.DockStyle.Fill;
			TextArea.TextChanged += (this.OnTextChanged);

			// INITIAL VIEW CONFIG
			TextArea.WrapMode = WrapMode.None;
			TextArea.IndentationGuides = IndentView.LookBoth;

			// STYLING
			InitColors();
			InitDefaultSyntaxColoring();

			// NUMBER MARGIN
			InitNumberMargin();

			// CODE FOLDING MARGIN
			InitCodeFolding();

			// DRAG DROP
			InitDragDropFile();

			// CLEAR HOTKEYS
			ClearHotkeys();

			// POPULATE LANGUAGE COMBO BOX
			PopulateLanguageDropBox();
		}

		private bool WarningUnsavedChanges()
		{
			string message = "There are unsaved changes! Proceed?";
			string caption = "Unsaved Changes";
			MessageBoxButtons buttons = MessageBoxButtons.YesNo;
			DialogResult result;

			// Displays the MessageBox.
			result = MessageBox.Show(message, caption, buttons);
			if (result == DialogResult.Yes)
			{
				return true;
			}
			return false;
		}

		private void SetLexer(string lang)
        {
			switch (lang)
            {
				case "asm":
					InitAsmSyntaxColoring();
					break;
				case "c":
					InitCppSyntaxColoring();
					break;
				case "none":
				default:
					TextArea.Lexer = Lexer.Null;
					TextArea.SetKeywords(0, "");
					TextArea.SetKeywords(1, "");
					break;
			}
        }

		private void PopulateLanguageDropBox()
        {
			/*
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.lang");
			foreach (var langFile in files)
            {
				var name = Path.GetFileNameWithoutExtension(langFile);
				cmb_language.Items.Add(name);
			}
			*/
			cmb_language.Items.Add("none");
			cmb_language.Items.Add("asm");
			cmb_language.Items.Add("c");
			cmb_language.SelectedItem = "none";
		}

		private void InitColors()
		{
			TextArea.SetSelectionBackColor(true, IntToColor(0x999999));
			TextArea.CaretForeColor = IntToColor(CARET_COLOR);
		}

		private void InitDefaultSyntaxColoring()
		{
			// Configure the default style
			TextArea.StyleResetDefault();
			TextArea.Styles[Style.Default].Font = "Courier New";
			TextArea.Styles[Style.Default].Size = 10;
			TextArea.Styles[Style.Default].BackColor = IntToColor(0x212121);
			TextArea.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
			TextArea.StyleClearAll();
		}
		
		private void InitAsmSyntaxColoring()
		{
			// Configure the Asm lexer styles
			TextArea.Styles[Style.Asm.Comment].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Asm.Number].ForeColor = IntToColor(0xb5f0a5);
			TextArea.Styles[Style.Asm.MathInstruction].ForeColor = IntToColor(0x84b6e3);
			TextArea.Styles[Style.Asm.String].ForeColor = IntToColor(0xf7bf74);
			TextArea.Styles[Style.Asm.Character].ForeColor = IntToColor(0xf7bf74);
			TextArea.Styles[Style.Asm.CpuInstruction].ForeColor = IntToColor(0xd68ef5);
			TextArea.Styles[Style.Asm.Register].ForeColor = IntToColor(0x84b6e3);
			TextArea.Styles[Style.Asm.Operator].ForeColor = IntToColor(0xe0e0e0);
			TextArea.Styles[Style.Asm.Identifier].ForeColor = IntToColor(0xf0f0f0);
			

			TextArea.Lexer = Lexer.Asm;
			TextArea.SetKeywords(0, _asmKeywordSet0);
			TextArea.SetKeywords(1, _asmKeywordSet1);
		}
		
		private void InitCppSyntaxColoring()
		{
			// Configure the CPP (C) lexer styles
			TextArea.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xf0f0f0);
			TextArea.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0x808080);
			TextArea.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xb5f0a5);
			TextArea.Styles[Style.Cpp.String].ForeColor = IntToColor(0xf7bf74);
			TextArea.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xf7bf74);
			TextArea.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
			TextArea.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
			TextArea.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
			TextArea.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
			TextArea.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

			TextArea.Lexer = Lexer.Cpp;
			TextArea.SetKeywords(0, _cKeywordSet0);
			TextArea.SetKeywords(1, _cKeywordSet1);

			InitCodeFolding();
		}

		private void ClearHotkeys()
		{
			// remove conflicting hotkeys from scintilla
			TextArea.ClearCmdKey(Keys.Control | Keys.F);
			TextArea.ClearCmdKey(Keys.Control | Keys.R);
			TextArea.ClearCmdKey(Keys.Control | Keys.H);
			TextArea.ClearCmdKey(Keys.Control | Keys.L);
			TextArea.ClearCmdKey(Keys.Control | Keys.U);
			TextArea.ClearCmdKey(Keys.Control | Keys.K);
			TextArea.ClearCmdKey(Keys.Control | Keys.S);
			TextArea.ClearCmdKey(Keys.Control | Keys.D);
			TextArea.ClearCmdKey(Keys.Control | Keys.G);
			TextArea.ClearCmdKey(Keys.Control | Keys.B);
			TextArea.ClearCmdKey(Keys.Control | Keys.N);
		}

		// Workaround for missing Scintilla.SupressControlCharacters
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.F))
			{
				OpenSearch();
				return true;
			}
			else if (keyData == (Keys.Control | Keys.S))
			{
				SaveFile();
				return true;
			}
			else if (keyData == (Keys.Control | Keys.O))
			{
				OpenFile();
				return true;
			}
			else if (keyData == (Keys.Control | Keys.N))
			{
				NewFile();
				return true;
			}
			else if (keyData == (Keys.Control | Keys.R))
			{
				RunFile();
				return true;
			}
			else if (keyData == (Keys.Escape))
			{
				CloseSearch();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			_fileChanged = true;
		}

		private void cmb_language_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetLexer(cmb_language.SelectedItem.ToString());
		}


		#region Numbers, Code Folding

		/// <summary>
		/// the background color of the text area
		/// </summary>
		private const int BACK_COLOR = 0x2A211C;

		/// <summary>
		/// default text color of the text area
		/// </summary>
		private const int FORE_COLOR = 0xB7B7B7;

		/// <summary>
		/// default caret color of the text area
		/// </summary>
		private const int CARET_COLOR = 0xB7B7B7;

		/// <summary>
		/// change this to whatever margin you want the line numbers to show in
		/// </summary>
		private const int NUMBER_MARGIN = 1;

		/// <summary>
		/// change this to whatever margin you want the code folding tree (+/-) to show in
		/// </summary>
		private const int FOLDING_MARGIN = 2;

		/// <summary>
		/// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
		/// </summary>
		private const bool CODEFOLDING_CIRCULAR = false;

		private void InitNumberMargin()
		{

			TextArea.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
			TextArea.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
			TextArea.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
			TextArea.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

			var nums = TextArea.Margins[NUMBER_MARGIN];
			nums.Width = 30;
			nums.Type = MarginType.Number;
			nums.Sensitive = true;
			nums.Mask = 0;
		}

		private void InitCodeFolding()
		{

			TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
			TextArea.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));

			// Enable code folding
			TextArea.SetProperty("fold", "1");
			TextArea.SetProperty("fold.compact", "1");

			// Configure a margin to display folding symbols
			TextArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
			TextArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
			TextArea.Margins[FOLDING_MARGIN].Sensitive = true;
			TextArea.Margins[FOLDING_MARGIN].Width = 20;

			// Set colors for all folding markers
			for (int i = 25; i <= 31; i++)
			{
				TextArea.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
				TextArea.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
			}

			// Configure folding markers with respective symbols
			TextArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
			TextArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
			TextArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
			TextArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
			TextArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
			TextArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
			TextArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

			// Enable automatic folding
			TextArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

		}

		#endregion

		#region Drag & Drop File

		public void InitDragDropFile()
		{

			TextArea.AllowDrop = true;
			TextArea.DragEnter += delegate (object sender, DragEventArgs e) {
				if (e.Data.GetDataPresent(DataFormats.FileDrop))
					e.Effect = DragDropEffects.Copy;
				else
					e.Effect = DragDropEffects.None;
			};
			TextArea.DragDrop += delegate (object sender, DragEventArgs e)
			{
				bool result = true;
				if (_fileChanged)
					result = WarningUnsavedChanges();

				if (result)
				{
					// get file drop
					if (e.Data.GetDataPresent(DataFormats.FileDrop))
					{

						Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
						if (a != null)
						{
							string path = a.GetValue(0).ToString();
							LoadDataFromFile(path);
							_fileChanged = false;
						}
					}
				}
			};
		}

        #endregion

        #region Load Save Run Data
        private void LoadDataFromFile(string path)
		{
			if (File.Exists(path))
			{
				_currentFilePath = path;
				this.Text = _currentFilePath;
				TextArea.Text = File.ReadAllText(path);
				_fileChanged = false;
				var lang = Path.GetExtension(_currentFilePath).ToLower();
				lang = lang.Replace(".", "");
				switch (lang)
                {
					case "asm":
					case "inc":
						cmb_language.SelectedItem = "asm";
						SetLexer("asm");
						break;
					case "c":
					case "cpp":
					case "h":
					case "hpp":
						cmb_language.SelectedItem = "c";
						SetLexer("c");
						break;
					default:
						cmb_language.SelectedItem = "none";
						SetLexer("none");
						break;
				}
				
			}
		}

		private void SaveDataToFile(string path)
		{
			Stream stream;

			if ((stream = saveFileDialog.OpenFile()) != null)
			{
				_currentFilePath = path;
				this.Text = _currentFilePath;
				var enc = new System.Text.ASCIIEncoding();
				stream.Write(enc.GetBytes(TextArea.Text), 0, TextArea.TextLength);
				stream.Close();
			}
		}

		private void SaveFile()
        {
			if (_currentFilePath == "")
			{
				SaveFileAs();
			}
			else
			{
				File.WriteAllText(_currentFilePath, TextArea.Text);
				_fileChanged = false;
			}
		}

		private void SaveFileAs()
        {
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				SaveDataToFile(saveFileDialog.FileName);
				_fileChanged = false;
			}
		}

		private void NewFile()
        {
			bool result = true;
			if (_fileChanged)
				result = WarningUnsavedChanges();

			if (result)
			{
				TextArea.Text = "";
				_currentFilePath = "";
				_fileChanged = false;
				this.Text = "NEW FILE";
			}
		}

		private void OpenFile()
        {
			bool result = true;
			if (_fileChanged)
				result = WarningUnsavedChanges();

			if (result)
			{
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					LoadDataFromFile(openFileDialog.FileName);
					_fileChanged = false;
				}
			}
		}

		private void RunFile()
        {
			if (_currentFilePath == "")
            {
				string message = "There is nothing to run";
				string caption = "Run File";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBox.Show(message, caption, buttons);
			}
			else
            {
				bool result = true;
				if (_fileChanged)
					result = WarningUnsavedChanges();

				if (result)
				{
					var filename = Path.GetFileName(_currentFilePath);
					var fullPath = Path.GetFullPath(_currentFilePath);
					var workDir = fullPath.Replace(filename, "");
					if (!File.Exists(workDir+"task.bat"))
                    {
						string message = "'task.bat' not found. Create one?";
						string caption = "Run File";
						MessageBoxButtons buttons = MessageBoxButtons.YesNo;
						var rslt = MessageBox.Show(message, caption, buttons);

						if (rslt == DialogResult.Yes)
                        {
							_currentFilePath = workDir + "task.bat";
							this.Text = _currentFilePath;
							TextArea.Text = "@echo off\ncd %~dp0\n\n\n:: Enter your commands here\n\n\npause";
							_fileChanged = true;
						}
					}
					else
                    {
						System.Diagnostics.Process.Start(workDir + "task.bat");
					}
				}
			}
        }
		#endregion

		#region Main Menu Commands

		private void btn_new_Click(object sender, EventArgs e)
		{
			NewFile();
		}

		private void btn_open_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		private void btn_save_Click(object sender, EventArgs e)
		{
			SaveFile();
		}

		private void btn_save_as_Click(object sender, EventArgs e)
		{
			SaveFileAs();
		}

		private void btn_run_Click(object sender, EventArgs e)
		{
			RunFile();
		}

		#endregion

		#region Quick Search Bar

		bool SearchIsOpen = false;

		private void OpenSearch()
		{

			SearchManager.SearchBox = TxtSearch;
			SearchManager.TextArea = TextArea;

			if (!SearchIsOpen)
			{
				SearchIsOpen = true;
				InvokeIfNeeded(delegate () {
					PanelSearch.Visible = true;
					TxtSearch.Text = SearchManager.LastSearch;
					TxtSearch.Focus();
					TxtSearch.SelectAll();
				});
			}
			else
			{
				InvokeIfNeeded(delegate () {
					TxtSearch.Focus();
					TxtSearch.SelectAll();
				});
			}
		}
		private void CloseSearch()
		{
			if (SearchIsOpen)
			{
				SearchIsOpen = false;
				InvokeIfNeeded(delegate () {
					PanelSearch.Visible = false;
				});
			}
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
			OpenSearch();
		}

		private void BtnPrevSearch_Click(object sender, EventArgs e)
		{
			SearchManager.Find(false, false);
		}
		private void BtnNextSearch_Click(object sender, EventArgs e)
		{
			SearchManager.Find(true, false);
		}
		private void BtnCloseSearch_Click(object sender, EventArgs e)
		{
			CloseSearch();
		}
		private void TxtSearch_TextChanged(object sender, EventArgs e)
		{
			SearchManager.Find(true, true);
		}

		#endregion

		#region Utils

		public static Color IntToColor(int rgb)
		{
			return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
		}

		public void InvokeIfNeeded(Action action)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(action);
			}
			else
			{
				action.Invoke();
			}
		}
        #endregion
    }
}

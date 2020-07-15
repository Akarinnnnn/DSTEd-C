﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using DSTEd.Core.LUA;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MoonSharp.Interpreter.Tree;

namespace DSTEd.Core.Contents.Editors {
    class Code : TextEditor, IDocumentHandler {
        private Document document;
		private CompletionWindow completion;
		private Configuration config = Configuration.getConfiguration();
		private static List<KeywordCompleteion> keywords = new List<KeywordCompleteion>();
		private List<FunctionCompleteion> funclist = new List<FunctionCompleteion>();
		private List<VariableCompletion> varlist = new List<VariableCompletion>();

		private Lexer lexer;//lexical analyzer, for autocompleteion. which is from modified moonsharp classlibrary

		public Code(Document document) {
            this.document = document;
			this.ShowLineNumbers = config.GetBool("Editor_ShowLineNumbers", true);
			this.FontFamily = new FontFamily(config.Get("Editor_FontFamily", "Consolas"));
			this.FontSize = config.GetDouble("Editor_FontSize", 14);
			this.Background = new SolidColorBrush(config.GetColor("Editor_Background", Color.FromArgb(255, 34, 34, 35)));
			this.Foreground = new SolidColorBrush(config.GetColor("Editor_Foreground", Color.FromArgb(255, 248, 248, 242)));

			TextEditorOptions options = new TextEditorOptions();
			options.AllowScrollBelowDocument = true;
			options.HighlightCurrentLine = true;
			this.Options = options;


			this.SyntaxHighlighting = LoadSyntax(Path.GetExtension(this.document.GetFilename()));
            this.Text = document.GetFileContent();
            this.Document.UpdateFinished += new EventHandler(OnChange);
            this.TextArea.TextEntering += OnEnter;
            this.TextArea.TextEntered += OnEntered;
			completion = new CompletionWindow(this.TextArea);
			OnInit();

			this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy();
            new XmlFoldingStrategy().UpdateFoldings(FoldingManager.Install(this.TextArea), this.Document);
        }
		static Code()//run once,to initialize basic completions
		{
			init_basic_completion();
		}
        private static void init_basic_completion()
        {
            keywords.Add(new KeywordCompleteion("if\n", "if"));
			keywords.Add(new KeywordCompleteion("else\n", "else"));
			keywords.Add(new KeywordCompleteion("end\n", "end"));
			keywords.Add(new KeywordCompleteion("true", ""));
			keywords.Add(new KeywordCompleteion("false", ""));
			keywords.Add(new KeywordCompleteion("function", ""));
			keywords.Add(new KeywordCompleteion("local", ""));
			keywords.Add(new KeywordCompleteion("require(", "require"));
		}

		public bool IsDocumentEqual(int HashCode)
        {
            return document.GetHashCode() == HashCode;
        }
		
        public void OnInit()
		{
			//init lexical analyzer
			switch(Path.GetExtension(this.document.GetFilename()))
			{
				case "lua":
					RefreshLexer();
					break;
			}
        }

		public void RefreshLexer()
		{
			if(Path.GetExtension(document.GetFilename()).CompareTo(".xml") == 0)
			{
				return;
			}
			lexer = new Lexer(0, document.GetFileContent(), false);
			funclist.Clear();
			varlist.Clear();
			while (lexer.Current.Type != TokenType.Eof)
			{
				var curtoken = lexer.Current;
				Token name;
				switch (curtoken.Type)
				{
					case TokenType.Function:
						do
						{
							lexer.Next();
						} while (lexer.Current.Type != TokenType.Name);//"Name" same as "identifier"
						name = lexer.Current;
						funclist.Add(new FunctionCompleteion(name.Text, name.Text, string.Empty));
						break;
					case TokenType.Local:
						if (lexer.PeekNext().Type == TokenType.Function)
						{
							do
							{
								lexer.Next();
							} while (lexer.Current.Type != TokenType.Name);//"Name" same as "identifier"
							name = lexer.Current;
							funclist.Add(new FunctionCompleteion(name.Text, name.Text, string.Empty));
						}
						else
						{
							do
							{
								lexer.Next();
							} while (lexer.Current.Type != TokenType.Name);
							name = lexer.Current;
							varlist.Add(new VariableCompletion(name.Text, name.Text));
						}
						break;
				}
				lexer.Next();
			}
		}

        private IHighlightingDefinition LoadSyntax(string extension) {
            if (extension == ".lua") {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DSTEd.Core.Contents.Editors.Format.lua_dark.xshd")) {
                    using (XmlTextReader reader = new XmlTextReader(stream)) {
                        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            } else {
                return HighlightingManager.Instance.GetDefinitionByExtension(extension);
            }
        }

        private void OnChange(object sender, EventArgs e) {
			this.document.UpdateChanges();
        }

        private void OnEnter(object sender, TextCompositionEventArgs e) {
            if (e.Text.Length > 0) {
				//if (!char.IsLetterOrDigit(e.Text[0])) {
				completion = new CompletionWindow(TextArea);
				completion.CompletionList.InsertionRequested += inserting;
                completion.CompletionList.RequestInsertion(e);
				//}
            }
        }

		private void inserting(object s,EventArgs Arg)
		{
			Dispatcher.Invoke(RefreshLexer);
			completion = new CompletionWindow(TextArea);
			var dataref = completion.CompletionList.CompletionData;
			if (Arg is TextCompositionEventArgs textarg)
			{
				if(textarg.Text[0]==':')
				{
					foreach (var F in funclist)
					{
						dataref.Add(F);
					}
				}
				else
				{
					foreach (var kw in keywords)
					{
						dataref.Add(kw);
					}
					foreach (var V in varlist)
					{
						dataref.Add(V);
					}
					foreach (var F in funclist)
					{
						dataref.Add(F);
					}
				}
				completion.Show();
				completion.Closed += (object lambdasender, EventArgs arg) =>
				{
					 completion = null;
					 //completion = new CompletionWindow(TextArea);
				};
				//dataref.sort
			}
		}

        private void OnEntered(object sender, TextCompositionEventArgs e) {
			/*completion = new CompletionWindow(TextArea);
			completion.CompletionList.IsFiltering = true;
			if (e.Text == ":")
			{
				var dataref = completion.CompletionList.CompletionData;
				foreach (var func in funclist)
				{
					//list all functions first......
					dataref.Add(func);
				}
				completion.Show();
				completion.Closed += delegate
				 {
					 completion = null;
				 };
				return;
			}

            if(e.Text.Length >= 1) {
                IList<ICompletionData> data = completion.CompletionList.CompletionData;
				foreach (var keyword in keywords)
				{
					data.Add(keyword);
				}
				//completion.
                completion.Show();
                completion.Closed += delegate {
					completion = null;
				};
            }*/
			}

		public StringBuilder Save()
		{
			return new StringBuilder(Text);
		}
	}
}

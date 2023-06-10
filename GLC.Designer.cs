namespace GLC
{
    using System.Data;
    using System.Text;
    using Terminal.Gui;

    public partial class GLCView : Window {
        public bool gameDetailsView = true;

        private ColorScheme colorDark;
        private ColorScheme colorDarkStatusBar;
        private ColorScheme colorDarkInfoView;
        private ColorScheme colorDarkInfoTableView;
        private ColorScheme colorLight;
        private ColorScheme colorLightStatusBar;
        private ColorScheme colorLightInfoView;
        private ColorScheme colorLightInfoTableView;
        private ColorScheme colorContrast;
        private ColorScheme colorContrastStatusBar;
        private ColorScheme colorContrastInfoView;
        private ColorScheme colorContrastInfoTableView;
        private Attribute colorTableHeaders;
        private Attribute colorBorders;

        private Button inputButton;
        private Label inputLabel;
        private TextField inputTextField;
        private Dialog inputDialog;

        private StatusItem[] statusItems;
        private StatusBar statusBar;

        private DataTable infoTableViewTable;
        public TableView infoTableView;
        public View infoView;

        public List<string> gameTableViewList;
        public DataTable gameTableViewDetails;
        public TableView gameTableView;
        public View gameView;

        public TreeNode searchTreeNode;
        public TreeNode hiddenTreeNode;
        public TreeView platformTreeView;
        private View platformView;

        public Dictionary<string, List<string[]>> gameData;
        public List<string[]> searchResults = new();
        public List<(string name, int num)> supers;
        public List<(string name, int num)> tags;
        public List<(string name, int num)> platforms;
        public string searchString;
        int infoViewHeight = 10;
        int platformViewWidth = 23;
        int inputTextFieldWidthDef = 18;
        public string defaultPlatformName = "MAME";
        public TreeNode defaultPlatform;
        public TreeNode platformNode;
        public TreeNode tagNode;
        public int defaultGame = 14;

        private void InitializeComponent() {
            Setup();

            platformTreeView = new() { Data = "platformTreeView", };
            if (gameDetailsView)
                gameTableView = new(new DataTableSource(gameTableViewDetails)) { Data = "gameTableDetails", };
            else
                gameTableView = new(new ListTableSource(gameTableViewList, gameTableView, new() { Orientation = Orientation.Vertical, ScrollParallel = false, })) { Data = "gameTableList", };
            infoTableView = new(new DataTableSource(infoTableViewTable)) { Data = "infoTableView", };
            statusBar = new(statusItems) { Data = "statusBar", };

            PopulateDummies();

            // root
            this.Title = "GameLauncher Console"; // only shown if border is enabled
            //SetDefaultBorder(this);
            this.BorderStyle = LineStyle.None;

            // PLATFORMS

            // View platformView
            platformView = new()
            {
                Data = "platformView",
                Width = platformViewWidth,
                Height = Dim.Fill(0),
                //X = 0,
                //Y = 0,
            };

            // TreeView platformTreeView
            platformTreeView.Width = Dim.Fill(0);
            platformTreeView.Height = Dim.Fill(0) - 1;
            platformTreeView.Style = new()
            {
                CollapseableSymbol = (Rune)'-',
                ColorExpandSymbol = false,
                ExpandableSymbol = (Rune)'+',
                InvertExpandSymbolColors = false,
                ShowBranchLines = false,
            };
            platformTreeView.TabIndex = 0;
            platformTreeView.SelectionChanged += OnPlatformSelectionChanged;

            platformView.Add(platformTreeView);
            this.Add(platformView);
            SetupScrollBar(platformTreeView);

            // GAMES

            // View gameView
            gameView = new()
            {
                Data = "gameView",
                Width = Dim.Fill(0),
                Height = Dim.Fill(0) - 9,
                X = Pos.Right(platformView) + 1,
                //Y = 0,
            };
            //SetDefaultBorder(gameView);

            // TableView gameTableView
            gameTableView.Width = Dim.Fill(0);
            gameTableView.Height = Dim.Fill(0) - 1;
            gameTableView.X = 2;
            //gameTableView.Y = 0;
            if (gameDetailsView)
            {
                gameTableView.FullRowSelect = true;
                gameTableView.Style = new()
                {
                    AlwaysShowHeaders = true,
                    ExpandLastColumn = true,
                    InvertSelectedCellFirstCharacter = false,
                    ShowHorizontalHeaderOverline = false,
                    ShowHorizontalHeaderUnderline = false,
                    ShowHorizontalScrollIndicators = false,
                    ShowVerticalCellLines = false,
                    ShowVerticalHeaderLines = false,
                };
            }
            else
            {
                gameTableView.Style = new()
                {
                    ShowHeaders = false,
                    ShowHorizontalHeaderOverline = false,
                    ExpandLastColumn = false,
                    InvertSelectedCellFirstCharacter = false,
                    ShowHorizontalScrollIndicators = true,
                    ShowVerticalCellLines = false,
                };
            }
            //SetTableHeaderColor(gameTableView);
            gameTableView.MaxCellWidth = 25;
            gameTableView.TabIndex = 1;
            gameTableView.SelectedCellChanged += (s, e) => UpdateInfo(e.NewRow);

            gameView.Add(gameTableView);
            this.Add(gameView);
            SetupScrollBar(gameTableView);

            // INFO PANEL

            // View infoView
            infoView = new()
            {
                Data = "infoView",
                Width = Dim.Fill(0) + 1,
                Height = infoViewHeight,
                X = Pos.Right(platformView) + 1,
                Y = Pos.Bottom(gameView) - 1,
                //TabIndex = 2,     // } Allow focus to the border to allow user to get an expanded info window
                //CanFocus = true,  // } (or maybe just use a hotkey instead?)
            };
            //SetDefaultBorder(infoView);

            // TableView infoTableView
            infoTableView.Width = Dim.Fill(0);
            infoTableView.Height = infoViewHeight;
            infoTableView.CanFocus = false;
            infoTableView.Enabled = false;
            infoTableView.Style = new()
            {
                ShowHeaders = false,
                ShowHorizontalHeaderOverline = false,
                ExpandLastColumn = true,
                ShowHorizontalScrollIndicators = false,
                ShowVerticalCellLines = false,
            };

            SetTableHeaderColor(infoTableView);

            infoView.CanFocus = false;
            infoView.Enabled = false;
            infoView.Add(infoTableView);
            this.Add(infoView);

            // STATUS BAR

            // StatusBar statusBar
            statusBar.Width = Dim.Fill(0);
            statusBar.Height = 1;
            //statusBar.X = 0;
            statusBar.Y = Pos.AnchorEnd(1);

            this.Add(statusBar);

            platformTreeView.SelectedObject = defaultPlatform;  // Set default to MAME
            int.TryParse(platformTreeView.SelectedObject.Tag.ToString(), out int i);
            if (platformTreeView.Bounds.Height < i)
                platformTreeView.EnsureVisible(platformNode);

            if (gameTableViewDetails.Rows.Count >= defaultGame + 1)
                gameTableView.SelectedRow = defaultGame;  // Set default to Pong
                //gameTableView.SelectedRow = 0;
            if (gameTableView.Bounds.Height < defaultGame)
                gameTableView.EnsureSelectedCellIsVisible();
            gameTableView.SetFocus();

            //infoTableView.SelectedRow = -1;

            this.ColorScheme = colorDark;
            statusBar.ColorScheme = colorDarkStatusBar;
            infoTableView.ColorScheme = colorDarkInfoTableView;
            infoView.ColorScheme = colorDarkInfoView;
        }

        private void OnPlatformSelectionChanged(object o, SelectionChangedEventArgs<ITreeNode> e)
        {
            /*
            if (e.NewValue is not null &&
                e.NewValue.Tag is not null &&
                e.NewValue.Tag.Equals(0))
                DoSearch(searchString); // repeat last search, if applicable
            else
            */
                PopulateGameTable();
        }

        private void SetDefaultBorder(View view)
        {
            view.BorderStyle = LineStyle.Single;
            view.Border.ColorScheme = new() { Normal = colorBorders };
        }

        private void SetTableHeaderColor(TableView tableView)
        {
            tableView.Style.RowColorGetter = (a) => { return a.RowIndex == 0 ? new ColorScheme() { Normal = colorTableHeaders } : null; };
            tableView.SetNeedsDisplay();
        }

        private string TrimPlatformName(string s)
        {
            if (s.Contains('['))
                return s[..s.IndexOf('[')].TrimEnd();
            return s;
        }

        private void Setup()
        {
            colorDark = new()
            {
                Normal = new Attribute(Color.Gray, Color.Black),
                HotNormal = new Attribute(Color.BrightRed, Color.Black),
                Focus = new Attribute(Color.Black, Color.Gray),
                HotFocus = new Attribute(Color.Red, Color.Gray),
                Disabled = new Attribute(Color.DarkGray, Color.Black),
            };
            colorDarkStatusBar = new() { Normal = new Attribute(Color.Black, Color.DarkGray), };
            colorDarkInfoView = new()
            {
                Normal = new Attribute(Color.DarkGray, Color.Black),
                HotNormal = new Attribute(Color.DarkGray, Color.Black),
                Focus = new Attribute(Color.DarkGray, Color.Black),
                HotFocus = new Attribute(Color.DarkGray, Color.Black),
                Disabled = new Attribute(Color.Red, Color.Black),
            };
            colorDarkInfoTableView = new()
            {
                Normal = new Attribute(Color.DarkGray, Color.Black),
                HotNormal = new Attribute(Color.DarkGray, Color.Black),
                Focus = new Attribute(Color.DarkGray, Color.Black),
                HotFocus = new Attribute(Color.DarkGray, Color.Black),
                Disabled = new Attribute(Color.DarkGray, Color.Black),
            };

            colorLight = new()
            {
                Normal = new Attribute(Color.Gray, Color.Blue),
                HotNormal = new Attribute(Color.BrightCyan, Color.Blue),
                Focus = new Attribute(Color.Gray, Color.Black),
                HotFocus = new Attribute(Color.White, Color.Black),
                Disabled = new Attribute(Color.DarkGray, Color.Blue),
            };
            colorLightStatusBar = new() { Normal = new Attribute(Color.Black, Color.Gray), };
            colorLightInfoView = new()
            {
                Normal = new Attribute(Color.DarkGray, Color.Blue),
                HotNormal = new Attribute(Color.Cyan, Color.Blue),
                Focus = new Attribute(Color.Cyan, Color.Blue),
                HotFocus = new Attribute(Color.Cyan, Color.Blue),
                Disabled = new Attribute(Color.DarkGray, Color.Blue),
            };
            colorLightInfoTableView = new()
            {
                Normal = new Attribute(Color.Cyan, Color.Blue),
                HotNormal = new Attribute(Color.Cyan, Color.Blue),
                Focus = new Attribute(Color.Cyan, Color.Blue),
                HotFocus = new Attribute(Color.Cyan, Color.Blue),
                Disabled = new Attribute(Color.Cyan, Color.Blue),
            };

            colorContrast = new()
            {
                Normal = new Attribute(Color.White, Color.Black),
                HotNormal = new Attribute(Color.BrightGreen, Color.Black),
                Focus = new Attribute(Color.Black, Color.BrightGreen),
                HotFocus = new Attribute(Color.Black, Color.BrightGreen),
                Disabled = new Attribute(Color.Gray, Color.Black),
            };
            colorContrastStatusBar = new() { Normal = new Attribute(Color.Black, Color.BrightMagenta), };
            colorContrastInfoView = new()
            {
                Normal = new Attribute(Color.Gray, Color.Black),
                HotNormal = new Attribute(Color.Gray, Color.Black),
                Focus = new Attribute(Color.Gray, Color.Black),
                HotFocus = new Attribute(Color.Gray, Color.Black),
                Disabled = new Attribute(Color.Gray, Color.Black),
            };
            colorContrastInfoTableView = new()
            {
                Normal = new Attribute(Color.Gray, Color.Black),
                HotNormal = new Attribute(Color.Gray, Color.Black),
                Focus = new Attribute(Color.Gray, Color.Black),
                HotFocus = new Attribute(Color.Gray, Color.Black),
                Disabled = new Attribute(Color.Gray, Color.Black),
            };

            statusItems = new StatusItem[]
            {
                new StatusItem(Key.F1, "F1 : Help", () =>
                {
                    MessageBox.Query("Help",
                        "GameLauncherConsole v2.0\n" +
                        "(c) 2023 GLC contributors\n" +
                        "   Esc : quit GLC                        \n" +
                        "   Tab : move between panes              \n" +
                        //" Enter : launch a game                   \n" +
                        "    F3 : search by name                  \n" +
                        //"    F5 : scan for new games              \n" +
                        "    F6 : switch between dark/light color \n", // +
                        //"    F9 : uninstall a game                \n",
                        "OK");
                }),
                new StatusItem(Key.F3, "F3 : Search", () =>
                {
                    string searchText = "Search:";
                    int iFVW = this.Bounds.Width;
                    int inputTextFieldWidth = inputTextFieldWidthDef;
                    if (iFVW > (inputTextFieldWidthDef + searchText.Length))
                        iFVW = inputTextFieldWidthDef + searchText.Length;
                    if ((iFVW - infoViewHeight - 1 < inputTextFieldWidthDef) &&
                        (iFVW - infoViewHeight - 1 > 0))
                        inputTextFieldWidth = iFVW - infoViewHeight - 1;
                    inputLabel = new()
                    {
                        Data = "inputLabel",
                        Text = searchText,
                    };
                    inputButton = new("OK", true) { Data = "inputButton", };
                    Button[] buttons = { inputButton, };
                    inputDialog = new(buttons)
                    {
                        Title = "Search",
                        Width = iFVW,
                        Data = "inputDialog",
                        Modal = true,
                        //CanFocus = false,
                    };
                    inputTextField = new(infoViewHeight - 1, 0, inputTextFieldWidth, "") { Data = "inputTextField", };
                    inputButton.Clicked += (s, e) =>
                    {
                        Application.RequestStop(inputDialog);
                        searchString = inputTextField.Text.ToString();
                        DoSearch(searchString);
                    };
                    inputDialog.Add(inputLabel);
                    inputDialog.Add(inputTextField);
                    inputTextField.FocusFirst();
                    Application.Run(inputDialog);
                    iFVW = -1;
                }),
                //new StatusItem(Key.F5, "F5 : Rescan", null),
                new StatusItem(Key.F6, "F6 : Color", () => {
                    if (this.ColorScheme == colorDark)
                    {
                        this.ColorScheme = colorLight;
                        statusBar.ColorScheme = colorLightStatusBar;
                        infoTableView.ColorScheme = colorLightInfoTableView;
                        infoView.ColorScheme = colorLightInfoView;
                        infoView.ColorScheme.Focus = colorLight.Normal;
                        colorTableHeaders = colorLight.Disabled;
                        colorBorders = colorLight.Disabled;
                    }
                    else if (this.ColorScheme == colorLight)
                    {
                        this.ColorScheme = colorContrast;
                        statusBar.ColorScheme = colorContrastStatusBar;
                        infoTableView.ColorScheme = colorContrastInfoTableView;
                        infoView.ColorScheme = colorContrastInfoView;
                        infoView.ColorScheme.Focus = colorContrast.Normal;
                        colorTableHeaders = colorContrast.Disabled;
                        colorBorders = colorContrast.Disabled;
                    }
                    else
                    {
                        this.ColorScheme = colorDark;
                        statusBar.ColorScheme = colorDarkStatusBar;
                        infoTableView.ColorScheme = colorDarkInfoTableView;
                        infoView.ColorScheme = colorDarkInfoView;
                        infoView.ColorScheme.Focus = colorDark.Normal;
                        colorTableHeaders = colorDark.Disabled;
                        colorBorders = colorDark.Disabled;
                    }
                    //SetDefaultBorder(infoView);
                }),
                new StatusItem(Key.F7, "F7 : Details", () => { ToggleGameTable(); }),
                //new StatusItem(Key.F9, "F9 : Uninst", null),
                new StatusItem(Key.Esc, "Esc : Quit", () => Application.RequestStop()),
            };

            gameTableViewList = new();
            gameTableViewDetails = new("Games");
            gameTableViewDetails.Columns.AddRange(new DataColumn[]
            {
                new("game"),
                new("fave"),
                new("hidn"),
                new("ratg"),
                new("platform")
            });

            infoTableViewTable = new("Game Information");
            infoTableViewTable.Columns.AddRange(new DataColumn[]
            {
                new("item"),
                new("value")
            });
        }

        private void SetupScrollBar(TreeView treeView)
        {
            //treeView.Style.LeaveLastRow = true;

            ScrollBarView _scrollBar = new(treeView, true);

            _scrollBar.ChangedPosition += (s, e) => {
                treeView.ScrollOffsetVertical = _scrollBar.Position;
                if (treeView.ScrollOffsetVertical != _scrollBar.Position)
                {
                    _scrollBar.Position = treeView.ScrollOffsetVertical;
                }
                treeView.SetNeedsDisplay();
            };

            /*
            _scrollBar.OtherScrollBarView.ChangedPosition += () => {
                treeView.ScrollOffsetHorizontal = _scrollBar.OtherScrollBarView.Position;
                if (treeView.ScrollOffsetHorizontal != _scrollBar.OtherScrollBarView.Position)
                {
                    _scrollBar.OtherScrollBarView.Position = treeView.ScrollOffsetHorizontal;
                }
                treeView.SetNeedsDisplay();
            };
            */

            treeView.DrawContent += (s, e) => {
                _scrollBar.Size = treeView.ContentHeight;
                _scrollBar.Position = treeView.ScrollOffsetVertical;
                //_scrollBar.OtherScrollBarView.Size = treeView.GetContentWidth(true);
                //_scrollBar.OtherScrollBarView.Position = treeView.ScrollOffsetHorizontal;
                _scrollBar.Refresh();
            };
        }
        private void SetupScrollBar(TableView tableView)
        {
            ScrollBarView _scrollBar = new(tableView, true);

            _scrollBar.ChangedPosition += (s, e) => {
                tableView.RowOffset = _scrollBar.Position;
                if (tableView.RowOffset != _scrollBar.Position)
                {
                    _scrollBar.Position = tableView.RowOffset;
                }
                tableView.SetNeedsDisplay();
            };

            /*
            _scrollBar.OtherScrollBarView.ChangedPosition += () => {
                tableView.ColumnOffset = _scrollBar.OtherScrollBarView.Position;
                if (tableView.ColumnOffset != _scrollBar.OtherScrollBarView.Position)
                {
                    _scrollBar.OtherScrollBarView.Position = tableView.ColumnOffset;
                }
                tableView.SetNeedsDisplay();
            };
            */

            tableView.DrawContent += (s, e) => {
                _scrollBar.Size = tableView.Bounds.Width;
                _scrollBar.Position = tableView.RowOffset;
                //_scrollBar.OtherScrollBarView.Size = tableView.Bounds.Width;
                //_scrollBar.OtherScrollBarView.Position = tableView.ColumnOffset;
                _scrollBar.Refresh();
            };
        }

        void GetHidden()
        {
            var hiddenGames = new List<string[]>();
            foreach (var platform in gameData)
            {
                foreach (var game in platform.Value)
                {
                    if (game[2].Equals("~"))
                        hiddenGames.Add(game);
                }
            }
            infoTableView.Clear();
            gameTableViewList.Clear();
            gameTableViewDetails.Rows.Clear();
            hiddenTreeNode = new(GetPlatformText("Hidden", hiddenGames.Count));
            CreatePlatformTree();
            PopulateGameView(hiddenGames);
        }

        void DoSearch(string s)
        {
            searchResults = new();
            foreach (var platform in gameData)
            {
                foreach (var game in platform.Value)
                {
                    // Check [0] title and [11] alias
                    if (game[0].ToLower().Contains((s ?? "").ToLower()) ||
                        (game.Length > 11 && game[11].Contains((s ?? "").ToLower())))
                        searchResults.Add(game);
                }
            }
            infoTableView.Clear();
            gameTableViewList.Clear();
            gameTableViewDetails.Rows.Clear();
            searchTreeNode = new(GetPlatformText("Search results", searchResults.Count));
            CreatePlatformTree();
            PopulateGameView(searchResults);
        }

        private void ToggleGameTable()
        {
            gameDetailsView ^= true;
            gameTableView.FullRowSelect ^= true;
            gameTableView.Style.ShowHeaders ^= true;

            if (gameDetailsView)
            {
                gameTableView.Data = "gameTableDetails";
                gameTableView.Table = new DataTableSource(gameTableViewDetails);
            }
            else
            {
                gameTableView.Data = "gameTableList";
                gameTableView.Table = new ListTableSource(gameTableViewList, gameTableView, new() { Orientation = Orientation.Vertical, ScrollParallel = false, });
            }

            PopulateGameTable();
        }

        private void PopulateGameView(List<string[]> games)
        {
            if (games.Count > 0)
            {
                int i = 0;
                foreach (var game in games.OrderBy(x => x[0]))
                {
                    i++;
                    if (game.Length > 4)
                        gameTableViewDetails.Rows.Add(game[0], game[1], game[2], game[3], game[4]);
                    else
                        gameTableViewDetails.Rows.Add(game[0], "", "", "", "");
                    gameTableViewList.Add(game[0]);
                }
                infoView.Title = games[0][0];
                gameTableView.SelectedRow = 0;
                gameTableView.EnsureSelectedCellIsVisible();
                gameTableView.FocusFirst();
            }
            else
            {
                infoView.Title = "";
                gameTableView.CanFocus = false;
                platformTreeView.FocusFirst();
            }
        }

        private void PopulateDummies()
        {
            tags = new()
            {
                ("kids", 20),
                ("emulators", 5),
            };
            platforms = new()
            {
                ("Custom", 10),
                ("Amazon", 165),
                ("Arc", 0),
                ("Battle.net", 2),
                ("Big Fish", 3),
                ("EA", 1),
                ("Epic", 42),
                ("Game Jolt", 0),
                ("GOG Galaxy", 72),
                ("Humble App", 0),
                ("IGClient", 41),
                ("itch", 48),
                ("Legacy", 0),
                ("MAME", 49),
                ("Oculus", 0),
                ("Paradox", 0),
                ("Plarium Play", 0),
                ("Rockstar", 0),
                ("Riot Client", 0),
                ("Steam", 222),
                ("Ubisoft", 22),
                ("Wargaming.net", 0),
            };
            supers = new()
            {
                ("New", 629),
                ("Favorites", 7),
                ("Installed", 175),
                ("All", 629),
                ("Tags", tags.Count),
                ("Platforms", platforms.Count),
                ("Not installed", 454),
                ("Hidden", 2),
            };

            infoTableViewTable.Rows.Add("", "");
            infoTableViewTable.Rows.Add(" platform:", "MAME");
            infoTableViewTable.Rows.Add("      age:", "E");
            infoTableViewTable.Rows.Add("  release:", "1972-11-29");
            infoTableViewTable.Rows.Add("developer:", "Al Alcorn");
            infoTableViewTable.Rows.Add("  ratings:", "7.6");
            infoTableViewTable.Rows.Add("   genres:", "family, sport");
            infoTableViewTable.Rows.Add("     tags:", "arcade");
            infoTableViewTable.Rows.Add("    alias:", "pong");
            //infoTableViewTable.Rows.Add("", "");

            CreatePlatformTree();
            PopulateGameTable();
        }

        private string GetPlatformText(string s, int i, int c = 0)
        {
            string sI = i.ToString();
            int w = platformViewWidth - s.Length - sI.Length - c - 5;
            if (w < 0) w = 0;

            return s + new string(' ', w) + '[' + sI + ']';
        }

        private void PopulateGameTable(bool details = true)
        {
            gameData = new();
            foreach (var platform in platforms)
            {
                int i = 0;
                /*
                gameData.Add(platform.name, new List<string[]>
                {
                    new string[] { platform.name + " Game", "", "", "3*", platform.name, "age", "date", "dev", "ratg", "genr", "tag", "ali" },
                });
                */
                gameData.Add(platform.name, new List<string[]>
                {
                    new string[] { "P.O.W. - Prisoners of War", "", "", "3*", platform.name, "age0", "date0", "dev0", "ratg0", "genr0", "tag0", "ali0" },
                    new string[] { "Pac & Pal", "", "", "3*", platform.name, "age1", "date1", "dev1", "ratg1", "genr1", "tag1", "ali1" },
                    new string[] { "Pac-Land", "", "", "3*", platform.name, "age2", "date2", "dev2", "ratg2", "genr2", "tag2", "ali2" },
                    new string[] { "Pac-Man", "*", "", "5*", platform.name, "age3", "date3", "dev3", "ratg3", "genr3", "tag3", "ali3" },
                    new string[] { "Pac-Man Plus", "", "", "3*", platform.name, "age4", "date4", "dev4", "ratg4", "genr4", "tag4", "ali4" },
                    new string[] { "Pac-Mania", "*", "", "5*", platform.name, "age5", "date5", "dev5", "ratg5", "genr5", "tag5", "ali5" },
                    new string[] { "Pengo", "", "", "3*", platform.name, "age6", "date6", "dev6", "ratg6", "genr6", "tag6", "ali6" },
                    new string[] { "Pipe Dream", "*", "", "5*", platform.name, "age7", "date7", "dev7", "ratg7", "genr7", "tag7", "ali7" },
                    new string[] { "Pit Fighter", "", "", "3*", platform.name, "age8", "date8", "dev8", "ratg8", "genr8", "tag8", "ali8" },
                    new string[] { "Pitfall II", "", "", "3*", platform.name, "age9", "date9", "dev9", "ratg9", "genr9", "tag9", "ali9" },
                    new string[] { "Play Girls", "", "~", "3*", platform.name, "age10", "date10", "dev10", "ratg10", "genr10", "tag10", "ali10" },
                    new string[] { "Play Girls 2", "", "~", "3*", platform.name, "age11", "date11", "dev11", "ratg11", "genr11", "tag11", "ali11" },
                    new string[] { "Pole Position", "", "", "3*", platform.name, "age12", "date12", "dev12", "ratg12", "genr12", "tag12", "ali12" },
                    new string[] { "Pole Position II", "", "", "3*", platform.name, "age13", "date13", "dev13", "ratg13", "genr13", "tag13", "ali13" },
                    new string[] { "Pong", "", "", "3*", platform.name, "E", "1972-11-29", "Al Alcorn", "7.6", "family, sport", "arcade", "pong" },
                    new string[] { "Popeye", "*", "", "4*", platform.name, "age15", "date15", "dev15", "ratg15", "genr15", "tag15", "ali15" },
                    new string[] { "Punch-Out!!", "*", "", "4*", platform.name, "age16", "date16", "dev16", "ratg16", "genr16", "tag16", "ali16" },
                    new string[] { "Punisher", "", "", "3*", platform.name },
                    new string[] { "Puyo Puyo", "*", "", "5*", platform.name },
                    new string[] { "Puyo Puyo 2", "", "", "4*", platform.name },
                    new string[] { "Puzzle Bobble", "*", "", "5*", platform.name },
                    new string[] { "Puzzle Bobble 2", "*", "", "5*", platform.name },
                    new string[] { "Puzzle Bobble 3", "*", "", "5*", platform.name },
                    new string[] { "Puzzle Bobble 4", "*", "", "5*", platform.name },
                    new string[] { "Q*bert", "*", "", "5*", platform.name },
                    new string[] { "Q*bert's Qubes", "", "", "3*", platform.name },
                    new string[] { "Qix", "", "", "3*", platform.name },
                    new string[] { "Race Drivin'", "", "", "3*", platform.name },
                    new string[] { "Rainbow Islands", "", "", "3*", platform.name },
                    new string[] { "Rally X", "", "", "3*", platform.name },
                    new string[] { "Rampage", "*", "", "5*", platform.name },
                    new string[] { "Rampage: World Tour", "", "", "3*", platform.name },
                    new string[] { "Rampart", "*", "", "5*", platform.name },
                    new string[] { "Rastan", "", "", "3*", platform.name },
                    new string[] { "Real Ghostbusters", "", "", "3*", platform.name },
                    new string[] { "Red Baron", "", "", "3*", platform.name },
                    new string[] { "Return of the Jedi", "", "", "3*", platform.name },
                    new string[] { "Road Blasters", "", "", "4*", platform.name },
                    new string[] { "Road Runner", "", "", "4*", platform.name },
                    new string[] { "Robocop", "", "", "4*", platform.name },
                    new string[] { "Robocop 2", "", "", "4*", platform.name },
                    new string[] { "Robotron: 2084", "", "", "3*", platform.name },
                    new string[] { "Rolling Thunder", "*", "", "4*", platform.name },
                    new string[] { "Rolling Thunder 2", "*", "", "4*", platform.name },
                    new string[] { "R-Type", "", "", "3*", platform.name },
                    new string[] { "R-Type II", "", "", "3*", platform.name },
                    new string[] { "R-Type Leo", "", "", "3*", platform.name },
                    new string[] { "Rush'n Attack", "*", "", "3*", platform.name },
                    new string[] { "Rygar", "", "", "3*", platform.name },
                });
            };

            gameTableViewList.Clear();
            gameTableViewDetails.Clear();
            if (platformTreeView.SelectedObject is not null)
            {
                var selectedPlatform = TrimPlatformName(platformTreeView.SelectedObject.Text);
                if (gameData.ContainsKey(selectedPlatform))
                {
                    List<string[]> selectedPlatformGames = gameData[selectedPlatform];
                    if (selectedPlatformGames is not null)
                    {
                        int i = 0;
                        foreach (var game in selectedPlatformGames)
                        {
                            i++;
                            gameTableViewDetails.Rows.Add(
                                game[0] ?? "null",
                                game[1] ?? "null",
                                game[2] ?? "null",
                                game[3] ?? "null",
                                game[4] ?? "null");
                            gameTableViewList.Add(game[0]);
                        }
                        gameTableView.CanFocus = true;
                    }
                }
            }
        }

        private void CreatePlatformTree()
        {
            platformTreeView.ClearObjects();
            TreeNode searchTreeNode = new(GetPlatformText("Search results", searchResults.Count));
            searchTreeNode.Tag = 0;
            platformTreeView.AddObject(searchTreeNode);

            int i = 1;
            foreach (var super in supers)
            {
                TreeNode node = new TreeNode(GetPlatformText(super.name, super.num));

                node.Tag = i;
                if (super.name.Equals("Tags"))
                {
                    tagNode = node;
                    foreach (var tag in tags)
                    {
                        i++;
                        TreeNode childNode = new TreeNode(GetPlatformText(tag.name, tag.num, 1));
                        childNode.Tag = i;
                        node.Children.Add(childNode);
                    }
                }
                else if (super.name.Equals("Platforms"))
                {
                    platformNode = node;
                    foreach (var platform in platforms)
                    {
                        i++;
                        TreeNode childNode = new TreeNode(GetPlatformText(platform.name, platform.num, 1));
                        childNode.Tag = i;
                        node.Children.Add(childNode);
                        if (platform.name.Equals(defaultPlatformName))
                            defaultPlatform = childNode;
                    }
                }

                platformTreeView.AddObject(node);
                i++;
            }

            platformTreeView.ExpandAll();
            platformTreeView.SelectedObject = searchTreeNode;
            platformTreeView.EnsureVisible(searchTreeNode);
        }

        public void UpdateInfo(int newRow)
        {
            var selectedPlatform = TrimPlatformName(platformTreeView.SelectedObject.Text);
            if (gameData.ContainsKey(selectedPlatform))
            {
                foreach (var game in gameData[selectedPlatform])
                {
                    if (game is not null &&
                        gameData[selectedPlatform][newRow].Length > 0)
                    {
                        var row = gameData[selectedPlatform][newRow];
                        infoView.Title = row[0] ?? "null";
                        DataTable newInfoTable = new("Game Information");
                        newInfoTable.Columns.AddRange(new DataColumn[]
                        {
                            new("item"),
                            new("value")
                        });

                        if (row.Length < 12)
                        {
                            if (row.Length < 5)
                                newInfoTable.Rows.Add(" platform:", "");
                            else
                                newInfoTable.Rows.Add(" platform:", row[4] ?? "null");

                            newInfoTable.Rows.Add("      age:", "");
                            newInfoTable.Rows.Add("  release:", "");
                            newInfoTable.Rows.Add("developer:", "");
                            newInfoTable.Rows.Add("  ratings:", "");
                            newInfoTable.Rows.Add("   genres:", "");
                            newInfoTable.Rows.Add("     tags:", "");
                            newInfoTable.Rows.Add("    alias:", "");
                        }
                        else
                        {
                            newInfoTable.Rows.Add(" platform:", row[4] ?? "null");
                            newInfoTable.Rows.Add("      age:", row[5] ?? "null");
                            newInfoTable.Rows.Add("  release:", row[6] ?? "null");
                            newInfoTable.Rows.Add("developer:", row[7] ?? "null");
                            newInfoTable.Rows.Add("  ratings:", row[8] ?? "null");
                            newInfoTable.Rows.Add("   genres:", row[9] ?? "null");
                            newInfoTable.Rows.Add("     tags:", row[10] ?? "null");
                            newInfoTable.Rows.Add("    alias:", row[11] ?? "null");
                        }
                        infoTableView.Table = new DataTableSource(newInfoTable);
                        //infoTableView.SelectedRow = -1;
                    }
                }
            }
        }
    }
}

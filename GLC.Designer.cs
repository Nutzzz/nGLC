namespace GLC
{
    using System.Data;
    using Terminal.Gui;
    using Terminal.Gui.Trees;

    public partial class GLCView : Window {
        
        private ColorScheme colorDark;
        private ColorScheme colorLight;

        private Button inputButton;
        private Label inputLabel;
        private TextField inputTextField;
        private Dialog inputDialog;

        private StatusItem[] statusItems;
        private StatusBar statusBar;

        private DataTable infoTableViewTable;
        public TableView infoTableView;
        public View infoView;

        public List<string[]> gameTableViewTableData;
        public DataTable gameTableViewTable;
        public TableView gameTableView;
        private ScrollView gameScrollView;
        private ScrollBarView gameScrollBarView;
        public View gameView;

        public TreeNode searchTreeNode;
        public TreeNode newTreeNode;
        public TreeNode faveTreeNode;
        public TreeNode instTreeNode;
        public TreeNode allTreeNode;
        public TreeNode tagsTreeNode;
        public TreeNode mameTreeNode;
        public TreeNode platformsTreeNode;
        public TreeNode notInstTreeNode;
        public TreeNode hidTreeNode;
        public TreeView platformTreeView;
        private ScrollView platformScrollView;
        private ScrollBarView platformScrollBarView;
        private View platformView;

        private void InitializeComponent() {
            inputLabel = new();
            inputLabel.Data = "inputLabel";
            inputTextField = new();
            inputTextField.Data = "inputTextField";
            inputButton = new("OK", true);
            inputButton.Data = "inputButton";

            inputDialog = new();
            inputDialog.Data = "inputDialog";

            infoView = new();
            infoView.Data = "infoView";
            gameView = new();
            gameView.Data = "gameView";
            platformTreeView = new();
            platformTreeView.Data = "platformTreeView";
            platformView = new();
            platformView.Data = "platformView";

            Setup();
            PopulateDefaults();

            statusBar = new(statusItems);
            statusBar.Data = "statusBar";
            infoTableView = new(infoTableViewTable);
            infoTableView.Data = "infoTableView";
            gameTableView = new(gameTableViewTable);
            gameTableView.Data = "gameTableView";

            // root
            this.ColorScheme = colorDark;
            this.Border.BorderStyle = BorderStyle.Rounded;
            this.Title = "GameLauncher Console";

            // PLATFORMS

            // View platformView
            platformView.Width = 22;
            platformView.Height = Dim.Fill(0);
            //platformView.X = 0;
            //platformView.Y = 0;

            // TreeView platformTreeView
            platformTreeView.Width = Dim.Fill(0);
            platformTreeView.Height = Dim.Fill(0);
            platformTreeView.Style.CollapseableSymbol = '-';
            platformTreeView.Style.ColorExpandSymbol = false;
            platformTreeView.Style.ExpandableSymbol = '+';
            platformTreeView.Style.InvertExpandSymbolColors = false;
            platformTreeView.Style.LeaveLastRow = true;
            platformTreeView.Style.ShowBranchLines = false;
            platformTreeView.TabIndex = 0;

            platformView.Add(platformTreeView);
            this.Add(platformView);

            // ScrollView platformScrollBarView
            platformScrollBarView = new(platformTreeView, true);
            platformScrollBarView.Data = "platformScrollBarView";
            //platformScrollBarView.Width = Dim.Fill(0);
            //platformScrollBarView.Height = Dim.Fill(0);
            //platformScrollBarView.X = 0;
            //platformScrollBarView.Y = 0;
            //platformScrollBarView.ContentSize = new Size(22, 22);
            //platformScrollBarView.AutoHideScrollBars = true;
            //platformScrollBarView.ShowHorizontalScrollIndicator = false;
            //platformScrollBarView.ShowVerticalScrollIndicator = true;

            //platformTreeView.SelectedObject = searchTreeNode;
            platformTreeView.ExpandAll();
            platformTreeView.SelectedObject = mameTreeNode;

            // GAMES

            // View gameView
            gameView.Width = Dim.Fill(0);
            gameView.Height = Dim.Fill(0) - 9;
            gameView.X = Pos.Right(platformView) + 1;
            //gameView.Y = 0;
            //gameView.Border = new Border();
            //gameView.Border.BorderStyle = BorderStyle.Rounded;

            // TableView gameTableView
            gameTableView.Width = Dim.Fill(0) - 1;
            gameTableView.Height = Dim.Fill(0) - 1;
            gameTableView.X = 1;
            //gameTableView.Y = 0;
            gameTableView.FullRowSelect = true;
            gameTableView.Style.AlwaysShowHeaders = true;
            gameTableView.Style.ExpandLastColumn = true;
            gameTableView.Style.InvertSelectedCellFirstCharacter = false;
            gameTableView.Style.ShowHorizontalHeaderOverline = false;
            gameTableView.Style.ShowHorizontalHeaderUnderline = false;
            gameTableView.Style.ShowVerticalCellLines = false;
            gameTableView.Style.ShowVerticalHeaderLines = false;
            gameTableView.Style.ShowHorizontalScrollIndicators = false;
            gameTableView.MaxCellWidth = 25;
            gameTableView.TabIndex = 1;

            //gameView.Add(gameTableView);

            // ScrollBarView gameScrollBarView
            /*
            gameScrollBarView = new(gameTableView, true);
            gameScrollBarView.Data = "gameScrollBarView";
            gameScrollBarView.Width = Dim.Fill(0);
            gameScrollBarView.Height = Dim.Fill(0);
            //gameScrollBarView.X = 1;
            //gameScrollBarView.Y = 0;
            gameScrollBarView.AutoHideScrollBars = true;
            */

            // ScrollView gameScrollView
            /*
            gameScrollView = new(gameView.Bounds);
            gameTableView.GetCurrentWidth(out int gVW);
            gameTableView.GetCurrentHeight(out int gVH);
            gameScrollView.ContentSize = new(gVW, gVH);
            gameScrollView.ShowHorizontalScrollIndicator = false;
            gameScrollView.ShowVerticalScrollIndicator = true;

            gameScrollView.Add(gameTableView);
            gameView.Add(gameScrollView);
            */
            gameView.Add(gameTableView);
            this.Add(gameView);
            gameTableView.FocusFirst();

            // INFO PANEL

            // View infoView
            infoView.Width = Dim.Fill(0);
            infoView.Height = 9;
            infoView.X = Pos.Right(platformView) + 1;
            infoView.Y = Pos.Bottom(gameView);
            //infoView.TabIndex = 2;     // } Allow focus to the border to allow user to get an expanded info window
            //infoView.CanFocus = true;  // } (or maybe just use a hotkey instead?)
            infoView.Border = new();
            infoView.Border.Title = "Pong";
            infoView.Border.BorderStyle = BorderStyle.Rounded;
            infoView.Border.BorderBrush = this.ColorScheme.Disabled.Foreground;
            infoView.Border.Background = this.ColorScheme.Disabled.Background;

            // TableView infoTableView
            infoTableView.Width = Dim.Fill(0);
            infoTableView.Height = 9;
            infoTableView.CanFocus = false;
            infoTableView.FullRowSelect = true;
            infoTableView.Style.AlwaysShowHeaders = false;
            infoTableView.Style.ExpandLastColumn = true;
            infoTableView.Style.ShowHorizontalHeaderOverline = false;
            infoTableView.Style.ShowHorizontalHeaderUnderline = false;
            infoTableView.Style.ShowHorizontalScrollIndicators = false;
            infoTableView.Style.ShowVerticalCellLines = false;
            infoTableView.Style.ShowVerticalHeaderLines = false;

            infoView.Add(infoTableView);
            this.Add(infoView);

            // this is a hack to hide the header row (along with a blank initial row)
            infoTableView.ChangeSelectionToEndOfTable(false);
            infoTableView.SelectedRow = -1;

            // STATUS BAR

            // StatusBar statusBar
            statusBar.Width = Dim.Fill(0);
            statusBar.Height = 1;
            //statusBar.X = 0;
            statusBar.Y = Pos.AnchorEnd(1);

            this.Add(statusBar);
        }

        private void Setup()
        {
            colorDark = new();
            colorDark.Normal = new Attribute(Color.Gray, Color.Black);
            colorDark.HotNormal = new Attribute(Color.BrightRed, Color.Black);
            colorDark.Focus = new Attribute(Color.Black, Color.Gray);
            colorDark.HotFocus = new Attribute(Color.Red, Color.Gray);
            colorDark.Disabled = new Attribute(Color.DarkGray, Color.Black);

            colorLight = new();
            colorLight.Normal = new Attribute(Color.Gray, Color.Blue);
            colorLight.HotNormal = new Attribute(Color.White, Color.Cyan);
            colorLight.Focus = new Attribute(Color.Black, Color.Gray);
            colorLight.HotFocus = new Attribute(Color.White, Color.Black);
            colorLight.Disabled = new Attribute(Color.DarkGray, Color.Blue);

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
                    this.GetCurrentWidth(out int iFVW);
                    if (iFVW > 25) iFVW = 25;
                    inputButton.Clicked += () =>
                    {
                        Application.RequestStop(inputDialog);
                        DoSearch(inputTextField.Text.ToString());
                    };
                    Button[] buttons = { inputButton, };
                    inputDialog = new("Search", iFVW, 5, buttons);
                    inputDialog.Modal = true;
                    //inputDialog.CanFocus = false;
                    inputLabel.Text = "Search:";
                    int iTFW = 18;
                    if (iFVW - 8 < 18 && iFVW - 8 > 0)
                        iTFW = iFVW - 8;
                    inputTextField = new(8, 0, iTFW, "");
                    inputDialog.Add(inputLabel);
                    inputDialog.Add(inputTextField);
                    inputTextField.FocusFirst();
                    Application.Run(inputDialog);
                }),
                //new StatusItem(Key.F5, "F5 : Rescan", null),
                new StatusItem(Key.F6, "F6 : Color", () => {
                    if (this.ColorScheme == colorDark)
                        this.ColorScheme = colorLight;
                    else
                        this.ColorScheme = colorDark;
                    infoView.Border.BorderBrush = this.ColorScheme.Disabled.Foreground;
                    infoView.Border.Background = this.ColorScheme.Disabled.Background;
                    infoView.Redraw(infoView.Bounds);
                }),
                //new StatusItem(Key.F9, "F9 : Uninst", null),
                new StatusItem(Key.Esc, "Esc : Quit", () => Application.RequestStop()),
            };

            gameTableViewTable = new("Games");
            gameTableViewTable.Columns.AddRange(new DataColumn[] {
                new("game"),
                new("fave"),
                new("hidn"),
                new("ratg"),
                new("platform")
            });

            infoTableViewTable = new("Game Information");
            infoTableViewTable.Columns.AddRange(new DataColumn[] {
                new("item"),
                new("value")
            });
        }

        void DoSearch(string s)
        {
            List<string> results = new();
            foreach (var game in gameTableViewTableData)
            {
                if (game[0].ToLower().Contains((s ?? "").ToLower()) ||
                    (game.Length > 11 && game[11].Contains((s ?? "").ToLower())))
                    results.Add(game[0]);
            }
            gameTableViewTable.Rows.Clear();
            foreach (var result in results)
            {
                gameTableViewTable.Rows.Add(result, "", "", "", "");
            }
            infoTableView.Clear();
            searchTreeNode = new("Search results [" + results.Count + "]");
            CreatePlatformTree();
            if (gameTableViewTable.Rows.Count > 0)
                gameTableView.SelectedRow = 0;
            gameTableView.FocusFirst();
        }

        private void PopulateDefaults()
        {
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

            searchTreeNode = new("Search results [0]");
            newTreeNode = new("New [629]");
            faveTreeNode = new("Favorites [7]");
            instTreeNode = new("Installed [175]");
            allTreeNode = new("All [629]");
            tagsTreeNode = new("Tags");
            tagsTreeNode.Children = new TreeNode[]
            {
                new TreeNode("kids [20]"),
                new TreeNode("emulators [5]"),
            };
            mameTreeNode = new("MAME [49]");
            platformsTreeNode = new("Platforms");
            platformsTreeNode.Children = new TreeNode[]
            {
                new TreeNode("Custom [10]"),
                new TreeNode("Amazon [165]"),
                new TreeNode("Arc [0]"),
                new TreeNode("Battle.net [2]"),
                new TreeNode("Big Fish [3]"),
                new TreeNode("EA [1]"),
                new TreeNode("Epic [42]"),
                new TreeNode("Game Jolt [0]"),
                new TreeNode("GOG Galaxy [72]"),
                new TreeNode("Humble App [0]"),
                new TreeNode("IGClient [41]"),
                new TreeNode("itch [48]"),
                new TreeNode("Legacy [0]"),
                mameTreeNode,
                new TreeNode("Oculus [0]"),
                new TreeNode("Paradox [0]"),
                new TreeNode("Plarium Play [0]"),
                new TreeNode("Rockstar [0]"),
                new TreeNode("Riot Client [0]"),
                new TreeNode("Steam [222]"),
                new TreeNode("Ubisoft [22]"),
                new TreeNode("Wargaming.net [0]"),
            };
            notInstTreeNode = new("Not installed [454]");
            hidTreeNode = new("Hidden [2]");
            CreatePlatformTree();

            gameTableViewTableData = new()
            {
                new string[] { "P.O.W. - Prisoners of War", "", "", "3*", "MAME", "age0", "date0", "dev0", "ratg0", "genr0", "tag0", "ali0" },
                new string[] { "Pac & Pal", "", "", "3*", "MAME", "age1", "date1", "dev1", "ratg1", "genr1", "tag1", "ali1" },
                new string[] { "Pac-Land", "", "", "3*", "MAME", "age2", "date2", "dev2", "ratg2", "genr2", "tag2", "ali2" },
                new string[] { "Pac-Man", "*", "", "5*", "MAME", "age3", "date3", "dev3", "ratg3", "genr3", "tag3", "ali3" },
                new string[] { "Pac-Man Plus", "", "", "3*", "MAME", "age4", "date4", "dev4", "ratg4", "genr4", "tag4", "ali4" },
                new string[] { "Pac-Mania", "*", "", "5*", "MAME", "age5", "date5", "dev5", "ratg5", "genr5", "tag5", "ali5" },
                new string[] { "Pengo", "", "", "3*", "MAME", "age6", "date6", "dev6", "ratg6", "genr6", "tag6", "ali6" },
                new string[] { "Pipe Dream", "*", "", "5*", "MAME", "age7", "date7", "dev7", "ratg7", "genr7", "tag7", "ali7" },
                new string[] { "Pit Fighter", "", "", "3*", "MAME", "age8", "date8", "dev8", "ratg8", "genr8", "tag8", "ali8" },
                new string[] { "Pitfall II", "", "", "3*", "MAME", "age9", "date9", "dev9", "ratg9", "genr9", "tag9", "ali9" },
                new string[] { "Play Girls", "", "~", "3*", "MAME" },
                new string[] { "Play Girls 2", "", "~", "3*", "MAME" },
                new string[] { "Pole Position", "", "", "3*", "MAME" },
                new string[] { "Pole Position II", "", "", "3*", "MAME" },
                new string[] { "Pong", "", "", "3*", "MAME" },
                new string[] { "Popeye", "*", "", "4*", "MAME" },
                new string[] { "Punch-Out!!", "*", "", "4*", "MAME" },
                new string[] { "Punisher", "", "", "3*", "MAME" },
                new string[] { "Puyo Puyo", "*", "", "5*", "MAME" },
                new string[] { "Puyo Puyo 2", "", "", "4*", "MAME" },
                new string[] { "Puzzle Bobble", "*", "", "5*", "MAME" },
                new string[] { "Puzzle Bobble 2", "*", "", "5*", "MAME" },
                new string[] { "Puzzle Bobble 3", "*", "", "5*", "MAME" },
                new string[] { "Puzzle Bobble 4", "*", "", "5*", "MAME" },
                new string[] { "Q*bert", "*", "", "5*", "MAME" },
                new string[] { "Q*bert's Qubes", "", "", "3*", "MAME" },
                new string[] { "Qix", "", "", "3*", "MAME" },
                new string[] { "Race Drivin'", "", "", "3*", "MAME" },
                new string[] { "Rainbow Islands", "", "", "3*", "MAME" },
                new string[] { "Rally X", "", "", "3*", "MAME" },
                new string[] { "Rampage", "*", "", "5*", "MAME" },
                new string[] { "Rampage: World Tour", "", "", "3*", "MAME" },
                new string[] { "Rampart", "*", "", "5*", "MAME" },
                new string[] { "Rastan", "", "", "3*", "MAME" },
                new string[] { "Real Ghostbusters", "", "", "3*", "MAME" },
                new string[] { "Red Baron", "", "", "3*", "MAME" },
                new string[] { "Return of the Jedi", "", "", "3*", "MAME" },
                new string[] { "Road Blasters", "", "", "4*", "MAME" },
                new string[] { "Road Runner", "", "", "4*", "MAME" },
                new string[] { "Robocop", "", "", "4*", "MAME" },
                new string[] { "Robocop 2", "", "", "4*", "MAME" },
                new string[] { "Robotron: 2084", "", "", "3*", "MAME" },
                new string[] { "Rolling Thunder", "*", "", "4*", "MAME" },
                new string[] { "Rolling Thunder 2", "*", "", "4*", "MAME" },
                new string[] { "R-Type", "", "", "3*", "MAME" },
                new string[] { "R-Type II", "", "", "3*", "MAME" },
                new string[] { "R-Type Leo", "", "", "3*", "MAME" },
                new string[] { "Rush'n Attack", "*", "", "3*", "MAME" },
                new string[] { "Rygar", "", "", "3*", "MAME" },
            };

            foreach (var game in gameTableViewTableData)
            {
                gameTableViewTable.Rows.Add(game[0] ?? "null", game[1] ?? "null", game[2] ?? "null", game[3] ?? "null", game[4] ?? "null");
            }
        }

        private void CreatePlatformTree()
        {
            platformTreeView.ClearObjects();
            platformTreeView.AddObject(searchTreeNode);
            platformTreeView.AddObjects(new TreeNode[]
            {
                newTreeNode,
                faveTreeNode,
                instTreeNode,
                allTreeNode,
                tagsTreeNode,
                platformsTreeNode,
                notInstTreeNode,
                hidTreeNode,
            });
            platformTreeView.SelectedObject = searchTreeNode;
        }
    }
}

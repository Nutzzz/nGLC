using System.Data;
using Terminal.Gui;
using GLC;

Application.Init();

try
{
    GLCView view = new();

    view.gameTableView.SelectedCellChanged += (e) => UpdateInfo(e.NewRow);

    void UpdateInfo(int newRow)
    {
        if (view.gameTableViewTableData[newRow] is not null &&
            view.gameTableViewTableData[newRow].Length > 0)
        {
            //view.infoView.Border.Title = view.gameTableViewTable.Rows[newRow].ItemArray[0].ToString() ?? "null";
            view.infoView.Border.Title = view.gameTableViewTableData[newRow][0] ?? "null";
            DataTable newInfoTable = new();
            newInfoTable = new("Game Information");
            newInfoTable.Columns.AddRange(new DataColumn[] {
                new("item"),
                new("value")
            });

            newInfoTable.Rows.Add("", "");
            if (view.gameTableViewTableData[newRow].Length < 12)
            {
                if (view.gameTableViewTableData[newRow].Length < 5)
                    newInfoTable.Rows.Add(" platform:", "");
                else
                    newInfoTable.Rows.Add(" platform:", view.gameTableViewTableData[newRow][4] ?? "null");

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
                newInfoTable.Rows.Add(" platform:", view.gameTableViewTableData[newRow][4] ?? "null");
                newInfoTable.Rows.Add("      age:", view.gameTableViewTableData[newRow][5] ?? "null");
                newInfoTable.Rows.Add("  release:", view.gameTableViewTableData[newRow][6] ?? "null");
                newInfoTable.Rows.Add("developer:", view.gameTableViewTableData[newRow][7] ?? "null");
                newInfoTable.Rows.Add("  ratings:", view.gameTableViewTableData[newRow][8] ?? "null");
                newInfoTable.Rows.Add("   genres:", view.gameTableViewTableData[newRow][9] ?? "null");
                newInfoTable.Rows.Add("     tags:", view.gameTableViewTableData[newRow][10] ?? "null");
                newInfoTable.Rows.Add("    alias:", view.gameTableViewTableData[newRow][11] ?? "null");
            }
            view.infoTableView.Table = newInfoTable;
            view.infoTableView.ChangeSelectionToEndOfTable(false);
            view.infoTableView.SelectedRow = -1;
        }
    }

    Application.Run(view);
}
finally
{
    Application.Shutdown();
}
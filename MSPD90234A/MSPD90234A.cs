using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using JPlatform.Client.Controls6;
using JPlatform.Client.CSIGMESBaseform6;
using JPlatform.Client.JBaseForm6;
using JPlatform.Client.Library6.interFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;


namespace CSI.GMES.PD
{
    public partial class MSPD90234A : CSIGMESBaseform6//JERPBaseForm
    {
        public bool _firstLoad = true, _dateLoad = false;
        public MyCellMergeHelper _Helper = null;
        private JPlatform.Client.Controls6.RepositoryItemCheckEditEx repositoryItemCheckEditEx1 = new RepositoryItemCheckEditEx();

        public MSPD90234A()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _firstLoad = true;

            base.OnLoad(e);
            NewButton = false;
            AddButton = false;
            DeleteRowButton = false;
            SaveButton = true;
            DeleteButton = false;
            PreviewButton = false;
            PrintButton = false;

            gbPlant.Visible = false;
            gbLine.Visible = false;
            gbMLine.Visible = false;
            gbStyle.Visible = false;
            gbModel.Visible = false;
            gbArea.Visible = false;
            gbItem.Visible = false;
            gbKey.Visible = false;

            txtModel.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
            txtModel.BackColor = Color.FromArgb(255, 228, 225);

            InitCombobox();
            Formart_Grid_Main();

            _firstLoad = false;
        }

        #region [Start Button Event Code By UIBuilder]

        public override void QueryClick()
        {
            try
            {
                pbProgressShow();
                InitControls(grdMain);

                DataTable _dtSource = GetData("Q_MAIN",
                                            cboFactory.EditValue.ToString(),
                                            cboPlant.EditValue.ToString(),
                                            cboLine.EditValue.ToString(),
                                            "",
                                            cboModel.EditValue.ToString());

                if (_dtSource != null && _dtSource.Rows.Count > 0)
                {
                    for (int iRow = 0; iRow < _dtSource.Rows.Count; iRow++)
                    {
                        byte[] data = Convert.FromBase64String(_dtSource.Rows[iRow]["KEY_NAME_VN"].ToString());
                        _dtSource.Rows[iRow]["KEY_NM"] = _dtSource.Rows[iRow]["KEY_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(data);

                        byte[] dataStand = Convert.FromBase64String(_dtSource.Rows[iRow]["STANDARD_NAME_VN"].ToString());
                        _dtSource.Rows[iRow]["STANDARD_NM"] = _dtSource.Rows[iRow]["STANDARD_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataStand);

                        byte[] dataProcess = Convert.FromBase64String(_dtSource.Rows[iRow]["PROCESS_NAME_VN"].ToString());
                        _dtSource.Rows[iRow]["PROCESS_NM"] = _dtSource.Rows[iRow]["PROCESS_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataProcess);

                        byte[] dataCheckpoint = Convert.FromBase64String(_dtSource.Rows[iRow]["CHECKPOINT_NAME_VN"].ToString());
                        _dtSource.Rows[iRow]["CHECKPOINT_NM"] = _dtSource.Rows[iRow]["CHECKPOINT_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataCheckpoint);
                    }
                    SetData(grdMain, _dtSource);
                    Formart_Grid_Main();
                    gvwMain.TopRowIndex = 0;
                }
            }
            catch { }
            finally
            {
                pbProgressHide();
            }
        }

        public void Formart_Grid_Main()
        {
            try
            {
                grdMain.BeginUpdate();

                for (int i = 0; i < gvwMain.Columns.Count; i++)
                {
                    gvwMain.Columns[i].OptionsColumn.AllowEdit = true;
                    gvwMain.Columns[i].OptionsColumn.AllowMerge = DefaultBoolean.True;
                    gvwMain.Columns[i].OptionsColumn.ReadOnly = false;
                    gvwMain.Columns[i].OptionsColumn.AllowSort = DefaultBoolean.False;

                    gvwMain.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gvwMain.Columns[i].AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    gvwMain.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gvwMain.Columns[i].AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    gvwMain.Columns[i].AppearanceCell.TextOptions.WordWrap = WordWrap.Wrap;
                    gvwMain.Columns[i].AppearanceCell.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);

                    gvwMain.Columns[i].OptionsColumn.AllowEdit = false;
                    gvwMain.Columns[i].OptionsColumn.ReadOnly = true;

                    if (gvwMain.Columns[i].FieldName.Contains("STT"))
                    {
                        gvwMain.Columns[i].Width = 80;
                    }

                    if (gvwMain.Columns[i].FieldName.Contains("USE_YN"))
                    {
                        gvwMain.Columns[i].Width = 80;
                        gvwMain.Columns[i].ColumnEdit = this.repositoryItemCheckEditEx1;
                    }

                    if (gvwMain.Columns[i].FieldName.Contains("KEY_NM"))
                    {
                        gvwMain.Columns[i].Width = 200;
                        gvwMain.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwMain.Columns[i].FieldName.Contains("STANDARD_NM"))
                    {
                        gvwMain.Columns[i].Width = 200;
                        gvwMain.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwMain.Columns[i].FieldName.Contains("AREA_NM"))
                    {
                        gvwMain.Columns[i].Width = 100;
                        gvwMain.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwMain.Columns[i].FieldName.Contains("PROCESS_NM"))
                    {
                        gvwMain.Columns[i].Width = 200;
                        gvwMain.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                        gvwMain.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwMain.Columns[i].FieldName.Contains("CHECKPOINT_NM"))
                    {
                        gvwMain.Columns[i].Width = 300;
                        gvwMain.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                        gvwMain.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }
                }

                gvwMain.RowHeight = 90;
                grdMain.EndUpdate();
            }
            catch 
            {
               
            }
        }

        #endregion [Start Button Event Code By UIBuilder] 

        #region [Grid]

        private DataTable GetData(string argType, string _factory = "", string _line = "", string _mline = "", string _style = "", string _model = "", string _condition = "")
        {
            try
            {
                P_MSPD90234A_Q proc = new P_MSPD90234A_Q();
                DataTable dtData = null;
                dtData = proc.SetParamData(dtData, argType, _factory, _line, _mline, _style, _model, _condition);
                ResultSet rs = CommonCallQuery(dtData, proc.ProcName, proc.GetParamInfo(), false, 90000, "", true);
                if (rs == null || rs.ResultDataSet == null || rs.ResultDataSet.Tables.Count == 0 || rs.ResultDataSet.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                return rs.ResultDataSet.Tables[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }


        #endregion [Grid]

        #region [Combobox]

        private void InitCombobox()
        {
            LoadDataCbo(cboFactory, "Factory", "Q_FTY");
            LoadDataCbo(cboPlant, "Plant", "Q_LINE", cboFactory.EditValue.ToString());
            LoadDataCbo(cboLine, "Line", "Q_MLINE", cboFactory.EditValue.ToString(), cboPlant.EditValue.ToString());
            LoadDataCbo(cboModel, "Model", "Q_MODEL", cboFactory.EditValue.ToString(), cboPlant.EditValue.ToString(), cboLine.EditValue.ToString());
        }

        private void LoadDataCbo(LookUpEditEx argCbo, string _cbo_nm, string argType, string _factory = "", string _line = "", string _mline = "", string _style = "", string _model = "", string _condition = "")
        {
            try
            {
                DataTable dt = GetData(argType, _factory, _line, _mline, _style, _model, _condition);

                if (dt == null || dt.Rows.Count < 1)
                {
                    argCbo.Properties.Columns.Clear();
                    argCbo.Properties.DataSource = null;

                    if (argType.Equals("Q_MODEL"))
                    {
                        txtModel.Text = "";
                    }

                    return;
                }

                string columnCode = dt.Columns[0].ColumnName;
                string columnName = dt.Columns[1].ColumnName;
                string captionCode = "Code";
                string captionName = _cbo_nm;

                argCbo.Properties.Columns.Clear();
                argCbo.Properties.DataSource = dt;
                argCbo.Properties.ValueMember = columnCode;
                argCbo.Properties.DisplayMember = columnName;
                argCbo.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(columnCode));
                argCbo.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(columnName));
                argCbo.Properties.Columns[columnCode].Visible = argType.Equals("Q_MODEL") ? true : false;
                argCbo.Properties.Columns[columnCode].Width = 10;
                argCbo.Properties.Columns[columnCode].Caption = captionCode;
                argCbo.Properties.Columns[columnName].Caption = captionName;
                argCbo.SelectedIndex = 0;

                if (argType.Equals("Q_MODEL"))
                {
                    txtModel.Text = cboModel.EditValue.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion [Combobox]

        #region Events

        private void cboFactory_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                LoadDataCbo(cboPlant, "Plant", "Q_LINE", cboFactory.EditValue.ToString());
            }
        }

        private void cboPlant_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                LoadDataCbo(cboLine, "Line", "Q_MLINE", cboFactory.EditValue.ToString(), cboPlant.EditValue.ToString());
            }
        }

        private void cboLine_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                LoadDataCbo(cboModel, "Model", "Q_MODEL", cboFactory.EditValue.ToString(), cboPlant.EditValue.ToString(), cboLine.EditValue.ToString());
            }
        }

        private void cboModel_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                txtModel.Text = cboModel.EditValue.ToString();
            }
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            MSPD90234A_POP view = new MSPD90234A_POP();
            view.SetBrowserMain(this._browserMain);
            view.ShowDialog();

            bool _result = view.CheckIsSaved();
            if (_result)
            {
                _firstLoad = true;

                LoadDataCbo(cboModel, "Model", "Q_MODEL", cboFactory.EditValue.ToString(), cboPlant.EditValue.ToString(), cboLine.EditValue.ToString());
                QueryClick();

                _firstLoad = false;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string _factory = cboFactory.EditValue.ToString();
            string _plant = cboPlant.EditValue.ToString();
            string _line = cboLine.EditValue.ToString();
            string _model = cboModel.EditValue.ToString();

            MSPD90234A_POP_COPY view = new MSPD90234A_POP_COPY(_factory, _plant, _line, "", _model);
            view.SetBrowserMain(this._browserMain);
            view.ShowDialog();
        }

        private void gvwMain_CellMerge(object sender, CellMergeEventArgs e)
        {
            try
            {
                if (grdMain.DataSource == null || gvwMain.RowCount <= 0) return;

                e.Merge = false;
                e.Handled = true;

                if (e.Column.FieldName.ToString() == "STT")
                {
                    string _value1 = gvwMain.GetRowCellValue(e.RowHandle1, e.Column.FieldName.ToString()).ToString();
                    string _value2 = gvwMain.GetRowCellValue(e.RowHandle2, e.Column.FieldName.ToString()).ToString();

                    if (_value1 == _value2)
                    {
                        e.Merge = true;
                    }
                }

                if (e.Column.FieldName.ToString() == "KEY_NM" || e.Column.FieldName.ToString() == "STANDARD_NM")
                {
                    string _value1 = gvwMain.GetRowCellValue(e.RowHandle1, "STT").ToString();
                    string _value2 = gvwMain.GetRowCellValue(e.RowHandle2, "STT").ToString();
                    string _value3 = gvwMain.GetRowCellValue(e.RowHandle1, e.Column.FieldName.ToString()).ToString();
                    string _value4 = gvwMain.GetRowCellValue(e.RowHandle2, e.Column.FieldName.ToString()).ToString();

                    if (_value1 == _value2 && _value3 == _value4)
                    {
                        e.Merge = true;
                    }
                }
            }
            catch
            {

            }
        }

        private void gvwMain_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (grdMain.DataSource == null || gvwMain.RowCount < 1) return;

                if (e.Column.FieldName.ToString().Equals("KEY_NM") || e.Column.FieldName.ToString().Equals("STANDARD_NM"))
                {
                    e.Appearance.BackColor = Color.LightYellow;
                }

                if (e.Column.FieldName.ToString().Equals("STT"))
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 228, 225);
                }
            }
            catch { }
        }

        private void gvwMain_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (grdMain.DataSource == null || gvwMain.RowCount < 1) return;

                if (e.Column.FieldName.ToString().Equals("STT") || 
                    e.Column.FieldName.ToString().Equals("KEY_NM") ||
                    e.Column.FieldName.ToString().Equals("STANDARD_NM") ||
                    e.Column.FieldName.ToString().Equals("AREA_NM") ||
                    e.Column.FieldName.ToString().Equals("PROCESS_NM") ||
                    e.Column.FieldName.ToString().Equals("CHECKPOINT_NM"))
                {
                    if (e.Clicks >= 2)
                    {
                        string _fty = gvwMain.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString();
                        string _plant = gvwMain.GetRowCellValue(e.RowHandle, "LINE_CD").ToString();
                        string _line = gvwMain.GetRowCellValue(e.RowHandle, "MLINE_CD").ToString();
                        string _style = gvwMain.GetRowCellValue(e.RowHandle, "STYLE_CD").ToString();
                        string _model = gvwMain.GetRowCellValue(e.RowHandle, "MODEL_CD").ToString();
                        string _key = "";

                        if(e.Column.FieldName.ToString().Equals("AREA_NM") ||
                            e.Column.FieldName.ToString().Equals("PROCESS_NM") ||
                            e.Column.FieldName.ToString().Equals("CHECKPOINT_NM"))
                        {
                            _key = gvwMain.GetRowCellValue(e.RowHandle, "KEY_CD").ToString();
                        }

                        MSPD90234A_POP view = new MSPD90234A_POP(_fty, _plant, _line, _style, _model, _key);
                        view.SetBrowserMain(this._browserMain);
                        view.ShowDialog();

                        bool _result = view.CheckIsSaved();
                        if (_result)
                        {
                            QueryClick();
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

        #region Database

        public class P_MSPD90234A_Q : BaseProcClass
        {
            public P_MSPD90234A_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "LMES.P_MSPD90234A_Q";
                ParamAdd();
            }
            private void ParamAdd()
            {
                _ParamInfo.Add(new ParamInfo("@ARG_WORK_TYPE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PLANT", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_LINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MLINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STYLE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_SEARCH", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable,
                                        System.String ARG_WORK_TYPE,
                                        System.String ARG_PLANT,
                                        System.String ARG_LINE,
                                        System.String ARG_MLINE,
                                        System.String ARG_STYLE,
                                        System.String ARG_MODEL,
                                        System.String ARG_SEARCH)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                                                ARG_WORK_TYPE,
                                                ARG_PLANT,
                                                ARG_LINE,
                                                ARG_MLINE,
                                                ARG_STYLE,
                                                ARG_MODEL,
                                                ARG_SEARCH
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class P_MSPD90234A_S : BaseProcClass
        {
            public P_MSPD90234A_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "LMES.P_MSPD90234A_S";
                ParamAdd();
            }
            private void ParamAdd()
            {
                _ParamInfo.Add(new ParamInfo("@ARG_TYPE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_DATE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PLANT", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_LINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MLINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STYLE", "Varchar2", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_SIZE", "Varchar2", 100, "Input", typeof(System.String)));


                _ParamInfo.Add(new ParamInfo("@ARG_TEST", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_FSS_FAIL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_FGA_FAIL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_SHOE_FAIL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_BONDING", "Varchar", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_REASON", "Varchar2", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STATUS", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PHOTO", "BLOB", 900000, "Input", typeof(byte[])));
                _ParamInfo.Add(new ParamInfo("@ARG_COUNTER", "Varchar2", 0, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_CREATE_PC", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_CREATE_PROGRAM_ID", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable,
                                        System.String ARG_TYPE,
                                        System.String ARG_DATE,
                                        System.String ARG_PLANT,
                                        System.String ARG_LINE,
                                        System.String ARG_MLINE,
                                        System.String ARG_STYLE,
                                        System.String ARG_SIZE,
                                        System.String ARG_TEST,
                                        System.String ARG_FSS_FAIL,
                                        System.String ARG_FGA_FAIL,
                                        System.String ARG_SHOE_FAIL,
                                        System.String ARG_BONDING,
                                        System.String ARG_REASON,
                                        System.String ARG_STATUS,
                                        byte[] ARG_PHOTO,
                                        System.String ARG_COUNTER,
                                        System.String ARG_CREATE_PC,
                                        System.String ARG_CREATE_PROGRAM_ID)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                    ARG_TYPE,
                    ARG_DATE,
                    ARG_PLANT,
                    ARG_LINE,
                    ARG_MLINE,
                    ARG_STYLE,
                    ARG_SIZE,
                    ARG_TEST,
                    ARG_FSS_FAIL,
                    ARG_FGA_FAIL,
                    ARG_SHOE_FAIL,
                    ARG_BONDING,
                    ARG_REASON,
                    ARG_STATUS,
                    ARG_PHOTO,
                    ARG_COUNTER,
                    ARG_CREATE_PC,
                    ARG_CREATE_PROGRAM_ID
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        #endregion

        DataTable GetDataTable(GridView view)
        {
            DataTable dt = new DataTable();
            foreach (GridColumn c in view.Columns)
                dt.Columns.Add(c.FieldName, c.ColumnType);
            for (int r = 0; r < view.RowCount; r++)
            {
                object[] rowValues = new object[dt.Columns.Count];
                for (int c = 0; c < dt.Columns.Count; c++)
                    rowValues[c] = view.GetRowCellValue(r, dt.Columns[c].ColumnName);
                dt.Rows.Add(rowValues);
            }
            return dt;
        }

        private DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();
            PropertyInfo[] columns = null;
            if (Linqlist == null) return dt;
            foreach (T Record in Linqlist)
            {
                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
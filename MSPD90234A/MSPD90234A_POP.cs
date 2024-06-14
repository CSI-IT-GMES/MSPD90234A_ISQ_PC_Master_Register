using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;

using JPlatform.Client.Library6.interFace;
using JPlatform.Client;
using JPlatform.Client.Controls6;
using JPlatform.Client.JBaseForm6;
using JPlatform.Client.CSIGMESBaseform6;
using DevExpress.XtraReports.UI;
using DevExpress.XtraGrid.Columns;
using System.Globalization;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting;
using System.Reflection;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using System.Data.OleDb;
using System.IO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraCharts;
using System.Diagnostics;
namespace CSI.GMES.PD
{
    public partial class MSPD90234A_POP : CSIGMESBaseform6 
    {
        #region Variable

        public DataTable _dtStyle = null, _dtKeyPoint = null, _dtProcess = null;
        public bool _firstLoad = true, _isSaved = false;
        public string _plant_cd = "", _line_cd = "", _mline_cd = "", _style_cd = "", _model_cd = "", _key_cd = "";
        public string _state = "KEY";
        private JPlatform.Client.Controls6.RepositoryItemCheckEditEx repositoryItemCheckEditEx1 = new RepositoryItemCheckEditEx();

        #endregion

        public MSPD90234A_POP()
        {
            InitializeComponent();
        }

        public MSPD90234A_POP(string _plant, string _line, string _mline, string _style, string _model, string _key)
        {
            InitializeComponent();
            _plant_cd = _plant;
            _line_cd = _line;
            _mline_cd = _mline;
            _style_cd = _style;
            _model_cd = _model;
            _key_cd = _key;
        }

        #region Load Data
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                _firstLoad = true;

                txtModelCode.BackColor = Color.FromArgb(255, 228, 225);
                txtModelName.BackColor = Color.FromArgb(255, 228, 225);
                txtKey_EN.BackColor = Color.FromArgb(255, 228, 225);
                txtKey_VN.BackColor = Color.FromArgb(255, 228, 225);
                txtStand_EN.BackColor = Color.FromArgb(255, 228, 225);
                txtStand_VN.BackColor = Color.FromArgb(255, 228, 225);

                txtStyle.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
                txtStyle.BackColor = Color.FromArgb(255, 228, 225);

                gbItem.Visible = false;

                LoadDataCbo(cboFactory, "Factory", "Q_FTY");
                if (!string.IsNullOrEmpty(_plant_cd))
                {
                    cboFactory.EditValue = _plant_cd;
                }

                LoadDataCbo(cboPlant, "Plant", "Q_LINE", cboFactory.EditValue.ToString());
                if (!string.IsNullOrEmpty(_line_cd))
                {
                    cboPlant.EditValue = _line_cd;
                }

                LoadDataCbo(cboLine, "Line", "Q_MLINE", cboFactory.EditValue.ToString(), cboPlant.EditValue.ToString());
                if (!string.IsNullOrEmpty(_mline_cd))
                {
                    cboLine.EditValue = _mline_cd;
                }

                LoadDataCbo(cboStyle, "Style", "Q_STYLE_ALL");

                DataTable _dtf = GetData("Q_PROCESS");
                _dtProcess = _dtf.Copy();

                if (string.IsNullOrEmpty(_key_cd))
                {
                    _state = "KEY";
                    radKey.Checked = true;
                }
                else
                {
                    _state = "ITEM";
                    radItem.Checked = true;
                }
                
                Binding_Layout();

                if (!string.IsNullOrEmpty(_style_cd))
                {
                    cboStyle.EditValue = _style_cd;

                    if (_state.Equals("ITEM"))
                    {
                        LoadDataCbo(cboKey,
                                    "Key Point",
                                    "Q_CBO_KEY",
                                    cboFactory.EditValue.ToString(),
                                    cboPlant.EditValue.ToString(),
                                    cboLine.EditValue.ToString(),
                                    cboStyle.EditValue.ToString(),
                                    txtModelCode.Text.ToString(),
                                    "");
                        txtStyle.Text = cboStyle.EditValue.ToString();
                        cboKey.EditValue = _key_cd;
                        getKeyPoint();
                    }

                    SearchData();
                }

                _firstLoad = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SetBrowserMain(JPlatform.Client.Library6.interFace.IBrowserMain JbrowserMain)
        {
            this._browserMain = JbrowserMain;
        }

        public bool CheckIsSaved()
        {
            return _isSaved;
        }

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

        private void LoadDataCbo(LookUpEditEx argCbo, string _cbo_nm, string argType, string _factory = "", string _line = "", string _mline = "", string _style = "", string _model = "", string _condition = "")
        {
            try
            {
                DataTable dt = GetData(argType, _factory, _line, _mline, _style, _model, _condition);

                if (dt == null || dt.Rows.Count < 1)
                {
                    argCbo.Properties.Columns.Clear();
                    argCbo.Properties.DataSource = null;

                    if (argType.Equals("Q_STYLE_ALL"))
                    {
                        _dtStyle = null;
                        getModel();
                    }
                    else if (argType.Equals("Q_STYLE"))
                    {
                        txtModelCode.Text = "";
                        txtModelName.Text = "";
                        txtStyle.Text = "";

                        _dtStyle = null;
                        getModel();

                        cboKey.Properties.Columns.Clear();
                        cboKey.Properties.DataSource = null;

                        _dtKeyPoint = null;
                        getKeyPoint();
                    }
                    else if (argType.Equals("Q_CBO_KEY"))
                    {
                        _dtKeyPoint = null;
                        getKeyPoint();
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
                argCbo.Properties.Columns[columnCode].Visible = (argType.Equals("Q_STYLE_ALL") || argType.Equals("Q_STYLE")) ? true : false;
                argCbo.Properties.Columns[columnCode].Width = 10;
                argCbo.Properties.Columns[columnCode].Caption = captionCode;
                argCbo.Properties.Columns[columnName].Caption = captionName;
                argCbo.SelectedIndex = 0;

                if (argType.Equals("Q_STYLE_ALL"))
                {
                    _dtStyle = dt.Copy();
                    getModel();
                }
                else if (argType.Equals("Q_STYLE"))
                {
                    _dtStyle = dt.Copy();
                    getModel();

                    if (_state.Equals("ITEM"))
                    {
                        LoadDataCbo(cboKey,
                                "Key Point",
                                "Q_CBO_KEY",
                                cboFactory.EditValue.ToString(),
                                cboPlant.EditValue.ToString(),
                                cboLine.EditValue.ToString(),
                                cboStyle.EditValue.ToString(),
                                txtModelCode.Text.ToString(),
                                "");
                    }
                }
                else if (argType.Equals("Q_CBO_KEY"))
                {
                    _dtKeyPoint = dt.Copy();
                    getKeyPoint();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void getModel()
        {
            try
            {
                if (_dtStyle == null || _dtStyle.Rows.Count < 1) 
                {
                    txtModelCode.Text = "";
                    txtModelName.Text = "";

                    return;
                }
                
                string _selected_style = cboStyle.EditValue.ToString();

                for(int iRow = 0; iRow < _dtStyle.Rows.Count; iRow++)
                {
                    if (_dtStyle.Rows[iRow]["CODE"].ToString().Equals(_selected_style))
                    {
                        txtModelCode.Text = _dtStyle.Rows[iRow]["MODEL_CD"].ToString();
                        txtModelName.Text = _dtStyle.Rows[iRow]["MODEL_NAME"].ToString();
                        break;
                    }
                }
            }
            catch { }
        }

        public void getKeyPoint()
        {
            try
            {
                if (_dtKeyPoint == null || _dtKeyPoint.Rows.Count < 1)
                {
                    txtKey_EN.Text = "";
                    txtKey_VN.Text = "";
                    txtStand_EN.Text = "";
                    txtStand_VN.Text = "";

                    return;
                }

                string _selected_key = cboKey.EditValue.ToString();

                for (int iRow = 0; iRow < _dtKeyPoint.Rows.Count; iRow++)
                {
                    if (_dtKeyPoint.Rows[iRow]["CODE"].ToString().Equals(_selected_key))
                    {
                        txtKey_EN.Text = _dtKeyPoint.Rows[iRow]["KEY_NAME_EN"].ToString();
                        byte[] data = Convert.FromBase64String(_dtKeyPoint.Rows[iRow]["KEY_NAME_VN"].ToString());
                        txtKey_VN.Text = Encoding.UTF8.GetString(data);

                        txtStand_EN.Text = _dtKeyPoint.Rows[iRow]["STANDARD_NAME_EN"].ToString();
                        byte[] dataStandard = Convert.FromBase64String(_dtKeyPoint.Rows[iRow]["STANDARD_NAME_VN"].ToString());
                        txtStand_VN.Text = Encoding.UTF8.GetString(dataStandard);

                        break;
                    }
                }
            }
            catch { }
        }

        public string getProcess(string code)
        {
            string _result = "";

            if (_dtProcess == null || _dtProcess.Rows.Count < 1) return _result;

            for(int iRow = 0; iRow < _dtProcess.Rows.Count; iRow++)
            {
                if (_dtProcess.Rows[iRow]["CODE"].ToString().Equals(code))
                {
                    _result = _dtProcess.Rows[iRow]["NAME"].ToString();
                    break;
                }
            }

            return _result;
        }

        private void Format_Grid_Base()
        {
            grdDetail.BeginUpdate();

            for (int i = 0; i < gvwDetail.Columns.Count; i++)
            {
                gvwDetail.Columns[i].OptionsColumn.AllowEdit = false;
                gvwDetail.Columns[i].OptionsColumn.AllowMerge = DefaultBoolean.False;
                gvwDetail.Columns[i].OptionsColumn.ReadOnly = true;
                gvwDetail.Columns[i].OptionsColumn.AllowSort = DefaultBoolean.False;

                gvwDetail.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gvwDetail.Columns[i].AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gvwDetail.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gvwDetail.Columns[i].AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gvwDetail.Columns[i].AppearanceCell.TextOptions.WordWrap = WordWrap.Wrap;
                gvwDetail.Columns[i].AppearanceCell.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);

                switch (_state)
                {
                    case "KEY":
                        if (gvwDetail.Columns[i].FieldName.ToString().Equals("KEY_NAME_EN") ||
                            gvwDetail.Columns[i].FieldName.ToString().Equals("KEY_NAME_VN") ||
                            gvwDetail.Columns[i].FieldName.ToString().Equals("STANDARD_NAME_EN") ||
                            gvwDetail.Columns[i].FieldName.ToString().Equals("STANDARD_NAME_VN"))
                        {
                            gvwDetail.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                            gvwDetail.Columns[i].Width = 220;
                            gvwDetail.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();

                            gvwDetail.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwDetail.Columns[i].OptionsColumn.ReadOnly = false;
                        }

                        if (gvwDetail.Columns[i].FieldName.Contains("USE_YN"))
                        {
                            gvwDetail.Columns[i].Width = 60;
                            gvwDetail.Columns[i].ColumnEdit = this.repositoryItemCheckEditEx1;
                            gvwDetail.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwDetail.Columns[i].OptionsColumn.ReadOnly = false;
                        }

                        break;
                    case "ITEM":

                        if (gvwDetail.Columns[i].FieldName.ToString().Equals("PROCESS_NAME_EN") ||
                            gvwDetail.Columns[i].FieldName.ToString().Equals("PROCESS_NAME_VN") ||
                            gvwDetail.Columns[i].FieldName.ToString().Equals("CHECKPOINT_NAME_EN") ||
                            gvwDetail.Columns[i].FieldName.ToString().Equals("CHECKPOINT_NAME_VN"))
                        {
                            gvwDetail.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                            gvwDetail.Columns[i].Width = 200;
                            gvwDetail.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                            gvwDetail.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwDetail.Columns[i].OptionsColumn.ReadOnly = false;
                        }

                        if (gvwDetail.Columns[i].FieldName.ToString().Equals("AREA_CD"))
                        {
                            gvwDetail.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwDetail.Columns[i].OptionsColumn.ReadOnly = false;
                            gvwDetail.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                        }

                        if (gvwDetail.Columns[i].FieldName.Contains("USE_YN"))
                        {
                            gvwDetail.Columns[i].Width = 60;
                            gvwDetail.Columns[i].ColumnEdit = this.repositoryItemCheckEditEx1;
                            gvwDetail.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwDetail.Columns[i].OptionsColumn.ReadOnly = false;
                        }

                        break;
                    default:
                        break;
                }
            }

            switch (_state)
            {
                case "KEY":
                    gvwDetail.RowHeight = 95;
                    break;
                case "ITEM":
                    gvwDetail.RowHeight = 75;
                    break;
                default:
                    break;
            }
            
            grdDetail.EndUpdate();
        }

        public void Binding_Layout()
        {
            JPlatform.Client.CSIGMESBaseform6.frmSplashScreenWait frmSplash = new JPlatform.Client.CSIGMESBaseform6.frmSplashScreenWait();

            try
            {
                frmSplash.Show();

                switch (_state)
                {
                    case "KEY":
                        pnTop.Height = 125;
                        btnSearch.Location = new Point(611, 81);
                        btnAdd.Location = new Point(713, 81);
                        btnDel.Location = new Point(828, 81);
                        btnSave.Location = new Point(943, 81);

                        gbKey.Visible = false;
                        gbStandard.Visible = false;
                        lblKey.Visible = false;
                        cboKey.Visible = false;

                        CreateSizeGrid(grdDetail, gvwDetail);
                        Format_Grid_Base();

                        _firstLoad = true;

                        txtStyle.Text = "";
                        txtStyle.Enabled = true;
                        LoadDataCbo(cboStyle, "Style", "Q_STYLE_ALL");

                        _firstLoad = false;

                        break;
                    case "ITEM":
                        pnTop.Height = 320;
                        btnSearch.Location = new Point(611, 282);
                        btnAdd.Location = new Point(713, 282);
                        btnDel.Location = new Point(828, 282);
                        btnSave.Location = new Point(943, 282);

                        gbKey.Visible = true;
                        gbStandard.Visible = true;
                        lblKey.Visible = true;
                        cboKey.Visible = true;

                        CreateSizeGrid(grdDetail, gvwDetail);
                        Format_Grid_Base();

                        _firstLoad = true;

                        txtStyle.Text = "";
                        txtStyle.Enabled = false;

                        LoadDataCbo(cboStyle, 
                                    "Style", 
                                    "Q_STYLE",
                                    cboFactory.EditValue.ToString(),
                                    cboPlant.EditValue.ToString(),
                                    cboLine.EditValue.ToString());
                        LoadDataCbo(cboKey, 
                                    "Key Point", 
                                    "Q_CBO_KEY",
                                    cboFactory.EditValue.ToString(),
                                    cboPlant.EditValue.ToString(),
                                    cboLine.EditValue.ToString(),
                                    cboStyle.EditValue.ToString(),
                                    txtModelCode.Text.ToString(),
                                    "");
                        txtStyle.Text = cboStyle.EditValue.ToString();

                        _firstLoad = false;

                        break;
                    default:
                        break;
                }

                frmSplash.Hide();
            }
            catch
            {
                frmSplash.Hide();
            }
        }

        public void CreateSizeGrid(GridControlEx gridControl, BandedGridViewEx gridView)
        {
            //gridControl.Hide();
            gridView.BeginDataUpdate();
            try
            {
                gridControl.DataSource = null;
                InitControls(gridControl);
                gridView.Columns.Clear();
                gridView.Bands.Clear();

                while (gridView.Columns.Count > 0)
                {
                    gridView.Columns.RemoveAt(0);
                }

                gridView.OptionsView.ShowColumnHeaders = false;

                GridBandEx gridBand = null;
                GridBandEx gridBandChild = null;
                BandedGridColumnEx colBand = new BandedGridColumnEx();

                ////////// STT
                gridBand = new GridBandEx() { Caption = "No" };
                gridView.Bands.Add(gridBand);
                gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                gridBand.AppearanceHeader.Options.UseBackColor = true;
                gridBand.RowCount = 1;
                gridBand.Visible = true;

                colBand = new BandedGridColumnEx()
                {
                    FieldName = "STT",
                    Visible = true,
                };
                colBand.Width = 60;
                gridBand.Columns.Add(colBand);

                switch (_state)
                {
                    case "KEY":
                        ////////// KEY_CD
                        gridBand = new GridBandEx() { Caption = "KEY_CD" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = false;

                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "KEY_CD",
                            Visible = false,
                        };
                        colBand.Width = 60;
                        gridBand.Columns.Add(colBand);


                        //////Key Point
                        gridBand = new GridBandEx() { Caption = "Key Point" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Eng)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "KEY_NAME_EN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Viet)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "KEY_NAME_VN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        //////Standard
                        gridBand = new GridBandEx() { Caption = "Standard" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Eng)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "STANDARD_NAME_EN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Viet)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "STANDARD_NAME_VN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        ////////// USE_YN
                        gridBand = new GridBandEx() { Caption = "Use Y/N" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "USE_YN",
                            Visible = true,
                        };
                        colBand.Width = 60;
                        gridBand.Columns.Add(colBand);

                        break;
                    case "ITEM":
                        ////////// ITEM_CD
                        gridBand = new GridBandEx() { Caption = "ITEM_CD" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = false;

                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "ITEM_CD",
                            Visible = false,
                        };
                        colBand.Width = 60;
                        gridBand.Columns.Add(colBand);

                        ////////// AREA_CD
                        gridBand = new GridBandEx() { Caption = "Area" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "AREA_CD",
                            Visible = true,
                        };
                        colBand.Width = 101;
                        gridBand.Columns.Add(colBand);

                        //////Process
                        gridBand = new GridBandEx() { Caption = "Process" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Eng)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "PROCESS_NAME_EN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Viet)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "PROCESS_NAME_VN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        //////Checkpoint
                        gridBand = new GridBandEx() { Caption = "Checkpoint" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Eng)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "CHECKPOINT_NAME_EN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        ///////Column
                        gridBandChild = new GridBandEx() { Caption = "Name (Viet)" };
                        gridBand.Children.Add(gridBandChild);
                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "CHECKPOINT_NAME_VN",
                            Visible = true,
                        };
                        colBand.Width = 200;
                        gridBandChild.Columns.Add(colBand);
                        gridBandChild.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBandChild.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBandChild.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
                        gridBandChild.AppearanceHeader.Options.UseBackColor = true;
                        gridBandChild.RowCount = 1;
                        gridBandChild.Visible = true;

                        ////////// USE_YN
                        gridBand = new GridBandEx() { Caption = "Use Y/N" };
                        gridView.Bands.Add(gridBand);
                        gridBand.AppearanceHeader.TextOptions.WordWrap = WordWrap.Wrap;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        gridBand.AppearanceHeader.Options.UseBackColor = true;
                        gridBand.RowCount = 1;
                        gridBand.Visible = true;

                        colBand = new BandedGridColumnEx()
                        {
                            FieldName = "USE_YN",
                            Visible = true,
                        };
                        colBand.Width = 60;
                        gridBand.Columns.Add(colBand);

                        break;
                    default:
                        break;
                }
            }
            catch
            {
                //throw EX;
            }
            //gridControl.Show();
            gridView.EndDataUpdate();
            gridView.ExpandAllGroups();
        }

        #endregion

        #region Events

        private void radKey_CheckedChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                _state = "KEY";
                Binding_Layout();
            }
        }

        private void radItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                _state = "ITEM";
                Binding_Layout();
            }
        }

        public bool SaveData(string _type, DataTable _dtDelete = null)
        {
            JPlatform.Client.CSIGMESBaseform6.frmSplashScreenWait frmSplash = new JPlatform.Client.CSIGMESBaseform6.frmSplashScreenWait();

            try
            {
                bool _result = true;
                DataTable dtData = null;
                P_MSPD90234A_S proc = new P_MSPD90234A_S();

                string machineName = $"{SessionInfo.UserName}|{ Environment.MachineName}|{GetIPAddress()}";
                string _plant = cboFactory.EditValue.ToString();
                string _line = cboPlant.EditValue.ToString();
                string _mline = cboLine.EditValue.ToString();
                string _style = cboStyle.EditValue.ToString();
                string _model = txtModelCode.Text.ToString();
                string _model_nm = txtModelName.Text.ToString();

                frmSplash.Show();

                if (_type.Equals("SAVE_KEY"))
                {
                    DataTable _dtf = GetDataTable(gvwDetail);
                    int iUpdate = 0, iCount = 0;

                    for (int iRow = 0; iRow < _dtf.Rows.Count; iRow++)
                    {
                        iUpdate++;

                        string _key_cd = _dtf.Rows[iRow]["KEY_CD"].ToString().Trim();

                        byte[] dataArr = System.Text.Encoding.UTF8.GetBytes(_dtf.Rows[iRow]["KEY_NAME_VN"].ToString().Trim());
                        string _txt_key_vietnam = System.Convert.ToBase64String(dataArr);

                        byte[] dataArrStandard = System.Text.Encoding.UTF8.GetBytes(_dtf.Rows[iRow]["STANDARD_NAME_VN"].ToString().Trim());
                        string _txt_standard_vietnam = System.Convert.ToBase64String(dataArrStandard);

                        dtData = proc.SetParamData(dtData,
                                                  _type,
                                                  _plant,
                                                  _line,
                                                  _mline,
                                                  _style,
                                                  _model,
                                                  _model_nm,
                                                  _key_cd,
                                                  _dtf.Rows[iRow]["KEY_NAME_EN"].ToString().Trim(),
                                                  _txt_key_vietnam,
                                                  _dtf.Rows[iRow]["STANDARD_NAME_EN"].ToString().Trim(),
                                                  _txt_standard_vietnam,
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  _dtf.Rows[iRow]["USE_YN"].ToString().Trim(),
                                                  machineName,
                                                  "CSI.GMES.PD.MSPD90234A_POP");

                        if (CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), grdDetail))
                        {
                            dtData = null;
                            iCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    frmSplash.Hide();

                    if (iUpdate == iCount)
                    {
                        _result = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_type.Equals("SAVE_ITEM"))
                {
                    DataTable _dtf = GetDataTable(gvwDetail);
                    int iUpdate = 0, iCount = 0;

                    for (int iRow = 0; iRow < _dtf.Rows.Count; iRow++)
                    {
                        iUpdate++;

                        string _key_cd = cboKey.EditValue.ToString();

                        byte[] dataArr = System.Text.Encoding.UTF8.GetBytes(txtKey_VN.Text.ToString().Trim());
                        string _txt_key_vietnam = System.Convert.ToBase64String(dataArr);

                        byte[] dataArrStandard = System.Text.Encoding.UTF8.GetBytes(txtStand_VN.Text.ToString().Trim());
                        string _txt_standard_vietnam = System.Convert.ToBase64String(dataArrStandard);

                        byte[] dataProcess = System.Text.Encoding.UTF8.GetBytes(_dtf.Rows[iRow]["PROCESS_NAME_VN"].ToString().Trim());
                        string _txt_process_vietnam = System.Convert.ToBase64String(dataProcess);

                        byte[] dataCheckpoint = System.Text.Encoding.UTF8.GetBytes(_dtf.Rows[iRow]["CHECKPOINT_NAME_VN"].ToString().Trim());
                        string _txt_checkpoint_vietnam = System.Convert.ToBase64String(dataCheckpoint);

                        dtData = proc.SetParamData(dtData,
                                                  _type,
                                                  _plant,
                                                  _line,
                                                  _mline,
                                                  _style,
                                                  _model,
                                                  _model_nm,
                                                  _key_cd,
                                                  txtKey_EN.Text.ToString().Trim(),
                                                  _txt_key_vietnam,
                                                  txtStand_EN.Text.ToString().Trim(),
                                                  _txt_standard_vietnam,
                                                  _dtf.Rows[iRow]["ITEM_CD"].ToString().Trim(),
                                                  _dtf.Rows[iRow]["AREA_CD"].ToString().Trim(),
                                                  getProcess(_dtf.Rows[iRow]["AREA_CD"].ToString()),
                                                  _dtf.Rows[iRow]["PROCESS_NAME_EN"].ToString().Trim(),
                                                  _txt_process_vietnam,
                                                  _dtf.Rows[iRow]["CHECKPOINT_NAME_EN"].ToString().Trim(),
                                                  _txt_checkpoint_vietnam,
                                                  "",
                                                  _dtf.Rows[iRow]["USE_YN"].ToString().Trim(),
                                                  machineName,
                                                  "CSI.GMES.PD.MSPD90234A_POP");

                        if (CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), grdDetail))
                        {
                            dtData = null;
                            iCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    frmSplash.Hide();

                    if (iUpdate == iCount)
                    {
                        _result = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_type.Equals("DELETE_KEY"))
                {
                    int iUpdate = 0, iCount = 0;

                    for (int iRow = 0; iRow < _dtDelete.Rows.Count; iRow++)
                    {
                        iUpdate++;
                        dtData = proc.SetParamData(dtData,
                                                  _type,
                                                  _plant,
                                                  _line,
                                                  _mline,
                                                  _style,
                                                  _model,
                                                  "",
                                                  _dtDelete.Rows[iRow]["KEY_CD"].ToString(),
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  machineName,
                                                  "CSI.GMES.PD.MSPD90234A_POP");

                        if (CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), grdDetail))
                        {
                            dtData = null;
                            iCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    frmSplash.Hide();

                    if (iUpdate == iCount)
                    {
                        _result = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_type.Equals("DELETE_ITEM"))
                {
                    int iUpdate = 0, iCount = 0;
                    string _key_point = string.IsNullOrEmpty(cboKey.EditValue.ToString()) ? "" : cboKey.EditValue.ToString();

                    for (int iRow = 0; iRow < _dtDelete.Rows.Count; iRow++)
                    {
                        iUpdate++;
                        dtData = proc.SetParamData(dtData,
                                                  _type,
                                                  _plant,
                                                  _line,
                                                  _mline,
                                                  _style,
                                                  _model,
                                                  "",
                                                  _key_point,
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  _dtDelete.Rows[iRow]["ITEM_CD"].ToString(),
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  machineName,
                                                  "CSI.GMES.PD.MSPD90234A_POP");

                        if (CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), grdDetail))
                        {
                            dtData = null;
                            iCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    frmSplash.Hide();

                    if (iUpdate == iCount)
                    {
                        _result = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                frmSplash.Hide();

                return _result;
            }
            catch (Exception ex)
            {
                frmSplash.Hide();
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void btnDel_Key_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlr = new DialogResult();
                string _sType = "";

                switch (_state)
                {
                    case "KEY":
                        _sType = "DELETE_KEY";
                        dlr = MessageBox.Show("Bạn có muốn xóa Key Point này không?", "Input", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        break;
                    case "ITEM":
                        _sType = "DELETE_ITEM";
                        dlr = MessageBox.Show("Bạn có muốn xóa Item Row này không?", "Input", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        break;
                    default:
                        break;
                }

                if (dlr == DialogResult.Yes)
                {
                    ArrayList rows = new ArrayList();

                    // Add the selected rows to the list.

                    Int32[] selectedRowHandles = gvwDetail.GetSelectedRows();
                    if (selectedRowHandles.Length == 0) return;
                    DataTable dt = GetDataTable(gvwDetail);
                    dt.Rows.Clear();

                    for (int i = selectedRowHandles.Length - 1; i >= 0; i--)
                    {
                        int selectedRowHandle = selectedRowHandles[i];
                        if (selectedRowHandle >= 0)
                        {
                            DataRow row = gvwDetail.GetDataRow(selectedRowHandle);
                            if (row.RowState == DataRowState.Added)
                            {
                                gvwDetail.DeleteRow(selectedRowHandle);
                            }
                            else if (row.RowState == DataRowState.Unchanged || row.RowState == DataRowState.Modified)
                            {
                                DataRow dr = gvwDetail.GetDataRow(selectedRowHandle);
                                dt.ImportRow(dr);
                                rows.Add(gvwDetail.GetDataRow(selectedRowHandle));
                                gvwDetail.DeleteRow(selectedRowHandle);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        bool result = SaveData(_sType, dt);
                        if (result)
                        {
                            MessageBoxW("Delete successfully!", IconType.Information);
                            _isSaved = true;
                            ///SearchData();
                        }
                        else
                        {
                            MessageBoxW("Delete failed!", IconType.Warning);
                        }
                    }
                }
            }
            catch { }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                switch (_state)
                {
                    case "KEY":
                        if (grdDetail.DataSource == null || gvwDetail.RowCount < 1)
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("STT", typeof(string));
                            dt.Columns.Add("KEY_CD", typeof(string));
                            dt.Columns.Add("KEY_NAME_EN", typeof(string));
                            dt.Columns.Add("KEY_NAME_VN", typeof(string));
                            dt.Columns.Add("STANDARD_NAME_EN", typeof(string));
                            dt.Columns.Add("STANDARD_NAME_VN", typeof(string));
                            dt.Columns.Add("USE_YN", typeof(string));

                            dt.Rows.Add();

                            dt.Rows[dt.Rows.Count - 1]["STT"] = (gvwDetail.RowCount + 1).ToString();
                            dt.Rows[dt.Rows.Count - 1]["KEY_CD"] = "";
                            dt.Rows[dt.Rows.Count - 1]["KEY_NAME_EN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["KEY_NAME_VN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["STANDARD_NAME_EN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["STANDARD_NAME_VN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["USE_YN"] = "Y";

                            grdDetail.DataSource = dt;
                        }
                        else
                        {
                            gvwDetail.FocusedRowHandle = gvwDetail.RowCount - 1;
                            Hashtable ht = new Hashtable();

                            ht.Add("STT", (gvwDetail.RowCount + 1).ToString());
                            ht.Add("KEY_CD", "");
                            ht.Add("KEY_NAME_EN", "");
                            ht.Add("KEY_NAME_VN", "");
                            ht.Add("STANDARD_NAME_EN", "");
                            ht.Add("STANDARD_NAME_VN", "");
                            ht.Add("USE_YN", "Y");

                            GridAddNewRow(ht, grdDetail);
                        }
                        break;
                    case "ITEM":
                        if (grdDetail.DataSource == null || gvwDetail.RowCount < 1)
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("STT", typeof(string));
                            dt.Columns.Add("ITEM_CD", typeof(string));
                            dt.Columns.Add("AREA_CD", typeof(string));
                            dt.Columns.Add("PROCESS_NAME_EN", typeof(string));
                            dt.Columns.Add("PROCESS_NAME_VN", typeof(string));
                            dt.Columns.Add("CHECKPOINT_NAME_EN", typeof(string));
                            dt.Columns.Add("CHECKPOINT_NAME_VN", typeof(string));
                            dt.Columns.Add("USE_YN", typeof(string));

                            dt.Rows.Add();

                            dt.Rows[dt.Rows.Count - 1]["STT"] = (gvwDetail.RowCount + 1).ToString();
                            dt.Rows[dt.Rows.Count - 1]["ITEM_CD"] = "";
                            dt.Rows[dt.Rows.Count - 1]["AREA_CD"] = "";
                            dt.Rows[dt.Rows.Count - 1]["PROCESS_NAME_EN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["PROCESS_NAME_VN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["CHECKPOINT_NAME_EN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["CHECKPOINT_NAME_VN"] = "";
                            dt.Rows[dt.Rows.Count - 1]["USE_YN"] = "Y";

                            grdDetail.DataSource = dt;
                        }
                        else
                        {
                            gvwDetail.FocusedRowHandle = gvwDetail.RowCount - 1;
                            Hashtable ht = new Hashtable();

                            ht.Add("STT", (gvwDetail.RowCount + 1).ToString());
                            ht.Add("ITEM_CD", "");
                            ht.Add("AREA_CD", "");
                            ht.Add("PROCESS_NAME_EN", "");
                            ht.Add("PROCESS_NAME_VN", "");
                            ht.Add("CHECKPOINT_NAME_EN", "");
                            ht.Add("CHECKPOINT_NAME_VN", "");
                            ht.Add("USE_YN", "Y");

                            GridAddNewRow(ht, grdDetail);
                        }

                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchData();
        }

        public void SearchData()
        {
            try
            {
                InitControls(grdDetail);
                string _qType = "";

                switch (_state)
                {
                    case "KEY":
                        _qType = "Q_KEY";
                        break;
                    case "ITEM":
                        _qType = "Q_ITEM";
                        break;
                    default:
                        break;
                }

                DataTable _dtSource = GetData(_qType,
                                            cboFactory.EditValue.ToString(),
                                            cboPlant.EditValue.ToString(),
                                            cboLine.EditValue.ToString(),
                                            cboStyle.EditValue.ToString(),
                                            txtModelCode.Text.ToString(),
                                            string.IsNullOrEmpty(cboKey.EditValue.ToString()) ? "" : cboKey.EditValue.ToString());

                if (_dtSource != null && _dtSource.Rows.Count > 0)
                {
                    switch (_state)
                    {
                        case "KEY":
                            for (int iRow = 0; iRow < _dtSource.Rows.Count; iRow++)
                            {
                                byte[] data = Convert.FromBase64String(_dtSource.Rows[iRow]["KEY_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["KEY_NAME_VN"] = Encoding.UTF8.GetString(data);

                                byte[] dataStandard = Convert.FromBase64String(_dtSource.Rows[iRow]["STANDARD_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["STANDARD_NAME_VN"] = Encoding.UTF8.GetString(dataStandard);
                            }
                            break;
                        case "ITEM":
                            for (int iRow = 0; iRow < _dtSource.Rows.Count; iRow++)
                            {
                                byte[] data = Convert.FromBase64String(_dtSource.Rows[iRow]["PROCESS_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["PROCESS_NAME_VN"] = Encoding.UTF8.GetString(data);

                                byte[] dataStandard = Convert.FromBase64String(_dtSource.Rows[iRow]["CHECKPOINT_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["CHECKPOINT_NAME_VN"] = Encoding.UTF8.GetString(dataStandard);
                            }
                            break;
                        default:
                            break;
                    }

                    SetData(grdDetail, _dtSource);
                    Format_Grid_Base();
                }

            }
            catch {
 
            }
        }

        public bool Allow_Update()
        {
            try
            {
                bool _result = true;
                DataTable _dtf = GetDataTable(gvwDetail);

                if (_dtf == null || _dtf.Rows.Count < 1) return false;

                switch (_state)
                {
                    case "KEY":
                        for (int iRow = 0; iRow < _dtf.Rows.Count; iRow++)
                        {
                            if (string.IsNullOrEmpty(_dtf.Rows[iRow]["KEY_NAME_EN"].ToString()))
                            {
                                _result = false;
                                break;
                            }
                        }
                        break;
                    case "ITEM":
                        for (int iRow = 0; iRow < _dtf.Rows.Count; iRow++)
                        {
                            if (string.IsNullOrEmpty(_dtf.Rows[iRow]["PROCESS_NAME_EN"].ToString()) ||
                                string.IsNullOrEmpty(_dtf.Rows[iRow]["CHECKPOINT_NAME_EN"].ToString()))
                            {
                                _result = false;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                return _result;
            }
            catch
            {
                return false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlr;
                bool _allow_update = true;

                switch (_state)
                {
                    case "KEY":
                        _allow_update = Allow_Update();

                        if (_allow_update)
                        {
                            dlr = MessageBox.Show("Bạn có muốn thêm Key Point mới cho Model này không?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        }
                        else
                        {
                            MessageBox.Show("Tên Key Point không được để trống!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (dlr == DialogResult.Yes)
                        {
                            bool result = SaveData("SAVE_KEY");
                            if (result)
                            {
                                MessageBoxW("Save successfully!", IconType.Information);
                                _isSaved = true;
                                SearchData();
                            }
                            else
                            {
                                MessageBoxW("Save failed!", IconType.Warning);
                            }
                        }
                        break;
                    case "ITEM":
                        _allow_update = Allow_Update();

                        if (_allow_update)
                        {
                            dlr = MessageBox.Show("Bạn có muốn thêm dữ liệu này không?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        }
                        else
                        {
                            MessageBox.Show("Tên Process và Checkpoint không được để trống!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (dlr == DialogResult.Yes)
                        {
                            bool result = SaveData("SAVE_ITEM");
                            if (result)
                            {
                                MessageBoxW("Save successfully!", IconType.Information);
                                _isSaved = true;
                                SearchData();
                            }
                            else
                            {
                                MessageBoxW("Save failed!", IconType.Warning);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvwDetail_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (grdDetail.DataSource == null || gvwDetail.RowCount < 1) return;

                if (e.Column.FieldName.Contains("USE_YN"))
                {
                    if (e.Value.ToString() == "Y")
                    {
                        gvwDetail.SetRowCellValue(e.RowHandle, "USE_YN", "Y");
                    }
                    else
                    {
                        gvwDetail.SetRowCellValue(e.RowHandle, "USE_YN", "N");
                    }
                }
            }
            catch { }
        }

        private void gvwDetail_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (grdDetail.DataSource == null || gvwDetail.RowCount < 1) return;

                if (e.Column.FieldName == "AREA_CD")
                {
                    if (gvwDetail.RowCount == 0) return;

                    RepositoryItemLookUpEditEx lookup = new RepositoryItemLookUpEditEx
                    {
                        DataSource = _dtProcess,
                        DisplayMember = "NAME",
                        ValueMember = "CODE",
                        NullText = ""
                    };
                    DevExpress.XtraEditors.Controls.LookUpColumnInfo col1 = new DevExpress.XtraEditors.Controls.LookUpColumnInfo();
                    col1.FieldName = "CODE";
                    col1.Caption = "Code";
                    DevExpress.XtraEditors.Controls.LookUpColumnInfo col2 = new DevExpress.XtraEditors.Controls.LookUpColumnInfo();
                    col2.FieldName = "NAME";
                    col2.Caption = "Process Name";
                    col1.Visible = false;
                    col2.Visible = true;
                    lookup.Columns.Add(col1);
                    lookup.Columns.Add(col2);
                    lookup.EditValueChanged += Lookup_EditValueChanged;
                    e.RepositoryItem = lookup;
                    e.Column.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
                    lookup.AppearanceDropDown.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lookup.AppearanceDropDown.Options.UseFont = true;
                }
            }
            catch { }
        }

        private void Lookup_EditValueChanged(object sender, EventArgs e)
        {
            LookUpColumnEdit lookup = ((LookUpColumnEdit)sender);

            if (lookup.GetColumnValue("NAME").ToString() != null)
            {
                string item2 = lookup.GetColumnValue("NAME").ToString();
                gvwDetail.SetRowCellValue(gvwDetail.FocusedRowHandle, "AREA_NM", item2);
            }
        }

        private void cboLine_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                if (_state.Equals("ITEM"))
                {
                    LoadDataCbo(cboStyle,
                                    "Style",
                                    "Q_STYLE",
                                    cboFactory.EditValue.ToString(),
                                    cboPlant.EditValue.ToString(),
                                    cboLine.EditValue.ToString());
                }
            }
        }

        private void gvwDetail_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (grdDetail.DataSource == null || gvwDetail.RowCount < 1) return;

                if (e.Column.FieldName.ToString().Equals("STT"))
                {
                    e.Appearance.BackColor = Color.LightYellow;
                }
            }
            catch { }
        }

        private void cboKey_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                getKeyPoint();
            }
        }

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

        private void txtStyle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtStyle.Text != null)
                {
                    LoadDataCbo(cboStyle, "Style", "Q_STYLE_ALL", "", "", "", "" ,"", txtStyle.Text.ToString().Trim());
                }
                txtStyle.Text = "";
            }
        }

        private void cboStyle_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                getModel();

                if (_state.Equals("ITEM"))
                {
                    LoadDataCbo(cboKey,
                            "Key Point",
                            "Q_CBO_KEY",
                            cboFactory.EditValue.ToString(),
                            cboPlant.EditValue.ToString(),
                            cboLine.EditValue.ToString(),
                            cboStyle.EditValue.ToString(),
                            txtModelCode.Text.ToString(),
                            "");

                    txtStyle.Text = cboStyle.EditValue.ToString();
                }
            }
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
                _ParamInfo.Add(new ParamInfo("@ARG_PLANT", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_LINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MLINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STYLE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL_NM", "Varchar", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_KEY_CD", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_KEY_NAME_EN", "Varchar2", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_KEY_NAME_VN", "Varchar2", 0, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_STANDARD_NAME_EN", "Varchar", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STANDARD_NAME_VN", "Varchar2", 0, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_ITEM_CD", "Varchar2", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_AREA_CD", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_AREA_NM", "Varchar", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_PROCESS_NAME_EN", "Varchar", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PROCESS_NAME_VN", "Varchar", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_CHECKPOINT_NAME_EN", "Varchar", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_CHECKPOINT_NAME_VN", "Varchar", 0, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_ISSUE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_USE_YN", "Varchar", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_CREATE_PC", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_CREATE_PROGRAM_ID", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable,
                                        System.String ARG_WORK_TYPE,
                                        System.String ARG_PLANT,
                                        System.String ARG_LINE,
                                        System.String ARG_MLINE,
                                        System.String ARG_STYLE,
                                        System.String ARG_MODEL,
                                        System.String ARG_MODEL_NM,

                                        System.String ARG_KEY_CD,
                                        System.String ARG_KEY_NAME_EN,
                                        System.String ARG_KEY_NAME_VN,

                                        System.String ARG_STANDARD_NAME_EN,
                                        System.String ARG_STANDARD_NAME_VN,

                                        System.String ARG_ITEM_CD,
                                        System.String ARG_AREA_CD,
                                        System.String ARG_AREA_NM,

                                        System.String ARG_PROCESS_NAME_EN,
                                        System.String ARG_PROCESS_NAME_VN,
                                        System.String ARG_CHECKPOINT_NAME_EN,
                                        System.String ARG_CHECKPOINT_NAME_VN,
                                        System.String ARG_ISSUE,
                                        System.String ARG_USE_YN,

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
                    ARG_WORK_TYPE,
                    ARG_PLANT,
                    ARG_LINE,
                    ARG_MLINE,
                    ARG_STYLE,
                    ARG_MODEL,
                    ARG_MODEL_NM,

                    ARG_KEY_CD,
                    ARG_KEY_NAME_EN,
                    ARG_KEY_NAME_VN,

                    ARG_STANDARD_NAME_EN,
                    ARG_STANDARD_NAME_VN,

                    ARG_ITEM_CD,
                    ARG_AREA_CD,
                    ARG_AREA_NM,

                    ARG_PROCESS_NAME_EN,
                    ARG_PROCESS_NAME_VN,
                    ARG_CHECKPOINT_NAME_EN,
                    ARG_CHECKPOINT_NAME_VN,
                    ARG_ISSUE,
                    ARG_USE_YN,

                    ARG_CREATE_PC,
                    ARG_CREATE_PROGRAM_ID
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

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

        #endregion
    }
}

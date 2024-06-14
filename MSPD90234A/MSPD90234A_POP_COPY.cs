using DevExpress.XtraEditors;
using JPlatform.Client.Controls6;
using JPlatform.Client.CSIGMESBaseform6;
using JPlatform.Client.JBaseForm6;
using JPlatform.Client.Library6.interFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CSI.GMES.PD
{
    public partial class MSPD90234A_POP_COPY : CSIGMESBaseform6
    {
        #region [Variables]

        private string _factory = "", _plant = "", _line = "", _style = "", _model = "";
        public bool _isSaved = false, _firstLoad = true;
        public DataTable _dtStyle = null, _dtStyleNew = null;

        #endregion

        #region [Start Button Event Code By UIBuilder]

        public MSPD90234A_POP_COPY()
        {
            InitializeComponent();
        }

        public MSPD90234A_POP_COPY(string _factory_cd, string _plant_cd, string _line_cd, string _style_cd, string _model_cd)
        {
            InitializeComponent();
            _factory = _factory_cd;
            _plant = _plant_cd;
            _line = _line_cd;
            _style = _style_cd;
            _model = _model_cd;
        }

        public void SetBrowserMain(JPlatform.Client.Library6.interFace.IBrowserMain browserMain)
        {
            this._browserMain = browserMain;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _firstLoad = true;

            txtModelCode.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
            txtModelCode.BackColor = Color.FromArgb(255, 228, 225);
            txtModelName.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
            txtModelName.BackColor = Color.FromArgb(255, 228, 225);
            txtModelCodeNew.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
            txtModelCodeNew.BackColor = Color.FromArgb(255, 228, 225);
            txtModelNameNew.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
            txtModelNameNew.BackColor = Color.FromArgb(255, 228, 225);

            txtStyle.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);
            txtStyle.BackColor = Color.FromArgb(255, 228, 225);
            txtStyleNew.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);

            groupControlEx2.BackColor = Color.FromArgb(245, 245, 245);
            groupControlEx3.BackColor = Color.FromArgb(245, 245, 245);

            fn_cbo_load(cboFactory, "Q_FTY", "Factory");
            fn_cbo_load(cboFactoryNew, "Q_FTY", "Factory");
            cboFactory.EditValue = _factory;

            fn_cbo_load(cboPlant, 
                        "Q_LINE", 
                        "Plant", 
                        cboFactory.EditValue.ToString());
            fn_cbo_load(cboPlantNew, 
                        "Q_LINE", 
                        "Plant",
                        cboFactoryNew.EditValue.ToString());
            cboPlant.EditValue = _plant;

            fn_cbo_load(cboLine,
                        "Q_MLINE", 
                        "Line",
                        cboFactory.EditValue.ToString(),
                        cboPlant.EditValue.ToString());
            fn_cbo_load(cboLineNew,
                        "Q_MLINE", 
                        "Line",
                        cboFactoryNew.EditValue.ToString(),
                        cboPlantNew.EditValue.ToString());
            cboLine.EditValue = _line;

            fn_cbo_load(cboStyle,
                        "Q_STYLE",
                        "Style",
                        cboFactory.EditValue.ToString(),
                        cboPlant.EditValue.ToString(),
                        cboLine.EditValue.ToString());
            fn_cbo_load(cboStyleNew,
                        "Q_STYLE_ALL",
                        "Style",
                        cboFactoryNew.EditValue.ToString(),
                        cboPlantNew.EditValue.ToString(),
                        cboLineNew.EditValue.ToString());

            txtStyle.Text = cboStyle.EditValue.ToString();
            getModel("OLD");
            getModel("NEW");

            //fn_cbo_load("Q_STYLE", cboFactory.EditValue.ToString());
            //cboStyle.EditValue = _style;
            //fn_cbo_load("Q_STYLE_ALL");

            _firstLoad = false;
        }

        public void getModel(string _sType)
        {
            try
            {
                if (_sType.Equals("OLD"))
                {
                    if (_dtStyle == null || _dtStyle.Rows.Count < 1)
                    {
                        txtModelCode.Text = "";
                        txtModelName.Text = "";

                        return;
                    }

                    string _selected_style = cboStyle.EditValue.ToString();

                    for (int iRow = 0; iRow < _dtStyle.Rows.Count; iRow++)
                    {
                        if (_dtStyle.Rows[iRow]["CODE"].ToString().Equals(_selected_style))
                        {
                            txtModelCode.Text = _dtStyle.Rows[iRow]["MODEL_CD"].ToString();
                            txtModelName.Text = _dtStyle.Rows[iRow]["MODEL_NAME"].ToString();
                            break;
                        }
                    }
                } else if (_sType.Equals("NEW"))
                {
                    if (_dtStyleNew == null || _dtStyleNew.Rows.Count < 1)
                    {
                        txtModelCodeNew.Text = "";
                        txtModelNameNew.Text = "";

                        return;
                    }

                    string _selected_style = cboStyleNew.EditValue.ToString();

                    for (int iRow = 0; iRow < _dtStyleNew.Rows.Count; iRow++)
                    {
                        if (_dtStyleNew.Rows[iRow]["CODE"].ToString().Equals(_selected_style))
                        {
                            txtModelCodeNew.Text = _dtStyleNew.Rows[iRow]["MODEL_CD"].ToString();
                            txtModelNameNew.Text = _dtStyleNew.Rows[iRow]["MODEL_NAME"].ToString();
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        public bool CheckIsSaved()
        {
            return _isSaved;
        }

        #endregion [End DB Related Code] 

        #region [Control Event Code By UIBuilder]

        #endregion [End Control Event Code]

        #region [New function from programmer]

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlr;
                string _question = "Bạn có muốn Copy dữ liệu không?";

                DataTable _dtCheck = GetData("Q_CHECK",
                                            cboFactoryNew.EditValue.ToString(),
                                            cboPlantNew.EditValue.ToString(),
                                            cboLineNew.EditValue.ToString(),
                                            cboStyleNew.EditValue.ToString(),
                                            txtModelCodeNew.Text.ToString());
                if(_dtCheck != null && _dtCheck.Rows.Count > 0)
                {
                    _question = "Key Point cho Model ở xưởng này đã tồn tại! Bạn có muốn tiếp tục Save không?";
                }

                if (cboStyle.EditValue == null || string.IsNullOrEmpty(cboStyle.EditValue.ToString()))
                {
                    MessageBox.Show("Không có dữ liệu để Copy!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (cboFactory.EditValue.ToString() == cboFactoryNew.EditValue.ToString() &&
                        cboPlant.EditValue.ToString() == cboPlantNew.EditValue.ToString() &&
                        cboLine.EditValue.ToString() == cboLineNew.EditValue.ToString() &&
                        cboStyle.EditValue.ToString() == cboStyleNew.EditValue.ToString() &&
                        txtModelCode.Text.ToString() == txtModelCodeNew.Text.ToString())
                {
                    MessageBox.Show("Chọn dữ liệu khác để Copy!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    dlr = MessageBox.Show(_question, "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }

                if (dlr == DialogResult.Yes)
                {
                    {
                        bool result = SaveData("Q_COPY");
                        if (result)
                        {
                            MessageBoxW("Save successfully!", IconType.Information);
                            _isSaved = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBoxW("Save failed!", IconType.Warning);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public bool SaveData(string _type)
        {
            try
            {
                bool _result = true;
                DataTable dtData = null;
                P_MSPD90234A_POP proc = new P_MSPD90234A_POP();
                string machineName = $"{SessionInfo.UserName}|{ Environment.MachineName}|{GetIPAddress()}";

                dtData = proc.SetParamData(dtData,
                                          _type,
                                          cboFactory.EditValue.ToString(),
                                          cboPlant.EditValue.ToString(),
                                          cboLine.EditValue.ToString(),
                                          cboStyle.EditValue.ToString(),
                                          txtModelCode.Text.ToString(),

                                          cboFactoryNew.EditValue.ToString(),
                                          cboPlantNew.EditValue.ToString(),
                                          cboLineNew.EditValue.ToString(),
                                          cboStyleNew.EditValue.ToString(),
                                          txtModelCodeNew.Text.ToString(),
                                          txtModelNameNew.Text.ToString(),

                                          machineName,
                                          "CSI.GMES.PD.MSPD90234A_COPY");

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    _result = CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), null);
                }
                else
                {
                    _result = false;
                }

                return _result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public string GetStyleName(string _style_cd)
        {
            string _result = "";

            for (int iRow = 0; iRow < _dtStyle.Rows.Count; iRow++)
            {
                if (_dtStyle.Rows[iRow]["CODE"].ToString().Equals(_style_cd))
                {
                    _result = _dtStyle.Rows[iRow]["NAME"].ToString();
                    break;
                }
            }

            return _result;
        }

        private void cboStyle_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                getModel("OLD");
            }
        }

        private void cboStyleNew_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                getModel("NEW");
            }
        }

        private void cboFactoryNew_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                fn_cbo_load(cboPlantNew,
                         "Q_LINE",
                         "Plant",
                         cboFactoryNew.EditValue.ToString());
            }
        }

        private void cboPlantNew_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                fn_cbo_load(cboLineNew,
                         "Q_MLINE",
                         "Plant",
                         cboFactoryNew.EditValue.ToString(),
                         cboPlantNew.EditValue.ToString());
            }
        }

        private void txtModel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtStyleNew.Text != null)
                {
                    fn_cbo_load(cboStyleNew, "Q_STYLE_ALL", "Style", "", "", "", "", "", txtStyleNew.Text.ToString().Trim());
                }
                txtStyleNew.Text = "";
            }
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
                return null;
            }
        }

        #endregion

        #region [Combobox]
        public void fn_cbo_load(LookUpEditEx argCbo, string COMBO_TYPE, string _combo_nm, string _factory = "", string _line = "", string _mline = "", string _style = "", string _model = "", string _condition = "")
        {
            try
            {
                P_MSPD90234A_Q cProc = new P_MSPD90234A_Q();
                DataTable dtData = null;

                dtData = cProc.SetParamData(dtData, COMBO_TYPE, _factory, _line, _mline, _style, _model, _condition);
                ResultSet rs = CommonCallQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), false, 90000, "", true);

                if (rs != null && rs.ResultDataSet != null && rs.ResultDataSet.Tables.Count > 0 && rs.ResultDataSet.Tables[0].Rows.Count > 0)
                {
                    DataTable dtf = rs.ResultDataSet.Tables[0];
                    if (dtf.Rows.Count > 0)
                    {
                        SetComboBox(dtf, argCbo, _combo_nm);
                    }

                    if (COMBO_TYPE.Equals("Q_STYLE"))
                    {
                        _dtStyle = dtf.Copy();
                        cboStyle.Properties.Columns[dtf.Columns[0].ColumnName].Visible = true;
                        cboStyle.Properties.Columns[dtf.Columns[0].ColumnName].Width = 10;
                    }
                    else if (COMBO_TYPE.Equals("Q_STYLE_ALL"))
                    {
                        _dtStyleNew = dtf.Copy();
                        cboStyleNew.Properties.Columns[dtf.Columns[0].ColumnName].Visible = true;
                        cboStyleNew.Properties.Columns[dtf.Columns[0].ColumnName].Width = 10;
                    }
                }
                else
                {
                    argCbo.Properties.Columns.Clear();
                    argCbo.Properties.DataSource = null;

                    if (COMBO_TYPE == "Q_STYLE")
                    {
                        _dtStyle = null;
                    }
                    else if (COMBO_TYPE.Equals("Q_STYLE_ALL"))
                    {
                        _dtStyleNew = null;
                    }
                }

                cProc = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetComboBox(DataTable dtData, LookUpEditEx cboName, string colName)
        {
            try
            {
                cboName.Properties.Columns.Clear();
                cboName.Properties.DataSource = dtData;
                string col1 = dtData.Columns[0].ColumnName;
                string col2 = dtData.Columns[1].ColumnName;
                cboName.Properties.ValueMember = col1;
                cboName.Properties.DisplayMember = col2;
                cboName.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(col1));
                cboName.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(col2));
                cboName.Properties.Columns[col1].Caption = "Code";
                cboName.Properties.Columns[col2].Caption = colName;
                cboName.Properties.Columns[col1].Visible = false;
                cboName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        public class P_MSPD90234A_POP : BaseProcClass
        {
            public P_MSPD90234A_POP()
            {
                // Modify Code : Procedure Name
                _ProcName = "LMES.P_MSPD90234A_POP";
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

                _ParamInfo.Add(new ParamInfo("@ARG_PLANT_NEW", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_LINE_NEW", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MLINE_NEW", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STYLE_NEW", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL_NEW", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL_NM_NEW", "Varchar", 100, "Input", typeof(System.String)));

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

                                        System.String ARG_PLANT_NEW,
                                        System.String ARG_LINE_NEW,
                                        System.String ARG_MLINE_NEW,
                                        System.String ARG_STYLE_NEW,
                                        System.String ARG_MODEL_NEW,
                                        System.String ARG_MODEL_NM_NEW,

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

                    ARG_PLANT_NEW,
                    ARG_LINE_NEW,
                    ARG_MLINE_NEW,
                    ARG_STYLE_NEW,
                    ARG_MODEL_NEW,
                    ARG_MODEL_NM_NEW,

                    ARG_CREATE_PC,
                    ARG_CREATE_PROGRAM_ID
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        #endregion
    }
}
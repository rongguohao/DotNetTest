﻿using Newtonsoft.Json;
using OmronFinsTCP.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FinsTcpTool
{
    public partial class Form1 : Form
    {
        EtherNetPLC _etherNetPLC;

        private string _ip;

        private ushort _port;

        private bool _isConnected = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btn_Close.Enabled = false;
            LocalConfig.Instance.LoadConfig();
            txt_IP.Text = LocalConfig.Instance.IP;
            txt_Port.Text = LocalConfig.Instance.Port.ToString();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Connect_Click(object sender, EventArgs e)
        {
            if (!ValidateParam()) return;

            try
            {
                await Task.Run(async () => {

                    this.Invoke(new Action(() => {
                        this.btn_Connect.Text = "连接中...";
                    }));

                    await Task.Delay(1000);

                    _etherNetPLC = new EtherNetPLC();

                    if (_etherNetPLC.Link(_ip, (short)_port, 5000) == 0)
                    {
                        _isConnected = true;

                        this.Invoke(new Action(() => {
                            this.btn_Connect.Text = "已连接...";
                            this.btn_Connect.Enabled = false;
                            this.btn_Close.Enabled = true;
                        }));
                    }
                    else
                    {
                        MessageBox.Show("连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                });
            }
            catch (Exception ex)
            {
                this.btn_Connect.Text = "连接";
                MessageBox.Show($"连接失败! {ex}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Close_Click(object sender, EventArgs e)
        {
            if (!_isConnected || _etherNetPLC == null) return;

            this.btn_Close.Enabled = false;
            try
            {
                await Task.Run(() => {

                    if (_etherNetPLC.Close() == 0)
                    {
                        _isConnected = false;
                        this.Invoke(new Action(() => {
                            this.btn_Connect.Text = "连接";
                            this.btn_Connect.Enabled = true;
                            this.btn_Close.Enabled = false;
                        }));
                    }
                    else
                    {
                        this.btn_Close.Enabled = true;
                        MessageBox.Show($"断开失败! ", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"断开失败! {ex}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存参数配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidateParam()) return;

            if (LocalConfig.Instance.IP == _ip && LocalConfig.Instance.Port == _port) return;

            LocalConfig.Instance.IP = _ip;

            LocalConfig.Instance.Port = _port;

            LocalConfig.Instance.SaveConfig();
        }


        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        private bool ValidateParam()
        {
            _ip = txt_IP.Text.Trim();

            if (string.IsNullOrWhiteSpace(_ip))
            {
                MessageBox.Show("请填写IP地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(_ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                MessageBox.Show("IP数据异常，请检查", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_Port.Text))
            {
                MessageBox.Show("请填写端口号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (ushort.TryParse(txt_Port.Text.Trim(), out ushort port))
            {
                _port = port;
            }
            else
            {
                MessageBox.Show("端口号异常，请检查", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// bool写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_WriteBool_Click(object sender, EventArgs e)
        {
            if (_etherNetPLC == null)
            {
                MessageBox.Show("请先连接通讯地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_InAddr.Text))
            {
                MessageBox.Show("请填写地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_InValue.Text))
            {
                MessageBox.Show("请填写值", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string writeValue = txt_InValue.Text.Trim();

            bool flag = false;

            if (writeValue == "1")
            {
                flag = true;
            }
            else if(writeValue == "0")
            {
                flag = false;
            }
            else
            {
                try
                {
                    flag = bool.Parse(txt_InValue.Text.Trim());
                }
                catch (Exception)
                {
                    MessageBox.Show("写入值有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            string writeAddr = txt_InAddr.Text.Trim();

            if(!writeAddr.Contains(".")) writeAddr += ".0";

            _etherNetPLC.SetBitState(PlcMemory.DM, writeAddr, flag ? BitState.ON : BitState.OFF);
        }

        /// <summary>
        /// bool读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReadBool_Click(object sender, EventArgs e)
        {
            if (_etherNetPLC == null)
            {
                MessageBox.Show("请先连接通讯地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_OutAddr.Text))
            {
                MessageBox.Show("请填写地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string readAddr = txt_OutAddr.Text.Trim();

            if (!readAddr.Contains(".")) readAddr += ".0";

            if(_etherNetPLC.GetBitState(PlcMemory.DM, readAddr, out short data) == 0)
            {
                SetListBoxData($"[{DateTime.Now.ToString("HH:mm:ss fff")}]  {(data == 1 ? "True" : "False")}");
            }
            else
            {
                MessageBox.Show("读取失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// bool数组写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_WriteBools_Click(object sender, EventArgs e)
        {
            if (_etherNetPLC == null)
            {
                MessageBox.Show("请先连接通讯地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_InAddr.Text))
            {
                MessageBox.Show("请填写地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_InValue.Text))
            {
                MessageBox.Show("请填写值", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool[] boolsArray;

            try
            {
                boolsArray = JsonConvert.DeserializeObject<bool[]>(txt_InValue.Text.Trim().ToLower());

                if (boolsArray.Length == 0)
                {
                    MessageBox.Show("数组不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("值有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }

            string writeAddr = txt_InAddr.Text.Trim();

            if (writeAddr.Contains(".")) writeAddr = writeAddr.Split('.')[0];

            var shortValue = BitsToWord(boolsArray);

            _etherNetPLC.WriteWord(PlcMemory.DM, short.Parse(writeAddr), shortValue);
        }

        /// <summary>
        /// bool数组读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReadBools_Click(object sender, EventArgs e)
        {
            if (_etherNetPLC == null)
            {
                MessageBox.Show("请先连接通讯地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_OutAddr.Text))
            {
                MessageBox.Show("请填写地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string readAddr = txt_OutAddr.Text.Trim();

            if (readAddr.Contains(".")) readAddr = readAddr.Split('.')[0];

            if (_etherNetPLC.ReadWord(PlcMemory.DM, short.Parse(readAddr), out short data) == 0)
            {
                var result = ConvertToArray(data);

                //var resultBytes = BitConverter.GetBytes(data);

                //var resultBOols = ConvertByteArrayToBoolArray(resultBytes);

                SetListBoxData($"[{DateTime.Now.ToString("HH:mm:ss fff")}]  {JsonConvert.SerializeObject(result)}");
            }
            else
            {
                MessageBox.Show("读取失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        private  short BitsToWord(bool[] bits)
        {
            short result = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    result |= (short)(1 << i);
            }
            return result;
        }



        /// <summary>
        /// Convert Byte Array To Bool Array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool[] ConvertByteArrayToBoolArray(byte[] bytes)
        {
            System.Collections.BitArray b = new System.Collections.BitArray(bytes);
            bool[] bitValues = new bool[b.Count];
            b.CopyTo(bitValues, 0);
            return bitValues;
        }

        ///// <summary>
        ///// Packs a bit array into bytes, most significant bit first
        ///// </summary>
        ///// <param name="boolArr"></param>
        ///// <returns></returns>
        //public static byte[] ConvertBoolArrayToByteArray(bool[] boolArr)
        //{
        //    byte[] byteArr = new byte[(boolArr.Length + 7) / 8];
        //    for (int i = 0; i < byteArr.Length; i++)
        //    {
        //        byteArr[i] = ReadByte(boolArr, 8 * i);
        //    }
        //    return byteArr;
        //}


        private bool[] ConvertToArray(short @short)
        {
            var result = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = (@short & (short)1) == (short)1 ? true : false;
                @short = (short)(@short >> 1);
            }
            return result;

        }


        //private byte[] PackBoolsInByteArray(bool[] bools)
        //{
        //    int len = bools.Length;
        //    int bytes = len >> 3;
        //    if ((len & 0x07) != 0) ++bytes;
        //    byte[] arr2 = new byte[bytes];
        //    for (int i = 0; i < bools.Length; i++)
        //    {
        //        if (bools[i])
        //            arr2[i >> 3] |= (byte)(1 << (i & 0x07));
        //    }
        //    return arr2;
        //}

        //IEnumerable<byte> PackBools(IEnumerable<bool> bools)
        //{
        //    int bitIndex = 0;
        //    byte currentByte = 0;
        //    foreach (bool val in bools)
        //    {
        //        if (val)
        //            currentByte |= (byte)(1 << bitIndex);
        //        if (++bitIndex == 8)
        //        {
        //            yield return currentByte;
        //            bitIndex = 0;
        //            currentByte = 0;
        //        }
        //    }
        //    if (bitIndex != 8)
        //    {
        //        yield return currentByte;
        //    }
        //}


        //public short toShort(byte[] b)
        //{
        //    short s = 0;
        //    short s0 = (short)(b[0] & 0xff);// 最低位  
        //    short s1 = (short)(b[1] & 0xff);
        //    s1 <<= 8;
        //    s = (short)(s0 | s1);
        //    return s;
        //}

        private void SetListBoxData(string str)
        {
            if (lstBox.InvokeRequired)
            {
                Action<string> actionDelegate = (x) =>
                {

                    lstBox.Items.Add(str);

                    lstBox.TopIndex = lstBox.Items.Count - (int)(lstBox.Height / lstBox.ItemHeight);

                };

                lstBox.Invoke(actionDelegate, str);
            }
            else
            {
                lstBox.Items.Add(str);

                lstBox.TopIndex = lstBox.Items.Count - (int)(lstBox.Height / lstBox.ItemHeight);
            }
        }
    }
}

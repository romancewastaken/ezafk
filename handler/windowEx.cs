using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using static ezafk.handler.nativeMethods;
using System.Windows.Forms;

namespace ezafk.handler
{
    internal class windowEx
    {
        private readonly Form _formObject = null;

        public List<Tuple<Control, int, int>> _shadowControls = new List<Tuple<Control, int, int>>();
        private Bitmap _shadowBmp = null;

        public windowEx(Form target, bool doDrag = true)
        {
            _formObject = target;

            if (doDrag)
                _formObject.MouseDown += FormObject_MouseDown;
        }

        private void DrawShadowSmooth(GraphicsPath gp, int intensity, int radius, Bitmap dest)
        {
            if (_formObject.Handle == null)
                return;

            using (Graphics g = Graphics.FromImage(dest))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceCopy;
                double alpha = 0;
                double astep = 0;
                double astepstep = (double)intensity / radius / (radius / 2D);
                for (int thickness = radius; thickness > 0; thickness--)
                {
                    using (Pen p = new Pen(Color.FromArgb((int)alpha, 0, 0, 0), thickness))
                    {
                        p.LineJoin = LineJoin.Round;
                        g.DrawPath(p, gp);
                    }
                    alpha += astep;
                    astep += astepstep;
                }
            }
        }
        public void Hook_Paint(object sender, PaintEventArgs e)
        {
            if (sender == null)
                return;
            if ((sender as Control).Handle == null)
                return;

            if (_shadowBmp == null || _shadowBmp.Size != _formObject.Size)
            {
                _shadowBmp?.Dispose();
                _shadowBmp = new Bitmap(_formObject.Width, _formObject.Height, PixelFormat.Format32bppArgb);
            }
            foreach (Tuple<Control, int, int> control in _shadowControls)
            {
                if (!control.Item1.Visible || !control.Item1.TopLevelControl.Visible)
                    continue;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRectangle(new Rectangle(control.Item1.Location.X, control.Item1.Location.Y, control.Item1.Size.Width, control.Item1.Size.Height));
                    DrawShadowSmooth(gp, control.Item2, control.Item3, _shadowBmp);
                    /// item2 usually 90, item3 usually 35
                }
                e.Graphics.DrawImage(_shadowBmp, new Point(0, 0));
            }
        }

        public void TryEnableDarkTheme()
        {
            int value = 1;

            if (NativeMethods.DwmSetWindowAttribute(_formObject.Handle, Constants.DWMA_USE_IMMERSIVE_DARK_MODE, ref value, 4) != 0)
            {
                NativeMethods.DwmSetWindowAttribute(_formObject.Handle, Constants.DWMA_USE_IMMERSIVE_DARK_MODE_OLD, ref value, 4);
            }
        }

        #region Window Movement

        public void FormObject_MouseDown(object source, MouseEventArgs ev)
        {
            if (ev.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();

                Type type = source.GetType().BaseType;

                if (type.FullName is "System.Windows.Forms.Form")
                {
                    NativeMethods.SendMessage(((Form)source).Handle, 0xA1, 0x2, 0);
                    return;
                }

                try
                {
                    Control ctrl = (Control)source;

                    NativeMethods.SendMessage(ctrl.FindForm().Handle, 0xA1, 0x2, 0);
                }
                catch
                {
                    return;
                }
            }
        }

        #endregion

        public bool AeroActive
        {
            get
            {
                try
                {
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        int enabled = 0;
                        NativeMethods.DwmIsCompositionEnabled(ref enabled);
                        return (enabled is 1);
                    }
                    else return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        #region DWM Shadow

        public void ShadowProc(ref Message m, IntPtr handle)
        {
            if ((uint)m.Msg is Constants.WM_NCPAINT)
            {
                if (AeroActive)
                {
                    var attrVal = 2;
                    NativeMethods.DwmSetWindowAttribute(handle, 2, ref attrVal, 4);

                    MARGINS wMargins = new MARGINS()
                    {
                        bottomHeight = 1,
                        leftWidth = 1,
                        rightWidth = 1,
                        topHeight = 1
                    };

                    NativeMethods.DwmExtendFrameIntoClientArea(handle, ref wMargins);
                }
            }
        }

        public void ShadowProc(ref Message m, IntPtr handle, MARGINS margins)
        {
            if ((uint)m.Msg is Constants.WM_NCPAINT)
            {
                if (AeroActive)
                {
                    var attrVal = 2;
                    NativeMethods.DwmSetWindowAttribute(handle, 2, ref attrVal, 4);
                    NativeMethods.DwmExtendFrameIntoClientArea(handle, ref margins);
                }
            }
        }

        #endregion

        #region Window Sizing

        public const int RESIZE_HANDLE_SIZE = 5;


        public void SizeProc(ref Message m, object source)
        {
            Type type = source.GetType().BaseType;

            if (type.FullName != "System.Windows.Forms.Form")
                return;

            Form window = (Form)source;

            if ((uint)m.Msg is Constants.WM_NCHITTEST)
            {
                if ((int)m.Result is 0x01)
                {
                    var screenPoint = new Point(m.LParam.ToInt32());
                    var clientPoint = window.PointToClient(screenPoint);

                    if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                    {
                        if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                            m.Result = (IntPtr)13;
                        else if (clientPoint.X < (window.Size.Width - RESIZE_HANDLE_SIZE))
                            m.Result = (IntPtr)12;
                        else
                            m.Result = (IntPtr)14;
                    }
                    else if (clientPoint.Y <= (window.Size.Height - RESIZE_HANDLE_SIZE))
                    {
                        if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                            m.Result = (IntPtr)10;
                        else if (clientPoint.X < (window.Size.Width - RESIZE_HANDLE_SIZE))
                            m.Result = (IntPtr)2;
                        else
                            m.Result = (IntPtr)11;
                    }
                    else
                    {
                        if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                            m.Result = (IntPtr)16;
                        else if (clientPoint.X < (window.Size.Width - RESIZE_HANDLE_SIZE))
                            m.Result = (IntPtr)15;
                        else
                            m.Result = (IntPtr)17;
                    }
                }
            }
        }

        #endregion
    }
}

using System;
using System.Runtime.InteropServices;

namespace ezafk.handler
{
    internal class nativeMethods
    {
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        /// <summary>
        /// Managed Wrapper Class for WinAPI Calls. Why? See <see href="https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2012/ms182161(v=vs.110)?redirectedfrom=MSDN">CA1060</see> for more info.
        /// </summary>
        internal class NativeMethods
        {
            /// <summary>
            /// Sends the specified message to a window or windows. See <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage">SendMessage function (winuser.h)</see> for more info.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose procedure will receive the message.</param>
            /// <param name="Msg">The message to be sent.  See <see href="https://docs.microsoft.com/en-us/windows/win32/winmsg/about-messages-and-message-queues">System Defined Messages</see> for more info.</param>
            /// <param name="wParam">Additional message specific information.</param>
            /// <param name="lParam">Additional message specific information.</param>
            /// <returns>Value of <see cref="int"/> (Win32 LRESULT) specifies the result of the message processing; it depends on the message sent. Use Marshal.GetLastWin32Error() for error details.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

            /// <summary>
            /// Releases the mouse capture from a window in the current thread and restores normal mouse input processing See <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-releasecapture">ReleaseCapture function (winuser.h)</see> for more info.
            /// </summary>
            /// <returns>If the function succeeds, the return value is nonzero. Use Marshal.GetLastWin32Error() for error details.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ReleaseCapture();

            /// <summary>
            /// Sets the value of Desktop Window Manager (DWM) non-client rendering attributes for a window. See <see href="https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute">DwmSetWindowAttribute function (dwmapi.h)</see> for more info.
            /// </summary>
            /// <param name="hwnd">The handle to the window for which the attribute value is to be set.</param>
            /// <param name="dwAttribute">A flag describing which value to set, specified as a value of the <see href="https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute">DWMWINDOWATTRIBUTE</see> enumeration. This parameter specifies which attribute to set, and the pvAttribute parameter points to an object containing the attribute value.</param>
            /// <param name="pvAttribute">A pointer to an object containing the attribute value to set. The type of the value set depends on the value of the dwAttribute parameter. </param>
            /// <param name="cbAttribute">The size, in bytes, of the attribute value being set via the pvAttribute parameter. The type of the value set, and therefore its size in bytes, depends on the value of the dwAttribute parameter.</param>
            /// <returns>If the function succeeds, it returns S_OK. Otherwise, it returns an <see href="https://docs.microsoft.com/en-us/windows/win32/com/structure-of-com-error-codes">HRESULT</see> error code. Use Marshal.GetLastWin32Error() for error details.</returns>
            [DllImport("dwmapi.dll", SetLastError = true)]
            public static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, ref int pvAttribute, int cbAttribute);

            /// <summary>
            /// Extends the window frame into the client area. See <see href="https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmextendframeintoclientarea">DwmExtendFrameIntoClientArea function (dwmapi.h)</see> for more info.
            /// </summary>
            /// <param name="hWnd">The handle to the window in which the frame will be extended into the client area.</param>
            /// <param name="pMarInset">A pointer to a <see cref="MARGINS"/> structure that describes the margins to use when extending the frame into the client area.</param>
            /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code. Use Marshal.GetLastWin32Error() for error details.</returns>
            [DllImport("dwmapi.dll", SetLastError = true)]
            public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

            /// <summary>
            /// Obtains a value that indicates whether Desktop Window Manager (DWM) composition is enabled. See <see href="https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmiscompositionenabled">DwmIsCompositionEnabled function (dwmapi.h)</see> for more info.
            /// </summary>
            /// <param name="pfEnabled">A pointer to a value that, when this function returns successfully, receives TRUE if DWM composition is enabled; otherwise, FALSE.</param>
            /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code. Use Marshal.GetLastWin32Error() for error details.</returns>
            [DllImport("dwmapi.dll", SetLastError = true)]
            public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        }

        /// <summary>
        /// Win32 API Constant Values.
        /// </summary>
        internal class Constants
        {
            /// <summary>
            /// The <see href="https://docs.microsoft.com/en-us/windows/win32/gdi/wm-ncpaint">WM_NCPAINT message</see> is sent to a window when its frame must be painted.
            /// </summary>
            public const uint WM_NCPAINT = 0x0085;
            /// <summary>
            /// Posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted. See <see href="https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nclbuttondown">WM_NCLBUTTONDOWN message</see> for more info.
            /// </summary>
            public const uint WM_NCLBUTTONDOWN = 0x00A1;
            /// <summary>
            /// Sent to a window in order to determine what part of the window corresponds to a particular screen coordinate.
            /// </summary>
            public const uint WM_NCHITTEST = 0x0084;
            /// <summary>
            /// Indicates the cursor is in a title bar. Part of WM_NCHITTEST message. See <see href="https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest">WM_NCHITTEST message</see> for more info.
            /// </summary>
            public const uint HT_CAPTION = 0x2;

            /// <summary>
            /// Undocumented DWM API | Enables Dark Titlebar on Win10
            /// </summary>
            public const uint DWMA_USE_IMMERSIVE_DARK_MODE = 0x00000014;
            /// <summary>
            /// Undocumented DWM API | Enables Dark Titlebar on Win10 (before 20H1)
            /// </summary>
            public const uint DWMA_USE_IMMERSIVE_DARK_MODE_OLD = 0x00000013;

            /// <summary>
            /// Window Class Value for CS_DROPSHADOW basic effect.
            /// </summary>
            public const uint CS_DROPSHADOW = 0x00020000;
        }
    }
}

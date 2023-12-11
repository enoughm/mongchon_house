using System;
using System.Runtime.InteropServices;

public class WindowHelper
{
  
    
    // Windows API를 가져오기 위한 라이브러리
    //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    // 실행 창을 최상위로 유지하기 위한 플래그
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


    // Unity의 실행창을 최상위로 유지하는 함수
    public static void MakeTopMost()
    {
        SetWindowPos(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }
    
    // 실행창을 최상위로 유지하는 함수를 취소하는 함수
    public static void DisableTopMost()
    {
        SetWindowPos(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ffxiv_fov
{
	public partial class Form1 : Form
	{
		[Flags]
		public enum ProcessAccessFlags : uint
		{
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VirtualMemoryOperation = 0x00000008,
			VirtualMemoryRead = 0x00000010,
			VirtualMemoryWrite = 0x00000020,
			DuplicateHandle = 0x00000040,
			CreateProcess = 0x000000080,
			SetQuota = 0x00000100,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			QueryLimitedInformation = 0x00001000,
			Synchronize = 0x00100000
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(
			 ProcessAccessFlags processAccess,
			 bool bInheritHandle,
			 int processId);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[Out] byte[] lpBuffer,
			int dwSize,
			out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			byte[] lpBuffer,
			int nSize,
			out IntPtr lpNumberOfBytesWritten);

		private Thread mainThread = null;

		private bool dx9WarningShown = false;

		private Process ffxiv = null;
		private IntPtr ffxivHandle = IntPtr.Zero;
		private IntPtr cameraOffset = IntPtr.Zero;
		// Indicates that the above variables have been set
		private bool ffxivFound = false;

		private static byte[] cameraPattern = new byte[] { 0x48, 0x8B, 0x1C, 0xC3, 0x48, 0x85, 0xDB, 0x0F, 0x84 };

		private const int fovOffset = 0x124;
		private const int minFovOffset = 0x128;
		private const int maxFovOffset = 0x12C;

		// Terminate main loop
		private bool exitLoop = false;

		delegate void SetTextCallback(string text);
		private void Message(string text)
		{
			if (statusBox.InvokeRequired)
			{
				var d = new SetTextCallback(Message);
				Invoke(d, new object[] { text });
			}
			else
			{
				var newText = statusBox.Text + text + Environment.NewLine;
				statusBox.Text = newText;
			}
		}

		private void ShowFOV(string text)
		{
			if (fovDisplay.InvokeRequired)
			{
				var d = new SetTextCallback(ShowFOV);
				Invoke(d, new object[] { text });
			}
			else
			{
				fovDisplay.Text = text;
			}
		}

		private void Reset()
		{
			fovInput.Text = "";
			Message("Waiting for FFXIV");
			ffxiv = null;
			ffxivHandle = IntPtr.Zero;
			ffxivFound = false;
		}

		private static IntPtr PatternScan(IntPtr handle, IntPtr start, int size, byte[] pattern)
		{
			var buf = new byte[pattern.Length];
			for (var addr = start; addr != (IntPtr)(size) - pattern.Length + 1; addr += 1)
			{
				IntPtr bytesRead;
				if (ReadProcessMemory(handle, addr, buf, pattern.Length, out bytesRead) && buf.SequenceEqual(pattern))
					return addr;
			}

			return IntPtr.Zero;
		}

		private delegate T ConverterType<T>(byte[] buf, int startIndex);
		private static bool ReadType<T>(IntPtr handle, IntPtr addr, ConverterType<T> converter, out T result)
		{
			IntPtr bytesRead;
			var size = Marshal.SizeOf(typeof(T));
			var buf = new byte[size];
			if (!ReadProcessMemory(handle, addr, buf, size, out bytesRead))
			{
				result = default(T);
				return false;
			}

			result = converter(buf, 0);
			return true;
		}

		private static bool WriteType<T>(IntPtr handle, IntPtr addr, T value)
		{
			IntPtr bytesRead;

			// Convert to byte buffer
			var size = Marshal.SizeOf(typeof(T));
			var buf = new byte[size];
			var gcHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
			Marshal.Copy(gcHandle.AddrOfPinnedObject(), buf, 0, size);

			return WriteProcessMemory(handle, addr, buf, size, out bytesRead);
		}

		// Convert FOV from the set display format to vertical+radians
		private float ConvertFOVFromDisplay(float value)
		{
			if (useHorFov.Checked)
				return (float)(Math.Atan(Math.Tan(value * Math.PI / 180.0 / 2.0) * 3.0 / 4.0) * 2.0);
			else
				return (float)(value * Math.PI / 180.0);
		}

		// Convert FOV from vertical+radians to the set display format
		private float ConvertFOVToDisplay(float value)
		{
			if (useHorFov.Checked)
				return (float)(Math.Atan(Math.Tan(value / 2.0) * 4.0 / 3.0) * 2.0 * 180.0 / Math.PI);
			else
				return (float)(value * 180.0 / Math.PI);
		}

		private void UpdateFOV()
		{
			Int64 camera;
			if (!ReadType<Int64>(ffxivHandle, cameraOffset, BitConverter.ToInt64, out camera))
			{
				Message("Invalid camera pointer address");
				exitLoop = true;
				return;
			}

			if (camera == 0)
				return;

			float maxFov;
			ReadType<float>(ffxivHandle, (IntPtr)(camera + maxFovOffset), BitConverter.ToSingle, out maxFov);

			ShowFOV(ConvertFOVToDisplay(maxFov).ToString());
		}

		private void setFov_Click(object sender, EventArgs e)
		{
			if (exitLoop || !ffxivFound)
				return;

			float newMaxFov;
			if (!float.TryParse(fovInput.Text, out newMaxFov))
				return;

			newMaxFov = ConvertFOVFromDisplay(newMaxFov);

			// Sanity check
			if (newMaxFov <= 0F || newMaxFov >= Math.PI)
				return;

			Int64 camera;
			if (!ReadType<Int64>(ffxivHandle, cameraOffset, BitConverter.ToInt64, out camera))
			{
				Message("Invalid camera pointer address");
				exitLoop = true;
				return;
			}

			if (camera == 0)
				return;

			float fov, minFov, maxFov;
			ReadType<float>(ffxivHandle, (IntPtr)(camera + fovOffset), BitConverter.ToSingle, out fov);
			ReadType<float>(ffxivHandle, (IntPtr)(camera + minFovOffset), BitConverter.ToSingle, out minFov);
			ReadType<float>(ffxivHandle, (IntPtr)(camera + maxFovOffset), BitConverter.ToSingle, out maxFov);

			var fovTan = Math.Tan(fov / 2.0);
			var minFovTan = Math.Tan(minFov / 2.0);
			var maxFovTan = Math.Tan(maxFov / 2.0);
			var newMaxFovTan = Math.Tan(newMaxFov / 2.0);
			var newMinFovTan = newMaxFovTan * minFovTan / maxFovTan;

			// Re-lerp for new max and min FOV
			var zoomPct = (fovTan - minFovTan) / (maxFovTan - minFovTan);
			var newFovTan = newMinFovTan + (newMaxFovTan - newMinFovTan) * zoomPct;

			var newMinFov = (float)(Math.Atan(newMinFovTan) * 2.0);
			var newFov = (float)(Math.Atan(newFovTan) * 2.0);

			WriteType<float>(ffxivHandle, (IntPtr)(camera + fovOffset), newFov);
			WriteType<float>(ffxivHandle, (IntPtr)(camera + minFovOffset), newMinFov);
			WriteType<float>(ffxivHandle, (IntPtr)(camera + maxFovOffset), newMaxFov);
		}

		private void ScanForFFXIV()
		{
			if (!dx9WarningShown && Process.GetProcessesByName("ffxiv").Length > 0)
			{
				Message("DX9 client is not supported");
				dx9WarningShown = true;
			}

			var processes = Process.GetProcessesByName("ffxiv_dx11");
			if (processes.Length == 0)
				return;

			ffxiv = processes[0];
			Message("Found FFXIV (PID: " + processes[0].Id + ")");

			const ProcessAccessFlags flags =
				ProcessAccessFlags.VirtualMemoryOperation |
				ProcessAccessFlags.VirtualMemoryRead |
				ProcessAccessFlags.VirtualMemoryWrite;

			ffxivHandle = OpenProcess(flags, false, ffxiv.Id);
			if(ffxivHandle == IntPtr.Zero)
			{
				Message("Failed to open handle");
				exitLoop = true;
				return;
			}

			var cameraRef = PatternScan(ffxivHandle, ffxiv.MainModule.BaseAddress, ffxiv.MainModule.ModuleMemorySize, cameraPattern);
			if (cameraRef == IntPtr.Zero)
			{
				Message("Pattern scan failed");
				exitLoop = true;
				return;
			}

			Int32 leaOffset;
			if (!ReadType<Int32>(ffxivHandle, cameraRef - 4, BitConverter.ToInt32, out leaOffset))
			{
				Message("Invalid camera pointer reference address");
				exitLoop = true;
				return;
			}

			cameraOffset = cameraRef + leaOffset;

			ffxivFound = true;
		}

		public void MainLoop()
		{
			while (!exitLoop)
			{
				if (ffxivFound && ffxiv.HasExited)
					Reset();

				if (!ffxivFound)
					ScanForFFXIV();

				if (ffxivFound)
					UpdateFOV();

				Thread.Sleep(1000);
			}
		}

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Reset();
			mainThread = new Thread(MainLoop);
			mainThread.Start();
		}

		private void Form1_FormClosed(object sender, EventArgs e)
		{
			exitLoop = true;
		}

		private void useHorFov_CheckedChanged(object sender, EventArgs e)
		{
			if (ffxivFound)
				UpdateFOV();
		}

		private void useVertFov_CheckedChanged(object sender, EventArgs e)
		{
			if (ffxivFound)
				UpdateFOV();
		}
	}
}

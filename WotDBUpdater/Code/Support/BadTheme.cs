﻿///
/// GenTheme
/// Original vb.Net Creator, AeonHack
/// Converted to C# by Faded
/// www.EmuDevs.com
/// 

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using WotDBUpdater.Code.Support;

abstract class BadThemeContainerControl : ContainerControl
{
	protected Bitmap bitmapObject;
	protected Graphics graphicObject;
	public BadThemeContainerControl()
	{
		SetStyle((ControlStyles)8198, true);
		bitmapObject = new Bitmap(1, 1);
		graphicObject = Graphics.FromImage(bitmapObject);
	}

	private bool ParentIsForm;
	protected override void OnHandleCreated(EventArgs e)
	{
		Dock = DockStyle.Fill;
		ParentIsForm = Parent is Form;
		if (ParentIsForm)
			ParentForm.FormBorderStyle = FormBorderStyle.None;
		base.OnHandleCreated(e);
	}

	private bool _Resizable = true;
	public bool Resizable
	{
		get { return _Resizable; }
		set { _Resizable = value; }
	}

	private int _TitleHeight = 26; 
	public int TitleHeight
	{
		get { return _TitleHeight; }
		set
		{
			_TitleHeight = value;
			Header = new Rectangle(7 + FormMargin, 7, TitleWidht, _TitleHeight - FormMargin - 7);
		}
	}

	private int _TitleWidht;
	public int TitleWidht
	{
		get 
		{
			_TitleWidht = Width - 7 - (FormMargin * 2) - 1;
			if (SystemExitImage != null) _TitleWidht -= SystemExitImage.Width;
			if (SystemMaximizeImage != null) _TitleWidht -= SystemMaximizeImage.Width;
			if (SystemMinimizeImage != null) _TitleWidht -= SystemMinimizeImage.Width;
			return _TitleWidht;
		}
	}

	private IntPtr Flag;
	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (!(e.Button == MouseButtons.Left))
			return;
		if (ParentIsForm)
			if (ParentForm.WindowState == FormWindowState.Maximized)
				return;

		if (Header.Contains(e.Location))
			Flag = new IntPtr(2);
		else if (Current.Position == 0 | !_Resizable)
			return;
		else
			Flag = new IntPtr(Current.Position);

		Capture = false;
		Message msg = Message.Create(Parent.Handle, 161, Flag, IntPtr.Zero);
		DefWndProc(ref msg);
		base.OnMouseDown(e);
	}

	private struct Pointer
	{
		public readonly Cursor Cursor;
		public readonly byte Position;
		public Pointer(Cursor c, byte p)
		{
			Cursor = c;
			Position = p;
		}
	}

	private Pointer GetPointer()
	{
		Point PTC = PointToClient(MousePosition);
		// Border pointers
		bool FrameLeft   = PTC.X < 7;
		bool FrameRight  = PTC.X > Width - 8;
		bool FrameTop    = PTC.Y < 7;
		bool FrameBottom = PTC.Y > Height - 8;
		// Debug.WriteLine (PTC.X.ToString() + ", " + PTC.Y.ToString());
		// System Icons ponters
		bool SysIconArea = false; 
		int SysIconWidth = 0;
		if (SystemExitImage != null)
		{
			SysIconWidth += SystemExitImage.Width;
			if (SystemMaximizeImage != null) SysIconWidth += SystemMaximizeImage.Width;
			if (SystemMinimizeImage != null) SysIconWidth += SystemMinimizeImage.Width;
			SysIconArea = PTC.Y < SystemExitImage.Height + FormMargin + 1 &
							PTC.Y > 3 + FormMargin &  // Have to point 3 pixels down on system icons, if not resize
							PTC.X > Width - SysIconWidth - (FormMargin * 2) - 1 &
							PTC.X < Width - FormMargin - 4;
			// Check if pointing at system icon position
			if (SysIconWidth > 0 & SysIconArea)
			{
				int SysIconLeft = Width - FormMargin - 1;
				if (SystemExitImage != null)
				{
					SysIconLeft -= SystemExitImage.Width;
					if (PTC.X > SysIconLeft & PTC.X < SysIconLeft + SystemExitImage.Width)
						SystemExitImageBackColor = ColorTheme.ControlBackMouseOver;
					else
						SystemExitImageBackColor = ColorTheme.FormBackTitle;
				}
				if (SystemMaximizeImage != null)
				{
					SysIconLeft -= SystemMaximizeImage.Width;
					if (PTC.X > SysIconLeft & PTC.X < SysIconLeft + SystemMaximizeImage.Width)
						SystemMaximizeImageBackColor = ColorTheme.ControlBackMouseOver;
					else
						SystemMaximizeImageBackColor = ColorTheme.FormBackTitle;
				}
				if (SystemMinimizeImage != null)
				{
					SysIconLeft -= SystemMinimizeImage.Width;
					if (PTC.X > SysIconLeft & PTC.X < SysIconLeft + SystemMinimizeImage.Width)
						SystemMinimizeImageBackColor = ColorTheme.ControlBackMouseOver;
					else
						SystemMinimizeImageBackColor = ColorTheme.FormBackTitle;
				}
				PaintSysIcons();
				return new Pointer(Cursors.Default, 0);
			}
		}
		// Check Border position for resizing
		if (_Resizable)
		{
			if (FrameLeft & FrameTop)
				return new Pointer(Cursors.SizeNWSE, 13);
			if (FrameLeft & FrameBottom)
				return new Pointer(Cursors.SizeNESW, 16);
			if (FrameRight & FrameTop)
				return new Pointer(Cursors.SizeNESW, 14);
			if (FrameRight & FrameBottom)
				return new Pointer(Cursors.SizeNWSE, 17);
			if (FrameLeft)
				return new Pointer(Cursors.SizeWE, 10);
			if (FrameRight)
				return new Pointer(Cursors.SizeWE, 11);
			if (FrameTop)
				return new Pointer(Cursors.SizeNS, 12);
			if (FrameBottom)
				return new Pointer(Cursors.SizeNS, 15);
		}
		if (SystemExitImageBackColor == ColorTheme.ControlBackMouseOver)
		{
			SystemExitImageBackColor = ColorTheme.FormBackTitle;
			PaintSysIcons();
		}
		if (SystemMaximizeImageBackColor == ColorTheme.ControlBackMouseOver)
		{
			SystemMaximizeImageBackColor = ColorTheme.FormBackTitle;
			PaintSysIcons();
		}
		if (SystemMinimizeImageBackColor == ColorTheme.ControlBackMouseOver)
		{
			SystemMinimizeImageBackColor = ColorTheme.FormBackTitle;
			PaintSysIcons();
		}
		return new Pointer(Cursors.Default, 0);
	}

	private Pointer Current;
	private Pointer Pending;
	private void SetCurrent()
	{
		Pending = GetPointer();
		//if (Current.Position == Pending.Position)
		//	return;
		Current = GetPointer();
		Cursor = Current.Cursor;
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		Point PTC = PointToClient(MousePosition);
		// System Icons ponters
		bool TitleArea = PTC.Y < TitleHeight + FormMargin + 1;
		int SysIconWidth = 0;
		if (SystemExitImage != null) SysIconWidth += SystemExitImage.Width;
		if (SystemMaximizeImage != null) SysIconWidth += SystemMaximizeImage.Width;
		if (SystemMinimizeImage != null) SysIconWidth += SystemMinimizeImage.Width;
		// Check System Icon position
		if (SysIconWidth > 0 & TitleArea & PTC.X > Width - SysIconWidth - (FormMargin * 2) - 1)
		{
			int SysIconLeft = Width - FormMargin - 1;
			if (SystemExitImage != null)
			{
				SysIconLeft -= SystemExitImage.Width;
				if (PTC.X > SysIconLeft & PTC.X < SysIconLeft + SystemExitImage.Width)
					ParentForm.Close();
			}
			if (SystemMaximizeImage != null)
			{
				SysIconLeft -= SystemMaximizeImage.Width;
				if (PTC.X > SysIconLeft & PTC.X < SysIconLeft + SystemMaximizeImage.Width)
					if (ParentForm.WindowState == FormWindowState.Normal)
						ParentForm.WindowState = FormWindowState.Maximized;
					else
						ParentForm.WindowState = FormWindowState.Normal;
			}
			if (SystemMinimizeImage != null)
			{
				SysIconLeft -= SystemMinimizeImage.Width;
				if (PTC.X > SysIconLeft & PTC.X < SysIconLeft + SystemMinimizeImage.Width)
					ParentForm.WindowState = FormWindowState.Minimized;
			}
		}
		// Set Main area values
		SetMainAreaSize();
	}

	public void SetMainAreaSize()
	{
		MainAreaClass calcMainArea = new MainAreaClass();
		calcMainArea.Top = FormMargin + TitleHeight + 1;
		calcMainArea.Left = FormMargin + 1 + FormInnerBorder;
		calcMainArea.Width = ClientRectangle.Width - (FormMargin * 2) - (FormInnerBorder * 2) - 2;
		calcMainArea.Height = ClientRectangle.Height - (FormMargin * 2) - FormInnerBorder - 2 - TitleHeight;
		if (FormFooter) calcMainArea.Height -= FormFooterHeight;
		MainArea = calcMainArea;
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (SystemExitImageBackColor == ColorTheme.ControlBackMouseOver)
		{
			SystemExitImageBackColor = ColorTheme.FormBackTitle;
			PaintSysIcons();
		}
		if (SystemMaximizeImageBackColor == ColorTheme.ControlBackMouseOver)
		{
			SystemMaximizeImageBackColor = ColorTheme.FormBackTitle;
			PaintSysIcons();
		}
		if (SystemMinimizeImageBackColor == ColorTheme.ControlBackMouseOver)
		{
			SystemMinimizeImageBackColor = ColorTheme.FormBackTitle;
			PaintSysIcons();
		}
		base.OnMouseLeave(e);
	}


	protected override void OnMouseMove(MouseEventArgs e)
	{
		SetCurrent();
		base.OnMouseMove(e);
	}

	protected Rectangle Header;
	protected override void OnSizeChanged(EventArgs e)
	{
		SetMainAreaSize(); //Header = new Rectangle(7 + FormMargin, 7, TitleWidht, _TitleHeight - FormMargin - 7);
		graphicObject.Dispose();
		bitmapObject.Dispose();
		bitmapObject = new Bitmap(Width, Height);
		graphicObject = Graphics.FromImage(bitmapObject);
		Invalidate();
		base.OnSizeChanged(e);
	}



	public void SetTransparent(Color c)
	{
		if (ParentIsForm)
			ParentForm.TransparencyKey = c;
	}

	protected override abstract void OnPaint(PaintEventArgs e);

	public void DrawCorners(Color c, Rectangle rect)
	{
		bitmapObject.SetPixel(rect.X, rect.Y, c);
		bitmapObject.SetPixel(rect.X + (rect.Width - 1), rect.Y, c);
		bitmapObject.SetPixel(rect.X, rect.Y + (rect.Height - 1), c);
		bitmapObject.SetPixel(rect.X + (rect.Width - 1), rect.Y + (rect.Height - 1), c);
	}

	public void DrawBorder(Pen outerPen, Rectangle rect, int BorderSize)
	{
		graphicObject.DrawRectangle(outerPen, rect.X + BorderSize, rect.Y + BorderSize, rect.Width - (BorderSize * 2) - 1, rect.Height - (BorderSize * 2) - 1);
	}

	public void DrawInnerBorder(Pen outerPen, Rectangle rect, int BorderSize, int InnerBorderWidth = 2)
	{
		for (int i = 1; i <=  InnerBorderWidth; i++)
		{
			graphicObject.DrawRectangle(outerPen, rect.X + BorderSize + i, rect.Y + BorderSize + i, rect.Width - (BorderSize * 2) - (i * 2) - 1, rect.Height - (i * 2) - (BorderSize * 2) - 1);	
		}
	}


	private Size TextSize;
	public void DrawText(HorizontalAlignment alignment, Brush brush) // Form text
	{
		int topPadding = FormMargin + 7;
		if (string.IsNullOrEmpty(Text))
			return;
		TextSize = graphicObject.MeasureString(Text, Font).ToSize();
		int imgPaddingAndWidth = 0;
		if (_Image != null)
		{
			imgPaddingAndWidth = Image.Width + 7;
			topPadding += 2;
		}
		switch (alignment)
		{
			case HorizontalAlignment.Left:
				graphicObject.DrawString(Text, Font, brush, 8 + imgPaddingAndWidth + FormMargin, topPadding);
				break;
			case HorizontalAlignment.Right:
				graphicObject.DrawString(Text, Font, brush, Width - 5 - TextSize.Width - Image.Width - imgPaddingAndWidth - FormMargin, topPadding);
				break;
			case HorizontalAlignment.Center:
				graphicObject.DrawString(Text, Font, brush, Width / 2 - TextSize.Width / 2, topPadding);
				break;
		}
	}

	public int ImageWidth
	{
		get
		{
			if (_Image == null)
				return 0;
			return _Image.Width;
		}
	}

	private Image _Image;
	public Image Image
	{
		get { return _Image; }
		set
		{
			_Image = value;
			Invalidate();
		}
	}


	public Color SystemExitImageBackColor = ColorTheme.FormBackTitle;
	private Image _SystemExitImage;
	public Image SystemExitImage
	{
		get { return _SystemExitImage; }
		set
		{
			_SystemExitImage = value;
			Invalidate();
		}
	}

	public Color SystemMaximizeImageBackColor = ColorTheme.FormBackTitle;
	private Image _SystemMaximizeImage;
	public Image SystemMaximizeImage
	{
		get { return _SystemMaximizeImage; }
		set
		{
			_SystemMaximizeImage = value;
			Invalidate();
		}
	}

	public Color SystemMinimizeImageBackColor = ColorTheme.FormBackTitle;
	private Image _SystemMinimizeImage;
	public Image SystemMinimizeImage
	{
		get { return _SystemMinimizeImage; }
		set
		{
			_SystemMinimizeImage = value;
			Invalidate();
		}
	}

	public void DrawIcon(HorizontalAlignment a)
	{
		int topPadding = FormMargin + 5;
		if (_Image == null)
			return;
		switch (a)
		{
			case HorizontalAlignment.Left:
				graphicObject.DrawImage(_Image, 9 + FormMargin, topPadding);
				break;
			case HorizontalAlignment.Right:
				graphicObject.DrawImage(_Image, Width - 9 - TextSize.Width, topPadding);
				break;
			case HorizontalAlignment.Center:
				graphicObject.DrawImage(_Image, Width / 2 - TextSize.Width / 2, topPadding);
				break;
		}
	}

	private Color _FormBorderColor = ColorTheme.FormBorderBlack;
	public Color FormBorderColor
	{
		get 
		{
			if (_FormBorderColor == Color.FromArgb(0, 0, 0, 0)) _FormBorderColor = ColorTheme.FormBorderBlack;
			return _FormBorderColor; 
		}
		set 
		{
			_FormBorderColor = value; 
		}
	}

	private bool _FormFooter = false;
	public bool FormFooter
	{
		get
		{
			return _FormFooter;
		}
		set
		{
			_FormFooter = value;
		}
	}

	private int _FormFooterHeight = 26;
	public int FormFooterHeight
	{
		get
		{
			return _FormFooterHeight;
		}
		set
		{
			_FormFooterHeight = value;
		}
	}

	private int _FormMargin = 0;
	public int FormMargin
	{
		get
		{
			return _FormMargin;
		}
		set
		{
			_FormMargin = value;
		}
	}

	private int _FormInnerBorder = 3;
	public int FormInnerBorder
	{
		get
		{
			return _FormInnerBorder;
		}
		set
		{
			_FormInnerBorder = value;
		}
	}


	public class MainAreaClass
	{
		public int Top;
		public int Left;
		public int Width;
		public int Height;
	}

	private MainAreaClass _MainArea;
	public MainAreaClass MainArea 
	{
		get { return _MainArea; }
		set { _MainArea = value; }
	}

	public void PaintSysIcons()
	{
		graphicObject.Dispose();
		bitmapObject.Dispose();
		bitmapObject = new Bitmap(Width, Height);
		graphicObject = Graphics.FromImage(bitmapObject);
		Invalidate();
	}

	public void AddSysIcons()
	{
		SolidBrush brush;
		// Add exit button
		int sysImgX = ClientRectangle.Width - FormMargin - 1;
		if (SystemExitImage != null)
		{
			sysImgX = sysImgX - SystemExitImage.Width;
			Rectangle rectangleSystemExit = new Rectangle(sysImgX, FormMargin + 1, SystemExitImage.Width, SystemExitImage.Height);
			brush = new SolidBrush(SystemExitImageBackColor);
			graphicObject.FillRectangle(brush, rectangleSystemExit);
			graphicObject.DrawImage(SystemExitImage, sysImgX, FormMargin + 1);

		}
		// Add max/normal button
		if (SystemMaximizeImage != null)
		{
			sysImgX = sysImgX - SystemMaximizeImage.Width;
			Rectangle rectangleSystemMaximize = new Rectangle(sysImgX, FormMargin + 1, SystemMaximizeImage.Width, SystemMaximizeImage.Height);
			brush = new SolidBrush(SystemMaximizeImageBackColor);
			graphicObject.FillRectangle(brush, rectangleSystemMaximize);
			graphicObject.DrawImage(SystemMaximizeImage, sysImgX, FormMargin + 1);
		}
		// Add min button
		if (SystemMinimizeImage != null)
		{
			sysImgX = sysImgX - SystemMinimizeImage.Width;
			Rectangle rectangleSystemMinimize = new Rectangle(sysImgX, FormMargin + 1, SystemMinimizeImage.Width, SystemMinimizeImage.Height);
			brush = new SolidBrush(SystemMinimizeImageBackColor);
			graphicObject.FillRectangle(brush, rectangleSystemMinimize);
			graphicObject.DrawImage(SystemMinimizeImage, sysImgX, FormMargin + 1);
		}
	}

}

abstract class BadThemeControl : Control
{
	protected Bitmap bitmapObject;
	protected Graphics grapichObject;
	public BadThemeControl()
	{
		SetStyle((ControlStyles)8198, true);
		bitmapObject = new Bitmap(1, 1);
		grapichObject = Graphics.FromImage(bitmapObject);
	}

	public void AllowTransparent()
	{
		SetStyle(ControlStyles.Opaque, false);
		SetStyle((ControlStyles)141314, true);
	}

	public enum State : byte
	{
		MouseNone = 0,
		MouseOver = 1,
		MouseDown = 2
	}

	protected State MouseState;
	protected override void OnMouseLeave(EventArgs e)
	{
		ChangeMouseState(State.MouseNone);
		base.OnMouseLeave(e);
	}
	protected override void OnMouseEnter(EventArgs e)
	{
		ChangeMouseState(State.MouseOver);
		base.OnMouseEnter(e);
	}
	protected override void OnMouseUp(MouseEventArgs e)
	{
		ChangeMouseState(State.MouseOver);
		base.OnMouseUp(e);
	}
	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
			ChangeMouseState(State.MouseDown);
		base.OnMouseDown(e);
	}

	private void ChangeMouseState(State e)
	{
		MouseState = e;
		Invalidate();
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		grapichObject.Dispose();
		bitmapObject.Dispose();
		bitmapObject = new Bitmap(Width, Height);
		grapichObject = Graphics.FromImage(bitmapObject);
		Invalidate();
		base.OnSizeChanged(e);
	}

	protected override abstract void OnPaint(PaintEventArgs e);

	public void DrawCorners(Color c, Rectangle rect)
	{
		bitmapObject.SetPixel(rect.X, rect.Y, c);
		bitmapObject.SetPixel(rect.X + (rect.Width - 1), rect.Y, c);
		bitmapObject.SetPixel(rect.X, rect.Y + (rect.Height - 1), c);
		bitmapObject.SetPixel(rect.X + (rect.Width - 1), rect.Y + (rect.Height - 1), c);
	}

	public void DrawBorders(Pen outerPen, Pen innerPen, Rectangle rect)
	{
		grapichObject.DrawRectangle(outerPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
		grapichObject.DrawRectangle(innerPen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
	}

	public void DrawBorders(Pen outerPen, Rectangle rect)
	{
		grapichObject.DrawRectangle(outerPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
	}


	private Size TextSize;
	public void DrawText(HorizontalAlignment horizontalAlignment, Brush brush, int offset = 0, bool verticalAlignmentMiddle = true) 
	{
		if (string.IsNullOrEmpty(Text))
			return;
		TextSize = grapichObject.MeasureString(Text, Font).ToSize();

		int yPos = 0;
		if (verticalAlignmentMiddle)
			yPos = Height / 2 - TextSize.Height / 2;

		switch (horizontalAlignment)
		{
			case HorizontalAlignment.Left:
				grapichObject.DrawString(Text, Font, brush, 5 + offset, yPos);
				break;
			case HorizontalAlignment.Right:
				grapichObject.DrawString(Text, Font, brush, Width - 5 - TextSize.Width - offset, yPos);
				break;
			case HorizontalAlignment.Center:
				grapichObject.DrawString(Text, Font, brush, Width / 2 - TextSize.Width / 2, yPos);
				break;
		}
	}

	public int ImageWidth
	{
		get
		{
			if (_Image == null)
				return 0;
			return _Image.Width;
		}
	}

	private Image _Image;
	public Image Image
	{
		get { return _Image; }
		set
		{
			_Image = value;
			Invalidate();
		}
	}

	public void DrawIcon(HorizontalAlignment a, int xOffset = 0, int yOffset = 0)
	{
		if (_Image == null)
			return;
		switch (a)
		{
			case HorizontalAlignment.Left:
				grapichObject.DrawImage(_Image, Width / 10 + xOffset, Height / 2 - _Image.Height / 2 + yOffset);
				break;
			case HorizontalAlignment.Right:
				grapichObject.DrawImage(_Image, Width - _Image.Width - 2, Height / 2 - TextSize.Height / 2 + yOffset);
				break;
			case HorizontalAlignment.Center:
				grapichObject.DrawImage(_Image, Width / 2 - TextSize.Width / 2, Height / 2 - TextSize.Height / 2 + yOffset);
				break;
		}
	}
}

class BadForm : BadThemeContainerControl
{
	public BadForm()
	{
		SetTransparent(Color.Fuchsia); // Transparency color = purple (FF00FF)
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		graphicObject.Clear(Color.Fuchsia);
		SolidBrush brush;
		// Draw Main area
		Rectangle rectangleMain = new Rectangle(FormMargin + 1, TitleHeight + FormMargin + 1, ClientRectangle.Width - (FormMargin * 2) - 2, ClientRectangle.Height - TitleHeight - (FormMargin * 2) - 1);
		brush = new SolidBrush(ColorTheme.FormBack);
		graphicObject.FillRectangle(brush, rectangleMain);
		// Add outer border
		Pen FormBorderPenColor = new Pen(FormBorderColor);
		DrawBorder(FormBorderPenColor, ClientRectangle, FormMargin); // Outer Border
		// Add inner border
		FormBorderPenColor = new Pen(ColorTheme.FormBackTitle);
		DrawInnerBorder(FormBorderPenColor, ClientRectangle, FormMargin, FormInnerBorder); // Inner Border
		// Draw title 
		Rectangle rectangleTitle = new Rectangle(FormMargin + 1, FormMargin + 1, ClientRectangle.Width - (FormMargin * 2) - 2, TitleHeight);
		brush = new SolidBrush(ColorTheme.FormBackTitle);
		graphicObject.FillRectangle(brush, rectangleTitle);
		// Draw sys icons
		AddSysIcons(); // Add Sys Icons in title bar
		// Footer
		if (FormFooter)
		{
			Rectangle rectangleFooter = new Rectangle(FormMargin + 1, ClientRectangle.Height - FormFooterHeight - FormMargin - FormInnerBorder - 1 , ClientRectangle.Width - (FormMargin * 2) - 2, FormFooterHeight);
			brush = new SolidBrush(ColorTheme.FormBackFooter);
			graphicObject.FillRectangle(brush, rectangleFooter);
		}
		// Add title icon and text
		DrawText(HorizontalAlignment.Left, new SolidBrush(ColorTheme.FormBackTitleFont)); // Add title text
		DrawIcon(HorizontalAlignment.Left); // Add title icon

		
		// Set Main area values
		SetMainAreaSize();
		
		// Draw theme on form
		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}


}

class BadButton : BadThemeControl
{

	public BadButton()
	{
		AllowTransparent();
	}

	protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
	{
		SolidBrush brushBackColor; //= new SolidBrush(); //ColorTheme.BackControl);
		if (MouseState == State.MouseDown)
			brushBackColor = new SolidBrush(ColorTheme.ControlBackMouseDown);
		else if (MouseState == State.MouseOver)
			brushBackColor = new SolidBrush(ColorTheme.ControlBackMouseOver);
		else
			brushBackColor = new SolidBrush(ColorTheme.ControlBack);
		grapichObject.FillRectangle(brushBackColor, ClientRectangle);

		DrawText(HorizontalAlignment.Center, new SolidBrush(ColorTheme.ControlFont));
		DrawIcon(HorizontalAlignment.Left);

		//Pen outerBorderPen = new Pen(Color.Black); // Button border
		//DrawBorders(outerBorderPen, ClientRectangle);
		//DrawCorners(BackColor, ClientRectangle); // Button corners

		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}
}

class BadSeperator : BadThemeControl
{

	public BadSeperator()
	{
		AllowTransparent();
		BackColor = Color.Transparent;
	}

	private Orientation _Direction;
	public Orientation Direction
	{
		get { return _Direction; }
		set
		{
			_Direction = value;
			Invalidate();
		}
	}

	private Color _SeparatorColor = ColorTheme.FormBackTitle;
	public Color SeparatorColor
	{
		get { return _SeparatorColor; }
		set
		{
			_SeparatorColor = value;
			Invalidate();
		}
	}


	protected override void OnPaint(PaintEventArgs e)
	{
		grapichObject.Clear(BackColor);

		if (_Direction == Orientation.Horizontal)
		{
			int Yoffset = 0;
			grapichObject.DrawLine(new Pen(SeparatorColor), 0, Height / 2 + Yoffset, Width, Height / 2 + Yoffset);
			//grapichObject.DrawLine(new Pen(SeparatorColor), 0, Height / 2 - 1 + Yoffset, Width, Height / 2 - 1 + Yoffset);
			//grapichObject.DrawLine(new Pen(SeparatorColor), 0, Height / 2 + 1 + Yoffset, Width, Height / 2 + 1 + Yoffset);
			if (Text != "")
			{
				int Xoffset = 10;
				SolidBrush brushBackColor;
				brushBackColor = new SolidBrush(ColorTheme.FormBack);
				Size textSize = grapichObject.MeasureString(Text, Font).ToSize();
				Rectangle rectangleTextBack = new Rectangle(4 + Xoffset, 0, textSize.Width + 2, Height);
				grapichObject.FillRectangle(brushBackColor, rectangleTextBack);
				DrawText(HorizontalAlignment.Left, new SolidBrush(ColorTheme.ControlDarkFont), Xoffset);
			}
		}
		else
		{
			grapichObject.DrawLine(new Pen(SeparatorColor), Width / 2, 0, Width / 2, Height);
			//grapichObject.DrawLine(new Pen(SeparatorColor), Width / 2 -1, 0, Width / 2 -1, Height);
			//grapichObject.DrawLine(new Pen(SeparatorColor), Width / 2 +1, 0, Width / 2 +1, Height);
		}

		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}
}


class BadGroupBox : BadThemeControl
{

	public BadGroupBox()
	{
		AllowTransparent();
		BackColor = Color.Transparent;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		grapichObject.Clear(BackColor);
		int Yoffset = 7;
		// Outer Border
		SolidBrush BorderColor = new SolidBrush(ColorTheme.FormBackTitle);
		Rectangle GroupBoxOuter = new Rectangle(0, Yoffset, ClientRectangle.Width, ClientRectangle.Height - Yoffset);
		grapichObject.FillRectangle(BorderColor, GroupBoxOuter);
		// Inner Area
		BorderColor = new SolidBrush(ColorTheme.FormBack);
		Rectangle GroupBoxInner = new Rectangle(1, Yoffset+1, ClientRectangle.Width-2, ClientRectangle.Height - Yoffset - 2);
		grapichObject.FillRectangle(BorderColor, GroupBoxInner);
		
		if (Text != "")
		{
			int Xoffset = 10;
			SolidBrush brushBackColor;
			brushBackColor = new SolidBrush(ColorTheme.FormBack);
			Size textSize = grapichObject.MeasureString(Text, Font).ToSize();
			Rectangle rectangleTextBack = new Rectangle(4 + Xoffset, 0, textSize.Width + 2, textSize.Height);
			grapichObject.FillRectangle(brushBackColor, rectangleTextBack);
			DrawText(HorizontalAlignment.Left, new SolidBrush(ColorTheme.ControlDarkFont), Xoffset, false);
		}
		
		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}
}

class BadLabel : BadThemeControl
{
	Label label = new Label();
	public BadLabel()
	{
		AllowTransparent();
		BackColor = ColorTheme.FormBack;
		label.BackColor = ColorTheme.FormBack;
		label.ForeColor = ColorTheme.ControlFont;
		label.Top = 5;
		this.Controls.Add(label);
	}

	protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
	{
		label.Text = Text;
		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}

	protected override void OnResize(EventArgs e)
	{
		label.Width = ClientRectangle.Width ;
		label.Height = ClientRectangle.Height ;
	}
}

class BadTextBox : BadThemeControl
{

	TextBox textBox = new TextBox();
	public BadTextBox()
	{
		AllowTransparent();
		textBox.BackColor = ColorTheme.FormBack;
		textBox.ForeColor = ColorTheme.ControlFont;
		textBox.BorderStyle = BorderStyle.None;
		textBox.Multiline = true;
		textBox.Top = 5;
		textBox.Left = 6;
		this.Controls.Add(textBox);
		textBox.TextChanged += new EventHandler(textBox_TextChanged);
	}

	protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
	{
		// Outer Border
		SolidBrush BorderColor = new SolidBrush(ColorTheme.ControlBorder);
		Rectangle GroupBoxOuter = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
		grapichObject.FillRectangle(BorderColor, GroupBoxOuter);
		// Inner Area
		BorderColor = new SolidBrush(ColorTheme.FormBack);
		Rectangle GroupBoxInner = new Rectangle(1, 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2);
		grapichObject.FillRectangle(BorderColor, GroupBoxInner);
		textBox.Text = Text;
		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}

	protected override void OnResize(EventArgs e)
	{
		textBox.Width = ClientRectangle.Width -10;
		textBox.Height = ClientRectangle.Height -8;
	}

	private void textBox_TextChanged(object sender, EventArgs e)
	{
		Text = textBox.Text;
	}

	protected override void OnTextChanged(EventArgs e)
	{
		textBox.Text = Text;
	}

}


class BadComboBox : BadThemeControl
{

	public BadComboBox()
	{
		AllowTransparent();
	}

	protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
	{
		SolidBrush brushBackColor; //= new SolidBrush(); //ColorTheme.BackControl);
		if (MouseState == State.MouseDown)
			brushBackColor = new SolidBrush(ColorTheme.ControlBackMouseDown);
		else if (MouseState == State.MouseOver)
			brushBackColor = new SolidBrush(ColorTheme.ControlBackMouseOver);
		else
			brushBackColor = new SolidBrush(ColorTheme.ControlBack);
		grapichObject.FillRectangle(brushBackColor, ClientRectangle);
		// Text
		DrawText(HorizontalAlignment.Left, new SolidBrush(ColorTheme.ControlFont));
		// Overwrite text to right to avoid hiding dropdown icon
		if (Image != null)
		{
			if (MouseState == State.MouseDown)
				brushBackColor = new SolidBrush(ColorTheme.ControlBackMouseDown);
			else if (MouseState == State.MouseOver)
				brushBackColor = new SolidBrush(ColorTheme.ControlBackMouseOver);
			else
				brushBackColor = new SolidBrush(ColorTheme.ControlBack);
			grapichObject.FillRectangle(brushBackColor, Width - Image.Width -4, 0, Image.Width + 4, Height);
			// Dropdown icon
			DrawIcon(HorizontalAlignment.Right,0,-2);
		}
		e.Graphics.DrawImage(bitmapObject, 0, 0);
	}

}

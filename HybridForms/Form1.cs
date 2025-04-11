using HybridForms.Pages;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection;

namespace HybridForms;

public partial class Form1 : Form {
	private readonly PictureBox title = new PictureBox();    // create a PictureBox
	private readonly Label minimise = new Label();           // this doesn't even have to be a label!
	private readonly Label maximise = new Label();           // this will simulate our this.maximise box
	private readonly Label close = new Label();              // simulates the this.close box

	private bool drag = false;                      // determine if we should be moving the form
	private Point startPoint = new Point(0, 0);     // also for the moving

	
	protected override CreateParams CreateParams {
		get {
			CreateParams parameters = base.CreateParams;
			parameters.Style = parameters.Style | 0x00040000;
			return parameters;
		}
	}

	protected override void WndProc(ref Message m) {
		base.WndProc(ref m);
		if (m.Msg == 0x84) // WM_NCHITTEST
		{
			if ((int)m.Result == 0x2) // HTCLIENT
				m.Result = (IntPtr)0x1; // HTCAPTION
		}
	}

	public Form1() {
		InitializeComponent();
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		this.ControlBox = false;
		this.Text = String.Empty;


		var services = new ServiceCollection();
		services.AddWindowsFormsBlazorWebView();
		services.AddBlazorWebViewDeveloperTools();
		blazorWebView1.HostPage = "wwwroot\\index.html";
		blazorWebView1.Services = services.BuildServiceProvider();
		blazorWebView1.RootComponents.Add<Counter>("#app");
		
		//

		

		// you may want to use PictureBoxes and display images
		// or use buttons, there are many alternatives. This is a mere example.
		this.minimise.Text = "Minimise";        // Doesn't have to be
		this.minimise.Location = new Point(this.Location.X + 5 + 200, this.Location.Y + 5); // give it a default location
		this.minimise.ForeColor = Color.Red;    // Give it a colour that will make it stand out
												// this is why I didn't use an image, just to keep things simple:
		this.minimise.BackColor = Color.Black;  // make it the same as the PictureBox
		this.Controls.Add(this.minimise);       // add it to the form's controls
		this.minimise.BringToFront();           // bring it to the front, to display it above the picture box

		this.maximise.Text = "Maximise";
		// remember to make sure it's far enough away so as not to overlap our minimise option
		this.maximise.Location = new Point(this.Location.X + 60 + 200, this.Location.Y + 5);
		this.maximise.ForeColor = Color.Red;
		this.maximise.BackColor = Color.Black;  // remember, we want it to match the background
		this.maximise.Width = 50;
		this.Controls.Add(this.maximise);       // add it to the form
		this.maximise.BringToFront();

		this.close.Text = "Close";
		this.close.Location = new Point(this.Location.X + 120 + 200, this.Location.Y + 5);
		this.close.ForeColor = Color.Red;
		this.close.BackColor = Color.Black;
		this.close.Width = 37;                  // this is just to make it fit nicely
		this.Controls.Add(this.close);
		this.close.BringToFront();

		// now we need to add some functionality. First off, let's give those labels
		// MouseHover and MouseLeave events, so they change colour
		// Since they're all going to change to the same colour, we can give them the same
		// event handler, which saves time of writing out all those extra functions
		this.minimise.MouseEnter += new EventHandler(Control_MouseEnter);
		this.maximise.MouseEnter += new EventHandler(Control_MouseEnter);
		this.close.MouseEnter += new EventHandler(Control_MouseEnter);

		// and we need to do the same for MouseLeave events, to change it back
		this.minimise.MouseLeave += new EventHandler(Control_MouseLeave);
		this.maximise.MouseLeave += new EventHandler(Control_MouseLeave);
		this.close.MouseLeave += new EventHandler(Control_MouseLeave);

		// and lastly, for these controls, we need to add some functionality
		this.minimise.MouseClick += new MouseEventHandler(Control_MouseClick);
		this.maximise.MouseClick += new MouseEventHandler(Control_MouseClick);
		this.close.MouseClick += new MouseEventHandler(Control_MouseClick);

		// finally, wouldn't it be nice to get some moveability on this control?
		this.title.MouseDown += new MouseEventHandler(Title_MouseDown);
		this.title.MouseUp += new MouseEventHandler(Title_MouseUp);
		this.title.MouseMove += new MouseEventHandler(Title_MouseMove);
	}


	private void Control_MouseEnter(object sender, EventArgs e) {
		if (sender.Equals(this.close))
			this.close.ForeColor = Color.White;
		else if (sender.Equals(this.maximise))
			this.maximise.ForeColor = Color.White;
		else // it's the minimize label
			this.minimise.ForeColor = Color.White;
	}

	private void Control_MouseLeave(object sender, EventArgs e) {
		// return them to their default colors
		if (sender.Equals(this.close))
			this.close.ForeColor = Color.Red;
		else if (sender.Equals(this.maximise))
			this.maximise.ForeColor = Color.Red;
		else // it's the minimise label
			this.minimise.ForeColor = Color.Red;
	}

	private void Control_MouseClick(object sender, MouseEventArgs e) {
		if (sender.Equals(this.close))
			this.Close(); // close the form
		else if (sender.Equals(this.maximise)) {
			// maximise is more interesting. We need to give it different functionality,
			// depending on the window state (Maximise/Restore)
			if (this.maximise.Text == "Maximise") {
				this.WindowState = FormWindowState.Maximized;   // maximise the form
				this.maximise.Text = "Restore";                 // change the text
				this.title.Width = this.Width;                  // stretch the title bar
			} else // we need to restore
			  {
				this.WindowState = FormWindowState.Normal;
				this.maximise.Text = "Maximise";
			}
		} else // it's the minimise label
			this.WindowState = FormWindowState.Minimized;       // minimise the form
	}

	void Title_MouseUp(object sender, MouseEventArgs e) {
		this.drag = false;
	}

	void Title_MouseDown(object sender, MouseEventArgs e) {
		this.startPoint = e.Location;
		this.drag = true;
	}

	void Title_MouseMove(object sender, MouseEventArgs e) {
		if (this.drag) {
			// if we should be dragging it, we need to figure out some movement
			Point p1 = new Point(e.X, e.Y);
			Point p2 = this.PointToScreen(p1);
			Point p3 = new Point(p2.X - this.startPoint.X,
								 p2.Y - this.startPoint.Y);
			this.Location = p3;
		}
	}


}

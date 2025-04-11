using HybridForms.Pages;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection;

namespace HybridForms;

public partial class Form1 : Form {
	public Form1() {
		InitializeComponent();

		var services = new ServiceCollection();

		services.AddWindowsFormsBlazorWebView();
		blazorWebView1.HostPage = "wwwroot\\index.html";
		blazorWebView1.Services = services.BuildServiceProvider();
		blazorWebView1.RootComponents.Add<Counter>("#app");

	}
}

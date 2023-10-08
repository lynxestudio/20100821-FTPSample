using System;
using Gtk;
using System.Net;
using System.IO;

public partial class MainWindow : Gtk.Window
{
	MonoFTP. FtpDAO _ftpdao;
	string NombreArchivo{ set; get;}
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		tvls.Selection.Mode = SelectionMode.Single;
		tvls.Selection.Changed += new EventHandler(SelectionChanged);
		tvls.AppendColumn(new TreeViewColumn("Listado del servidor",new CellRendererText(),"text",0));
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	protected virtual void OnBtnOkClicked (object sender, System.EventArgs e)
	{
		Listar();
	}
	
	protected virtual void OnBtnCancelClicked (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
	
	void MessageBox(string msg,MessageType msgtype){
		 using(Dialog messageBox = new MessageDialog(this,
			         DialogFlags.DestroyWithParent,
			         msgtype,
			         ButtonsType.Ok,
			         msg)){
				messageBox.Run();
				messageBox.Destroy();
			}
	}
		
	protected virtual void OnChkanonToggled (object sender, System.EventArgs e)
	{
			txtUser.IsEditable = txtPass.IsEditable = !chkanon.Active;
			txtUser.Text = (chkanon.Active == true ? "anonymous" : "") ;
			txtPass.Text = (chkanon.Active == true ? "xomalli@gmail.com" : "") ;
	}
	
	protected virtual void OnBtnDownClicked (object sender, System.EventArgs e)
	{
		
		if(NombreArchivo == null)
			MessageBox("Escoga un archivo",MessageType.Info);
		else{
			try{
		Uri serverUri = new Uri(string.Format("ftp://{0}/{1}/{2}",txthost.Text,txtDir.Text,NombreArchivo));
		_ftpdao = new MonoFTP.FtpDAO(txtUser.Text ,txtPass.Text);
		_ftpdao.DescargarArchivo(serverUri, Directory.GetCurrentDirectory() + "/" + NombreArchivo,rbbin.Active);
		MessageBox("Archivo creado",MessageType.Info);
		}catch(WebException webex){
			MessageBox("Red:" + webex.Message,MessageType.Error);
		}
		catch(IOException ioex){
			MessageBox("Entrada/Salida:" + ioex.Message,MessageType.Error);
		}
		catch(Exception ex){
			MessageBox("Excepcion:" + ex.Message,MessageType.Error);
		}
		}
	}
	
    
	
	void SelectionChanged (object o, EventArgs args)
	{
			TreeSelection selection = (TreeSelection)o;
			TreeIter iter;
			TreeModel model;
			if(selection.GetSelected(out model,out iter))
		      NombreArchivo = Convert.ToString(model.GetValue(iter,0));
	}
	protected virtual void OnChkLsToggled (object sender, System.EventArgs e)
	{
			btnDown.Visible = !chkLs.Active;
		 Listar();
	}	
	
	void Listar(){
		try{
		Uri serverUri = new Uri(string.Format("ftp://{0}/{1}/",txthost.Text,txtDir.Text));
		_ftpdao = new MonoFTP.FtpDAO(txtUser.Text ,txtPass.Text);
		tvls.Model =  _ftpdao.ObtenerListado(serverUri,chkLs.Active);
		}
		catch(WebException webex){
			MessageBox("Red:" + webex.Message,MessageType.Error);
		}
		catch(IOException ioex){
			MessageBox("Entrada/Salida:" + ioex.Message,MessageType.Error);
		}
		catch(Exception ex){
			MessageBox("Excepcion:" + ex.Message,MessageType.Error);
		}
	}
}

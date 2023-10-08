using System;
using System.Net;
using System.IO;
using Gtk;

namespace MonoFTP
{
	public class FtpDAO
	{
		FtpWebRequest _ftprequest;
		NetworkCredential creden;
		public FtpDAO (string us,string pass)
		{
			creden = new NetworkCredential(us,pass);
		}
public ListStore ObtenerListado(Uri serverUri,bool lsl){
_ftprequest = (FtpWebRequest)WebRequest.Create(serverUri);
_ftprequest.Credentials = creden;
_ftprequest.KeepAlive = true;
ListStore ls_l = new ListStore(typeof(string));
if(lsl)
_ftprequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
else
_ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;
_ftprequest.UseBinary = true;
_ftprequest.UsePassive = true;
_ftprequest.Timeout = 20000;
using(FtpWebResponse response = (FtpWebResponse)_ftprequest.GetResponse()){
using(StreamReader reader = new StreamReader(response.GetResponseStream())){
string ln = "";
do
{
ln = reader.ReadLine();
if(ln != null){
ls_l.AppendValues(ln);	
}
}while(ln != null);
}
}
return ls_l;
}
	
public void DescargarArchivo(Uri serverUri,string destino,bool modobin)
{
_ftprequest = (FtpWebRequest)WebRequest.Create(serverUri);
_ftprequest.Credentials = creden;
_ftprequest.KeepAlive = true;
_ftprequest.Method = WebRequestMethods.Ftp.DownloadFile;
_ftprequest.UseBinary = modobin;
_ftprequest.UsePassive = true;
string contenido = "";
char[] charbuffer = null;
byte[] bytebuffer = new byte[2048];
int tbytes = 0;
using(FtpWebResponse response = (FtpWebResponse)_ftprequest.GetResponse())
{
using(Stream stream = response.GetResponseStream())
{
using (FileStream fileout = new FileStream(destino, FileMode.Create, FileAccess.ReadWrite))
{
if (modobin)
{
do
{
tbytes = stream.Read(bytebuffer, 0, 2048);
fileout.Write(bytebuffer,0,bytebuffer.Length);
} while (tbytes != 0);

}
else
{
using(StreamReader reader = new StreamReader(stream)){
contenido = reader.ReadToEnd();
charbuffer = contenido.ToCharArray();
}
using(StreamWriter text = new StreamWriter(fileout)){
text.Write(charbuffer,0,charbuffer.Length);
}}}}} 
}


	}
}
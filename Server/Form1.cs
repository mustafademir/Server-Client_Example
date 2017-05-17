using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets; //Programda soketler kullanıldı
using System.Threading;// Programda aynı anda birden fazla iş yapılması gerektiği için Thread kavramını kullanmam gerekti
using System.IO;  //Streamlerin okunması ve yazılması için gerekli 
using System.Net;
using System.Data.SqlTypes;
namespace Server
{
    public partial class Form1 : Form
    {
        // Programda kullanılacak değişkenlerin tanımlanması
        Thread t;
        Thread t_s;
        IPAddress ip;
        IPAddress ip_s;
        TcpListener dinle;
        TcpListener dinle_s;
        Socket soket;
        Socket soket_s;
        NetworkStream ag_s;
        NetworkStream ag;
        StreamWriter yaz_s;
        StreamWriter yaz;
        StreamWriter dene;
        StreamReader oku;
        int sayac = 0;
        public delegate void ricdegis(string text);

        public Form1()
        {
            InitializeComponent();
        }
        // Akış içerisindeki verilerin client tarafından elde edilmesini sağlayan metod ;
        public void okumayabasla()
        {
            soket = dinle.AcceptSocket();
            ag = new NetworkStream(soket);
            oku = new StreamReader(ag);
            while (true)
            {
                try
                {
                    string yazi = oku.ReadLine();
                    ekranabas(yazi);
                }
                catch
                {
                    return;
                }
            }
        }
        //okumayabasla() metodu ile elde edilen verileri işlenebilecek ve ekranda gösterebilecek duruma getirin metod ;
        public void ekranabas(string s)
        {
            if (this.InvokeRequired)
            {
                ricdegis degis = new ricdegis(ekranabas);   //İşlenecek verinin olup olmadığı hata alınmaması için kotrol ediliyor
                this.Invoke(degis, s);
            }
            else
            // Eğer işlenecek veri varsa bu program için oluşturduğum basit bir imzalama algoritması çalışır ;
            // Algoritma öncelikle gelen verinin 0 indisli (ilk elemanı) elemanına bakar;    
            {
                String a = s.Substring(0, 1); //İlk eleman çekilerek a değişkenine atandı
                if (a == "a")
                {
                    veriyolla_alan1();     // Eğer ilk eleman "a" ise Client'in Alan-1 GETİR butonu çalışmıştır isteği kabul et ve veriyi yolla
                }
                else if (a == "b")   
                {
                    veriyolla_alan2();    // Eğer ilk eleman "b" ise Client'in Alan-2 GETİR butonu çalışmıştır isteği kabul et ve veriyi yolla
                }
                else if (a == "e")
                {
                    richTextBox3.AppendText(s.Substring(1));  // Eğer ilk eleman "e" ise veri Client'in Alan-3 bölgesinden gelmiştir, gelen veriyi Alan-3 bölgesine yazdır.
                }
                else if (a == "d")
                {
                    richTextBox4.AppendText(s.Substring(1));  // Eğer ilk eleman "d" ise veri Client'in Alan-4 bölgesinden gelmiştir, gelen veriyi Alan-4 bölgesine yazdır.
                }
            }

        }
        //Server'ın 1234 portunu dinlemeye başlaması için yazdığım metod, başlatıığım thread okumayabasla metodunu çağırıyor
        private void dinlemeye_basla()
        {
            try
            {
                ip = IPAddress.Parse("127.0.0.1");
                dinle = new TcpListener(ip, 1234);
                dinle.Start();
                t = new Thread(new ThreadStart(okumayabasla));
                t.Start();
                label1.Text = "Client ile bağlantı sağlandı ..."+ Environment.NewLine + "1234 numaralı port dinleniyor ...";
            }
            catch (Exception)
            {

                label1.Text="Dinleme girişimi başarısız";
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        //Server'dan Client'e veri yollayan metodlar ; 
        public void veriyolla_alan1()
        {
            yaz = new StreamWriter(ag);

            yaz.WriteLine("1" + richTextBox1.Text); // Alan-1 de ki verileri imzalama algoritmasına göre başına "1" değerini ekleyerek yolluyor

            yaz.Flush();

        }
        public void veriyolla_alan2()
        {
            dene = new StreamWriter(ag);
            dene.WriteLine("2" + richTextBox2.Text); // Alan-2 de ki verileri imzalama algoritmasına göre başına "2" değerini ekleyerek yolluyor
            dene.Flush();
        }
        public void veriyolla_sayac()
        {
            soket_s = dinle_s.AcceptSocket();
            ag_s = new NetworkStream(soket_s);
            yaz_s = new StreamWriter(ag_s);
            while (true)
            {
                try
                {
                    yaz_s.WriteLine("3" + (Convert.ToInt32(label2.Text) + 10).ToString());//Sayaç değerine 10 ekleyip stringe çevirdikten sonra imzalama algoritmasına göre başına "3" ekliyor.
                    yaz_s.Flush();
                }
                catch
                {
                    return;
                }
            }
        //---------------------------------------------------------------------------------------------------------------------------
        }
        private void button1_Click(object sender, EventArgs e) // DİNLEMEYİ BAŞLAT BUTONU
        {
            dinlemeye_basla();
            button1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e) // Timer ekleyerek sayacın çalışması sağlandı
        {
            sayac++;
            label2.Text = sayac.ToString();
        }

        private void button2_Click(object sender, EventArgs e) // SAYACI BAŞLAT BUTONU
        {
            timer1.Interval = 1000; // Timer saniyede 1 artacak şekilde ayarlandı
            timer1.Start();
            //////////////
            try
            {
                ip_s = IPAddress.Parse("127.0.0.1");
                dinle_s = new TcpListener(ip_s, 3456); // Sayaç 3456 portunu kullanmaktadır.
                dinle_s.Start();
                t_s = new Thread(new ThreadStart(veriyolla_sayac));
                t_s.Start();

            }
            catch (Exception)
            {

                label2.Text = "Sayaç başlatılamadı ...";
            }

        }
    }
}

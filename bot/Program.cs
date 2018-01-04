using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotSharp;
using TelegramBotSharp.Types;


namespace bot
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(DateTime.Now.DayOfWeek);
            Thrit trit = new Thrit();
            Console.WriteLine("Before Push Start Thread Bot");
            Thread tritalarm = new Thread(new ThreadStart(trit.thrit_alarm));
            Thread tritbot = new Thread(new ThreadStart(trit.tritBOT));


            
            tritbot.Start();
            tritalarm.Start();
            Console.ReadLine();

        }
    }


    class Thrit
    {
        private static Boolean stopThread = false;
        private static readonly object lockObject = new object();
        private Int32 durasi_pull = 1000 * 1 * 1;
        private Int32 durasi_push = 1000 * 60 * 1;

        string datelog = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
      
        public static TelegramBot bot;

        public const string FORMAT_PESAN = "TUJUAN#LAPORAN#JUDUL#DESC";
        public void thrit_alarm()
        {
            DBConnect db = new DBConnect();

            while (stopThread != true)
            {
                string datelog2 = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
                string CekJam = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string jam = DateTime.Now.ToString("HH:mm");
       
                string hari= DateTime.Now.DayOfWeek.ToString();

                if (hari == "Saturday")
                {

                }
                else if (hari == "Sunday")
                {

                }
                else
                { 

                #region alarm
                switch (jam)
                {
                    case "14:00":
                        //db.JustInfo();
                        break;

                    case "15:00":
                        db.Alert();
                        break;

                    case "15:30":
                        db.Alert();
                        break;

                    case "16:00":
                        db.Alert();
                        break;

                    case "16:30":
                        db.Alert();
                        break;

                    case "17:00":
                        db.Alert();
                        break;

                    case "17:30":
                        db.Alert();
                        break;

                    case "18:00":
                        db.Alert();
                        break;

                    case "18:30":
                        db.Alert();
                        break;
                    case "19:00":
                        db.Alert();
                        break;

                    case "19:30":
                        db.Alert();
                        break;
                    case "20:00":
                        db.Alert();
                        break;
                    case "20:30":
                        db.Alert();
                        break;

                    case "21:00":
                        db.Alert();
                        break;

                    default:
                       // db.Alert();
                        break;
                }
                #endregion

                }

                //Console.WriteLine(DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss")"{0}", " TRIT PUSH SATU");
                Console.WriteLine("Alarm Thrit  {0} ,hari -> {1}", datelog2, hari);

                Thread.Sleep(durasi_push);

            }
        }


        public void tritBOT()
        {
            while (stopThread != true)
            {
               
                    string datelog2 = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");

                    DateTime time = DateTime.Now;
                    string format = time.ToString("d");

                    

                    Pesan pesen = new Pesan();
                    SendMail email = new SendMail();

                    Console.WriteLine("Initializing Bot...");
                    bot = new TelegramBot(System.IO.File.ReadAllText("apikey.txt"));

                    Console.WriteLine("Bot initialized.");
                    Console.WriteLine("Hi, i'm {0}! ID: {1}", bot.Me.FirstName, bot.Me.Id);

                    Console.WriteLine(" This Threat BOT  {0} ", datelog2);
                    new Task(PollMessages).Start();

                    
                    Console.ReadLine();
                    Thread.Sleep(durasi_pull);
                
            }
        }



        public async void PollMessages()
        {
            string datelog = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");

            Pesan pesen = new Pesan();
            DBConnect db = new DBConnect();

            string CekJam = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            
            while (true)
            {
                var result = await bot.GetMessages();

                string datelog2 = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
                string jam = DateTime.Now.ToString("HH:mm");



                foreach (Message m in result)
                {
                    if (m.Chat != null)
                    {
                        Console.WriteLine("[{0}] {1}: {2}", m.Chat.Title, m.From.Username, m.Text);
                    }
                    else
                    {
                        Console.WriteLine("{0}: {1}", m.From.Username, m.Text);
                    }

                    HandleMessage(m);
                    //pesen.HandleMessage(m);

                }

                Console.WriteLine("Bot Running -> {0}", datelog2);

            }



        }


        public void HandleMessage(Message pesan)
        {
            DBConnect db = new DBConnect();

            MessageTarget target = ((MessageTarget)pesan.Chat ?? pesan.From);
            Pesan pesanClass = new Pesan();

            if (pesan.Text == null)
            {
                return;
            }
            else
            {
                Int32 targetID = target.Id; //klo minus berarti group 
                Int32 idFrom = pesan.From.Id; //id pengirim chat user 
                string First = pesan.From.FirstName; //first name pengirim chat
                string last = pesan.From.LastName; // last name pengirim chat
                string usernmae = pesan.From.Username; //username pengirim chat
                Int32 chatid = pesan.MessageId; //id pesan
                string botname = bot.Me.FirstName; //nama bot 

                string tipe = null;
                string namagroup = target.ToString();

                string[] pecahGroup = namagroup.Split('.');

                tipe = pecahGroup[2].ToString();

                if (pesan.Text == "#")
                {
                    return;
                }

                string txt = pesanClass.filterPesan(pesan.Text, idFrom, First, last, usernmae, chatid);

                db.InsertChat(chatid.ToString(), idFrom.ToString(), pesan.Text.ToString(), botname.ToString(), tipe.ToString(), null, chatid.ToString());

                //messageid,userid,text,botname,type,replymessage_id,updateid


                if (txt == null)
                {
                    string messageToSend = "Maaf Format Pesan " + pesan.From.Username + " Salah Silahkan Ulangi kembali dengan Format: " + FORMAT_PESAN;

                    // bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                }
                else
                {
                    string[] pecah = txt.Split('|');
                    string messageToSend = null;

                    #region swit
                    switch (pecah[0].ToString())
                    {
                        case "WRONG":
                            messageToSend = "Maaf Format Pesan " + pesan.From.FirstName + " Salah Silahkan Ulangi kembali dengan Format: " + FORMAT_PESAN;
                            //bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);


                            break;

                        case "SAVE":
                            messageToSend = "Terima kasih Bapak atau Ibu " + pesan.From.FirstName.ToUpper() + " Laporan anda kami terima ";
                            //bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);

                            break;
                        case "RegisOn":
                            messageToSend = "Maaf " + pesan.From.FirstName + " Anda Sudah Terdaftar di System";
                            // bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);

                            break;
                        case "RegisSuccess":
                            messageToSend = "Proses Registrasi Akun Anda Berhasil Terima kasih, " + pesan.From.FirstName.ToUpper();

                            //bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);

                            break;

                        case "ME":
                            string message = "\xF0\x9F\x98\x81";
                            string encode = RestSharp.Contrib.HttpUtility.UrlEncode(message);
                            messageToSend = " Hai \n1.ID anda=" + idFrom + " ,\n2.Firstname=" + First + " , \n3.lastname=" + last + ", \n4.username=" + usernmae;



                            //bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);

                            break;

                        case "LaporanSudahAda":
                            messageToSend = "Anda Sudah Mengirim 5 Laporan hari ini Bapak/Ibu  ," + pesan.From.FirstName + " Maaf Laporan anda kami tolak ";
                            // bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);

                            break;
                        case "LaporanTersimpan":
                            messageToSend = "Terima kasih Bapak atau Ibu  " + pesan.From.FirstName.ToUpper() + " Laporan anda kami terima ";
                            //bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendMessage(target, messageToSend);

                            pesanClass.ForWardMessage("Laporan dari Bapak atau Ibu " + pesan.From.FirstName.ToUpper() + " \n deskripsi laporan : \n" + pesan.Text);
                            pesanClass.ForWardMessage2("Laporan dari  Bapak atau /Ibu " + pesan.From.FirstName.ToUpper() + " \n deskripsi laporan : \n" + pesan.Text);

                            //pesanClass.ForWardMessage("136331911",);

                            break;

                        case "HELP":
                            messageToSend = "Hai Perkenankan Saya Adalah Assiten kalian saat ini aku Versi 1.0,"
                                            + "\nBiasanya orang orang menyebut ku BOT atau Robot "
                                            + "\nTugas ku adalah untuk membantu mencatat apa yang kalian kerjakan dan mengingatkan kalian\n "
                                            + "\nUntuk Mencatat laporan kalian aku perlu beberapa parameter khusus "
                                            + "\nAgar aku bisa membedakan mana perintah kalian yang perlu aku catat dan tidak perlu aku catat.\n"
                                            + "\n1.Format yang aku catat untuk laporan sbb:"
                                            + "\n\nkepada siapa#laporan#judul laporan#isi laporan\n"
                                            + "\nbaiklah sekarang akan aku contohkan dalam penulisan nya. contohnya dibawah ini\n"
                                            + "\nkepada yth camat Tamansari perihal#laporan#kinerja ku dihari senin 25 April 2015#kegiatan kerja 27 april 2016# hr ini telah melaksanakan agenda surat masuk dan agenda surat keluar Hormat saya Bapak Budi."
                                            + "\n\n\n2.selain mencatat laporan harian kalian kalian bisa menggunakan perintah \n"
                                            + "\n 2.1. #me ( perintah yg anda ketik) untuk mengetahui detail akun anda pribadi "
                                            + "\n 2.2. #register (perintah yang anda ketik) untuk mendaftarkan Data anda di system aku."
                                            +"\n\n3. Oh iya Pesan yang sekarang kalian terima ini hanya bisa dipanggil dengan perintah #help"
                                            + "\n\n Hatur nuhun. ";

                            // bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            bot.SendPhoto(target, new FileStream("perhatian.png", FileMode.Open), "Versi 1.0", "perhatian.png");
                            bot.SendMessage(target, messageToSend);
                            

                            break;
                        default:

                            break;

                    }
                    #endregion

                    Int32 targetID2 = target.Id; //klo minus berarti group 
                    Int32 idFrom2 = pesan.From.Id; //id pengirim chat user 
                    string First2 = pesan.From.FirstName; //first name pengirim chat
                    string last2 = pesan.From.LastName; // last name pengirim chat
                    string username2 = pesan.From.Username; //username pengirim chat
                    Int32 chatid2 = pesan.MessageId; //id pesan
                    string botname2 = bot.Me.FirstName; //nama bot 

                    string tipe2 = null;
                    string namagroup2 = target.ToString();

                    string[] pecahGroup2 = namagroup2.Split('.');

                    tipe2 = pecahGroup2[2].ToString();



                    db.InsertChat(chatid2.ToString(), idFrom2.ToString(), messageToSend, botname2.ToString(), tipe2.ToString(), chatid2.ToString(), chatid2.ToString());


                }


            }



        }//end void




    }




}

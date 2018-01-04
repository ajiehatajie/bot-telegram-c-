using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotSharp;
using TelegramBotSharp.Types;

namespace bot
{


    class Pesan
    {
        Log log = new Log();

        public static TelegramBot bot;
        //bot = new TelegramBot(System.IO.File.ReadAllText("apikey.txt"));



        public const string FORMAT_PESAN = "TUJUAN LAPORAN #laporan# JUDUL Laporan# deskripsi laporan";
        public void HandleMessage(Message pesan)
        {
            DBConnect db = new DBConnect();

            MessageTarget target = ((MessageTarget)pesan.Chat ?? pesan.From);

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
                string botname = "rajacuan"; //nama bot 

                string tipe = null;
                string namagroup = target.ToString();

                string[] pecahGroup = namagroup.Split('.');

                tipe = pecahGroup[2].ToString();

                if (pesan.Text == "/")
                {
                    return;
                }

                string txt = filterPesan(pesan.Text, idFrom, First, last, usernmae, chatid);

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
                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));

                            break;

                        case "SAVE":
                            messageToSend = "Terima kasih , " + pesan.From.FirstName + " Laporan anda kami terima ";
                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));

                            break;
                        case "RegisOn":
                            messageToSend = "Maaf " + pesan.From.FirstName + " Anda Sudah Terdaftar di System";
                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            break;
                        case "RegisSuccess":
                            messageToSend = "Proses Registrasi Akun Anda Berhasil Terima kasih, " + pesan.From.FirstName.ToUpper();

                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            break;

                        case "ME":
                            messageToSend = "ID anda=" + idFrom + " , Firstname=" + First + " , lastname=" + last + ", username=" + usernmae;
                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            break;

                        case "LaporanSudahAda":
                            messageToSend = "Anda Sudah Mengirim Laporan hari ini  , @" + pesan.From.FirstName + " Laporan anda kami tolak ";
                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));
                            break;
                        case "LaporanTersimpan":
                            messageToSend = "Terima kasih , @" + pesan.From.FirstName + " Laporan anda kami terima ";
                            bot.SendMessage(target, messageToSend, false, pesan, new ForceReplyOptions(true));

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

        public string filterPesan(string pesan, Int32 id, string first, string lastname, string username, Int32 chatid)
        {

            DBConnect db = new DBConnect();
            //string[] BacaPesan = pesan.Split('/');//split file dgn param simbol =
            string[] BacaPesan = pesan.Split('#');//split file dgn param simbol =



            int jumlah = BacaPesan.Length;
            string balikan = null;

            if (jumlah > 1)
            {
                string file = BacaPesan[1].ToString();
                string[] isi = file.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

                int jumlah2 = isi.Length;



               

                //if (jumlah == 2)
                if (jumlah == 4)

                {
                    string header = isi[0].ToString();
                    #region filter
                    //if (jumlah2 >2)
                    if (jumlah2 == 1)

                    {
                       // string judul = isi[1].ToString();
                       // string desc = isi[2].ToString();

                        string judul = BacaPesan[2].ToString();
                        string desc = BacaPesan[3].ToString();
                        string headerPesan = BacaPesan[0].ToString();

                        switch (header.ToUpper())
                        {
                            case "LAPORAN":
                                //(string userid, string chatid, string judul, string desc)

                                return balikan = db.InsertLaporan(id.ToString(), chatid.ToString(), judul, desc, headerPesan);
                            default:
                                return null;

                        }


                    }
                    else
                    {
                        string txtChat = isi[0].ToString();

                        //string txtChat = BacaPesan[0].ToString();

                        switch (txtChat.ToUpper())
                        {

                            case "REGISTER":

                                //simpanMembers(id.ToString(), first, lastname,username);

                                return balikan = db.InsertNewMember(id.ToString(), first, lastname, username);
                            case "ME":

                                // simpanMembers(id.ToString(), first, lastname, username);

                                return balikan = "ME" + "|" + id.ToString() + "|" + first + "|" + lastname + "|" + username;

                            case "LAPORAN":
                                return balikan = "WRONG";

                            default:
                                return null;

                        }
                    }

                    #endregion

                }
                #region tambahan
                else if(jumlah==2)
                {
                    string txtChat = BacaPesan[1].ToString();

                    switch (txtChat.ToUpper())
                    {

                        case "REGISTER":

                            //simpanMembers(id.ToString(), first, lastname,username);

                            return balikan = db.InsertNewMember(id.ToString(), first, lastname, username);
                        case "ME":

                            // simpanMembers(id.ToString(), first, lastname, username);

                            return balikan = "ME" + "|" + id.ToString() + "|" + first + "|" + lastname + "|" + username;

                        case "LAPORAN":
                            return balikan = "WRONG";
                        case "HELP":
                            return balikan = "HELP";

                        default:
                            return null;

                    }
                }
                #endregion

                else
                {
                    balikan = null;

                }
            }
            else
            {
                balikan = null;
            }



            return balikan;
        }//end void


        public string Token()
        {

            string conf = Path.GetFullPath(@"apikey.txt");
            string line = null;
            string pathconf = null;
            StreamReader sr = File.OpenText(conf);
            while ((line = sr.ReadLine()) != null)
            {
                pathconf += line;
            } sr.Close();
            pathconf = pathconf.Trim();

            return pathconf;
        }

        public void pm(string token, string chatid, string message)
        {
            string encode = RestSharp.Contrib.HttpUtility.UrlEncode(message);
            string url = "https://api.telegram.org/bot" + token + "/sendMessage?chat_id=" + chatid + "&text=" + message + "&parse_mode=html";

            var request = WebRequest.Create(url);
            request.Timeout = 90000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";


            try
            {
                var resa = request.GetResponse();

                using (var reader = new StreamReader(resa.GetResponseStream()))
                {
                    string content = reader.ReadToEnd();
                }

                resa.Close();
            }
            catch (WebException error)
            {

                Log log = new Log();
                log.CreateLog("error-sendPm", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error Send Pm Bot", error.Message);

            }
        }


        public void ForWardMessage2(string message)
        {
            string chatid = "136331911";
            string token = Token();

            string v = message.Replace("/", " ");
            string v1 = v.Replace("#", " ");

            string encode = RestSharp.Contrib.HttpUtility.UrlEncode(v1);

            string v2 = v.Replace("LAPORAN", " ");


            string url = "https://api.telegram.org/bot" + token + "/sendMessage?chat_id=" + chatid + "&text=" + v1;

            var request = WebRequest.Create(url);
            request.Timeout = 90000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                var resa = request.GetResponse();

                using (var reader = new StreamReader(resa.GetResponseStream()))
                {
                    string content = reader.ReadToEnd();
                }

                resa.Close();
            }
            catch (WebException error)
            {

                Log log = new Log();
                log.CreateLog("error-sendForward", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error send Forward Bot", error.Message);

            }
        }


        public void ForWardMessage(string message)
        {
            string chatid = "206390585";
            string token = Token();

            string v = message.Replace("/", " ");
            string v1 = v.Replace("#", " ");
            string v2 = v.Replace("LAPORAN", " ");


            string url = "https://api.telegram.org/bot" + token + "/sendMessage?chat_id=" + chatid + "&text=" + v1 + "&parse_mode=html";

            var request = WebRequest.Create(url);
            request.Timeout = 90000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                var resa = request.GetResponse();

                using (var reader = new StreamReader(resa.GetResponseStream()))
                {
                    string content = reader.ReadToEnd();
                }

                resa.Close();
            }
            catch (WebException error)
            {

                Log log = new Log();
                log.CreateLog("error-sendForward", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error send Forward Bot", error.Message);

            }
        }


    }//end class
} //end namespace

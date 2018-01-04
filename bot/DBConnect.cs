using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Add MySql Library
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using TelegramBotSharp;
using TelegramBotSharp.Types;

namespace bot
{
    class DBConnect
    {

        private MySqlConnection connection;

        private string server;
        private string database;
        private string uid;
        private string password;

        public const string passwordDB = "c4r3r4";

        //http://zetcode.com/db/mysqlcsharptutorial/
        //http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        public void Initialize()
        {

            //-126502897 id group
            server = "localhost";
            database = "telegram-tamansari";
            uid = "root";
            password = passwordDB;
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
            //connection2 = new MySqlConnection(connectionString);

        }


        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                // connection2.Open();
                Console.WriteLine("connect to server");

                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                    default:

                        Console.WriteLine("Cannot connect to server.  Contact administrator case default");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                //connection2.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }



        public string InsertNewMember(string id, string first, string last, string username)
        {

            string result = null;

            try
            {
                string SQl_user = "SELECT * FROM user where id=@id";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(SQl_user, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    if (dataReader.Read())
                    {
                        //sudah terdaftar
                        result = "RegisOn";
                    }
                    else
                    {
                        dataReader.Dispose();
                        MySqlCommand cmd2 = new MySqlCommand();
                        cmd2.CommandText = "INSERT INTO user(id,first_name,last_name,username,created_at) VALUES(@id,@first,@last,@username,now())";
                        cmd2.Connection = connection;
                        cmd2.Prepare();

                        cmd2.Parameters.AddWithValue("@id", id);
                        cmd2.Parameters.AddWithValue("@first", first);
                        cmd2.Parameters.AddWithValue("@last", last);
                        cmd2.Parameters.AddWithValue("@username", username);

                        cmd2.ExecuteNonQuery();
                        this.CloseConnection();
                        //belum terdafar
                        result = "RegisSuccess";
                    }
                }

            }
            catch (Exception error)
            {
                //error
                result = "Error";

                Console.WriteLine("Error: {0}", error.ToString());

                Log log = new Log();
                log.CreateLog("error-InsertNewMember", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error InsertNewMember", error.Message);
                this.CloseConnection();
            }

            return result;


        }

        public void InsertChat(string messageid, string userid, string text, string botname, string type, string replymessage_id, string updateid)
        {
            Log log = new Log();




            try
            {

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = connection;
                    cmd2.CommandText = "INSERT INTO chat(messageid,userid,text,created_at,botname,type,replymessage_id,updateid) "
                    + "VALUES(@messageid,@userid,@text,now(),@botname,@type,@replymessage_id,@updateid)";

                    cmd2.Prepare();

                    cmd2.Parameters.AddWithValue("@messageid", messageid);
                    cmd2.Parameters.AddWithValue("@userid", userid);
                    cmd2.Parameters.AddWithValue("@text", text);
                    cmd2.Parameters.AddWithValue("@botname", botname);
                    cmd2.Parameters.AddWithValue("@type", type);
                    cmd2.Parameters.AddWithValue("@replymessage_id", replymessage_id);
                    cmd2.Parameters.AddWithValue("@updateid", updateid);
                    cmd2.ExecuteNonQuery();
                }

            }
            catch (MySqlException error)
            {
                Console.WriteLine("Error: {0}", error.ToString());

                log.CreateLog("error-SimpanChat", error.StackTrace, error.Message + " | prosesSimpanChat", null);
                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error SimpanChat", error.Message);

            }
            finally
            {
                this.CloseConnection();
            }
        }

        public string InsertLaporan(string userid, string chatid, string judul, string desc,string header)
        {
            DateTime time = DateTime.Now;
            string format = time.ToString("d");

            Console.WriteLine(time.ToString(format));
            MySqlDataReader rdr = null;

            string result = null;
            try
            {
                if (this.OpenConnection() == true)
                {
                    string stm = "SELECT * FROM laporan where userid='" + userid + "'" + " and DATE_FORMAT(created_at,'%d/%m/%Y')='" + format + "'";

                    MySqlCommand cmd = new MySqlCommand(stm, connection);
                    //cmd.Parameters.AddWithValue("@id", userid);
                    //cmd.Parameters.AddWithValue("@waktu", format);
                    rdr = cmd.ExecuteReader();

                    int jumlah_laporan = rdr.FieldCount;
                    // if (rdr.Read())
                    if (rdr.HasRows)
                    {

                        int res = 0;
                        while (rdr.Read())
                        { ++res; }

                        if (res > 5)
                        {
                            result = "LaporanSudahAda";
                        }
                        else
                        {
                            this.CloseConnection();
                            rdr.Close();
                            //conn.Close();

                            if (this.OpenConnection() == true)
                            {
                                MySqlCommand cmd2 = new MySqlCommand();
                                cmd2.Connection = connection;
                                cmd2.CommandText = "INSERT INTO laporan(userid,chatid,judul,deskripsi,created_at,header) VALUES(@userid,@chatid,@judul,@desc,now(),@header)";
                                cmd2.Prepare();

                                cmd2.Parameters.AddWithValue("@userid", userid);
                                cmd2.Parameters.AddWithValue("@chatid", chatid);
                                cmd2.Parameters.AddWithValue("@judul", judul);
                                cmd2.Parameters.AddWithValue("@desc", desc);
                                cmd2.Parameters.AddWithValue("@header", header);
                                cmd2.ExecuteNonQuery();
                                result = "LaporanTersimpan";

                            }//end if

                        }//end else




                    }
                    else
                    {
                        this.CloseConnection();
                        rdr.Close();
                        //conn.Close();

                        if (this.OpenConnection() == true)
                        {
                            MySqlCommand cmd2 = new MySqlCommand();
                            cmd2.Connection = connection;
                            cmd2.CommandText = "INSERT INTO laporan(userid,chatid,judul,deskripsi,created_at,header) VALUES(@userid,@chatid,@judul,@desc,now(),@header)";
                            cmd2.Prepare();

                            cmd2.Parameters.AddWithValue("@userid", userid);
                            cmd2.Parameters.AddWithValue("@chatid", chatid);
                            cmd2.Parameters.AddWithValue("@judul", judul);
                            cmd2.Parameters.AddWithValue("@desc", desc);
                            cmd2.Parameters.AddWithValue("@header", header);
                            cmd2.ExecuteNonQuery();
                            result = "LaporanTersimpan";

                        }//end if
                    }

                }

                //return true;
            }
            catch (MySqlException error)
            {
                Console.WriteLine("Error: {0}", error.ToString());

                result = "failed";
                Log log = new Log();
                log.CreateLog("error-InsertLaporan", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error InsertLaporan", error.Message);

            }
            finally
            {
                this.CloseConnection();
            }

            return result;

        }


        public void InsertAlert(string userid, string txt)
        {


            try
            {
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = connection;
                    cmd2.CommandText = "INSERT INTO alert(userid,pesan,created_at) VALUES(@useridid,@txt,now())";
                    cmd2.Prepare();

                    cmd2.Parameters.AddWithValue("@useridid", userid);
                    cmd2.Parameters.AddWithValue("@txt", txt);

                    cmd2.ExecuteNonQuery();
                }

            }
            catch (MySqlException error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
                Log log = new Log();
                log.CreateLog("error-InsertAlert", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error InsertAlert", error.Message);

            }
            finally
            {
                this.CloseConnection();

            }
        }



        public void InsertAlert2(string userid, string txt)
        {


            try
            {

                MySqlCommand cmd2 = new MySqlCommand();
                cmd2.Connection = connection;
                cmd2.CommandText = "INSERT INTO alert(userid,pesan,created_at) VALUES(@useridid,@txt,now())";
                cmd2.Prepare();

                cmd2.Parameters.AddWithValue("@useridid", userid);
                cmd2.Parameters.AddWithValue("@txt", txt);

                cmd2.ExecuteNonQuery();


            }
            catch (MySqlException error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
                Log log = new Log();
                log.CreateLog("error-InsertAlert", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error InsertAlert", error.Message);

            }
            finally
            {
                this.CloseConnection();

            }
        }

        public string CekLaporan(string id, string waktu)
        {
            string result = null;
            DateTime time = DateTime.Now;
            string format = time.ToString("d");
            string hari = DateTime.Now.ToString("dd-MM-yyyy");

            try
            {

                if (this.OpenConnection() == true)
                {

                    string stm = " SELECT id,first_name FROM user WHERE id NOT IN "
                          + "( "
                          + "   SELECT userid FROM laporan "
                          + "   WHERE DATE_FORMAT(created_at,'%d/%m/%Y')='"+format+ "'"
                          + " )";
                    MySqlCommand cmd = new MySqlCommand(stm, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //cmd.Parameters.AddWithValue("@waktu", format);
                    dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        string id_telegram = dataReader[0].ToString();
                        string firstname = dataReader[1].ToString();


                        try
                        {
                            string sql = " SELECT * FROM alert WHERE userid=@id and  DATE_FORMAT(created_at,'%d/%m/%Y')=@waktu";
                            MySqlCommand cmd2 = new MySqlCommand(sql, connection);
                            cmd2.Connection = connection;
                            cmd2.Parameters.AddWithValue("@id", id);
                            cmd2.Parameters.AddWithValue("@waktu", format);
                            dataReader = cmd2.ExecuteReader();

                            if (dataReader.HasRows)
                            {

                            }
                            else
                            {
                                string messageToSend = "Bapak/Ibu " + firstname.ToUpper() + " ANDA BELUM MELAPORKAN PEKERJAAN DINAS HARIAN " + hari;

                                //string TOKEN = Token();

                                //InsertAlert(id_telegram, messageToSend);
                                //pm(TOKEN, id_telegram, messageToSend);


                            }

                            this.CloseConnection();

                        }
                        catch (Exception error)
                        {
                            Log log = new Log();
                            log.CreateLog("error-CekLaporan", error.StackTrace, error.Message, null);

                            SendMail mail = new SendMail();
                            mail.SendEmailKeIT("Error CekLaporan", error.Message);

                            this.CloseConnection();
                        }




                    }

                }


            }
            catch (Exception error)
            {
                Log log = new Log();
                log.CreateLog("error-CekLaporan", error.StackTrace, error.Message, null);

                SendMail mail = new SendMail();
                mail.SendEmailKeIT("Error CekLaporan", error.Message);

            }


            return result;
        }


        //        public void Alert()
        //        {

        //            DateTime time = DateTime.Now;
        //            string format = time.ToString("d");
        //            string hari = DateTime.Now.ToString("dd-MM-yyyy");
        //            MySqlDataReader rdr = null;
        //            MySqlDataReader rdr2 = null;
        //            string TOKEN = Token();

        //            try
        //            {
        //                #region select
        //                if (this.OpenConnection() == true)
        //                {

        //                    string stm = " SELECT id,first_name FROM user WHERE id NOT IN "
        //                            + "( "
        //                            + "   SELECT userid FROM laporan "
        //                            + "   WHERE DATE_FORMAT(created_at,'%d/%m/%Y')=@waktu "
        //                            + ")";

        //                    MySqlCommand cmd = new MySqlCommand(stm, connection);

        //                    cmd.Parameters.AddWithValue("@waktu", format);
        //                    rdr = cmd.ExecuteReader();

        //                    while (rdr.Read())
        //                    {
        //                        string id_telegram = rdr[0].ToString();
        //                        string firstname = rdr[1].ToString();

        //                        try
        //                        {
        //                            string cs = @"server=localhost;userid=root;
        //            password=root;database=telegram-tamansari";

        //                            MySqlConnection conn2 = null;
        //                            conn2 = new MySqlConnection(cs);

        //                            string sql = " SELECT * FROM alert WHERE userid=@id and  DATE_FORMAT(created_at,'%d/%m/%Y')=@waktu order by id desc";
        //                            MySqlCommand cmd2 = new MySqlCommand(sql, conn2);
        //                            conn2.Open();
        //                            cmd2.Parameters.AddWithValue("@id", id_telegram);
        //                            cmd2.Parameters.AddWithValue("@waktu", format);
        //                            rdr2 = cmd2.ExecuteReader();

        //                            if (rdr2.HasRows)
        //                            {
        //                                int res = 0;
        //                                while (rdr.Read())
        //                                { ++res; }

        //                                Int32 jumlah_alert = rdr2.FieldCount;//nih salah

        //                            }
        //                            else
        //                            {

        //                                string messageToSend = "Bapak/Ibu " + firstname.ToUpper() + " ANDA BELUM MELAPORKAN PEKERJAAN DINAS HARIAN " + hari;


        //                                InsertAlert2(id_telegram, messageToSend);
        //                                Pesan pesan = new Pesan();
        //                                pesan.pm(TOKEN, id_telegram, messageToSend);
        //                            }
        //                        }
        //                        catch (Exception error)
        //                        {
        //                            Log log = new Log();
        //                            log.CreateLog("error-Alert", error.StackTrace, error.Message, null);

        //                            SendMail mail = new SendMail();
        //                            mail.SendEmailKeIT("Error Alert", error.Message);

        //                            this.CloseConnection();
        //                        }


        //                    }//end while
        //                    rdr2.Dispose();

        //                    rdr.Dispose();

        //                }//end if 
        //                #endregion


        //            }//end try
        //            catch (Exception error)
        //            {
        //                Log log = new Log();
        //                log.CreateLog("error-alert", error.StackTrace, error.Message, null);

        //                SendMail mail = new SendMail();
        //                mail.SendEmailKeIT("Error alert", error.Message);
        //            }
        //        }

        static string Token()
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


        public void Alert()
        {
            string CekDate = DateTime.Now.ToString("yyyy-MM-dd 15:00");
            string hari = DateTime.Now.ToString("dd-MM-yyyy");


            DateTime time = DateTime.Now;
            string format = DateTime.Now.ToString("dd/MM/yyyy");

            string cs = @"server=localhost;userid=root;
            password="+passwordDB+";database=telegram-tamansari";

            MySqlConnection conn = null;
            MySqlConnection conn2 = null;

            MySqlDataReader rdr = null;
           
            conn = new MySqlConnection(cs);
            conn2 = new MySqlConnection(cs);

            conn.Open();
            string stm = " SELECT id,first_name FROM user WHERE id NOT IN "
                         + "( "
                         + "   SELECT userid FROM laporan "
                         + "   WHERE DATE_FORMAT(created_at,'%d/%m/%Y')='" + format + "'"
                         + " ) and  id!='206390585' ORDER BY first_name ASC";

            MySqlCommand cmd = new MySqlCommand(stm, conn);

            //cmd.Parameters.AddWithValue("@waktu", format);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                string id_telegram = rdr[0].ToString();
                string firstname = rdr[1].ToString();


                try
                {
                    #region off
                    //string sql = " SELECT * FROM alert WHERE userid=@id and  DATE_FORMAT(created_at,'%d/%m/%Y')=@waktu";
                    //MySqlCommand cmd2 = new MySqlCommand(sql, conn2);
                    //conn2.Open();
                    //cmd2.Parameters.AddWithValue("@id", id_telegram);
                    //cmd2.Parameters.AddWithValue("@waktu", format);
                    //rdr2 = cmd2.ExecuteReader();

                    //if (rdr2.HasRows)
                    //{
                       
                    //    string messageToSend = "Bapak / Ibu " + firstname.ToUpper() + " Anda Belum Melaporkan Pekerjaan Dinas Harian " + hari +" Mohon Untuk Segera Mengirimkan Laporan tks";

                    //    string TOKEN = Token();

                    //    InsertAlert(id_telegram, messageToSend);

                    //    Pesan pesan = new Pesan();

                    //    pesan.pm(TOKEN, id_telegram, messageToSend);

                    //}
                    //else
                    //{
                    //    string messageToSend = "Bapak / Ibu " + firstname.ToUpper() + " Anda Belum Melaporkan Pekerjaan Dinas Harian " + hari + " Mohon Untuk Segera Mengirimkan Laporan tks";

                    //    string TOKEN = Token();

                    //    InsertAlert(id_telegram, messageToSend);

                    //    Pesan pesan = new Pesan();

                    //    pesan.pm(TOKEN, id_telegram, messageToSend);


                    //}
                    #endregion
                    Log log = new Log();
                    log.CreateLog("alert", id_telegram, firstname+"| "+ stm, null);

                    string messageToSend = "Bapak / Ibu " + firstname.ToUpper() + ", \nAnda Belum Melaporkan Pekerjaan Dinas Harian " + hari + " Mohon Untuk Segera Mengirimkan Laporan \nTerima kasih";

                    string TOKEN = Token();

                    InsertAlert(id_telegram, messageToSend);

                    Pesan pesan = new Pesan();

                    pesan.pm(TOKEN, id_telegram, messageToSend);
                    conn2.Close();

                }
                catch (Exception error)
                {
                    conn.Close();
                    Log log = new Log();
                    log.CreateLog("error-alert", error.StackTrace, error.Message, null);

                    SendMail mail = new SendMail();
                    mail.SendEmailKeIT("Error alert", error.Message);
                }




            }
            //rdr2.Dispose();

            rdr.Dispose();
            conn.Close();
           // conn2.Close();

        }

        public void JustInfo()
        {
            string CekDate = DateTime.Now.ToString("yyyy-MM-dd 15:00");
            string hari = DateTime.Now.ToString("dd-MM-yyyy");


            DateTime time = DateTime.Now;
            string format = time.ToString("d");

            string cs = @"server=localhost;userid=root;
            password=" + passwordDB + ";database=telegram-tamansari";

            MySqlConnection conn = null;
            MySqlConnection conn2 = null;

            MySqlDataReader rdr = null;
            MySqlDataReader rdr2 = null;

            conn = new MySqlConnection(cs);
            conn2 = new MySqlConnection(cs);

            conn.Open();
            string stm = " SELECT id,first_name FROM user ";

            MySqlCommand cmd = new MySqlCommand(stm, conn);

            cmd.Parameters.AddWithValue("@waktu", format);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                string id_telegram = rdr[0].ToString();
                string firstname = rdr[1].ToString();


                try
                {
                    string sql = " SELECT * FROM alert WHERE userid!=@id and  DATE_FORMAT(created_at,'%d/%m/%Y')=@waktu";
                    MySqlCommand cmd2 = new MySqlCommand(sql, conn2);
                    conn2.Open();
                    cmd2.Parameters.AddWithValue("@id", id_telegram);
                    cmd2.Parameters.AddWithValue("@waktu", format);
                    rdr2 = cmd2.ExecuteReader();

                    if (rdr2.HasRows)
                    {

                    }
                    else
                    {
                        string messageToSend = "Bapak / Ibu " + firstname.ToUpper() + " Yth , Sekedar mengingatkan u ";

                        string TOKEN = Token();

                        InsertAlert(id_telegram, messageToSend);

                        Pesan pesan = new Pesan();

                        pesan.pm(TOKEN, id_telegram, messageToSend);


                    }
                    conn2.Close();

                }
                catch (Exception error)
                {
                    conn.Close();
                    Log log = new Log();
                    log.CreateLog("error-alert", error.StackTrace, error.Message, null);

                    SendMail mail = new SendMail();
                    mail.SendEmailKeIT("Error alert", error.Message);
                }




            }
            rdr2.Dispose();

            rdr.Dispose();
            conn.Close();
            conn2.Close();

        }

    } //end class
}//end namespace
